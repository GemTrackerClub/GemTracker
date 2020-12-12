namespace GemTracker.Shared.Builders
{
    public class TelegramPremiumReport : ReportBuilder
    {
        public override void SetReportType()
        {
            reportObject.ReportType = "Telegram Premium Gem";
        }
    }
}