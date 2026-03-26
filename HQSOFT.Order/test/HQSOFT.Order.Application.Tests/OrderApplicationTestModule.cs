using HQSOFT.Inventory.Integration;
using HQSOFT.Order.EntityFrameworkCore;
using HQSOFT.Order.SaleOrders;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Volo.Abp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace HQSOFT.Order;

[DependsOn(
    typeof(OrderApplicationModule),
    typeof(OrderDomainTestModule),
    typeof(OrderEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
)]
public class OrderApplicationTestModule : AbpModule
{
    private SqliteConnection? _sqliteConnection;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });

        Configure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });

        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        ConfigureInMemorySqlite(context.Services);

        var fakeInventory = new FakeInventoryIntegrationService();
        var fakeEventBus = new RecordingDistributedEventBus();
        var fakeJobManager = new RecordingBackgroundJobManager();

        context.Services.Replace(ServiceDescriptor.Singleton<IInventoryIntegrationService>(fakeInventory));
        context.Services.Replace(ServiceDescriptor.Singleton(fakeInventory));
        context.Services.Replace(ServiceDescriptor.Singleton<IDistributedEventBus>(fakeEventBus));
        context.Services.Replace(ServiceDescriptor.Singleton(fakeEventBus));
        context.Services.Replace(ServiceDescriptor.Singleton<IBackgroundJobManager>(fakeJobManager));
        context.Services.Replace(ServiceDescriptor.Singleton(fakeJobManager));
        context.Services.Replace(ServiceDescriptor.Singleton<IDataSeeder, NullDataSeeder>());
        context.Services.AddTransient<AutoCancelDraftSalesOrderJob>();
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection?.Dispose();
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(builder =>
            {
                builder.DbContextOptions.UseSqlite(_sqliteConnection);
            });
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new OrderDbContext(options);
        context.GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }

    private class NullDataSeeder : IDataSeeder
    {
        public Task SeedAsync(DataSeedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
