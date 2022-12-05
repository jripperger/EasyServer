/*
 * Queue View
 * 
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Queue : ContentView
{
    private Employee employee;
    private List<ServerQueue> serverQueues;
    private ServerQueue serverQueue;
    private List<Button> serviceButtons;
    private Hashtable requestServiceStates;

    public List<Button> ServiceButtons { get { return serviceButtons; } set { serviceButtons = value; } }

    public Employee Employee { get { return employee; } }

    public Queue(Employee employee, List<ServerQueue> serverQueues, Hashtable requestServiceStates)
	{
		InitializeComponent();

        this.employee = employee;
        this.serverQueues = serverQueues;
        this.requestServiceStates = requestServiceStates;

        serverQueue = this.serverQueues.Where(x => x.Employee.EmployeeId == this.employee.EmployeeId).FirstOrDefault();

        serviceButtons = new List<Button>();

        if (serverQueue.Queue.Count == 0)
        {
            QueueContentsLbl.IsVisible = true;
        }
        else
        {
            QueueContentsLbl.IsVisible = false;
        }
  
        QueueLbl.Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + "'s Queue";

        GenerateGridContents();
    }
    
    public void GenerateGridContents()
    {
        GenerateGridLayout();

        ServerQueue serverQueue = serverQueues.Where(x => x.Employee.EmployeeId == employee.EmployeeId).FirstOrDefault();

        for (int i = 0; i < serverQueue.Queue.Count; i++)
        {
            var button = new Button
            {
                Text = "Table " + serverQueue.Queue[i].TableId.ToString(),
                WidthRequest = 150
            };

            if (i == 0)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }

            button.Clicked += new EventHandler(ServeTable);

            serviceButtons.Add(button);
            QueueGrid.Add(button);
            QueueGrid.SetRow(button, i);
            QueueGrid.SetColumn(button, 0);
        }
    }

    private void GenerateGridLayout()
    {
        int rows = serverQueues.Where(x => x.Employee.EmployeeId == employee.EmployeeId).FirstOrDefault().Queue.Count;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            QueueGrid.AddRowDefinition(rowDefinition);
        }
    }

    private void ServeTable(object sender, System.EventArgs e)
    {
        requestServiceStates[serverQueue.Queue.FirstOrDefault().TableId] = false;
        serverQueues.Where(x => x.Employee.EmployeeId == serverQueue.Queue.FirstOrDefault().EmployeeId).FirstOrDefault().Queue.Remove(serverQueue.Queue.FirstOrDefault());

        ToggleQueueContentsLbl();
        ClearQueueGrid();
        GenerateGridContents();
    }

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

    public void ClearQueueGrid()
    {
        QueueGrid.RowDefinitions.Clear();
        QueueGrid.ColumnDefinitions.Clear();
        QueueGrid.Clear();
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        Visibility isVisible = (Visibility)value;
        return isVisible;
    }
}