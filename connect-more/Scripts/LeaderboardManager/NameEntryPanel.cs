using Godot;
using ConnectMore.Scripts.Data;

namespace ConnectMore.Scripts.UI
{
	public partial class NameEntryPanel : PanelContainer
	{
		[Export] public LineEdit NameLineEdit { get; set; }
		[Export] public Button SaveButton { get; set; }
		[Export] public Label TitleLabel { get; set; }
		
		private int pendingScore;
		private string pendingWinnerText;

		public override void _Ready()
		{
			Hide();

			if (SaveButton != null) SaveButton.Pressed += OnSavePressed;
		}

		public void ShowForWinner(string winnerString, int score)
		{
			pendingWinnerText = winnerString;
			pendingScore = score;

			TitleLabel.Text = !string.IsNullOrEmpty(winnerString)
				? $"{winnerString}\nEnter Your Name:"
				: "New High Score!\nEnter Your Name:";

			NameLineEdit.Text = "";
			Show();

			NameLineEdit.CallDeferred("grab_focus");
		}

		private void OnSavePressed()
		{
			string playerName = NameLineEdit.Text.Trim();

			if (string.IsNullOrEmpty(playerName))
			{
				if (!string.IsNullOrEmpty(pendingWinnerText)
				 && this.pendingWinnerText.Contains("Tie"))
					playerName = "Winner";
				else if (!string.IsNullOrEmpty(pendingWinnerText))
					playerName = pendingWinnerText.Split(' ')[0];
				else
					playerName = "PLayer";
			}
			
			LeaderboardEntry entry = new LeaderboardEntry(playerName, pendingScore);
			LeaderboardManager.AddScore(entry);

			pendingWinnerText = null;
			pendingScore = 0;

			if (this.GetParent() != null)
			{
				this.GetParent().CallDeferred("RefreshAfterSave");
			}
			else
			{
				Hide();
			}
		}

		public void HidePanel()
		{
			pendingWinnerText = null;
			pendingScore = 0;
			this.Hide();
		}
	}
}
