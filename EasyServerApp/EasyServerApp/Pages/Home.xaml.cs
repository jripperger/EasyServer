using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private EasyServerRepository easyServerRepository;
    private Employee employee;
    
    private Hashtable queuePages;
    private Hashtable requestServicePages;

    private RestaurantTable table;
    private bool isTables;
 
    private DateTime TOD;

    public Home(Employee employee, Tables tablesPage, RestaurantTable table, Hashtable queuePages, Hashtable requestServicePages, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        this.queuePages = queuePages;
        this.requestServicePages = requestServicePages;
        this.easyServerRepository = easyServerRepository; 
        this.employee = employee;
        this.table = table;

        TOD = DateTime.Now;
        
        if (this.table != null)
        {
            HomeFrame.Content = (RequestService)requestServicePages[this.table.TableId];
            DisplayCustomerGreeting();
         
            ToggleViewBtn.IsVisible = true;
            ToTablesBtn.IsVisible = false;
            ToQueueBtn.IsVisible = false;
            ToEmployeesBtn.IsVisible = false;

            isTables = false;
        }
        else
        {
            if (this.employee.Role == "Manager")
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
        HomeFrame.Content = new Employees(employee, queuePages, requestServicePages, easyServerRepository);
    }

    private async void SignOut(object sender, System.EventArgs e)
    {
        if (isTables)
        {
            Task task = new(() => { Navigation.PopToRootAsync(); });
            task.RunSynchronously();
        } 
        else
        {
            string password = await DisplayPromptAsync("Table Sign Out", "Enter administrator password:"); 

            if (password != null)
            {
                Employee employee = easyServerRepository.GetEmployeeByPassword(password);

                if (employee != null)
                {
                    if (employee.Role.Trim() == "Manager")
                    {
                        Task task = new(() => { Navigation.PopToRootAsync(); });
                        task.RunSynchronously();
                    }
                    else
                    {
                        Task task = new(() => { DisplayAlert("Logout Failed", "Invalid password", "OK"); });
                        task.RunSynchronously();
                    }
                } 
                else
                {
                    Task task = new(() => { DisplayAlert("Logout Failed", "Invalid password", "OK"); });
                    task.RunSynchronously();
                }        
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
            int id = (int)table.EmployeeId;
            HomeFrame.Content = (Pages.Queue)queuePages[id];
            
            ToggleViewBtn.Text = "Request Service";
        }
        else
        {
            int id = table.TableId;
            HomeFrame.Content = (RequestService)requestServicePages[id];

            ToggleViewBtn.Text = "View Queue";
        }
    }
}