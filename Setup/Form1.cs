using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Configuration;
using Common;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Blockchain;

namespace Setup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitConfig();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void InitConfig()
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");

            chkRunTest.Checked = config.ExitOnFirstRun;
            chkRunAsService.Checked = config.RunAsService;
            numCheckInterval.Value = config.CheckInterval;
            numStatusInterval.Value = config.StatusInterval;

            txtTelegramBotToken.Text = config.TelegramBotToken ?? String.Empty;
            txtTelegramBotUsername.Text = config.TelegramBotUsername ?? String.Empty;
            txtStartWallet.Text = config.StartWallet ?? String.Empty;
            txtStartSeed.Text = config.StartSeed ?? String.Empty;
            txtStartPrivateKey.Text = config.StartPrivateKey ?? String.Empty;
            txtTargetWallet.Text = config.TargetWallet ?? String.Empty;
            numPercentage.Value = Decimal.Parse((config.Percentage ?? "100%").Replace("%", ""));

            // Aggiungi una colonna al DataGridView, se non è già presente
            if (grdRpcMain.Columns.Count == 0)
            {
                grdRpcMain.Columns.Add("RpcMain", "Url Rpc Mainnet");
            }

            // Pulisci le righe esistenti
            grdRpcMain.Rows.Clear();

            // Itera sulla lista e aggiungi ogni elemento come nuova riga nel DataGridView
            foreach (string item in config.RpcUrls)
            {
                grdRpcMain.Rows.Add(item);
            }
            // Aggiungi una colonna al DataGridView, se non è già presente
            if (grdRpcTestnet.Columns.Count == 0)
            {
                grdRpcTestnet.Columns.Add("RpcTest", "Url Rpc Testnet");
            }

            // Pulisci le righe esistenti
            grdRpcTestnet.Rows.Clear();

            // Itera sulla lista e aggiungi ogni elemento come nuova riga nel DataGridView
            foreach (string item in config.TestnetRpcUrls)
            {
                grdRpcTestnet.Rows.Add(item);
            }
            // Aggiungi una colonna al DataGridView, se non è già presente
            if (grdSacTokens.Columns.Count == 0)
            {
                grdSacTokens.Columns.Add("SacTokens", "Name of Sac Token to Search");
            }

            // Pulisci le righe esistenti
            grdSacTokens.Rows.Clear();

            // Itera sulla lista e aggiungi ogni elemento come nuova riga nel DataGridView
            foreach (string item in System.IO.File.ReadAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.ProcessPath), "sacTokens.txt")).ToList())
            {
                grdSacTokens.Rows.Add(item);
            }
            // Aggiungi una colonna al DataGridView, se non è già presente
            if (grdClonedTokens.Columns.Count == 0)
            {
                grdClonedTokens.Columns.Add("ClonedTokens", "Cloned Tokens Contract Address");
            }

            // Pulisci le righe esistenti
            grdClonedTokens.Rows.Clear();

            // Itera sulla lista e aggiungi ogni elemento come nuova riga nel DataGridView
            foreach (string item in System.IO.File.ReadAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.ProcessPath), "clonedTokens.txt")).ToList())
            {
                grdClonedTokens.Rows.Add(item);
            }

        }

        private void SaveConfig()
        {
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");

            config.ExitOnFirstRun = chkRunTest.Checked;
            config.RunAsService = chkRunAsService.Checked;
            config.CheckInterval = Int32.Parse(numCheckInterval.Value.ToString());
            config.StatusInterval = Int32.Parse(numStatusInterval.Value.ToString());

            config.TelegramBotToken = txtTelegramBotToken.Text;
            config.TelegramBotUsername = txtTelegramBotUsername.Text;
            config.StartWallet = txtStartWallet.Text;
            config.StartSeed = txtStartSeed.Text;
            config.StartPrivateKey = txtStartPrivateKey.Text;
            config.TargetWallet = txtTargetWallet.Text;
            config.Percentage = Convert.ToInt32(numPercentage.Value).ToString() + "%";
            config.RpcUrls = new List<string>();
            foreach (DataGridViewRow row in grdRpcMain.Rows)
            {
                if (!row.IsNewRow) // Ignora l'ultima riga vuota nel DataGridView
                {
                    config.RpcUrls.Add(row.Cells[0].Value.ToString());
                }
            }

            config.TestnetRpcUrls = new List<string>();
            foreach (DataGridViewRow row in grdRpcTestnet.Rows)
            {
                if (!row.IsNewRow) // Ignora l'ultima riga vuota nel DataGridView
                {
                    config.TestnetRpcUrls.Add(row.Cells[0].Value.ToString());
                }
            }
            List<string> _sacTokens = new List<string>();
            foreach (DataGridViewRow row in grdSacTokens.Rows)
            {
                if (!row.IsNewRow) // Ignora l'ultima riga vuota nel DataGridView
                {
                    _sacTokens.Add(row.Cells[0].Value.ToString());
                }
            }
            List<string> _clonedTokens = new List<string>();
            foreach (DataGridViewRow row in grdClonedTokens.Rows)
            {
                if (!row.IsNewRow) // Ignora l'ultima riga vuota nel DataGridView
                {
                    _clonedTokens.Add(row.Cells[0].Value.ToString());
                }
            }

            config.Save();

            System.IO.File.WriteAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.ProcessPath), "sacTokens.txt"), _sacTokens.ToArray());
            System.IO.File.WriteAllLines(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Environment.ProcessPath), "clonedTokens.txt"), _clonedTokens.ToArray());

            MessageBox.Show("Configuration Saved!");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitConfig();
            MessageBox.Show("Configuration reloaded!");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void chkRunTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkRunTest.Checked) { numPercentage.Value = 100; numPercentage.Enabled = false; } else { numPercentage.Enabled = true; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TelegramBot bot = new TelegramBot();
            bot.Show();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            
            PWMConfiguration config = PWMConfiguration.LoadFromFile("config.xml");

            if((String.IsNullOrWhiteSpace( config.StartSeed ) && String.IsNullOrWhiteSpace( config.StartPrivateKey)) || String.IsNullOrWhiteSpace(config.StartWallet))
            {
                MessageBox.Show("Please insert Start Wallet, Start Seed or Start Private Key in the configuration tab");
                return;
            }
            
            string rpcUrl = string.Empty;

            using (var inputBox = new InputBox("Insert Rpc node to test", "Insert Rpc address:"))
            {
                var result = inputBox.ShowDialog();
                if (result == DialogResult.OK)
                {
                    rpcUrl = inputBox.InputText;
                    // Fai qualcosa con rpcUrl
                    MessageBox.Show(rpcUrl + " Node inserted");
                }
                else
                {
                    // L'utente ha annullato l'operazione
                    MessageBox.Show("Operation Cancelled.");
                }
            }
            try
            {
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


                if (await PulseChainRpcService.VerifyPrivateKeyOnBlockchainAsync(rpcUrl, privateKey, config.StartWallet))
                {
                    MessageBox.Show("The Wallet is correct!");
                }
                else
                {
                    MessageBox.Show("An error occurred\nPlease check Address, Seed Phrase and Private Key");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred\nPlease check Address, Seed Phrase and Private Key");
            }
        }
    }

    public class InputBox : Form
    {
        // Aggiungi i controlli necessari come campi privati
        private Label _label;
        private TextBox _textBox;
        private Button _okButton;
        private Button _cancelButton;

        // Aggiungi una proprietà pubblica per ottenere il testo inserito
        public string InputText => _textBox.Text;

        // Costruttore della classe InputBox
        public InputBox(string title, string prompt)
        {
            // Imposta il titolo e il testo del prompt
            Text = title;
            _label = new Label { Text = prompt, Left = 10, Top = 10, Width = 300 };
            _textBox = new TextBox { Left = 10, Top = 30, Width = 300 };

            // Configura il pulsante OK
            _okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 150, Top = 60 };
            _okButton.Click += (sender, e) => Close();

            // Configura il pulsante Annulla
            _cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 230, Top = 60 };
            _cancelButton.Click += (sender, e) => Close();

            // Aggiungi i controlli alla form
            Controls.Add(_label);
            Controls.Add(_textBox);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);

            // Imposta altre proprietà della form
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new System.Drawing.Size(320, 100);
        }
    }

}