using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class MainTests : BaseTest
{
    private MainPage _mainPage = default!;

    [SetUp]
    public async Task Init()
    {
        _mainPage = await new MainPage(Page).OpenAsync();
    }

    [Test]
    public async Task pw_Main_Should_See_NavBar()
    {
        await _mainPage.NavBar.AssertIfNavBarDisplayedAsync();
    }

    [Test]
    public async Task pw_Main_Should_Display_Sections_Text()
    {
        var heroText = await _mainPage.GetSimPitchSectionTextAsync();
        Assert.That(heroText, Does.Contain("SimPitch"));
        Assert.That(heroText, Does.Contain("Advanced football match and league simulation based on Bayesian models."));
        Assert.That(heroText, Does.Contain("Prepare a new simulation"));

        Assert.That(await _mainPage.IsElementPresentAndEnabledAsync(_mainPage.PrepareSimulationBtn), Is.True,
            "Prepare simulation button is not displayed!");

        Assert.That(await _mainPage.GetSimulationModelsSectionTextAsync(), Does.Contain("Simulation Models"));
        Assert.That(await _mainPage.GetVisualizationsSectionTextAsync(), Does.Contain("Heat Map"));
    }
}
