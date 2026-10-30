using System.Globalization;
using MyShop.PL.Extensions;

namespace MyShop.UnitTests.Extensions;

[Trait("Category", "Unit")]
public sealed class CurrencyFormatterTests
{
    [Theory]
    [InlineData(1234.5, "EGP 1,234.50")]
    [InlineData(0, "EGP 0.00")]
    [InlineData(-12.3, "EGP -12.30")]
    public void ToEgp_FormatsAmountUsingInvariantSeparators(
        decimal amount,
        string expected)
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var result = CurrencyFormatter.ToEgp(amount);

            Assert.Equal(expected, result);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
