namespace GemTracker.Shared.Builders
{
    public abstract class ReportBuilder
    {
        protected Report reportObject;
        public abstract void SetReportType();
        public void CreateNewReport()
        {
            reportObject = new Report();
        }
        public Report GetReport()
            => reportObject;
    }
}