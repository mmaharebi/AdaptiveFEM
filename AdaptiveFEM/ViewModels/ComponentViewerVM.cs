using AdaptiveFEM.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveFEM.ViewModels
{
    public class ComponentViewerVM : ViewerVMBase
    {
        private ObservableCollection<Path> _geometries;

        public override ObservableCollection<Path> Items
        {
            get => _geometries;
            set
            {
                _geometries = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public ComponentViewerVM(Design design) : base(design)
        {
            _geometries = new ObservableCollection<Path>();

            design.DesignChanged += OnDesignChanged;
        }

        private void OnDesignChanged(object? sender, System.EventArgs e)
        {
            UpdateItems();
        }

        protected override void UpdateItems()
        {
            Items.Clear();

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(1, -1));
            transformGroup.Children.Add(new TranslateTransform(CoordinateCenter.X,
                CoordinateCenter.Y));

            if (design.Model.Domain != null)
            {
                Geometry dg = design.Model.Domain.Geometry;
                Path dp = geometryElements.DomainPath(dg);
                dp.RenderTransform = transformGroup;
                Items.Add(dp);
            }

            design.Model.Regions.ForEach(r =>
            {
                Geometry rg = r.Geometry;
                Path rp = geometryElements.RegionPath(rg);
                rp.RenderTransform = transformGroup;
                Items.Add(rp);
            });

            Items.Add(CoordinateCircle);
            Items.Add(XAxis);
            Items.Add(YAxis);

            OnPropertyChanged(nameof(Items));
        }
    }
}
