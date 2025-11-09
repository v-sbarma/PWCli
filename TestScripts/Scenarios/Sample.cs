using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using PWCli.Core.Utils;
using Xunit;

namespace PWCli.TestScripts.Scenarios;

[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection : ICollectionFixture<object> { }

[Collection("Sequential")]
public class Sample : PageTest
{
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
            
            TestLogger.LogStep("Navigating to Playwright documentation page");
            await Page.GotoAsync("https://playwright.dev/dotnet/docs/intro");
            
            TestLogger.LogStep($"Clicking {framework} framework tab");
            await Page.GetByRole(AriaRole.Tab, new() { Name = framework }).First.ClickAsync();
            
            TestLogger.LogStep($"Verifying '{attribute}' code attribute is visible");
            await Expect(Page.Locator($"code:has-text('{attribute}')").First).ToBeVisibleAsync();
            
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