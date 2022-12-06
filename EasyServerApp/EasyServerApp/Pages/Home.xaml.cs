/*
 * Home Page
 * 
 * This page is displayed immediately after a login. It can contain 
 * 1 of 4 views at a given moment: Employees, Queue, RequestService, 
 * or Tables. A navigation bar at the top of the page allows for easy
 * navigation between these views. A "view queue / request service" button
 * is displayed in this navigation bar if the RequestService view is being
 * displayed. This button allows customers to view their table's server's 
 * queue.
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Home : ContentPage
{
    private EasyServerRepository easyServerRepository;  // "API"
    private Employee employee;                          // The currently logged in employee
    private Hashtable requestServiceStates;             // States for whether or not customers are awaiting service at each table
    private RestaurantTable table;                      // The currently loggin in table - if a table was logged in to
    private bool isTables;                              // Boolean to determine which page is being displayed after login
    private DateTime TOD;                               // Current TOD

    public Home(Employee employee, RestaurantTable table, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        // Assign values to global variables
        this.requestServiceStates = requestServiceStates;
        this.easyServerRepository = easyServerRepository; 
        TOD = DateTime.Now;

        // If a table was logged in to
        if (table != null)
        {
            this.table = table;

            // Retrieve the server of the table
            Employee tableServer = easyServerRepository.GetEmployeeById((int)table.EmployeeId);

            // Create a queue page that can be viewed with "view queue" button
            Pages.Queue queuePage = new(tableServer, easyServerRepository.ServerQueues, this.requestServiceStates);

            // Display the RequestService view for the logged in table in the home frame
            HomeFrame.Content = new RequestService(table, queuePage, this.requestServiceStates, this.easyServerRepository);

            DisplayCustomerGreeting();

            // "View queue / request service" button is visible 
            ToggleViewBtn.IsVisible = true;

            // All other navigation buttons are not available
            ToTablesBtn.IsVisible = false;
            ToQueueBtn.IsVisible = false;
            ToEmployeesBtn.IsVisible = false;

            // RequestService view was displayed on login
            isTables = false;
        }
        else
        {
            this.employee = employee;

            // If the logged in employee is a manager, the button to navigate to the Employees view is available
            if (employee.Role == "Manager")
            {
                ToEmployeesBtn.IsVisible = true;
            }
            else
            {
                ToEmployeesBtn.IsVisible = false;
            }

            // Display the Tables view in the home frame
            HomeFrame.Content = new Tables(this.employee, this.requestServiceStates, this.easyServerRepository);

            DisplayEmployeeGreeting();

            // "View queue/ request service" button is not visible since a table wasn't logged in to
            ToggleViewBtn.IsVisible = false;

            // Other navigation buttons are available
            ToTablesBtn.IsVisible = true;
            ToQueueBtn.IsVisible = true;

            // Tables view was displayed on login
            isTables = true;
        }
    }

    // Function to navigate to Tables view
    private void NavigateToTables(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Tables(employee, requestServiceStates, easyServerRepository);
    }

    // Function to navigate to Queue view
    private void NavigateToQueue(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Pages.Queue(employee, easyServerRepository.ServerQueues, requestServiceStates);
    }

    // Function to navigate to Employees view
    private void NavigateToEmployees(object sender, System.EventArgs e)
    {
        HomeFrame.Content = new Employees(employee, requestServiceStates, easyServerRepository);
    }

    // Function to sign out of the application
    private async void SignOut(object sender, System.EventArgs e)
    {
        // If user is not logged in to a table, simply sign out of the user's account and return to Login view
        if (isTables)
        {
            Task task = new(() => { Navigation.PopToRootAsync(); });
            task.RunSynchronously();
        }
        else
        {
            // Otherwise, verify that the user signing out is an administrator / manager to prevent customers
            // from signing out of their table
            string password = await DisplayPromptAsync("Table Sign Out", "Enter administrator password:"); 

            if (password != null)  // If a password was provided, retrieve the employee based on the password
            {
                Employee employee = easyServerRepository.GetEmployeeByPassword(password);

                if (employee != null)  // If an employee was found, verify that the employee is an administrator / manager
                {
                    if (employee.Role.Trim() == "Manager")
                    {
                        Task task = new(() => { Navigation.PopToRootAsync(); });
                        task.RunSynchronously();
                    }
                    else  // If they are a server, do not log out
                    {
                        Task task = new(() => { DisplayAlert("Logout Failed", "Invalid password", "OK"); });
                        task.RunSynchronously();
                    }
                } 
                else  // If no employee was found, do not log out
                {
                    Task task = new(() => { DisplayAlert("Logout Failed", "Invalid password", "OK"); });
                    task.RunSynchronously();
                }
            }
        }
    }

    // Function to display greeting for employees (includes name)
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

    // Function to display greeting for customers (does not include name)
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

    // Function to toggle between Queue view and RequestService view
    private void ToggleView(object sender, System.EventArgs e)
    {
        // Retrieve the server of the table
        Employee tableServer = easyServerRepository.GetEmployeeById((int)table.EmployeeId);

        // Create a queue page for the server
        Pages.Queue queuePage = new(tableServer, easyServerRepository.ServerQueues, requestServiceStates);

        // If the button text is "View Queue", navigate to the server's Queue View
        if (ToggleViewBtn.Text == "View Queue")
        {
            // Disable all the buttons on the queue page to customers cannot remove tables from their server's queue
            queuePage.ServiceButtons.ForEach(b => b.IsEnabled = false);

            HomeFrame.Content = queuePage;
            ToggleViewBtn.Text = "Request Service";
        }
        else 
        {
            // Otherwise, navigate to the table's RequestService view
            HomeFrame.Content = new RequestService(table, queuePage, requestServiceStates, easyServerRepository);

            ToggleViewBtn.Text = "View Queue";
        }
    }
}