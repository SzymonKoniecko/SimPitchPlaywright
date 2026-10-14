using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class AboutPage : BasePage
{
    protected readonly ILocator Title;

    public AboutPage(IPage page) : base(page)
    {
        Title = ByTestId("title-about");
    }

    internal async Task AssertIfDisplayedAsync()
    {
        await Expect(Title).ToBeVisibleAsync();
    }
}
