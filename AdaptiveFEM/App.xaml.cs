using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.Stores;
using AdaptiveFEM.ViewModels;
using System.Windows;

namespace AdaptiveFEM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly MessageService _messageService;

        private readonly Design _design;

        private readonly MaterialStore _materialStore;

        public App()
        {
            _materialStore = new MaterialStore();
            _messageService = new MessageService();
            _design = new Design("AdaptiveFEM Design", _messageService);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow
            {
                DataContext = new MainVM(_design, _materialStore, _messageService)
            };

            MainWindow.Show();
        }
    }
}
