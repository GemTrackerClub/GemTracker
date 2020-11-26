using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Extensions;
using System.IO;

namespace GemTracker.Shared.Domain.Statics
{
    public static class PathTo
    {
        public static string Job(string jobName)
            => Path.Combine(Directory.GetCurrentDirectory(), $"{jobName}.json");
        public static string Storage(string path, string fileName)
            => Path.Combine(path, $"{fileName}.json");
        public static string All(DexType dexType, string storagePath)
            => Storage(storagePath, $"{dexType.GetDescription()}-all");
        public static string Deleted(DexType dexType, string storagePath)
            => Storage(storagePath, $"{dexType.GetDescription()}-all-deleted");
        public static string Added(DexType dexType, string storagePath)
            => Storage(storagePath, $"{dexType.GetDescription()}-all-added");
    }
}