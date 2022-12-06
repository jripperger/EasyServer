using EasyServerApp.EasyServerDB;
using EasyServerApp.Pages;
using System.Collections;

namespace EasyServerApp;

public partial class MainPage : ContentPage
{
    private EasyServerRepository _easyServerRepository;  // "API"
    private Hashtable requestServiceStates;              // States for whether or not customers are awaiting service at each table       

    public EasyServerRepository easyServerRepository { get { return _easyServerRepository; }  }

    public MainPage()
	{
		InitializeComponent();

        // Create a singleton of the pseudo API
        CreateAPI();

        requestServiceStates = new Hashtable();

        List<RestaurantTable> tables = easyServerRepository.RestaurantTables;

        // Add a service request state entry for each table to the hashtable
        for (int i = 0; i < tables.Count; i++)
        {
            requestServiceStates[tables[i].TableId] = false;
        }

        MainFrame.Content = new Login(requestServiceStates, easyServerRepository);
    }

    private EasyServerRepository CreateAPI()
    {
        if (_easyServerRepository == null)
        {
            _easyServerRepository = new EasyServerRepository();
        }

        return _easyServerRepository;
    }
}

