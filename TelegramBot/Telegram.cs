using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bots.Http;
using System.Diagnostics;
using Telegram.Bot.Exceptions;
using log4net;
using Common;
using Telegram.Bots;

namespace TelegramBot
{
    public class PWMTelegram
    {
        public TelegramBotClient botClient;

        private readonly ILog log = LogManager.GetLogger(typeof(PWMTelegram));
        public class MessageHandler
        {
            private readonly TelegramBotClient _botClient;
            private readonly ILog _log;
            private readonly ILog log = LogManager.GetLogger(typeof(MessageHandler));

            public MessageHandler(TelegramBotClient botClient, ILog log)
            {
                _botClient = botClient;
                _log = log;
            }

            public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
            {
                PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
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
                log.Info($"Message received from {(message.From.Username ?? message.From.FirstName)}: {message.Text}");

                TestResult result = new TestResult("testresult.xml");

                // Qui puoi implementare la tua logica per rispondere ai messaggi
                // Ad esempio, puoi inviare una risposta di eco al messaggio ricevuto
                UriHostNameType res;
                string risposta = String.Empty;
                switch (comando)
                {
                    case "/start":
                        risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nList of Bot's Commands\n/start: This Message\n/add: Subscribe to istant updates\n/remove: Unsubscribe\n/status: Last Status\n/updatehourly: Keep me updated that everythin is running fine\n/removehourly: Unsubscribe from scheduled updates\n/addnotification: Increment the number of istant update you will receive\n/resetnotification: reset notification's number you will receive";
                        break;
                    case "/add":
                        if (config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nYou are already subscribed to the istant update list";
                        else
                        {
                            config.TelegramNotificationsIds.Add(message.Chat.Id, 1);
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nI've added your id {message.Chat.Id.ToString().EscapeMarkdownV2()} to the istant update list\nYou will receive 1 update.";
                        }
                        break;
                    case "/remove":
                        if (!config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nYou weren't present in the istant update list";
                        else
                        {
                            config.TelegramNotificationsIds.Remove(message.Chat.Id);
                            config.TelegramStatusIds.Remove(message.Chat.Id);
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nI've removed your id {message.Chat.Id.ToString().EscapeMarkdownV2()} from every update list\nYou won't receive any update";
                        }
                        break;
                    case "/status":
                        string dnsWeb = "**All DNS RECORDS are still missing**";
                        string rpcWeb = "**all RPS SERVER are still offline**";
                        if (result.dnsStatus.Any(x => x.Value == true))
                        {
                            dnsWeb = $"There are {result.dnsStatus.Where(w => w.Value == true).Count()} DNS SERVER configured:\n\n{Tools.GetAllDnsStatus(result.dnsStatus.Where(w => w.Value == true).ToDictionary(x => x.Key, x => x.Value))}\n\nThere are still *{result.dnsStatus.Where(w => w.Value == true).Count()}* dns missing";
                        }
                        if (result.rpcStatus.Any(x => x.Value == true))
                        {
                            rpcWeb = $"There are {result.rpcStatus.Where(w => w.Value == true).Count()} RPC SERVER configured:\n\n{Tools.GetAllRpcStatus(result.rpcStatus.Where(w => w.Value == true).ToDictionary(x => x.Key, x => x.Value))}\n\nThere are still *{result.rpcStatus.Where(w => w.Value == true).Count()}* rpc missing";
                        }
                        risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nThe last update was at *{result.lastUpdate.ToShortTimeString().EscapeMarkdownV2()}* and this is the status of all DNS RECORDS:\n\n*{dnsWeb}*\n\nThis is RPC status:\n\n*{rpcWeb}*";
                        break;
                    case "/updatehourly":
                        if (config.TelegramStatusIds.Any(i => i.Key == message.Chat.Id))
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nYou are already subscribed to the scheduled update list";
                        else
                        {
                            config.TelegramStatusIds.Add(message.Chat.Id, 1);
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nI've added your id {message.Chat.Id.ToString().EscapeMarkdownV2()} to the istant updated list";
                        }
                        break;
                    case "/removehourly":
                        if (!config.TelegramStatusIds.Any(id => id.Key == message.Chat.Id))
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nYou weren't present in the scheduled update list";
                        else
                        {
                            config.TelegramStatusIds.Remove(message.Chat.Id);
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nI've removed your id {message.Chat.Id.ToString().EscapeMarkdownV2()} from the scheduled update list";
                        }
                        break;
                    case "/addnotification":
                        if (!config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nYou are not present in the istant update list";
                        else
                        {
                            int valore = config.TelegramNotificationsIds[message.Chat.Id];
                            valore = valore + 1;
                            config.TelegramNotificationsIds.Remove(message.Chat.Id);
                            config.TelegramNotificationsIds.Add(message.Chat.Id, valore);
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nI've raised the number of notification you will receive at {config.TelegramNotificationsIds[message.Chat.Id]}";
                        }
                        break;
                    case "/resetnotification":
                        if (!config.TelegramNotificationsIds.Any(i => i.Key == message.Chat.Id))
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nYou are not present in the istant update list";
                        else
                        {
                            config.TelegramNotificationsIds[message.Chat.Id] = 1;
                            risposta = $"Hi {message.From.Username.EscapeMarkdownV2()}\nI've set the number of notification you will receive at {config.TelegramNotificationsIds[message.Chat.Id]}";
                        }
                        break;
                    default:
                        break;
                }
                config.Save();
                if (!String.IsNullOrWhiteSpace(risposta))
                {
                    log.Info($"Sending Message to a {message.From.Username.EscapeMarkdownV2()} id {message.Chat.Id}:\n{risposta}");
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: risposta,
                        replyToMessageId: message.MessageId,
                        cancellationToken: cancellationToken,
                        parseMode: ParseMode.MarkdownV2
                    );
                }
            }

        }
        public async Task InitializeTelegramBot(CancellationToken cancellationToken)
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");

            string telegramBotToken = config.TelegramBotToken;

            if (string.IsNullOrEmpty(telegramBotToken))
            {
                log.Error("Telegram Bot's Token not found");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    botClient = new TelegramBotClient(telegramBotToken);

                    var me = await botClient.GetMeAsync(cancellationToken);
                    log.Info($"Telegram bot started: {me.Username}");

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
                    log.Error($"Error Starting Telegram Bot: {ex.Message}.");
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
        }
        private async Task ErrorHandler(Exception exception, CancellationToken cancellationToken)
        {
            log.Error($"Error during update message: {exception.Message}");
        }

    }
    public static class Helper
    {
        public static async Task SendTelegramMessage(string botToken, SerializableDictionary<long, int> chatIds, string message)
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
            ILog log = LogManager.GetLogger(typeof(PWMTelegram));
            if (string.IsNullOrEmpty(config.TelegramBotToken))
            {
                log.Error("Telegram Bot's Token not found");
                return;
            }

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

}