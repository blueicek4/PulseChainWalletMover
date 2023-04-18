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

[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace PulseChainWallet
{
    static class Program
    {
        private static TimeSpan checkInterval = TimeSpan.FromMinutes(3); // Intervallo tra i controlli
        static async Task Main(string[] args)
        {
            Configuration config = Configuration.LoadFromFile("config.xml");
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

        static async Task ExecuteAsync()
        {
            Configuration config = Configuration.LoadFromFile("config.xml");
            bool cicle = true;
            while (cicle)
            {
                config = Configuration.LoadFromFile("config.xml");



                await Task.Delay(checkInterval);
                checkInterval = TimeSpan.FromSeconds(config.CheckInterval);
            }
        }

        private static bool CheckWebsiteAvailability(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = httpClient.GetAsync(url).Result;

                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private static bool CheckDnsEntry(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string host = uri.Host;
                LookupClient dnsClient = new LookupClient(new LookupClientOptions(new[] { System.Net.IPAddress.Parse("8.8.8.8") }) { UseCache = false, Timeout = TimeSpan.FromSeconds(2) });
                var result = dnsClient.Query(host, QueryType.ANY);

                return result.Answers.Any(record => record.RecordType == ResourceRecordType.A || record.RecordType == ResourceRecordType.AAAA || record.RecordType == ResourceRecordType.CNAME);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /*
        private async Task InitializeTelegramBot(CancellationToken cancellationToken)
        {
            Configuration config = Configuration.LoadFromFile("configurazione.xml");

            string telegramBotToken = config.TelegramBotToken;

            if (string.IsNullOrEmpty(telegramBotToken))
            {
                log.Error("Token del bot Telegram non configurato.");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    botClient = new TelegramBotClient(telegramBotToken);

                    var me = await botClient.GetMeAsync(cancellationToken);
                    log.Info($"Bot di Telegram inizializzato: {me.Username}");
                    var messageHandler = new MessageHandler(botClient, log);

                    var updateHandler = new DefaultUpdateHandler(
                        (botClient, update, cancellationToken) => messageHandler.HandleUpdateAsync(update, cancellationToken),
                        (botClient, exception, cancellationToken) => ErrorHandler(exception, cancellationToken)
                    );

                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = Array.Empty<UpdateType>()
                    };

                    botClient.StartReceiving(
                        updateHandler,
                        receiverOptions,
                        cancellationToken
                    );

                    // Attendi che il token di annullamento venga attivato
                    await Task.Delay(Timeout.Infinite, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Il token di annullamento è stato attivato, usciamo dal ciclo
                    break;
                }
                catch (Exception ex)
                {
                    log.Error($"Errore nell'inizializzazione del bot Telegram: {ex.Message}. Tentativo di riavvio tra 1 minuto.");
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
        }
        private async Task ErrorHandler(Exception exception, CancellationToken cancellationToken)
        {
            log.Error($"Errore durante la gestione dell'aggiornamento: {exception.Message}");
        }
        */
    }

    public class PulseChainWorker : BackgroundService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PulseChainWorker));
        private readonly IConfiguration _configuration;
        // Intervallo tra i controlli
        private static TimeSpan checkInterval = TimeSpan.FromSeconds(30);
        private TelegramBotClient botClient;

        public PulseChainWorker()
        {
        }
        public PulseChainWorker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Configuration config = Configuration.LoadFromFile("config.xml");
            bool cicle = true;
            while (cicle)
            {
                config = Configuration.LoadFromFile("config.xml");
                if (config.RunTest)
                    config.RpcUrls = config.TestnetRpcUrls;
                foreach(var rpc in config.RpcUrls)
                {
                    string _chainId = await CheckRpcOnline(rpc);
                    if (!String.IsNullOrWhiteSpace(_chainId))
                    {
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} THE FOLLOWING RPC SERVER IS ONLINE: {rpc} - CHAIN ID IS: {_chainId} - STARTING TRANSFER!");
                        cicle = false;
                        var pulseChainScannerApi = new PulseChainScannerApi();
                        pulseChainScannerApi.HttpClient = new HttpClient() { BaseAddress = new Uri(rpc.Replace("rpc", "scan")) };
                        var startAddress = config.StartWallet;
                        var targetAddress = config.TargetWallet;
                        string mnemonicPhrase = config.StartSeed;
                        string passphrase = String.Empty; // Passphrase is optional, you can leave it as an empty string if you don't have one

                        // Create a wallet using the mnemonic and passphrase
                        Wallet wallet = new Wallet(mnemonicPhrase, passphrase);

                        // Derive the desired private key using a derivation path
                        var derivationPath = 0;//"m/44'/60'/0'/0/0"; // Replace with the desired derivation path
                        var privateKey = wallet.GetPrivateKey(derivationPath);
                        var publicKey = wallet.GetPublicKey(derivationPath);
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

                        sactokenInfos = (await TokenBalanceChecker.GetTokenBalancesAsync(startAddress, sactokenInfos, rpcUrl, chainId, privateKey.ToHex()));                      
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

                        clonedtokenInfos = (await TokenBalanceChecker.GetTokenBalancesAsync(startAddress, clonedtokenInfos, rpcUrl, chainId, privateKey.ToHex()));
                        clonedtokenInfos = clonedtokenInfos.Where(t => Convert.ToDecimal(t.Balance) > 0).ToList();
                        var coinService = new PulseChainRpcService();
                        var PlsAmount = await coinService.GetNativeCoinBalanceAsync(startAddress, rpcUrl, chainId, privateKey.ToHex());
                        Console.WriteLine($"Native Coin owned by address {startAddress} is {PlsAmount}:\n\n");

                        Console.WriteLine($"SAC Tokens owned by address {startAddress}:\n\n");
                        foreach (var token in sactokenInfos)
                        {
                            Console.WriteLine($"Name: {token.Name}, Symbol: {token.Symbol}, Contract Address: {token.ContractAddress}, Balance: {token.Balance}");
                        }
                        Console.WriteLine($"\n\nCLONED Tokens owned by address {startAddress}:");
                        foreach (var token in clonedtokenInfos)
                        {
                            Console.WriteLine($"Name: {token.Name}, Symbol: {token.Symbol}, Contract Address: {token.ContractAddress}, Balance: {token.Balance}");
                        }

                        PlsAmount -= 0.5M;
                        decimal percentage = 1M;
                        percentage = decimal.Parse(config.Percentage.Replace("%", "")) / 100;

                        PlsAmount *= percentage;
                        var coinHash = await coinService.TransferNativeCoinAsync(privateKey.ToHex(), targetAddress, PlsAmount, rpcUrl, chainId);
                        Console.WriteLine($"Transaction hash: {coinHash} - Transferred: {PlsAmount} of PLS to: {targetAddress}");
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
                        await tokenService.TransferMultipleTokensAsync(privateKey.ToHex(), sacTransfers, rpcUrl, chainId);
                        await tokenService.TransferMultipleTokensAsync(privateKey.ToHex(), clonedTransfers, rpcUrl, chainId);
                    }
                    else
                    {
                        Console.WriteLine($"THE FOLLOWING RPC SERVER IS OFFLINE: {rpc} - CHAIN ID IS MISSING - MOVING TO NEXT SERVER");
                    }
                    await Task.Delay(TimeSpan.FromSeconds(5));

                }
                if(cicle)
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} ALL RPC SERVER ARE OFFLINE, WAITING {checkInterval} SECONDS BEFORE NEXT CHECK");
                else
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} ALL TOKEN WERE TRANSFERRED, CHECK YOUR DESTINATION WALLET");


                checkInterval = TimeSpan.FromSeconds(config.CheckInterval);
                await Task.Delay(checkInterval);
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

        private static async Task SendTelegramMessage(string botToken, SerializableDictionary<long, int> chatIds, string message)
        {
            ILog log = LogManager.GetLogger(typeof(PulseChainWorker));
            log.Info($"Invio notifiche telegram massive a {chatIds.Count} destinatari in corso.");
            if ((chatIds ?? new SerializableDictionary<long, int>()).Count > 0)
            {
                TelegramBotClient bot = new TelegramBotClient(botToken);
                foreach (var id in chatIds)
                {
                    for (int n = 0; n < id.Value; n++)
                        await bot.SendTextMessageAsync(id.Key, message, parseMode: ParseMode.MarkdownV2);
                }
            }
        }
    }
    public class MessageHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILog _log;
        private readonly ILog log = LogManager.GetLogger(typeof(PulseChainWorker));

        public MessageHandler(TelegramBotClient botClient, ILog log)
        {
            _botClient = botClient;
            _log = log;
        }
        /*
        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            Configuration config = Configuration.LoadFromFile("configurazione.xml");
            if (update.Type != UpdateType.Message && update.Type != UpdateType.EditedMessage)
                return;

            var message = update.Message ?? update.EditedMessage;

            if (message?.Text == null)
                return;

            string comando = String.Empty;
            string parametri = String.Empty;

            ///extract from message.text the command and the parameters
            ///
            if (message.Text.StartsWith('/'))
            {
                int index = message.Text.IndexOf(' ');
                if (index > 0)
                {
                    comando = message.Text.Substring(0, index);
                    parametri = message.Text.Substring(index + 1);
                }
                else
                {
                    comando = message.Text;
                }
            }

            if (message.Chat.Type == ChatType.Group || message.Chat.Type == ChatType.Supergroup)
            {
                if (!message.Text.Contains($"@{config.TelegramBotUsername}"))
                    return;
                comando = comando.Replace($"@{config.TelegramBotUsername}", "");
            }
            log.Info($"Messaggio ricevuto da {(message.From.Username ?? message.From.FirstName)}: {message.Text}");

            TestResult result = new TestResult("testresult.xml");

            // Qui puoi implementare la tua logica per rispondere ai messaggi
            // Ad esempio, puoi inviare una risposta di eco al messaggio ricevuto
            UriHostNameType res;
            string risposta = String.Empty;
            switch (comando)
            {
                case "/start":
                    risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nGuida all'uso del Bot\n/start: Visualizzare questo messaggio\n/add: aggiungere alle notifiche\n/remove: rimuovere dalle notifiche\n/status: visualizza ultimo stato\n/statusdetail: visualizza ultimo stato completo\n/updatehourly: aggiornami ogni ora\n/removehourly: rimuovi le notifiche orarie\n/addnotification: incrementa le notifiche che riceverai privatamente\n/resetnotification: reimposta ad 1 il numero di notifiche che riceverai";
                    break;
                case "/add":
                    if (config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nSei già presente nelle notifiche immediate appena un sito diventa online";
                    else
                    {
                        config.TelegramNotificationsIds.Add(message.Chat.Id, 1);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nHo aggiunto il tuo id {message.Chat.Id.ToString().EscapeMarkdownV2()} all'elenco delle notifiche da ricevere non appena uno dei siti risulta online\nRiceverai 1 notifica in caso di attivazione";
                    }
                    break;
                case "/remove":
                    if (!config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nNon eri presente nelle notifiche immediate quando un sito torna online";
                    else
                    {
                        config.TelegramNotificationsIds.Remove(message.Chat.Id);
                        config.TelegramStatusIds.Remove(message.Chat.Id);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nHo rimosso il tuo id {message.Chat.Id.ToString().EscapeMarkdownV2()} all'elenco di tutte le notifiche da ricevere\\. non riceverai più nessuna notifica";
                    }
                    break;
                case "/status":
                    string sitiWeb = "**tutti i siti sono offline**";
                    string dnsWeb = "**tutti i dns sono ancora mancanti**";
                    if (result.websiteStatus.Any(w => w.Value == true))
                    {

                        sitiWeb = $"ci sono {result.websiteStatus.Where(w => w.Value == true).Count()} siti online:\n\n{PulseChainWorker.GetAllWebsitesStatus(result.websiteStatus.Where(w => w.Value == true).ToDictionary(x => x.Key, x => x.Value))}\n\nCi sono ancora *{result.websiteStatus.Where(w => w.Value == true).Count()}* siti offline";
                    }

                    if (result.dnsStatus.Any(x => x.Value == true))
                    {
                        dnsWeb = $"ci sono {result.dnsStatus.Where(w => w.Value == true).Count()} dns configurati:\n\n{PulseChainWorker.GetAllDnsStatus(result.dnsStatus.Where(w => w.Value == true).ToDictionary(x => x.Key, x => x.Value))}\n\nCi sono ancora *{result.dnsStatus.Where(w => w.Value == true).Count()}* dns mancanti";
                    }
                    risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nl'utimo aggiornamento c'è stato alle *{result.lastUpdate.ToShortTimeString().EscapeMarkdownV2()}* e questo è lo stato attuale dei Siti:\n\n*{sitiWeb}*\n\nQuesto lo stato dei record DNS:\n\n*{dnsWeb}*";
                    break;
                case "/statusdetail":
                    risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nl'utimo aggiornamento c'è stato alle *{result.lastUpdate.ToLongTimeString().EscapeMarkdownV2()}* e questo è lo stato attuale dei Siti:\n\n{PulseChainWorker.GetAllWebsitesStatus(result.websiteStatus)}\n\nQuesto lo stato dei record DNS:\n\n{PulseChainWorker.GetAllDnsStatus(result.dnsStatus)}";
                    break;
                case "/updatehourly":
                    if (config.TelegramStatusIds.Any(i => i.Key == message.Chat.Id))
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nSei già presente nelle notifiche orario sullo stato dei siti";
                    else
                    {
                        config.TelegramStatusIds.Add(message.Chat.Id, 1);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nHo aggiunto il tuo id {message.Chat.Id.ToString().EscapeMarkdownV2()} all'elenco delle notifiche da ricevere di verifica stato ogni ora";
                    }
                    break;
                case "/removehourly":
                    if (!config.TelegramStatusIds.Any(id => id.Key == message.Chat.Id))
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nNon eri presente nell'elenco delle notifiche degli aggiornamenti orari sullo stato dei siti";
                    else
                    {
                        config.TelegramStatusIds.Remove(message.Chat.Id);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nHo rimosso il tuo id {message.Chat.Id.ToString().EscapeMarkdownV2()} dall'elenco delle notifiche da ricevere di verifica stato ogni ora";
                    }
                    break;
                case "/addnotification":
                    if (!config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nNon sei presente nelle notifiche immediate quando un sito torna online";
                    else
                    {
                        int valore = config.TelegramNotificationsIds[message.Chat.Id];
                        valore = valore + 1;
                        config.TelegramNotificationsIds.Remove(message.Chat.Id);
                        config.TelegramNotificationsIds.Add(message.Chat.Id, valore);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nho incrementato le notifiche dirette che riceverai a {config.TelegramNotificationsIds[message.Chat.Id]}";
                    }
                    break;
                case "/resetnotification":
                    if (!config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nNon sei presente nelle notifiche immediate quando un sito torna online";
                    else
                    {
                        config.TelegramNotificationsIds[message.Chat.Id] = 1;
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nho reimpostato le notifiche dirette che riceverai a {config.TelegramNotificationsIds[message.Chat.Id]}";
                    }
                    break;
                case "/addsite":
                    string websiteadd = parametri.Trim();
                    res = Uri.CheckHostName(parametri.Trim());
                    if (res != UriHostNameType.Unknown || String.IsNullOrWhiteSpace(parametri))
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nper favore inserisci i siti mettendo il protocollo http o https davanti";
                        break;
                    }
                    if (config.RpcUrls.Any(w => w == websiteadd))
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nIl sito {websiteadd.EscapeMarkdownV2()} e' gia presente nell'elenco dei siti da monitorare";
                    }
                    else
                    {
                        config.RpcUrls.Add(websiteadd);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nHo aggiunto il sito {websiteadd.EscapeMarkdownV2()} nell'elenco dei siti da monitorare";
                    }
                    break;
                case "/delsite":
                    string websiteremove = parametri.Trim();
                    res = Uri.CheckHostName(parametri.Trim());
                    if (res != UriHostNameType.Unknown || String.IsNullOrWhiteSpace(parametri))
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nper favore inserisci i siti mettendo il protocollo http o https davanti";
                        break;
                    }
                    if (config.RpcUrls.Any(w => w == websiteremove))
                    {
                        config.RpcUrls.Remove(websiteremove);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nIl sito {websiteremove.EscapeMarkdownV2()} e' stato rimosso dall'elenco dei siti da monitorare";
                    }
                    else
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nil sito {websiteremove.EscapeMarkdownV2()} non era presente nell'elenco dei siti da monitorare";
                    }
                    break;
                case "/addtest":
                    string testadd = parametri.Trim();
                    res = Uri.CheckHostName(parametri.Trim());
                    if (res != UriHostNameType.Unknown || String.IsNullOrWhiteSpace(parametri))
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nper favore inserisci i siti mettendo il protocollo http o https davanti";
                        break;
                    }
                    if (config.TestnetRpcUrls.Any(w => w == testadd))
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nIl sito {testadd.EscapeMarkdownV2()} e' gia presente nell'elenco dei siti da monitorare";
                    }
                    else
                    {
                        config.TestnetRpcUrls.Add(testadd);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nHo aggiunto il sito {testadd.EscapeMarkdownV2()} nell'elenco dei siti da monitorare";
                    }
                    break;
                case "/deltest":
                    string testremove = parametri.Trim();
                    res = Uri.CheckHostName(parametri.Trim());
                    if (res != UriHostNameType.Unknown || String.IsNullOrWhiteSpace(parametri))
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nper favore inserisci i siti mettendo il protocollo http o https davanti";
                        break;
                    }
                    if (config.TestnetRpcUrls.Any(w => w == testremove))
                    {
                        config.TestnetRpcUrls.Remove(testremove);
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nIl sito {testremove.EscapeMarkdownV2()} e' stato rimosso dall'elenco dei siti da monitorare";
                    }
                    else
                    {
                        risposta = $"Ciao {message.From.Username.EscapeMarkdownV2()}\nil sito {testremove.EscapeMarkdownV2()} non era presente nell'elenco dei siti da monitorare";
                    }
                    break;

                default:
                    break;
            }
            config.Save();
            if (!String.IsNullOrWhiteSpace(risposta))
            {
                log.Info($"invio messaggio a {message.From.Username.EscapeMarkdownV2()} id {message.Chat.Id}:\n{risposta}");
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: risposta,
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken,
                    parseMode: ParseMode.MarkdownV2
                );
            }
        }
        */
    }

    public class Configuration
    {
        public bool RunTest { get; set; }
        public bool RunAsService { get; set; }
        public int CheckInterval { get; set; }
        public int StatusInterval { get; set; }
        public string TelegramBotToken { get; set; }
        public string TelegramBotUsername { get; set; }
        public string StartWallet { get; set; }
        public string StartSeed { get; set; }
        public string TargetWallet { get; set; }
        public string Percentage { get; set; }
        public SerializableDictionary<long, int> TelegramNotificationsIds { get; set; }
        public SerializableDictionary<long, int> TelegramStatusIds { get; set; }
        [XmlArray("RpcUrls")]
        [XmlArrayItem("Url")]
        public List<string> RpcUrls { get; set; }

        [XmlArray("TestnetRpcUrls")]
        [XmlArrayItem("Url")]
        public List<string> TestnetRpcUrls { get; set; }
        public static Configuration LoadFromFile(string filePath)
        {
            string fullPath;

            if (Path.IsPathRooted(filePath))
            {
                fullPath = filePath;
            }
            else
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exePath);
                fullPath = Path.Combine(exeDirectory, filePath);
            }
            XmlParser xml = new XmlParser();
            var conf = xml.DeserializeToObject<Configuration>(fullPath);
            if (conf.TelegramStatusIds == null)
                conf.TelegramStatusIds = new SerializableDictionary<long, int>();
            if (conf.TelegramNotificationsIds == null)
                conf.TelegramNotificationsIds = new SerializableDictionary<long, int>();
            return conf;
        }
        public void Save()
        {
            string filePath = "config.xml";
            string fullPath;

            if (Path.IsPathRooted(filePath))
            {
                fullPath = filePath;
            }
            else
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exePath);
                fullPath = Path.Combine(exeDirectory, filePath);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ", // Usare quattro spazi per l'indentazione
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace,
                Encoding = new UTF8Encoding(false) // Impostare false per evitare l'aggiunta di BOM all'inizio del file
            };

            using (XmlWriter writer = XmlWriter.Create(fullPath, settings))
            {
                serializer.Serialize(writer, this);
            }
        }

    }

    public class XmlParser
    {
        public T DeserializeToObject<T>(string filepath) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(filepath))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }

    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool isEmptyElement = reader.IsEmptyElement;
            reader.Read();

            if (isEmptyElement)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Item");
                reader.ReadStartElement("Key");

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                TKey key = (TKey)keySerializer.Deserialize(reader);

                reader.ReadEndElement();
                reader.ReadStartElement("Value");

                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
                TValue value = (TValue)valueSerializer.Deserialize(reader);

                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("Item");
                writer.WriteStartElement("Key");

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                keySerializer.Serialize(writer, key);

                writer.WriteEndElement();
                writer.WriteStartElement("Value");

                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }

    public class TokenInfo
    {
        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        [JsonProperty("decimals")]
        public string Decimals { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public List<TokenInfo> Result { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

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
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

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
                            Console.WriteLine($"Token transfer completed: {retryTransactionHash.Split("||")[0]}. Token Transferred: {retryTransactionHash.Split("||")[1]}.\nRemaining pending transfers: {transferTasks.Count}");
                            break;
                        }
                        else if (i == 2)
                        {
                            Console.WriteLine($"Transfer to {transferData.ToAddress} - Transferring: {transferData.Amount} of {transferData.ContractName} failed after 3 retries.");
                        }
                    }
                }
                else
                {
                    transactionHashes.Add(transactionHash);
                    Console.WriteLine($"Token transfer completed: {transactionHash.Split("||")[0]}. Token Transferred: {transactionHash.Split("||")[1]}.\nRemaining pending transfers: {transferTasks.Count}");
                }
            }

            return transactionHashes;
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

    public class TransferData
    {
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string ContractAddress  { get; set; }
        public string ContractName { get; set; }
    }


}