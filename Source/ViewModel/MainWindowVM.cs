using System.Windows.Controls;
using AmiiboAPIProject.Model;
using AmiiboAPIProject.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AmiiboAPIProject.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        public MainWindowVM()
        {
            //_currentPage = MainPage;
        }

        private MainMenuPage _menuPage;

        public MainMenuPage MenuPage
        {
            get
            {
                if (_menuPage == null)
                    _menuPage = new MainMenuPage();

                return _menuPage;
            }

        }

        private DetailPage _detailPage;

        public DetailPage DetailPage
        {
            get
            {
                if (_detailPage == null)
                    _detailPage = new DetailPage();

                return _detailPage;
            }
        }

        private Page _currentPage;

        public Page CurrentPage
        {
            get
            {
                if (_currentPage == null)
                    _currentPage = MenuPage;

                return _currentPage;
            }

            set { _currentPage = value; }
        }

        private string _buttonText;

        public string ButtonText
        {
            get
            {
                if (CurrentPage is MainMenuPage)
                    return "SHOW DETAILS";
                else
                    return "GO BACK";
            }
        }

        private RelayCommand _switchPageCommand;

        public RelayCommand SwitchPageCommand
        {
            get
            {
                if (_switchPageCommand == null)
                    _switchPageCommand = new RelayCommand(SwitchPage);

                return _switchPageCommand;
            }
        }

        public void SwitchPage()
        {
            if (CurrentPage is MainMenuPage)
            {
                Amiibo amiibo = (MenuPage.DataContext as MainMenuPageVM).SelectedAmiibo;
                if (amiibo == null || amiibo.Name == "Loading...") return;

                (DetailPage.DataContext as DetailPageVM).CurrentAmiibo = amiibo;

                CurrentPage = DetailPage;
            }
            else
            {
                CurrentPage = MenuPage;
            }

            RaisePropertyChanged("CurrentPage");
            RaisePropertyChanged("ButtonText");
            RaisePropertyChanged("ButtonPosGridIdx");
            RaisePropertyChanged("ButtonColor");
        }

        private int _buttonPosGridIdx;

        public int ButtonPosGridIdx
        {
            get
            {
                if (CurrentPage is MainMenuPage)
                    return 1;
                else
                    return 0;
            }
        }


        private string _buttonColor;

        public string ButtonColor
        {
            get
            {
                if (CurrentPage is MainMenuPage)
                    return "#dddddd";
                else
                    return "#faf1e4";
            }
        }
    }
}
