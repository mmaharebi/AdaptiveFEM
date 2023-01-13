using AdaptiveFEM.Models;
using AdaptiveFEM.Services;

namespace AdaptiveFEM.Commands
{
    public class ResetProject : CommandBase
    {
        private Design _design;
        private readonly MessageService _messageService;

        public ResetProject(Design design, MessageService messageService)
        {
            _design = design;
            _messageService = messageService;
        }

        public override void Execute(object? parameter)
        {
            bool canReset = _messageService.AskBoolean("Do you want to reset this project?");
            if (canReset)
                _design.Reset();
        }
    }
}
