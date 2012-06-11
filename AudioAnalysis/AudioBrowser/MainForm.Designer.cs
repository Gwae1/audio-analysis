﻿namespace AudioBrowser
{
    using System.Drawing;
    using System.Windows.Forms;

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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageSourceFiles = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSourceFileAnalysisType = new System.Windows.Forms.ComboBox();
            this.btnAnalyseSelectedAudioFiles = new System.Windows.Forms.Button();
            this.dataGridViewFileList = new System.Windows.Forms.DataGridView();
            this.selectedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.fileNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.durationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mediaTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mediaFileItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectSourceDirectory = new System.Windows.Forms.Button();
            this.btnUpdateSourceFileList = new System.Windows.Forms.Button();
            this.tfSourceDirectory = new System.Windows.Forms.TextBox();
            this.tabPageOutputFiles = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxCSVFileAnalysisType = new System.Windows.Forms.ComboBox();
            this.btnUpdateOutputFileList = new System.Windows.Forms.Button();
            this.btnLoadSelectedCSVFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tfOutputDirectory = new System.Windows.Forms.TextBox();
            this.dataGridCSVfiles = new System.Windows.Forms.DataGridView();
            this.dataGridViewCheckBoxColumnSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumnFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CsvFileDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumnFileLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.csvFileItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnSelectOutputDirectory = new System.Windows.Forms.Button();
            this.tabPageDisplay = new System.Windows.Forms.TabPage();
            this.labelCSVHeaders = new System.Windows.Forms.Label();
            this.listBoxDisplayedTracks = new System.Windows.Forms.ListBox();
            this.panelDisplayImages = new System.Windows.Forms.Panel();
            this.panelDisplayImageAndTrackBar = new System.Windows.Forms.Panel();
            this.pictureBoxVisualIndex = new System.Windows.Forms.PictureBox();
            this.pictureBoxBarTrack = new System.Windows.Forms.PictureBox();
            this.panelDisplaySpectrogram = new System.Windows.Forms.Panel();
            this.pictureBoxSonogram = new System.Windows.Forms.PictureBox();
            this.checkBoxSonogramAnnotate = new System.Windows.Forms.CheckBox();
            this.checkBoxSonnogramNoiseReduce = new System.Windows.Forms.CheckBox();
            this.labelSonogramFileName = new System.Windows.Forms.Label();
            this.labelCursorValue = new System.Windows.Forms.Label();
            this.labelSonogramName = new System.Windows.Forms.Label();
            this.buttonRefreshSonogram = new System.Windows.Forms.Button();
            this.labelSourceFileName = new System.Windows.Forms.Label();
            this.buttonAudacityRun = new System.Windows.Forms.Button();
            this.textBoxCursorValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelSourceFileDurationInMinutes = new System.Windows.Forms.Label();
            this.textBoxCursorLocation = new System.Windows.Forms.TextBox();
            this.tabPageConsole = new System.Windows.Forms.TabPage();
            this.textBoxConsole = new System.Windows.Forms.TextBox();
            this.folderBrowserDialogChooseDir = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorkerUpdateSourceFileList = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdateCSVFileList = new System.ComponentModel.BackgroundWorker();
            this.tabControlMain.SuspendLayout();
            this.tabPageSourceFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFileList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediaFileItemBindingSource)).BeginInit();
            this.tabPageOutputFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCSVfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.csvFileItemBindingSource)).BeginInit();
            this.tabPageDisplay.SuspendLayout();
            this.panelDisplayImages.SuspendLayout();
            this.panelDisplayImageAndTrackBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVisualIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBarTrack)).BeginInit();
            this.panelDisplaySpectrogram.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSonogram)).BeginInit();
            this.tabPageConsole.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.tabPageSourceFiles);
            this.tabControlMain.Controls.Add(this.tabPageOutputFiles);
            this.tabControlMain.Controls.Add(this.tabPageDisplay);
            this.tabControlMain.Controls.Add(this.tabPageConsole);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1222, 633);
            this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageSourceFiles
            // 
            this.tabPageSourceFiles.Controls.Add(this.label4);
            this.tabPageSourceFiles.Controls.Add(this.comboBoxSourceFileAnalysisType);
            this.tabPageSourceFiles.Controls.Add(this.btnAnalyseSelectedAudioFiles);
            this.tabPageSourceFiles.Controls.Add(this.dataGridViewFileList);
            this.tabPageSourceFiles.Controls.Add(this.label1);
            this.tabPageSourceFiles.Controls.Add(this.btnSelectSourceDirectory);
            this.tabPageSourceFiles.Controls.Add(this.btnUpdateSourceFileList);
            this.tabPageSourceFiles.Controls.Add(this.tfSourceDirectory);
            this.tabPageSourceFiles.Location = new System.Drawing.Point(4, 22);
            this.tabPageSourceFiles.Name = "tabPageSourceFiles";
            this.tabPageSourceFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSourceFiles.Size = new System.Drawing.Size(1214, 607);
            this.tabPageSourceFiles.TabIndex = 2;
            this.tabPageSourceFiles.Text = "Source Audio Files";
            this.tabPageSourceFiles.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Analysis Type:";
            // 
            // comboBoxSourceFileAnalysisType
            // 
            this.comboBoxSourceFileAnalysisType.FormattingEnabled = true;
            this.comboBoxSourceFileAnalysisType.Location = new System.Drawing.Point(87, 6);
            this.comboBoxSourceFileAnalysisType.Name = "comboBoxSourceFileAnalysisType";
            this.comboBoxSourceFileAnalysisType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSourceFileAnalysisType.TabIndex = 14;
            // 
            // btnAnalyseSelectedAudioFiles
            // 
            this.btnAnalyseSelectedAudioFiles.Location = new System.Drawing.Point(840, 4);
            this.btnAnalyseSelectedAudioFiles.Name = "btnAnalyseSelectedAudioFiles";
            this.btnAnalyseSelectedAudioFiles.Size = new System.Drawing.Size(120, 23);
            this.btnAnalyseSelectedAudioFiles.TabIndex = 1;
            this.btnAnalyseSelectedAudioFiles.Text = "Analyse Audio File";
            this.btnAnalyseSelectedAudioFiles.UseVisualStyleBackColor = true;
            this.btnAnalyseSelectedAudioFiles.Click += new System.EventHandler(this.btnAnalyseSelectedAudioFiles_Click);
            // 
            // dataGridViewFileList
            // 
            this.dataGridViewFileList.AllowUserToAddRows = false;
            this.dataGridViewFileList.AllowUserToDeleteRows = false;
            this.dataGridViewFileList.AllowUserToResizeRows = false;
            this.dataGridViewFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewFileList.AutoGenerateColumns = false;
            this.dataGridViewFileList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewFileList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewFileList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFileList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.selectedDataGridViewCheckBoxColumn,
            this.fileNameDataGridViewTextBoxColumn,
            this.fileDateDataGridViewTextBoxColumn,
            this.durationDataGridViewTextBoxColumn,
            this.fileLengthDataGridViewTextBoxColumn,
            this.mediaTypeDataGridViewTextBoxColumn});
            this.dataGridViewFileList.DataSource = this.mediaFileItemBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewFileList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewFileList.Location = new System.Drawing.Point(6, 37);
            this.dataGridViewFileList.Name = "dataGridViewFileList";
            this.dataGridViewFileList.ReadOnly = true;
            this.dataGridViewFileList.Size = new System.Drawing.Size(1202, 564);
            this.dataGridViewFileList.TabIndex = 0;
            this.dataGridViewFileList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFileListSourceFileList_CellClick);
            this.dataGridViewFileList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFileListSourceFileList_CellContentClick);
            this.dataGridViewFileList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewFileList_CellFormatting);
            this.dataGridViewFileList.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewFileListSourceFileList_CellPainting);
            this.dataGridViewFileList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFileListSourceFileList_CellValueChanged);
            this.dataGridViewFileList.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewFileListSourceFileList_CurrentCellDirtyStateChanged);
            // 
            // selectedDataGridViewCheckBoxColumn
            // 
            this.selectedDataGridViewCheckBoxColumn.HeaderText = "";
            this.selectedDataGridViewCheckBoxColumn.Name = "selectedDataGridViewCheckBoxColumn";
            this.selectedDataGridViewCheckBoxColumn.ReadOnly = true;
            this.selectedDataGridViewCheckBoxColumn.Width = 5;
            // 
            // fileNameDataGridViewTextBoxColumn
            // 
            this.fileNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.fileNameDataGridViewTextBoxColumn.DataPropertyName = "FileName";
            this.fileNameDataGridViewTextBoxColumn.HeaderText = "File Name";
            this.fileNameDataGridViewTextBoxColumn.Name = "fileNameDataGridViewTextBoxColumn";
            this.fileNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.fileNameDataGridViewTextBoxColumn.Width = 79;
            // 
            // fileDateDataGridViewTextBoxColumn
            // 
            this.fileDateDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.fileDateDataGridViewTextBoxColumn.DataPropertyName = "LastModified";
            this.fileDateDataGridViewTextBoxColumn.HeaderText = "Last Modified";
            this.fileDateDataGridViewTextBoxColumn.Name = "fileDateDataGridViewTextBoxColumn";
            this.fileDateDataGridViewTextBoxColumn.ReadOnly = true;
            this.fileDateDataGridViewTextBoxColumn.Width = 95;
            // 
            // durationDataGridViewTextBoxColumn
            // 
            this.durationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.durationDataGridViewTextBoxColumn.DataPropertyName = "Duration";
            this.durationDataGridViewTextBoxColumn.HeaderText = "Duration";
            this.durationDataGridViewTextBoxColumn.Name = "durationDataGridViewTextBoxColumn";
            this.durationDataGridViewTextBoxColumn.ReadOnly = true;
            this.durationDataGridViewTextBoxColumn.Width = 72;
            // 
            // fileLengthDataGridViewTextBoxColumn
            // 
            this.fileLengthDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.fileLengthDataGridViewTextBoxColumn.DataPropertyName = "FileLength";
            this.fileLengthDataGridViewTextBoxColumn.HeaderText = "File Length";
            this.fileLengthDataGridViewTextBoxColumn.Name = "fileLengthDataGridViewTextBoxColumn";
            this.fileLengthDataGridViewTextBoxColumn.ReadOnly = true;
            this.fileLengthDataGridViewTextBoxColumn.Width = 84;
            // 
            // mediaTypeDataGridViewTextBoxColumn
            // 
            this.mediaTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.mediaTypeDataGridViewTextBoxColumn.DataPropertyName = "MediaType";
            this.mediaTypeDataGridViewTextBoxColumn.HeaderText = "MediaType";
            this.mediaTypeDataGridViewTextBoxColumn.Name = "mediaTypeDataGridViewTextBoxColumn";
            this.mediaTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.mediaTypeDataGridViewTextBoxColumn.Width = 85;
            // 
            // mediaFileItemBindingSource
            // 
            this.mediaFileItemBindingSource.DataSource = typeof(AudioBrowser.MediaFileItem);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(214, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Source Folder:";
            // 
            // btnSelectSourceDirectory
            // 
            this.btnSelectSourceDirectory.CausesValidation = false;
            this.btnSelectSourceDirectory.Location = new System.Drawing.Point(574, 4);
            this.btnSelectSourceDirectory.Name = "btnSelectSourceDirectory";
            this.btnSelectSourceDirectory.Size = new System.Drawing.Size(120, 23);
            this.btnSelectSourceDirectory.TabIndex = 9;
            this.btnSelectSourceDirectory.Text = "Select Source Folder";
            this.btnSelectSourceDirectory.UseVisualStyleBackColor = true;
            this.btnSelectSourceDirectory.Click += new System.EventHandler(this.btnSelectSourceDirectory_Click);
            // 
            // btnUpdateSourceFileList
            // 
            this.btnUpdateSourceFileList.CausesValidation = false;
            this.btnUpdateSourceFileList.Location = new System.Drawing.Point(705, 4);
            this.btnUpdateSourceFileList.Name = "btnUpdateSourceFileList";
            this.btnUpdateSourceFileList.Size = new System.Drawing.Size(120, 23);
            this.btnUpdateSourceFileList.TabIndex = 10;
            this.btnUpdateSourceFileList.Text = "Update File List";
            this.btnUpdateSourceFileList.UseVisualStyleBackColor = true;
            this.btnUpdateSourceFileList.Click += new System.EventHandler(this.btnUpdateSourceFiles_Click);
            // 
            // tfSourceDirectory
            // 
            this.tfSourceDirectory.Location = new System.Drawing.Point(296, 6);
            this.tfSourceDirectory.Name = "tfSourceDirectory";
            this.tfSourceDirectory.Size = new System.Drawing.Size(272, 20);
            this.tfSourceDirectory.TabIndex = 8;
            // 
            // tabPageOutputFiles
            // 
            this.tabPageOutputFiles.Controls.Add(this.label5);
            this.tabPageOutputFiles.Controls.Add(this.comboBoxCSVFileAnalysisType);
            this.tabPageOutputFiles.Controls.Add(this.btnUpdateOutputFileList);
            this.tabPageOutputFiles.Controls.Add(this.btnLoadSelectedCSVFile);
            this.tabPageOutputFiles.Controls.Add(this.label2);
            this.tabPageOutputFiles.Controls.Add(this.tfOutputDirectory);
            this.tabPageOutputFiles.Controls.Add(this.dataGridCSVfiles);
            this.tabPageOutputFiles.Controls.Add(this.btnSelectOutputDirectory);
            this.tabPageOutputFiles.Location = new System.Drawing.Point(4, 22);
            this.tabPageOutputFiles.Name = "tabPageOutputFiles";
            this.tabPageOutputFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutputFiles.Size = new System.Drawing.Size(1214, 607);
            this.tabPageOutputFiles.TabIndex = 3;
            this.tabPageOutputFiles.Text = "Output CSV Files";
            this.tabPageOutputFiles.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Analysis Type:";
            // 
            // comboBoxCSVFileAnalysisType
            // 
            this.comboBoxCSVFileAnalysisType.FormattingEnabled = true;
            this.comboBoxCSVFileAnalysisType.Location = new System.Drawing.Point(89, 7);
            this.comboBoxCSVFileAnalysisType.Name = "comboBoxCSVFileAnalysisType";
            this.comboBoxCSVFileAnalysisType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCSVFileAnalysisType.TabIndex = 16;
            // 
            // btnUpdateOutputFileList
            // 
            this.btnUpdateOutputFileList.CausesValidation = false;
            this.btnUpdateOutputFileList.Location = new System.Drawing.Point(705, 4);
            this.btnUpdateOutputFileList.Name = "btnUpdateOutputFileList";
            this.btnUpdateOutputFileList.Size = new System.Drawing.Size(120, 23);
            this.btnUpdateOutputFileList.TabIndex = 15;
            this.btnUpdateOutputFileList.Text = "Update File List";
            this.btnUpdateOutputFileList.UseVisualStyleBackColor = true;
            this.btnUpdateOutputFileList.Click += new System.EventHandler(this.btnUpdateCSVFileList_Click);
            // 
            // btnLoadSelectedCSVFile
            // 
            this.btnLoadSelectedCSVFile.Location = new System.Drawing.Point(840, 5);
            this.btnLoadSelectedCSVFile.Name = "btnLoadSelectedCSVFile";
            this.btnLoadSelectedCSVFile.Size = new System.Drawing.Size(120, 23);
            this.btnLoadSelectedCSVFile.TabIndex = 2;
            this.btnLoadSelectedCSVFile.Text = "Load CSV File";
            this.btnLoadSelectedCSVFile.UseVisualStyleBackColor = true;
            this.btnLoadSelectedCSVFile.Click += new System.EventHandler(this.btnLoadVisualIndexAllSelected_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(216, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Output Folder:";
            // 
            // tfOutputDirectory
            // 
            this.tfOutputDirectory.Location = new System.Drawing.Point(296, 7);
            this.tfOutputDirectory.Name = "tfOutputDirectory";
            this.tfOutputDirectory.Size = new System.Drawing.Size(272, 20);
            this.tfOutputDirectory.TabIndex = 11;
            // 
            // dataGridCSVfiles
            // 
            this.dataGridCSVfiles.AllowUserToAddRows = false;
            this.dataGridCSVfiles.AllowUserToDeleteRows = false;
            this.dataGridCSVfiles.AllowUserToResizeRows = false;
            this.dataGridCSVfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridCSVfiles.AutoGenerateColumns = false;
            this.dataGridCSVfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridCSVfiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridCSVfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCSVfiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewCheckBoxColumnSelected,
            this.dataGridViewTextBoxColumnFileName,
            this.CsvFileDate,
            this.dataGridViewTextBoxColumnFileLength});
            this.dataGridCSVfiles.DataSource = this.csvFileItemBindingSource;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridCSVfiles.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridCSVfiles.Location = new System.Drawing.Point(6, 36);
            this.dataGridCSVfiles.MultiSelect = false;
            this.dataGridCSVfiles.Name = "dataGridCSVfiles";
            this.dataGridCSVfiles.Size = new System.Drawing.Size(1202, 565);
            this.dataGridCSVfiles.TabIndex = 1;
            this.dataGridCSVfiles.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFileListCSVFileList_CellClick);
            this.dataGridCSVfiles.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFileListCSVFileList_CellContentClick);
            this.dataGridCSVfiles.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridCSVfiles_CellFormatting);
            this.dataGridCSVfiles.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewFileListCSVFileList_CellPainting);
            this.dataGridCSVfiles.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFileListCSVFileList_CellValueChanged);
            this.dataGridCSVfiles.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewFileListCSVFileList_CurrentCellDirtyStateChanged);
            // 
            // dataGridViewCheckBoxColumnSelected
            // 
            this.dataGridViewCheckBoxColumnSelected.HeaderText = "";
            this.dataGridViewCheckBoxColumnSelected.Name = "dataGridViewCheckBoxColumnSelected";
            this.dataGridViewCheckBoxColumnSelected.ReadOnly = true;
            this.dataGridViewCheckBoxColumnSelected.Width = 5;
            // 
            // dataGridViewTextBoxColumnFileName
            // 
            this.dataGridViewTextBoxColumnFileName.DataPropertyName = "FileName";
            this.dataGridViewTextBoxColumnFileName.HeaderText = "FileName";
            this.dataGridViewTextBoxColumnFileName.Name = "dataGridViewTextBoxColumnFileName";
            this.dataGridViewTextBoxColumnFileName.ReadOnly = true;
            this.dataGridViewTextBoxColumnFileName.Width = 76;
            // 
            // CsvFileDate
            // 
            this.CsvFileDate.DataPropertyName = "LastModified";
            this.CsvFileDate.HeaderText = "Last Modified";
            this.CsvFileDate.Name = "CsvFileDate";
            this.CsvFileDate.ReadOnly = true;
            this.CsvFileDate.Width = 95;
            // 
            // dataGridViewTextBoxColumnFileLength
            // 
            this.dataGridViewTextBoxColumnFileLength.DataPropertyName = "FileLength";
            this.dataGridViewTextBoxColumnFileLength.HeaderText = "FileLength";
            this.dataGridViewTextBoxColumnFileLength.Name = "dataGridViewTextBoxColumnFileLength";
            this.dataGridViewTextBoxColumnFileLength.ReadOnly = true;
            this.dataGridViewTextBoxColumnFileLength.Width = 81;
            // 
            // csvFileItemBindingSource
            // 
            this.csvFileItemBindingSource.DataSource = typeof(AudioBrowser.CsvFileItem);
            // 
            // btnSelectOutputDirectory
            // 
            this.btnSelectOutputDirectory.CausesValidation = false;
            this.btnSelectOutputDirectory.Location = new System.Drawing.Point(574, 5);
            this.btnSelectOutputDirectory.Name = "btnSelectOutputDirectory";
            this.btnSelectOutputDirectory.Size = new System.Drawing.Size(120, 23);
            this.btnSelectOutputDirectory.TabIndex = 12;
            this.btnSelectOutputDirectory.Text = "Select Output Folder";
            this.btnSelectOutputDirectory.UseVisualStyleBackColor = true;
            this.btnSelectOutputDirectory.Click += new System.EventHandler(this.btnSelectOutputDirectory_Click);
            // 
            // tabPageDisplay
            // 
            this.tabPageDisplay.Controls.Add(this.labelCSVHeaders);
            this.tabPageDisplay.Controls.Add(this.listBoxDisplayedTracks);
            this.tabPageDisplay.Controls.Add(this.panelDisplayImages);
            this.tabPageDisplay.Controls.Add(this.checkBoxSonogramAnnotate);
            this.tabPageDisplay.Controls.Add(this.checkBoxSonnogramNoiseReduce);
            this.tabPageDisplay.Controls.Add(this.labelSonogramFileName);
            this.tabPageDisplay.Controls.Add(this.labelCursorValue);
            this.tabPageDisplay.Controls.Add(this.labelSonogramName);
            this.tabPageDisplay.Controls.Add(this.buttonRefreshSonogram);
            this.tabPageDisplay.Controls.Add(this.labelSourceFileName);
            this.tabPageDisplay.Controls.Add(this.buttonAudacityRun);
            this.tabPageDisplay.Controls.Add(this.textBoxCursorValue);
            this.tabPageDisplay.Controls.Add(this.label3);
            this.tabPageDisplay.Controls.Add(this.labelSourceFileDurationInMinutes);
            this.tabPageDisplay.Controls.Add(this.textBoxCursorLocation);
            this.tabPageDisplay.Location = new System.Drawing.Point(4, 22);
            this.tabPageDisplay.Name = "tabPageDisplay";
            this.tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDisplay.Size = new System.Drawing.Size(1214, 607);
            this.tabPageDisplay.TabIndex = 0;
            this.tabPageDisplay.Text = "Display";
            this.tabPageDisplay.UseVisualStyleBackColor = true;
            // 
            // labelCSVHeaders
            // 
            this.labelCSVHeaders.AutoSize = true;
            this.labelCSVHeaders.Location = new System.Drawing.Point(3, 150);
            this.labelCSVHeaders.Name = "labelCSVHeaders";
            this.labelCSVHeaders.Size = new System.Drawing.Size(101, 13);
            this.labelCSVHeaders.TabIndex = 11;
            this.labelCSVHeaders.Text = "Headers in CSV File";
            //this.labelCSVHeaders.Click += new System.EventHandler(this.label6_Click);
            // 
            // listBoxDisplayedTracks
            // 
            this.listBoxDisplayedTracks.FormattingEnabled = true;
            this.listBoxDisplayedTracks.Location = new System.Drawing.Point(8, 166);
            this.listBoxDisplayedTracks.Name = "listBoxDisplayedTracks";
            this.listBoxDisplayedTracks.Size = new System.Drawing.Size(153, 186);
            this.listBoxDisplayedTracks.TabIndex = 10;
            // 
            // panelDisplayImages
            // 
            this.panelDisplayImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisplayImages.AutoScroll = true;
            this.panelDisplayImages.Controls.Add(this.panelDisplayImageAndTrackBar);
            this.panelDisplayImages.Controls.Add(this.panelDisplaySpectrogram);
            this.panelDisplayImages.Location = new System.Drawing.Point(178, 6);
            this.panelDisplayImages.Name = "panelDisplayImages";
            this.panelDisplayImages.Size = new System.Drawing.Size(1031, 581);
            this.panelDisplayImages.TabIndex = 9;
            // 
            // panelDisplayImageAndTrackBar
            // 
            this.panelDisplayImageAndTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisplayImageAndTrackBar.AutoScroll = true;
            this.panelDisplayImageAndTrackBar.Controls.Add(this.pictureBoxVisualIndex);
            this.panelDisplayImageAndTrackBar.Controls.Add(this.pictureBoxBarTrack);
            this.panelDisplayImageAndTrackBar.Location = new System.Drawing.Point(3, 3);
            this.panelDisplayImageAndTrackBar.Name = "panelDisplayImageAndTrackBar";
            this.panelDisplayImageAndTrackBar.Size = new System.Drawing.Size(821, 393);
            this.panelDisplayImageAndTrackBar.TabIndex = 7;
            // 
            // pictureBoxVisualIndex
            // 
            this.pictureBoxVisualIndex.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVisualIndex.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxVisualIndex.Name = "pictureBoxVisualIndex";
            this.pictureBoxVisualIndex.Size = new System.Drawing.Size(1600, 340);
            this.pictureBoxVisualIndex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxVisualIndex.TabIndex = 0;
            this.pictureBoxVisualIndex.TabStop = false;
            this.pictureBoxVisualIndex.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVisualIndex_MouseClick);
            this.pictureBoxVisualIndex.MouseHover += new System.EventHandler(this.pictureBoxVisualIndex_MouseHover);
            this.pictureBoxVisualIndex.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVisualIndex_MouseMove);
            // 
            // pictureBoxBarTrack
            // 
            this.pictureBoxBarTrack.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.pictureBoxBarTrack.Location = new System.Drawing.Point(3, 349);
            this.pictureBoxBarTrack.Name = "pictureBoxBarTrack";
            this.pictureBoxBarTrack.Size = new System.Drawing.Size(1583, 24);
            this.pictureBoxBarTrack.TabIndex = 3;
            this.pictureBoxBarTrack.TabStop = false;
            // 
            // panelDisplaySpectrogram
            // 
            this.panelDisplaySpectrogram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisplaySpectrogram.AutoScroll = true;
            this.panelDisplaySpectrogram.Controls.Add(this.pictureBoxSonogram);
            this.panelDisplaySpectrogram.Location = new System.Drawing.Point(3, 402);
            this.panelDisplaySpectrogram.Name = "panelDisplaySpectrogram";
            this.panelDisplaySpectrogram.Size = new System.Drawing.Size(821, 288);
            this.panelDisplaySpectrogram.TabIndex = 8;
            // 
            // pictureBoxSonogram
            // 
            this.pictureBoxSonogram.BackColor = System.Drawing.Color.DarkGray;
            this.pictureBoxSonogram.Location = new System.Drawing.Point(3, 0);
            this.pictureBoxSonogram.Name = "pictureBoxSonogram";
            this.pictureBoxSonogram.Size = new System.Drawing.Size(1584, 270);
            this.pictureBoxSonogram.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxSonogram.TabIndex = 2;
            this.pictureBoxSonogram.TabStop = false;
            // 
            // checkBoxSonogramAnnotate
            // 
            this.checkBoxSonogramAnnotate.AutoSize = true;
            this.checkBoxSonogramAnnotate.Location = new System.Drawing.Point(24, 463);
            this.checkBoxSonogramAnnotate.Name = "checkBoxSonogramAnnotate";
            this.checkBoxSonogramAnnotate.Size = new System.Drawing.Size(118, 17);
            this.checkBoxSonogramAnnotate.TabIndex = 5;
            this.checkBoxSonogramAnnotate.Text = "Annotate sonogram";
            this.checkBoxSonogramAnnotate.UseVisualStyleBackColor = true;
            // 
            // checkBoxSonnogramNoiseReduce
            // 
            this.checkBoxSonnogramNoiseReduce.AutoSize = true;
            this.checkBoxSonnogramNoiseReduce.Location = new System.Drawing.Point(24, 439);
            this.checkBoxSonnogramNoiseReduce.Name = "checkBoxSonnogramNoiseReduce";
            this.checkBoxSonnogramNoiseReduce.Size = new System.Drawing.Size(138, 17);
            this.checkBoxSonnogramNoiseReduce.TabIndex = 4;
            this.checkBoxSonnogramNoiseReduce.Text = "Noise reduce sonogram";
            this.checkBoxSonnogramNoiseReduce.UseVisualStyleBackColor = true;
            // 
            // labelSonogramFileName
            // 
            this.labelSonogramFileName.AutoSize = true;
            this.labelSonogramFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSonogramFileName.Location = new System.Drawing.Point(6, 400);
            this.labelSonogramFileName.Name = "labelSonogramFileName";
            this.labelSonogramFileName.Size = new System.Drawing.Size(116, 13);
            this.labelSonogramFileName.TabIndex = 3;
            this.labelSonogramFileName.Text = "sonogram file name";
            // 
            // labelCursorValue
            // 
            this.labelCursorValue.AutoSize = true;
            this.labelCursorValue.Location = new System.Drawing.Point(3, 96);
            this.labelCursorValue.Name = "labelCursorValue";
            this.labelCursorValue.Size = new System.Drawing.Size(67, 13);
            this.labelCursorValue.TabIndex = 6;
            this.labelCursorValue.Text = "Cursor Value";
            // 
            // labelSonogramName
            // 
            this.labelSonogramName.AutoSize = true;
            this.labelSonogramName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSonogramName.Location = new System.Drawing.Point(6, 384);
            this.labelSonogramName.Name = "labelSonogramName";
            this.labelSonogramName.Size = new System.Drawing.Size(102, 15);
            this.labelSonogramName.TabIndex = 2;
            this.labelSonogramName.Text = "Sonogram Name";
            // 
            // buttonRefreshSonogram
            // 
            this.buttonRefreshSonogram.Location = new System.Drawing.Point(24, 497);
            this.buttonRefreshSonogram.Name = "buttonRefreshSonogram";
            this.buttonRefreshSonogram.Size = new System.Drawing.Size(120, 28);
            this.buttonRefreshSonogram.TabIndex = 1;
            this.buttonRefreshSonogram.Text = "Refresh Sonogram";
            this.buttonRefreshSonogram.UseVisualStyleBackColor = true;
            this.buttonRefreshSonogram.Click += new System.EventHandler(this.buttonRefreshSonogram_Click);
            // 
            // labelSourceFileName
            // 
            this.labelSourceFileName.AutoSize = true;
            this.labelSourceFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSourceFileName.Location = new System.Drawing.Point(6, 3);
            this.labelSourceFileName.Name = "labelSourceFileName";
            this.labelSourceFileName.Size = new System.Drawing.Size(107, 13);
            this.labelSourceFileName.TabIndex = 3;
            this.labelSourceFileName.Text = "Source File Name";
            //this.labelSourceFileName.Click += new System.EventHandler(this.labelSourceFileName_Click);
            // 
            // buttonAudacityRun
            // 
            this.buttonAudacityRun.Location = new System.Drawing.Point(24, 548);
            this.buttonAudacityRun.Name = "buttonAudacityRun";
            this.buttonAudacityRun.Size = new System.Drawing.Size(120, 28);
            this.buttonAudacityRun.TabIndex = 0;
            this.buttonAudacityRun.Text = "Run Audacity";
            this.buttonAudacityRun.UseVisualStyleBackColor = true;
            this.buttonAudacityRun.Click += new System.EventHandler(this.buttonRunAudacity_Click);
            // 
            // textBoxCursorValue
            // 
            this.textBoxCursorValue.Location = new System.Drawing.Point(9, 112);
            this.textBoxCursorValue.Name = "textBoxCursorValue";
            this.textBoxCursorValue.Size = new System.Drawing.Size(160, 20);
            this.textBoxCursorValue.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Cursor Location";
            // 
            // labelSourceFileDurationInMinutes
            // 
            this.labelSourceFileDurationInMinutes.AutoSize = true;
            this.labelSourceFileDurationInMinutes.Location = new System.Drawing.Point(6, 20);
            this.labelSourceFileDurationInMinutes.Name = "labelSourceFileDurationInMinutes";
            this.labelSourceFileDurationInMinutes.Size = new System.Drawing.Size(111, 13);
            this.labelSourceFileDurationInMinutes.TabIndex = 4;
            this.labelSourceFileDurationInMinutes.Text = "File Duration (minutes)";
            // 
            // textBoxCursorLocation
            // 
            this.textBoxCursorLocation.Location = new System.Drawing.Point(9, 69);
            this.textBoxCursorLocation.Name = "textBoxCursorLocation";
            this.textBoxCursorLocation.Size = new System.Drawing.Size(160, 20);
            this.textBoxCursorLocation.TabIndex = 2;
            // 
            // tabPageConsole
            // 
            this.tabPageConsole.Controls.Add(this.textBoxConsole);
            this.tabPageConsole.Location = new System.Drawing.Point(4, 22);
            this.tabPageConsole.Name = "tabPageConsole";
            this.tabPageConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConsole.Size = new System.Drawing.Size(1214, 607);
            this.tabPageConsole.TabIndex = 1;
            this.tabPageConsole.Text = "Console";
            this.tabPageConsole.UseVisualStyleBackColor = true;
            // 
            // textBoxConsole
            // 
            this.textBoxConsole.BackColor = System.Drawing.Color.Black;
            this.textBoxConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxConsole.Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Bold);
            this.textBoxConsole.ForeColor = System.Drawing.Color.Lime;
            this.textBoxConsole.Location = new System.Drawing.Point(3, 3);
            this.textBoxConsole.Multiline = true;
            this.textBoxConsole.Name = "textBoxConsole";
            this.textBoxConsole.ReadOnly = true;
            this.textBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxConsole.Size = new System.Drawing.Size(1208, 601);
            this.textBoxConsole.TabIndex = 0;
            // 
            // folderBrowserDialogChooseDir
            // 
            this.folderBrowserDialogChooseDir.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialogChooseDir.ShowNewFolderButton = false;
            // 
            // backgroundWorkerUpdateSourceFileList
            // 
            //this.backgroundWorkerUpdateSourceFileList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdateSourceFileList_DoWork);
            // 
            // backgroundWorkerUpdateCSVFileList
            // 
            //this.backgroundWorkerUpdateCSVFileList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdateCSVFileList_DoWork);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 633);
            this.Controls.Add(this.tabControlMain);
            this.Location = new System.Drawing.Point(90, 90);
            this.MinimumSize = new System.Drawing.Size(830, 670);
            this.Name = "MainForm";
            this.Text = "Acoustic Environment Browser";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageSourceFiles.ResumeLayout(false);
            this.tabPageSourceFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFileList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediaFileItemBindingSource)).EndInit();
            this.tabPageOutputFiles.ResumeLayout(false);
            this.tabPageOutputFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCSVfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.csvFileItemBindingSource)).EndInit();
            this.tabPageDisplay.ResumeLayout(false);
            this.tabPageDisplay.PerformLayout();
            this.panelDisplayImages.ResumeLayout(false);
            this.panelDisplayImageAndTrackBar.ResumeLayout(false);
            this.panelDisplayImageAndTrackBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVisualIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBarTrack)).EndInit();
            this.panelDisplaySpectrogram.ResumeLayout(false);
            this.panelDisplaySpectrogram.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSonogram)).EndInit();
            this.tabPageConsole.ResumeLayout(false);
            this.tabPageConsole.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TabPage tabPageSourceFiles;
        private DataGridView dataGridViewFileList;
        private TabPage tabPageDisplay;
        private TabPage tabPageConsole;
        private TextBox textBoxConsole;
        private Button btnUpdateSourceFileList;
        private Button btnSelectSourceDirectory;
        private TextBox tfSourceDirectory;
        private FolderBrowserDialog folderBrowserDialogChooseDir;
        private BindingSource mediaFileItemBindingSource;
        private TextBox tfOutputDirectory;
        private Button btnSelectOutputDirectory;
        private Label label2;
        private Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdateSourceFileList;
        private Button btnAnalyseSelectedAudioFiles;
        private PictureBox pictureBoxVisualIndex;
        private TabPage tabPageOutputFiles;
        private Button btnUpdateOutputFileList;
        private Button btnLoadSelectedCSVFile;
        private DataGridView dataGridCSVfiles;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdateCSVFileList;
        private BindingSource csvFileItemBindingSource;
        private Label labelSourceFileDurationInMinutes;
        private Label labelSourceFileName;
        private TextBox textBoxCursorLocation;
        private Label label3;
        private Button buttonAudacityRun;
        private Label labelCursorValue;
        private TextBox textBoxCursorValue;
        private PictureBox pictureBoxBarTrack;
        private Button buttonRefreshSonogram;
        private CheckBox checkBoxSonogramAnnotate;
        private CheckBox checkBoxSonnogramNoiseReduce;
        private Label labelSonogramFileName;
        private Label labelSonogramName;
        public TabControl tabControlMain;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumnSelected;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumnFileName;
        private DataGridViewTextBoxColumn CsvFileDate;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumnFileLength;
        private Panel panelDisplayImages;
        private Panel panelDisplayImageAndTrackBar;
        private Panel panelDisplaySpectrogram;
        private PictureBox pictureBoxSonogram;
        private DataGridViewCheckBoxColumn selectedDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fileDateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn durationDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fileLengthDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn mediaTypeDataGridViewTextBoxColumn;
        private Label label4;
        private ComboBox comboBoxSourceFileAnalysisType;
        private Label label5;
        private ComboBox comboBoxCSVFileAnalysisType;
        private ListBox listBoxDisplayedTracks;
        private Label labelCSVHeaders;


    }
}

