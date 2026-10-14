using Microsoft.Playwright;
using SimPitchPlaywright.Models;
using SimPitchPlaywright.Utils;
using static Microsoft.Playwright.Assertions;

namespace SimPitchPlaywright.Pages;

public class PrepareSimulationPage : BasePage
{
    protected readonly ILocator Title;
    internal readonly ILocator Season2022_2023CheckBox;
    internal readonly ILocator Season2023_2024CheckBox;
    internal readonly ILocator Season2024_2025CheckBox;
    internal readonly ILocator Season2025_2026CheckBox;
    internal readonly ILocator Season2026_2027CheckBox;
    internal readonly ILocator TitleInput;
    internal readonly ILocator LeagueInput;
    internal readonly ILocator LeagueRoundInput;
    internal readonly ILocator NumberIterationInput;
    internal readonly ILocator SeedInput;
    internal readonly ILocator GamesToReachTrustRange;
    internal readonly ILocator ConfidenceLevelRange;
    internal readonly ILocator NoiseFactorRange;
    internal readonly ILocator HomeAdvantageRange;
    internal readonly ILocator GamesToReachTrustLabel;
    internal readonly ILocator ConfidenceLevelLabel;
    internal readonly ILocator NoiseFactorLabel;
    internal readonly ILocator HomeAdvantageLabel;
    internal readonly ILocator CreateScoreboardsCheckbox;
    internal readonly ILocator CreatedSimulationMessage;
    internal readonly ILocator CreatedSimulationButton;
    internal readonly ILocator ValidationError;
    internal readonly string[] SimulationModels;

    public PrepareSimulationPage(IPage page) : base(page)
    {
        Title = ByTestId("title-prepare-simulation");

        Season2022_2023CheckBox = SeasonCheckBox("2022/2023");
        Season2023_2024CheckBox = SeasonCheckBox("2023/2024");
        Season2024_2025CheckBox = SeasonCheckBox("2024/2025");
        Season2025_2026CheckBox = SeasonCheckBox("2025/2026");
        Season2026_2027CheckBox = SeasonCheckBox("2026/2027");
        TitleInput = ByTestId("input-title");
        LeagueInput = ById("leagueId");
        LeagueRoundInput = ById("leagueRoundId");
        NumberIterationInput = ByTestId("input-iterations");

        SeedInput = ByTestId("seed");
        GamesToReachTrustRange = ByTestId("gamesToReachTrust");
        ConfidenceLevelRange = ByTestId("confidenceLevel");
        NoiseFactorRange = ByTestId("noiseFactor");
        HomeAdvantageRange = ByTestId("homeAdvantage");

        GamesToReachTrustLabel = ByTestId("gamesToReachTrustLabel");
        ConfidenceLevelLabel = ByTestId("confidenceLevelLabel");
        NoiseFactorLabel = ByTestId("noiseFactorLabel");
        HomeAdvantageLabel = ByTestId("homeAdvantageLabel");

        CreateScoreboardsCheckbox = ByTestId("input-create-scoreboards");
        CreatedSimulationMessage = ByTestId("simulation-id-text");
        CreatedSimulationButton = ByTestId("simulation-result");
        ValidationError = ByClass("validation-error");

        SimulationModels = Enum.GetNames<SimulationModelEnum>();
    }

    private ILocator SeasonCheckBox(string season) => Page.Locator($"input[type='checkbox'][value='{season}']");

    internal async Task<string> GetSimulationIdAsync()
    {
        return (await GetTextAsync(CreatedSimulationMessage))
            .Split("Simulation ID:")[1]
            .Trim();
    }

    internal async Task SelectSeasonYearsAsync(PrepareSimulationModel model)
    {
        if (model.isSeason2022_2023)
            await Season2022_2023CheckBox.ClickAsync();
        if (model.isSeason2023_2024)
            await Season2023_2024CheckBox.ClickAsync();
        if (model.isSeason2024_2025)
            await Season2024_2025CheckBox.ClickAsync();
        if (model.isSeason2025_2026)
            await Season2025_2026CheckBox.ClickAsync();
    }

    internal async Task SelectTitleAsync(string title)
    {
        await TitleInput.FillAsync(title);
        await Title.ClickAsync();
    }

    internal Task SelectLeagueAsync(string leagueName) => SelectFromDropdownAsync(LeagueInput, leagueName);

