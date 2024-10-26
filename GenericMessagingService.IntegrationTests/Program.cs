using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.IntegrationTests.Helpers;
using GenericMessagingService.IntegrationTests.Servers;
using GenericMessagingService.IntegrationTests.Tests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests
{
    /// <summary>
    /// Integration test application.
    /// Should be able to start a version of the WebAPI, then use the client to fire requests at it
    /// </summary>
    public class Program
    {
        private static ILoggerProvider loggerProvider;
        private static ILogger Logger;
        private static IntegrationTestSettings settings;

        public static void Main(string[] args)
        {
            var logger = LoggerFactory.Create(x => x.AddTestLogging("C:\\GMSTests\\Log")).CreateLogger("Test");
            Logger = logger;
            ServerHelper.logger = logger;

            settings = ParseArgs(args);
            DiscoverServers();
            var tests = DiscoverTests();
            RunTests(tests).Wait();
        }
        private static IntegrationTestSettings ParseArgs(string[] args)
        {
            var settings = new IntegrationTestSettings { All = true, HostUrl = "https://localhost:58008", ApiKey = "integration-test-api-key"};
            for (var i = 0; i < args.Length; i++) 
            {
                var arg = args[i];
                if(arg == "--name")
                {
                    i++;
                    settings.All = false;
                    settings.Names = new[] { args[i] }; 
                }
                if(arg == "--names")
                {
                    i++;
                    settings.All = false;
                    settings.Names = args[i].Split(',');
                }
                if(arg == "--url")
                {
                    i++;
                    settings.HostUrl = args[i];
                }
                if(arg == "--api-key")
                {
                    i++;
                    settings.ApiKey = args[i];
                }
            }
            if (args.Any())
            {
                Logger.LogDebug("Running Integration Tests with args: " + string.Join(" ", args));
            } else
            {
                Logger.LogDebug("Running Integration Tests with no args");
            }
            return settings;
        }

        private static Dictionary<string, Dictionary<string, MethodInfo>> DiscoverTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<TestFixtureAttribute>() != null);
            var typeDict = new Dictionary<string, List<Type>>();
            foreach (var type in types)
            {
                var serverName = type.GetCustomAttribute<TestFixtureAttribute>().ServerName;
                if(settings.Names != null && !settings.Names.Contains(serverName))
                {
                    continue;
                }
                if (!typeDict.ContainsKey(serverName))
                {
                    typeDict[serverName] = new List<Type>();
                }
                typeDict[serverName].Add(type);
                if (!testObjectCache.ContainsKey(type))
                {
                    var server = servers[serverName];
                    var testBase = (TestBase)type.GetConstructors()[0].Invoke(new object[] { });
                    testBase.logger = Logger;
                    testBase.clientSettings = server.WebClientSettings;
                    testBase.gmsSettings = server.Settings;

                    testObjectCache[type] = testBase;
                }
            }
            var tests = typeDict
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.SelectMany(t => t.GetMethods().Where( m => m.GetCustomAttribute<TestAttribute>() != null))
                        .ToDictionary(
                            m => {
                                var fixtureName = m.DeclaringType.GetCustomAttribute<TestFixtureAttribute>()!.TestFixtureName ?? m.DeclaringType.Name;
                                var testName = m.GetCustomAttribute<TestAttribute>()!.TestName ?? m.Name;
                                return fixtureName + ":" + testName;
                            },
                            m => m));
            return tests;
        }

        private static void DiscoverServers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<ServerAttribute>() != null);
            foreach (var type in types) 
            {
                var serverName = type.GetCustomAttribute<ServerAttribute>().Name;
                var server = (BaseServer)type.GetConstructors()[0].Invoke(new object[0]);
                server.hostUrl = settings.HostUrl;
                server.apiKey = settings.ApiKey;
                servers[serverName] = server;
            }
        }


        private static Dictionary<string, BaseServer> servers = new Dictionary<string, BaseServer>();
        private static Dictionary<Type, object> testObjectCache = new Dictionary<Type, object>();

        private static async Task RunTests(Dictionary<string, Dictionary<string, MethodInfo>> tests)
        {
            var timer = new Stopwatch();
            timer.Start();
            Logger.LogDebug("Starting test run");
            var testsCount = tests.Select(x => x.Value.Count).Sum();
            var testNumber = 1;
            var failures = new List<string>();
            Logger.LogDebug(testsCount + " tests found");
            foreach(var (serverName, testMethods) in tests)
            {
                Logger.LogDebug($"Running tests for server '{serverName}'");
                Logger.LogDebug("Starting server");
                Server server;
                try
                {
                    server = servers[serverName].Get();
                } catch(Exception ex)
                {
                    Logger.LogDebug("Server start failed");
                    Logger.LogDebug(ex.Message);
                    Logger.LogDebug(ex.StackTrace);
                    failures.AddRange(testMethods.Select(x => $"'{x.Key}': Server start failed"));
                    continue;
                }
                Logger.LogDebug("Server started");
                foreach(var (testName, test) in testMethods)
                {
                    Logger.LogDebug($"Running test '{testName}' {testNumber} of {testsCount}");
                    testNumber++;
                    var obj = testObjectCache[test.DeclaringType];
                    try
                    {
                        test.Invoke(obj, new object[] { });
                        Logger.LogDebug("Test completed successfully");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDebug("Test failed");
                        Logger.LogDebug(ex.Message);
                        Logger.LogDebug(ex.StackTrace);
                        failures.Add($"'{testName}': Test failed");
                    }
                }
                await server.DisposeAsync();
                Logger.LogDebug($"Tests for server '{serverName}' completed");
            }
            var elapsedTime = timer.Elapsed;

            Logger.LogDebug($"Test run completed in {elapsedTime.ToString("hh\\:mm\\:ss")}");
            if (failures.Any())
            {
                Logger.LogDebug($"{failures.Count} tests failed");
                foreach (var failure in failures) { Logger.LogDebug(failure); }
            }
            else { 
                Logger.LogDebug("All tests succeeded");
            }
        }
    }
}
