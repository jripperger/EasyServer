using DocumentFormat.OpenXml.Wordprocessing;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private Employee employee;
    private int id;
    private string firstName;
    private string lastName;
    private string username;
    private string password;
    private string role;
    private EasyServerRepository easyServerRepository;
    private DateTime TOD;

    public Home(Employee employee, EasyServerRepository easyServerRepository)
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
        TOD = DateTime.Now;

        if (role == "Manager")
        {
            ToEmployeesBtn.IsVisible = true;
        }
        else
        {
            ToEmployeesBtn.IsVisible = false;
        }

        DisplayGreeting();
        
        HomeFrame.Content = new Tables(employee, easyServerRepository);
    }

    private void NavigateToTables(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Tables(employee, easyServerRepository);
    }

    private void NavigateToQueue(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Queue(employee, easyServerRepository);
    }

    private void NavigateToEmployees(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Employees(easyServerRepository);
    }

    private async void SignOut(object sender, System.EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private void DisplayGreeting()
    {
        if (TOD.Hour < 12)
        {
            WelcomeLbl.Text = "Good Morning " + firstName;
        }
        else if (TOD.Hour >= 12 && TOD.Hour < 18)
        {
            WelcomeLbl.Text = "Good Afternoon " + firstName;
        }
        else
        {
            WelcomeLbl.Text = "Good Evening " + firstName;
        }
    }
}