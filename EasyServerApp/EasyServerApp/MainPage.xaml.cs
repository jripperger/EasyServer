using EasyServerApp.EasyServerDB;
using EasyServerApp.Pages;
using System.Collections;

namespace EasyServerApp;

public partial class MainPage : ContentPage
{
    private EasyServerRepository _easyServerRepository;
    public EasyServerRepository easyServerRepository { get { return _easyServerRepository; }  }

    private Hashtable queuePages;
    private Hashtable requestServicePages;

    public MainPage()
	{
		InitializeComponent();
        CreateAPI();
        CreatePages();

		MainFrame.Content = new Login(queuePages, requestServicePages, easyServerRepository);
    }

    private EasyServerRepository CreateAPI()
    {
        if (_easyServerRepository == null)
        {
            _easyServerRepository = new EasyServerRepository();
        }

        return _easyServerRepository;
    }

    private void CreatePages()
    {
        List<Employee> employees = easyServerRepository.Employees;
        List<RestaurantTable> tables = easyServerRepository.RestaurantTables;

        if (queuePages == null)
        {
            queuePages = new Hashtable();
        }

        if (requestServicePages == null)
        {
            requestServicePages = new Hashtable();
        }

        for (int i = 0; i< employees.Count; i++)
        {
            Employee formattedEmployee = employees[i];
            formattedEmployee.FirstName = formattedEmployee.FirstName.Trim();
            formattedEmployee.LastName = formattedEmployee.LastName.Trim();
            formattedEmployee.Username = formattedEmployee.Username.Trim();
            formattedEmployee.Password = formattedEmployee.Password.Trim();
            formattedEmployee.Role = formattedEmployee.Role.Trim();

            List<RestaurantTable> serverQueue = easyServerRepository.GetServerQueue(formattedEmployee.EmployeeId);
            Pages.Queue queuePage = new(formattedEmployee, serverQueue, requestServicePages);
            queuePages.Add(employees[i].EmployeeId, queuePage);
        }

        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].EmployeeId.HasValue)
            { 
                RequestService requestServicePage = new(tables[i], (Pages.Queue)queuePages[tables[i].EmployeeId], easyServerRepository);
                requestServicePages.Add(tables[i].TableId, requestServicePage);
            }
            else
            {
                RequestService requestServicePage = new(tables[i], null, easyServerRepository);
                requestServicePages.Add(tables[i].TableId, requestServicePage);
            }                    
        }
    }
}

