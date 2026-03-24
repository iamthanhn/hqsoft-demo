using System.Threading.Tasks;

namespace HQSOFT.Order.Data;

public interface IOrderDbSchemaMigrator
{
    Task MigrateAsync();
}
