using AdaptiveFEM.ViewModels.ComponentProfiles;
using System;
using System.Windows;

namespace AdaptiveFEM.Views.ComponentProfiles
{
    /// <summary>
    /// Interaction logic for ProfileContainer.xaml
    /// </summary>
    public partial class ProfileContainer : Window
    {
        public ProfileContainer()
        {
            InitializeComponent();

            Loaded += ProfileContainer_Loaded;
        }

        private void ProfileContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileContainerVM vm)
                vm.CloseThis += (object? sender, EventArgs e) =>
                {
                    this.Close();
                };
        }
    }
}
