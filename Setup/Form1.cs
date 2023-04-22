using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Configuration;

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
            Configuration config = Configuration.LoadFromFile("config.xml");

            chkRunTest.Checked = config.RunTest;
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
            Configuration config = Configuration.LoadFromFile("config.xml");

            config.RunTest =chkRunTest.Checked;
            config.RunAsService=chkRunAsService.Checked;
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
        public string? StartPrivateKey { get; set; }

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

}