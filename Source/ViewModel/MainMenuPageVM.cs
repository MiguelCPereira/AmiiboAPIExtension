using System.Collections.Generic;
using System.Net.NetworkInformation;
using AmiiboAPIProject.Model;
using AmiiboAPIProject.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using static System.Globalization.CultureInfo;

namespace AmiiboAPIProject.ViewModel
{
    class MainMenuPageVM : ViewModelBase
    {
        public IAmiiboRepository Repository { get; set; }
        public AmiiboLocalRepository LocalRepository { get; set; } = new AmiiboLocalRepository();
        public AmiiboApiRepository ApiRepository { get; set; } = new AmiiboApiRepository();


        public List<Amiibo> AmiiboList { get; set; }
        public List<string> FranchiseList { get; set; }


        public Amiibo SelectedAmiibo { get; set; }
        private string _selectedFranchise;
        public string SelectedFranchise
        {
            get { return _selectedFranchise; }
            set
            {
                _selectedFranchise = value;
                GetAmiibosByFranchiseAsync(value);
            }
        }

        public string SearchedName { get; set; }


        public MainMenuPageVM()
        {
            InitializeListsAsync();
        }


        private async void GetAmiibosByFranchiseAsync(string selectedFranchise)
        {
            // Check if the internet connection is still up (and show/hide the error message accordingly)
            if (InternetConnectionOn() == false)
            {
                // If it's not and the Api repository is in use, switch to local
                if (Repository == ApiRepository)
                {
                    Repository = LocalRepository;
                    RaisePropertyChanged("Repository");
                    RaisePropertyChanged("RepositoryCommandText");
                }
            }

            // Setting 'loading' message
            var loadingAmiiboList = new List<Amiibo>();
            loadingAmiiboList.Add(new Amiibo { Name = "Loading..." });
            AmiiboList = loadingAmiiboList;
            RaisePropertyChanged("AmiiboList");

            // Loading the actual filtered list
            AmiiboList = await Repository.GetAmiibosByFranchiseAsync(selectedFranchise);
            RaisePropertyChanged("AmiiboList");
        }

        private async void InitializeListsAsync()
        {
            // Check if there's an internet connection and define the starter repository
            // (and show/hide the error message accordingly)
            if (InternetConnectionOn())
                Repository = ApiRepository;
            else
                Repository = LocalRepository;

            RaisePropertyChanged("Repository");
            RaisePropertyChanged("RepositoryCommandText");


            // Setting 'loading' messages
            var loadingAmiiboList = new List<Amiibo>();
            loadingAmiiboList.Add(new Amiibo{Name="Loading..."});
            var loadingFranchiseList = new List<string>();
            loadingFranchiseList.Add("Loading...");
            AmiiboList = loadingAmiiboList;
            FranchiseList = loadingFranchiseList;
            RaisePropertyChanged("AmiiboList");
            RaisePropertyChanged("FranchiseList");


            // Loading the actual information
            AmiiboList = await Repository.GetAllAmiibosAsync();
            FranchiseList = await Repository.GetFranchisesAsync();
            FranchiseList.Insert(0, "ALL FRANCHISES");
            RaisePropertyChanged("AmiiboList");
            RaisePropertyChanged("FranchiseList");
        }

        private string _errorVisibility;
        public string ErrorVisibility
        {
            get
            {
                if (_errorVisibility == null)
                    _errorVisibility = "Hidden";
                return _errorVisibility;
            }
            set
            {
                _errorVisibility = value;
            }
        }

        private string _repositoryCommandText;
        public string RepositoryCommandText
        {
            get
            {
                if (Repository == LocalRepository)
                    return "Switch To Online Repository";
                else
                    return "Switch To Local Repository";

            }
        }

        private RelayCommand _switchRepositoryCommand;
        public RelayCommand SwitchRepositoryCommand
        {
            get
            {
                if (_switchRepositoryCommand == null)
                    _switchRepositoryCommand = new RelayCommand(SwitchRepository);

                return _switchRepositoryCommand;
            }
        }
        public void SwitchRepository()
        {
            // Check if the internet connection is still up (and show/hide the error message accordingly)
            // And only switch if the connection's confirmed
            if (InternetConnectionOn())
            {
                if (Repository == LocalRepository)
                    Repository = ApiRepository;
                else
                    Repository = LocalRepository;
            }
            else
            {
                //If it fails, make sure the repository in use is the local
                Repository = LocalRepository;
            }

            RaisePropertyChanged("Repository");
            RaisePropertyChanged("RepositoryCommandText");
        }



        private bool _nonFiguresHidden;