    internal Task SelectLeagueRoundAsync(int roundNumber) => SelectFromDropdownAsync(LeagueRoundInput, "round:" + roundNumber);

    internal Task SelectNumberOfIterationsAsync(int iterationsNumber) => NumberIterationInput.FillAsync(iterationsNumber.ToString());

    internal Task SelectSeedAsync(int seed) => SeedInput.FillAsync(seed.ToString());

    internal async Task SelectGamesToReachTrustAsync(int value)
    {
        await SetRangeValueAsync(GamesToReachTrustRange, value);
        await AssertGamesToReachTrustAsync(value);
    }

    internal async Task SelectConfidenceLevelAsync(float value)
    {
        await SetRangeValueAsync(ConfidenceLevelRange, value);
        await AssertConfidenceLevelAsync(value);
    }

    internal async Task SelectNoiseFactorAsync(float value)
    {
        await SetRangeValueAsync(NoiseFactorRange, value);
        await AssertNoiseFactorAsync(value);
    }

    internal async Task SelectHomeAdvantageAsync(float value)
    {
        await SetRangeValueAsync(HomeAdvantageRange, value);
        await AssertHomeAdvantageAsync(value);
    }

    internal Task SelectCreateScoreboardsCheckboxAsync() => CreateScoreboardsCheckbox.ClickAsync();

    internal Task SelectModelAsync(string value) => ByTestId($"model-{value}").ClickAsync();

    internal Task ClickStartSimulationAsync() => ButtonPrimary.ClickAsync();

    internal Task ClickResetFormAsync() => ButtonSecondary.ClickAsync();

    internal async Task<SimulationItemPage> GoToSimulationItemPageAsync()
    {
        await WaitHelper.Sleep();
        await CreatedSimulationButton.ClickAsync();
        return new SimulationItemPage(Page);
    }

    internal async Task AssertIfDisplayedAsync()
    {
        await Expect(Title).ToBeVisibleAsync();
    }

    internal async Task AssertSelectedSeasonYearsAsync(
        bool isSeason2022_2023 = false,
        bool isSeason2023_2024 = false,
        bool isSeason2024_2025 = false,
        bool isSeason2025_2026 = false,
        bool isSeason2026_2027 = true)
    {
        await AssertIfCheckedAsync(Season2022_2023CheckBox, isSeason2022_2023);
        await AssertIfCheckedAsync(Season2023_2024CheckBox, isSeason2023_2024);
        await AssertIfCheckedAsync(Season2024_2025CheckBox, isSeason2024_2025);
        await AssertIfCheckedAsync(Season2025_2026CheckBox, isSeason2025_2026);
        await AssertIfCheckedAsync(Season2026_2027CheckBox, isSeason2026_2027);
    }

    internal Task AssertSelectedModelAsync(string value) => AssertIfCheckedAsync(ByTestId($"model-{value}"));

    internal async Task AssertTitleAsync(string expectedTitle)
    {
        await Expect(TitleInput).ToHaveValueAsync(expectedTitle);
    }

    internal async Task AssertNumberOfIterationsAsync(string expectedIterationsNumber)
    {
        await Expect(NumberIterationInput).ToHaveValueAsync(expectedIterationsNumber);
    }

    internal Task AssertLeagueAsync(string expectedLeague) => AssertDropdownValueAsync(LeagueInput, expectedLeague);

    internal Task AssertLeagueRoundAsync(int expectedLeagueRound) => AssertDropdownValueAsync(LeagueRoundInput, expectedLeagueRound.ToString());

    internal async Task AssertSeedAsync(int expectedSeed)
    {
        await Expect(SeedInput).ToHaveValueAsync(expectedSeed.ToString());
    }

    internal async Task AssertGamesToReachTrustAsync(int expected)
    {
        Assert.That((await GetTextAsync(GamesToReachTrustLabel)).Replace('.', ','), Is.EqualTo(expected.ToString()),
            "AssertGamesToReachTrust");
    }

    internal Task AssertConfidenceLevelAsync(float expected) => AssertSliderLabelAsync(ConfidenceLevelLabel, expected, "AssertConfidenceLevel");

    internal Task AssertNoiseFactorAsync(float expected) => AssertSliderLabelAsync(NoiseFactorLabel, expected, "AssertNoiseFactor");

    internal Task AssertHomeAdvantageAsync(float expected) => AssertSliderLabelAsync(HomeAdvantageLabel, expected, "AssertHomeAdvantage");

