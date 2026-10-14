using SimPitchPlaywright.Pages;
using SimPitchPlaywright.Utils;

namespace SimPitchPlaywright.Tests;

[TestFixture]
[NonParallelizable]
public class SimulationItemTests : BaseTest
{
    private SimulationItemPage _simulationItemPage = default!;
    private MainPage _mainPage = default!;
    private string SimulationId = string.Empty;
    private static string StaticSimulationId = string.Empty;

    [SetUp]
    public async Task Init()
    {
        _mainPage = await new MainPage(Page).OpenAsync();
    }

    [Test]
    public async Task pw_SimulationItem_Simple_Creation_Test()
    {
        var prepPage = await _mainPage.GoToPrepareSimulationViaSectionButtonAsync();
        await prepPage.StartAnySimulationAsync(1); // just 1 iteration so it's fast
        SimulationId = await prepPage.GetSimulationIdAsync();
        _createdSimulationIds.Add(SimulationId);
        _simulationItemPage = await prepPage.GoToSimulationItemPageAsync();
        await _simulationItemPage.AssertIfDisplayedAsync(SimulationId);
    }

    private async Task EnsureSimulationExistsAsync()
    {
        if (string.IsNullOrEmpty(StaticSimulationId))
        {
            var prepPage = await _mainPage.GoToPrepareSimulationViaSectionButtonAsync();
            await prepPage.StartAnySimulationAsync(20);
            StaticSimulationId = await prepPage.GetSimulationIdAsync();
            _createdSimulationIds.Add(StaticSimulationId);
            _simulationItemPage = await prepPage.GoToSimulationItemPageAsync();
        }
        else
        {
            _simulationItemPage = await _mainPage.GoToSimulationItemPageViaUrlAsync(StaticSimulationId);
        }

        await _simulationItemPage.AssertIfDisplayedAsync(StaticSimulationId);
        await _simulationItemPage.WaitForCompletedSimulationAsync();
        SimulationId = StaticSimulationId;
    }

    [Test]
    public async Task pw_SimulationItem_Assert_Status_And_Refresh()
    {
        await Task.Delay(4000); // Give it a moment to start
        // Preparation (large iteration number to cover changes in statuses)
        _mainPage = await new MainPage(Page).OpenAsync();
        var prepPage = await _mainPage.GoToPrepareSimulationViaSectionButtonAsync();
        await prepPage.StartAnySimulationAsync(200);
        SimulationId = await prepPage.GetSimulationIdAsync();
        _createdSimulationIds.Add(SimulationId);
        _simulationItemPage = await prepPage.GoToSimulationItemPageViaUrlAsync(SimulationId);
        await _simulationItemPage.AssertIfDisplayedAsync(SimulationId);

        await _simulationItemPage.AssertSimulationStateAsync("Running");
        await _simulationItemPage.AssertIfIterationsPercentageIsNot100Async();
    }

    [Test]
    public async Task pw_SimulatonItem_Assert_Pagination()
    {
        await EnsureSimulationExistsAsync();
        if (string.IsNullOrEmpty(SimulationId))
            throw new Exception("Init not completed? Init() - pw_SimulatonItem_Assert_Pagination");

        // Pagination
        await _simulationItemPage.Pagination.CheckIfItsFirstPageAsync();
        await _simulationItemPage.AssertIterationCountAsync(10);
        await _simulationItemPage.Pagination.GoToLatestPageAsync();
        await _simulationItemPage.Pagination.SelectPageSizeAsync("5");
        await WaitHelper.Sleep();
        await _simulationItemPage.WaitForTextAsync("Check complete iteration details");
        await _simulationItemPage.AssertIterationCountAsync(5);
    }

