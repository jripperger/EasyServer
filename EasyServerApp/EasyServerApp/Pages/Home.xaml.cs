using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private EasyServerRepository easyServerRepository;
    private Employee employee;
    
    private Hashtable queuePages;
    public Hashtable QueuePages { get { return queuePages; } set { queuePages = value; } }
    private Hashtable requestServicePages;
    public Hashtable RequestServicePages { get { return requestServicePages; } set { requestServicePages = value; } }

    private RestaurantTable table;
    private bool isTables;

    
    private DateTime TOD;

    public Home(Employee employee, Tables tablesPage, RestaurantTable table, Hashtable queuePages, Hashtable requestServicePages, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        this.queuePages = queuePages;
        this.requestServicePages = requestServicePages;
        this.easyServerRepository = easyServerRepository; 

        TOD = DateTime.Now;
        
        if (table != null)
        {
            this.table = table;
            HomeFrame.Content = (RequestService)requestServicePages[table.TableId];
            DisplayCustomerGreeting();
         
            ToggleViewBtn.IsVisible = true;
            ToTablesBtn.IsVisible = false;
            ToQueueBtn.IsVisible = false;
            ToEmployeesBtn.IsVisible = false;

            isTables = false;
        }
        else
        {
            this.employee = employee;

            if (employee.Role == "Manager")
            {
                ToEmployeesBtn.IsVisible = true;
            }
            else
            {
                ToEmployeesBtn.IsVisible = false;
            }

            HomeFrame.Content = tablesPage;
            DisplayEmployeeGreeting();

            ToggleViewBtn.IsVisible = false;
            ToTablesBtn.IsVisible = true;
            ToQueueBtn.IsVisible = true;

            isTables = true;
        }
    }

    private void NavigateToTables(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Tables(employee, queuePages, requestServicePages, easyServerRepository);
    }

    private void NavigateToQueue(object sender, System.EventArgs e)
    {
        HomeFrame.Content = (Pages.Queue)queuePages[employee.EmployeeId];
    }

    private void NavigateToEmployees(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Employees(easyServerRepository);
    }

    private async void SignOut(object sender, System.EventArgs e)
    {
        if (isTables)
        {
            Navigation.PopToRootAsync().RunSynchronously();
        } 
        else
        {
            string password = await DisplayPromptAsync("Table Sign Out", "Enter administrator password:");
            Employee employee = easyServerRepository.GetEmployeeByPassword(password);

            if (employee != null)
            {
                if (employee.Role.Trim() == "Manager")
                {
                    Navigation.PopToRootAsync().RunSynchronously();
                }
                else
                {
                    DisplayAlert("Logout Failed", "Invalid password", "OK").RunSynchronously();
                }
            } 
            else
            {
                DisplayAlert("Logout Failed", "Invalid password", "OK").RunSynchronously();
            }        
        }
    }

    private void DisplayEmployeeGreeting()
    {
        if (TOD.Hour < 12)
        {
            WelcomeLbl.Text = "Good Morning " + employee.FirstName;
        }
        else if (TOD.Hour >= 12 && TOD.Hour < 18)
        {
            WelcomeLbl.Text = "Good Afternoon " + employee.FirstName;
        }
        else
        {
            WelcomeLbl.Text = "Good Evening " + employee.FirstName;
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

    private void ToggleView(object sender, System.EventArgs e)
    {
        if (ToggleViewBtn.Text == "View Queue")
        {
            HomeFrame.Content = (Pages.Queue)queuePages[table.EmployeeId];
            
            ToggleViewBtn.Text = "Request Service";
        }
        else
        {
            HomeFrame.Content = (RequestService)requestServicePages[table.TableId];

            ToggleViewBtn.Text = "View Queue";
        }
    }
}