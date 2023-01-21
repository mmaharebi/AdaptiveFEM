using System;

namespace AdaptiveFEM.Commands.ViewerCommands.MeshViewerCommands
{
    public class UpdateMeshItems : CommandBase
    {
        private Action _updateMeshItems;

        public UpdateMeshItems(Action updateMeshItems)
        {
            _updateMeshItems = updateMeshItems;
        }

        public override void Execute(object? parameter)
        {
            _updateMeshItems();
        }
    }
}
