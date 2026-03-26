using HQSOFT.Order.IdentityUsers;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace HQSOFT.Order.EntityFrameworkCore;

public static class OrderEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OrderGlobalFeatureConfigurator.Configure();
        OrderModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string>(
                    UserConsts.DepartmentPropertyName,
                    (_, propertyBuilder) =>
                    {
                        propertyBuilder.HasDefaultValue("NON");
                        propertyBuilder.HasMaxLength(UserConsts.MaxDepartmentLength);
                    }
                );
        });
    }
}