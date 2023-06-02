
namespace CPM
{
    partial class MainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveContainerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveContainerAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.readRAWImageFromCFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveRAWToCFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvFiles = new System.Windows.Forms.DataGridView();
            this.splitContainerVertical = new System.Windows.Forms.SplitContainer();
            this.lbImages = new System.Windows.Forms.ListBox();
            this.cbDirEntries = new System.Windows.Forms.ComboBox();
            this.lblDirEntries = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.btnBoot = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonInsert = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSaveFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNewFile = new System.Windows.Forms.ToolStripButton();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).BeginInit();
            this.splitContainerVertical.Panel1.SuspendLayout();
            this.splitContainerVertical.Panel2.SuspendLayout();
            this.splitContainerVertical.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(9, 9);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(90, 24);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveFilesToolStripMenuItem,
            this.toolStripSeparator5,
            this.insertToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveContainerToolStripMenuItem,
            this.saveContainerAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.readRAWImageFromCFToolStripMenuItem,
            this.saveRAWToCFToolStripMenuItem,
            this.toolStripMenuItem3,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.Image = global::CPM.Properties.Resources._new;
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.newFileToolStripMenuItem.Text = "&New";
            this.newFileToolStripMenuItem.Click += new System.EventHandler(this.New_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::CPM.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.Open_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::CPM.Properties.Resources.close;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.Close_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(240, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::CPM.Properties.Resources.save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.Save_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::CPM.Properties.Resources.save_as;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // saveFilesToolStripMenuItem
            // 
            this.saveFilesToolStripMenuItem.Image = global::CPM.Properties.Resources.save_binary;
            this.saveFilesToolStripMenuItem.Name = "saveFilesToolStripMenuItem";
            this.saveFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.saveFilesToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveFilesToolStripMenuItem.Text = "Save &Files";
            this.saveFilesToolStripMenuItem.Click += new System.EventHandler(this.SaveFiles_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(240, 6);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.Image = global::CPM.Properties.Resources.insert;
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.insertToolStripMenuItem.Text = "&Insert";
            this.insertToolStripMenuItem.Click += new System.EventHandler(this.Insert_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(240, 6);
            // 
            // saveContainerToolStripMenuItem
            // 
            this.saveContainerToolStripMenuItem.Name = "saveContainerToolStripMenuItem";
            this.saveContainerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.saveContainerToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveContainerToolStripMenuItem.Text = "Save &Container";
            this.saveContainerToolStripMenuItem.Click += new System.EventHandler(this.SaveContainer_Click);
            // 
            // saveContainerAsToolStripMenuItem
            // 
            this.saveContainerAsToolStripMenuItem.Name = "saveContainerAsToolStripMenuItem";
            this.saveContainerAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.saveContainerAsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveContainerAsToolStripMenuItem.Text = "Save Container As";
            this.saveContainerAsToolStripMenuItem.Click += new System.EventHandler(this.SaveContainerAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(240, 6);
            // 
            // readRAWImageFromCFToolStripMenuItem
            // 
            this.readRAWImageFromCFToolStripMenuItem.Name = "readRAWImageFromCFToolStripMenuItem";
            this.readRAWImageFromCFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.readRAWImageFromCFToolStripMenuItem.Text = "Read RAW image from CF";
            this.readRAWImageFromCFToolStripMenuItem.Click += new System.EventHandler(this.readRAWImageFromCF_Click);
            // 
            // saveRAWToCFToolStripMenuItem
            // 
            this.saveRAWToCFToolStripMenuItem.Name = "saveRAWToCFToolStripMenuItem";
            this.saveRAWToCFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveRAWToCFToolStripMenuItem.Text = "Save RAW image to CF";
            this.saveRAWToCFToolStripMenuItem.Click += new System.EventHandler(this.saveRAWToCF_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(240, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.Quit_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // viewHelpToolStripMenuItem
            // 
            this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
            this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.viewHelpToolStripMenuItem.Text = "View Help";
            this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.Help_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.About_Click);
            // 
            // dgvFiles
            // 
            this.dgvFiles.AllowUserToAddRows = false;
            this.dgvFiles.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dgvFiles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFiles.Location = new System.Drawing.Point(3, 5);
            this.dgvFiles.MultiSelect = false;
            this.dgvFiles.Name = "dgvFiles";
            this.dgvFiles.ReadOnly = true;
            this.dgvFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFiles.Size = new System.Drawing.Size(537, 511);
            this.dgvFiles.TabIndex = 19;
            this.dgvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvFiles_KeyDown);
            // 
            // splitContainerVertical
            // 
            this.splitContainerVertical.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerVertical.Location = new System.Drawing.Point(9, 37);
            this.splitContainerVertical.Name = "splitContainerVertical";
            // 
            // splitContainerVertical.Panel1
            // 
            this.splitContainerVertical.Panel1.Controls.Add(this.lbImages);
            // 
            // splitContainerVertical.Panel2
            // 
            this.splitContainerVertical.Panel2.Controls.Add(this.dgvFiles);
            this.splitContainerVertical.Size = new System.Drawing.Size(763, 519);
            this.splitContainerVertical.SplitterDistance = 216;
            this.splitContainerVertical.TabIndex = 21;
            // 
            // lbImages
            // 
            this.lbImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbImages.FormattingEnabled = true;
            this.lbImages.Location = new System.Drawing.Point(3, 5);
            this.lbImages.Name = "lbImages";
            this.lbImages.Size = new System.Drawing.Size(210, 511);
            this.lbImages.TabIndex = 20;
            this.lbImages.SelectedIndexChanged += new System.EventHandler(this.lbImages_SelectedIndexChanged);
            // 
            // cbDirEntries
            // 
            this.cbDirEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDirEntries.FormattingEnabled = true;
            this.cbDirEntries.Items.AddRange(new object[] {
            "512",
            "2048"});
            this.cbDirEntries.Location = new System.Drawing.Point(699, 10);
            this.cbDirEntries.Name = "cbDirEntries";
            this.cbDirEntries.Size = new System.Drawing.Size(70, 21);
            this.cbDirEntries.TabIndex = 22;
            // 
            // lblDirEntries
            // 
            this.lblDirEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDirEntries.AutoSize = true;
            this.lblDirEntries.Location = new System.Drawing.Point(606, 13);
            this.lblDirEntries.Name = "lblDirEntries";
            this.lblDirEntries.Size = new System.Drawing.Size(87, 13);
            this.lblDirEntries.TabIndex = 23;
            this.lblDirEntries.Text = "Directory Entries:";
            // 
            // lblSize
            // 
            this.lblSize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(229, 13);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(58, 13);
            this.lblSize.TabIndex = 24;
            this.lblSize.Text = "Size: 0 MB";
            // 
            // btnBoot
            // 
            this.btnBoot.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBoot.Location = new System.Drawing.Point(315, 8);
            this.btnBoot.Name = "btnBoot";
            this.btnBoot.Size = new System.Drawing.Size(75, 23);
            this.btnBoot.TabIndex = 25;
            this.btnBoot.Text = "BOOT";
            this.btnBoot.UseVisualStyleBackColor = true;
            this.btnBoot.Visible = false;
            this.btnBoot.Click += new System.EventHandler(this.btnBoot_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonInsert,
            this.toolStripSeparator4,
            this.toolStripButtonSaveFiles,
            this.toolStripButtonSaveAs,
            this.toolStripButtonSave,
            this.toolStripSeparator3,
            this.toolStripButtonClose,
            this.toolStripButtonOpen,
            this.toolStripButtonNewFile});
            this.toolStrip.Location = new System.Drawing.Point(396, 8);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip.Size = new System.Drawing.Size(207, 25);
            this.toolStrip.TabIndex = 26;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripButtonInsert
            // 
            this.toolStripButtonInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInsert.Image = global::CPM.Properties.Resources.insert;
            this.toolStripButtonInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInsert.Name = "toolStripButtonInsert";
            this.toolStripButtonInsert.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonInsert.ToolTipText = "Insert Files Into Image";
            this.toolStripButtonInsert.Click += new System.EventHandler(this.Insert_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSaveFiles
            // 
            this.toolStripButtonSaveFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveFiles.Image = global::CPM.Properties.Resources.save_binary;
            this.toolStripButtonSaveFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveFiles.Name = "toolStripButtonSaveFiles";
            this.toolStripButtonSaveFiles.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveFiles.ToolTipText = "Extract/Save Files";
            this.toolStripButtonSaveFiles.Click += new System.EventHandler(this.SaveFiles_Click);
            // 
            // toolStripButtonSaveAs
            // 
            this.toolStripButtonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveAs.Image = global::CPM.Properties.Resources.save_as;
            this.toolStripButtonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveAs.Name = "toolStripButtonSaveAs";
            this.toolStripButtonSaveAs.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveAs.ToolTipText = "Save Image As";
            this.toolStripButtonSaveAs.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = global::CPM.Properties.Resources.save;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.ToolTipText = "Save Image";
            this.toolStripButtonSave.Click += new System.EventHandler(this.Save_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonClose
            // 
            this.toolStripButtonClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClose.Image = global::CPM.Properties.Resources.close;
            this.toolStripButtonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClose.Name = "toolStripButtonClose";
            this.toolStripButtonClose.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClose.ToolTipText = "Close Image";
            this.toolStripButtonClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.Image = global::CPM.Properties.Resources.open;
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpen.ToolTipText = "Open Image/Container";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.Open_Click);
            // 
            // toolStripButtonNewFile
            // 
            this.toolStripButtonNewFile.BackColor = System.Drawing.Color.Transparent;
            this.toolStripButtonNewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNewFile.Image = global::CPM.Properties.Resources._new;
            this.toolStripButtonNewFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNewFile.Name = "toolStripButtonNewFile";
            this.toolStripButtonNewFile.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNewFile.Text = "New Image ";
            this.toolStripButtonNewFile.Click += new System.EventHandler(this.New_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnBoot);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lblDirEntries);
            this.Controls.Add(this.cbDirEntries);
            this.Controls.Add(this.splitContainerVertical);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RC2014 CPM Disk Explorer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
            this.splitContainerVertical.Panel1.ResumeLayout(false);
            this.splitContainerVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVertical)).EndInit();
            this.splitContainerVertical.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgvFiles;
        private System.Windows.Forms.SplitContainer splitContainerVertical;
        private System.Windows.Forms.ListBox lbImages;
        private System.Windows.Forms.ToolStripMenuItem saveFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbDirEntries;
        private System.Windows.Forms.Label lblDirEntries;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Button btnBoot;
        private System.Windows.Forms.ToolStripMenuItem saveContainerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveContainerAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveRAWToCFToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsert;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveAs;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonNewFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readRAWImageFromCFToolStripMenuItem;
    }
}

