using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private Employee employee;
    private int employeeID;
    private string firstName;
    private string lastName;
    private string username;
    private string password;
    private string role;
    private int tableID;
    private bool isTables;
    private EasyServerRepository easyServerRepository;
    private DateTime TOD;

    public Home(Employee employee, Tables tables, RequestService requestService, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        this.employee = employee;
        this.easyServerRepository = easyServerRepository;
        employeeID = employee.EmployeeId;
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
        
        if (requestService != null)
        {
            HomeFrame.Content = requestService;
            DisplayCustomerGreeting();

            ViewQueueBtn.IsVisible = true;
            ToTablesBtn.IsVisible = false;
            ToQueueBtn.IsVisible = false;
            ToEmployeesBtn.IsVisible = false;

            isTables = false;
        }
        else
        {
            HomeFrame.Content = tables;
            DisplayEmployeeGreeting();

            ViewQueueBtn.IsVisible = false;
            ToTablesBtn.IsVisible = true;
            ToQueueBtn.IsVisible = true;
            ToEmployeesBtn.IsVisible = true;

            isTables = true;
        }
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
        if (isTables)
        {
            await Navigation.PopToRootAsync();
        } 
        else
        {
            string password = await DisplayPromptAsync("Table Sign Out", "Enter manager password:");
            Employee employee = easyServerRepository.GetEmployeeByPassword(password);

            if (employee.Role.Trim() == "Manager")
            {
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Logout Failed", "Incorrect password", "OK");
            }
        }
        
    }

    private void DisplayEmployeeGreeting()
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

    private void DisplayCustomerGreeting()
    {
        if (TOD.Hour < 12)
        {
            WelcomeLbl.Text = "Good Morning";
        }
        else if (TOD.Hour >= 12 && TOD.Hour < 18)
        {
            WelcomeLbl.Text = "Good Afternoon";
        }
        else
        {
            WelcomeLbl.Text = "Good Evening";
        }
    }

    private void ViewQueue(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Queue(employee, easyServerRepository);
    }
}