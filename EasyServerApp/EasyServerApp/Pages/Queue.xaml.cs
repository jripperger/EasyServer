/*
 * Queue View
 * 
 * This view is displayed in the frame on the Home page. This view
 * allows servers to view their server queue: a list of all the tables 
 * that they are serving and are currently awaiting service.
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Queue : ContentView
{
    private Employee employee;               // The currently logged in employee
    private List<ServerQueue> serverQueues;  // A list of the servers' server queues
    private ServerQueue serverQueue;         // The currently logged in employee's server queue
    private List<Button> serviceButtons;     // A list of the buttons for tables awaiting service
    private Hashtable requestServiceStates;  // States for the request service button of each table, indicating clicked (T) or not clicked (F)

    public List<Button> ServiceButtons { get { return serviceButtons; } set { serviceButtons = value; } }

    public Employee Employee { get { return employee; } }

    public Queue(Employee employee, List<ServerQueue> serverQueues, Hashtable requestServiceStates)
	{
		InitializeComponent();

        // Assign values to global variables
        this.employee = employee;
        this.serverQueues = serverQueues;
        this.requestServiceStates = requestServiceStates;
        serverQueue = this.serverQueues.Where(x => x.Employee.EmployeeId == this.employee.EmployeeId).FirstOrDefault();
        serviceButtons = new List<Button>();

        // If there are no tables awaiting service, display the label that indicates an empty queue
        ToggleQueueContentsLbl();

        // Display the employee's name above their queue
        QueueLbl.Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + "'s Queue";

        // Create the grid display
        GenerateGridContents();
    }

    /* 
     * Function to create the grid display for all tables in the server's queue
     * 
     * Each row location has a button with text that displays a table's ID and 
     * corresponds to a table that's currently awaiting service
     * 
     */
    public void GenerateGridContents()
    {
        // Creates the appropriate number of rows
        GenerateGridLayout();

        // For each table in the server's queue, create the aforementioned row/column contents
        for (int i = 0; i < serverQueue.Queue.Count; i++)
        {
            // Create the button with the appropriate table ID text
            var button = new Button
            {
                Text = "Table " + serverQueue.Queue[i].TableId.ToString(),
                WidthRequest = 150
            };

            // If the button is not first in the queue, it is disabled
            if (i == 0)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }

            // Assign the button's clicked event the serve table function
            button.Clicked += new EventHandler(ServeTable);

            // Add the button to the service buttons list for later reference
            serviceButtons.Add(button);

            // Add the button to the grid
            QueueGrid.Add(button);
            QueueGrid.SetRow(button, i);
            QueueGrid.SetColumn(button, 0);
        }
    }

    
    // Function to generate the appropriate number of rows   
    private void GenerateGridLayout()
    {
        // Calculate the number of rows needed: the number of tables in the server's queue
        int rows = serverQueues.Where(x => x.Employee.EmployeeId == employee.EmployeeId).FirstOrDefault().Queue.Count;

        // Add rows to the grid
        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            QueueGrid.AddRowDefinition(rowDefinition);
        }
    }
 
    // Function to serve a table, thereby removing it from the server's queue    
    private void ServeTable(object sender, System.EventArgs e)
    {
        // Set the table's request service state to not clicked
        requestServiceStates[serverQueue.Queue.FirstOrDefault().TableId] = false;

        // Remove the table from the server's queue
        serverQueues.Where(x => x.Employee.EmployeeId == employee.EmployeeId).FirstOrDefault().Queue.Remove(serverQueue.Queue.FirstOrDefault());

        // Re-create the view to reflect the removed table
        ToggleQueueContentsLbl();
        ClearQueueGrid();
        GenerateGridContents();
    }

    // Function to check whether there are tables awaiting service; if not, a label is displayed to indicate this
    public void ToggleQueueContentsLbl()
    {
        if (serverQueues.Where(x => x.Employee.EmployeeId == employee.EmployeeId).FirstOrDefault().Queue.Count == 0)
        {
            QueueContentsLbl.IsVisible = true;
        }
        else
        {
            QueueContentsLbl.IsVisible = false;
        }
    }

    // Function to clear out the grid
    public void ClearQueueGrid()
    {
        QueueGrid.RowDefinitions.Clear();
        QueueGrid.ColumnDefinitions.Clear();
        QueueGrid.Clear();
    }
}