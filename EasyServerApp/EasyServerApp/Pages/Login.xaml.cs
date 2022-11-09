using CloudinaryDotNet.Actions;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class Login : ContentView
{
    private readonly EasyServerRepository easyServerRepository;
    public Login(EasyServerRepository easyServerRepository)
    {
        InitializeComponent();
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
                Employee formattedEmployee = new()
                {
                    FirstName = employee.FirstName.Trim(),
                    LastName = employee.LastName.Trim(),
                    Username = employee.Username.Trim(),
                    Password = employee.Password.Trim(),
                    Role = employee.Role.Trim()
                };

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
                                RequestService requestService = new(table, easyServerRepository);
                                ContentPage home = new Home(formattedEmployee, null, requestService, easyServerRepository);
                                await Navigation.PushAsync(home);
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
                    Tables tables = new(employee, easyServerRepository);
                    ContentPage home = new Home(formattedEmployee, tables, null, easyServerRepository);
                    await Navigation.PushAsync(home);
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

        Warning.Text = "";
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

        Employee newEmployee = new Employee()
        {
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            Password = password
        };

        if (username == null || password == null || firstName == null || lastName == null)
        {
            Warning.Text = "You must complete all fields.";
        }
        else if (easyServerRepository.GetEmployeeByUsername(username) != null)
        {
            Warning.Text = "This username is already taken.";
        }
        else
        {
            easyServerRepository.InsertEmployeeRow(firstName, lastName, username, password);

            Tables tables = new(newEmployee, easyServerRepository);
            ContentPage home = new Home(newEmployee, tables, null, easyServerRepository);
            await Navigation.PushAsync(home);

            FirstNameField.Text = "";
            LastNameField.Text = "";
            NewUsernameField.Text = "";
            NewPasswordField.Text = "";
            Warning.Text = "";

            LoginLbl.Text = "Login";
            FirstNameField.IsVisible = false;
            LastNameField.IsVisible = false;
            NewUsernameField.IsVisible = false;
            NewPasswordField.IsVisible = false;
            CreateAccBtn.IsVisible = false;
            LgnAccFieldsBtn.IsVisible = false;

            UsernameField.IsVisible = true;
            PasswordField.IsVisible = true;
            LoginBtn.IsVisible = true;
        }
    }
}