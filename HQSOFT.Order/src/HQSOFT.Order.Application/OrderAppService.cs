using HQSOFT.Order.Localization;
using Volo.Abp.Application.Services;

namespace HQSOFT.Order;

/* Inherit your application services from this class.
 */
public abstract class OrderAppService : ApplicationService
{
    protected OrderAppService()
    {
        LocalizationResource = typeof(OrderResource);
    }
}
