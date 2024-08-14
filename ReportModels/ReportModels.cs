using System.Collections.Generic;

namespace Trxlog2Html.ReportModels
{
    /// <summary>
    /// root class of model
    /// </summary>
    public class ReportModel
    {
        public string StartTime { get; set; }

        public ReportSummaryModel Summary { get; set; }

        public List<ReportTestClassModel> TestClasses { get; set; }
    }

    /// <summary>
    /// report test class
    /// </summary>
    public class ReportTestClassModel
    {

        public string ClassName { get; set; }

        public List<ReportTestResultModel> TestResults { get; set; }
    }

    /// <summary>
    /// report test result
    /// </summary>
    public class ReportTestResultModel
    {
        public string TestMethod { get; set; }

        public string DisplayName { get; set; }

        public string Duration { get; set; }

        public string Outcome { get; set; }
        public IEnumerable<string> ResultFiles { get; set; }
    }

    /// <summary>
    /// report summary
    /// </summary>
    public class ReportSummaryModel
    {
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
