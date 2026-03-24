using HQSOFT.Order.Localization;
using Volo.Abp.AspNetCore.Components;

namespace HQSOFT.Order.Blazor;

public abstract class OrderComponentBase : AbpComponentBase
{
    protected OrderComponentBase()
    {
        LocalizationResource = typeof(OrderResource);
    }
}
