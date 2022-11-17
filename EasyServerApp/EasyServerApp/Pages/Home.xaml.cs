using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private EasyServerRepository easyServerRepository;
    private Employee employee;
    
    private HashSet<Pages.Queue> queuePages;
    private HashSet<RequestService> requestServicePages;

    private RestaurantTable table;
    private bool isTables;
 
    private DateTime TOD;

    public Home(Employee employee, Tables tablesPage, RestaurantTable table, HashSet<Pages.Queue> queuePages, HashSet<RequestService> requestServicePages, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        this.queuePages = queuePages;
        this.requestServicePages = requestServicePages;
        this.easyServerRepository = easyServerRepository; 

        TOD = DateTime.Now;
        
        if (table != null)
        {
            this.table = table;
            HomeFrame.Content = requestServicePages.Where(x => x.Table.TableId == table.TableId).FirstOrDefault();
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
        HomeFrame.Content = queuePages.Where(x => x.Employee.EmployeeId == employee.EmployeeId).FirstOrDefault();
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
            HomeFrame.Content = queuePages.Where(x => x.Employee.EmployeeId == table.EmployeeId).FirstOrDefault();
            
            ToggleViewBtn.Text = "Request Service";
        }
        else
        {
            HomeFrame.Content = requestServicePages.Where(x => x.Table.TableId == table.TableId).FirstOrDefault();

            ToggleViewBtn.Text = "View Queue";
        }
    }
}