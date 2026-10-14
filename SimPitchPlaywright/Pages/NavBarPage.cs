using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class NavBarPage : BasePage
{
    protected readonly ILocator Logo;
    protected readonly ILocator HomeBtn;
    protected readonly ILocator PrepareSimulationBtn;
    protected readonly ILocator AllSimulationsBtn;
    protected readonly ILocator AboutBtn;
    protected readonly ILocator AllMatchesBtn;

    public NavBarPage(IPage page) : base(page)
    {
        Logo = ByClass("logo");
        HomeBtn = ByTestId("home-nav");
        PrepareSimulationBtn = ByTestId("prepareSimulation-nav");
        AllSimulationsBtn = ByTestId("simulations-nav");
        AboutBtn = ByTestId("about-nav");
        AllMatchesBtn = ByTestId("all-matches-nav");
    }

    internal async Task<MainPage> GoToMainPageAsync()
    {
        await HomeBtn.ClickAsync();
        return new MainPage(Page);
    }

    internal async Task<PrepareSimulationPage> GoToPrepareSimulationPageAsync()
    {
        await PrepareSimulationBtn.ClickAsync();
        return new PrepareSimulationPage(Page);
    }

    internal async Task<AllSimulationsPage> GoToAllSimulationsPageAsync()
    {
        await AllSimulationsBtn.ClickAsync();
        return new AllSimulationsPage(Page);
    }

    internal async Task<AboutPage> GoToAboutPageAsync()
    {
        await AboutBtn.ClickAsync();
        return new AboutPage(Page);
    }

    internal async Task<LeagueOverviewsPage> GoToLeagueOverviewsPageAsync()
    {
        await AllMatchesBtn.ClickAsync();
        return new LeagueOverviewsPage(Page);
    }

    internal async Task AssertIfNavBarDisplayedAsync()
    {
        await Expect(Logo).ToBeVisibleAsync();
        await Expect(HomeBtn).ToBeVisibleAsync();
        await Expect(PrepareSimulationBtn).ToBeVisibleAsync();
        await Expect(AllSimulationsBtn).ToBeVisibleAsync();
        await Expect(AboutBtn).ToBeVisibleAsync();
        await Expect(AllMatchesBtn).ToBeVisibleAsync();
    }

    internal Task<string> GetLogoTextAsync() => GetTextAsync(Logo);
    internal Task<string> GetHomeBtnTextAsync() => GetTextAsync(HomeBtn);
    internal Task<string> GetPrepareSimulationBtnTextAsync() => GetTextAsync(PrepareSimulationBtn);
    internal Task<string> GetAllSimulationsBtnTextAsync() => GetTextAsync(AllSimulationsBtn);
    internal Task<string> GetAboutBtnTextAsync() => GetTextAsync(AboutBtn);
    internal Task<string> GetAllMatchesBtnTextAsync() => GetTextAsync(AllMatchesBtn);
}
