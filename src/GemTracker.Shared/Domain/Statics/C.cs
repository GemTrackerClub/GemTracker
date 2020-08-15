using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using System.IO;

namespace GemTracker.Shared.Domain.Statics
{
    public static class C
    {
        public static string JobFilePath(string jobName)
            => Path.Combine(Directory.GetCurrentDirectory(), $"{jobName}.json");
        public static string StorageFilePath(string path, string fileName)
            => Path.Combine(path, $"{fileName}.json");
        public static string Storage(string storagePath, UniswapApiVersion uniswapApiVersion, UniswapEndpoint uniswapEndpoint)
            => StorageFilePath(storagePath, $"uniswap-{uniswapApiVersion.GetDescription()}-{uniswapEndpoint.GetDescription()}");
        public static string StorageDeleted(string storagePath, UniswapApiVersion uniswapApiVersion, UniswapEndpoint uniswapEndpoint) 
            => StorageFilePath(storagePath, $"uniswap-{uniswapApiVersion.GetDescription()}-{uniswapEndpoint.GetDescription()}-deleted");
        public static string StorageAdded(string storagePath, UniswapApiVersion uniswapApiVersion, UniswapEndpoint uniswapEndpoint)
            => StorageFilePath(storagePath, $"uniswap-{uniswapApiVersion.GetDescription()}-{uniswapEndpoint.GetDescription()}-added");
    }
}