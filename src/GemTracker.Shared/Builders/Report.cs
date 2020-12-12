using System;
using System.Collections.Generic;
using System.Text;

namespace GemTracker.Shared.Builders
{
    public class Report
    {
        public string ReportType { get; set; }
        public string DisplayReport()
            => $"Report from {ReportType}";
    }
}