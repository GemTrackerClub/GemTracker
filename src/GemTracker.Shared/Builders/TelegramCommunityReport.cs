namespace GemTracker.Shared.Builders
{
    public class TelegramCommunityReport : ReportBuilder
    {
        public override void SetReportType()
        {
            reportObject.ReportType = "Telegram Community Gem";
        }
    }
}