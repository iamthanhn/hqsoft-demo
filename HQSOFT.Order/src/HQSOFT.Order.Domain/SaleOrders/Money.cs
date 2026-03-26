using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace HQSOFT.Order.SaleOrders;

/// <summary>
/// Đại diện cho một số tiền với đơn vị tiền tệ.
/// </summary>
public class Money : ValueObject
{
    /// <summary>
    /// Số tiền
    /// </summary>
    public decimal Amount { get; protected set; }

    /// <summary>
    /// Đơn vị tiền tệ, ví dụ: "VND", "USD"
    /// </summary>
    public string Currency { get; protected set; } = string.Empty;

    protected Money()
    {
    } // for ORM

    /// <summary>
    /// Khởi tạo một đối tượng Money mới với số tiền và loại tiền tệ.
    /// </summary>
    /// <param name="amount">Số tiền</param>
    /// <param name="currency">Loại tiền tệ, mặc định là "VND"</param>
    public Money(decimal amount, string currency = "VND")
    {
        Amount = amount;
        Currency = Check.NotNullOrWhiteSpace(currency, nameof(currency));
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch");

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch");

        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money a, decimal multiplier)
    {
        return new Money(a.Amount * multiplier, a.Currency);
    }

    public static Money operator /(Money a, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();

        return new Money(a.Amount / divisor, a.Currency);
    }


    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}