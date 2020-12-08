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
    public class KyberDexchange : Dexchange, IDexchange<KyberToken, Gem>
    {
        private readonly IKyberService _kyberService;
        private readonly IFileService _fileService;
        public KyberDexchange(
            IKyberService kyberService,
            IFileService fileService,
            string storagePath)
        {
            _kyberService = kyberService;
            _fileService = fileService;

            SetPaths(storagePath, DexType.KYBER);
        }
        public async Task<ListServiceResponse<KyberToken>> FetchAllAsync()
        {
            var result = new ListServiceResponse<KyberToken>();

            var response = await _kyberService.FetchAllAsync();

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
        public async Task<ListLoadedResponse<KyberToken, Gem>> LoadAllAsync()
        {
            var result = new ListLoadedResponse<KyberToken, Gem>();
            try
            {
                result.OldList = await _fileService.GetAsync<IEnumerable<KyberToken>>(StorageFilePath);
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