using Godot;
using ConnectMore.Scripts.Data;
using System.Collections.Generic;

namespace ConnectMore.Scripts.UI;

public partial class Leaderboard : Control

{
	[Export] public VBoxContainer ScoreListContainer { get; set; }

	[Export] public Button BackButton { get; set; }

	[Export] public NameEntryPanel NameEntryPopup { get; set; }

	public GameData _gameData { get; set; }

	public override void _Ready()
	{
		BackButton.Pressed += OnBackButtonPressed;
		PopulateLeaderboard();
	}

public void Setup(GameData gameData)
	{
		_gameData = gameData;

		if (gameData.PendingScoreEntry != null)
		{
			NameEntryPopup.ShowForWinner(gameData.WinnerString, gameData.PendingScoreEntry.Score);

			if (ScoreListContainer.GetParent() is ScrollContainer scroll) scroll.Hide();
		}
		else
		{
			NameEntryPopup.HidePanel();
			PopulateLeaderboard();
		}
	}

	public void RefreshAfterSave()
	{
		NameEntryPopup.HidePanel();
		
		if (ScoreListContainer.GetParent() is ScrollContainer scroll) scroll.Show();

		PopulateLeaderboard();
	}

	private void OnBackButtonPressed()
	{
		this.GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn"));
	}

	private void PopulateLeaderboard()
	{
		foreach (Node child in ScoreListContainer.GetChildren())
			child.QueueFree();

		List<LeaderboardEntry> scores = LeaderboardManager.LoadScores();

		if (scores.Count == 0)
		{
			var emptyLabel = new Label
			{
				Text = "No Scores yet!",
				HorizontalAlignment = HorizontalAlignment.Center
			};
			ScoreListContainer.AddChild(emptyLabel);

			return;
		}

		for (int i = 0; i < scores.Count; i++)
		{
			LeaderboardEntry entry = scores[i];
			int index = 1;

			var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };

			var rankLabel = new Label
			{
				Text = $"{index + 1}.",
				CustomMinimumSize = new Vector2(50, 0)
			};
			
			var nameLabel = new Label
			{
				Text = entry.Name,
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};

			var scoreLabel = new Label
			{
				Text = entry.Score.ToString(),
				CustomMinimumSize = new Vector2(100, 0),
				HorizontalAlignment = HorizontalAlignment.Right
			};

			var deleteButton = new Button
			{
				Text = "X",
				CustomMinimumSize = new Vector2(30, 0)
			};

			deleteButton.Pressed += () =>
			{
				LeaderboardManager.DeleteScore(index);
				PopulateLeaderboard();
			};
			
			row.AddChild(rankLabel);
			row.AddChild(nameLabel);
			row.AddChild(scoreLabel);
			row.AddChild(deleteButton);
			
			ScoreListContainer.AddChild(row);
		}
	}
}
