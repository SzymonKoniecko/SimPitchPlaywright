using Microsoft.Playwright;

namespace SimPitchPlaywright.Pages;

public class PaginationPage : BasePage
{
    protected readonly ILocator PrevButton;
    protected readonly ILocator NextButton;
    protected readonly ILocator TotalCount;

    public PaginationPage(IPage page) : base(page)
    {
        PrevButton = ByTestId("prev-button");
        NextButton = ByTestId("next-button");
        TotalCount = ByTestId("total-count");
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
        await SelectFromDropdownAsync(ByTestId("size-select"), size);
        await Task.Delay(500);
    }
}
