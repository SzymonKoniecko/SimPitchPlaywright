using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class LeagueOverviewsPage : BasePage
{
    private readonly ILocator _seasonSelect;
    private readonly ILocator _leagueSelect;
    private readonly ILocator _scoreboardCompleteDetails;

    public LeagueOverviewsPage(IPage page) : base(page)
    {
        _seasonSelect = ByTestId("season-select");
        _leagueSelect = ByTestId("league-select");
        _scoreboardCompleteDetails = ByTestId("scoreboard_complete_details").First;
    }

    public async Task AssertIfDisplayedAsync()
    {
        await Expect(_seasonSelect).ToBeVisibleAsync();
    }

    public async Task SelectSeasonAsync(string seasonYear)
    {
        await SelectFromDropdownAsync(_seasonSelect, seasonYear);
    }

    public async Task SelectLeagueAsync(string leagueNameId)
    {
        await SelectFromDropdownAsync(_leagueSelect, leagueNameId);
    }

    public async Task AssertScoreboardDisplayedAsync()
    {
        await Expect(_scoreboardCompleteDetails).ToBeVisibleAsync();
    }

    public async Task<TeamPage> GoToFirstTeamAsync()
    {
        await Page.Locator(".team-card__link").First.ClickAsync();
        return new TeamPage(Page);
    }
}
