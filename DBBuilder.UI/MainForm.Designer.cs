namespace DBBuilder.UI
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
			this.components = new System.ComponentModel.Container();
			this.connectButton = new System.Windows.Forms.Button();
			this.serverNameTextBox = new System.Windows.Forms.TextBox();
			this.serverNameLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.scriptButton = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.databaseComboBox = new System.Windows.Forms.ComboBox();
			this.scriptOptionsGroupBox = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.logicObjectsCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.logicDependencyFileNameTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.dataObjectsCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.dataDependencyFileNameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.scriptOptionsGroupBox.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// connectButton
			// 
			this.connectButton.Location = new System.Drawing.Point(369, 10);
			this.connectButton.Name = "connectButton";
			this.connectButton.Size = new System.Drawing.Size(75, 23);
			this.connectButton.TabIndex = 2;
			this.connectButton.Text = "Connect";
			this.connectButton.UseVisualStyleBackColor = true;
			this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
			// 
			// serverNameTextBox
			// 
			this.serverNameTextBox.Location = new System.Drawing.Point(90, 12);
			this.serverNameTextBox.Name = "serverNameTextBox";
			this.serverNameTextBox.Size = new System.Drawing.Size(273, 20);
			this.serverNameTextBox.TabIndex = 1;
			this.serverNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.serverNameTextBox_Validating);
			// 
			// serverNameLabel
			// 
			this.serverNameLabel.AutoSize = true;
			this.serverNameLabel.Location = new System.Drawing.Point(12, 15);
			this.serverNameLabel.Name = "serverNameLabel";
			this.serverNameLabel.Size = new System.Drawing.Size(67, 13);
			this.serverNameLabel.TabIndex = 0;
			this.serverNameLabel.Text = "Server name";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Database";
			// 
			// scriptButton
			// 
			this.scriptButton.Location = new System.Drawing.Point(369, 40);
			this.scriptButton.Name = "scriptButton";
			this.scriptButton.Size = new System.Drawing.Size(75, 23);
			this.scriptButton.TabIndex = 5;
			this.scriptButton.Text = "Script It";
			this.scriptButton.UseVisualStyleBackColor = true;
			this.scriptButton.Click += new System.EventHandler(this.scriptButton_Click);
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// databaseComboBox
			// 
			this.databaseComboBox.FormattingEnabled = true;
			this.databaseComboBox.Location = new System.Drawing.Point(90, 40);
			this.databaseComboBox.Name = "databaseComboBox";
			this.databaseComboBox.Size = new System.Drawing.Size(273, 21);
			this.databaseComboBox.TabIndex = 4;
			// 
			// scriptOptionsGroupBox
			// 
			this.scriptOptionsGroupBox.Controls.Add(this.groupBox2);
			this.scriptOptionsGroupBox.Controls.Add(this.groupBox1);
			this.scriptOptionsGroupBox.Location = new System.Drawing.Point(15, 67);
			this.scriptOptionsGroupBox.Name = "scriptOptionsGroupBox";
			this.scriptOptionsGroupBox.Size = new System.Drawing.Size(429, 236);
			this.scriptOptionsGroupBox.TabIndex = 6;
			this.scriptOptionsGroupBox.TabStop = false;
			this.scriptOptionsGroupBox.Text = " Scripting options ";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.logicObjectsCheckedListBox);
			this.groupBox2.Controls.Add(this.logicDependencyFileNameTextBox);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Location = new System.Drawing.Point(223, 19);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 211);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Logic objects";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 61);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "objects to include";
			// 
			// logicObjectsCheckedListBox
			// 
			this.logicObjectsCheckedListBox.FormattingEnabled = true;
			this.logicObjectsCheckedListBox.Location = new System.Drawing.Point(9, 77);
			this.logicObjectsCheckedListBox.Name = "logicObjectsCheckedListBox";
			this.logicObjectsCheckedListBox.Size = new System.Drawing.Size(182, 124);
			this.logicObjectsCheckedListBox.TabIndex = 6;
			// 
			// logicDependencyFileNameTextBox
			// 
			this.logicDependencyFileNameTextBox.Location = new System.Drawing.Point(9, 38);
			this.logicDependencyFileNameTextBox.Name = "logicDependencyFileNameTextBox";
			this.logicDependencyFileNameTextBox.Size = new System.Drawing.Size(182, 20);
			this.logicDependencyFileNameTextBox.TabIndex = 5;
			this.logicDependencyFileNameTextBox.Text = "logicobjects.dependencies.xml";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 22);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(111, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "dependency file name";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.dataObjectsCheckedListBox);
			this.groupBox1.Controls.Add(this.dataDependencyFileNameTextBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(6, 19);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(194, 211);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data objects";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "objects to include";
			// 
			// dataObjectsCheckedListBox
			// 
			this.dataObjectsCheckedListBox.FormattingEnabled = true;
			this.dataObjectsCheckedListBox.Location = new System.Drawing.Point(6, 77);
			this.dataObjectsCheckedListBox.Name = "dataObjectsCheckedListBox";
			this.dataObjectsCheckedListBox.Size = new System.Drawing.Size(182, 124);
			this.dataObjectsCheckedListBox.TabIndex = 2;
			// 
			// dataDependencyFileNameTextBox
			// 
			this.dataDependencyFileNameTextBox.Location = new System.Drawing.Point(6, 38);
			this.dataDependencyFileNameTextBox.Name = "dataDependencyFileNameTextBox";
			this.dataDependencyFileNameTextBox.Size = new System.Drawing.Size(182, 20);
			this.dataDependencyFileNameTextBox.TabIndex = 1;
			this.dataDependencyFileNameTextBox.Text = "dataobjects.dependency.xml";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "dependency file name";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 315);
			this.Controls.Add(this.scriptOptionsGroupBox);
			this.Controls.Add(this.databaseComboBox);
			this.Controls.Add(this.scriptButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.serverNameLabel);
			this.Controls.Add(this.serverNameTextBox);
			this.Controls.Add(this.connectButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.Text = "Database scripting";
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.scriptOptionsGroupBox.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button connectButton;
		private System.Windows.Forms.TextBox serverNameTextBox;
		private System.Windows.Forms.Label serverNameLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button scriptButton;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.GroupBox scriptOptionsGroupBox;
		private System.Windows.Forms.ComboBox databaseComboBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox dataDependencyFileNameTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckedListBox logicObjectsCheckedListBox;
		private System.Windows.Forms.TextBox logicDependencyFileNameTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckedListBox dataObjectsCheckedListBox;
	}
}

