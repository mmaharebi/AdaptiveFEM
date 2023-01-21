using AdaptiveFEM.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels
{
    public class ComponentViewerVM : ViewerVMBase
    {
        public override ObservableCollection<Component> Items { get; set; }

        public ComponentViewerVM(Design design) : base(design)
        {
            Items = new ObservableCollection<Component>();
            design.ComponentAdded += OnComponentAdded;
        }

        private void OnComponentAdded(object? sender, Component e)
        {
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform(1, -1));
            transform.Children
                .Add(new TranslateTransform(coordinateCenterPosition.X,
                coordinateCenterPosition.Y));

            e.Geometry.Transform = transform;

            // Get insert of center circle
            int index = Items.IndexOf(Items.Where(i => i.Name == centerCircleName).First());

            Items.Insert(index, e);

        }
    }
}
