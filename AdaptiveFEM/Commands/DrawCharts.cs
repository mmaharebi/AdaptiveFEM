using AdaptiveFEM.Models;
using AdaptiveFEM.Services;
using AdaptiveFEM.ViewModels;
using System.Windows.Media.Imaging;

namespace AdaptiveFEM.Commands
{
    public class DrawCharts : CommandBase
    {
        private readonly TabResultVM _tabResultVM;

        private readonly Design _design;
        private readonly MainVM _mainVM;

        public DrawCharts(TabResultVM tabResultVM, Design design, MainVM mainVM)
        {
            _tabResultVM = tabResultVM;
            _design = design;
            _mainVM = mainVM;

            _tabResultVM.PropertyChanged += OnViewModelPropertyChanged;
            _design.Solution.SolutionChanged += OnSolutionChanged;
        }

        private void OnSolutionChanged(object? sender, System.EventArgs e)
        {
            if (_design.Solution.IsPotentialValid)
                OnCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) &&
                (_tabResultVM.PotentialChart || _tabResultVM.ElectricFieldChart) &&
                _design.Solution.IsPotentialValid;
        }

        public override void Execute(object? parameter)
        {

            if (_mainVM.ChartVM is ChartVM vm)
            {
                Chart chart = new();
                BitmapImage bitmapImage = new();

                if (_tabResultVM.PotentialChart)
                {
                    bitmapImage = chart.ContourPlot(_design.Solution.Potential,
                    _design.Solution.PotentialXSize,
                    _design.Solution.PotentialYSize);
                }

                if (_tabResultVM.ElectricFieldChart)
                {
                    bitmapImage = chart.ElectricFieldPlot(_design.Solution.Potential,
                    _design.Solution.PotentialXSize,
                    _design.Solution.PotentialYSize);
                }

                vm.BitmapImage = bitmapImage;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabResultVM.PotentialChart) ||
                e.PropertyName == nameof(TabResultVM.ElectricFieldChart))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
