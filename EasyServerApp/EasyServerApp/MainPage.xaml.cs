using EasyServerApp.EasyServerDB;
using EasyServerApp.Pages;
using System.Data.SqlTypes;

namespace EasyServerApp;

public partial class MainPage : ContentPage
{
    public EasyServerRepository easyServerRepository { get { return _easyServerRepository; }  }
    private EasyServerRepository _easyServerRepository;

    public MainPage()
	{
		InitializeComponent();
        CreateAPI();
		MainFrame.Content = new Login(easyServerRepository);
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

