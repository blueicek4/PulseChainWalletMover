namespace Setup
{
    partial class TelegramBot
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblBotStatus = new Label();
            btnResetChats = new Button();
            btnClose = new Button();
            btnOpenChat = new Button();
            SuspendLayout();
            // 
            // lblBotStatus
            // 
            lblBotStatus.AutoSize = true;
            lblBotStatus.Location = new Point(12, 9);
            lblBotStatus.Name = "lblBotStatus";
            lblBotStatus.Size = new Size(16, 15);
            lblBotStatus.TabIndex = 0;
            lblBotStatus.Text = "...";
            // 
            // btnResetChats
            // 
            btnResetChats.Location = new Point(12, 64);
            btnResetChats.Name = "btnResetChats";
            btnResetChats.Size = new Size(174, 23);
            btnResetChats.TabIndex = 1;
            btnResetChats.Text = "Reset Chat Destinations";
            btnResetChats.UseVisualStyleBackColor = true;
            btnResetChats.Click += btnResetChats_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(12, 93);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(174, 23);
            btnClose.TabIndex = 2;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnOpenChat
            // 
            btnOpenChat.Location = new Point(12, 35);
            btnOpenChat.Name = "btnOpenChat";
            btnOpenChat.Size = new Size(174, 23);
            btnOpenChat.TabIndex = 3;
            btnOpenChat.Text = "Open Telegram Chat";
            btnOpenChat.UseVisualStyleBackColor = true;
            btnOpenChat.Click += btnOpenChat_Click;
            // 
            // TelegramBot
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(198, 128);
            Controls.Add(btnOpenChat);
            Controls.Add(btnClose);
            Controls.Add(btnResetChats);
            Controls.Add(lblBotStatus);
            Name = "TelegramBot";
            Text = "TelegramBot";
            Load += TelegramBot_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblBotStatus;
        private Button btnResetChats;
        private Button btnClose;
        private Button btnOpenChat;
    }
}