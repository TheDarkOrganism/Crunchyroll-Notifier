namespace App
{
	internal partial class Prompt : Form
	{
		#region Variables

		#region Private

		private readonly string? _pattern;

		#endregion

		#region Public

		public string Value { get; private set; } = string.Empty;

		#endregion

		#endregion

		public Prompt(string name, string text, int maxLength, string? pattern)
		{
			_pattern = pattern;

			InitializeComponent();

			Text = $"{name} Prompt";
			InputLabel.Text = $"Enter a value to {text} {name}:";
			TextInput.MaxLength = maxLength;

			AcceptButton = OkButton;
		}

		#region Events

		private void OkButton_Click(object sender, EventArgs e)
		{
			string value = TextInput.Text;

			if (value.Any())
			{
				if (_pattern is null || Regex.IsMatch(value, _pattern))
				{
					Value = value;

					DialogResult = DialogResult.OK;

					Close();
				}
				else
				{
					_ = MessageBox.Show("The format must be valid", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			else
			{
				_ = MessageBox.Show("The value cannot be empty", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion
	}
}
