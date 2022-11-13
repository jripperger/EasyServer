using EasyServerApp.EasyServerDB;
using System.Collections;
using System.Security.AccessControl;

namespace EasyServerApp.Pages;

public partial class Queue : ContentView
{
    private Employee employee;
    private EasyServerRepository easyServerRepository;
    private List<RestaurantTable> serverQueue;
    private List<Button> serviceButtons;
    private Hashtable requestServicePages;

    public Queue(Employee employee, List<RestaurantTable> serverQueue, Hashtable requestServicePages, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.employee = employee;
        this.serverQueue = serverQueue;
        this.requestServicePages = requestServicePages;
        this.easyServerRepository = easyServerRepository;
        serviceButtons= new List<Button>();
  
        QueueLbl.Text = employee.FirstName + " " + employee.LastName + "'s Queue";

        GenerateGridContents();
    }
    
    public void GenerateGridContents()
    {
        GenerateGridLayout();

        for (int i = 0; i < serverQueue.Count; i++)
        {
            var button = new Button
            {
                Text = "Table " + serverQueue[i].TableId.ToString()
            };

            if (i == 0)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }

            button.Clicked += new EventHandler(serveTable);

            serviceButtons.Add(button);
            QueueGrid.Add(button);
            QueueGrid.SetRow(button, i);
            QueueGrid.SetColumn(button, 0);
        }
    }

    private void GenerateGridLayout()
    {
        int rows = serverQueue.Count;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            QueueGrid.AddRowDefinition(rowDefinition);
        }
    }

    private void serveTable(object sender, System.EventArgs e)
    {
        ((RequestService)requestServicePages[serverQueue.FirstOrDefault().TableId]).ToggleServer();

        QueueGrid.Clear();
        GenerateGridContents();
    }
}