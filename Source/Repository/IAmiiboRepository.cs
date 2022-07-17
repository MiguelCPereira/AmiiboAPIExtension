using System.Collections.Generic;
using System.Threading.Tasks;
using AmiiboAPIProject.Model;

namespace AmiiboAPIProject.Repository
{
    interface IAmiiboRepository
    {
        Task<List<Amiibo>> GetAllAmiibosAsync();
        Task<List<Amiibo>> GetAmiibosByFranchiseAsync(string selectedFranchise);
        Task<List<Amiibo>> GetAmiibosByTypeAsync(bool nonFiguresHidden);
        Task<List<Amiibo>> GetAmiibosByNameAsync(string name);
        Task<List<Amiibo>> ClearNameAsync();
        Task<List<string>> GetFranchisesAsync();
    }
}
