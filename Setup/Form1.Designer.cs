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
            this.gridRpcTest = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numPercentage = new System.Windows.Forms.NumericUpDown();
            this.txtTargetWallet = new System.Windows.Forms.TextBox();
            this.txtStartSeed = new System.Windows.Forms.TextBox();
            this.txtStartWallet = new System.Windows.Forms.TextBox();
            this.txtTelegramBotUsername = new System.Windows.Forms.TextBox();
            this.txtTelegramBotToken = new System.Windows.Forms.TextBox();
            this.numStatusInterval = new System.Windows.Forms.NumericUpDown();
            this.numCheckInterval = new System.Windows.Forms.NumericUpDown();
            this.chkRunTest = new System.Windows.Forms.CheckBox();
            this.chkRunAsService = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grdRpcMain = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grdRpcTestnet = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.grdSacTokens = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.grdClonedTokens = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.gridRpcTest.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStatusInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckInterval)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdRpcMain)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdRpcTestnet)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSacTokens)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdClonedTokens)).BeginInit();
            this.SuspendLayout();
            // 
            // gridRpcTest
            // 
            this.gridRpcTest.Controls.Add(this.tabPage1);
            this.gridRpcTest.Controls.Add(this.tabPage2);
            this.gridRpcTest.Controls.Add(this.tabPage3);
            this.gridRpcTest.Controls.Add(this.tabPage4);
            this.gridRpcTest.Controls.Add(this.tabPage5);
            this.gridRpcTest.Location = new System.Drawing.Point(12, 12);
            this.gridRpcTest.Name = "gridRpcTest";
            this.gridRpcTest.SelectedIndex = 0;
            this.gridRpcTest.Size = new System.Drawing.Size(776, 378);
            this.gridRpcTest.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.numPercentage);
            this.tabPage1.Controls.Add(this.txtTargetWallet);
            this.tabPage1.Controls.Add(this.txtStartSeed);
            this.tabPage1.Controls.Add(this.txtStartWallet);
            this.tabPage1.Controls.Add(this.txtTelegramBotUsername);
            this.tabPage1.Controls.Add(this.txtTelegramBotToken);
            this.tabPage1.Controls.Add(this.numStatusInterval);
            this.tabPage1.Controls.Add(this.numCheckInterval);
            this.tabPage1.Controls.Add(this.chkRunTest);
            this.tabPage1.Controls.Add(this.chkRunAsService);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(768, 350);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General Options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(333, 259);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(151, 15);
            this.label8.TabIndex = 18;
            this.label8.Text = "Original Wallet Seed Phrase";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(688, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(70, 316);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(204, 15);
            this.label7.TabIndex = 16;
            this.label7.Text = "Percentage of Token to be transferred";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(333, 288);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Destination Wallet Address";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(333, 230);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Original Wallet Address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(333, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Telegram Bot Username";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(333, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "Telegram Bot Token";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Update Frequency";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(70, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Check Interval Delay";
            // 
            // numPercentage
            // 
            this.numPercentage.Location = new System.Drawing.Point(20, 314);
            this.numPercentage.Name = "numPercentage";
            this.numPercentage.Size = new System.Drawing.Size(44, 23);
            this.numPercentage.TabIndex = 9;
            // 
            // txtTargetWallet
            // 
            this.txtTargetWallet.Location = new System.Drawing.Point(20, 285);
            this.txtTargetWallet.Name = "txtTargetWallet";
            this.txtTargetWallet.Size = new System.Drawing.Size(307, 23);
            this.txtTargetWallet.TabIndex = 8;
            // 
            // txtStartSeed
            // 
            this.txtStartSeed.Location = new System.Drawing.Point(20, 256);
            this.txtStartSeed.Name = "txtStartSeed";
            this.txtStartSeed.Size = new System.Drawing.Size(307, 23);
            this.txtStartSeed.TabIndex = 7;
            // 
            // txtStartWallet
            // 
            this.txtStartWallet.Location = new System.Drawing.Point(20, 227);
            this.txtStartWallet.Name = "txtStartWallet";
            this.txtStartWallet.Size = new System.Drawing.Size(307, 23);
            this.txtStartWallet.TabIndex = 6;
            // 
            // txtTelegramBotUsername
            // 
            this.txtTelegramBotUsername.Location = new System.Drawing.Point(20, 170);
            this.txtTelegramBotUsername.Name = "txtTelegramBotUsername";
            this.txtTelegramBotUsername.Size = new System.Drawing.Size(307, 23);
            this.txtTelegramBotUsername.TabIndex = 5;
            // 
            // txtTelegramBotToken
            // 
            this.txtTelegramBotToken.Location = new System.Drawing.Point(20, 141);
            this.txtTelegramBotToken.Name = "txtTelegramBotToken";
            this.txtTelegramBotToken.Size = new System.Drawing.Size(307, 23);
            this.txtTelegramBotToken.TabIndex = 4;
            // 
            // numStatusInterval
            // 
            this.numStatusInterval.Location = new System.Drawing.Point(20, 112);
            this.numStatusInterval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numStatusInterval.Name = "numStatusInterval";
            this.numStatusInterval.Size = new System.Drawing.Size(44, 23);
            this.numStatusInterval.TabIndex = 3;
            // 
            // numCheckInterval
            // 
            this.numCheckInterval.Location = new System.Drawing.Point(20, 83);
            this.numCheckInterval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numCheckInterval.Name = "numCheckInterval";
            this.numCheckInterval.Size = new System.Drawing.Size(44, 23);
            this.numCheckInterval.TabIndex = 2;
            // 
            // chkRunTest
            // 
            this.chkRunTest.AutoSize = true;
            this.chkRunTest.Location = new System.Drawing.Point(20, 58);
            this.chkRunTest.Name = "chkRunTest";
            this.chkRunTest.Size = new System.Drawing.Size(169, 19);
            this.chkRunTest.TabIndex = 1;
            this.chkRunTest.Text = "Run on TestNet Rpc Servers";
            this.chkRunTest.UseVisualStyleBackColor = true;
            // 
            // chkRunAsService
            // 
            this.chkRunAsService.AutoSize = true;
            this.chkRunAsService.Location = new System.Drawing.Point(20, 33);
            this.chkRunAsService.Name = "chkRunAsService";
            this.chkRunAsService.Size = new System.Drawing.Size(103, 19);
            this.chkRunAsService.TabIndex = 0;
            this.chkRunAsService.Text = "Run As Service";
            this.chkRunAsService.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.grdRpcMain);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(768, 350);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mainnet Rpc Urls";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // grdRpcMain
            // 
            this.grdRpcMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdRpcMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdRpcMain.Location = new System.Drawing.Point(20, 35);
            this.grdRpcMain.Name = "grdRpcMain";
            this.grdRpcMain.RowTemplate.Height = 25;
            this.grdRpcMain.Size = new System.Drawing.Size(723, 177);
            this.grdRpcMain.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.grdRpcTestnet);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(768, 350);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Testnet Rpc Urls";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // grdRpcTestnet
            // 
            this.grdRpcTestnet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdRpcTestnet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdRpcTestnet.Location = new System.Drawing.Point(20, 35);
            this.grdRpcTestnet.Name = "grdRpcTestnet";
            this.grdRpcTestnet.RowTemplate.Height = 25;
            this.grdRpcTestnet.Size = new System.Drawing.Size(723, 177);
            this.grdRpcTestnet.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.grdSacTokens);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(768, 350);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Native Tokens to Transfer";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // grdSacTokens
            // 
            this.grdSacTokens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdSacTokens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSacTokens.Location = new System.Drawing.Point(20, 35);
            this.grdSacTokens.Name = "grdSacTokens";
            this.grdSacTokens.RowTemplate.Height = 25;
            this.grdSacTokens.Size = new System.Drawing.Size(723, 177);
            this.grdSacTokens.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.grdClonedTokens);
            this.tabPage5.Location = new System.Drawing.Point(4, 24);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(768, 350);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Cloned Token Contract Address to Transfer";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // grdClonedTokens
            // 
            this.grdClonedTokens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdClonedTokens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdClonedTokens.Location = new System.Drawing.Point(20, 35);
            this.grdClonedTokens.Name = "grdClonedTokens";
            this.grdClonedTokens.RowTemplate.Height = 25;
            this.grdClonedTokens.Size = new System.Drawing.Size(723, 177);
            this.grdClonedTokens.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.Location = new System.Drawing.Point(16, 396);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(760, 42);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "SAVE CONFIGURATION";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gridRpcTest);
            this.Name = "Form1";
            this.Text = "Pulsechain Wallet Mover Config";
            this.gridRpcTest.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStatusInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckInterval)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdRpcMain)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdRpcTestnet)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSacTokens)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdClonedTokens)).EndInit();
            this.ResumeLayout(false);

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
    }
}