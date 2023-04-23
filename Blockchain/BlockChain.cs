using System.Globalization;
using System.Numerics;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System;
using Common;
using Telegram;
using System.Runtime.CompilerServices;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Newtonsoft.Json;
using Nethereum.HdWallet;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts.Managed;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Org.BouncyCastle.Utilities.Net;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using NBitcoin;
using Nethereum.Model;
using Nethereum.JsonRpc.Client;
using NBitcoin.Logging;
using Telegram.Bots.Http;
using System.Diagnostics;
using log4net;
using Telegram.Bot.Exceptions;
using Account = Nethereum.Web3.Accounts.Account;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using log4net.Util;
using Nethereum.StandardTokenEIP20;
using Nethereum.Hex.HexConvertors;
using Nethereum.Util.ByteArrayConvertors;
using System.Globalization;
using Microsoft.Extensions.Logging;
using IPAddress = System.Net.IPAddress;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Policy;
using DnsClient;
using DnsClient.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using TelegramBot;

namespace Blockchain
{

    public class PulseChainScannerApi
    {
        public HttpClient HttpClient { get; set; }
        public PulseChainScannerApi()
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://scan.mainnet.pulsechain.com")
            };
        }
        public async Task<List<TokenInfo>> GetTokensOwnedByAddressAsync(string address)
        {
            var response = await HttpClient.GetAsync($"/api?module=account&action=tokenlist&address={address}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<Common.ApiResponse>(content);

            if (apiResponse.Status == "1" && apiResponse.Message == "OK")
            {
                return apiResponse.Result;
            }

            throw new Exception($"Error getting token list: {apiResponse.Message}");
        }
        public async Task<TokenInfo> GetTokenFromAddress(string tokenAddress)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api?module=token&action=getToken&contractaddress={tokenAddress}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                TokenInfo contractAddresses = await ExtractTokenFromAddresses(content);

                return contractAddresses;
            }
            catch
            {
                return new TokenInfo();
            }

        }


        public async Task<List<TokenInfo>> GetTokenAddressFromName(string tokenName)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/tokens?type=JSON&filter={tokenName}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                List<TokenInfo> contractAddresses = await ExtractContractAddresses(content);

                return contractAddresses;
            }
            catch
            {
                return new List<TokenInfo>();
            }

        }
        public async Task<TokenInfo> ExtractTokenFromAddresses(string jsonText)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonText);

                TokenInfo tokenInfo = new TokenInfo
                {
                    ContractAddress = (string)jsonObject["result"]["contractAddress"],
                    Name = (string)jsonObject["result"]["name"],
                    Symbol = (string)jsonObject["result"]["symbol"],
                    Decimals = (string)jsonObject["result"]["decimals"],
                    Type = (string)jsonObject["result"]["type"]
                };

                return tokenInfo;

            }
            catch
            {
                return new TokenInfo();
            }

        }

        public async Task<List<TokenInfo>> ExtractContractAddresses(string jsonText)
        {
            try
            {
                List<TokenInfo> tokens = new List<TokenInfo>();
                JObject jsonObject = JObject.Parse(jsonText);
                JArray itemsArray = (JArray)jsonObject["items"];
                List<string> contractAddresses = new List<string>();
                string pattern = "data-identifier-hash=\"(0x[0-9a-fA-F]{40})\"";

                foreach (string item in itemsArray)
                {
                    Match match = Regex.Match(item, pattern);
                    if (match.Success)
                    {
                        tokens.Add(await GetTokenFromAddress(match.Groups[1].Value));
                    }
                }

                return tokens;
            }
            catch
            {
                return new List<TokenInfo>();
            }

        }

    }
    public class PulseChainRpcService
    {

        [Function("transfer", "bool")]
        public class TransferFunction : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 2)]
            public BigInteger Value { get; set; }
        }

        public static async Task<bool> VerifyPrivateKeyOnBlockchainAsync(string rpcUrl, string privateKey, string expectedAddress)
        {
            try
            {
                // Verifica la corrispondenza tra chiave privata e indirizzo
                if (!ValidatePrivateKeyAndAddress(privateKey, expectedAddress))
                {
                    return false;
                }
                // Crea un'istanza di Account utilizzando la chiave privata
                var account = new Account(privateKey);

                // Crea un'istanza di Web3 utilizzando l'account e l'URL del nodo RPC
                var web3 = new Web3(account, rpcUrl);

                // Ottieni il bilancio dell'indirizzo
                var balance = await web3.Eth.GetBalance.SendRequestAsync(expectedAddress);

                // Verifica se il bilancio è maggiore di 0 (o qualsiasi altra condizione che desideri utilizzare)
                return balance.Value >= BigInteger.Zero;
            }
            catch (Exception)
            {
                // Gestisci le eccezioni in base alle tue esigenze
                return false;
            }
        }
        public async Task<decimal> GetNativeCoinBalanceAsync(string walletAddress, string rpcUrl, int chainId, string privateKey)
        {
            var account = new Account(privateKey, chainId);
            var web3 = new Web3(account, rpcUrl);

            // Get the balance in the smallest unit (e.g., Wei for ETH or PLS)
            BigInteger balanceInSmallestUnit = await web3.Eth.GetBalance.SendRequestAsync(walletAddress);

            // Convert the balance from the smallest unit to the native coin unit (e.g., ETH or PLS)
            decimal balance = Web3.Convert.FromWei(balanceInSmallestUnit);

            return balance;
        }

        public async Task<string> TransferNativeCoinAsync(string privateKey, string toAddress, decimal amount, string rpcUrl, int chainId)
        {
            var account = new Account(privateKey, chainId);
            var web3 = new Web3(account, rpcUrl);

            // Convert the amount from the native coin unit to the smallest unit (e.g., Wei for ETH or PLS)
            BigInteger amountInSmallestUnit = Web3.Convert.ToWei(amount);

            // Estimate the gas required for the transaction
            var gasEstimate = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(new CallInput
            {
                From = account.Address,
                To = toAddress,
                Value = new HexBigInteger(amountInSmallestUnit)
            });

            // Get the current nonce
            var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            Console.WriteLine($"Starting Transfer of NATIVE COIN to: {toAddress} - Transferring: {amount}");

            // Get the current gas price
            var currentGasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            BigInteger aggressiveGasPrice = BigInteger.Multiply(currentGasPrice.Value, new BigInteger(14)) / 10;
            // Set the aggressive gas price by multiplying the current gas price by 1.4
            BigInteger aggressiveGasEstimate = BigInteger.Multiply(gasEstimate.Value, new BigInteger(14)) / 10;


            // Send the signed transaction
            var transactionHash = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(toAddress, amount, (decimal)aggressiveGasPrice / (decimal)Math.Pow(10, 9), null, nonce);

            return transactionHash.TransactionHash;
        }
        public async Task<string> TransferAsync(string privateKey, string toAddress, string TokenContract, string TokenName, decimal amount, string rpcUrl, int chainId, bool _aggressiveGasEstimate = true)
        {
            try
            {
                var account = new Account(privateKey);
                var managedAccount = new Account(privateKey, chainId);
                var web3 = new Web3(managedAccount, rpcUrl);

                var abi = @"[{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'','type':'bool'}],'type':'function'}]";

                var tokenService = web3.Eth.GetContract(abi, TokenContract);
                var decimals = await tokenService.GetFunction("decimals").CallAsync<byte>();
                var amountInTokenUnits = Web3.Convert.ToWei(amount, decimals);

                var transferFunction = new TransferFunction
                {
                    To = toAddress,
                    Value = amountInTokenUnits
                };

                // Estimate gas and apply aggressive factor
                var estimatedGas = await web3.Eth.GetContractTransactionHandler<TransferFunction>()
                    .EstimateGasAsync(TokenContract, transferFunction);
                var aggressiveGas = new HexBigInteger((BigInteger)(Convert.ToDecimal(estimatedGas.Value.ToString()) * 1.5M));
                if (_aggressiveGasEstimate)
                {
                    // Update gas in transfer function
                    transferFunction.Gas = aggressiveGas;
                }
                Console.WriteLine($"Starting Transfer to: {toAddress} - Transferring: {amount} of {TokenName}");

                // Set the timeout
                var timeoutTask = Task.Delay(60000); // 30 seconds

                // Create a cancellation token source
                var cancellationTokenSource = new CancellationTokenSource();

                // Pass the cancellation token to the SendRequestAndWaitForReceiptAsync method
                var sendTask = web3.Eth.GetContractTransactionHandler<TransferFunction>()
                    .SendRequestAndWaitForReceiptAsync(TokenContract, transferFunction, cancellationTokenSource.Token);

                // Wait for the first completed task
                var completedTask = await Task.WhenAny(sendTask, timeoutTask);

                // Check if the transfer was successful
                if (completedTask == timeoutTask)
                {
                    // Cancel the sendTask
                    cancellationTokenSource.Cancel();

                    throw new Exception("The transfer timed out.");
                }

                var transactionReceipt = sendTask.Result;

                return $"{transactionReceipt.TransactionHash}||{TokenName}||{toAddress}||{TokenContract}||{amount}";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}||{TokenName}||{toAddress}||{TokenContract}||{amount}";

            }
        }
        public async Task<List<string>> TransferMultipleTokensAsync(string privateKey, List<TransferData> transfers, string rpcUrl, int chainId)
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");

            List<Task<string>> transferTasks = new List<Task<string>>();
            List<string> transactionHashes = new List<string>();

            // Create transfer tasks for each transfer
            foreach (var transfer in transfers)
            {
                var transferTask = TransferAsync(privateKey, transfer.ToAddress, transfer.ContractAddress, transfer.ContractName, transfer.Amount, rpcUrl, chainId);
                transferTasks.Add(transferTask);
            }

            // Process tasks as they complete
            while (transferTasks.Count > 0)
            {
                // Wait for any task to complete
                var completedTask = await Task.WhenAny(transferTasks);

                // Remove the completed task from the list
                transferTasks.Remove(completedTask);

                // Get the transaction hash and add it to the list
                string transactionHash = await completedTask;
                transactionHashes.Add(transactionHash);

                // If the transaction failed due to "replacement transaction underpriced" error, retry with a higher gas price
                if (transactionHash.Contains("replacement transaction underpriced"))
                {
                    var transferData = GetTransferDataFromTransactionHash(transactionHash);

                    // Retry the transfer with a higher gas price
                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine($"Retry {i + 1} Transfer to: {transferData.ToAddress} - Transferring: {transferData.Amount} of {transferData.ContractName}");
                        var retryTransactionHash = await TransferAsync(privateKey, transferData.ToAddress, transferData.ContractAddress, transferData.ContractName, transferData.Amount, rpcUrl, chainId, false);
                        if (!retryTransactionHash.Contains("replacement transaction underpriced"))
                        {
                            transactionHashes.Add(retryTransactionHash);
                            string msgok1 = $"Token transfer completed: {retryTransactionHash.Split("||")[0]}. Token Transferred: {retryTransactionHash.Split("||")[1]}.\nRemaining pending transfers: {transferTasks.Count}";
                            Console.WriteLine(msgok1);
                            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉\n{msgok1.EscapeMarkdownV2()}\n🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉");

                            break;
                        }
                        else if (i == 2)
                        {
                            string msgko1 = $"Transfer to {transferData.ToAddress} - Transferring: {transferData.Amount} of {transferData.ContractName} failed after 3 retries.";
                            Console.WriteLine(msgko1);
                            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼\n{msgko1.EscapeMarkdownV2()}\n‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼‼");
                        }
                    }
                }
                else
                {
                    transactionHashes.Add(transactionHash);
                    string msgok = $"Token transfer completed: {transactionHash.Split("||")[0]}. Token Transferred: {transactionHash.Split("||")[1]}.\nRemaining pending transfers: {transferTasks.Count}";
                    Console.WriteLine(msgok);
                    await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉\n{msgok.EscapeMarkdownV2()}\n🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉");
                }
            }

            return transactionHashes;
        }

        public async Task<List<string>> TransferMultiCallTokensAsync(string privateKey, List<TransferData> transfers, string rpcUrl, int chainId)
        {
            try
            {
                PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");

                var multicallAbi = @"[{'constant':true,'inputs':[],'name':'getCurrentBlockTimestamp','outputs':[{'name':'timestamp','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'components':[{'name':'target','type':'address'},{'name':'callData','type':'bytes'}],'name':'calls','type':'tuple[]'}],'name':'aggregate','outputs':[{'name':'blockNumber','type':'uint256'},{'name':'returnData','type':'bytes[]'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'getLastBlockHash','outputs':[{'name':'blockHash','type':'bytes32'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'addr','type':'address'}],'name':'getEthBalance','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'getCurrentBlockDifficulty','outputs':[{'name':'difficulty','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'getCurrentBlockGasLimit','outputs':[{'name':'gaslimit','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'getCurrentBlockCoinbase','outputs':[{'name':'coinbase','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'blockNumber','type':'uint256'}],'name':'getBlockHash','outputs':[{'name':'blockHash','type':'bytes32'}],'payable':false,'stateMutability':'view','type':'function'}]";
                var multicallContractAddress = "0xeefBa1e63905eF1D7ACbA5a8513c70307C1cE441";

                List<Task<string>> transferTasks = new List<Task<string>>();
                List<string> transactionHashes = new List<string>();

                var managedAccount = new Account(privateKey, chainId);

                var web3 = new Web3(managedAccount, rpcUrl);
                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(managedAccount.Address);
                var multicallContract = web3.Eth.GetContract(multicallAbi, multicallContractAddress);

                var calls = new List<Call>();

                // Create transfer tasks for each transfer
                foreach (var transfer in transfers)
                {
                    var abi = @"[{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'','type':'bool'}],'type':'function'}]";
                    var tokenService = web3.Eth.GetContract(abi, transfer.ContractAddress);

                    var decimals = await tokenService.GetFunction("decimals").CallAsync<byte>();
                    var amountInTokenUnits = Web3.Convert.ToWei(transfer.Amount, decimals);

                    var transferFunction = tokenService.GetFunction("transfer");
                    var target = transfer.ToAddress;

                    var encodedTransferFunction = transferFunction.CreateTransactionInput(target, amountInTokenUnits).Data;
                    var call = new Call
                    {
                        Target = transfer.ContractAddress,
                        CallData = FromHexString(encodedTransferFunction)
                    };
                    Console.WriteLine($"Call data: Target: {call.Target}, CallData: {BitConverter.ToString(call.CallData).Replace("-", "")}");
                    Console.WriteLine($"Decoded Call data: {DecodeCallData(call.CallData)}");

                    calls.Add(call);
                }
                var aggregateFunction = multicallContract.GetFunction("aggregate");

                string data = aggregateFunction.GetData(new object[] { calls.ToArray() });
                Console.WriteLine($"Aggregate data: {data}");
                var decodedAggregateData = DecodeAggregateCallData(data);
                foreach (var decodedData in decodedAggregateData)
                {
                    Console.WriteLine($"Decoded Aggregate Call data: {decodedData}");
                }
                string fromAddress = managedAccount.Address;

                var transactionInput = new TransactionInput(data, config.TargetWallet, fromAddress, new HexBigInteger(5000000), gasPrice, value: new HexBigInteger(0));

                var signedTransaction = await web3.Eth.TransactionManager.SignTransactionAsync(transactionInput);
                var transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + signedTransaction);

                transactionHashes.Add(transactionHash);

                return transactionHashes;
            }
            catch (Exception ex)
            {
                return new List<string>() { ex.Message };
            }
        }

        private static byte[] FromHexString(string hexString)
        {
            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Substring(2);
            }

            int length = hexString.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return bytes;
        }
        private static TransferData GetTransferDataFromTransactionHash(string transactionHash)
        {
            // Split the transaction hash into its parts
            var split = transactionHash.Split("||");

            // Get the contract name
            var contractName = split[1];

            // Use the contract name to get the contract address
            var contractAddress = split[3];

            // Get the to address from the transaction hash
            var toAddress = split[2];

            var amount = split[4];

            // Create a new TransferData object and return it
            return new TransferData
            {
                ContractName = contractName,
                ContractAddress = contractAddress,
                ToAddress = toAddress,
                Amount = Convert.ToDecimal(amount)
            };
        }

        private static string DecodeCallData(byte[] callData)
        {
            var hexData = BitConverter.ToString(callData).Replace("-", "");
            var functionSignature = hexData.Substring(0, 8);
            var targetAddress = hexData.Substring(24, 40);
            var encodedAmount = hexData.Substring(64);

            var amount = Web3.Convert.FromWei(BigInteger.Parse(encodedAmount, NumberStyles.HexNumber));

            return $"Function Signature: {functionSignature}, Target Address: 0x{targetAddress}, Amount: {amount}";
        }

        private static List<string> DecodeAggregateCallData(string aggregateData)
        {
            List<string> decodedData = new List<string>();

            try
            {

                // Remove the first 10 characters (0x and the function signature)
                string dataWithoutSignature = aggregateData.Substring(10);

                // Get the number of call data entries (next 64 characters after the data length)
                string numberOfCallDataEntriesHex = dataWithoutSignature.Substring(64, 64);
                int numberOfCallDataEntries = int.Parse(numberOfCallDataEntriesHex, System.Globalization.NumberStyles.HexNumber);

                // Iterate through each call data entry and decode it
                int offset = 128;
                for (int i = 0; i < numberOfCallDataEntries; i++)
                {
                    int callDataLength = 0;
                    try
                    {
                        // Get the call data length
                        string callDataLengthHex = dataWithoutSignature.Substring(offset, 64);
                        callDataLength = int.Parse(callDataLengthHex, System.Globalization.NumberStyles.HexNumber);

                        // Get the call data
                        string callData = dataWithoutSignature.Substring(offset + 64, callDataLength * 2);

                        decodedData.Add(callData);
                        offset += 64 + callDataLength * 2;
                    }
                    catch (Exception ex)
                    {
                        decodedData.Add(ex.Message);
                        offset += 64 + callDataLength * 2;

                    }
                }

                return decodedData;
            }
            catch (Exception ex)
            {
                decodedData.Add(ex.Message);
                return decodedData;
            }
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static bool ValidatePrivateKeyAndAddress(string privateKey, string expectedAddress)
        {
            try
            {
                var ethECKey = new EthECKey(privateKey);
                var recoveredAddress = ethECKey.GetPublicAddress();

                var utils = new AddressUtil();
                if (utils.IsChecksumAddress(expectedAddress))
                {
                    return utils.AreAddressesTheSame(recoveredAddress, expectedAddress);
                }
                else
                {
                    return recoveredAddress.Equals(expectedAddress, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {
                // Se la chiave privata non è valida, EthECKey lancerà un'eccezione.
                return false;
            }
        }

        #region

        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "_owner", 1)] public string Owner { get; set; }
        }
        public class Call : CallBase { }

        public class CallBase
        {
            [Parameter("address", "target", 1)]
            public virtual string Target { get; set; }
            [Parameter("bytes", "callData", 2)]
            public virtual byte[] CallData { get; set; }
        }

        public partial class AggregateFunction : AggregateFunctionBase { }

        [Function("aggregate", typeof(AggregateOutputDTO))]
        public class AggregateFunctionBase : FunctionMessage
        {
            [Parameter("tuple[]", "calls", 1)]
            public virtual List<Call> Calls { get; set; }
        }

        public partial class AggregateOutputDTO : AggregateOutputDTOBase { }

        [FunctionOutput]
        public class AggregateOutputDTOBase : IFunctionOutputDTO
        {
            [Parameter("uint256", "blockNumber", 1)]
            public virtual BigInteger BlockNumber { get; set; }
            [Parameter("bytes[]", "returnData", 2)]
            public virtual List<byte[]> ReturnData { get; set; }
        }

        #endregion
    }

    public static class TokenBalanceChecker
    {
        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "_owner", 1)]
            public string Owner { get; set; }
        }

        public static async Task<List<TokenInfo>> GetTokenBalancesAsync(string walletAddress, List<TokenInfo> tokenInfoList, string rpcUrl, int chainId, string privateKey)
        {
            var account = new Account(privateKey);

            var managedAccount = new Account(privateKey, chainId);
            Web3 web3 = new Web3(managedAccount, rpcUrl);

            foreach (var tokenInfo in tokenInfoList)
            {
                try
                {



                    var abi = @"[{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'type':'function'}]";

                    var contract = web3.Eth.GetContract(abi, tokenInfo.ContractAddress);

                    var balanceOfFunction = contract.GetFunction<BalanceOfFunction>();
                    var balance = await balanceOfFunction.CallAsync<BigInteger>(new BalanceOfFunction { Owner = walletAddress });
                    int decimals = int.Parse(tokenInfo.Decimals);
                    decimal balanceInTokens = (decimal)balance / (decimal)Math.Pow(10, decimals);

                    tokenInfo.Balance = balanceInTokens.ToString();
                }
                catch (Exception ex)
                {
                    tokenInfo.Balance = "0";
                    Console.WriteLine($"Errore nel recupero del saldo per il token {tokenInfo.Name}: {ex.Message}");
                }
            }

            return tokenInfoList;
        }
    }

    public static class EVMTools
    {
    }
}