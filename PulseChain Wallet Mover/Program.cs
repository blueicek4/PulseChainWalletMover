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
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
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
using Common;
using Telegram;
using Blockchain;
using TelegramBot;
using System.Linq.Expressions;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace PulseChainWallet
{
    static class Program
    {
        private static TimeSpan checkInterval = TimeSpan.FromMinutes(3); // Intervallo tra i controlli
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
            var host = CreateHostBuilder(args).Build();
            Boolean console = false;
            if (config.RunAsService)
            {
                ///
                /// To do stuff to run as service 
                /// 
                /// 
            }
            else
            {
                Console.WriteLine("Run as ConsoleApp");
                await host.RunAsync();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<PulseChainWorker>();
        });

    }

    public class PulseChainWorker : BackgroundService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PulseChainWorker));
        private readonly IConfiguration _configuration;
        // Intervallo tra i controlli
        private static TimeSpan checkInterval = TimeSpan.FromSeconds(30);

        public PulseChainWorker()
        {
        }
        public PulseChainWorker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
            var cts = new CancellationTokenSource();
            Task.Run(() => new PWMTelegram().InitializeTelegramBot(cts.Token), cts.Token);
            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"PulseChain Wallet Mover\nStarting RPC Monitoring");
            int updatefrom = 0;
            int maxupdate = (config.StatusInterval * 60) / (config.CheckInterval + (config.RpcUrls.Count * 5));
            bool cicle = true;
            while (cicle)
            {
                if(updatefrom ==0)
                {
                    string msg = "PulseChain Wallet Mover is online and running";
                    await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramStatusIds, $"✅✅✅\n{msg.EscapeMarkdownV2()}\n✅✅✅");

                }
                if (updatefrom < maxupdate)
                {
                    updatefrom++;
                }
                else
                {
                    updatefrom = 0;
                }
                config = PWMConfiguration.LoadFromFile("config.xml");
                if (config.ExitOnFirstRun)
                {
                    config.RpcUrls = config.TestnetRpcUrls;
                }

                TestResult result = new TestResult();

                result.rpcStatus.Clear();
                result.dnsStatus.Clear();
                // Inizializza lo stato dei siti come offline
                foreach (string url in config.RpcUrls)
                {
                    result.rpcStatus[url] = false;
                    result.dnsStatus[url] = false;
                }
                result.Save();

                foreach (var rpc in config.RpcUrls)
                {

                    bool dnsCheck = CheckDnsEntry(rpc);
                    if (dnsCheck)
                    {
                        result.dnsStatus[rpc] = true;
                        result.Save();

                        await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠\n\n\n\n**THE DNS {rpc.Replace(".", "\\.")} IS ONLINE**\n\n\n\n⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠\n\nTRYING TO CONNECT TO THE RPC SERVER\n\n⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠");
                    }
                    else
                    {
                        result.dnsStatus[rpc] = false;
                        result.rpcStatus[rpc] = false;
                        result.Save();

                    }
                    string _chainId = await CheckRpcOnline(rpc);
                    if (!String.IsNullOrWhiteSpace(_chainId))
                    {
                        result.rpcStatus[rpc] = true;
                        result.Save();
                        try
                        {
                            string messageLog = $"{DateTime.Now.ToShortTimeString()} THE FOLLOWING RPC SERVER IS ONLINE {rpc} - CHAIN ID IS {_chainId} - STARTING TRANSFER!";
                            Console.WriteLine(messageLog);
                            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠\n{messageLog.EscapeMarkdownV2()}\n⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠");
                            var pulseChainScannerApi = new PulseChainScannerApi();
                            pulseChainScannerApi.HttpClient = new HttpClient() { BaseAddress = new Uri(rpc.Replace("rpc", "scan")) };
                            var startAddress = config.StartWallet;
                            var targetAddress = config.TargetWallet;
                            string mnemonicPhrase = config.StartSeed;
                            string passphrase = String.Empty; // Passphrase is optional, you can leave it as an empty string if you don't have one
                            string privateKey;
                            if (String.IsNullOrWhiteSpace(config.StartPrivateKey))
                            {
                                // Create a wallet using the mnemonic and passphrase
                                Wallet wallet = new Wallet(mnemonicPhrase, passphrase);

                                // Derive the desired private key using a derivation path
                                var derivationPath = 0;//"m/44'/60'/0'/0/0"; // Replace with the desired derivation path
                                privateKey = wallet.GetPrivateKey(derivationPath).ToHex();
                            }
                            else
                            {
                                privateKey = config.StartPrivateKey;
                            }

                            string rpcUrl = rpc; // Replace with a PulseChain node URL
                            int chainId = Convert.ToInt32(_chainId);

                            List<TokenInfo> sactokenInfos = new List<TokenInfo>();
                            List<TokenInfo> clonedtokenInfos = new List<TokenInfo>();

                            var sacTokensStrings = System.IO.File.ReadAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.ProcessPath), "sacTokens.txt")).ToList();
                            foreach (var t in sacTokensStrings)
                            {
                                var sacTokens = await pulseChainScannerApi.GetTokenAddressFromName(t);
                                if (sacTokens.Count != 0)
                                {
                                    foreach (var sacToken in sacTokens)
                                        Console.WriteLine($"Name: {sacToken.Name}, Symbol: {sacToken.Symbol}, Contract Address: {sacToken.ContractAddress}, Balance: {sacToken.Balance}");
                                    sactokenInfos.AddRange(sacTokens);
                                }
                            }

                            sactokenInfos = (await TokenBalanceChecker.GetTokenBalancesAsync(startAddress, sactokenInfos, rpcUrl, chainId, privateKey));
                            sactokenInfos = sactokenInfos.Where(t => Convert.ToDecimal(t.Balance) > 0).ToList();

                            var clonedTokensStrings = System.IO.File.ReadAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.ProcessPath), "clonedTokens.txt")).ToList();
                            foreach (var t in clonedTokensStrings)
                            {

                                var clonedTokens = await pulseChainScannerApi.GetTokenFromAddress(t.Trim().Trim(';').Trim(','));
                                var token = clonedTokens;
                                if (token != null)
                                {
                                    Console.WriteLine($"Name: {token.Name}, Symbol: {token.Symbol}, Contract Address: {token.ContractAddress}, Decimals: {token.Decimals}");
                                    clonedtokenInfos.Add(clonedTokens);
                                }
                            }

                            clonedtokenInfos = (await TokenBalanceChecker.GetTokenBalancesAsync(startAddress, clonedtokenInfos, rpcUrl, chainId, privateKey));
                            clonedtokenInfos = clonedtokenInfos.Where(t => Convert.ToDecimal(t.Balance) > 0).ToList();
                            var coinService = new PulseChainRpcService();
                            var PlsAmount = await coinService.GetNativeCoinBalanceAsync(startAddress, rpcUrl, chainId, privateKey);
                            string msgPlsTotal = $"➡➡➡ Native Coin's Balance owned by address {startAddress} is {PlsAmount}";
                            Console.WriteLine($"{msgPlsTotal}\n\n");
                            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, msgPlsTotal.EscapeMarkdownV2());
                            if (sactokenInfos.Count > 0)
                            {
                                string msgSacTotal = $"List of SAC Tokens owned by address {startAddress}";
                                await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, msgSacTotal.EscapeMarkdownV2());
                                Console.WriteLine($"{msgSacTotal}:\n\n");
                            }
                            foreach (var token in sactokenInfos)
                            {
                                string msgSacLog = $"➡➡➡ Name: {token.Name}, Symbol: {token.Symbol}, Balance: {token.Balance}\nContract Address: {token.ContractAddress}";
                                Console.WriteLine(msgSacLog);
                                await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, msgSacLog.EscapeMarkdownV2());
                            }
                            if (clonedtokenInfos.Count > 0)
                            {
                                string msgClonedTotal = $"List of CLONED Tokens owned by address {startAddress}";
                                Console.WriteLine($"\n\n{msgClonedTotal}:");
                                await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, msgClonedTotal.EscapeMarkdownV2());
                            }
                            foreach (var token in clonedtokenInfos)
                            {
                                string msgCloneLog = $"➡➡➡ Name: {token.Name}, Symbol: {token.Symbol}, Balance: {token.Balance}\nContract Address: {token.ContractAddress}";
                                Console.WriteLine(msgCloneLog);
                                await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, msgCloneLog.EscapeMarkdownV2());

                            }
                            if(PlsAmount > 0.6M)
                            PlsAmount -= 0.5M;
                            else if (PlsAmount > 0)
                                PlsAmount /= 2;
                            else
                            {
                                string msgCloneLog = $"Attention!!! Your Native Coine Balance is ZERO, you cannot pay fee for transfers!";
                                Console.WriteLine(msgCloneLog);
                                await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠\n{msgCloneLog.EscapeMarkdownV2()}\n⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠");
                                continue;
                            }
                            decimal percentage = 1M;
                            percentage = decimal.Parse(config.Percentage.Replace("%", "")) / 100;

                            PlsAmount *= percentage;
                            var coinHash = await coinService.TransferNativeCoinAsync(privateKey, targetAddress, PlsAmount, rpcUrl, chainId);
                            string msgHashLog = $"Succesfully transferred: {PlsAmount} PLS to: {targetAddress}!!!\nTransaction hash: {coinHash}";
                            Console.WriteLine(msgHashLog);
                            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉\n{msgHashLog.EscapeMarkdownV2()}\n🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉");

                            percentage = 1M;
                            decimal tokenAmount = 0M;
                            List<TransferData> sacTransfers = new List<TransferData>();
                            List<TransferData> clonedTransfers = new List<TransferData>();
                            foreach (var token in sactokenInfos)
                            {
                                tokenAmount = Convert.ToDecimal(token.Balance);
                                percentage = decimal.Parse(config.Percentage.Replace("%", "")) / 100;
                                tokenAmount *= percentage;
                                sacTransfers.Add(new TransferData() { Amount = tokenAmount, ToAddress = targetAddress, ContractAddress = token.ContractAddress, ContractName = token.Name });

                            }
                            foreach (var token in clonedtokenInfos)
                            {
                                tokenAmount = Convert.ToDecimal(token.Balance);
                                percentage = decimal.Parse(config.Percentage.Replace("%", "")) / 100;
                                tokenAmount *= percentage;
                                clonedTransfers.Add(new TransferData() { Amount = tokenAmount, ToAddress = targetAddress, ContractAddress = token.ContractAddress, ContractName = token.Name });

                            }

                            var tokenService = new PulseChainRpcService();
                            List<TransferData> allTokens = sacTransfers;
                            allTokens.AddRange(clonedTransfers);
                            //await tokenService.TransferMultiCallTokensAsync(privateKey, allTokens, rpc, chainId);
                            await tokenService.TransferMultipleTokensAsync(privateKey, sacTransfers, rpcUrl, chainId);
                            await tokenService.TransferMultipleTokensAsync(privateKey, clonedTransfers, rpcUrl, chainId);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    else
                    {
                        if (dnsCheck)
                            await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"\U000023F0⏰⏰RPC SERVER IS STILL OFFLINE, TRYING IN THE NEXT CYCLE");
                        Console.WriteLine($"THE FOLLOWING RPC SERVER IS OFFLINE: {rpc} - CHAIN ID IS MISSING - MOVING TO NEXT SERVER");
                    }

                    if (config.ExitOnFirstRun) { cicle = false; }
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                if (cicle)
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} ALL RPC SERVER ARE OFFLINE, WAITING {checkInterval} SECONDS BEFORE NEXT CHECK");
                else
                {
                    string msgOkLog = $"{DateTime.Now.ToShortTimeString()} ALL TOKEN WERE TRANSFERRED, CHECK YOUR DESTINATION WALLET";
                    Console.WriteLine(msgOkLog);
                    await Helper.SendTelegramMessage(config.TelegramBotToken, config.TelegramNotificationsIds, $"🎉🎉🎉 {msgOkLog.EscapeMarkdownV2()}");
                }


                checkInterval = TimeSpan.FromSeconds(config.CheckInterval);
                await Task.Delay(checkInterval);
            }
            this.Dispose();
            
        }
        private static bool CheckWebsiteAvailability(string url)
        {
            ILog log = LogManager.GetLogger(typeof(PulseChainWorker));
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = httpClient.GetAsync(url).Result;

                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Host sconosciuto") && !ex.Message.Contains("nome richiesto è valido"))
                        log.Error($"Errore nel controllo della disponibilità del sito {url}: {ex.Message}");
                    return false;
                }
            }
        }

        private static bool CheckDnsEntry(string url)
        {
            ILog log = LogManager.GetLogger(typeof(PulseChainWorker));
            try
            {
                Uri uri = new Uri(url);
                string host = uri.Host;
                LookupClient dnsClient = new LookupClient(new LookupClientOptions(new[] { IPAddress.Parse("8.8.8.8") }) { UseCache = false, Timeout = TimeSpan.FromSeconds(2) });
                var result = dnsClient.Query(host, QueryType.ANY);

                return result.Answers.Any(record => record.RecordType == ResourceRecordType.A || record.RecordType == ResourceRecordType.AAAA || record.RecordType == ResourceRecordType.CNAME);
            }
            catch (Exception ex)
            {
                log.Error($"Errore nel controllo dell'entry DNS per {url}: {ex.Message}");
                return false;
            }
        }

        private static async Task<String> CheckRpcOnline(string rpcUrl)
        {
            try
            {
                var web3 = new Web3(rpcUrl);
                string chainId = await web3.Net.Version.SendRequestAsync();

                Console.WriteLine($"RPC node is online. Chain ID: {chainId}");
                return chainId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return String.Empty; ;
            }

        }


    }

}