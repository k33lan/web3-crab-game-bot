using System.Net;
using System.Numerics;
using Crabbot.Miner;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Crabbot.Miner;

public class Web3Client
{
    private ConfigOptions _options = null;
    
    public Web3Client(ConfigOptions options)
    {
        _options = options;
    }

    public async Task<TransactionReceipt?> closeGame(int mineGameId)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(_options.Key,_options.ChainId);
            var web3 = new Web3(account, _options.Web3BaseUrl);
            var contractHandler = web3.Eth.GetContractHandler(_options.ContractAddress);
    
            var closeGameFunction = new CloseGameFunction();
            closeGameFunction.GameId = mineGameId;
            var closeGameFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(closeGameFunction);
            return closeGameFunctionTxnReceipt;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    public async Task<TransactionReceipt?> sendTeam(int teamId)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(_options.Key,_options.ChainId);
            var web3 = new Web3(account, _options.Web3BaseUrl);
            var contractHandler = web3.Eth.GetContractHandler(_options.ContractAddress);
    
            var startGameFunction = new StartGameFunction();
            startGameFunction.TeamId = teamId;
            var startGameFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(startGameFunction);
            return startGameFunctionTxnReceipt;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<TransactionReceipt> reinforceDefense(int gameId, int CrabbyId, BigInteger borrowPrice)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(_options.Key,_options.ChainId);
            var web3 = new Web3(account, _options.Web3BaseUrl);
            var contractHandler = web3.Eth.GetContractHandler(_options.ContractAddress);
            
            var reinforceDefenseFunction = new ReinforceDefenseFunction();
            reinforceDefenseFunction.GameId = gameId;
            reinforceDefenseFunction.crabbyId = CrabbyId;
            reinforceDefenseFunction.BorrowPrice = borrowPrice;
            var reinforceDefenseFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(reinforceDefenseFunction);
            return reinforceDefenseFunctionTxnReceipt;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<TransactionReceipt> attack(int gameId, int attackTeamId)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(_options.Key,_options.ChainId);
            var web3 = new Web3(account, _options.Web3BaseUrl);
            var contractHandler = web3.Eth.GetContractHandler(_options.ContractAddress);
            var attackFunction = new AttackFunction();
            attackFunction.GameId = gameId;
            attackFunction.AttackTeamId = attackTeamId;
            var attackFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(attackFunction);
            return attackFunctionTxnReceipt;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}

 