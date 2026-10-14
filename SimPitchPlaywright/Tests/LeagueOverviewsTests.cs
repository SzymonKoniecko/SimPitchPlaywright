using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class LeagueOverviewsTests : BaseTest
{
    private LeagueOverviewsPage _leagueOverviewsPage = default!;

    [SetUp]
    public async Task Init()
    {
        var mainPage = await new MainPage(Page).OpenAsync();
        _leagueOverviewsPage = await mainPage.NavBar.GoToLeagueOverviewsPageAsync();
    }

    [Test]
    public async Task pw_LeagueOverviews_Should_Navigate_Via_NavBar()
    {
        await _leagueOverviewsPage.AssertIfDisplayedAsync();
    }

    [Test]
    public async Task pw_LeagueOverviews_Should_Select_League_And_Display_Rounds()
    {
        await _leagueOverviewsPage.AssertIfDisplayedAsync();

        // Default season is Current Season. Select PKO BP Ekstraklasa.
        await _leagueOverviewsPage.SelectLeagueAsync("pko-bp-ekstraklasa");

        // Now it should display rounds or "Loading rounds..."
        await _leagueOverviewsPage.WaitForTextAsync("Scoreboard");
    }

    [Test]
    public async Task pw_LeagueOverviews_Should_Display_Scoreboard_For_Past_Season()
    {
        await _leagueOverviewsPage.AssertIfDisplayedAsync();

        await _leagueOverviewsPage.SelectSeasonAsync("2024/2025");
        await _leagueOverviewsPage.SelectLeagueAsync("pko-bp-ekstraklasa");

        // Assert the complete details scoreboard shows up
        await _leagueOverviewsPage.AssertScoreboardDisplayedAsync();
    }
}
