using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class Queue : ContentView
{
    private Employee employee;
    private int id;
    private string firstName;
    private string lastName;
    private string username;
    private string password;
    private string role;
    private EasyServerRepository easyServerRepository;

    public Queue(Employee employee, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.employee = employee;
        this.easyServerRepository = easyServerRepository;
        id = employee.EmployeeId;
        firstName = employee.FirstName;
        lastName = employee.LastName;
        username = employee.Username;
        password = employee.Password;
        role = employee.Role;
    }
}