using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class NotFoundTests : BaseTest
{
    [SetUp]
    public async Task Init()
    {
        await new MainPage(Page).OpenAsync();
    }

    [Test]
    public async Task pw_NotFound_Should_Display_For_Invalid_URL()
    {
        await Page.GotoAsync("/this-page-does-not-exist");

        var notFoundPage = new NotFoundPage(Page);
        await notFoundPage.AssertIfDisplayedAsync();
    }
}
