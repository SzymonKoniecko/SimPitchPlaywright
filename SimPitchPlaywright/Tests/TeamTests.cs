using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class TeamTests : BaseTest
{
    private LeagueOverviewsPage _leagueOverviewsPage = default!;

    [SetUp]
    public async Task Init()
    {
        var mainPage = await new MainPage(Page).OpenAsync();
        _leagueOverviewsPage = await mainPage.NavBar.GoToLeagueOverviewsPageAsync();
    }

    [Test]
    public async Task pw_Team_Should_Display_Details()
    {
        // Navigate to League Overviews, pick a league, and click the first team card
        await _leagueOverviewsPage.AssertIfDisplayedAsync();
        await _leagueOverviewsPage.SelectSeasonAsync("2023/2024");
        await _leagueOverviewsPage.SelectLeagueAsync("pko-bp-ekstraklasa");
        await _leagueOverviewsPage.AssertScoreboardDisplayedAsync();

        var teamPage = await _leagueOverviewsPage.GoToFirstTeamAsync();

        // Assert team details
        await teamPage.AssertIfDisplayedAsync();
        await teamPage.AssertDetailsDisplayedAsync();
    }

    [Test]
    public async Task pw_Team_Should_Navigate_Back()
    {
        await _leagueOverviewsPage.AssertIfDisplayedAsync();
        await _leagueOverviewsPage.SelectSeasonAsync("2023/2024");
        await _leagueOverviewsPage.SelectLeagueAsync("pko-bp-ekstraklasa");
        await _leagueOverviewsPage.AssertScoreboardDisplayedAsync();

        var teamPage = await _leagueOverviewsPage.GoToFirstTeamAsync();
        await teamPage.AssertIfDisplayedAsync();

        _leagueOverviewsPage = await teamPage.NavBar.GoToLeagueOverviewsPageAsync();

        await _leagueOverviewsPage.AssertIfDisplayedAsync();
    }
}
