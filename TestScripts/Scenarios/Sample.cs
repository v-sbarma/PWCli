using Microsoft.Playwright;
using PWCli.Core.Utils;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace PWCli.TestScripts.Scenarios;

[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection : ICollectionFixture<SharedBrowserFixture> { }

[Collection("Sequential")]
public class Sample : SharedBrowserTestBase
{
    public Sample(SharedBrowserFixture browserFixture) : base(browserFixture) { }

    [Fact]
    public async Task MSTest() => await RunFrameworkTest("MSTest", "TestMethod");
    
    [Fact] 
    public async Task NUnit() => await RunFrameworkTest("NUnit", "Test");
    
    [Fact]
    public async Task xUnit() => await RunFrameworkTest("xUnit", "Fact");

    private async Task RunFrameworkTest(string framework, string attribute)
    {
        var startTime = DateTime.Now;
        var testName = $"{framework} Framework Test";
        
        try
        {
            TestLogger.LogTestStart(testName);
            
            TestLogger.LogStep($"Clicking {framework} framework tab");
            await ClickWithHighlightAsync(Page.GetByRole(AriaRole.Tab, new() { Name = framework }).First);
            
            TestLogger.LogStep($"Verifying '{attribute}' code attribute is visible");
            var codeLocator = Page.Locator($"code:has-text('{attribute}')").First;
            await Expect(codeLocator).ToBeVisibleAsync();
            
            // Highlight the found element briefly
            var waitTime = int.Parse(Environment.GetEnvironmentVariable("UI_WAIT_TIME") ?? "500");
            var highlightColor = Environment.GetEnvironmentVariable("UI_HIGHLIGHT_COLOR") ?? "yellow";
            await codeLocator.EvaluateAsync($"element => {{ element.style.backgroundColor = '{highlightColor}'; }}");
            await Task.Delay(waitTime);
            await codeLocator.EvaluateAsync("element => { element.style.backgroundColor = ''; }");
            
            var duration = DateTime.Now - startTime;
            TestLogger.LogPass(testName, duration);
        }
        catch (Exception ex)
        {
            var duration = DateTime.Now - startTime;
            TestLogger.LogFail(testName, ex.Message, duration);
            throw;
        }
    }
}