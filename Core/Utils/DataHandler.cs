using System.Reflection;
using System.Text.Json;
using Xunit;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PWCli.Core.Utils;

public class JsonDataAttribute : DataAttribute
{
    private readonly string _fileName;
    public JsonDataAttribute(string fileName) => _fileName = fileName;

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var json = File.ReadAllText(Path.Combine("TestScripts", "Cases", _fileName));
        var data = JsonSerializer.Deserialize<TestData[]>(json) ?? [];
        return data.Select(item => new object[] { item });
    }
}

public record TestData(string Framework, string Text);

public static class TestLogger
{
    private static readonly object _lock = new object();
    private static int _testCounter = 0;

    public static void LogTestStart(string testName)
    {
        lock (_lock)
        {
            var testNumber = Interlocked.Increment(ref _testCounter);
            Console.WriteLine();
            Console.WriteLine("=================================================================");
            Console.WriteLine($"🧪 TEST #{testNumber}: {testName}");
            Console.WriteLine($"⏰ Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine("=================================================================");
        }
    }

    public static void LogStep(string step)
    {
        lock (_lock)
        {
            Console.WriteLine($"   ➤ [{DateTime.Now:HH:mm:ss.fff}] {step}");
        }
    }

    public static void LogPass(string testName, TimeSpan duration)
    {
        lock (_lock)
        {
            Console.WriteLine($"   ✅ [{DateTime.Now:HH:mm:ss.fff}] PASSED - Duration: {duration.TotalMilliseconds:F0}ms");
            Console.WriteLine("=================================================================");
        }
    }

    public static void LogFail(string testName, string error, TimeSpan duration)
    {
        lock (_lock)
        {
            Console.WriteLine($"   ❌ [{DateTime.Now:HH:mm:ss.fff}] FAILED - Duration: {duration.TotalMilliseconds:F0}ms");
            Console.WriteLine($"   📝 Error: {error}");
            Console.WriteLine("=================================================================");
        }
    }
}