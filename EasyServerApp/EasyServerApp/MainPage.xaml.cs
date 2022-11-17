using EasyServerApp.EasyServerDB;
using EasyServerApp.Pages;
using System.Collections;

namespace EasyServerApp;

public partial class MainPage : ContentPage
{
    private EasyServerRepository _easyServerRepository;
    public EasyServerRepository easyServerRepository { get { return _easyServerRepository; }  }

    private HashSet<Pages.Queue> queuePages;
    private HashSet<RequestService> requestServicePages;

    public MainPage()
	{
		InitializeComponent();
        CreateAPI();
        CreatePages();

        List<RestaurantTable> tables = easyServerRepository.RestaurantTables;

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
            queuePages = new HashSet<Pages.Queue>();
        }

        if (requestServicePages == null)
        {
            requestServicePages = new HashSet<RequestService>();
        }

        for (int i = 0; i< employees.Count; i++)
        {
            Employee formattedEmployee = employees[i];
            formattedEmployee.FirstName = formattedEmployee.FirstName.Trim();
            formattedEmployee.LastName = formattedEmployee.LastName.Trim();
            formattedEmployee.Username = formattedEmployee.Username.Trim();
            formattedEmployee.Password = formattedEmployee.Password.Trim();
            formattedEmployee.Role = formattedEmployee.Role.Trim();

            ServerQueue serverQueue = easyServerRepository.GetServerQueue(formattedEmployee.EmployeeId);
            Pages.Queue queuePage = new(formattedEmployee, serverQueue, requestServicePages);
            queuePages.Add(queuePage);
        }

        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].EmployeeId.HasValue)
            {
                Pages.Queue queuePage = queuePages.Where(x => x.Employee.EmployeeId == tables[i].EmployeeId).FirstOrDefault();
                RequestService requestServicePage = new(tables[i], queuePage, easyServerRepository);
                requestServicePages.Add(requestServicePage);
            }
            else
            {
                RequestService requestServicePage = new(tables[i], null, easyServerRepository);
                requestServicePages.Add(requestServicePage);
            }                    
        }
    }
}

