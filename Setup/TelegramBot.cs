using Microsoft.VisualBasic.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using log4net;
using Common;
using Telegram;
using TelegramBot;
using System.Diagnostics;
using System.Security.Policy;

namespace Setup
{
    public partial class TelegramBot : Form
    {
        CancellationTokenSource cts;
        public TelegramBot()
        {
            InitializeComponent();
        }

        private void TelegramBot_Load(object sender, EventArgs e)
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
        if(!String.IsNullOrWhiteSpace(config.TelegramBotUsername) && !String.IsNullOrWhiteSpace(config.TelegramBotToken))
            {
                lblBotStatus.Text = "Telegram bot Started";
                btnOpenChat.Enabled = true;
                btnResetChats.Enabled = true;
            cts = new CancellationTokenSource();
            Task.Run(() => new PWMTelegram().InitializeTelegramBot(cts.Token), cts.Token);

            }
            else
            {
                lblBotStatus.Text = "Telegram bot not configured";
                btnOpenChat.Enabled = false;
                btnResetChats.Enabled = false;
            }
            
        }

        private void btnOpenChat_Click(object sender, EventArgs e)
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {$"https://t.me/{config.TelegramBotUsername}"}") { CreateNoWindow = true });
        }

        private void btnResetChats_Click(object sender, EventArgs e)
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");
            config.TelegramStatusIds = new Common.SerializableDictionary<long, int>();
            config.TelegramNotificationsIds = new Common.SerializableDictionary<long, int>();
            config.Save();
            MessageBox.Show("Chats notification's were cleared.\nPlease subscribe again");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            this.Close();
        }
    }
}
