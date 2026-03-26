using System;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderConfirmedEventHandler_Tests
{
    [Fact]
    public async Task HandleEventAsync_Should_Write_Mock_Email_Log()
    {
        var handler = new SalesOrderConfirmedEventHandler();
        var eto = new SalesOrderConfirmedEto
        {
            OrderId = Guid.NewGuid(),
            OrderNumber = "SO-20260326-001",
            TotalAmount = 1230000,
            Currency = "VND"
        };

        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            await handler.HandleEventAsync(eto);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = writer.ToString();
        output.ShouldContain("MOCK EMAIL");
        output.ShouldContain("SO-20260326-001");
    }
}
