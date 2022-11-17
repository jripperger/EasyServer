using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    private RestaurantTable table;
    public RestaurantTable Table { get { return table; } set { table = value; } }
    private Pages.Queue queuePage;
    public Pages.Queue QueuePage { get { return queuePage; } set { queuePage = value; } }
    public int? TableServerID { get { return table.EmployeeId; } set { table.EmployeeId = value; } }

    private EasyServerRepository easyServerRepository;

    private Hashtable requestServiceStates;

    public RequestService(RestaurantTable table, Pages.Queue queuePage, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        this.table = table;
        this.queuePage = queuePage;
        this.requestServiceStates = requestServiceStates;

        if ((bool)this.requestServiceStates[table.TableId] == false)
        {
            ReqServerBtn.Text = "Request Server";
        }
        else
        {
            ReqServerBtn.Text = "Cancel Server Request";
        }
            
        TableLbl.Text = "Table " + table.TableId;
    }

    private void RequestServer(object sender, System.EventArgs e)
    {
        ToggleServer();
    }

    public void ToggleServer()
    {
        if ((bool)requestServiceStates[table.TableId] == false)
        {
            easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == (int)table.EmployeeId).FirstOrDefault().Queue.Add(table);
            requestServiceStates[table.TableId] = true;
            ReqServerBtn.Text = "Cancel Server Request";
        } 
        else
        {
            ServerQueue serverQueue = easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == table.EmployeeId).FirstOrDefault();
            List<RestaurantTable> newQueue = new();

            for (int i = 0; i < serverQueue.Queue.Count; i++)
            {
                if (serverQueue.Queue[i].TableId != table.TableId)
                {
                    newQueue.Add(serverQueue.Queue[i]);
                }
            }

            easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == table.EmployeeId).FirstOrDefault().Queue = newQueue;

            requestServiceStates[table.TableId] = false;
            ReqServerBtn.Text = "Request Server";
        }
            
        QueuePage.ToggleQueueContentsLbl();
        QueuePage.ClearQueueGrid();
        QueuePage.GenerateGridContents();
    }
}