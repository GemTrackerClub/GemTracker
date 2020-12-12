using GemTracker.Shared.Dexchanges.Abstract;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Domain.Statics;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using GemTracker.Shared.Services.Responses.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemTracker.Shared.Dexchanges
{
    public class KyberDexchange : IDexchange<KyberToken, Gem>
    {
        private readonly IKyberService _kyberService;
        private readonly IFileService _fileService;
        public KyberDexchange(
            IKyberService kyberService,
            IFileService fileService)
        {
            _kyberService = kyberService;
            _fileService = fileService;
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
        public async Task<ListLoadedResponse<KyberToken, Gem>> LoadAllAsync(string storagePath)
        {
            var result = new ListLoadedResponse<KyberToken, Gem>();
            try
            {
                result.OldList = await _fileService.GetAsync<IEnumerable<KyberToken>>(PathTo.All(DexType.KYBER, storagePath));
                result.OldListDeleted = await _fileService.GetAsync<List<Gem>>(PathTo.Deleted(DexType.KYBER, storagePath));
                result.OldListAdded = await _fileService.GetAsync<List<Gem>>(PathTo.Added(DexType.KYBER, storagePath));
            }
            catch (Exception ex)
            {
                result.Message = ex.GetFullMessage();
            }
            return result;
        }
    }
}