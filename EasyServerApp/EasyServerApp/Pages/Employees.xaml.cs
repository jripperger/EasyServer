using DocumentFormat.OpenXml.Drawing.Charts;
using EasyServerApp.EasyServerDB;

namespace EasyServerApp.Pages;

public partial class Employees : ContentView
{
    private EasyServerRepository easyServerRepository;
    private List<Employee> employees;

    private List<Label> labels;
    private List<Picker> pickers;

    public Employees(EasyServerRepository easyServerRepository)
	{
		InitializeComponent();

        this.easyServerRepository = easyServerRepository;
        employees = easyServerRepository.GetEmployeeList();

        labels = new List<Label>();
        pickers = new List<Picker>();

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
            var label = new Label();

            int employeeID = employees[i].EmployeeId;
            Employee employee = easyServerRepository.GetEmployeeById(employeeID);

            label.Text = employee.FirstName.Trim() + " " + employee.LastName.Trim() + " [" + employeeID + "]: " + employee.Role;
            labels.Add(label);

            EmployeesGrid.Add(label);
            EmployeesGrid.SetRow(label, rowIndex);
            EmployeesGrid.SetColumn(label, columnIndex);

            var picker = new Picker
            {
                Title = "Choose a role: "
            };

            picker.Items.Add("Server");
            picker.Items.Add("Manager");

            pickers.Add(picker);
            EmployeesGrid.Add(picker);
            EmployeesGrid.SetRow(picker, rowIndex + 1);
            EmployeesGrid.SetColumn(picker, columnIndex);

            columnIndex++;

            if (columnIndex == columnCount)
            {
                columnIndex = 0;

                rowIndex += 2;
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

        int rows = ((int)Math.Ceiling((double)employees.Count / EmployeesGrid.ColumnDefinitions.Count) * 2);

        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDefinition = new();
            EmployeesGrid.AddRowDefinition(rowDefinition);

            if (i % 2 != 0)
            {
                EmployeesGrid.RowDefinitions[i].Height = 100;
            }
        }
    }

    private void SaveServers(object sender, System.EventArgs e)
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            if (pickers[i].SelectedItem != null && pickers[i].SelectedItem.ToString() != employees[i].Role.Trim())
            {
                easyServerRepository.UpdateEmployeeRole(employees[i].EmployeeId, pickers[i].SelectedItem.ToString());
                labels[i].Text = employees[i].FirstName.Trim() + " " + employees[i].LastName.Trim() + " [" + employees[i].EmployeeId + "]: " + pickers[i].SelectedItem.ToString();
            }

            pickers[i].SelectedItem = null;
        }
    }
}