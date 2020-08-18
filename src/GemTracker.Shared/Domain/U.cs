using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Domain.Statics;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using GemTracker.Shared.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GemTracker.Shared.Domain
{
    public class U
    {
        private readonly IUniswapService _uniswapService;
        private readonly IFileService _fileService;

        public UniswapApiVersion ApiVersion { get; private set; }
        public UniswapEndpoint UniswapEndpoint { get; private set; }
        public string StorageFilePath { get; private set; }
        public string StorageFilePathDeleted { get; private set; }
        public string StorageFilePathAdded { get; private set; }
        public IEnumerable<Token> Tokens { get; set; }
        public U(IUniswapService uniswapService,
            IFileService fileService)
        {
            _uniswapService = uniswapService;
            _fileService = fileService;
        }
        public void SetPaths(string storagePath)
        {
            ApiVersion = UniswapApiVersion.V2;
            UniswapEndpoint = UniswapEndpoint.GRAPH;
            StorageFilePath = C.Storage(storagePath, ApiVersion, UniswapEndpoint);
            StorageFilePathDeleted = C.StorageDeleted(storagePath, ApiVersion, UniswapEndpoint);
            StorageFilePathAdded = C.StorageAdded(storagePath, ApiVersion, UniswapEndpoint);
        }
        public async Task<UniswapResponse> FetchFromUniswap(int maxSize = 5000)
        {
            var result = new UniswapResponse();

            var response = await _uniswapService.FetchAllAsync(maxSize);

            if (response.Success)
            {
                result.Tokens = response.Tokens;
            }
            return result;
        }
        public async Task<Loaded> LoadFromStorage()
        {
            var result = new Loaded();
            try
            {
                result.OldList = await _fileService.GetAsync<IEnumerable<Token>>(StorageFilePath);
                result.OldListDeleted = await _fileService.GetAsync<List<Gem>>(StorageFilePathDeleted);
                result.OldListAdded = await _fileService.GetAsync<List<Gem>>(StorageFilePathAdded);
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }

        public IEnumerable<Gem> CheckDeleted(IEnumerable<Token> oldList, IEnumerable<Token> newList)
        {
            var recentlyDeleted = new List<Gem>();

            var deletedFromUniswap = oldList
                    .Where(p => newList
                    .All(p2 => p2.Id != p.Id))
                    .ToList();

            if (deletedFromUniswap.Count() > 0)
            {
                foreach (var item in deletedFromUniswap)
                {
                    var gem = new Gem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Symbol = item.Symbol,
                        Recently = TokenAction.DELETED,
                        Date = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                        IsPublished = false
                    };
                    recentlyDeleted.Add(gem);
                }
            }
            return recentlyDeleted;
        }

        public IEnumerable<Gem> CheckAdded(IEnumerable<Token> oldList, IEnumerable<Token> newList)
        {
            var recentlyAdded = new List<Gem>();

            var addedToUniswap = newList
                    .Where(p => oldList
                    .All(p2 => p2.Id != p.Id))
                    .ToList();

            if (addedToUniswap.Count() > 0)
            {
                foreach (var item in addedToUniswap)
                {
                    var gem = new Gem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Symbol = item.Symbol,
                        Recently = TokenAction.ADDED,
                        Date = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                        IsPublished = false
                    };
                    recentlyAdded.Add(gem);
                }
            }
            return recentlyAdded;
        }
    }
}