using HQSOFT.Order.Samples;
using Xunit;

namespace HQSOFT.Order.EntityFrameworkCore.Domains;

[Collection(OrderTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<OrderEntityFrameworkCoreTestModule>
{

}
