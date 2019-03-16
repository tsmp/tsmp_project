namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    partial class NetworkMonitor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkMonitor));
            this.btnExit = new System.Windows.Forms.Label();
            this.GUI = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.img_rules = new System.Windows.Forms.PictureBox();
            this.GUI_ACTIVATE_INFORMER = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MonitoringActivate = new System.Windows.Forms.CheckBox();
            this.btnUnblockAllTraffic = new System.Windows.Forms.Button();
            this.btnDelFilters = new System.Windows.Forms.Button();
            this.btnBlockAllTraffic = new System.Windows.Forms.Button();
            this.NetworkDeleteFilterAtCloseForm = new System.Windows.Forms.CheckBox();
            this.MaxCurrentPacketReceived = new System.Windows.Forms.NumericUpDown();
            this.MaxCurrentBlockIP = new System.Windows.Forms.NumericUpDown();
            this.MaxCurrentConnections = new System.Windows.Forms.NumericUpDown();
            this.NetworkProtectionPacketsCounter = new System.Windows.Forms.CheckBox();
            this.NetworkProtectionBlockSrvcs = new System.Windows.Forms.CheckBox();
            this.NetworkBlockedActivate = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnDelFilterInBlockList = new System.Windows.Forms.LinkLabel();
            this.UI_VALUE_COUNT = new System.Windows.Forms.Label();
            this.HistoryBlocked = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnDelFilter = new System.Windows.Forms.LinkLabel();
            this.UI_IPSEC_INFO = new System.Windows.Forms.Label();
            this.IPSecBlockList = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ui_Info_list = new System.Windows.Forms.Label();
            this.WhiteIP = new System.Windows.Forms.RichTextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.Update = new System.Windows.Forms.Timer(this.components);
            this.ThreadUpdate = new System.ComponentModel.BackgroundWorker();
            this.btnMinimized = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img_rules)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCurrentPacketReceived)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCurrentBlockIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCurrentConnections)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnExit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(778, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(15, 15);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "X";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // GUI
            // 
            this.GUI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GUI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.GUI.Enabled = false;
            this.GUI.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GUI.ForeColor = System.Drawing.Color.Black;
            this.GUI.Location = new System.Drawing.Point(0, 0);
            this.GUI.Name = "GUI";
            this.GUI.Size = new System.Drawing.Size(797, 22);
            this.GUI.TabIndex = 6;
            this.GUI.Text = "S.E.R.V.E.R - Network Monitor";
            this.GUI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(1, 23);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(797, 506);
            this.tabControl1.TabIndex = 9;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.Controls.Add(this.img_rules);
            this.tabPage2.Controls.Add(this.GUI_ACTIVATE_INFORMER);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(789, 480);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Настройки программы";
            // 
            // img_rules
            // 
            this.img_rules.BackColor = System.Drawing.Color.LightCoral;
            this.img_rules.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("img_rules.BackgroundImage")));
            this.img_rules.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.img_rules.Location = new System.Drawing.Point(9, 2);
            this.img_rules.Name = "img_rules";
            this.img_rules.Size = new System.Drawing.Size(23, 22);
            this.img_rules.TabIndex = 67;
            this.img_rules.TabStop = false;
            // 
            // GUI_ACTIVATE_INFORMER
            // 
            this.GUI_ACTIVATE_INFORMER.BackColor = System.Drawing.Color.LightCoral;
            this.GUI_ACTIVATE_INFORMER.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GUI_ACTIVATE_INFORMER.ForeColor = System.Drawing.Color.White;
            this.GUI_ACTIVATE_INFORMER.Location = new System.Drawing.Point(1, 2);
            this.GUI_ACTIVATE_INFORMER.Name = "GUI_ACTIVATE_INFORMER";
            this.GUI_ACTIVATE_INFORMER.Size = new System.Drawing.Size(788, 23);
            this.GUI_ACTIVATE_INFORMER.TabIndex = 66;
            this.GUI_ACTIVATE_INFORMER.Text = "Используя данный функционал программы, Вы несете ответственность за все выполняем" +
    "ые действия.";
            this.GUI_ACTIVATE_INFORMER.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MonitoringActivate);
            this.groupBox1.Controls.Add(this.btnUnblockAllTraffic);
            this.groupBox1.Controls.Add(this.btnDelFilters);
            this.groupBox1.Controls.Add(this.btnBlockAllTraffic);
            this.groupBox1.Controls.Add(this.NetworkDeleteFilterAtCloseForm);
            this.groupBox1.Controls.Add(this.MaxCurrentPacketReceived);
            this.groupBox1.Controls.Add(this.MaxCurrentBlockIP);
            this.groupBox1.Controls.Add(this.MaxCurrentConnections);
            this.groupBox1.Controls.Add(this.NetworkProtectionPacketsCounter);
            this.groupBox1.Controls.Add(this.NetworkProtectionBlockSrvcs);
            this.groupBox1.Controls.Add(this.NetworkBlockedActivate);
            this.groupBox1.Location = new System.Drawing.Point(178, 147);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(433, 138);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры блокировки соединений";
            // 
            // MonitoringActivate
            // 
            this.MonitoringActivate.Appearance = System.Windows.Forms.Appearance.Button;
            this.MonitoringActivate.BackColor = System.Drawing.Color.Coral;
            this.MonitoringActivate.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MonitoringActivate.Location = new System.Drawing.Point(4, 12);
            this.MonitoringActivate.Name = "MonitoringActivate";
            this.MonitoringActivate.Size = new System.Drawing.Size(425, 121);
            this.MonitoringActivate.TabIndex = 16;
            this.MonitoringActivate.Text = resources.GetString("MonitoringActivate.Text");
            this.MonitoringActivate.UseVisualStyleBackColor = false;
            this.MonitoringActivate.CheckedChanged += new System.EventHandler(this.MonitoringActivate_CheckedChanged);
            // 
            // btnUnblockAllTraffic
            // 
            this.btnUnblockAllTraffic.BackColor = System.Drawing.Color.LightGreen;
            this.btnUnblockAllTraffic.Enabled = false;
            this.btnUnblockAllTraffic.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUnblockAllTraffic.Location = new System.Drawing.Point(156, 107);
            this.btnUnblockAllTraffic.Name = "btnUnblockAllTraffic";
            this.btnUnblockAllTraffic.Size = new System.Drawing.Size(137, 23);
            this.btnUnblockAllTraffic.TabIndex = 20;
            this.btnUnblockAllTraffic.Text = "Разрешить трафик";
            this.btnUnblockAllTraffic.UseVisualStyleBackColor = false;
            this.btnUnblockAllTraffic.Click += new System.EventHandler(this.btnUnblockAllTraffic_Click);
            // 
            // btnDelFilters
            // 
            this.btnDelFilters.BackColor = System.Drawing.Color.PowderBlue;
            this.btnDelFilters.Enabled = false;
            this.btnDelFilters.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDelFilters.Location = new System.Drawing.Point(292, 107);
            this.btnDelFilters.Name = "btnDelFilters";
            this.btnDelFilters.Size = new System.Drawing.Size(132, 23);
            this.btnDelFilters.TabIndex = 18;
            this.btnDelFilters.Text = "Удалить все фильтры";
            this.btnDelFilters.UseVisualStyleBackColor = false;
            this.btnDelFilters.Click += new System.EventHandler(this.btnDelFilters_Click);
            // 
            // btnBlockAllTraffic
            // 
            this.btnBlockAllTraffic.BackColor = System.Drawing.Color.Gold;
            this.btnBlockAllTraffic.Enabled = false;
            this.btnBlockAllTraffic.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBlockAllTraffic.Location = new System.Drawing.Point(7, 107);
            this.btnBlockAllTraffic.Name = "btnBlockAllTraffic";
            this.btnBlockAllTraffic.Size = new System.Drawing.Size(150, 23);
            this.btnBlockAllTraffic.TabIndex = 19;
            this.btnBlockAllTraffic.Text = "Блокировать весь трафик";
            this.btnBlockAllTraffic.UseVisualStyleBackColor = false;
            this.btnBlockAllTraffic.Click += new System.EventHandler(this.btnBlockAllTraffic_Click);
            // 
            // NetworkDeleteFilterAtCloseForm
            // 
            this.NetworkDeleteFilterAtCloseForm.AutoSize = true;
            this.NetworkDeleteFilterAtCloseForm.Location = new System.Drawing.Point(8, 89);
            this.NetworkDeleteFilterAtCloseForm.Name = "NetworkDeleteFilterAtCloseForm";
            this.NetworkDeleteFilterAtCloseForm.Size = new System.Drawing.Size(379, 17);
            this.NetworkDeleteFilterAtCloseForm.TabIndex = 17;
            this.NetworkDeleteFilterAtCloseForm.Text = "При закрытий окна программы всегда удалять фильтры блокировок";
            this.NetworkDeleteFilterAtCloseForm.UseVisualStyleBackColor = true;
            // 
            // MaxCurrentPacketReceived
            // 
            this.MaxCurrentPacketReceived.Enabled = false;
            this.MaxCurrentPacketReceived.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MaxCurrentPacketReceived.Location = new System.Drawing.Point(319, 66);
            this.MaxCurrentPacketReceived.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.MaxCurrentPacketReceived.Name = "MaxCurrentPacketReceived";
            this.MaxCurrentPacketReceived.Size = new System.Drawing.Size(105, 20);
            this.MaxCurrentPacketReceived.TabIndex = 15;
            this.MaxCurrentPacketReceived.Value = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            // 
            // MaxCurrentBlockIP
            // 
            this.MaxCurrentBlockIP.Enabled = false;
            this.MaxCurrentBlockIP.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.MaxCurrentBlockIP.Location = new System.Drawing.Point(319, 44);
            this.MaxCurrentBlockIP.Maximum = new decimal(new int[] {
            -1539607552,
            11,
            0,
            0});
            this.MaxCurrentBlockIP.Name = "MaxCurrentBlockIP";
            this.MaxCurrentBlockIP.Size = new System.Drawing.Size(105, 20);
            this.MaxCurrentBlockIP.TabIndex = 13;
            this.MaxCurrentBlockIP.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // MaxCurrentConnections
            // 
            this.MaxCurrentConnections.Enabled = false;
            this.MaxCurrentConnections.Location = new System.Drawing.Point(319, 22);
            this.MaxCurrentConnections.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.MaxCurrentConnections.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.MaxCurrentConnections.Name = "MaxCurrentConnections";
            this.MaxCurrentConnections.Size = new System.Drawing.Size(105, 20);
            this.MaxCurrentConnections.TabIndex = 11;
            this.MaxCurrentConnections.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // NetworkProtectionPacketsCounter
            // 
            this.NetworkProtectionPacketsCounter.AutoSize = true;
            this.NetworkProtectionPacketsCounter.Enabled = false;
            this.NetworkProtectionPacketsCounter.Location = new System.Drawing.Point(8, 67);
            this.NetworkProtectionPacketsCounter.Name = "NetworkProtectionPacketsCounter";
            this.NetworkProtectionPacketsCounter.Size = new System.Drawing.Size(298, 17);
            this.NetworkProtectionPacketsCounter.TabIndex = 14;
            this.NetworkProtectionPacketsCounter.Text = "Максимальный предел приема пакетов/сек(rate) c IP";
            this.NetworkProtectionPacketsCounter.UseVisualStyleBackColor = true;
            // 
            // NetworkProtectionBlockSrvcs
            // 
            this.NetworkProtectionBlockSrvcs.AutoSize = true;
            this.NetworkProtectionBlockSrvcs.Enabled = false;
            this.NetworkProtectionBlockSrvcs.Location = new System.Drawing.Point(8, 23);
            this.NetworkProtectionBlockSrvcs.Name = "NetworkProtectionBlockSrvcs";
            this.NetworkProtectionBlockSrvcs.Size = new System.Drawing.Size(302, 17);
            this.NetworkProtectionBlockSrvcs.TabIndex = 10;
            this.NetworkProtectionBlockSrvcs.Text = "Максимальный порог установленных соединений с IP";
            this.NetworkProtectionBlockSrvcs.UseVisualStyleBackColor = true;
            this.NetworkProtectionBlockSrvcs.CheckedChanged += new System.EventHandler(this.NetworkProtectionBlockSrvcs_CheckedChanged);
            // 
            // NetworkBlockedActivate
            // 
            this.NetworkBlockedActivate.AutoSize = true;
            this.NetworkBlockedActivate.Enabled = false;
            this.NetworkBlockedActivate.Location = new System.Drawing.Point(8, 45);
            this.NetworkBlockedActivate.Name = "NetworkBlockedActivate";
            this.NetworkBlockedActivate.Size = new System.Drawing.Size(294, 17);
            this.NetworkBlockedActivate.TabIndex = 12;
            this.NetworkBlockedActivate.Text = "Ограничить количество блокировок (лимит адресов)";
            this.NetworkBlockedActivate.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.White;
            this.tabPage3.Controls.Add(this.btnDelFilterInBlockList);
            this.tabPage3.Controls.Add(this.UI_VALUE_COUNT);
            this.tabPage3.Controls.Add(this.HistoryBlocked);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(789, 480);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Текущие блокировки";
            // 
            // btnDelFilterInBlockList
            // 
            this.btnDelFilterInBlockList.AutoSize = true;
            this.btnDelFilterInBlockList.BackColor = System.Drawing.Color.PaleGreen;
            this.btnDelFilterInBlockList.Location = new System.Drawing.Point(663, 5);
            this.btnDelFilterInBlockList.Name = "btnDelFilterInBlockList";
            this.btnDelFilterInBlockList.Size = new System.Drawing.Size(121, 13);
            this.btnDelFilterInBlockList.TabIndex = 21;
            this.btnDelFilterInBlockList.TabStop = true;
            this.btnDelFilterInBlockList.Text = "Снять все блокировки";
            this.btnDelFilterInBlockList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnDelFilterInBlockList_LinkClicked);
            // 
            // UI_VALUE_COUNT
            // 
            this.UI_VALUE_COUNT.BackColor = System.Drawing.Color.PaleGreen;
            this.UI_VALUE_COUNT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UI_VALUE_COUNT.Location = new System.Drawing.Point(0, 0);
            this.UI_VALUE_COUNT.Name = "UI_VALUE_COUNT";
            this.UI_VALUE_COUNT.Size = new System.Drawing.Size(789, 23);
            this.UI_VALUE_COUNT.TabIndex = 10;
            this.UI_VALUE_COUNT.Text = "Network Information:";
            this.UI_VALUE_COUNT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HistoryBlocked
            // 
            this.HistoryBlocked.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.HistoryBlocked.FullRowSelect = true;
            this.HistoryBlocked.Location = new System.Drawing.Point(0, 24);
            this.HistoryBlocked.Name = "HistoryBlocked";
            this.HistoryBlocked.Size = new System.Drawing.Size(789, 456);
            this.HistoryBlocked.TabIndex = 9;
            this.HistoryBlocked.UseCompatibleStateImageBehavior = false;
            this.HistoryBlocked.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Remote IP";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Current connections count";
            this.columnHeader2.Width = 139;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Description";
            this.columnHeader3.Width = 373;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Time";
            this.columnHeader4.Width = 144;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.White;
            this.tabPage4.Controls.Add(this.btnDelFilter);
            this.tabPage4.Controls.Add(this.UI_IPSEC_INFO);
            this.tabPage4.Controls.Add(this.IPSecBlockList);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(789, 480);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Лист блокировок";
            // 
            // btnDelFilter
            // 
            this.btnDelFilter.AutoSize = true;
            this.btnDelFilter.BackColor = System.Drawing.Color.PaleGreen;
            this.btnDelFilter.Location = new System.Drawing.Point(663, 5);
            this.btnDelFilter.Name = "btnDelFilter";
            this.btnDelFilter.Size = new System.Drawing.Size(121, 13);
            this.btnDelFilter.TabIndex = 20;
            this.btnDelFilter.TabStop = true;
            this.btnDelFilter.Text = "Снять все блокировки";
            this.btnDelFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnDelFilter_LinkClicked);
            // 
            // UI_IPSEC_INFO
            // 
            this.UI_IPSEC_INFO.BackColor = System.Drawing.Color.PaleGreen;
            this.UI_IPSEC_INFO.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UI_IPSEC_INFO.Location = new System.Drawing.Point(0, 0);
            this.UI_IPSEC_INFO.Name = "UI_IPSEC_INFO";
            this.UI_IPSEC_INFO.Size = new System.Drawing.Size(789, 23);
            this.UI_IPSEC_INFO.TabIndex = 19;
            this.UI_IPSEC_INFO.Text = "Information Address in BlockList:";
            this.UI_IPSEC_INFO.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IPSecBlockList
            // 
            this.IPSecBlockList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.IPSecBlockList.FullRowSelect = true;
            this.IPSecBlockList.Location = new System.Drawing.Point(0, 24);
            this.IPSecBlockList.Name = "IPSecBlockList";
            this.IPSecBlockList.ShowItemToolTips = true;
            this.IPSecBlockList.Size = new System.Drawing.Size(789, 456);
            this.IPSecBlockList.TabIndex = 18;
            this.IPSecBlockList.UseCompatibleStateImageBehavior = false;
            this.IPSecBlockList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Remote IP";
            this.columnHeader5.Width = 764;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ui_Info_list);
            this.tabPage5.Controls.Add(this.WhiteIP);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(789, 480);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Список разрешенных адресов";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ui_Info_list
            // 
            this.ui_Info_list.BackColor = System.Drawing.Color.PaleGreen;
            this.ui_Info_list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ui_Info_list.Location = new System.Drawing.Point(0, 0);
            this.ui_Info_list.Name = "ui_Info_list";
            this.ui_Info_list.Size = new System.Drawing.Size(789, 23);
            this.ui_Info_list.TabIndex = 20;
            this.ui_Info_list.Text = "Список разрешенных адресов которые не будут блокироваться. Каждый новый адрес дол" +
    "жен начинаться с новой строки.";
            this.ui_Info_list.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WhiteIP
            // 
            this.WhiteIP.BackColor = System.Drawing.Color.White;
            this.WhiteIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WhiteIP.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WhiteIP.Location = new System.Drawing.Point(0, 26);
            this.WhiteIP.Name = "WhiteIP";
            this.WhiteIP.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.WhiteIP.Size = new System.Drawing.Size(789, 454);
            this.WhiteIP.TabIndex = 0;
            this.WhiteIP.Text = "";
            this.WhiteIP.TextChanged += new System.EventHandler(this.WhiteIP_TextChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipTitle = "Network Monitor";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "S.E.R.V.E.R - Network Monitor";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // Update
            // 
            this.Update.Enabled = true;
            this.Update.Interval = 1500;
            this.Update.Tick += new System.EventHandler(this.Update_Tick);
            // 
            // ThreadUpdate
            // 
            this.ThreadUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ThreadUpdate_DoWork);
            this.ThreadUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ThreadUpdate_RunWorkerCompleted);
            // 
            // btnMinimized
            // 
            this.btnMinimized.AutoSize = true;
            this.btnMinimized.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnMinimized.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMinimized.Location = new System.Drawing.Point(758, 3);
            this.btnMinimized.Name = "btnMinimized";
            this.btnMinimized.Size = new System.Drawing.Size(14, 15);
            this.btnMinimized.TabIndex = 17;
            this.btnMinimized.Text = "_";
            this.btnMinimized.Click += new System.EventHandler(this.btnMinimized_Click);
            // 
            // NetworkMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(797, 529);
            this.ControlBox = false;
            this.Controls.Add(this.btnMinimized);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.GUI);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetworkMonitor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.img_rules)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCurrentPacketReceived)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCurrentBlockIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxCurrentConnections)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label btnExit;
        private System.Windows.Forms.Label GUI;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown MaxCurrentConnections;
        private System.Windows.Forms.CheckBox NetworkProtectionBlockSrvcs;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer Update;
        private System.ComponentModel.BackgroundWorker ThreadUpdate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView HistoryBlocked;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.CheckBox NetworkBlockedActivate;
        private System.Windows.Forms.CheckBox MonitoringActivate;
        private System.Windows.Forms.NumericUpDown MaxCurrentPacketReceived;
        private System.Windows.Forms.CheckBox NetworkProtectionPacketsCounter;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label UI_IPSEC_INFO;
        private System.Windows.Forms.ListView IPSecBlockList;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.LinkLabel btnDelFilter;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label ui_Info_list;
        private System.Windows.Forms.RichTextBox WhiteIP;
        private System.Windows.Forms.CheckBox NetworkDeleteFilterAtCloseForm;
        private System.Windows.Forms.Button btnUnblockAllTraffic;
        private System.Windows.Forms.Button btnDelFilters;
        private System.Windows.Forms.Button btnBlockAllTraffic;
        private System.Windows.Forms.PictureBox img_rules;
        private System.Windows.Forms.Label GUI_ACTIVATE_INFORMER;
        private System.Windows.Forms.NumericUpDown MaxCurrentBlockIP;
        private System.Windows.Forms.LinkLabel btnDelFilterInBlockList;
        private System.Windows.Forms.Label UI_VALUE_COUNT;
        private System.Windows.Forms.Label btnMinimized;
    }
}