using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    
    private Employee employee;
    private RestaurantTable table;
    public RestaurantTable Table { get { return table; } set { table = value; } }
    private Pages.Queue queuePage;
    public Pages.Queue QueuePage { get { return queuePage; } set { queuePage = value; } }
    private int tableServerId;
    public int TableServerID { get { return (int)table.EmployeeId; } set { table.EmployeeId = value; } }

    private EasyServerRepository easyServerRepository;

    public RequestService(RestaurantTable table, Pages.Queue queuePage, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        this.table = table;
        tableServerId = (int)table.EmployeeId;
        this.queuePage = queuePage;
            
        TableLbl.Text = "Table " + table.TableId;
    }

    private void RequestServer(object sender, System.EventArgs e)
    {
        ToggleServer();
    }

    public void ToggleServer()
    {
        List<RestaurantTable> serverQueue = easyServerRepository.GetServerQueue(TableServerID);

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

        QueuePage.ToggleQueueContentsLbl();
        QueuePage.ClearQueueGrid();
        QueuePage.GenerateGridContents();
    }
}