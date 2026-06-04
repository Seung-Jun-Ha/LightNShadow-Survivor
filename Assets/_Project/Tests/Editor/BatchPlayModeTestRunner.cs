using System;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightNShadowSurvivor.Tests
{
    public static class BatchPlayModeTestRunner
    {
        private const string TestAssemblyName = "LightNShadowSurvivor.PlayModeTests";
        private static TestRunnerApi api;
        private static ResultCallbacks callbacks;
        private static double startTime;

        public static void RunFlashlightTests()
        {
            EditorApplication.update += StartWhenReady;
        }

        private static void StartWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            EditorApplication.update -= StartWhenReady;
            startTime = EditorApplication.timeSinceStartup;

            string resultPath = GetCommandLineValue("-testResults") ?? Path.Combine(Directory.GetCurrentDirectory(), "Temp", "flashlight-playmode-test-results.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(resultPath));

            api = ScriptableObject.CreateInstance<TestRunnerApi>();
            callbacks = new ResultCallbacks(resultPath);
            api.RegisterCallbacks(callbacks);

            var filter = new Filter
            {
                testMode = UnityEditor.TestTools.TestRunner.Api.TestMode.PlayMode,
                assemblyNames = new[] { TestAssemblyName }
            };

            Debug.Log($"BatchPlayModeTestRunner: running {TestAssemblyName}.");
            api.Execute(new UnityEditor.TestTools.TestRunner.Api.ExecutionSettings
            {
                filters = new[] { filter }
            });

            EditorApplication.update += Watchdog;
        }

        private static void Watchdog()
        {
            if (EditorApplication.timeSinceStartup - startTime < 120d)
            {
                return;
            }

            Debug.LogError("BatchPlayModeTestRunner: timed out waiting for test completion.");
            EditorApplication.Exit(3);
        }

        private static string GetCommandLineValue(string key)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], key, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private sealed class ResultCallbacks : ICallbacks
        {
            private readonly string resultPath;

            public ResultCallbacks(string resultPath)
            {
                this.resultPath = resultPath;
            }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"BatchPlayModeTestRunner: run started: {testsToRun.FullName}.");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                EditorApplication.update -= Watchdog;
                TestRunnerApi.SaveResultToFile(result, resultPath);
                Debug.Log($"BatchPlayModeTestRunner: run finished. Passed={result.PassCount}, Failed={result.FailCount}, Skipped={result.SkipCount}, Inconclusive={result.InconclusiveCount}. Results={resultPath}");
                EditorApplication.Exit(result.FailCount > 0 ? 2 : 0);
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                Debug.Log($"BatchPlayModeTestRunner: {result.FullName} => {result.ResultState}");
            }
        }
    }
}
