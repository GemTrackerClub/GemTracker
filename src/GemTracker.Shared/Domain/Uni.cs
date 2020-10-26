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
    public class Uni
    {
        private readonly IUniswapService _uniswapService;
        private readonly IFileService _fileService;

        public string StorageFilePath { get; private set; }
        public string StorageFilePathDeleted { get; private set; }
        public string StorageFilePathAdded { get; private set; }
        public Uni(IUniswapService uniswapService,
            IFileService fileService)
        {
            _uniswapService = uniswapService;
            _fileService = fileService;
        }
        public void SetPaths(string storagePath)
        {
            StorageFilePath = PathTo.All(storagePath);
            StorageFilePathDeleted = PathTo.Deleted(storagePath);
            StorageFilePathAdded = PathTo.Added(storagePath);
        }
        public async Task<UniswapResponse> FetchAllAsync()
        {
            var result = new UniswapResponse();

            var response = await _uniswapService.FetchAllAsync();

            if (response.Success)
            {
                result.Tokens = response.Tokens;
            }
            else
            {
                result.Message = response.Message;
            }
            return result;
        }
        public async Task<Loaded> LoadAllAsync()
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