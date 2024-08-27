using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Trxlog2Html.ResultXmlElements
{
        public class TestSettingsElement
    {
        public TestSettingsElement()
        {
        }
            public TestSettingsElement(XElement elem)
        {
            Name = elem.Attribute("name").Value;
            Id = elem.Attribute("id").Value;
            runDeploymentRoot = elem.Elements().Where(x => x.Name.LocalName == "Deployment")?
                    .Select(d => d.Attribute("runDeploymentRoot").Value).ToArray();
        }

        public string Name { get; }
        public string Id { get; }
        public string[] runDeploymentRoot { get; }
    }
    /// <summary>
    /// UnitTestResult
    /// </summary>
    public class UnitTestResultElement
    {
        public UnitTestResultElement()
        {
        }

        public UnitTestResultElement(XElement elem)
        {
            TestId = elem.Attribute("testId").Value;
            ExecutionId = elem.Attribute("executionId").Value;
            Duration = elem.Attribute("duration").Value;
            Outcome = elem.Attribute("outcome").Value;
            TestName = elem.Attribute("testName").Value;
            ComputerName = elem.Attribute("computerName").Value;
            ResultFiles = elem.Elements().FirstOrDefault(x => x.Name.LocalName == "ResultFiles")?
                    .Elements().Select(e => e.Attribute("path").Value).ToArray();
        }

        /// <summary>
        /// testId
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// executionId
        /// </summary>
        public string ExecutionId { get; set; }

        public string ComputerName { get; set; }

        /// <summary>
        /// duration
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// outcome
        /// </summary>
        public string Outcome { get; set; }

        /// <summary>
        /// testName
        /// <summary>
        public string TestName { get; set; }
        public IEnumerable<string> ResultFiles { get; set; }
    }

    /// <summary>
    /// UnitTest
    /// </summary>
    public class UnitTestElement
    {

        public UnitTestElement()
        {

        }

        public UnitTestElement(XElement elem)
        {
            Id = elem.Attribute("id").Value;
            Name = elem.Attribute("name").Value;
            var testMethodElem = elem.Elements().Where(x => x.Name.LocalName == "TestMethod").FirstOrDefault();
            if (testMethodElem != null)
            {
                TestMethod = new TestMethodElement(testMethodElem);
            }
        }


        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// TestMethod
        /// </summary>
        public TestMethodElement TestMethod { get; set; }
    }

    /// <summary>
    /// TestMethod
    /// </summary>
    public class TestMethodElement
    {
        public TestMethodElement()
        {

        }

        public TestMethodElement(XElement elem)
        {
            ClassName = elem.Attribute("className").Value;
            Name = elem.Attribute("name").Value;
        }

        /// <summary>
        /// className
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// ResultSummary
    /// </summary>
    public class ResultSummaryElement
    {
        public ResultSummaryElement()
        {
        }

        public ResultSummaryElement(XElement elem)
        {
            var countersElem = elem.Elements().Where(x => x.Name.LocalName == "Counters").FirstOrDefault();
            if (countersElem != null)
            {
                Counters = new CountersElement(countersElem);
            }
        }
        public CountersElement Counters { get; set; }
    }

    /// <summary>
    /// Counters
    /// </summary>
    public class CountersElement
    {
        public CountersElement()
        {
        }

        public CountersElement(XElement elem)
        {
            Total = elem.Attribute("total")?.Value;
            Executed = elem.Attribute("executed")?.Value;
            Passed = elem.Attribute("passed")?.Value;
            Failed = elem.Attribute("failed")?.Value;
            Error = elem.Attribute("error")?.Value;
            Timeout = elem.Attribute("timeout")?.Value;
            Aborted = elem.Attribute("aborted")?.Value;
            Inconclusive = elem.Attribute("inconclusive")?.Value;
            PassedButRunAborted = elem.Attribute("passedButRunAborted")?.Value;
            NotRunnable = elem.Attribute("notRunnable")?.Value;
            NotExecuted = elem.Attribute("notExecuted")?.Value;
            Disconnected = elem.Attribute("disconnected")?.Value;
            Warning = elem.Attribute("warning")?.Value;
            Completed = elem.Attribute("completed")?.Value;
            InProgress = elem.Attribute("inProgress")?.Value;
            Pending = elem.Attribute("pending")?.Value;
        }

        public string Total { get; set; }

        public string Executed { get; set; }

        public string Passed { get; set; }

        public string Failed { get; set; }

        public string Error { get; set; }

        public string Timeout { get; set; }

        public string Aborted { get; set; }

        public string Inconclusive { get; set; }

        public string PassedButRunAborted { get; set; }

        public string NotRunnable { get; set; }

        public string NotExecuted { get; set; }

        public string Disconnected { get; set; }

        public string Warning { get; set; }

        public string Completed { get; set; }

        public string InProgress { get; set; }

        public string Pending { get; set; }



    }
}
