/*
 * RequestService View
 * 
 * This view is displayed in the frame on the Home page. This view
 * allows customers to request service from the server that is working
 * their table.
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    private RestaurantTable table;                      // The currently logged in table
    private Pages.Queue queuePage;                      // The Queue view associated with this RequestService view
    private EasyServerRepository easyServerRepository;  // "API"
    private Hashtable requestServiceStates;             // States for whether or not customers are awaiting service at each table

    public RestaurantTable Table { get { return table; } set { table = value; } }
    public Pages.Queue QueuePage { get { return queuePage; } set { queuePage = value; } }
    public int? TableServerID { get { return table.EmployeeId; } set { table.EmployeeId = value; } }

    public RequestService(RestaurantTable table, Pages.Queue queuePage, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        // Assign values to global variables
        this.easyServerRepository = easyServerRepository;
        this.table = table;
        this.queuePage = queuePage;
        this.requestServiceStates = requestServiceStates;

        // Determine whether the customer is currently awaiting service
        if ((bool)this.requestServiceStates[table.TableId] == false)
        { 
            // If they are not, indicate that the request service button is not clicked
            ReqServerBtn.Text = "Request Server";
        }
        else 
        {
            // If they are, indicate that the request service button is clicked
            ReqServerBtn.Text = "Cancel Server Request";
        }

        // Display a label at the top of the view which indicates the currently logged in table
        TableLbl.Text = "Table " + table.TableId;
    }

    // Function to toggle service request
    public void ToggleServer(object sender, System.EventArgs e)
    {
        // If the customer is not currently awaiting service, add them to their server's queue
        if ((bool)requestServiceStates[table.TableId] == false)
        {
            easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == (int)table.EmployeeId).FirstOrDefault().Queue.Add(table);

            // Update the request service state to indicate that the customer is now awaiting service
            requestServiceStates[table.TableId] = true;

            // Update the request service button's text to reflect that the customer is now awaiting service
            ReqServerBtn.Text = "Cancel Server Request";
        } 
        else  // If the cusomter is currently awaiting service, "remove" their table from their server's queue
        {
            ServerQueue serverQueue = easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == table.EmployeeId).FirstOrDefault();
            List<RestaurantTable> newQueue = new();

            // Remove did not actively update the server's Queue view, so I recreated the queue and excluded the "removed" table
            for (int i = 0; i < serverQueue.Queue.Count; i++)
            {
                if (serverQueue.Queue[i].TableId != table.TableId)
                {
                    newQueue.Add(serverQueue.Queue[i]);
                }
            }

            // Assign new queue to the server
            easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == table.EmployeeId).FirstOrDefault().Queue = newQueue;

            // Update the request service button's text to indicate the customer is no longer awaiting service
            requestServiceStates[table.TableId] = false;

            // Update the request service button's text to reflect that the customer is no longer awaiting service
            ReqServerBtn.Text = "Request Server";
        }

        // Re-create the corresponding server's Queue view to reflect server toggling
        QueuePage.ToggleQueueContentsLbl();
        QueuePage.ClearQueueGrid();
        QueuePage.GenerateGridContents();
    }
}