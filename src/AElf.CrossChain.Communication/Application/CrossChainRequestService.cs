using System.Linq;
using System.Threading.Tasks;
using Acs7;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AElf.CrossChain.Communication.Application
{
    public class CrossChainRequestService : ICrossChainRequestService, ITransientDependency
    {
        private readonly ICrossChainClientService _crossChainClientService;
        private readonly ICrossChainService _crossChainService;

        public ILogger<CrossChainRequestService> Logger { get; set; }

        public CrossChainRequestService(ICrossChainService crossChainService, 
            ICrossChainClientService crossChainClientService)
        {
            _crossChainService = crossChainService;
            _crossChainClientService = crossChainClientService;
        }

        public async Task RequestCrossChainDataFromOtherChainsAsync()
        {
            var chainIdHeightDict = _crossChainService.GetNeededChainIdAndHeightPairs();
            Logger.LogTrace(
                $"Try to request from chain {string.Join(",", chainIdHeightDict.Keys.Select(ChainHelper.ConvertChainIdToBase58))}");
            foreach (var chainIdHeightPair in chainIdHeightDict)
            {
                var client = await _crossChainClientService.GetClientAsync(chainIdHeightPair.Key);
                if (client == null)
                    continue;
                Logger.LogTrace($" Request chain {ChainHelper.ConvertChainIdToBase58(chainIdHeightPair.Key)} from {chainIdHeightPair.Value}");
                _ = client.RequestCrossChainDataAsync(chainIdHeightPair.Value);
            }
        }

        public async Task<ChainInitializationData> RequestChainInitializationDataAsync(int chainId)
        {
            var client = _crossChainClientService.CreateClientForChainInitializationData(chainId);
            var chainInitializationData =
                await client.RequestChainInitializationDataAsync(chainId);
            return chainInitializationData;
        }
    }
}