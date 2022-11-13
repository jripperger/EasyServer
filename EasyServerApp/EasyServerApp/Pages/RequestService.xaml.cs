using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    EasyServerRepository easyServerRepository;
    private RestaurantTable table;
    private Employee employee;
    private Queue queuePage;

    public int TableID { get { return table.TableId; } }
    public RestaurantTable Table { get { return table; } }

    public Queue QueuePage { get { return queuePage; } set { queuePage = value; } }

    public RequestService(RestaurantTable table, Queue queuePage, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        this.table = table;
        this.queuePage = queuePage;

        if (table.EmployeeId.HasValue)
        {
            employee = easyServerRepository.GetEmployeeById((int)table.EmployeeId);
        }
            
        TableLbl.Text = "Table " + table.TableId.ToString();
    }

    private void RequestServer(object sender, System.EventArgs e)
    {
        ToggleServer();
    }

    public void ToggleServer()
    {
        List<RestaurantTable> serverQueue = easyServerRepository.GetServerQueue(employee.EmployeeId);

        if (ReqServerBtn.Text == "Request Server")
        {
            serverQueue.Add(table);
            ReqServerBtn.Text = "Cancel Server Request";
        } 
        else
        {
            serverQueue.Remove(table);
            ReqServerBtn.Text = "Request Server";
        }

        QueuePage.GenerateGridContents();
    }
}