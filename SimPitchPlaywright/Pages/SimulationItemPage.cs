using System.Globalization;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class SimulationItemPage : BasePage
{
    internal readonly PaginationPage Pagination;
    internal readonly FilterPage Filter;
    protected readonly ILocator Title;
    internal readonly ILocator SimulationParamsDetails;
    internal readonly ILocator SimulationParamsDetailsList;
    internal readonly ILocator SimulationState;
    internal readonly ILocator SimulationStateValue;
    internal readonly ILocator SimulationIterations;
    internal readonly ILocator SimulationIterationsValue;
    internal readonly ILocator Iterations;
    internal readonly ILocator HeatMap;
    protected readonly ILocator StopSimulationBtn;

    public SimulationItemPage(IPage page) : base(page)
    {
        Title = ByTestId("title-simulation-item");
        SimulationParamsDetails = ByTestId("sim-params-details");
        SimulationParamsDetailsList = ByTestId("sim-params-details-list");
        SimulationState = ByTestId("state");
        SimulationStateValue = ByTestId("state-value");
        SimulationIterations = ByTestId("iterations");
        SimulationIterationsValue = ByTestId("iterations-value");
        Iterations = ByClass("scoreboard-block");
        HeatMap = ByTestId("heatmap-details");
        StopSimulationBtn = Page.Locator("button", new() { HasTextString = "Stop simulation" });

        Pagination = new PaginationPage(Page);
        Filter = new FilterPage(Page);
    }

    internal async Task<IterationResultPage> GoToIterationAsync(int index)
    {
        await ByTestId($"iteration-{index}").ClickAsync();
        return new IterationResultPage(Page);
    }

    internal async Task StopSimulationAsync()
    {
        await StopSimulationBtn.ClickAsync();
    }

    internal async Task<string> GetCompletedIterationsStringAsync()
    {
        return (await GetTextAsync(SimulationIterationsValue)).Trim();
    }

    internal async Task<string> GetSimulationStateAsync()
    {
        return (await GetTextAsync(SimulationStateValue)).Trim();
    }

    internal async Task WaitForCompletedSimulationAsync(int timeoutSeconds = 60)
    {
        string state = await GetSimulationStateAsync();
        while (state.Contains("Running") || state.Contains("Pending"))
        {
            await Task.Delay(1000);
            await RefreshPageAsync();
            await SimulationStateValue.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeoutSeconds * 1000 });
            state = await GetSimulationStateAsync();
        }

        await WaitForAllScoreboardsAsync(timeoutSeconds);
    }

    /// <summary>
    /// Scoreboards are created asynchronously, so the state may already be "Completed" while some
    /// iterations still have no scoreboard - the page then groups them into fewer blocks and does not
    /// refresh on its own. Reload until the first page shows all of its iterations.
    /// </summary>
    private async Task WaitForAllScoreboardsAsync(int timeoutSeconds)
    {
        const int defaultPageSize = 10;
        // e.g. "20 / 20 (100%)"
        var totalIterations = int.Parse((await GetCompletedIterationsStringAsync()).Split('/')[1].Split('(')[0].Trim());
        var expectedCount = Math.Min(defaultPageSize, totalIterations);

        var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        while (await Iterations.CountAsync() != expectedCount && DateTime.UtcNow < deadline)
        {
            await Task.Delay(1000);
            await RefreshPageAsync();
            await SimulationStateValue.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }
    }

    internal async Task RefreshPageAsync()
    {
        await ButtonPrimary.ClickAsync();
    }

    internal async Task AssertIfDisplayedAsync(string simulationId)
    {
        // Right after creation (especially under load) the backend may not have persisted
        // the simulation yet and responds with 404 - the page then shows an error and does
        // not retry on its own, so reload it until the simulation shows up.
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (!await IsVisibleWithinAsync(Title, 5) && DateTime.UtcNow < deadline)
        {
            await RefreshPageAsync();
        }

        await Expect(Title).ToBeVisibleAsync();
        await AssertUrlContainsAsync(simulationId);
    }

    internal async Task AssertIterationCountAsync(int expectedCount)
    {
        await Expect(Iterations).ToHaveCountAsync(expectedCount);
    }

    internal async Task AssertIfIterationsPercentageIsNot100Async()
    {
        Assert.That(await GetCompletedIterationsStringAsync(), Does.Not.Contain("(100%)"),
            "AssertIfIterationsPercentageIsNot100");
    }

    internal async Task AssertSimulationParamsAsync(params string[] expectedTexts)
    {
        await SimulationParamsDetails.ClickAsync();

        var text = await GetTextAsync(SimulationParamsDetailsList);
        foreach (var expected in expectedTexts)
        {
            Assert.That(text, Does.Contain(expected), "AssertSimulationParams");
        }
    }

    internal async Task AssertSimulationStateAsync(string expectedString)
    {
        Assert.That(await GetSimulationStateAsync(), Does.Contain(expectedString), "AssertSimulationState");
    }

    public Task AssertPercentSumEquals100Async(int rowNumber, string context) =>
        AssertPercentSumEquals100Async(ByAnyTestId($"cell-{rowNumber}-"), context);

    public async Task AssertPercentSumEquals100Async(ILocator cells, string context = "")
    {
        await HeatMap.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 15_000 });
        if (!await IsElementWithTextDisplayedAsync("Pos 1"))
            await HeatMap.ClickAsync();

        var texts = await cells.AllInnerTextsAsync();
        if (texts.Count == 0)
        {
            Assert.Fail($"No elements found for locator: {cells} ({context})");
        }

        double total = 0.0;
        foreach (var cellText in texts)
        {
            var text = cellText.Trim().Replace("%", "").Trim();
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                total += value;
            }
            else
            {
                Assert.Fail($"Invalid percentage format in cell: '{text}' ({context})");
            }
        }

        Assert.That(total, Is.EqualTo(100.0).Within(0.01),
            $"Sum of percentages does not equal 100%. Total = {total:F3}% ({context})");
    }
}
