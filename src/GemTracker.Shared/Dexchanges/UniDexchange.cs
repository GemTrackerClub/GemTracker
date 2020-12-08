using GemTracker.Shared.Dexchanges.Abstract;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Dexchanges
{
    public class UniDexchange : Dexchange, IDexchange<Token, Gem>
    {
        private readonly IUniswapService _uniswapService;
        private readonly IFileService _fileService;
        public UniDexchange(
            IUniswapService uniswapService,
            IFileService fileService,
            string storagePath)
        {
            _uniswapService = uniswapService;
            _fileService = fileService;

            SetPaths(storagePath, DexType.UNISWAP);
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
        public async Task<ListLoadedResponse<Token, Gem>> LoadAllAsync()
        {
            var result = new ListLoadedResponse<Token, Gem>();
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
    }
}