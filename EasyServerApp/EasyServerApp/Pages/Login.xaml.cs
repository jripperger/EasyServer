/*
 * Login View
 * 
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Login : ContentView
{
    private EasyServerRepository easyServerRepository;

    private Hashtable requestServiceStates;

    public Login(Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
    {
        InitializeComponent();
        
        this.requestServiceStates = requestServiceStates;
        this.easyServerRepository = easyServerRepository;
    }

    private async void AuthenticateUser(object sender, System.EventArgs e)
    {
        string username = UsernameField.Text;
        string password = PasswordField.Text;

        if (username == null || password == null)
        {
            Warning.Text = "You must enter a username and password";
        }
        else
        {
            Employee employee = easyServerRepository.GetEmployeeByCredentials(username, password);

            if (employee != null)
            {
                Employee formattedEmployee = employee;
                formattedEmployee.FirstName = formattedEmployee.FirstName.Trim();
                formattedEmployee.LastName = formattedEmployee.LastName.Trim();
                formattedEmployee.Username = formattedEmployee.Username.Trim();
                formattedEmployee.Password = formattedEmployee.Password.Trim();
                formattedEmployee.Role = formattedEmployee.Role.Trim();

                if (TableIDField.IsVisible == true)
                {
                    if (formattedEmployee.Role == "Manager")
                    {
                        int tableID = int.Parse(TableIDField.Text.Trim());
                        RestaurantTable table = easyServerRepository.GetTableById(tableID);

                        if (table != null)
                        {
                            if (table.EmployeeId.HasValue)
                            {
                                ContentPage home = new Home(formattedEmployee, table, requestServiceStates, easyServerRepository);
                                await Navigation.PushAsync(home);
                                Warning.Text = "";
                                GetLgnAccFields();
                            }
                            else
                            {
                                Warning.Text = "No server is working the requested table";
                            }
                        }
                        else
                        {
                            Warning.Text = "Table not found";
                        }
                    }
                    else
                    {
                        Warning.Text = "You must be a manager for table login";
                    }
                }
                else
                {
                    ContentPage home = new Home(formattedEmployee, null, requestServiceStates, easyServerRepository);
                    await Navigation.PushAsync(home);
                    Warning.Text = "";
                    GetLgnAccFields();
                }
            }
            else
            {
                Warning.Text = "Account not found";
            }
        }

        UsernameField.Text = "";
        PasswordField.Text = "";
        TableIDField.Text = "";
    }

    private void ToggleLgnAccFields(object sender, System.EventArgs e)
    {
        GetLgnAccFields();
        Warning.Text = "";
    }

    private void GetLgnAccFields()
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

    private async void CreateAccount(object sender, System.EventArgs e)
    {
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

        if (username == "" || password == "" || firstName == "" || lastName == "")
        {
            Warning.Text = "You must complete all fields";
        }
        else if (easyServerRepository.GetEmployeeByUsername(username) != null)
        {
            Warning.Text = "This username is already taken";
        }
        else
        {
            Employee insertedEmployee = easyServerRepository.InsertEmployeeRow(firstName, lastName, username, password);
            insertedEmployee.FirstName = insertedEmployee.FirstName.Trim();
            insertedEmployee.LastName = insertedEmployee.LastName.Trim();
            insertedEmployee.Username = insertedEmployee.Username.Trim();
            insertedEmployee.Password = insertedEmployee.Password.Trim();
            insertedEmployee.Role = insertedEmployee.Role.Trim();

            ServerQueue serverQueue = easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == insertedEmployee.EmployeeId).FirstOrDefault();

            Tables tablesPage = new(insertedEmployee, requestServiceStates, easyServerRepository);

            ContentPage home = new Home(insertedEmployee, null, requestServiceStates, easyServerRepository);
            await Navigation.PushAsync(home);

            GetLgnAccFields();
            Warning.Text = "";
        }
    }
}