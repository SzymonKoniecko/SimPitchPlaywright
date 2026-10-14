using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class IterationResultPage : BasePage
{
    protected readonly ILocator ScoreboardTable;
    protected readonly ILocator Match;
    protected readonly ILocator SimulatedMatchesCount;
    protected readonly ILocator SimulatedMatchLabel;
    protected readonly ILocator ScatterDetails;
    protected readonly ILocator ResetScatterButton;
    protected readonly ILocator TeamRadarDetails;

    public IterationResultPage(IPage page) : base(page)
    {
        ScoreboardTable = ByTestId("scoreboard_complete_details").First;
        Match = ByTestId("match");
        SimulatedMatchesCount = ByTestId("number-simulated-matches");
        SimulatedMatchLabel = ByTestId("simulated");
        ScatterDetails = ByTestId("scatter-details");
        ResetScatterButton = ByTestId("reset-scatter-button");
        TeamRadarDetails = ByTestId("team-radar-details").First;
    }

    public async Task<int> GetNumberOfSimulatedMatchesAsync()
    {
        var value = (await GetTextAsync(SimulatedMatchesCount))
            .Split("Number of simulated matches: ")[1]
            .Trim();
        return int.Parse(value);
    }

    public async Task AssertIfDisplayedAsync()
    {
        await Expect(ScoreboardTable).ToBeVisibleAsync();
    }

    public async Task AssertNumOfTeamsInTableAsync(int expectedCount)
    {
        Assert.That(await GetTableCellCountAsync(ScoreboardTable), Is.EqualTo(expectedCount), "AssertNumOfTeamsInTable");
    }

    public async Task AssertNumOfSimulatedMatchesAsync()
    {
        Assert.That(
            await GetElementCountAsync(SimulatedMatchLabel),
            Is.EqualTo(await GetNumberOfSimulatedMatchesAsync()),
            "AssertNumOfSimulatedMatches");
    }

    public async Task AssertScatterPlotDisplayedAndResetAsync()
    {
        await Expect(ScatterDetails).ToBeVisibleAsync();
        await ResetScatterButton.ClickAsync();
    }

    public async Task AssertRadarChartDisplayedAsync()
    {
        // Not every round has team strengths (only played, simulated matches do),
        // so pick the first round that actually contains them.
        const string teamStrengthsSummary = "xpath=.//span[text()='Teams strengths']/..";
        var round = Page.Locator("details.match-details")
            .Filter(new() { Has = Page.Locator(teamStrengthsSummary) })
            .First;

        // First open the match round details to expose the match list
        await round.Locator(":scope > summary").ClickAsync();

        // Then open the team strengths details inside the first match of that round
        await round.Locator(teamStrengthsSummary).First.ClickAsync();

        await Expect(round.Locator(BySeleniumIdSelector("team-radar-details")).First).ToBeVisibleAsync();
    }
}
