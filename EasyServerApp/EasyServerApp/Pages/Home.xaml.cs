using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private Employee employee;

    private RequestService requestServicePage;
    private Queue queuePage;

    private RestaurantTable table;
    private bool isTables;

    private EasyServerRepository easyServerRepository;
    private DateTime TOD;

    public Home(Employee employee, Tables tablesPage, RequestService requestServicePage, Queue queuePage, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        this.employee = employee;
        this.queuePage= queuePage;
        this.requestServicePage = requestServicePage;
        this.easyServerRepository = easyServerRepository; 

        TOD = DateTime.Now;

        if (employee.Role == "Manager")
        {
            ToEmployeesBtn.IsVisible = true;
        }
        
        if (requestServicePage != null)
        {
            HomeFrame.Content = requestServicePage;
            DisplayCustomerGreeting();

            table = requestServicePage.Table;

            ToggleViewBtn.IsVisible = true;
            ToTablesBtn.IsVisible = false;
            ToQueueBtn.IsVisible = false;
            ToEmployeesBtn.IsVisible = false;

            isTables = false;
        }
        else
        {
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
        HomeFrame.Content = new Tables(employee, easyServerRepository);
    }

    private void NavigateToQueue(object sender, System.EventArgs e)
    {
        HomeFrame.Content = queuePage;
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

            if (employee != null)
            {
                if (employee.Role.Trim() == "Manager")
                {
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await DisplayAlert("Logout Failed", "Invalid password", "OK");
                }
            } 
            else
            {
                await DisplayAlert("Logout Failed", "Invalid password", "OK");
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
            HomeFrame.Content = queuePage;
            
            ToggleViewBtn.Text = "Request Service";
        }
        else
        {
            HomeFrame.Content = requestServicePage;

            ToggleViewBtn.Text = "View Queue";
        }
    }
}