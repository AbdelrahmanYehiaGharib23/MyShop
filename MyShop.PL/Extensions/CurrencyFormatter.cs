using System.Globalization;

namespace MyShop.PL.Extensions;

public static class CurrencyFormatter
{
    public static string ToEgp(decimal amount) =>
        $"EGP {amount.ToString("N2", CultureInfo.InvariantCulture)}";
}
