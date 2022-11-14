using DocumentFormat.OpenXml.Drawing.Charts;
using EasyServerApp.EasyServerDB;
using System.Collections;

namespace EasyServerApp.Pages;

public partial class Employees : ContentView
{
    private EasyServerRepository easyServerRepository;
    private List<Employee> employees;
    private Hashtable queuePages;
    private Hashtable requestServicePages;
    private List<Label> labels;
    private List<Picker> pickers;
    private List<Button> buttons;

    public Employees(Hashtable queuePages, Hashtable requestServicePages, EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        employees = easyServerRepository.Employees;
        this.queuePages = queuePages;
        this.requestServicePages = requestServicePages;

        labels = new List<Label>();
        pickers = new List<Picker>();
        buttons = new List<Button>();

        GenerateGridContents();
    }

    private void GenerateGridContents()
    {
        GenerateGridLayout();

        int rowIndex = 0;

        int columnCount = EmployeesGrid.ColumnDefinitions.Count;
        int columnIndex = 0;

        for (int i = 0; i < employees.Count; i++)
        {
            var label = new Label
            {
                ClassId = employees[i].EmployeeId.ToString()
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
                ClassId = employees[i].EmployeeId.ToString()
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
                HeightRequest = 50,
                VerticalOptions = LayoutOptions.Start
            };

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

    private void GenerateGridLayout()
    {
        for (int i = 0; i < 6; i++)
        {
            ColumnDefinition columnDefinition = new();
            EmployeesGrid.AddColumnDefinition(columnDefinition);
        }

        int rows = ((int)Math.Ceiling((double)employees.Count / EmployeesGrid.ColumnDefinitions.Count) * 3);

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            EmployeesGrid.AddRowDefinition(rowDefinition);

            if (i % 3 != 0)
            {
                EmployeesGrid.RowDefinitions[i].Height = 100;
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

    private void DeleteEmployee(object sender, System.EventArgs e)
    {
        Button button = buttons.Find((Predicate<Button>)sender);
        int employeeID = int.Parse(button.ClassId);
        Employee removedEmployee = easyServerRepository.DeleteEmployeeRow(employeeID);

        queuePages.Remove(removedEmployee.EmployeeId);

        for (int i = 1; i <= requestServicePages.Count; i++)
        {
            if (((RequestService)requestServicePages[i]).TableServerID == employeeID)
            {
                ((RequestService)requestServicePages[i]).TableServerID = null;
                easyServerRepository.UpdateTableServer((int)((RequestService)requestServicePages[i]).TableServerID, null);
            }
        }

        EmployeesGrid.Clear();
        GenerateGridContents();
    }
}