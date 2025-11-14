using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation()
    .AddSource("*")
    .AddOtlpExporter()
    .Build();
using var metricsProvider = Sdk.CreateMeterProviderBuilder()
    .AddHttpClientInstrumentation()
    .AddMeter("*")
    .AddOtlpExporter()
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.ClearProviders();
    builder.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
        options.SingleLine = true;
        options.TimestampFormat = "[HH:mm:ss] ";
    });
    builder.SetMinimumLevel(LogLevel.Debug);
    builder.AddOpenTelemetry(opt => opt.AddOtlpExporter());
    // builder.AddConsole();
    // builder.SetMinimumLevel(LogLevel.Information);
});

var openAiClient = new OpenAIClient(builder.Configuration["OPEN_API_KEY"]).GetChatClient("gpt-4o-mini");

// Create a sampling client.
using var samplingClient = openAiClient.AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
    .Build();

var mcpClient = await McpClient.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Name = "Demo Server",
        Command = "dotnet",
        Arguments = ["run", "--project", Path.Combine(GetCurrentSourceDirectory(), "../McpCrmDemo.Server")],
    }),
    clientOptions: new McpClientOptions
    {
        Handlers = new McpClientHandlers
        {
            SamplingHandler = samplingClient.CreateSamplingHandler()
        }
    },
    loggerFactory: loggerFactory);

// Get all available tools
Console.WriteLine("Tools available:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"  {tool}");
}

Console.WriteLine();

// Create an IChatClient that can use the tools.
using var chatClient = openAiClient.AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
    .Build();

// Have a conversation, making all tools available to the LLM.
List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Q: ");
    messages.Add(new ChatMessage(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (var update in chatClient.GetStreamingResponseAsync(messages, new ChatOptions { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }

    Console.WriteLine();

    messages.AddMessages(updates);
}

static string GetCurrentSourceDirectory([CallerFilePath] string? currentFile = null)
{
    Debug.Assert(!string.IsNullOrWhiteSpace(currentFile));
    return Path.GetDirectoryName(currentFile) ??
           throw new InvalidOperationException("Unable to determine source directory.");
}