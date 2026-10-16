//==========================================================================
                          //CHATBOTFORMDESIGNER
//==========================================================================
namespace CybersecurityChatbotGUI
{
    partial class ChatbotForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code

        //1. Required method for Designer support - do not modify the contents
        // of this method with the code editor.
        private void InitializeComponent() 
        {
            pnlChat = new Panel();
            rtbChat = new RichTextBox();
            pnlHeader = new Panel();
            lblAscii = new Label();
            lblSubtitle = new Label();
            pnlInput = new Panel();
            txtInput = new TextBox();
            btnSend = new Button();
            pnlChat.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlInput.SuspendLayout();
            SuspendLayout();
            //2.
            // pnlChat
            // 
            pnlChat.BackColor = Color.FromArgb(15, 23, 42);
            pnlChat.Controls.Add(rtbChat);
            pnlChat.Dock = DockStyle.Fill;
            pnlChat.Name = "pnlChat";
            pnlChat.Padding = new Padding(16, 12, 16, 12);
            pnlChat.TabIndex = 0;
            //3. 
            // rtbChat
            // 
            rtbChat.BackColor = Color.FromArgb(15, 23, 42);
            rtbChat.BorderStyle = BorderStyle.None;
            rtbChat.Dock = DockStyle.Fill;
            rtbChat.Font = new Font("Segoe UI", 11F);
            rtbChat.ForeColor = Color.FromArgb(226, 232, 240);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbChat.TabIndex = 0;
            rtbChat.TabStop = false;
            rtbChat.Text = "";
            //4. 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(17, 28, 46);
            pnlHeader.Controls.Add(lblAscii);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(900, 175);
            pnlHeader.TabIndex = 1;
            pnlHeader.Resize += pnlHeader_Resize;
            //5. 
            // lblAscii
            // 
            lblAscii.AutoSize = true;
            lblAscii.Font = new Font("Consolas", 9F);
            lblAscii.ForeColor = Color.FromArgb(34, 197, 94);
            lblAscii.Location = new Point(0, 10);
            lblAscii.Name = "lblAscii";
            lblAscii.Size = new Size(0, 14);
            lblAscii.TabIndex = 0;
            //6. 
            // lblSubtitle
            // 
            lblSubtitle.Dock = DockStyle.Bottom;
            lblSubtitle.Font = new Font("Segoe UI", 9.5F);
            lblSubtitle.ForeColor = Color.FromArgb(148, 163, 184);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(900, 30);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Ask me about passwords, phishing, safe browsing, scams and privacy. Type 'help' for topics.";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            //7. 
            // pnlInput
            // 
            pnlInput.BackColor = Color.FromArgb(17, 28, 46);
            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);
            pnlInput.Dock = DockStyle.Bottom;
            pnlInput.Name = "pnlInput";
            pnlInput.Size = new Size(900, 68);
            pnlInput.TabIndex = 2;
            //8. 
            // txtInput
            // 
            txtInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInput.BackColor = Color.FromArgb(30, 41, 59);
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Font = new Font("Segoe UI", 12F);
            txtInput.ForeColor = Color.White;
            txtInput.Location = new Point(14, 17);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(750, 29);
            txtInput.TabIndex = 0;
            //9. 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.BackColor = Color.FromArgb(22, 163, 74);
            btnSend.Cursor = Cursors.Hand;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnSend.ForeColor = Color.White;
            btnSend.Location = new Point(776, 15);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(110, 34);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            //10. 
            // ChatbotForm
            // 
            AcceptButton = btnSend;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(11, 18, 32);
            ClientSize = new Size(900, 700);
            Controls.Add(pnlChat);
            Controls.Add(pnlHeader);
            Controls.Add(pnlInput);
            MinimumSize = new Size(660, 560);
            Name = "ChatbotForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cybersecurity Awareness Bot";
            Load += ChatbotForm_Load;
            pnlChat.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlInput.ResumeLayout(false);
            pnlInput.PerformLayout();
            ResumeLayout(false);
        }

        //11. Closing the design
        #endregion

        private Panel pnlChat;
        private RichTextBox rtbChat;
        private Panel pnlHeader;
        private Label lblAscii;
        private Label lblSubtitle;
        private Panel pnlInput;
        private TextBox txtInput;
        private Button btnSend;
    }
}
