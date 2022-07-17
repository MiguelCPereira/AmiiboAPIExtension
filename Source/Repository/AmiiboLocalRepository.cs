using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AmiiboAPIProject.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AmiiboAPIProject.Repository
{
    class AmiiboLocalRepository : IAmiiboRepository
    {
        public static List<Amiibo> AmiiboList { get; private set; }

        public static bool NonFiguresHidden { get; set; }

        public static string SearchedName { get; set; }

        private static string _selectedFranchise;
        public static string SelectedFranchise
        {
            get
            {
                if (_selectedFranchise == null)
                    _selectedFranchise = "ALL FRANCHISES";
                return _selectedFranchise;
            }
            set { _selectedFranchise = value; }
        }

        public async Task<List<Amiibo>> GetAllAmiibosAsync()
        {
            if (AmiiboList != null)
            {
                return AmiiboList;
            }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "AmiiboAPIProject.Resources.amiibos.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();

                    // Deserialize json object
                    var jObjectCard = JsonConvert.DeserializeObject<JObject>(json).SelectToken("amiibo");

                    // Register and return the amiibo list from object
                    AmiiboList = jObjectCard.ToObject<List<Amiibo>>();
                    return AmiiboList;
                }
            }
        }

        public async Task<List<string>> GetFranchisesAsync()
        {
            await GetAllAmiibosAsync();
            return AmiiboList.Select(amiibo => amiibo.Franchise).Distinct().ToList();
        }

        public async Task<List<Amiibo>> GetAmiibosByFranchiseAsync(string franchise)
        {
            SelectedFranchise = franchise;
            await GetAllAmiibosAsync();

            if (SearchedName == null)
            {
                if (NonFiguresHidden == false)
                {
                    if (SelectedFranchise.Equals("ALL FRANCHISES"))
                        return AmiiboList;
                    else
                        return AmiiboList.FindAll(x => x.Franchise.Equals(SelectedFranchise));
                }
                else
                {
                    if (SelectedFranchise.Equals("ALL FRANCHISES"))
                        return AmiiboList.FindAll(x => x.Type.Equals("Figure"));
                    else
                        return AmiiboList.FindAll(x => x.Franchise.Equals(SelectedFranchise) && x.Type.Equals("Figure"));
                }
            }
            else
            {
                if (NonFiguresHidden == false)
                {
                    if (SelectedFranchise.Equals("ALL FRANCHISES"))
                        return AmiiboList.FindAll(x => NameMatch(SearchedName, x.Name));
                    else
                        return AmiiboList.FindAll(x => x.Franchise.Equals(SelectedFranchise) && NameMatch(SearchedName, x.Name));
                }
                else
                {
                    if (SelectedFranchise.Equals("ALL FRANCHISES"))
                        return AmiiboList.FindAll(x => x.Type.Equals("Figure") && NameMatch(SearchedName, x.Name));
                    else
                        return (AmiiboList.FindAll(x => x.Franchise.Equals(SelectedFranchise) && x.Type.Equals("Figure") && NameMatch(SearchedName, x.Name)));
                }
            }
        }

        public async Task<List<Amiibo>> GetAmiibosByTypeAsync(bool nonFiguresHidden)
        {
            NonFiguresHidden = nonFiguresHidden;
            return await GetAmiibosByFranchiseAsync(SelectedFranchise);
        }

        public async Task<List<Amiibo>> GetAmiibosByNameAsync(string name)
        {
            SearchedName = name;
            return await GetAmiibosByFranchiseAsync(SelectedFranchise);
        }

        public async Task<List<Amiibo>> ClearNameAsync()
        {
            SearchedName = null;
            return await GetAmiibosByFranchiseAsync(SelectedFranchise);
        }

        private bool NameMatch(string searchedName, string amiiboName)
        {
            string pattern = $@".*{searchedName}.*";
            Regex rg = new Regex(pattern);
            return rg.IsMatch(amiiboName);
        }
    }
}
