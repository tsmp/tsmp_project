namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    partial class ColorEvents
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorEvents));
            this.btnExit = new System.Windows.Forms.Label();
            this.GUIEvents = new System.Windows.Forms.Label();
            this.SrvColorEvents = new System.Windows.Forms.ListView();
            this.events_table = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ThreadEvents = new System.ComponentModel.BackgroundWorker();
            this.BaseMenu1 = new System.Windows.Forms.MenuStrip();
            this.GetPlayersHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.GetEventsHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.BaseMenu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnExit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(759, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(15, 15);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "X";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
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
            this.GUIEvents.TabIndex = 17;
            this.GUIEvents.Text = "S.E.R.V.E.R - История событий";
            this.GUIEvents.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SrvColorEvents
            // 
            this.SrvColorEvents.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.SrvColorEvents.BackColor = System.Drawing.SystemColors.MenuText;
            this.SrvColorEvents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SrvColorEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.events_table});
            this.SrvColorEvents.ForeColor = System.Drawing.Color.Black;
            this.SrvColorEvents.FullRowSelect = true;
            this.SrvColorEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.SrvColorEvents.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SrvColorEvents.Location = new System.Drawing.Point(0, 50);
            this.SrvColorEvents.Name = "SrvColorEvents";
            this.SrvColorEvents.ShowItemToolTips = true;
            this.SrvColorEvents.Size = new System.Drawing.Size(778, 435);
            this.SrvColorEvents.TabIndex = 179;
            this.SrvColorEvents.UseCompatibleStateImageBehavior = false;
            this.SrvColorEvents.View = System.Windows.Forms.View.Details;
            // 
            // events_table
            // 
            this.events_table.Text = "Тип события";
            this.events_table.Width = 757;
            // 
            // ThreadEvents
            // 
            this.ThreadEvents.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Thread_DoWork);
            this.ThreadEvents.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ThreadEvents_RunWorkerCompleted);
            // 
            // BaseMenu1
            // 
            this.BaseMenu1.AutoSize = false;
            this.BaseMenu1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.BaseMenu1.Dock = System.Windows.Forms.DockStyle.None;
            this.BaseMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GetPlayersHistory,
            this.GetEventsHistory});
            this.BaseMenu1.Location = new System.Drawing.Point(1, 22);
            this.BaseMenu1.Name = "BaseMenu1";
            this.BaseMenu1.Size = new System.Drawing.Size(776, 27);
            this.BaseMenu1.TabIndex = 180;
            this.BaseMenu1.Text = "BaseMenu1";
            // 
            // GetPlayersHistory
            // 
            this.GetPlayersHistory.Image = ((System.Drawing.Image)(resources.GetObject("GetPlayersHistory.Image")));
            this.GetPlayersHistory.Name = "GetPlayersHistory";
            this.GetPlayersHistory.Size = new System.Drawing.Size(248, 23);
            this.GetPlayersHistory.Text = "Вывести историю списка всех игроков";
            this.GetPlayersHistory.Click += new System.EventHandler(this.GetPlayersHistory_Click);
            // 
            // GetEventsHistory
            // 
            this.GetEventsHistory.Image = ((System.Drawing.Image)(resources.GetObject("GetEventsHistory.Image")));
            this.GetEventsHistory.Name = "GetEventsHistory";
            this.GetEventsHistory.Size = new System.Drawing.Size(195, 23);
            this.GetEventsHistory.Text = "Отобразить важные события";
            this.GetEventsHistory.Click += new System.EventHandler(this.GetEventsHistory_Click);
            // 
            // ColorEvents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(778, 485);
            this.ControlBox = false;
            this.Controls.Add(this.BaseMenu1);
            this.Controls.Add(this.SrvColorEvents);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.GUIEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorEvents";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BaseMenu1.ResumeLayout(false);
            this.BaseMenu1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label btnExit;
        public System.Windows.Forms.Label GUIEvents;
        private System.Windows.Forms.ListView SrvColorEvents;
        private System.Windows.Forms.ColumnHeader events_table;
        private System.ComponentModel.BackgroundWorker ThreadEvents;
        private System.Windows.Forms.MenuStrip BaseMenu1;
        private System.Windows.Forms.ToolStripMenuItem GetPlayersHistory;
        private System.Windows.Forms.ToolStripMenuItem GetEventsHistory;
    }
}