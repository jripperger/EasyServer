using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private int id;
    private string firstName;
    private string lastName;
    private string username;
    private string password;
    private string role;
    private DateTime TOD;

    public Home(Employee employee, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        id = employee.EmployeeId;
        firstName = employee.FirstName;
        lastName = employee.LastName;
        username = employee.Username;
        password = employee.Password;
        role = employee.Role;
        TOD = DateTime.Now;

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
        
        HomeFrame.Content = new Tables(employee, easyServerRepository);
    }
}