    [Test]
    public async Task pw_SimulatonItem_Assert_Filter()
    {
        await EnsureSimulationExistsAsync();
        if (string.IsNullOrEmpty(SimulationId))
            throw new Exception("Init not completed? Init() - pw_SimulatonItem_Assert_Filter");

        // Filter
        await _simulationItemPage.Pagination.CheckIfItsFirstPageAsync();
        await _simulationItemPage.Pagination.SelectPageSizeAsync("10");
        await WaitHelper.Sleep();
        await _simulationItemPage.WaitForTextAsync("Check complete iteration details");
        await _simulationItemPage.Filter.SetSortingMethodAsync("order-by-iteration");
        await WaitHelper.Sleep();
        await _simulationItemPage.WaitForTextAsync("Check complete iteration details");
        await _simulationItemPage.Filter.SetSortingMethodAsync("order-by-iteration");
        await _simulationItemPage.AssertIterationCountAsync(10);
        await _simulationItemPage.Filter.ChangeSortingOrderAsync();
        await _simulationItemPage.AssertIterationCountAsync(10);
        await _simulationItemPage.AssertTextDisplayedAsync("Toggle Ascending");

        await _simulationItemPage.Filter.SetSortingMethodAsync("team", "jagiellonia-bialystok");
        await _simulationItemPage.WaitForTextAsync("Check complete iteration details");
    }

    [Test]
    public async Task pw_SimulationItem_Assert_HeatMap()
    {
        await EnsureSimulationExistsAsync();
        if (string.IsNullOrEmpty(SimulationId))
            throw new Exception("Init not completed? Init() - pw_SimulationItem_Assert_HeatMap");

        await WaitHelper.Sleep();

        await _simulationItemPage.AssertPercentSumEquals100Async(0, "Team row index 0");
        await _simulationItemPage.AssertPercentSumEquals100Async(10, "Team row index 10");
    }

    [Test]
    public async Task pw_SimulationItem_Assert_Stop_Simulation()
    {
        await Task.Delay(4000); // Give it a moment to start
        _mainPage = await new MainPage(Page).OpenAsync();
        var prepPage = await _mainPage.GoToPrepareSimulationViaSectionButtonAsync();
        await prepPage.StartAnySimulationAsync(400);
        SimulationId = await prepPage.GetSimulationIdAsync();
        _createdSimulationIds.Add(SimulationId);
        _simulationItemPage = await prepPage.GoToSimulationItemPageViaUrlAsync(SimulationId);
        await _simulationItemPage.AssertIfDisplayedAsync(SimulationId);

        await _simulationItemPage.AssertSimulationStateAsync("Running");

        var api = await NewApiContextAsync();
        try
        {
            // Cancel via API directly
            var response = await api.DeleteAsync($"/api/engine/Simulation/stop/{SimulationId}");
            Assert.That(response.Ok, Is.True, "API call to stop simulation failed.");

            await Task.Delay(1000); // Give it a moment to stop

            // Assert state via API instead of UI
            var getResponse = await api.GetAsync($"/api/engine/Simulation/{SimulationId}");
            Assert.That(getResponse.Ok, Is.True, "API call to get simulation failed.");

            var json = await getResponse.JsonAsync();
            if (json is { } body && body.TryGetProperty("state", out var state))
            {
                Assert.That(state.ToString(), Does.Contain("Cancelled"), "Simulation state in API is not Cancelled.");
            }
            else
            {
                throw new Exception("Missing Simulation state");
            }
        }
        finally
        {
            await api.DisposeAsync();
        }
    }

    [Test]
    public async Task pw_SimulationItem_Should_Navigate_To_IterationItem()
    {
        await EnsureSimulationExistsAsync();
        if (string.IsNullOrEmpty(SimulationId))
            throw new Exception("Init not completed? Init() - pw_SimulationItem_Should_Navigate_To_IterationItem");

        // Click on the first iteration
        var iterationPage = await _simulationItemPage.GoToIterationAsync(0);

        // Assert that we navigated to the iteration page
        await iterationPage.AssertIfDisplayedAsync();
    }
}
