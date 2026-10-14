using Microsoft.Playwright;

namespace SimPitchPlaywright.Pages;

public class PaginationPage : BasePage
{
    protected readonly ILocator PrevButton;
    protected readonly ILocator NextButton;
    protected readonly ILocator TotalCount;

    public PaginationPage(IPage page) : base(page)
    {
        // Some pages render the pagination twice (above and below the list) - use the first one.
        PrevButton = ByTestId("prev-button").First;
        NextButton = ByTestId("next-button").First;
        TotalCount = ByTestId("total-count").First;
    }

    internal async Task<int> GetTotalCountAsync()
    {
        var value = (await GetTextAsync(TotalCount))
            .Split("Total count: ")[1]
            .Trim();

        return int.Parse(value);
    }

    internal async Task GoToPreviousPageAsync()
    {
        await PrevButton.ClickAsync();
    }

    internal async Task GoToNextPageAsync()
    {
        if (!await IsButtonDisabledAsync(NextButton))
        {
            await NextButton.ClickAsync();
        }
    }

    internal async Task CheckIfItsFirstPageAsync()
    {
        await AssertIfButtonDisabledAsync(PrevButton, true);
    }

    internal async Task GoToLatestPageAsync()
    {
        while (!await IsButtonDisabledAsync(NextButton))
        {
            await GoToNextPageAsync();
            await Task.Delay(500);
        }
    }

    internal async Task SelectPageSizeAsync(string size)
    {
        await SelectFromDropdownAsync(ByTestId("size-select").First, size);
        await Task.Delay(500);
    }
}
