using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class TeamPage : BasePage
{
    private readonly ILocator _titleTeam;
    private readonly ILocator _teamLeagueBadge;
    private readonly ILocator _teamStadium;
    private readonly ILocator _backToLeaguesButton;

    public TeamPage(IPage page) : base(page)
    {
        _titleTeam = ByTestId("title-team");
        _teamLeagueBadge = ByTestId("team-league-badge");
        _teamStadium = ByTestId("team-stadium");
        _backToLeaguesButton = Page.Locator("a.button-secondary[href*='/league']").First;
    }

    public async Task AssertIfDisplayedAsync()
    {
        await Expect(_titleTeam).ToBeVisibleAsync();
    }

    public async Task AssertDetailsDisplayedAsync()
    {
        await Expect(_teamLeagueBadge).ToBeVisibleAsync();
        await Expect(_teamStadium).ToBeVisibleAsync();
    }

    public async Task<LeagueOverviewsPage> GoBackToLeaguesAsync()
    {
        await _backToLeaguesButton.ClickAsync();
        return new LeagueOverviewsPage(Page);
    }
}
