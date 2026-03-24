using Xunit;

namespace HQSOFT.Order.EntityFrameworkCore;

[CollectionDefinition(OrderTestConsts.CollectionDefinitionName)]
public class OrderEntityFrameworkCoreCollection : ICollectionFixture<OrderEntityFrameworkCoreFixture>
{

}
