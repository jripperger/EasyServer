using CloudinaryDotNet.Actions;
using EasyServerApp.EasyServerDB;
using Microsoft.Maui.Controls;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace EasyServerApp.Pages;

public partial class Tables : ContentView
{
    private EasyServerRepository easyServerRepository;
    
    private Employee employee;
    private bool isManager;

    private List<Picker> pickers;
    private List<Label> labels;

    private List<RestaurantTable> restaurantTables;

    private Hashtable queuePages;
    private Hashtable requestServicePages;

    public Tables(Employee employee, Hashtable queuePages, Hashtable requestServicePages, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.queuePages = queuePages;
        this.requestServicePages = requestServicePages;
        this.easyServerRepository = easyServerRepository;

        this.employee = employee;
        isManager = CheckUser();

        if (isManager)
        {
            SaveBtn.IsVisible = true;
        }
        else
        {
            SaveBtn.IsVisible = false;
        }

        labels = new List<Label>();
        pickers = new List<Picker>();

        restaurantTables = easyServerRepository.GetTableList();

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
        List<Employee> employees = easyServerRepository.GetEmployeeList();

        GenerateGridLayout();

        int rowIndex = 0;

        int columnCount = TablesGrid.ColumnDefinitions.Count;
        int columnIndex = 0;

        for (int i = 0; i < restaurantTables.Count; i++)
        {
            var label = new Label
            {
                ClassId = "TableIDLabel",
                StyleId = "TableIDLabel" + (i + 1)
            };

            if (restaurantTables[i].EmployeeId != null)
            {
                int employeeID = (int)restaurantTables[i].EmployeeId;
                Employee employee = easyServerRepository.GetEmployeeById(employeeID);
                label.Text = "Table " + (i + 1) + ": " + employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employeeID + "]";
            }
            else
            {
                label.Text = "Table " + (i + 1) + ": Not assigned";
            }
            
            labels.Add(label);
            TablesGrid.Add(label);
            TablesGrid.SetRow(label, rowIndex);
            TablesGrid.SetColumn(label, columnIndex);

            if (isManager)
            {
                var picker = new Picker();
                picker.Title = "Choose an employee: ";

                for (int x = 0; x < employees.Count; x++)
                {
                    picker.Items.Add(employees[x].FirstName.Trim() + " " + employees[x].LastName.Trim() + " [" + employees[x].EmployeeId + "]");
                }

                pickers.Add(picker);
                TablesGrid.Add(picker);
                TablesGrid.SetRow(picker, rowIndex + 1);
                TablesGrid.SetColumn(picker, columnIndex);
            }

            columnIndex++;

            if (columnIndex == columnCount)
            {
                columnIndex = 0;

                if (isManager) rowIndex += 2;
                else rowIndex += 1;
            }
        }
    }

    private void GenerateGridLayout()
    {
        for (int i = 0; i < 6; i++)
        {
            ColumnDefinition columnDefinition = new();
            TablesGrid.AddColumnDefinition(columnDefinition);
        }

        int rows = (int)Math.Ceiling((double)restaurantTables.Count / TablesGrid.ColumnDefinitions.Count);
        if (isManager) rows *= 2;

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            TablesGrid.AddRowDefinition(rowDefinition);

            if (isManager)
            {
                if (i % 2 != 0)
                {
                    TablesGrid.RowDefinitions[i].Height = 100;
                }
            }
            else TablesGrid.RowDefinitions[i].Height = 50;
        }
    }

    private void SaveServers(object sender, System.EventArgs e)
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            if (pickers[i].SelectedItem != null)
            {
                string employeeName = pickers[i].SelectedItem.ToString();

                int firstNameIndex = employeeName.IndexOf(" ");
                int lastNameIndex = employeeName.IndexOf("[") - 1;

                string firstName = employeeName.Substring(0, firstNameIndex);
                string lastName = employeeName.Substring(firstNameIndex + 1, lastNameIndex - firstNameIndex - 1);

                Employee employee = easyServerRepository.GetEmployeeByName(firstName, lastName);

                if (restaurantTables[i].EmployeeId != employee.EmployeeId)
                {
                    ((RequestService)requestServicePages[restaurantTables[i].TableId]).QueuePage = (Queue)queuePages[employee.EmployeeId];
                    easyServerRepository.UpdateTableServer(restaurantTables[i].TableId, employee.EmployeeId);
                    labels[i].Text = restaurantTables[i].TableId + ": " + employeeName;
                }

                pickers[i].SelectedItem = null;
            }        
        }
    }
}