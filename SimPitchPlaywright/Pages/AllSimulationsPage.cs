using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class AllSimulationsPage : BasePage
{
    internal readonly PaginationPage Pagination;
    internal readonly FilterPage Filter;
    protected readonly ILocator Title;
    protected readonly ILocator Simulations;
    protected ILocator SimulationDetails(int index) => ByTestId($"title-details-{index}");
    protected ILocator SimulationDetailsClose(int index) => ByTestId($"title-details-close-{index}");

    public AllSimulationsPage(IPage page) : base(page)
    {
        Title = ByTestId("title-all-simulations");
        Simulations = ByClass("simulation");

        Pagination = new PaginationPage(Page);
        Filter = new FilterPage(Page);
    }

    internal async Task RefreshPageAsync()
    {
        await ButtonPrimary.ClickAsync();
    }

    internal async Task AssertIfDisplayedAsync()
    {
        await Expect(Title).ToBeVisibleAsync();
    }

    internal async Task AssertSimulationDetailsAsync(int index, params string[] expectedTexts)
    {
        var text = await GetTextAsync(SimulationDetails(index));
        foreach (var expected in expectedTexts)
        {
            Assert.That(text, Does.Contain(expected), $"AssertSimulationDetails-Index:{index}");
        }
    }

    internal async Task<int> GetSimulationIndexByTitleAsync(string title)
    {
        for (int i = 0; i < 15; i++)
        {
            var details = SimulationDetails(i);
            if (await details.CountAsync() > 0 && (await GetTextAsync(details)).Contains(title))
            {
                return i;
            }
        }
        throw new Exception($"Simulation with title '{title}' not found on the page.");
    }

    internal async Task AssertClosedSimulationDetailsAsync(int index, params string[] expectedTexts)
    {
        await SimulationDetailsClose(index).ClickAsync();

        var text = await GetTextAsync(SimulationDetailsClose(index));
        foreach (var expected in expectedTexts)
        {
            Assert.That(text, Does.Contain(expected), $"AssertClosedSimulationDetails-Index:{index}");
        }
    }

    internal async Task AssertSimulationCountAsync(int expectedCount)
    {
        await Expect(Simulations).ToHaveCountAsync(expectedCount);
    }
}
