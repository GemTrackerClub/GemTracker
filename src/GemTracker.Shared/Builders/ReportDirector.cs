namespace GemTracker.Shared.Builders
{
    public class ReportDirector
    {
        public Report MakeReport(ReportBuilder reportBuilder)
        {
            reportBuilder.CreateNewReport();
            reportBuilder.SetReportType();

            return reportBuilder.GetReport();
        }
    }
}