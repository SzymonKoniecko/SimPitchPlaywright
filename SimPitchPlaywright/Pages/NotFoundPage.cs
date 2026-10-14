using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class NotFoundPage : BasePage
{
    private readonly ILocator _heading;

    public NotFoundPage(IPage page) : base(page)
    {
        _heading = Page.Locator("h1").First;
    }

    public async Task AssertIfDisplayedAsync()
    {
        await Expect(_heading).ToHaveTextAsync("NotFound");
    }
}
