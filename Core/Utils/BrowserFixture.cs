using Microsoft.Playwright;
using Xunit;

namespace PWCli.Core.Utils;

/// <summary>
/// Shared browser fixture for efficient test execution.
/// Opens browser once, runs all tests in series, then closes browser.
/// </summary>
public class SharedBrowserFixture : IAsyncLifetime
{
    public IBrowser Browser { get; private set; } = null!;
    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var playwright = await Playwright.CreateAsync();
        
        // Read settings from environment variables
        var headless = Environment.GetEnvironmentVariable("HEADED") != "1";
        var slowMo = int.Parse(Environment.GetEnvironmentVariable("BROWSER_SLOWMO") ?? "300");
        
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
            SlowMo = slowMo,
            Args = new[] { "--start-maximized" }
        });
        
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = ViewportSize.NoViewport // Use full screen
        });
        
        Page = await Context.NewPageAsync();
        
        // Pre-navigate to the documentation page once
        await Page.GotoAsync("https://playwright.dev/dotnet/docs/intro");
    }

    public async Task DisposeAsync()
    {
        if (Page != null) await Page.CloseAsync();
        if (Context != null) await Context.CloseAsync();
        if (Browser != null) await Browser.CloseAsync();
    }
}

/// <summary>
/// Base class for test scenarios that use shared browser instance.
/// Provides common functionality and browser access.
/// </summary>
public abstract class SharedBrowserTestBase : IClassFixture<SharedBrowserFixture>
{
    protected readonly SharedBrowserFixture BrowserFixture;
    protected IPage Page => BrowserFixture.Page;
    protected IBrowserContext Context => BrowserFixture.Context;
    protected IBrowser Browser => BrowserFixture.Browser;

    protected SharedBrowserTestBase(SharedBrowserFixture browserFixture)
    {
        BrowserFixture = browserFixture;
    }

    /// <summary>
    /// Reset page state between tests (optional - call if needed)
    /// </summary>
    protected async Task ResetPageStateAsync()
    {
        // Clear any dialogs, alerts, or overlays
        try
        {
            await Page.Keyboard.PressAsync("Escape");
            await Task.Delay(100);
        }
        catch
        {
            // Ignore if no dialogs to close
        }
    }

    /// <summary>
    /// Navigate to a fresh page if needed (optional utility)
    /// </summary>
    protected async Task NavigateToFreshPageAsync(string url)
    {
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Enhanced click with highlight and wait functionality
    /// </summary>
    protected async Task ClickWithHighlightAsync(ILocator locator)
    {
        var waitTime = int.Parse(Environment.GetEnvironmentVariable("UI_WAIT_TIME") ?? "500");
        var highlightColor = Environment.GetEnvironmentVariable("UI_HIGHLIGHT_COLOR") ?? "yellow";
        
        // Highlight element
        await locator.EvaluateAsync($"element => {{ element.style.backgroundColor = '{highlightColor}'; element.style.border = '2px solid red'; }}");
        
        // Wait before action
        await Task.Delay(waitTime);
        
        // Perform click
        await locator.ClickAsync();
        
        // Remove highlight
        await locator.EvaluateAsync("element => { element.style.backgroundColor = ''; element.style.border = ''; }");
        
        // Wait after action
        await Task.Delay(waitTime);
    }

    /// <summary>
    /// Enhanced type with highlight and wait functionality
    /// </summary>
    protected async Task TypeWithHighlightAsync(ILocator locator, string text)
    {
        var waitTime = int.Parse(Environment.GetEnvironmentVariable("UI_WAIT_TIME") ?? "500");
        var highlightColor = Environment.GetEnvironmentVariable("UI_HIGHLIGHT_COLOR") ?? "yellow";
        
        // Highlight element
        await locator.EvaluateAsync($"element => {{ element.style.backgroundColor = '{highlightColor}'; element.style.border = '2px solid red'; }}");
        
        // Wait before action
        await Task.Delay(waitTime);
        
        // Perform type
        await locator.FillAsync(text);
        
        // Remove highlight
        await locator.EvaluateAsync("element => { element.style.backgroundColor = ''; element.style.border = ''; }");
        
        // Wait after action
        await Task.Delay(waitTime);
    }
}