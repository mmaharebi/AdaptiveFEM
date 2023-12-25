using AdaptiveFEM.Commands;
using AdaptiveFEM.Models;
using System.Windows.Input;

namespace AdaptiveFEM.ViewModels
{
    public class TabResultVM : ViewModelBase
    {
        private bool _potentialChart;

        public bool PotentialChart
        {
            get => _potentialChart;
            set
            {
                _potentialChart = value;
                OnPropertyChanged(nameof(PotentialChart));
            }
        }

        private bool _electricFieldChart;

        public bool ElectricFieldChart
        {
            get => _electricFieldChart;
            set
            {
                _electricFieldChart = value;
                OnPropertyChanged(nameof(ElectricFieldChart));
            }
        }

        public ICommand DrawCharts { get; }

        public TabResultVM(Design design, MainVM mainVM)
        {
            _potentialChart = false;
            _electricFieldChart = false;

            DrawCharts = new DrawCharts(this, design, mainVM);
        }

    }
}
