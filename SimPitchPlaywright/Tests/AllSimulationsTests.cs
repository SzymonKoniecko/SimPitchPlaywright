using SimPitchPlaywright.Pages;

namespace SimPitchPlaywright.Tests;

[TestFixture]
public class AllSimulationsTests : BaseTest
{
    private AllSimulationsPage _allSimulationsPage = default!;

    [SetUp]
    public async Task Init()
    {
        var mainPage = await new MainPage(Page).OpenAsync();
        _allSimulationsPage = await mainPage.NavBar.GoToAllSimulationsPageAsync();
    }

    [Test]
    public async Task pw_AllSimulations_Should_Navigate_And_Display()
    {
        await _allSimulationsPage.AssertIfDisplayedAsync();
    }
}
