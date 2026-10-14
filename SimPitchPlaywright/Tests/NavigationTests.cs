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
    public async Task pw_Nav_Should_Display_NavBar_Text()
    {
        await _mainPage.NavBar.AssertIfNavBarDisplayedAsync();

        Assert.That(await _mainPage.NavBar.GetLogoTextAsync(), Is.EqualTo("SimPitch ⚽"));
        Assert.That(await _mainPage.NavBar.GetHomeBtnTextAsync(), Is.EqualTo("Home"));
        Assert.That(await _mainPage.NavBar.GetPrepareSimulationBtnTextAsync(), Is.EqualTo("Prepare a new simulation"));
        Assert.That(await _mainPage.NavBar.GetAllSimulationsBtnTextAsync(), Is.EqualTo("All simulations"));
        Assert.That(await _mainPage.NavBar.GetAboutBtnTextAsync(), Is.EqualTo("About"));
        Assert.That(await _mainPage.NavBar.GetAllMatchesBtnTextAsync(), Is.EqualTo("Matches and results"));
    }

    [Test]
    public async Task pw_Nav_Should_Each_Element_Work()
    {
        await _mainPage.NavBar.AssertIfNavBarDisplayedAsync();

        var prepPage = await _mainPage.NavBar.GoToPrepareSimulationPageAsync();
        await prepPage.AssertIfDisplayedAsync();

        var allSimulationsPage = await _mainPage.NavBar.GoToAllSimulationsPageAsync();
        await allSimulationsPage.AssertIfDisplayedAsync();

        var aboutPage = await _mainPage.NavBar.GoToAboutPageAsync();
        await aboutPage.AssertIfDisplayedAsync();

        var leagueOverviewsPage = await _mainPage.NavBar.GoToLeagueOverviewsPageAsync();
        await leagueOverviewsPage.AssertIfDisplayedAsync();

        var mainPage = await _mainPage.NavBar.GoToMainPageAsync();
        await mainPage.AssertIfDisplayedAsync();
    }
}
