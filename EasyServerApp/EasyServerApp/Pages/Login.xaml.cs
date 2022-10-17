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
                ContentPage home = new Home(employee, easyServerRepository);
                await Navigation.PushAsync(home);
            }
            else
            {
                Warning.Text = "Account not found";
            }
        }
    }

    private void ShowFields(object sender, System.EventArgs e)
    {
        if (FirstNameField.IsVisible == false)
        {
            LoginLbl.Text = "Create an Account";
            FirstNameField.IsVisible = true;
            LastNameField.IsVisible = true;
            NewUsernameField.IsVisible = true;
            NewPasswordField.IsVisible = true;
            CreateAccBtn.IsVisible = true;

            ToggleFieldsBtn.Text = "Login";
            UsernameField.IsVisible = false;
            PasswordField.IsVisible = false;
            LoginBtn.IsVisible = false;
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
        }
        

    }

    private void CreateAccount(object sender, System.EventArgs e)
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

        if (username == null || password == null)
        {
            Warning.Text = "You must enter a username and password.";
        }
        else
        {
            easyServerRepository.InsertEmployeeRow(firstName, lastName, username, password);
            ContentPage home = new Home(newEmployee, easyServerRepository);
        }
    }
}