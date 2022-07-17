using System.Collections.Generic;
using AmiiboAPIProject.Model;
using GalaSoft.MvvmLight;

namespace AmiiboAPIProject.ViewModel
{
    class DetailPageVM : ViewModelBase
    {
        private Amiibo _currentAmiibo;
        public Amiibo CurrentAmiibo
        {
            get
            {
                if (_currentAmiibo == null)
                {
                    _currentAmiibo = new Amiibo() { AmiiboSeries = "Amiibo Series", Character = "Character", Franchise = "Franchise",
                        ImageUrl = "https://pixelartmaker-data-78746291193.nyc3.digitaloceanspaces.com/image/ef680c30a573972.png",
                        Name = "Name", ReleaseDates = new List<string>{"Date 01", "Date 02", "Date 03", "Date 04"}, Type = "Type"};
                }

                return _currentAmiibo;
            }
            set
            {
                _currentAmiibo = value;
                RaisePropertyChanged("CurrentAmiibo");
            }
        }
    }
}
