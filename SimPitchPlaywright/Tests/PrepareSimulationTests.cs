using SimPitchPlaywright.Models;
using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class PrepareSimulationTests : BaseTest
{
    private PrepareSimulationPage _prepareSimulationPage = default!;
    private readonly Random rand = new();

    [SetUp]
    public async Task Init()
    {
        var mainPage = await new MainPage(Page).OpenAsync();
        _prepareSimulationPage = await mainPage.GoToPrepareSimulationViaSectionButtonAsync();
    }

    [Test]
    [Category("Prepare")]
    public async Task pw_PrepareSimulation_Should_Display_Validation_Errors()
    {
        await _prepareSimulationPage.ClickStartSimulationAsync();
        await _prepareSimulationPage.AssertValidationErrorsAsync(
            "You must select a league");
        await _prepareSimulationPage.SelectTitleAsync("AB");
        await _prepareSimulationPage.SelectModelAsync("Advanced");
        await _prepareSimulationPage.ClickStartSimulationAsync();
        await _prepareSimulationPage.AssertValidationErrorsAsync(
            "Title must have at least 3 characters",
            "You must select a league");
    }

    [Test]
    public async Task pw_PrepareSimulation_Should_Reset_Form_To_Default_State()
    {
        PrepareSimulationModel prep = new()
        {
            isSeason2022_2023 = true,
            isSeason2025_2026 = true,

            Title = "AB",
            League = "la-liga",
            NumberOfIterations = 999,
            CreateScoreboards = true,
            Model = SimulationModelEnum.DixonColes.ToString()
        };

        await _prepareSimulationPage.PrepareSimulationByModelAsync(prep, startSimulation: false);
        await _prepareSimulationPage.AssertPrepareSimulationByModelAsync(prep);

        await _prepareSimulationPage.ClickResetFormAsync();
        await _prepareSimulationPage.AssertSelectedSeasonYearsAsync();
        await _prepareSimulationPage.AssertSelectedModelAsync(prep.Model);
    }

    [Test]
    public async Task pw_PrepareSimulation_Should_Create_ALL_Simulation()
    {
        PrepareSimulationModel model = new()
        {
            isSeason2024_2025 = true,

            Title = $"Playwright {TestContext.CurrentContext.Test.Name} - {rand.Next(0, 99)}",
            League = "pko-bp-ekstraklasa",
            LeagueRound = 3,
            NumberOfIterations = 2,
            CreateScoreboards = true,
            Seed = 291363,
            GamesToReachTrust = 15,
            ConfidenceLevel = 0.95f,
            NoiseFactor = 0.12f,
            HomeAdvantage = 1.05f,
            Model = SimulationModelEnum.DixonColes.ToString()
        };

        await _prepareSimulationPage.PrepareSimulationByModelAsync(model, true);

        string simulationId = await _prepareSimulationPage.GetSimulationIdAsync();
        _createdSimulationIds.Add(simulationId);

        var simulationItemPage = await _prepareSimulationPage.GoToSimulationItemPageAsync();
        await simulationItemPage.AssertIfDisplayedAsync(simulationId);
    }
}
