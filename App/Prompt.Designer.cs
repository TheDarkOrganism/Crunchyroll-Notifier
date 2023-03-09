namespace App
{
	internal partial class Prompt
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Prompt));
			InputLabel = new Label();
			TextInput = new TextBox();
			OkButton = new Button();
			SuspendLayout();
			// 
			// InputLabel
			// 
			InputLabel.AutoSize = true;
			InputLabel.Location = new Point(11, 21);
			InputLabel.Name = "InputLabel";
			InputLabel.Size = new Size(0, 20);
			InputLabel.TabIndex = 0;
			// 
			// TextInput
			// 
			TextInput.Location = new Point(13, 59);
			TextInput.Name = "TextInput";
			TextInput.PlaceholderText = "Enter a value";
			TextInput.Size = new Size(313, 27);
			TextInput.TabIndex = 1;
			// 
			// OkButton
			// 
			OkButton.Location = new Point(331, 57);
			OkButton.Name = "OkButton";
			OkButton.Size = new Size(101, 31);
			OkButton.TabIndex = 2;
			OkButton.Text = "Ok";
			OkButton.UseVisualStyleBackColor = true;
			OkButton.Click += OkButton_Click;
			// 
			// Prompt
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(443, 113);
			Controls.Add(OkButton);
			Controls.Add(TextInput);
			Controls.Add(InputLabel);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			Name = "Prompt";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterScreen;
			TopMost = true;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label InputLabel;
		private TextBox TextInput;
		private Button OkButton;
	}
}