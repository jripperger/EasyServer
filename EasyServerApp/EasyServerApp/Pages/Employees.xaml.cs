using DocumentFormat.OpenXml.Drawing.Charts;
using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Employees : ContentView
{
    private EasyServerRepository easyServerRepository;
    private List<Label> labels;
    private List<Picker> pickers;
    private List<Button> buttons;
    private Employee employee;
    private Hashtable requestServiceStates;

    public Employees(Employee employee, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        this.employee = employee;
        this.requestServiceStates = requestServiceStates;

        labels = new List<Label>();
        pickers = new List<Picker>();
        buttons = new List<Button>();

        GenerateGridContents();
    }

    private void GenerateGridContents()
    {
        List<Employee> employees = easyServerRepository.Employees;

        GenerateGridLayout(employees.Count);

        int rowIndex = 0;

        int columnCount = EmployeesGrid.ColumnDefinitions.Count;
        int columnIndex = 0;

        for (int i = 0; i < employees.Count; i++)
        {
            var label = new Label
            {
                ClassId = employees[i].EmployeeId.ToString(),
                WidthRequest = 250,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15
            };

            int employeeID = employees[i].EmployeeId;
            Employee employee = easyServerRepository.GetEmployeeById(employeeID);

            label.Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employeeID + "]: " + employee.Role;
            labels.Add(label);

            EmployeesGrid.Add(label);
            EmployeesGrid.SetRow(label, rowIndex);
            EmployeesGrid.SetColumn(label, columnIndex);

            var picker = new Picker
            {
                Title = "Choose a role: ",
                ClassId = employees[i].EmployeeId.ToString(),
                WidthRequest = 250,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center
            };

            picker.Items.Add("Server");
            picker.Items.Add("Manager");

            pickers.Add(picker);
            EmployeesGrid.Add(picker);
            EmployeesGrid.SetRow(picker, rowIndex + 1);
            EmployeesGrid.SetColumn(picker, columnIndex);

            var button = new Button
            {
                Text = "Delete Employee",
                ClassId = employees[i].EmployeeId.ToString(),
                HeightRequest = 40,
                WidthRequest = 160,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center
            };

            if (button.ClassId == this.employee.EmployeeId.ToString())
            {
                button.IsEnabled = false;
            }
            else
            {
                button.IsEnabled = true;
            }

            button.Clicked += new EventHandler(DeleteEmployee);
            buttons.Add(button);
            EmployeesGrid.Add(button);
            EmployeesGrid.SetRow(button, rowIndex + 2);
            EmployeesGrid.SetColumn(button, columnIndex);

            columnIndex++;

            if (columnIndex == columnCount)
            {
                columnIndex = 0;

                rowIndex += 3;
            }
        }
    }

    private void GenerateGridLayout(int employeeCount)
    {
        for (int i = 0; i < 6; i++)
        {
            ColumnDefinition columnDefinition = new()
            {
                Width = 250
            };

            EmployeesGrid.AddColumnDefinition(columnDefinition);
        }

        int rows = (int)Math.Ceiling((double)employeeCount / EmployeesGrid.ColumnDefinitions.Count) * 3;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            EmployeesGrid.AddRowDefinition(rowDefinition);

            if (i % 3 != 0)
            {
                EmployeesGrid.RowDefinitions[i].Height = 75;
            }
        }
    }

    private void SaveServers(object sender, System.EventArgs e)
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            int employeeID = int.Parse(pickers[i].ClassId);
            Employee employee = easyServerRepository.GetEmployeeById(employeeID);

            if (pickers[i].SelectedItem != null && pickers[i].SelectedItem.ToString() != employee.Role.Trim())
            {
                easyServerRepository.UpdateEmployeeRole(employee.EmployeeId, pickers[i].SelectedItem.ToString());
                labels[i].Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employee.EmployeeId + "]: " + pickers[i].SelectedItem.ToString();
            }

            pickers[i].SelectedItem = null;
        }
    }

    private async void DeleteEmployee(object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        int employeeID = int.Parse(button.ClassId);
        Employee employee = easyServerRepository.GetEmployeeById(employeeID);

        ContentPage homePage = (ContentPage)Parent.Parent.Parent.Parent;
        bool answer = await homePage.DisplayAlert("Warning", "Are you sure you would like to delete " + employee.FirstName.Trim() + " " + employee.LastName.Trim() + "?", "Yes", "No");

        if (answer == true)
        {
            easyServerRepository.DeleteEmployeeRow(employeeID);

            for (int i = 0; i < easyServerRepository.RestaurantTables.Count; i++)
            {
                if (easyServerRepository.RestaurantTables[i].EmployeeId == employeeID)
                {
                    requestServiceStates[easyServerRepository.RestaurantTables[i].TableId] = false;
                }
            }

            labels.Clear();
            pickers.Clear();
            buttons.Clear();
            EmployeesGrid.Clear();
            EmployeesGrid.RowDefinitions.Clear();
            EmployeesGrid.ColumnDefinitions.Clear();
            GenerateGridContents();
        }
    }
}