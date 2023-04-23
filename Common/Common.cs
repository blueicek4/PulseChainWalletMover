using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using log4net;

namespace Common
{
    [XmlRoot("Configuration")]
    public class PWMConfiguration
    {
        public bool ExitOnFirstRun { get; set; }
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
        public string StartPrivateKey { get; set; }

        public static PWMConfiguration LoadFromFile(string filePath)
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
            var conf = xml.DeserializeToObject<PWMConfiguration>(fullPath);
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

            XmlSerializer serializer = new XmlSerializer(typeof(PWMConfiguration));
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
    public class TransferData
    {
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string ContractAddress { get; set; }
        public string ContractName { get; set; }
    }
    public class TestResult
    {
        public DateTime lastUpdate { get; set; }
        public SerializableDictionary<string, bool> dnsStatus { get; set; }
        public SerializableDictionary<string, bool> rpcStatus { get; set; }
        public TestResult()
        {
            lastUpdate = DateTime.MinValue;
            rpcStatus = new SerializableDictionary<string, bool>();
            dnsStatus = new SerializableDictionary<string, bool>();
        }
        public TestResult(string filePath)
        {
            try
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
                TestResult temp;
                XmlSerializer serializer = new XmlSerializer(typeof(TestResult));
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    temp = (TestResult)serializer.Deserialize(reader);
                }


                this.lastUpdate = temp.lastUpdate;
                this.rpcStatus = temp.rpcStatus;
                this.dnsStatus = temp.dnsStatus;
                this.rpcStatus = temp.rpcStatus;
            }
            catch (Exception ex)
            {
                this.lastUpdate = DateTime.MinValue;
                this.rpcStatus = new SerializableDictionary<string, bool>();
                this.dnsStatus = new SerializableDictionary<string, bool>();
                this.rpcStatus = new SerializableDictionary<string, bool>();
            }
        }
        public static TestResult LoadFromFile(string filePath)
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

            XmlSerializer serializer = new XmlSerializer(typeof(TestResult));
            using (StreamReader reader = new StreamReader(fullPath))
            {
                return (TestResult)serializer.Deserialize(reader);
            }
        }

        public void SaveToFile(string filePath)
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

            XmlSerializer serializer = new XmlSerializer(typeof(TestResult));
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
        public void Save()
        {
            this.lastUpdate = DateTime.Now;
            string filePath = "testresult.xml";
            SaveToFile(filePath);
        }
    }
    public static class StringExtensions
    {
        public static string EscapeMarkdownV2(this string text)
        {
            return (text ?? String.Empty)
                .Replace("_", "\\_")
                .Replace("*", "\\*")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("~", "\\~")
                .Replace("`", "\\`")
                .Replace(">", "\\>")
                .Replace("#", "\\#")
                .Replace("+", "\\+")
                .Replace("-", "\\-")
                .Replace("=", "\\=")
                .Replace("|", "\\|")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace(".", "\\.")
                .Replace("!", "\\!");
        }
    }

    public static class Tools
    {
        public static string GetAllWebsitesStatus(Dictionary<string, bool> websiteStatus)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, bool> entry in websiteStatus.OrderByDescending(k => k.Value))
            {
                sb.AppendLine($"{(entry.Value ? "\U0001F7E2" : "\U0001F534")} {entry.Key.EscapeMarkdownV2()}");
            }

            return sb.ToString();
        }
        public static string GetAllDnsStatus(Dictionary<string, bool> dnsStatus)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, bool> entry in dnsStatus.OrderByDescending(k => k.Value))
            {
                sb.AppendLine($"{(entry.Value ? "\U0001F7E2" : "\U0001F534")} {entry.Key.EscapeMarkdownV2()}");
            }

            return sb.ToString();
        }
        public static string GetAllRpcStatus(Dictionary<string, bool> rpcStatus)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, bool> entry in rpcStatus.OrderByDescending(k => k.Value))
            {
                sb.AppendLine($"{(entry.Value ? "\U0001F7E2" : "\U0001F534")} {entry.Key.EscapeMarkdownV2()}");
            }

            return sb.ToString();
        }

    }

}