        public bool NonFiguresHidden
        {
            get { return _nonFiguresHidden; }
            set
            {
                _nonFiguresHidden = value;
                GetAmiibosByTypeAsync(value);
            }
        }

        private async void GetAmiibosByTypeAsync(bool nonFiguresHidden)
        {
            // Check if the internet connection is still up (and show/hide the error message accordingly)
            if (InternetConnectionOn() == false)
            {
                // If it's not and the Api repository is in use, switch to local
                if (Repository == ApiRepository)
                {
                    Repository = LocalRepository;
                    RaisePropertyChanged("Repository");
                    RaisePropertyChanged("RepositoryCommandText");
                }
            }

            // Setting 'loading' message
            var loadingAmiiboList = new List<Amiibo>();
            loadingAmiiboList.Add(new Amiibo { Name = "Loading..." });
            AmiiboList = loadingAmiiboList;
            RaisePropertyChanged("AmiiboList");

            // Loading the actual filtered list
            AmiiboList = await Repository.GetAmiibosByTypeAsync(nonFiguresHidden);
            RaisePropertyChanged("AmiiboList");
        }



        private string _typeCommandText;
        public string TypeCommandText
        {
            get
            {
                if (NonFiguresHidden)
                    return "Show Non-Figure Amiibos";
                else
                    return "Hide Non-Figure Amiibos";

            }
        }

        private RelayCommand _toggleTypeFilterCommand;
        public RelayCommand ToggleTypeFilterCommand
        {
            get
            {
                if (_toggleTypeFilterCommand == null)
                    _toggleTypeFilterCommand = new RelayCommand(ToggleTypeFilter);

                return _toggleTypeFilterCommand;
            }
        }
        public void ToggleTypeFilter()
        {
            NonFiguresHidden = !NonFiguresHidden;
            RaisePropertyChanged("TypeCommandText");
        }



        private RelayCommand _searchByNameCommand;
        public RelayCommand SearchByNameCommand
        {
            get
            {
                if (_searchByNameCommand == null)
                    _searchByNameCommand = new RelayCommand(SearchByName);

                return _searchByNameCommand;
            }
        }
        public async void SearchByName()
        {
            // Check if the internet connection is still up (and show/hide the error message accordingly)
            if (InternetConnectionOn() == false)
            {
                // If it's not and the Api repository is in use, switch to local
                if (Repository == ApiRepository)
                {
                    Repository = LocalRepository;
                    RaisePropertyChanged("Repository");
                    RaisePropertyChanged("RepositoryCommandText");
                }
            }

            // Setting 'loading' message
            var loadingAmiiboList = new List<Amiibo>();
            loadingAmiiboList.Add(new Amiibo { Name = "Loading..." });
            AmiiboList = loadingAmiiboList;
            RaisePropertyChanged("AmiiboList");

            // Loading the actual filtered list 
            string upperCasedName = GetCultureInfo("en-gb").TextInfo.ToTitleCase(SearchedName.ToLower());
            AmiiboList = await Repository.GetAmiibosByNameAsync(upperCasedName);
            RaisePropertyChanged("AmiiboList");
        }



        private RelayCommand _clearNameCommand;
        public RelayCommand ClearNameCommand
        {
            get
            {
                if (_clearNameCommand == null)
                    _clearNameCommand = new RelayCommand(ClearName);

                return _clearNameCommand;
            }
        }
        public async void ClearName()
        {
            // Check if the internet connection is still up (and show/hide the error message accordingly)
            if (InternetConnectionOn() == false)
            {
                // If it's not and the Api repository is in use, switch to local
                if (Repository == ApiRepository)
                {
                    Repository = LocalRepository;
                    RaisePropertyChanged("Repository");
                    RaisePropertyChanged("RepositoryCommandText");
                }
            }

            // Setting 'loading' message
            var loadingAmiiboList = new List<Amiibo>();
            loadingAmiiboList.Add(new Amiibo { Name = "Loading..." });
            AmiiboList = loadingAmiiboList;
            RaisePropertyChanged("AmiiboList");

            // Loading the actual filtered list 
            AmiiboList = await Repository.ClearNameAsync();
            RaisePropertyChanged("AmiiboList");
        }

        bool InternetConnectionOn()
        {
            // Ping google to test the internet connection
            // And hide/show the error message depending on the result

            try
            {
                if (new Ping().Send("www.google.com.mx").Status == IPStatus.Success)
                {
                    //Repository = ApiRepository;
                    ErrorVisibility = "Hidden";
                    RaisePropertyChanged("ErrorVisibility");
                }
                return true;
            }
            catch
            {
                //Repository = LocalRepository;
                ErrorVisibility = "Visible";
                RaisePropertyChanged("ErrorVisibility");
                return false;
            }
        }
    }
}
