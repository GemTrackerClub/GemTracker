using System.IO;

namespace GemTracker.Shared.Domain.Statics
{
    public static class PathTo
    {
        public static string Job(string jobName)
            => Path.Combine(Directory.GetCurrentDirectory(), $"{jobName}.json");
        public static string Storage(string path, string fileName)
            => Path.Combine(path, $"{fileName}.json");
        public static string All(string storagePath)
            => Storage(storagePath, $"uniswap-v2-all");
        public static string Deleted(string storagePath)
            => Storage(storagePath, $"uniswap-v2-all-deleted");
        public static string Added(string storagePath)
            => Storage(storagePath, $"uniswap-v2-all-added");
    }
}