    private async Task AssertSliderLabelAsync(ILocator label, float expected, string context)
    {
        Assert.That(
            TextHelper.ReplaceNumberToAssert(await GetTextAsync(label)),
            Is.EqualTo(TextHelper.ReplaceNumberToAssert(expected.ToString())),
            context);
    }

    internal Task AssertCreateScoreboardsCheckboxAsync(bool isSelected) => AssertIfCheckedAsync(CreateScoreboardsCheckbox, isSelected);

    internal async Task AssertValidationErrorsAsync(params string[] validationErrors)
    {
        foreach (var validationError in validationErrors)
        {
            await Expect(ValidationError).ToContainTextAsync(validationError);
        }
    }

    internal async Task AssertStartedSimulationMessageAsync()
    {
        await Expect(CreatedSimulationMessage).ToContainTextAsync("Simulation ID:");
    }

    internal async Task StartAnySimulationAsync(int iterations = 2)
    {
        Random rand = new();

        PrepareSimulationModel prep = new()
        {
            isSeason2022_2023 = true,
            isSeason2025_2026 = true,

            Title = "Any test - " + DateTime.Now,
            League = "pko-bp-ekstraklasa",
            NumberOfIterations = iterations,
            Model = SimulationModels[rand.Next(0, SimulationModels.Length)]
        };
        await SelectSeasonYearsAsync(prep);
        await SelectTitleAsync(prep.Title);
        await SelectLeagueAsync(prep.League);
        await SelectModelAsync(prep.Model);
        await SelectNumberOfIterationsAsync(prep.NumberOfIterations);
        await AssertNumberOfIterationsAsync(prep.NumberOfIterations.ToString());

        await ClickStartSimulationAsync();

        await AssertStartedSimulationMessageAsync();
    }

    internal async Task PrepareSimulationByModelAsync(PrepareSimulationModel model, bool startSimulation = false)
    {
        await SelectSeasonYearsAsync(model);
        await SelectLeagueAsync(model.League);
        await SelectNumberOfIterationsAsync(model.NumberOfIterations);

        if (model.LeagueRound != null)
            await SelectLeagueRoundAsync(model.LeagueRound.Value);
        if (model.Model != null)
            await SelectModelAsync(model.Model);
        if (model.Title != null)
            await SelectTitleAsync(model.Title);
        if (model.Seed != null)
            await SelectSeedAsync(model.Seed.Value);
        if (model.GamesToReachTrust != null)
            await SelectGamesToReachTrustAsync(model.GamesToReachTrust.Value);
        if (model.ConfidenceLevel != null)
            await SelectConfidenceLevelAsync(model.ConfidenceLevel.Value);
        if (model.NoiseFactor != null)
            await SelectNoiseFactorAsync(model.NoiseFactor.Value);
        if (model.HomeAdvantage != null)
            await SelectHomeAdvantageAsync(model.HomeAdvantage.Value);

        if (startSimulation)
        {
            await ClickStartSimulationAsync();

            await AssertStartedSimulationMessageAsync();
        }
    }

    internal async Task AssertPrepareSimulationByModelAsync(PrepareSimulationModel model)
    {
        await AssertSelectedSeasonYearsAsync(model.isSeason2022_2023, model.isSeason2023_2024, model.isSeason2024_2025, model.isSeason2025_2026, model.isSeason2026_2027);
        await AssertLeagueAsync(model.League);
        await AssertNumberOfIterationsAsync(model.NumberOfIterations.ToString());

        if (model.LeagueRound != null)
            await AssertLeagueRoundAsync(model.LeagueRound.Value);
        if (model.Model != null)
            await AssertSelectedModelAsync(model.Model);
        if (model.Title != null)
            await AssertTitleAsync(model.Title);
        if (model.Seed != null)
            await AssertSeedAsync(model.Seed.Value);
        if (model.GamesToReachTrust != null)
            await AssertGamesToReachTrustAsync(model.GamesToReachTrust.Value);
        if (model.ConfidenceLevel != null)
            await AssertConfidenceLevelAsync(model.ConfidenceLevel.Value);
        if (model.NoiseFactor != null)
            await AssertNoiseFactorAsync(model.NoiseFactor.Value);
        if (model.HomeAdvantage != null)
            await AssertHomeAdvantageAsync(model.HomeAdvantage.Value);
    }
}
