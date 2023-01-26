using AdaptiveFEM.Models;

namespace AdaptiveFEM.Commands
{
    public class Solve : CommandBase
    {
        private readonly Design _design;

        public Solve(Design design)
        {
            _design = design;
            _design.DesignChanged += OnDesignChanged;
        }

        private void OnDesignChanged(object? sender, System.EventArgs e)
        {
            OnCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) &&
                _design.Model.Domain != null;
        }

        public override void Execute(object? parameter)
        {
            _design.Solution.Solve();
        }
    }
}
