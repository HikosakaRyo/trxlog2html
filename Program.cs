using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using RazorEngine;
using RazorEngine.Templating;
using System.IO;
using Trxlog2Html.ReportModels;
using Trxlog2Html.ResultXmlElements;
using System.Reflection;

namespace Trxlog2Html
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }


        public void All(
            [Option("i", "Input file path.")] string inputFilePath,
            [Option("o", "Output file path.")] string outputFilePath,
            [Option("t", "Template file path.")] string templateFilePath = null)
        {
            if (templateFilePath == null)
            {
                // use built-in template
                templateFilePath = Path.Combine(GetBuiltInTemplatesDir(), "jstest_like.cshtml");
            }
            var template = ReadTemplate(templateFilePath);
            var model = CreateModelFromXml(inputFilePath);
            var html = Engine.Razor.RunCompile(
                template, "templateKey", typeof(ReportModel), model);
            WriteResult(outputFilePath, html);
        }

        private string GetBuiltInTemplatesDir()
        {
            return Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "built_in_templates");
        }


        private string ReadTemplate(string path)
        {
            using (var reader = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void WriteResult(string path, string result)
        {
            using (var writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                writer.Write(result);
            }
        }

        /// <summary>
        /// create report models for razor engine from the trx log file.
        /// </summary>
        /// <param name="xmlPath">path of the trx log file</param>
        /// <returns></returns>
        private ReportModel CreateModelFromXml(string xmlPath)
        {
            var doc = XDocument.Load(xmlPath);
            var unitTestClasses = doc.Descendants()
                .Where(x => x.Name.LocalName == "UnitTest")
                .Select(x => new UnitTestElement(x))
                .Where(x => x.TestMethod != null)
                .GroupBy(x => x.TestMethod.ClassName)
                .OrderBy(x => x.Key)
                .ToList();
            var unitTestResults = doc.Descendants()
                .Where(x => x.Name.LocalName == "UnitTestResult")
                .Select(x => new UnitTestResultElement(x))
                .GroupBy(x => x.TestId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var testSettings = doc.Descendants().
                Where(x => x.Name.LocalName == "TestSettings")
                .Select(x => new TestSettingsElement(x))
                .FirstOrDefault();

            var resultSummary = doc.Descendants().
                Where(x => x.Name.LocalName == "ResultSummary")
                .Select(x => new ResultSummaryElement(x))
                .FirstOrDefault();

            var startTime = doc.Descendants().
                Where(x => x.Name.LocalName == "Times")
                .Select(x => x.Attribute("start").Value)
                .FirstOrDefault();
            var model = new ReportModel();
            model.Summary = Map(resultSummary);
            model.TestClasses = unitTestClasses.Select(x => Map(x, unitTestResults)).ToList();
            model.StartTime = startTime;
            model.Settings = Map(testSettings);
            return model;
        }
        private ReportTestClassModel Map(
            IGrouping<string, UnitTestElement> src,
            Dictionary<string, List<UnitTestResultElement>> testResults)
        {
            var ret = new ReportTestClassModel();
            ret.ClassName = src.Key;

            ret.TestResults = src
                .SelectMany(x => Map(x, testResults))
                .Where(x => x != null)
                .OrderBy(x => x.TestMethod)
                .ToList();
            return ret;
        }
        private IEnumerable<ReportTestResultModel> Map(
            UnitTestElement src,
            Dictionary<string, List<UnitTestResultElement>> testResults)
        {
            if (testResults.TryGetValue(src.Id, out var testResult))
            {
                foreach (var result in testResult)
                {
                    yield return new ReportTestResultModel
                    {
                        DisplayName = result.TestName,
                        TestMethod = src.TestMethod.Name,
                        Duration = result.Duration,
                        Outcome = result.Outcome,
                        ExecutionId = result.ExecutionId,
                        ComputerName = result.ComputerName,
                        ResultFiles = result.ResultFiles ?? Enumerable.Empty<string>(),
                    };
                }
            }
        }
        private TestSettingsModel  Map(TestSettingsElement src)
        {
            var ret = new TestSettingsModel()
            {
                Name = src.Name,
                Id = src.Id,
                DeploymentRoot = src.runDeploymentRoot.FirstOrDefault(),
            };
            return ret;
        }
        private ReportSummaryModel Map(ResultSummaryElement src)
        {
            var ret = new ReportSummaryModel()
            {
                Aborted = src.Counters.Aborted,
                Completed = src.Counters.Completed,
                Disconnected = src.Counters.Disconnected,
                Error = src.Counters.Error,
                Executed = src.Counters.Executed,
                Failed = src.Counters.Failed,
                Inconclusive = src.Counters.Inconclusive,
                InProgress = src.Counters.InProgress,
                NotExecuted = src.Counters.NotExecuted,
                NotRunnable = src.Counters.NotRunnable,
                Passed = src.Counters.Passed,
                PassedButRunAborted = src.Counters.PassedButRunAborted,
                Pending = src.Counters.Pending,
                Timeout = src.Counters.Timeout,
                Total = src.Counters.Total,
                Warning = src.Counters.Warning,
            };
            return ret;
        }
    }
}
