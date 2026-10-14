using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class MainPage : BasePage
{
    protected readonly ILocator SimPitchSection;
    protected readonly ILocator SimulationModelsSection;
    protected readonly ILocator VisualizationsSection;
    protected readonly ILocator PosteriorSection;
    internal readonly ILocator PrepareSimulationBtn;

    public MainPage(IPage page) : base(page)
    {
        SimPitchSection = ByClass("hero");
        SimulationModelsSection = ByTestId("simulation-models");
        VisualizationsSection = ByTestId("visualizations");
        PosteriorSection = ByTestId("using-example-2");
        PrepareSimulationBtn = ByTestId("prepareSimulation");
    }

    public async Task<MainPage> OpenAsync()
    {
        await Page.GotoAsync("/");
        return this;
    }

    public async Task<PrepareSimulationPage> GoToPrepareSimulationViaSectionButtonAsync()
    {
        await PrepareSimulationBtn.ClickAsync();
        return new PrepareSimulationPage(Page);
    }

    internal async Task AssertIfDisplayedAsync()
    {
        await Expect(SimulationModelsSection).ToBeVisibleAsync();
    }

    public Task<string> GetSimPitchSectionTextAsync() => GetTextAsync(SimPitchSection);
    public Task<string> GetSimulationModelsSectionTextAsync() => GetTextAsync(SimulationModelsSection);
    public Task<string> GetVisualizationsSectionTextAsync() => GetTextAsync(VisualizationsSection);
    public Task<string> GetPosteriorSectionTextAsync() => GetTextAsync(PosteriorSection);
}
