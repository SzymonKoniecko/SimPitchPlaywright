# SimPitch.Playwright

Automated E2E tests for the SimPitch frontend (Vue 3) built with **Playwright for .NET** + **NUnit**.

## Requirements

- .NET SDK 9.0
- Running SimPitch app (by default `http://localhost:8080`, see `appsettings.json`)

## First-time setup

```bash
dotnet build
```

Install the browsers (once per Playwright version):

```bash
# with PowerShell (pwsh)
pwsh SimPitchPlaywright/bin/Debug/net9.0/playwright.ps1 install chromium

# without PowerShell (macOS arm64 – use linux-x64 / darwin-x64 etc. on other platforms)
SimPitchPlaywright/bin/Debug/net9.0/.playwright/node/darwin-arm64/node \
  SimPitchPlaywright/bin/Debug/net9.0/.playwright/package/cli.js install chromium
```

## Running tests

```bash
dotnet test
```

Useful options:

```bash
# headed browser
dotnet test -- Playwright.LaunchOptions.Headless=false

# a different browser (requires `install firefox` / `install webkit`)
dotnet test -- Playwright.BrowserName=firefox

# a different app URL (environment variables override appsettings.json)
BASEURL=http://localhost:5173 dotnet test

# a single test
dotnet test --filter "FullyQualifiedName~NavigationTests"
```

## Configuration

| File | What it configures |
| --- | --- |
| `SimPitchPlaywright/.runsettings` | browser, headless, `Expect` timeout, number of parallel workers |
| `SimPitchPlaywright/appsettings.json` | `baseUrl`, `apiBaseUrl`, timeouts, tracing mode (`off` / `on` / `retain-on-failure`) |

Every key in `appsettings.json` can be overridden with an environment variable (`timeouts.default` → `TIMEOUTS_DEFAULT`).

## Traces

With `retain-on-failure` mode, a trace of each failed test is saved to
`bin/Debug/net9.0/playwright-traces/*.zip` and attached to the test result. To view it:

```bash
SimPitchPlaywright/bin/Debug/net9.0/.playwright/node/darwin-arm64/node \
  SimPitchPlaywright/bin/Debug/net9.0/.playwright/package/cli.js show-trace <file.zip>
```

## Structure

```
SimPitchPlaywright/
├── Models/     # test data models (PrepareSimulationModel, SimulationModelEnum)
├── Pages/      # Page Objects (BasePage, NavBarPage, MainPage, PrepareSimulationPage, ...)
├── Tests/      # tests (BaseTest : PageTest)
└── Utils/      # helpers (ConfigReader, TextHelper, WaitHelper)
```

The tests mirror the Selenium suite (`src/Selenium`) 1:1. Every test name has the `pw_` prefix,
e.g. `pw_Nav_Should_Display_NavBar_Text`, so you can run them selectively:

```bash
dotnet test --filter "Name~pw_SimulationItem"
```

Simulations created by tests (`_createdSimulationIds`) are stopped through the API
(`DELETE /api/engine/Simulation/stop/{id}`) in `BaseTest` teardown.

Elements are located by the `selenium-id` attribute (shared with the Selenium tests) – `BaseTest`
registers it as the test-id attribute, so `Page.GetByTestId("home-nav")` works.
