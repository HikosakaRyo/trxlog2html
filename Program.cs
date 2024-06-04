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
                .Where(x => x.TestMethod != null && x.Execution != null)
                .GroupBy(x => x.TestMethod.ClassName)
                .OrderBy(x => x.Key)
                .ToList();
            var unitTestResults = doc.Descendants()
                .Where(x => x.Name.LocalName == "UnitTestResult")
                .Select(x => new UnitTestResultElement(x)).ToDictionary(x => x.ExecutionId);

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
            return model;
        }

        private ReportTestClassModel Map(
            IGrouping<string, UnitTestElement> src,
            Dictionary<string, UnitTestResultElement> testResults)
        {
            var ret = new ReportTestClassModel();
            ret.ClassName = src.Key;

            ret.TestResults = src
                .Select(x => Map(x, testResults))
                .Where(x => x != null)
                .OrderBy(x => x.TestMethod)
                .ThenBy(x => x.TestMethod)
                .ToList();
            return ret;
        }

        private ReportTestResultModel Map(
            UnitTestElement src,
            Dictionary<string, UnitTestResultElement> testResults)
        {
            if (!testResults.TryGetValue(src.Execution.Id, out var testResult))
            {
                return null;
            }
            var ret = new ReportTestResultModel();
            ret.DisplayName = src.Name;
            ret.TestMethod = src.TestMethod.Name;
            ret.Duration = testResult.Duration;
            ret.Outcome = testResult.Outcome;
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
