using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class NavigationTests : BaseTest
{
    private MainPage _mainPage = default!;

    [SetUp]
    public async Task Init()
    {
        _mainPage = await new MainPage(Page).OpenAsync();
    }

    [Test]
    public async Task Nav_Should_Display_NavBar_Text()
    {
        var navBar = _mainPage.NavBar;

        await Expect(navBar.Logo).ToHaveTextAsync("SimPitch ⚽");
        await Expect(navBar.HomeBtn).ToHaveTextAsync("Home");
        await Expect(navBar.PrepareSimulationBtn).ToHaveTextAsync("Prepare a new simulation");
        await Expect(navBar.AllSimulationsBtn).ToHaveTextAsync("All simulations");
        await Expect(navBar.AboutBtn).ToHaveTextAsync("About");
        await Expect(navBar.AllMatchesBtn).ToHaveTextAsync("Matches and results");
    }
}
