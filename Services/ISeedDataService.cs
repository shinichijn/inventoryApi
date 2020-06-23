using InventoryApi.Repositories;
using System.Threading.Tasks;

namespace InventoryApi.Services
{
    public interface ISeedDataService
    {
        Task Initialize(InventoryDbContext context);
    }
}
