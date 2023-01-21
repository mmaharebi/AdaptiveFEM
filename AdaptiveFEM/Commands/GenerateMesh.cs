using AdaptiveFEM.Models;
using System.Runtime.CompilerServices;

namespace AdaptiveFEM.Commands
{
    public class GenerateMesh : CommandBase
    {
        private bool _isExecuting;

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                OnCanExecuteChanged();
            }
        }

        private readonly Design _design;

        public GenerateMesh(Design design)
        {
            _design = design;
            _design.ComponentAdded += OnComponentAdded;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) &&
                _design.Model.Domain != null &&
                !IsExecuting;
        }

        private void OnComponentAdded(object? sender, Component e)
        {
            OnCanExecuteChanged();
        }

        public override void Execute(object? parameter)
        {
            IsExecuting = true;
            _design.Solution.GenerateMesh();
            IsExecuting = false;
        }
    }
}
