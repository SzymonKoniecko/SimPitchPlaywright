using Microsoft.Playwright;

namespace SimPitchPlaywright.Pages;

public class FilterPage : BasePage
{
    protected readonly ILocator SortingOrderButton;

    public FilterPage(IPage page) : base(page)
    {
        SortingOrderButton = ByTestId("button-order");
    }

    internal async Task SetSortingMethodAsync(string sortingName, string? condition = null)
    {
        await SelectFromDropdownAsync(ByTestId("sorting-select"), sortingName);
        if (!string.IsNullOrEmpty(condition))
        {
            await SelectFromDropdownAsync(ByTestId("condition-select"), condition);
        }
    }

    internal async Task ChangeSortingOrderAsync()
    {
        await SortingOrderButton.ClickAsync();
    }
}
