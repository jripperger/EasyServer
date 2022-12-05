/*
 * Employees View
 * 
 * View that is displayed in the frame on the Home page. This view
 * is only available to users with the "Manager" role, and it allows
 * for the manipulation of all employees in the Employee Table.
 * 
 */

using DocumentFormat.OpenXml.Drawing.Charts;
using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Employees : ContentView
{
    private EasyServerRepository easyServerRepository;  // "API"
    private List<Label> labels;                         // List of employee labels
    private List<Picker> pickers;                       // List of dropdown menus with the role options
    private List<Button> buttons;                       // List of delete employee buttons
    private Employee employee;                          // The currently logged in employee
    private Hashtable requestServiceStates;             // States for the request service button of each table, indicating clicked (T) or not clicked (F)

    public Employees(Employee employee, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        // Assign values to global variables
        this.easyServerRepository = easyServerRepository;
        this.employee = employee;
        this.requestServiceStates = requestServiceStates;

        // Initialize lists
        labels = new List<Label>();
        pickers = new List<Picker>();
        buttons = new List<Button>();

        // Create the grid display
        GenerateGridContents();
    }

    /* 
     * Function to create the grid display for all employees in the Employee table
     * 
     * Each Row/Column location has an employee label, a dropdown menu with the role
     * options (Manager or server), and a delete employee button
     */
    private void GenerateGridContents()
    {
        // Get the employees list from the API property
        List<Employee> employees = easyServerRepository.Employees;

        // Creates the appropriate number of rows
        GenerateGridLayout(employees.Count);

        int rowIndex = 0;

        int columnCount = EmployeesGrid.ColumnDefinitions.Count;
        int columnIndex = 0;

        // For each employee in the employees list, create the aforementioned row/column contents
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

            // Set the label text based on the employee ID of the current employee of iteration
            int employeeID = employees[i].EmployeeId;
            Employee employee = easyServerRepository.GetEmployeeById(employeeID);
            label.Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employeeID + "]: " + employee.Role;

            // Add the label to the labels list for later reference
            labels.Add(label);

            // Add the label to the grid
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

            // Add the two role options to the created picker
            picker.Items.Add("Server");
            picker.Items.Add("Manager");

            // Add the picker to the pickers list for later reference
            pickers.Add(picker);

            // Add the picker to the grid
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

            // If the button is for the currently logged in employee, disable the delete employee button
            // to avoid any complications of deleting the currently logged in employee
            if (button.ClassId == this.employee.EmployeeId.ToString())
            {
                button.IsEnabled = false;
            }
            else
            {
                button.IsEnabled = true;
            }

            // Assign the button's clicked event the delete employee function
            button.Clicked += new EventHandler(DeleteEmployee);

            // Add the button to the buttons list for later reference
            buttons.Add(button);

            // Add the button to the grid 
            EmployeesGrid.Add(button);
            EmployeesGrid.SetRow(button, rowIndex + 2);
            EmployeesGrid.SetColumn(button, columnIndex);

            columnIndex++;

            // If column index reaches column count, reset to 0
            if (columnIndex == columnCount)
            {
                columnIndex = 0;

                // Increment by three to account for label, dropdown, and delete button
                rowIndex += 3;
            }
        }
    }

    /* 
     * Function to generate the appropriate number of rows based on a hardcoded number of
     * columns and the passed in number of employees
     */
    private void GenerateGridLayout(int employeeCount)
    {
        // Add hardcoded number of columns to the grid
        for (int i = 0; i < 6; i++)
        {
            ColumnDefinition columnDefinition = new()
            {
                Width = 250
            };

            EmployeesGrid.AddColumnDefinition(columnDefinition);
        }

        // The number of rows is the number of employees divided by the number of columns multiplied by 3
        // to account for a label, dropdown, and a delete button, rounded up
        int rows = (int)Math.Ceiling((double)employeeCount / EmployeesGrid.ColumnDefinitions.Count) * 3;

        // Add calculated number of rows to the grid
        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            EmployeesGrid.AddRowDefinition(rowDefinition);

            // Assign higher row height for rows that are not for labels (just looks nicer)
            if (i % 3 != 0)
            {
                EmployeesGrid.RowDefinitions[i].Height = 75;
            }
        }
    }

    
    // Function to update an employee's role
    private void SaveServers(object sender, System.EventArgs e)
    { 
        for (int i = 0; i < pickers.Count; i++)
        {
            int employeeID = int.Parse(pickers[i].ClassId);
            Employee employee = easyServerRepository.GetEmployeeById(employeeID);

            // Check if picker's value was changed and does not equal the associated server's current role
            if (pickers[i].SelectedItem != null && pickers[i].SelectedItem.ToString() != employee.Role.Trim())
            {
                // If condition is met, assign employee new role in the database
                easyServerRepository.UpdateEmployeeRole(employee.EmployeeId, pickers[i].SelectedItem.ToString());

                // Modify label to reflect role change
                labels[i].Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employee.EmployeeId + "]: " + pickers[i].SelectedItem.ToString();
            }

            pickers[i].SelectedItem = null;
        }
    }

    // Function to delete an employee
    private async void DeleteEmployee(object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        int employeeID = int.Parse(button.ClassId);

        // Get employee associated with delete button that was clicked based on ID
        Employee employee = easyServerRepository.GetEmployeeById(employeeID);

        // Display an alert to warn about deletion
        ContentPage homePage = (ContentPage)Parent.Parent.Parent.Parent;
        bool answer = await homePage.DisplayAlert("Warning", "Are you sure you would like to delete " + employee.FirstName.Trim() + " " + employee.LastName.Trim() + "?", "Yes", "No");

        if (answer == true)
        {
            // If OK, remove employee from database
            easyServerRepository.DeleteEmployeeRow(employeeID);

            // For each table that the server was working, update the table's request service button to not clicked
            for (int i = 0; i < easyServerRepository.RestaurantTables.Count; i++)
            {
                if (easyServerRepository.RestaurantTables[i].EmployeeId == employeeID)
                {
                    requestServiceStates[easyServerRepository.RestaurantTables[i].TableId] = false;
                }
            }

            // Re-create the page to reflect deleted employee
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