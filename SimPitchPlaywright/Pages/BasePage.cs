using System.Globalization;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    private NavBarPage? _navBar;
    public NavBarPage NavBar => _navBar ??= new NavBarPage(Page);

    protected readonly ILocator AppDiv;
    internal readonly ILocator ButtonPrimary;
    internal readonly ILocator ButtonSecondary;

    protected BasePage(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));

        AppDiv = ByClass("app").First;
        ButtonPrimary = ByClass("button-primary").First;
        ButtonSecondary = ByClass("button-secondary").First;
    }

    /// <summary>Exact match on the <c>selenium-id</c> attribute (registered as the test-id attribute).</summary>
    internal ILocator ByTestId(string seleniumId) => Page.GetByTestId(seleniumId);

    /// <summary>Partial match on the <c>selenium-id</c> attribute.</summary>
    internal ILocator ByAnyTestId(string seleniumId) => Page.Locator($"[selenium-id*='{seleniumId}']");

    /// <summary>Exact match on the whole <c>class</c> attribute (same semantics as in the Selenium project).</summary>
    internal ILocator ByClass(string className) => Page.Locator($"[class='{className}']");

    internal ILocator ById(string id) => Page.Locator($"[id='{id}']");

    internal async Task<SimulationItemPage> GoToSimulationItemPageViaUrlAsync(string simulationId)
    {
        await Task.Delay(500);
        await Page.GotoAsync($"/simulation/{simulationId}");
        return new SimulationItemPage(Page);
    }

    internal async Task<IterationResultPage> GoToIterationResultPageAsync(string simulationId, string iterationResultId)
    {
        await Task.Delay(500);
        await Page.GotoAsync($"/simulation/{simulationId}/iteration/{iterationResultId}");
        return new IterationResultPage(Page);
    }

    /// <summary>Visible text of an element; value for inputs, selected option text for selects.</summary>
    protected static async Task<string> GetTextAsync(ILocator locator)
    {
        await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var tagName = await locator.EvaluateAsync<string>("e => e.tagName.toLowerCase()");

        return tagName switch
        {
            "input" or "textarea" => await locator.InputValueAsync(),
            "select" => await locator.EvaluateAsync<string>("e => e.selectedOptions[0]?.text ?? ''"),
            _ => (await locator.InnerTextAsync()).Trim(),
        };
    }

    protected static async Task<bool> IsVisibleWithinAsync(ILocator locator, int timeoutSeconds = 10)
    {
        try
        {
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = timeoutSeconds * 1000 });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task<bool> IsElementPresentAndEnabledAsync(ILocator locator)
    {
        return await IsVisibleWithinAsync(locator) && await locator.IsEnabledAsync();
    }

    public Task<int> GetElementCountAsync(ILocator locator) => locator.CountAsync();

    public async Task AssertTextDisplayedAsync(string text)
    {
        var html = await AppDiv.InnerHTMLAsync();
        Assert.That(html, Does.Contain(text).IgnoreCase,
            $"Expected text '{text}' was not found in the application HTML.");
    }

    internal async Task WaitForTextAsync(string text)
    {
        if (!await IsElementWithTextDisplayedAsync(text))
        {
            throw new Exception($"Text '{text}' was not displayed within the timeout.");
        }
    }

    internal async Task<bool> IsElementWithTextDisplayedAsync(string text, string? tag = null, int timeoutSeconds = 30)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        var locator = string.IsNullOrWhiteSpace(tag)
            ? Page.GetByText(text)
            : Page.Locator(tag).Filter(new() { HasText = text });

        return await IsVisibleWithinAsync(locator.First, timeoutSeconds);
    }

    public async Task AssertIfCheckedAsync(ILocator locator, bool shouldBeChecked = true)
    {
        if (shouldBeChecked)
            await Expect(locator).ToBeCheckedAsync();
        else
            await Expect(locator).Not.ToBeCheckedAsync();
    }

    /// <summary>
    /// Selects an option of a native &lt;select&gt; matching (case-insensitive) its text, value or selenium-id.
    /// Waits until such option is rendered (options are often loaded asynchronously).
    /// </summary>
    public async Task SelectFromDropdownAsync(ILocator select, string option)
    {
        var index = await PollAsync(() => select.EvaluateAsync<int>(
            @"(el, wanted) => {
                wanted = wanted.trim().toLowerCase();
                return [...el.options].findIndex(o =>
                    [o.text, o.value, o.getAttribute('selenium-id')]
                        .some(v => (v ?? '').trim().toLowerCase() === wanted));
            }", option), i => i >= 0);

        if (index < 0)
            Assert.Fail($"Cannot find '{option}' in dropdown {select}.");

        await select.SelectOptionAsync(new SelectOptionValue { Index = index });
    }

    public async Task AssertDropdownValueAsync(ILocator select, string expectedValue)
    {
        var selected = await PollAsync(() => select.EvaluateAsync<string[]>(
            @"el => {
                const o = el.selectedOptions[0];
                return o ? [o.text.trim(), o.value.trim(), (o.getAttribute('selenium-id') ?? '').trim()] : [];
            }"),
            values => values.Any(v => string.Equals(v, expectedValue, StringComparison.OrdinalIgnoreCase)));

        Assert.That(selected.Any(v => string.Equals(v, expectedValue, StringComparison.OrdinalIgnoreCase)), Is.True,
            $"Expected: '{expectedValue}' is not selected in {select}. (Found: {string.Join(", ", selected)})");
    }

    public async Task AssertUrlContainsAsync(string expectedPart)
    {
        await Expect(Page).ToHaveURLAsync(new Regex(Regex.Escape(expectedPart), RegexOptions.IgnoreCase));
    }

    public async Task<bool> IsButtonDisabledAsync(ILocator locator)
    {
        try
        {
            return await locator.IsDisabledAsync()
                   || await locator.GetAttributeAsync("aria-disabled") == "true";
        }
        catch (PlaywrightException)
        {
            return false;
        }
    }

    public async Task AssertIfButtonDisabledAsync(ILocator locator, bool shouldBeDisabled = true)
    {
        if (shouldBeDisabled)
            await Expect(locator).ToBeDisabledAsync();
        else
            await Expect(locator).ToBeEnabledAsync();
    }

    internal async Task<bool> ScrollToTextAsync(string text)
    {
        try
        {
            await Page.GetByText(text).First.ScrollIntoViewIfNeededAsync();
            return true;
        }
        catch (PlaywrightException)
        {
            return false;
        }
    }

    public async Task<int> GetTableCellCountAsync(ILocator table)
    {
        var cells = table.Locator("td");
        await Expect(cells.First).ToBeAttachedAsync();
        return await cells.CountAsync();
    }

    public async Task SetRangeValueAsync(ILocator slider, int value)
    {
        await slider.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        int min = int.Parse(await slider.GetAttributeAsync("min") ?? "0", CultureInfo.InvariantCulture);
        int max = int.Parse(await slider.GetAttributeAsync("max") ?? "100", CultureInfo.InvariantCulture);
        int step = int.Parse(await slider.GetAttributeAsync("step") ?? "1", CultureInfo.InvariantCulture);

        if (value < min || value > max)
            Assert.Fail($"Value {value} is outside range {min}-{max} for slider {slider}.");

        if ((value - min) % step != 0)
            Assert.Fail($"Value {value} does not match slider step {step} for {slider}.");

        await SetSliderAsync(slider, value.ToString(CultureInfo.InvariantCulture));
    }

    public async Task SetRangeValueAsync(ILocator slider, float value)
    {
        await slider.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await SetSliderAsync(slider, value.ToString(CultureInfo.InvariantCulture));
    }

    private static async Task SetSliderAsync(ILocator slider, string value)
    {
        await slider.EvaluateAsync(@"(el, v) => {
            el.value = v;
            el.dispatchEvent(new Event('input', { bubbles: true }));
            el.dispatchEvent(new Event('change', { bubbles: true }));
        }", value);
    }

    private async Task<T> PollAsync<T>(Func<Task<T>> getValue, Func<T, bool> isDone)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (true)
        {
            var value = await getValue();
            if (isDone(value) || DateTime.UtcNow > deadline)
                return value;
            await Task.Delay(200);
        }
    }
}
