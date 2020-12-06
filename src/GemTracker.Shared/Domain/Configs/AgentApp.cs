using GemTracker.Shared.Domain.Configs.Jobs;
using GemTracker.Shared.Domain.Configs.Services;
using System.Collections.Generic;

namespace GemTracker.Shared.Domain.Configs
{
    public class AgentApp
    {
        public string StoragePath { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public IEnumerable<TelegramConfig> Telegram { get; set; }
        public TwitterConfig TwitterConfig { get; set; }
        public EtherScanConfig EtherScan { get; set; }
        public EthPlorerConfig EthPlorer { get; set; }
    }
}