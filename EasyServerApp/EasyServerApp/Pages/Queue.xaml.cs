using EasyServerApp.EasyServerDB;
using System.Security.AccessControl;

namespace EasyServerApp.Pages;

public partial class Queue : ContentView
{
    private Employee employee;
    private EasyServerRepository easyServerRepository;

    public Queue(Employee employee, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.employee = employee;
        this.easyServerRepository = easyServerRepository;
  
        QueueLbl.Text = employee.FirstName + " " + employee.LastName + "'s Queue";

        GenerateGridContents();
    }

    private void GenerateGridContents()
    {
        GenerateGridLayout();

        for (int i = 0; i < employee.Queue.Count; i++)
        {
            var button = new Button
            {
                Text = employee.Queue[i].TableId.ToString()
            };

            QueueGrid.Add(button);
            QueueGrid.SetRow(button, i);
            QueueGrid.SetColumn(button, 0);
        }
    }

    private void GenerateGridLayout()
    {
        int rows = employee.Queue.Count;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            QueueGrid.AddRowDefinition(rowDefinition);
        }
    }
}