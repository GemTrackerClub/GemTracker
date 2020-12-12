namespace GemTracker.Shared.Builders
{
    public class TelegramPremiumVerifiedReport : ReportBuilder
    {
        public override void SetReportType()
        {
            reportObject.ReportType = "Telegram Premium Verified Gem";
        }
    }
}