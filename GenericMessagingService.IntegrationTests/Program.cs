using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.IntegrationTests.Helpers;
using GenericMessagingService.IntegrationTests.Servers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
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
        private static ILogger Logger;
        private static IntegrationTestSettings settings;

        public static void Main(string[] args)
        {
            settings = ParseArgs(args);
            Logger = new ComboLogger(
                new ConsoleLogger(),
                new FileLogger("C:\\GMSTests\\Log\\logfile.txt"));
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
            return settings;
        }

        private static Dictionary<string, Dictionary<string, MethodInfo>> DiscoverTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(x => x.GetCustomAttribute<TestAttribute>() != null);
            var typeDict = new Dictionary<string, List<Type>>();
            foreach (var type in types)
            {
                var serverName = type.GetCustomAttribute<TestAttribute>().ServerName;
                if (!typeDict.ContainsKey(serverName))
                {
                    typeDict[serverName] = new List<Type>();
                }
                typeDict[serverName].Add(type);
                if (!testObjectCache.ContainsKey(type))
                {
                    var server = servers[serverName];
                    testObjectCache[type] = type.GetConstructors()[0].Invoke(new object[] {Logger, server.WebClientSettings});
                }
            }
            var tests = typeDict
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.SelectMany(t => t.GetMethods().Where( m => m.GetCustomAttribute<TestNameAttribute>() != null))
                        .ToDictionary(
                            m => m.GetCustomAttribute<TestNameAttribute>().Name,
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
                servers[serverName] = (BaseServer)type.GetConstructors()[0].Invoke(new object[] { settings.HostUrl, settings.ApiKey });
            }
        }


        private static Dictionary<string, BaseServer> servers = new Dictionary<string, BaseServer>();
        private static Dictionary<Type, object> testObjectCache = new Dictionary<Type, object>();

        private static async Task RunTests(Dictionary<string, Dictionary<string, MethodInfo>> tests)
        {
            Logger.Log("Starting test run");
            Logger.Log("");
            foreach(var (serverName, testMethods) in tests)
            {
                Logger.Log($"Running tests for {serverName}");
                Logger.Log("Starting server");
                Server server;
                try
                {
                    server = servers[serverName].Get();
                } catch(Exception ex)
                {
                    Logger.Log("Server start failed");
                    Logger.Log(ex.Message);
                    Logger.Log(ex.StackTrace);
                    continue;
                }
                Logger.Log("Server started");
                Logger.Log("");
                foreach(var (testName, method) in testMethods)
                {
                    await RunTest(testName, method);
                }
                await server.DisposeAsync();
            }
        }

        private static async Task RunTest(string testName, MethodInfo test)
        {
            Logger.Log($"Running test '{testName}'");
            var obj = testObjectCache[test.DeclaringType];
            try
            {
                test.Invoke(obj, new object[] { });
                Logger.Log("Test completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Log("Test failed");
                Logger.Log(ex.Message);
                Logger.Log(ex.StackTrace);
            }
            finally
            {
                Logger.Log("");
            }
        }
    }
}
