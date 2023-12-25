using System.Windows.Media.Imaging;

namespace AdaptiveFEM.ViewModels
{
    public class ChartVM : ViewModelBase
    {
        private BitmapImage _bitmapImage;

        public BitmapImage BitmapImage
        {
            get => _bitmapImage;
            set
            {
                _bitmapImage = value;
                OnPropertyChanged(nameof(BitmapImage));
            }
        }

        public ChartVM()
        {
            _bitmapImage = new BitmapImage();
        }

    }
}
