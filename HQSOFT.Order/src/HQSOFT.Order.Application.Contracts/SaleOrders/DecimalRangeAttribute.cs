using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace HQSOFT.Order.SaleOrders;

/// <summary>
/// A RangeAttribute replacement for decimal that uses InvariantCulture,
/// avoiding issues when server runs under non-US cultures (e.g. Vietnamese "0,01").
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class DecimalRangeAttribute : ValidationAttribute
{
    public decimal Min { get; }
    public decimal Max { get; }

    public DecimalRangeAttribute(double minimum, double maximum)
    {
        Min = (decimal)minimum;
        Max = (decimal)maximum;
    }

    public DecimalRangeAttribute(string minimum, string maximum)
    {
        Min = decimal.Parse(minimum, CultureInfo.InvariantCulture);
        Max = decimal.Parse(maximum, CultureInfo.InvariantCulture);
    }

    public override bool IsValid(object? value)
    {
        if (value == null) return true;
        if (value is not decimal d) return false;
        return d >= Min && d <= Max;
    }
}
