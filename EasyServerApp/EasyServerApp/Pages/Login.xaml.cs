/*
 * Login View
 * 
 * This view is displayed in the frame on the Main page. This view
 * allows users to login to an existing account, create a new account,
 * or login to a table.
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Login : ContentView
{
    private EasyServerRepository easyServerRepository;  // "API"
    private Hashtable requestServiceStates;             // States for the request service button of each table, indicating clicked (T) or not clicked (F)

    public Login(Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();

        // Assign values to global variables
        this.requestServiceStates = requestServiceStates;
        this.easyServerRepository = easyServerRepository;
    }

    private async void AuthenticateUser(object sender, System.EventArgs e)
    {
        // Retrieve text in username and password text boxes
        string username = UsernameField.Text;
        string password = PasswordField.Text;

        if (username == null || password == null)
        {
            Warning.Text = "You must enter a username and password";
        }
        else
        {
            // If neither of the fields are empty, attempt to get an employee based on the inputted credentials 
            Employee employee = easyServerRepository.GetEmployeeByCredentials(username, password);

            if (employee != null) // If an employee was found, proceed with login
            {
                Employee formattedEmployee = employee;
                formattedEmployee.FirstName = formattedEmployee.FirstName.Trim();
                formattedEmployee.LastName = formattedEmployee.LastName.Trim();
                formattedEmployee.Username = formattedEmployee.Username.Trim();
                formattedEmployee.Password = formattedEmployee.Password.Trim();
                formattedEmployee.Role = formattedEmployee.Role.Trim();

                if (TableIDField.IsVisible == true) // If attempting to login to a table, check if the employee is a manager
                {
                    if (formattedEmployee.Role == "Manager")  // If the employee is a manager, attempt to get a table based on the inputted table ID
                    {
                        int tableID = int.Parse(TableIDField.Text.Trim());
                        RestaurantTable table = easyServerRepository.GetTableById(tableID);

                        if (table != null)  // If a table was found, determine if a server is working the table
                        {
                            if (table.EmployeeId.HasValue)  // If a server is working the table, login to the table
                            {
                                ContentPage home = new Home(formattedEmployee, table, requestServiceStates, easyServerRepository);
                                await Navigation.PushAsync(home);
                                Warning.Text = "";
                                ToggleLgnAccFields();
                            }
                            else  // If no server is working the requested table, display an error
                            {
                                Warning.Text = "No server is working the requested table";
                            }
                        }
                        else  // If no table was found, display an error
                        {
                            Warning.Text = "Table not found";
                        }
                    }
                    else  // If the employee is a server, display an error
                    {
                        Warning.Text = "You must be a manager for table login";
                    }
                }
                else  // If logging into a user, navigate to the Home page
                {
                    ContentPage home = new Home(formattedEmployee, null, requestServiceStates, easyServerRepository);
                    await Navigation.PushAsync(home);
                    Warning.Text = "";
                    ToggleLgnAccFields();
                }
            }
            else  // If no employee account was found, display an error
            {
                Warning.Text = "Account not found";
            }
        }

        // Reset text boxes
        UsernameField.Text = "";
        PasswordField.Text = "";
        TableIDField.Text = "";
    }

    // Reset to the default login display
    private void ResetLgnFields(object sender, System.EventArgs e)
    {
        ToggleLgnAccFields();
        Warning.Text = "";
    }

    // Display the appropriate fields for an employee login
    private void ToggleLgnAccFields()
    {
        LoginLbl.Text = "Login to your Account";
        FirstNameField.IsVisible = false;
        LastNameField.IsVisible = false;
        NewUsernameField.IsVisible = false;
        NewPasswordField.IsVisible = false;
        CreateAccBtn.IsVisible = false;
        LgnAccFieldsBtn.IsVisible = false;

        CrtAccFieldsBtn.IsVisible = true;
        UsernameField.IsVisible = true;
        PasswordField.IsVisible = true;
        LoginBtn.IsVisible = true;

        TableIDField.IsVisible = false;
        Divider2.IsVisible = false;

        FirstNameField.Text = "";
        LastNameField.Text = "";
        UsernameField.Text = "";
        PasswordField.Text = "";
        NewUsernameField.Text = "";
        NewPasswordField.Text = "";
    }

    // Display the appropriate fields for creating an account
    private void ToggleCrtAccFields(object sender, System.EventArgs e)
    {
        LoginLbl.Text = "Create an Account";
        FirstNameField.IsVisible = true;
        LastNameField.IsVisible = true;
        NewUsernameField.IsVisible = true;
        NewPasswordField.IsVisible = true;
        CreateAccBtn.IsVisible = true;
        LgnAccFieldsBtn.IsVisible = true;

        CrtAccFieldsBtn.IsVisible = false;
        UsernameField.IsVisible = false;
        PasswordField.IsVisible = false;
        LoginBtn.IsVisible = false;

        TableIDField.IsVisible = false;
        Divider2.IsVisible = false;

        FirstNameField.Text = "";
        LastNameField.Text = "";
        UsernameField.Text = "";
        PasswordField.Text = "";
        NewUsernameField.Text = "";
        NewPasswordField.Text = "";

        Warning.Text = "";
    }

    // Display the appropriate fields for a table login
    private void ToggleTableFields(object sender, System.EventArgs e)
    {
        LoginLbl.Text = "Login to a Table";

        TableIDField.IsVisible = true;
        LgnAccFieldsBtn.IsVisible = true;
        CrtAccFieldsBtn.IsVisible = true;
        Divider2.IsVisible = true;
        UsernameField.IsVisible = true;
        PasswordField.IsVisible = true;
        LoginBtn.IsVisible = true;

        FirstNameField.IsVisible = false;
        LastNameField.IsVisible = false;
        NewUsernameField.IsVisible = false;
        NewPasswordField.IsVisible = false;
        CreateAccBtn.IsVisible = false;

        FirstNameField.Text = "";
        LastNameField.Text = "";
        UsernameField.Text = "";
        PasswordField.Text = "";
        NewUsernameField.Text = "";
        NewPasswordField.Text = "";

        Warning.Text = "";
    }

    // Function to create an employee account
    private async void CreateAccount(object sender, System.EventArgs e)
    {
        // Retrieve text in each of the following fields
        string firstName = FirstNameField.Text;
        string lastName = LastNameField.Text;
        string username = NewUsernameField.Text;
        string password = NewPasswordField.Text;

        Employee newEmployee = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            Password = password
        };

        if (username == "" || password == "" || firstName == "" || lastName == "")  // If any fields are empty, display an error
        {
            Warning.Text = "You must complete all fields";
        }
        else if (easyServerRepository.GetEmployeeByUsername(username) != null)      // If username already exists, display an error
        {
            Warning.Text = "This username is already taken";
        }
        else  // Otherwise, add the employee to the database
        {
            Employee insertedEmployee = easyServerRepository.InsertEmployeeRow(firstName, lastName, username, password);
            insertedEmployee.FirstName = insertedEmployee.FirstName.Trim();
            insertedEmployee.LastName = insertedEmployee.LastName.Trim();
            insertedEmployee.Username = insertedEmployee.Username.Trim();
            insertedEmployee.Password = insertedEmployee.Password.Trim();
            insertedEmployee.Role = insertedEmployee.Role.Trim();

            // Get the newly created employee's server queue
            ServerQueue serverQueue = easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == insertedEmployee.EmployeeId).FirstOrDefault();

            // Navigate to the Home page
            ContentPage home = new Home(insertedEmployee, null, requestServiceStates, easyServerRepository);
            await Navigation.PushAsync(home);

            // Reset to the default login display
            ToggleLgnAccFields();
            Warning.Text = "";
        }
    }
}