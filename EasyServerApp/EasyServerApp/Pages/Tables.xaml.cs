/*
 * Tables View
 * 
 * This view is displayed in the frame on the Home page. This view
 * allows employees to view table server assignments. If the currently
 * logged in employee is a manager, they may also modify table assignments
 * and add/remove tables to/from the database.
 * 
 */

using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Tables : ContentView
{
    private EasyServerRepository easyServerRepository;  // "API"
    private Employee employee;                          // The currently logged in employee
    private bool isManager;                             // Boolean to indicate whether the currently logged in user is a manager
    private List<Picker> pickers;                       // List of dropdown menus with each employee as an item
    private List<Label> labels;                         // List of table labels
    private List<Button> buttons;                       // List of delete table button
    private Hashtable requestServiceStates;             // States for whether or not customers are awaiting service at each table

    public Tables(Employee employee, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        // Assign values to global variables
        this.easyServerRepository = easyServerRepository;
        this.employee = employee;
        isManager = CheckUser();
        this.requestServiceStates = requestServiceStates;
        labels = new List<Label>();
        pickers = new List<Picker>();
        buttons = new List<Button>();

        if (isManager)  // If the user is a manager, display table manipulation buttons
        {
            SaveBtn.IsVisible = true;
            AddBtn.IsVisible = true;
        }
        else            // Otherwise, don't displaty them
        {
            SaveBtn.IsVisible = false;
            AddBtn.IsVisible = false;
        }

        // Create the grid display
        GenerateGridContents();
    }

    // Function to check whether the current user is a manager
	private bool CheckUser()
	{
        if (employee.Role == "Manager")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /* 
     * Function to create the grid display for all tables in the RestaurantTable table
     * 
     * Each Row/Column location has an table label, a dropdown menu with each employee as
     * an item, and a delete table button
     * 
     */
    private void GenerateGridContents()
    {
        // Get the employees list from the API property
        List<Employee> employees = easyServerRepository.Employees;

        // Get the tables list from the API property
        List<RestaurantTable> restaurantTables = easyServerRepository.RestaurantTables;

        // Create the appropriate number of rows
        GenerateGridLayout(restaurantTables.Count);

        int rowIndex = 0;

        int columnCount = TablesGrid.ColumnDefinitions.Count;
        int columnIndex = 0;

        // For each table in the tables list, create the aforementioned row/column contents
        for (int i = 0; i < restaurantTables.Count; i++)
        {
            var label = new Label
            {
                ClassId = restaurantTables[i].TableId.ToString(),
                WidthRequest = 250,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15
            };

            // If the table is being worked by a server, display their first name, last name, and employee ID
            if (restaurantTables[i].EmployeeId != null)
            {
                int employeeID = (int)restaurantTables[i].EmployeeId;
                Employee employee = easyServerRepository.GetEmployeeById(employeeID);
                label.Text = "Table " + label.ClassId + ": " + employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employeeID + "]";
            }
            else  // Otherwise, display that the table is not assigned
            {
                label.Text = "Table " + label.ClassId + ": Not assigned";
            }

            // Add the label to the labels list for later reference
            labels.Add(label);

            // Add the label to the grid
            TablesGrid.Add(label);
            TablesGrid.SetRow(label, rowIndex);
            TablesGrid.SetColumn(label, columnIndex);

            // If the currently logged in employee is a manager, add a dropdown menu and delete table button
            if (isManager)
            {
                var picker = new Picker
                {
                    Title = "Choose an employee: ",
                    ClassId = restaurantTables[i].TableId.ToString(),
                    WidthRequest = 250,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center
                };

                // If a manager wanted to remove a table's server assignment altogether
                picker.Items.Add("N/A");

                // Add each employee in the employees list as a dropdown menu item
                for (int x = 0; x < employees.Count; x++)
                {
                    picker.Items.Add(employees[x].FirstName.Trim() + " " + employees[x].LastName.Trim() + " [" + employees[x].EmployeeId + "]");
                }

                // Add the picker to the pickers list for later reference
                pickers.Add(picker);

                // Add the picker to the grid
                TablesGrid.Add(picker);
                TablesGrid.SetRow(picker, rowIndex + 1);
                TablesGrid.SetColumn(picker, columnIndex);

                var button = new Button
                {
                    Text = "Delete Table",
                    ClassId = restaurantTables[i].TableId.ToString(),
                    HeightRequest = 40,
                    WidthRequest = 160,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center
                };

                // Assign the button's clicked event the delete table function
                button.Clicked += new EventHandler(DeleteTable);

                // Add the button to the buttons list for later reference
                buttons.Add(button);

                // Add the button to the grid
                TablesGrid.Add(button);
                TablesGrid.SetRow(button, rowIndex + 2);
                TablesGrid.SetColumn(button, columnIndex);
            }

            columnIndex++;

            if (columnIndex == columnCount)
            {
                columnIndex = 0;

                if (isManager)  // If the current employee is a manager, a label, dropdown menu, and delete table button was added to the grid
                {
                    rowIndex += 3;  // Increment rows by three to account for each of the elements added
                }
                else  // If the current employee is a server, only a label was added to the grid
                {
                    rowIndex += 1;  // Increment rows by only one
                }
            }
        }
    }

    /* 
     * Function to generate the appropriate number of rows based on a hardcoded number of
     * columns and the passed in number of tables
     * 
     */
    private void GenerateGridLayout(int tableCount)
    {
        // Add a hardcoded number of columns to the grid
        for (int i = 0; i < 6; i++)
        {
            ColumnDefinition columnDefinition = new();
            columnDefinition.Width = 250;
            TablesGrid.AddColumnDefinition(columnDefinition);
        }

        // The number of rows is the number of tables divided by the number of columns, rounded up
        int rows = (int)Math.Ceiling((double)tableCount / TablesGrid.ColumnDefinitions.Count);

        // If the current employee is a manager, multiply rows by three to account for label, dropdown, and delete button
        if (isManager) rows *= 3;

        // Add calculated number of rows to the grid
        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            TablesGrid.AddRowDefinition(rowDefinition);

            if (isManager && i % 3 != 0)
            {
                // Assign higher row height for rows that are not for labels (just looks nicer)
                TablesGrid.RowDefinitions[i].Height = 75;
            }
            else if (!isManager || i % 3 == 0)
            {
                TablesGrid.RowDefinitions[i].Height = 50;
            }
        }
    }

    // Function to update tables' servers
    private void SaveServers(object sender, System.EventArgs e)
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            // If picker's value was changed, see if the table's server should be updated
            if (pickers[i].SelectedItem != null)
            {
                int tableID = int.Parse(pickers[i].ClassId);
                string employeeName = pickers[i].SelectedItem.ToString();
           
                if (employeeName == "N/A")  // If the server was updated to N/A, simply remove server assignment
                {
                    labels[i].Text = "Table " + tableID + ": Not assigned";
                    easyServerRepository.UpdateTableServer(tableID, null);
                }
                else  // Otherwise, use substring functions to get the selected employee's ID
                {
                    int idStart = employeeName.IndexOf("[") + 1;
                    int idEnd = employeeName.IndexOf("]");

                    int employeeeID = int.Parse(employeeName.Substring(idStart, idEnd - idStart));

                    Employee employee = easyServerRepository.GetEmployeeById(employeeeID);
                    RestaurantTable table = easyServerRepository.GetTableById(tableID);

                    // If the table's current server does not equal the selected server, update the table's server
                    if (table.EmployeeId != employee.EmployeeId)
                    {
                        // Since server was changed, cancel customer's service request to avoid queue mismatching
                        requestServiceStates[table.TableId] = false;

                        // Assign table new server in the database
                        easyServerRepository.UpdateTableServer(tableID, employee.EmployeeId);

                        // Update the table's label to reflect the server change
                        labels[i].Text = "Table " + tableID + ": " + employeeName;
                    }
                }

                pickers[i].SelectedItem = null;
            }        
        }
    }

    // Function to add a table
    private void AddTable(object sender, System.EventArgs e)
    {
        // Possible characters in a QR code
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[3];
        Random random = new();
        string qrCode;

        // Generate a unique QR code
        do
        {
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            qrCode = new string(stringChars);
        }
        while (easyServerRepository.GetTableByQRCode(qrCode) != null);
        
        // Add a table to the database
        RestaurantTable newTable = easyServerRepository.InsertRestaurantTableRow(qrCode, null);

        // Add a request service state entry for the table to the hashtable
        requestServiceStates.Add(newTable.TableId, false);

        // Re-create the view to reflect added table
        labels.Clear();
        pickers.Clear();
        buttons.Clear();
        TablesGrid.Clear();
        TablesGrid.RowDefinitions.Clear();
        TablesGrid.ColumnDefinitions.Clear();
        GenerateGridContents();
    }

    // Function to delete a table
    private async void DeleteTable(object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        int tableID = int.Parse(button.ClassId);

        ContentPage homePage = (ContentPage)Parent.Parent.Parent.Parent;
        bool answer = await homePage.DisplayAlert("Warning", "Are you sure you would like to delete Table " + tableID + "?", "Yes", "No");

        if (answer == true)
        {
            // If OK, remove the table with matching ID of the clicked delete button from the database
            RestaurantTable removedTable = easyServerRepository.DeleteRestaurantTableRow(tableID);

            // Remove the table's request service state entry from the hashtable
            requestServiceStates.Remove(removedTable.TableId);

            // If the table was being worked by a server, update the server's queue
            if (removedTable.EmployeeId != null)
            {
                ServerQueue serverQueue = easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == removedTable.EmployeeId).FirstOrDefault();
                List<RestaurantTable> newQueue = new();

                // Remove did not actively update the server's Queue view, so I recreated the queue and excluded the "removed" table
                for (int i = 0; i < serverQueue.Queue.Count; i++)
                {
                    if (serverQueue.Queue[i].TableId != removedTable.TableId)
                    {
                        newQueue.Add(serverQueue.Queue[i]);
                    }
                }

                // Assign new queue to the server
                easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == removedTable.EmployeeId).FirstOrDefault().Queue = newQueue;
            }

            // Re-create the view to reflect deleted table
            labels.Clear();
            pickers.Clear();
            buttons.Clear();
            TablesGrid.Clear();
            TablesGrid.RowDefinitions.Clear();
            TablesGrid.ColumnDefinitions.Clear();
            GenerateGridContents();
        }      
    }
}