namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    partial class ServerEvents
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
            this.GUIEvents = new System.Windows.Forms.Label();
            this.ListEvent = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listEvents = new System.Windows.Forms.ListView();
            this.List1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.List2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listEvents2 = new System.Windows.Forms.ListView();
            this.Header1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listAdminEvents = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listWeaponEvents = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.listChatHistory = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ThreadEventsType = new System.ComponentModel.BackgroundWorker();
            this.btnExit = new System.Windows.Forms.Label();
            this.ListEvent.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // GUIEvents
            // 
            this.GUIEvents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.GUIEvents.Enabled = false;
            this.GUIEvents.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GUIEvents.ForeColor = System.Drawing.Color.Black;
            this.GUIEvents.Location = new System.Drawing.Point(0, 0);
            this.GUIEvents.Name = "GUIEvents";
            this.GUIEvents.Size = new System.Drawing.Size(778, 22);
            this.GUIEvents.TabIndex = 3;
            this.GUIEvents.Text = "S.E.R.V.E.R - Статистика сервера";
            this.GUIEvents.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.GUIEvents.Click += new System.EventHandler(this.GUIEvents_Click);
            // 
            // ListEvent
            // 
            this.ListEvent.Controls.Add(this.tabPage1);
            this.ListEvent.Controls.Add(this.tabPage2);
            this.ListEvent.Controls.Add(this.tabPage3);
            this.ListEvent.Controls.Add(this.tabPage4);
            this.ListEvent.Controls.Add(this.tabPage5);
            this.ListEvent.Location = new System.Drawing.Point(-3, 25);
            this.ListEvent.Name = "ListEvent";
            this.ListEvent.SelectedIndex = 0;
            this.ListEvent.ShowToolTips = true;
            this.ListEvent.Size = new System.Drawing.Size(784, 463);
            this.ListEvent.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listEvents);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(776, 437);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Статистика";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listEvents
            // 
            this.listEvents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.List1,
            this.List2});
            this.listEvents.FullRowSelect = true;
            this.listEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listEvents.Location = new System.Drawing.Point(0, 0);
            this.listEvents.Name = "listEvents";
            this.listEvents.ShowItemToolTips = true;
            this.listEvents.Size = new System.Drawing.Size(776, 437);
            this.listEvents.TabIndex = 0;
            this.listEvents.UseCompatibleStateImageBehavior = false;
            this.listEvents.View = System.Windows.Forms.View.Details;
            // 
            // List1
            // 
            this.List1.Text = "Тип события";
            this.List1.Width = 661;
            // 
            // List2
            // 
            this.List2.Text = "Всего событий";
            this.List2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.List2.Width = 91;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listEvents2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(776, 437);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Заблокированные атаки";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listEvents2
            // 
            this.listEvents2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listEvents2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Header1});
            this.listEvents2.FullRowSelect = true;
            this.listEvents2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listEvents2.Location = new System.Drawing.Point(0, 0);
            this.listEvents2.Name = "listEvents2";
            this.listEvents2.ShowItemToolTips = true;
            this.listEvents2.Size = new System.Drawing.Size(776, 437);
            this.listEvents2.TabIndex = 1;
            this.listEvents2.UseCompatibleStateImageBehavior = false;
            this.listEvents2.View = System.Windows.Forms.View.Details;
            // 
            // Header1
            // 
            this.Header1.Text = "Статистика";
            this.Header1.Width = 752;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listAdminEvents);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(776, 437);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Удаленный доступ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listAdminEvents
            // 
            this.listAdminEvents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listAdminEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listAdminEvents.FullRowSelect = true;
            this.listAdminEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listAdminEvents.Location = new System.Drawing.Point(0, 0);
            this.listAdminEvents.Name = "listAdminEvents";
            this.listAdminEvents.ShowItemToolTips = true;
            this.listAdminEvents.Size = new System.Drawing.Size(776, 437);
            this.listAdminEvents.TabIndex = 2;
            this.listAdminEvents.UseCompatibleStateImageBehavior = false;
            this.listAdminEvents.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Статистика";
            this.columnHeader1.Width = 752;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listWeaponEvents);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(776, 437);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Статистика оружия";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listWeaponEvents
            // 
            this.listWeaponEvents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listWeaponEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listWeaponEvents.FullRowSelect = true;
            this.listWeaponEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listWeaponEvents.Location = new System.Drawing.Point(0, 0);
            this.listWeaponEvents.Name = "listWeaponEvents";
            this.listWeaponEvents.ShowItemToolTips = true;
            this.listWeaponEvents.Size = new System.Drawing.Size(776, 437);
            this.listWeaponEvents.TabIndex = 3;
            this.listWeaponEvents.UseCompatibleStateImageBehavior = false;
            this.listWeaponEvents.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Статистика";
            this.columnHeader2.Width = 752;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.listChatHistory);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(776, 437);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "История чата";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // listChatHistory
            // 
            this.listChatHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listChatHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listChatHistory.FullRowSelect = true;
            this.listChatHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listChatHistory.Location = new System.Drawing.Point(0, 0);
            this.listChatHistory.Name = "listChatHistory";
            this.listChatHistory.ShowItemToolTips = true;
            this.listChatHistory.Size = new System.Drawing.Size(776, 437);
            this.listChatHistory.TabIndex = 4;
            this.listChatHistory.UseCompatibleStateImageBehavior = false;
            this.listChatHistory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Статистика";
            this.columnHeader3.Width = 756;
            // 
            // ThreadEventsType
            // 
            this.ThreadEventsType.WorkerSupportsCancellation = true;
            this.ThreadEventsType.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ThreadEventsType_DoWork);
            this.ThreadEventsType.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ThreadEventsType_RunWorkerCompleted);
            // 
            // btnExit
            // 
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnExit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(760, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(15, 15);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "X";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // ServerEvents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(778, 485);
            this.ControlBox = false;
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.ListEvent);
            this.Controls.Add(this.GUIEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ServerEvents";
            this.ListEvent.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label GUIEvents;
        private System.Windows.Forms.TabControl ListEvent;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listEvents;
        private System.Windows.Forms.ColumnHeader List1;
        private System.Windows.Forms.ColumnHeader List2;
        private System.ComponentModel.BackgroundWorker ThreadEventsType;
        private System.Windows.Forms.Label btnExit;
        private System.Windows.Forms.ListView listEvents2;
        private System.Windows.Forms.ColumnHeader Header1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView listAdminEvents;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListView listWeaponEvents;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ListView listChatHistory;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}