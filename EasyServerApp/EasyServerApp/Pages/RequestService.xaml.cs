using DocumentFormat.OpenXml.Drawing;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    EasyServerRepository easyServerRepository;
    private RestaurantTable table;
    private Employee employee;
    private int tableID;

    public int TableID { get { return tableID; } }
    public RestaurantTable Table { get { return table; } }

    public RequestService(RestaurantTable table, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        this.table = table;
        tableID = table.TableId;

        employee = easyServerRepository.GetEmployeeById(table.TableId);

        TableLbl.Text = "Table " + table.TableId.ToString();
    }

    private void RequestServer(object sender, System.EventArgs e)
    {
        if (ReqServerBtn.Text == "Request Server")
        {
            employee.Queue.Add(table);
            ReqServerBtn.Text = "Cancel Server Request";
        } 
        else
        {
            employee.Queue.Remove(table);
            ReqServerBtn.Text = "Request Server";
        } 
    }
}