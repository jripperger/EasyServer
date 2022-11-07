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
                            RequestService requestService = new(table, easyServerRepository);
                            ContentPage home = new Home(formattedEmployee, null, requestService, easyServerRepository);
                            await Navigation.PushAsync(home);
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

    private void ToggleFields(object sender, System.EventArgs e)
    {
        if (FirstNameField.IsVisible == false && TableIDField.IsVisible == false)
        {
            LoginLbl.Text = "Create an Account";
            FirstNameField.IsVisible = true;
            LastNameField.IsVisible = true;
            NewUsernameField.IsVisible = true;
            NewPasswordField.IsVisible = true;
            CreateAccBtn.IsVisible = true;

            ToggleFieldsBtn.Text = "Login to your Account";
            UsernameField.IsVisible = false;
            PasswordField.IsVisible = false;
            LoginBtn.IsVisible = false;

            TableIDField.IsVisible = false;
        }
        else
        {
            LoginLbl.Text = "Login";
            FirstNameField.IsVisible = false;
            LastNameField.IsVisible = false;
            NewUsernameField.IsVisible = false;
            NewPasswordField.IsVisible = false;
            CreateAccBtn.IsVisible = false;

            ToggleFieldsBtn.Text = "Create an Account";
            UsernameField.IsVisible = true;
            PasswordField.IsVisible = true;
            LoginBtn.IsVisible = true;

            TableIDField.IsVisible = false;
        }

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
        if (TableIDField.IsVisible == false)
        {
            LoginLbl.Text = "Table Login";
            TableIDField.IsVisible = true; 
            ToggleFieldsBtn.Text = "Login to your Account";
        }
        else
        {
            LoginLbl.Text = "Login";
            TableIDField.IsVisible = false;
            ToggleFieldsBtn.Text = "Create an Account";
        }

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

            ToggleFieldsBtn.Text = "Create an Account";
            UsernameField.IsVisible = true;
            PasswordField.IsVisible = true;
            LoginBtn.IsVisible = true;
        }
    }
}