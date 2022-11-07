using DocumentFormat.OpenXml.Drawing;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class RequestService : ContentView
{
    EasyServerRepository easyServerRepository;
    private List<Employee> employees;
    private int tableID;
    private int? employeeID;

    public RequestService(RestaurantTable table, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        employees = easyServerRepository.GetEmployeeList();
        tableID = table.TableId;
        employeeID = table.EmployeeId;

        //GeneratePicker();
    }

    /*private void GeneratePicker()
    {
        for (int i = 0; i < employees.Count; i++)
        {
            var picker = new Picker();
            picker.Title = "Choose an employee's queue to view: ";

            picker.Items.Add(employees[i].FirstName.Trim() + " " + employees[i].LastName.Trim() + " [" + employees[i].EmployeeId + "]");

            RequestServiceGrid.Add(picker);
            RequestServiceGrid.SetRow(picker, 2);
        }
    }*/
}