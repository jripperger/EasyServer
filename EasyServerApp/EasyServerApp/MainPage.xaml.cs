using EasyServerApp.EasyServerDB;
using EasyServerApp.Pages;
using System.Collections;

namespace EasyServerApp;

public partial class MainPage : ContentPage
{
    private EasyServerRepository _easyServerRepository;
    public EasyServerRepository easyServerRepository { get { return _easyServerRepository; }  }

    private Hashtable requestServiceStates;

    public MainPage()
	{
		InitializeComponent();
        CreateAPI();

        requestServiceStates= new Hashtable();

        List<RestaurantTable> tables = easyServerRepository.RestaurantTables;

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

