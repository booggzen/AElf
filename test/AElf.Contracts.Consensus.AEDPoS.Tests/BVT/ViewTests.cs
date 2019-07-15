using System.Threading.Tasks;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Shouldly;
using Xunit;

namespace AElf.Contracts.Consensus.AEDPoS
{
    public partial class AEDPoSTest
    {
        [Fact]
        public async Task Query_RoundInformation()
        {
            //first round
            var roundNumber = await BootMiner.GetCurrentRoundNumber.CallAsync(new Empty());
            
            var roundInfo = await BootMiner.GetCurrentRoundInformation.CallAsync(new Empty());
            roundInfo.TermNumber.ShouldBe(roundNumber.Value);
            
            //second round            
            var nextTermInformation = (await BootMiner.GetInformationToUpdateConsensus.CallAsync(
                new AElfConsensusTriggerInformation
                {
                    Behaviour = AElfConsensusBehaviour.NextRound,
                    Pubkey = ByteString.CopyFrom(BootMinerKeyPair.PublicKey)
                }.ToBytesValue())).ToConsensusHeaderInformation();

            var transactionResult = await BootMiner.NextRound.SendAsync(nextTermInformation.Round);
            transactionResult.TransactionResult.Status.ShouldBe(TransactionResultStatus.Mined);
            
            roundNumber = await BootMiner.GetCurrentRoundNumber.CallAsync(new Empty());
            roundNumber.Value.ShouldBe(2);
            
            roundInfo = await BootMiner.GetCurrentRoundInformation.CallAsync(new Empty());
            roundInfo.RoundNumber.ShouldBe(2);

            var roundInformation = await BootMiner.GetRoundInformation.CallAsync(new SInt64Value
            {
                Value = 2
            });
            roundInformation.RoundNumber.ShouldBe(2);
            roundInformation.TermNumber.ShouldBe(1);
            
            //get term number
            var termNumber = await BootMiner.GetCurrentTermNumber.CallAsync(new Empty());
            termNumber.Value.ShouldBe(1);
        }
    }
}