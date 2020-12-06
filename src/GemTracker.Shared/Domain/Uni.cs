using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Domain.Models;
using GemTracker.Shared.Domain.Statics;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Collections.Generic;
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
            StorageFilePath = PathTo.All(DexType.UNISWAP, storagePath);
            StorageFilePathDeleted = PathTo.Deleted(DexType.UNISWAP, storagePath);
            StorageFilePathAdded = PathTo.Added(DexType.UNISWAP, storagePath);
        }
        public async Task<ListServiceResponse<Token>> FetchAllAsync()
        {
            var result = new ListServiceResponse<Token>();

            var response = await _uniswapService.FetchAllAsync();

            if (response.Success)
            {
                result.ListResponse = response.ListResponse;
            }
            else
            {
                result.Message = response.Message;
            }
            return result;
        }
        public async Task<Loaded<Token>> LoadAllAsync()
        {
            var result = new Loaded<Token>();
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
            => DexTokenCompare.DeletedTokens(oldList, newList);

        public IEnumerable<Gem> CheckAdded(IEnumerable<Token> oldList, IEnumerable<Token> newList)
            => DexTokenCompare.AddedTokens(oldList, newList);
    }
}