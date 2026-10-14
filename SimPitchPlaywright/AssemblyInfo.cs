using NUnit.Framework;

// Run tests in parallel across different test fixtures (classes).
// Each Playwright test gets its own isolated BrowserContext, so this is safe.
[assembly: Parallelizable(ParallelScope.Fixtures)]
