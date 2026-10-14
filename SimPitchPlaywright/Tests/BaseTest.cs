using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using SimPitchPlaywright.Utils;

namespace SimPitchPlaywright.Tests;

public abstract class BaseTest : PageTest
{
    protected string BaseUrl = default!;
    protected List<string> _createdSimulationIds = new();

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = ConfigReader.GetBaseUrl(),
        ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
        Locale = "en-US",
    };

    [SetUp]
    public async Task BaseSetUp()
    {
        // The frontend marks elements with a custom attribute instead of data-testid.
        Playwright.Selectors.SetTestIdAttribute("selenium-id");

        BaseUrl = ConfigReader.GetBaseUrl();
        Page.SetDefaultTimeout(ConfigReader.GetDefaultTimeoutSec() * 1000);
        Page.SetDefaultNavigationTimeout(ConfigReader.GetNavigationTimeoutSec() * 1000);

        if (ConfigReader.GetTracingMode() != "off")
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = TestContext.CurrentContext.Test.FullName,
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            });
        }
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        await StopCreatedSimulationsAsync();

        var tracingMode = ConfigReader.GetTracingMode();
        if (tracingMode == "off")
        {
            return;
        }

        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        string? path = null;
        if (tracingMode == "on" || failed)
        {
            var name = string.Concat(TestContext.CurrentContext.Test.FullName.Split(Path.GetInvalidFileNameChars()));
            path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-traces", $"{name}.zip");
        }

        await Context.Tracing.StopAsync(new() { Path = path });

        if (path is not null)
        {
            TestContext.AddTestAttachment(path, "Playwright trace (open with: playwright show-trace <file>)");
        }
    }

    protected Task<IAPIRequestContext> NewApiContextAsync() =>
        Playwright.APIRequest.NewContextAsync(new() { BaseURL = ConfigReader.GetApiBaseUrl() });

    private async Task StopCreatedSimulationsAsync()
    {
        if (_createdSimulationIds.Count == 0)
        {
            return;
        }

        var api = await NewApiContextAsync();
        try
        {
            foreach (var simId in _createdSimulationIds)
            {
                try
                {
                    var response = await api.DeleteAsync($"/api/engine/Simulation/stop/{simId}");
                    if (!response.Ok)
                    {
                        TestContext.Out.WriteLine($"Failed to stop simulation {simId}. Status code: {response.Status}");
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Out.WriteLine($"Exception while stopping simulation {simId}: {ex.Message}");
                }
            }
        }
        finally
        {
            _createdSimulationIds.Clear();
            await api.DisposeAsync();
        }
    }
}
