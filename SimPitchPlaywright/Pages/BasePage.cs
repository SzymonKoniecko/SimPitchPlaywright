using Microsoft.Playwright;

namespace SimPitchPlaywright.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    /// <summary>Locates an element by its <c>selenium-id</c> attribute.</summary>
    protected ILocator ByTestId(string id) => Page.GetByTestId(id);

    public NavBarPage NavBar => new(Page);
}
