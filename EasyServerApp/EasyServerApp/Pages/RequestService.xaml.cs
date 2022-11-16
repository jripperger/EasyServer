using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    private RestaurantTable table;
    public RestaurantTable Table { get { return table; } set { table = value; } }
    private Pages.Queue queuePage;
    public Pages.Queue QueuePage { get { return queuePage; } set { queuePage = value; } }
    public int? TableServerID { get { return table.EmployeeId; } set { table.EmployeeId = value; } }

    private EasyServerRepository easyServerRepository;

    public RequestService(RestaurantTable table, Pages.Queue queuePage, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        this.table = table;
        this.queuePage = queuePage;
            
        TableLbl.Text = "Table " + table.TableId;
    }

    private void RequestServer(object sender, System.EventArgs e)
    {
        ToggleServer();
    }

    public void ToggleServer()
    {
        List<RestaurantTable> serverQueue = easyServerRepository.GetServerQueue((int)TableServerID);

        if (ReqServerBtn.Text == "Request Server")
        {
            serverQueue.Add(Table);
            ReqServerBtn.Text = "Cancel Server Request";
        } 
        else
        {
            serverQueue.Remove(Table);
            ReqServerBtn.Text = "Request Server";
        }

        QueuePage.ToggleQueueContentsLbl();
        QueuePage.ClearQueueGrid();
        QueuePage.GenerateGridContents();
    }
}