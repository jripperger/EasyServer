using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Queue : ContentView
{
    private Employee employee;
    private ServerQueue serverQueue;
    private List<Button> serviceButtons;
    private HashSet<RequestService> requestServicePages;

    public Employee Employee { get { return employee; } }

    public Queue(Employee employee, ServerQueue serverQueue, HashSet<RequestService> requestServicePages)
	{
		InitializeComponent();

        this.employee = employee;
        this.serverQueue = serverQueue;
        this.requestServicePages = requestServicePages;

        serviceButtons= new List<Button>();
  
        QueueLbl.Text = employee.FirstName + " " + employee.LastName + "'s Queue";

        GenerateGridContents();
    }
    
    public void GenerateGridContents()
    {
        GenerateGridLayout();

        for (int i = 0; i < serverQueue.Queue.Count; i++)
        {
            var button = new Button
            {
                Text = "Table " + serverQueue.Queue[i].TableId.ToString()
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
        int rows = serverQueue.Queue.Count;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            QueueGrid.AddRowDefinition(rowDefinition);
        }
    }

    private void serveTable(object sender, System.EventArgs e)
    {
        requestServicePages.Where(x => x.Table.TableId == serverQueue.Queue.FirstOrDefault().TableId).FirstOrDefault().ToggleServer();

        ToggleQueueContentsLbl();
        ClearQueueGrid();
        GenerateGridContents();
    }

    public void ToggleQueueContentsLbl()
    {
        if (serverQueue.Queue.Count == 0)
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
        QueueGrid.Clear();
    }
}