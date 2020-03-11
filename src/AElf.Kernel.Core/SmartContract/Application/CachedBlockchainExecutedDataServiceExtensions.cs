using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Types;

namespace AElf.Kernel.SmartContract.Application
{
    public static class CachedBlockchainExecutedDataServiceExtensions
    {
        public static async Task AddBlockExecutedDataAsync<T>(
            this ICachedBlockchainExecutedDataService<T> cachedBlockchainExecutedDataGettingService,
            Hash blockHash, string key, T blockExecutedData)
        {
            var dic = new Dictionary<string, T>
            {
                {key, blockExecutedData}
            };
            await cachedBlockchainExecutedDataGettingService.AddBlockExecutedDataAsync(blockHash, dic);
        }
    }
}