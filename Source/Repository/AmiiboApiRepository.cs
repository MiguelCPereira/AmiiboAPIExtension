using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AmiiboAPIProject.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AmiiboAPIProject.Repository
{
    class AmiiboApiRepository : IAmiiboRepository
    {
        public static List<Amiibo> AmiiboList { get; private set; }

        public static bool NonFiguresHidden { get; set; }

        public static string SearchedName { get; set; }

        private static string _selectedFranchise;
        public static string SelectedFranchise
        {
            get
            {
                if(_selectedFranchise == null)
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

            string endpoint = "https://www.amiiboapi.com/api/amiibo/";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Request info
                    var response = await client.GetAsync(endpoint);
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(response.ReasonPhrase);

                    // Read json string
                    string json = await response.Content.ReadAsStringAsync();

                    // Deserialize json object
                    var jTokenAmiibo = JsonConvert.DeserializeObject<JObject>(json).SelectToken("amiibo");

                    // Register and return the amiibo list from object
                    AmiiboList = jTokenAmiibo.ToObject<List<Amiibo>>();
                    return AmiiboList;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public async Task<List<string>> GetFranchisesAsync()
        {
            //await GetAllAmiibosAsync();
            //return AmiiboList.Select(amiibo => amiibo.Franchise).Distinct().ToList();

            string endpoint = "https://www.amiiboapi.com/api/gameseries/";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Request info
                    var response = await client.GetAsync(endpoint);
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(response.ReasonPhrase);

                    // Read json string
                    string json = await response.Content.ReadAsStringAsync();

                    // Deserialize json object
                    var jTokenFranchise = JsonConvert.DeserializeObject<JObject>(json).SelectToken("amiibo");

                    // Make a string list from the franchises with unique names
                    // (As some of them have the same name but different key - but keys are irrelevant for this program)
                    var jTokenList = jTokenFranchise.ToObject<List<JObject>>().Select(x => x.SelectToken("name")).Distinct().ToList();
                    List<string> returnList = new List<string>();
                    foreach (var jToken in jTokenList)
                        returnList.Add(jToken.ToObject<string>());

                    // And return said list
                    return returnList;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
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
