namespace App
{
	internal partial class Settings : Form
	{
		#region Fields

		private readonly CancellationToken _token;

		private const string _dubPattern = @"^[a-zA-Z\-]+$";

		#endregion

		public Settings(FeedConfig config, CancellationToken token)
		{
			_token = token;

			InitializeComponent();

			IntervalInput.Text = config.Interval.TotalSeconds.ToString();
			VisibilityInput.Items.AddRange(Enum.GetNames<Visibility>());
			VisibilityInput.SelectedItem = config.Visibility.ToString();
			FeedHostTypeInput.Items.AddRange(Enum.GetNames<FeedHostType>());
			FeedHostTypeInput.SelectedItem = config.FeedHost.ToString();
			MaxNotificationsInput.Maximum = FeedConfig.MaxNotificationsMax;
			MaxNotificationsInput.Value = config.MaxNotifications;
			ShowFirstRunInput.Checked = config.ShowFirstRun;
			DubsValues.Items.AddRange(config.Dubs.ToArray());
			NamesValues.Items.AddRange(config.Names.ToArray());

			AcceptButton = SaveButton;
		}

		#region Prompt Handlers

		private static void HandlePrompt(string name, string text, int maxLength, string? pattern, Action<string> callback)
		{
			using Prompt prompt = new(name, text, maxLength, pattern);

			if (prompt.ShowDialog() == DialogResult.OK)
			{
				callback(prompt.Value);
			}
		}

		private static void HandlePromptAdd(string name, int maxLength, ListBox list, string? pattern = default)
		{
			HandlePrompt(name, "add to", maxLength, pattern, value =>
			{
				if (!list.Items.OfType<string>().Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase)))
				{
					_ = list.Items.Add(value);
				}
			});
		}

		private static void HandlePromptRemove(string name, int maxLength, ListBox list, string? pattern = default)
		{
			HandlePrompt(name, "remove from", maxLength, pattern, value =>
			{
				if (list.Items.Contains(value))
				{
					list.Items.Remove(value);
				}
			});
		}

		#endregion

		#region Events

		#region Change

		private void MaxNotificationsInput_ValueChanged(object sender, EventArgs e)
		{
			MaxNotificationsLabel.Text = $"Max Notifications ({MaxNotificationsInput.Value})";
		}

		#endregion

		#region Click

		private async void SaveButton_Click(object sender, EventArgs e)
		{
			if (double.TryParse(IntervalInput.Text, out double interval) && interval >= FeedConfig.IntervalMin && Enum.TryParse(VisibilityInput.SelectedItem?.ToString(), out Visibility visibility) && Enum.TryParse(FeedHostTypeInput.SelectedItem?.ToString(), out FeedHostType feedHostType))
			{
				if (await ConfigManager.SaveAsync(new()
				{
					Interval = TimeSpan.FromSeconds(interval),
					MaxNotifications = MaxNotificationsInput.Value,
					Visibility = visibility,
					FeedHost = feedHostType,
					ShowFirstRun = ShowFirstRunInput.Checked,
					Dubs = DubsValues.Items.OfType<string>(),
					Names = NamesValues.Items.OfType<string>()
				}, _token))
				{
					_ = MessageBox.Show("Saved", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);

					Close();
				}
				else
				{
					_ = MessageBox.Show("Unable to save (You might have the config file open in another program)", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				_ = MessageBox.Show("Invalid Values", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void DubsAddButton_Click(object sender, EventArgs e)
		{
			HandlePromptAdd(nameof(FeedConfig.Dubs), 40, DubsValues, _dubPattern);
		}

		private void DubsRemoveButton_Click(object sender, EventArgs e)
		{
			HandlePromptRemove(nameof(FeedConfig.Dubs), 40, DubsValues, _dubPattern);
		}

		private void NamesAddButton_Click(object sender, EventArgs e)
		{
			HandlePromptAdd(nameof(FeedConfig.Names), 60, NamesValues);
		}

		private void NamesRemoveButton_Click(object sender, EventArgs e)
		{
			HandlePromptRemove(nameof(FeedConfig.Names), 60, NamesValues);
		}

		private async void OpenConfigButton_Click(object sender, EventArgs e)
		{
			await ConfigManager.OpenConfigAsync(_token);
		}

		#endregion

		#endregion
	}
}
