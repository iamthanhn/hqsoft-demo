using HQSOFT.Order.Samples;
using Xunit;

namespace HQSOFT.Order.EntityFrameworkCore.Applications;

[Collection(OrderTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<OrderEntityFrameworkCoreTestModule>
{

}
