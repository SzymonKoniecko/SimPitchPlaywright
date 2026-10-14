using Microsoft.Playwright;

namespace SimPitchPlaywright.Pages;

public class NavBarPage
{
    private readonly IPage _page;

    public NavBarPage(IPage page)
    {
        _page = page;
    }

    public ILocator Logo => _page.Locator(".logo");
    public ILocator HomeBtn => _page.GetByTestId("home-nav");
    public ILocator PrepareSimulationBtn => _page.GetByTestId("prepareSimulation-nav");
    public ILocator AllSimulationsBtn => _page.GetByTestId("simulations-nav");
    public ILocator AboutBtn => _page.GetByTestId("about-nav");
    public ILocator AllMatchesBtn => _page.GetByTestId("all-matches-nav");
}
