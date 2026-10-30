using MyShop.IntegrationTests.Infrastructure;

namespace MyShop.IntegrationTests;

[Trait("Category", "Integration")]
public sealed class AccountPageTests : IClassFixture<MyShopWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AccountPageTests(MyShopWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginPage_ReturnsRenderedHtml()
    {
        using var response = await _client.GetAsync(
            "/Account/Login",
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);

        var html = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken);
        Assert.Contains("Login", html, StringComparison.OrdinalIgnoreCase);
    }
}
