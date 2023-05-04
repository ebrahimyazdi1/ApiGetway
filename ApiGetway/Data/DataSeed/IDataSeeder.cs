using System.Threading.Tasks;

namespace Gss.ApiGateway.Data.DataSeed
{
    public interface IDataSeeder
    {
        Task SeedData();
    }
}
