namespace App
{
	internal partial class Settings
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
			SaveButton = new Button();
			IntervalLabel = new Label();
			IntervalInput = new TextBox();
			VisibilityLabel = new Label();
			VisibilityInput = new ComboBox();
			FeedHostTypeLabel = new Label();
			FeedHostTypeInput = new ComboBox();
			MaxNotificationsLabel = new Label();
			ShowFirstRunLabel = new Label();
			ShowFirstRunInput = new CheckBox();
			DubsValues = new ListBox();
			DubsLabel = new Label();
			DubsRemoveButton = new Button();
			DubsAddButton = new Button();
			NamesLabel = new Label();
			NamesValues = new ListBox();
			NamesRemoveButton = new Button();
			NamesAddButton = new Button();
			MaxNotificationsInput = new TrackBar();
			OpenConfigButton = new Button();
			((System.ComponentModel.ISupportInitialize)MaxNotificationsInput).BeginInit();
			SuspendLayout();
			// 
			// SaveButton
			// 
			SaveButton.Location = new Point(196, 519);
			SaveButton.Margin = new Padding(3, 4, 3, 4);
			SaveButton.Name = "SaveButton";
			SaveButton.Size = new Size(94, 29);
			SaveButton.TabIndex = 0;
			SaveButton.Text = "Save";
			SaveButton.UseVisualStyleBackColor = true;
			SaveButton.Click += SaveButton_Click;
			// 
			// IntervalLabel
			// 
			IntervalLabel.AutoSize = true;
			IntervalLabel.Location = new Point(7, 12);
			IntervalLabel.Name = "IntervalLabel";
			IntervalLabel.Size = new Size(58, 20);
			IntervalLabel.TabIndex = 1;
			IntervalLabel.Text = "Interval";
			// 
			// IntervalInput
			// 
			IntervalInput.Location = new Point(176, 9);
			IntervalInput.Margin = new Padding(3, 4, 3, 4);
			IntervalInput.MaxLength = 6;
			IntervalInput.Name = "IntervalInput";
			IntervalInput.Size = new Size(114, 27);
			IntervalInput.TabIndex = 2;
			IntervalInput.TextAlign = HorizontalAlignment.Center;
			// 
			// VisibilityLabel
			// 
			VisibilityLabel.AutoSize = true;
			VisibilityLabel.Location = new Point(7, 47);
			VisibilityLabel.Name = "VisibilityLabel";
			VisibilityLabel.Size = new Size(65, 20);
			VisibilityLabel.TabIndex = 3;
			VisibilityLabel.Text = "Visibility";
			// 
			// VisibilityInput
			// 
			VisibilityInput.FormattingEnabled = true;
			VisibilityInput.Location = new Point(152, 44);
			VisibilityInput.Margin = new Padding(3, 4, 3, 4);
			VisibilityInput.Name = "VisibilityInput";
			VisibilityInput.Size = new Size(138, 28);
			VisibilityInput.TabIndex = 4;
			// 
			// FeedHostTypeLabel
			// 
			FeedHostTypeLabel.AutoSize = true;
			FeedHostTypeLabel.Location = new Point(7, 83);
			FeedHostTypeLabel.Name = "FeedHostTypeLabel";
			FeedHostTypeLabel.Size = new Size(76, 20);
			FeedHostTypeLabel.TabIndex = 5;
			FeedHostTypeLabel.Text = "Feed Host";
			// 
			// FeedHostTypeInput
			// 
			FeedHostTypeInput.FormattingEnabled = true;
			FeedHostTypeInput.Location = new Point(152, 80);
			FeedHostTypeInput.Margin = new Padding(3, 4, 3, 4);
			FeedHostTypeInput.Name = "FeedHostTypeInput";
			FeedHostTypeInput.Size = new Size(138, 28);
			FeedHostTypeInput.TabIndex = 6;
			// 
			// MaxNotificationsLabel
			// 
			MaxNotificationsLabel.AutoSize = true;
			MaxNotificationsLabel.Location = new Point(7, 124);
			MaxNotificationsLabel.Name = "MaxNotificationsLabel";
			MaxNotificationsLabel.Size = new Size(126, 20);
			MaxNotificationsLabel.TabIndex = 7;
			MaxNotificationsLabel.Text = "Max Notifications";
			// 
			// ShowFirstRunLabel
			// 
			ShowFirstRunLabel.AutoSize = true;
			ShowFirstRunLabel.Location = new Point(7, 160);
			ShowFirstRunLabel.Name = "ShowFirstRunLabel";
			ShowFirstRunLabel.Size = new Size(105, 20);
			ShowFirstRunLabel.TabIndex = 8;
			ShowFirstRunLabel.Text = "Show First Run";
			// 
			// ShowFirstRunInput
			// 
			ShowFirstRunInput.AutoSize = true;
			ShowFirstRunInput.Location = new Point(152, 163);
			ShowFirstRunInput.Margin = new Padding(3, 4, 3, 4);
			ShowFirstRunInput.Name = "ShowFirstRunInput";
			ShowFirstRunInput.Size = new Size(18, 17);
			ShowFirstRunInput.TabIndex = 9;
			ShowFirstRunInput.UseVisualStyleBackColor = true;
			// 
			// DubsValues
			// 
			DubsValues.FormattingEnabled = true;
			DubsValues.HorizontalScrollbar = true;
			DubsValues.Location = new Point(7, 222);
			DubsValues.Margin = new Padding(3, 4, 3, 4);
			DubsValues.Name = "DubsValues";
			DubsValues.Size = new Size(283, 124);
			DubsValues.TabIndex = 10;
			// 
			// DubsLabel
			// 
			DubsLabel.AutoSize = true;
			DubsLabel.Location = new Point(7, 195);
			DubsLabel.Name = "DubsLabel";
			DubsLabel.Size = new Size(43, 20);
			DubsLabel.TabIndex = 11;
			DubsLabel.Text = "Dubs";
			// 
			// DubsRemoveButton
			// 
			DubsRemoveButton.Location = new Point(196, 186);
			DubsRemoveButton.Name = "DubsRemoveButton";
			DubsRemoveButton.Size = new Size(94, 29);
			DubsRemoveButton.TabIndex = 12;
			DubsRemoveButton.Text = "Remove";
			DubsRemoveButton.UseVisualStyleBackColor = true;
			DubsRemoveButton.Click += DubsRemoveButton_Click;
			// 
			// DubsAddButton
			// 
			DubsAddButton.Location = new Point(96, 187);
			DubsAddButton.Name = "DubsAddButton";
			DubsAddButton.Size = new Size(94, 29);
			DubsAddButton.TabIndex = 13;
			DubsAddButton.Text = "Add";
			DubsAddButton.UseVisualStyleBackColor = true;
			DubsAddButton.Click += DubsAddButton_Click;
			// 
			// NamesLabel
			// 
			NamesLabel.AutoSize = true;
			NamesLabel.Location = new Point(7, 362);
			NamesLabel.Name = "NamesLabel";
			NamesLabel.Size = new Size(55, 20);
			NamesLabel.TabIndex = 14;
			NamesLabel.Text = "Names";
			// 
			// NamesValues
			// 
			NamesValues.FormattingEnabled = true;
			NamesValues.HorizontalScrollbar = true;
			NamesValues.Location = new Point(7, 388);
			NamesValues.Name = "NamesValues";
			NamesValues.Size = new Size(283, 124);
			NamesValues.TabIndex = 15;
			// 
			// NamesRemoveButton
			// 
			NamesRemoveButton.Location = new Point(196, 353);
			NamesRemoveButton.Name = "NamesRemoveButton";
			NamesRemoveButton.Size = new Size(94, 29);
			NamesRemoveButton.TabIndex = 16;
			NamesRemoveButton.Text = "Remove";
			NamesRemoveButton.UseVisualStyleBackColor = true;
			NamesRemoveButton.Click += NamesRemoveButton_Click;
			// 
			// NamesAddButton
			// 
			NamesAddButton.Location = new Point(96, 353);
			NamesAddButton.Name = "NamesAddButton";
			NamesAddButton.Size = new Size(94, 29);
			NamesAddButton.TabIndex = 17;
			NamesAddButton.Text = "Add";
			NamesAddButton.UseVisualStyleBackColor = true;
			NamesAddButton.Click += NamesAddButton_Click;
			// 
			// MaxNotificationsInput
			// 
			MaxNotificationsInput.Location = new Point(176, 124);
			MaxNotificationsInput.Minimum = 1;
			MaxNotificationsInput.Name = "MaxNotificationsInput";
			MaxNotificationsInput.Size = new Size(114, 56);
			MaxNotificationsInput.TabIndex = 18;
			MaxNotificationsInput.Value = 1;
			MaxNotificationsInput.ValueChanged += MaxNotificationsInput_ValueChanged;
			// 
			// OpenConfigButton
			// 
			OpenConfigButton.Location = new Point(96, 519);
			OpenConfigButton.Name = "OpenConfigButton";
			OpenConfigButton.Size = new Size(94, 29);
			OpenConfigButton.TabIndex = 19;
			OpenConfigButton.Text = "Config";
			OpenConfigButton.UseVisualStyleBackColor = true;
			OpenConfigButton.Click += OpenConfigButton_Click;
			// 
			// Settings
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(299, 549);
			Controls.Add(OpenConfigButton);
			Controls.Add(MaxNotificationsInput);
			Controls.Add(NamesAddButton);
			Controls.Add(NamesRemoveButton);
			Controls.Add(NamesValues);
			Controls.Add(NamesLabel);
			Controls.Add(DubsAddButton);
			Controls.Add(DubsRemoveButton);
			Controls.Add(DubsLabel);
			Controls.Add(DubsValues);
			Controls.Add(ShowFirstRunInput);
			Controls.Add(ShowFirstRunLabel);
			Controls.Add(MaxNotificationsLabel);
			Controls.Add(VisibilityInput);
			Controls.Add(VisibilityLabel);
			Controls.Add(FeedHostTypeLabel);
			Controls.Add(FeedHostTypeInput);
			Controls.Add(IntervalInput);
			Controls.Add(IntervalLabel);
			Controls.Add(SaveButton);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(3, 4, 3, 4);
			MaximizeBox = false;
			Name = "Settings";
			SizeGripStyle = SizeGripStyle.Hide;
			Text = "Settings";
			((System.ComponentModel.ISupportInitialize)MaxNotificationsInput).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button SaveButton;
		private Label IntervalLabel;
		private TextBox IntervalInput;
		private Label VisibilityLabel;
		private ComboBox VisibilityInput;
		private Label FeedHostTypeLabel;
		private ComboBox FeedHostTypeInput;
		private Label MaxNotificationsLabel;
		private Label ShowFirstRunLabel;
		private CheckBox ShowFirstRunInput;
		private ListBox DubsValues;
		private Label DubsLabel;
		private Button DubsRemoveButton;
		private Button DubsAddButton;
		private Label NamesLabel;
		private ListBox NamesValues;
		private Button NamesRemoveButton;
		private Button NamesAddButton;
		private TrackBar MaxNotificationsInput;
		private Button OpenConfigButton;
	}
}