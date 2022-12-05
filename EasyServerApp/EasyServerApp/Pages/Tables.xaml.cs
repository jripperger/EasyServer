/*
 * Tables View
 * 
 * 
 */

using EasyServerApp.EasyServerDB;
using Microsoft.Maui.Graphics.Text;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Tables : ContentView
{
    private EasyServerRepository easyServerRepository;
    
    private Employee employee;
    private bool isManager;

    private List<Picker> pickers;
    private List<Label> labels;
    private List<Button> buttons;

    private Hashtable requestServiceStates;

    public Tables(Employee employee, Hashtable requestServiceStates, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;

        this.employee = employee;
        isManager = CheckUser();

        this.requestServiceStates = requestServiceStates;

        if (isManager)
        {
            SaveBtn.IsVisible = true;
            AddBtn.IsVisible = true;
        }
        else
        {
            SaveBtn.IsVisible = false;
            AddBtn.IsVisible = false;
        }

        labels = new List<Label>();
        pickers = new List<Picker>();
        buttons = new List<Button>();

        GenerateGridContents();
    }

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

    private void GenerateGridContents()
    {
        List<Employee> employees = easyServerRepository.Employees;
        List<RestaurantTable> restaurantTables = easyServerRepository.RestaurantTables;

        GenerateGridLayout(restaurantTables.Count);

        int rowIndex = 0;

        int columnCount = TablesGrid.ColumnDefinitions.Count;
        int columnIndex = 0;

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

            if (restaurantTables[i].EmployeeId != null)
            {
                int employeeID = (int)restaurantTables[i].EmployeeId;
                Employee employee = easyServerRepository.GetEmployeeById(employeeID);
                label.Text = "Table " + label.ClassId + ": " + employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employeeID + "]";
            }
            else
            {
                label.Text = "Table " + label.ClassId + ": Not assigned";
            }
            
            labels.Add(label);
            TablesGrid.Add(label);
            TablesGrid.SetRow(label, rowIndex);
            TablesGrid.SetColumn(label, columnIndex);

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

                picker.Items.Add("N/A");

                for (int x = 0; x < employees.Count; x++)
                {
                    picker.Items.Add(employees[x].FirstName.Trim() + " " + employees[x].LastName.Trim() + " [" + employees[x].EmployeeId + "]");
                }

                pickers.Add(picker);
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

                button.Clicked += new EventHandler(DeleteTable);
                buttons.Add(button);
                TablesGrid.Add(button);
                TablesGrid.SetRow(button, rowIndex + 2);
                TablesGrid.SetColumn(button, columnIndex);
            }

            columnIndex++;

            if (columnIndex == columnCount)
            {
                columnIndex = 0;

                if (isManager)
                {
                    rowIndex += 3;
                }
                else
                {
                    rowIndex += 1;
                }
            }
        }
    }

    private void GenerateGridLayout(int tableCount)
    {
        for (int i = 0; i < 6; i++)
        {
            ColumnDefinition columnDefinition = new();
            columnDefinition.Width = 250;
            TablesGrid.AddColumnDefinition(columnDefinition);
        }

        int rows = (int)Math.Ceiling((double)tableCount / TablesGrid.ColumnDefinitions.Count);
        if (isManager) rows *= 3;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            TablesGrid.AddRowDefinition(rowDefinition);

            if (isManager)
            {
                if (i % 3 != 0)
                {
                    TablesGrid.RowDefinitions[i].Height = 75;
                }
            }
            else
            {
                TablesGrid.RowDefinitions[i].Height = 50;
            }
        }
    }

    private void SaveServers(object sender, System.EventArgs e)
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            if (pickers[i].SelectedItem != null)
            {
                int tableID = int.Parse(pickers[i].ClassId);
                string employeeName = pickers[i].SelectedItem.ToString();

                if (employeeName == "N/A")
                {
                    labels[i].Text = "Table " + tableID + ": Not assigned";
                    easyServerRepository.UpdateTableServer(tableID, null);
                }
                else
                {
                    int idStart = employeeName.IndexOf("[") + 1;
                    int idEnd = employeeName.IndexOf("]");

                    int employeeeID = int.Parse(employeeName.Substring(idStart, idEnd - idStart));

                    Employee employee = easyServerRepository.GetEmployeeById(employeeeID);
                    RestaurantTable table = easyServerRepository.GetTableById(tableID);

                    if (table.EmployeeId != employee.EmployeeId)
                    {
                        requestServiceStates[table.TableId] = false;
                        easyServerRepository.UpdateTableServer(tableID, employee.EmployeeId);

                        labels[i].Text = "Table " + tableID + ": " + employeeName;
                    }
                }

                pickers[i].SelectedItem = null;
            }        
        }
    }

    private void AddTable(object sender, System.EventArgs e)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[3];
        Random random = new();
        string qrCode;

        do
        {
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            qrCode = new string(stringChars);
        }
        while (easyServerRepository.GetTableByQRCode(qrCode) != null);
        

        RestaurantTable newTable = easyServerRepository.InsertRestaurantTableRow(qrCode, null);
        requestServiceStates.Add(newTable.TableId, false);

        labels.Clear();
        pickers.Clear();
        buttons.Clear();
        TablesGrid.Clear();
        TablesGrid.RowDefinitions.Clear();
        TablesGrid.ColumnDefinitions.Clear();
        GenerateGridContents();
    }

    private async void DeleteTable(object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        int tableID = int.Parse(button.ClassId);

        ContentPage homePage = (ContentPage)Parent.Parent.Parent.Parent;
        bool answer = await homePage.DisplayAlert("Warning", "Are you sure you would like to delete Table " + tableID + "?", "Yes", "No");

        if (answer == true)
        {
            RestaurantTable removedTable = easyServerRepository.DeleteRestaurantTableRow(tableID);

            requestServiceStates.Remove(removedTable.TableId);

            if (removedTable.EmployeeId != null)
            {
                ServerQueue serverQueue = easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == removedTable.EmployeeId).FirstOrDefault();
                List<RestaurantTable> newQueue = new();

                for (int i = 0; i < serverQueue.Queue.Count; i++)
                {
                    if (serverQueue.Queue[i].TableId != removedTable.TableId)
                    {
                        newQueue.Add(serverQueue.Queue[i]);
                    }
                }
                
                easyServerRepository.ServerQueues.Where(x => x.Employee.EmployeeId == removedTable.EmployeeId).FirstOrDefault().Queue = newQueue;
            }
            
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