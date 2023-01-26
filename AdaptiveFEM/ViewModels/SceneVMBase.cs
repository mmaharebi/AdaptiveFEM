using AdaptiveFEM.Commands;
using AdaptiveFEM.Commands.ViewerCommands;
using AdaptiveFEM.Models;
using System.Windows;
using System.Windows.Input;

namespace AdaptiveFEM.ViewModels
{
    public class SceneVMBase : ViewModelBase
    {
        #region View properties
        public double ViewWidth { get; protected set; }

        public double ViewHeight { get; protected set; }

        public double ZoomFactor { get; protected set; }
        #endregion

        #region Commands
        public ICommand ViewLoad { get; }

        public ICommand ViewSizeChange { get; }

        public ICommand Zoom { get; }

        public ICommand ButtonTranslate { get; }
        #endregion

        protected Point coordinateCenterPosition = new Point(0, 0);

        private readonly Design design;

        public SceneVMBase(Design design)
        {
            this.design = design;
            ZoomFactor = 1.0;

            //
            ViewLoad = new ViewLoad(OnViewLoaded);
            ViewSizeChange = new ViewSizeChange(OnViewSizeChanged);
            Zoom = new Zoom(OnZoom, ResetZoom);
            ButtonTranslate = new ButtonTranslate(OnTranslate);
        }

        protected virtual void OnViewLoaded(double viewWidth, double viewHeight)
        {
            coordinateCenterPosition = new Point(viewWidth / 2, viewHeight / 2);
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
        }

        protected virtual void OnViewSizeChanged(double viewWidth, double viewHeight)
        {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
        }

        protected virtual void OnTranslate(double deltaX, double deltaY)
        {
            coordinateCenterPosition.X -= deltaX;
            coordinateCenterPosition.Y -= deltaY;
        }

        private void OnZoom(double scaleFactor)
        {
            ZoomFactor *= scaleFactor;
            OnPropertyChanged(nameof(ZoomFactor));
        }

        private void ResetZoom()
        {
            ZoomFactor = 1;
            OnPropertyChanged(nameof(ZoomFactor));
        }
    }
}
