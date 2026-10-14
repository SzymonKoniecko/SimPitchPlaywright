using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class IterationResultTests : BaseTest
{
    private IterationResultPage _iterationResultPage = default!;
    private MainPage _mainPage = default!;
    private string SimulationId = string.Empty;
    private static string StaticSimulationId = string.Empty;

    [SetUp]
    public async Task Init()
    {
        _mainPage = await new MainPage(Page).OpenAsync();
    }

    [Test]
    public async Task pw_IterationResult_Simple_Navigation_Test()
    {
        // Navigate directly to a fake iteration URL to test the page skeleton without invoking the backend
        var fakeSimulationId = Guid.NewGuid().ToString();
        await Page.GotoAsync($"/simulation/{fakeSimulationId}/iteration/0");

        // At this point we just want to ensure we didn't crash.
        Assert.DoesNotThrow(() =>
        {
            _iterationResultPage = new IterationResultPage(Page);
        });
    }

    private async Task EnsureSimulationExistsAsync()
    {
        SimulationItemPage simulationItemPage;
        if (string.IsNullOrEmpty(StaticSimulationId))
        {
            var prepPage = await _mainPage.GoToPrepareSimulationViaSectionButtonAsync();
            await prepPage.StartAnySimulationAsync(1);
            StaticSimulationId = await prepPage.GetSimulationIdAsync();
            _createdSimulationIds.Add(StaticSimulationId);
            simulationItemPage = await prepPage.GoToSimulationItemPageAsync();
        }
        else
        {
            simulationItemPage = await _mainPage.GoToSimulationItemPageViaUrlAsync(StaticSimulationId);
        }

        await simulationItemPage.AssertIfDisplayedAsync(StaticSimulationId);
        await simulationItemPage.WaitForCompletedSimulationAsync();
        _iterationResultPage = await simulationItemPage.GoToIterationAsync(0);
        SimulationId = StaticSimulationId;
    }

    [Test]
    public async Task pw_IterationResult_Assert_RadarChart()
    {
        await EnsureSimulationExistsAsync();

        await _iterationResultPage.AssertIfDisplayedAsync();
        await _iterationResultPage.AssertRadarChartDisplayedAsync();
    }
}
