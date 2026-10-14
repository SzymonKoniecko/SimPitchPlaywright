using Microsoft.Playwright;

namespace SimPitchPlaywright.Pages;

public class MainPage : BasePage
{
    public MainPage(IPage page) : base(page)
    {
    }

    public async Task<MainPage> OpenAsync()
    {
        await Page.GotoAsync("/");
        return this;
    }
}
