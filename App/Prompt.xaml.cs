using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace App
{
	/// <summary>
	/// Interaction logic for Prompt.xaml
	/// </summary>
	public sealed partial class Prompt : Window
	{
		#region Variables

		#region Private

		private readonly RegexSelector? _pattern;

		private string? _value;

		#endregion

		//#region Public

		//public string Value { get; private set; } = string.Empty;

		//#endregion

		#endregion

		internal Prompt(string name, string text, int maxLength, RegexSelector? pattern)
		{
			_pattern = pattern;

			InitializeComponent();

			Title = $"{name} Prompt";
			PromptLabel.Content = $"Enter a value to {text} {name}:";
			ValueInput.MaxLength = maxLength;
		}

		#region Methods

		public bool ShowDialog([NotNullWhen(true)] out string? value)
		{
			value = ShowDialog() is true ? _value : null;

			return value is not null;
		}

		#endregion

		#region Events

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			string value = ValueInput.Text;

			if (value.Length > 0)
			{
				if (_pattern is null || _pattern().IsMatch(value))
				{
					_value = value;

					DialogResult = true;

					Close();
				}
				else
				{
					_ = MessageBox.Show("The format must be valid", string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			else
			{
				_ = MessageBox.Show("The value cannot be empty", string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion
	}
}
