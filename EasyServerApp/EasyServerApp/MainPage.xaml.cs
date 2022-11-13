using EasyServerApp.EasyServerDB;
using EasyServerApp.Pages;
using System.Collections;
using System.Data.SqlTypes;
using System.Xml.Linq;

namespace EasyServerApp;

public partial class MainPage : ContentPage
{
    public EasyServerRepository easyServerRepository { get { return _easyServerRepository; }  }
    private EasyServerRepository _easyServerRepository;

    private Hashtable _queuePages;
    public Hashtable QueuePages { get { return _queuePages; } }

    private Hashtable _requestServicePages;
    public Hashtable RequestServicePages { get { return _requestServicePages; } }


    public MainPage()
	{
		InitializeComponent();
        CreateAPI();
        CreatePages();

		MainFrame.Content = new Login(QueuePages, RequestServicePages, easyServerRepository);
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

        if (_queuePages == null)
        {
            _queuePages = new Hashtable();
        }

        if (_requestServicePages == null)
        {
            _requestServicePages = new Hashtable();
        }

        for (int i = 0; i< employees.Count; i++)
        {
            Employee formattedEmployee = new() {
                FirstName = employees[i].FirstName.Trim(),
                LastName = employees[i].LastName.Trim(),
                Username = employees[i].Username.Trim(),
                Password = employees[i].Password.Trim(),
                Role = employees[i].Role.Trim(),
                EmployeeId = employees[i].EmployeeId
            };

            List<RestaurantTable> serverQueue = easyServerRepository.GetServerQueue(formattedEmployee.EmployeeId);
            Pages.Queue queuePage = new(formattedEmployee, serverQueue, RequestServicePages, easyServerRepository);
            _queuePages.Add(formattedEmployee.EmployeeId, queuePage);
        }

        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].EmployeeId.HasValue)
            { 
                RequestService requestServicePage = new(tables[i], (Pages.Queue)QueuePages[tables[i].EmployeeId], easyServerRepository);
                _requestServicePages.Add(tables[i].TableId, requestServicePage);
            }
            else
            {
                RequestService requestServicePage = new(tables[i], null, easyServerRepository);
                _requestServicePages.Add(tables[i].TableId, requestServicePage);
            }                    
        }
    }
}

