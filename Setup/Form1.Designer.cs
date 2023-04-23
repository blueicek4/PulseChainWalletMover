namespace Setup
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gridRpcTest = new TabControl();
            tabPage1 = new TabPage();
            button3 = new Button();
            button2 = new Button();
            txtStartPrivateKey = new TextBox();
            label9 = new Label();
            label8 = new Label();
            button1 = new Button();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            numPercentage = new NumericUpDown();
            txtTargetWallet = new TextBox();
            txtStartSeed = new TextBox();
            txtStartWallet = new TextBox();
            txtTelegramBotUsername = new TextBox();
            txtTelegramBotToken = new TextBox();
            numStatusInterval = new NumericUpDown();
            numCheckInterval = new NumericUpDown();
            chkRunTest = new CheckBox();
            chkRunAsService = new CheckBox();
            tabPage2 = new TabPage();
            grdRpcMain = new DataGridView();
            tabPage3 = new TabPage();
            grdRpcTestnet = new DataGridView();
            tabPage4 = new TabPage();
            grdSacTokens = new DataGridView();
            tabPage5 = new TabPage();
            grdClonedTokens = new DataGridView();
            btnSave = new Button();
            gridRpcTest.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPercentage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStatusInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCheckInterval).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdRpcMain).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdRpcTestnet).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdSacTokens).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdClonedTokens).BeginInit();
            SuspendLayout();
            // 
            // gridRpcTest
            // 
            gridRpcTest.Controls.Add(tabPage1);
            gridRpcTest.Controls.Add(tabPage2);
            gridRpcTest.Controls.Add(tabPage3);
            gridRpcTest.Controls.Add(tabPage4);
            gridRpcTest.Controls.Add(tabPage5);
            gridRpcTest.Location = new Point(12, 12);
            gridRpcTest.Name = "gridRpcTest";
            gridRpcTest.SelectedIndex = 0;
            gridRpcTest.Size = new Size(776, 378);
            gridRpcTest.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(button3);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(txtStartPrivateKey);
            tabPage1.Controls.Add(label9);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(numPercentage);
            tabPage1.Controls.Add(txtTargetWallet);
            tabPage1.Controls.Add(txtStartSeed);
            tabPage1.Controls.Add(txtStartWallet);
            tabPage1.Controls.Add(txtTelegramBotUsername);
            tabPage1.Controls.Add(txtTelegramBotToken);
            tabPage1.Controls.Add(numStatusInterval);
            tabPage1.Controls.Add(numCheckInterval);
            tabPage1.Controls.Add(chkRunTest);
            tabPage1.Controls.Add(chkRunAsService);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(768, 350);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "General Options";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(650, 227);
            button3.Name = "button3";
            button3.Size = new Size(110, 23);
            button3.TabIndex = 22;
            button3.Text = "Check Wallet";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(650, 165);
            button2.Name = "button2";
            button2.Size = new Size(110, 23);
            button2.TabIndex = 21;
            button2.Text = "Setup Telegram";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // txtStartPrivateKey
            // 
            txtStartPrivateKey.Location = new Point(20, 257);
            txtStartPrivateKey.Name = "txtStartPrivateKey";
            txtStartPrivateKey.Size = new Size(307, 23);
            txtStartPrivateKey.TabIndex = 20;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(333, 260);
            label9.Name = "label9";
            label9.Size = new Size(143, 15);
            label9.TabIndex = 19;
            label9.Text = "Original Wallet PrivateKey";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(333, 231);
            label8.Name = "label8";
            label8.Size = new Size(151, 15);
            label8.TabIndex = 18;
            label8.Text = "Original Wallet Seed Phrase";
            // 
            // button1
            // 
            button1.Location = new Point(650, 6);
            button1.Name = "button1";
            button1.Size = new Size(112, 23);
            button1.TabIndex = 17;
            button1.Text = "Refresh";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(70, 316);
            label7.Name = "label7";
            label7.Size = new Size(204, 15);
            label7.TabIndex = 16;
            label7.Text = "Percentage of Token to be transferred";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(333, 288);
            label6.Name = "label6";
            label6.Size = new Size(148, 15);
            label6.TabIndex = 15;
            label6.Text = "Destination Wallet Address";
            label6.Click += label6_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(333, 202);
            label5.Name = "label5";
            label5.Size = new Size(130, 15);
            label5.TabIndex = 14;
            label5.Text = "Original Wallet Address";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(333, 173);
            label4.Name = "label4";
            label4.Size = new Size(132, 15);
            label4.TabIndex = 13;
            label4.Text = "Telegram Bot Username";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(333, 144);
            label3.Name = "label3";
            label3.Size = new Size(110, 15);
            label3.TabIndex = 12;
            label3.Text = "Telegram Bot Token";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(70, 114);
            label2.Name = "label2";
            label2.Size = new Size(149, 15);
            label2.TabIndex = 11;
            label2.Text = "Update Frequency Minutes";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(70, 85);
            label1.Name = "label1";
            label1.Size = new Size(161, 15);
            label1.TabIndex = 10;
            label1.Text = "Check Interval Delay Seconds";
            // 
            // numPercentage
            // 
            numPercentage.Location = new Point(20, 314);
            numPercentage.Name = "numPercentage";
            numPercentage.Size = new Size(44, 23);
            numPercentage.TabIndex = 9;
            // 
            // txtTargetWallet
            // 
            txtTargetWallet.Location = new Point(20, 285);
            txtTargetWallet.Name = "txtTargetWallet";
            txtTargetWallet.Size = new Size(307, 23);
            txtTargetWallet.TabIndex = 8;
            // 
            // txtStartSeed
            // 
            txtStartSeed.Location = new Point(20, 228);
            txtStartSeed.Name = "txtStartSeed";
            txtStartSeed.Size = new Size(307, 23);
            txtStartSeed.TabIndex = 7;
            // 
            // txtStartWallet
            // 
            txtStartWallet.Location = new Point(20, 199);
            txtStartWallet.Name = "txtStartWallet";
            txtStartWallet.Size = new Size(307, 23);
            txtStartWallet.TabIndex = 6;
            // 
            // txtTelegramBotUsername
            // 
            txtTelegramBotUsername.Location = new Point(20, 170);
            txtTelegramBotUsername.Name = "txtTelegramBotUsername";
            txtTelegramBotUsername.Size = new Size(307, 23);
            txtTelegramBotUsername.TabIndex = 5;
            // 
            // txtTelegramBotToken
            // 
            txtTelegramBotToken.Location = new Point(20, 141);
            txtTelegramBotToken.Name = "txtTelegramBotToken";
            txtTelegramBotToken.Size = new Size(307, 23);
            txtTelegramBotToken.TabIndex = 4;
            // 
            // numStatusInterval
            // 
            numStatusInterval.Location = new Point(20, 112);
            numStatusInterval.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            numStatusInterval.Name = "numStatusInterval";
            numStatusInterval.Size = new Size(44, 23);
            numStatusInterval.TabIndex = 3;
            // 
            // numCheckInterval
            // 
            numCheckInterval.Location = new Point(20, 83);
            numCheckInterval.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            numCheckInterval.Name = "numCheckInterval";
            numCheckInterval.Size = new Size(44, 23);
            numCheckInterval.TabIndex = 2;
            // 
            // chkRunTest
            // 
            chkRunTest.AutoSize = true;
            chkRunTest.Location = new Point(20, 58);
            chkRunTest.Name = "chkRunTest";
            chkRunTest.Size = new Size(169, 19);
            chkRunTest.TabIndex = 1;
            chkRunTest.Text = "Run on TestNet Rpc Servers";
            chkRunTest.UseVisualStyleBackColor = true;
            chkRunTest.CheckedChanged += chkRunTest_CheckedChanged;
            // 
            // chkRunAsService
            // 
            chkRunAsService.AutoSize = true;
            chkRunAsService.Location = new Point(20, 33);
            chkRunAsService.Name = "chkRunAsService";
            chkRunAsService.Size = new Size(123, 19);
            chkRunAsService.TabIndex = 0;
            chkRunAsService.Text = "Exit After First Run";
            chkRunAsService.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(grdRpcMain);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(768, 350);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Mainnet Rpc Urls";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // grdRpcMain
            // 
            grdRpcMain.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grdRpcMain.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdRpcMain.Location = new Point(20, 35);
            grdRpcMain.Name = "grdRpcMain";
            grdRpcMain.RowTemplate.Height = 25;
            grdRpcMain.Size = new Size(723, 177);
            grdRpcMain.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(grdRpcTestnet);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(768, 350);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Testnet Rpc Urls";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // grdRpcTestnet
            // 
            grdRpcTestnet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grdRpcTestnet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdRpcTestnet.Location = new Point(20, 35);
            grdRpcTestnet.Name = "grdRpcTestnet";
            grdRpcTestnet.RowTemplate.Height = 25;
            grdRpcTestnet.Size = new Size(723, 177);
            grdRpcTestnet.TabIndex = 1;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(grdSacTokens);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(768, 350);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Native Tokens to Transfer";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // grdSacTokens
            // 
            grdSacTokens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grdSacTokens.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdSacTokens.Location = new Point(20, 35);
            grdSacTokens.Name = "grdSacTokens";
            grdSacTokens.RowTemplate.Height = 25;
            grdSacTokens.Size = new Size(723, 177);
            grdSacTokens.TabIndex = 0;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(grdClonedTokens);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(768, 350);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Cloned Token Contract Address to Transfer";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // grdClonedTokens
            // 
            grdClonedTokens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grdClonedTokens.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdClonedTokens.Location = new Point(20, 35);
            grdClonedTokens.Name = "grdClonedTokens";
            grdClonedTokens.RowTemplate.Height = 25;
            grdClonedTokens.Size = new Size(723, 177);
            grdClonedTokens.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnSave.Location = new Point(16, 396);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(760, 42);
            btnSave.TabIndex = 1;
            btnSave.Text = "SAVE CONFIGURATION";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSave);
            Controls.Add(gridRpcTest);
            Name = "Form1";
            Text = "Pulsechain Wallet Mover Config";
            gridRpcTest.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPercentage).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStatusInterval).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCheckInterval).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdRpcMain).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdRpcTestnet).EndInit();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdSacTokens).EndInit();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdClonedTokens).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl gridRpcTest;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private NumericUpDown numPercentage;
        private TextBox txtTargetWallet;
        private TextBox txtStartSeed;
        private TextBox txtStartWallet;
        private TextBox txtTelegramBotUsername;
        private TextBox txtTelegramBotToken;
        private NumericUpDown numStatusInterval;
        private NumericUpDown numCheckInterval;
        private CheckBox chkRunTest;
        private CheckBox chkRunAsService;
        private Label label8;
        private Button button1;
        private Label label7;
        private DataGridView grdRpcMain;
        private TabPage tabPage3;
        private DataGridView grdRpcTestnet;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private Button btnSave;
        private DataGridView grdSacTokens;
        private DataGridView grdClonedTokens;
        private TextBox txtStartPrivateKey;
        private Label label9;
        private Button button3;
        private Button button2;
    }
}