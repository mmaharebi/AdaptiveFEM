using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using System.Windows;

namespace AdaptiveFEM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Design _design;

        private readonly MessageService _messageService;

        public App()
        {
            _messageService = new MessageService();
            _design = new Design("AdaptiveFEM Design", _messageService);
        }
    }
}
