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
        // First open the first match round details to expose the match list
        await Page.Locator(".match-details summary").First.ClickAsync();

        // Then open the team strengths details inside the first match
        await Page.Locator("xpath=//span[text()='Teams strengths']/..").First.ClickAsync();

        await Expect(TeamRadarDetails).ToBeVisibleAsync();
    }
}
