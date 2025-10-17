using Godot;
using ConnectMore.Scripts.Data;
using System.Collections.Generic;

namespace ConnectMore.Scripts.UI;

public partial class Leaderboard : Control

{
	[Export] public VBoxContainer ScoreListContainer { get; set; }
	[Export] public Button BackButton { get; set; }
	[Export] public PanelContainer NameEntryPopup { get; set; }
	[Export] public LineEdit NameLineEdit { get; set; }
	[Export] public Button SaveNameButton { get; set; }
	[Export] public Label PopupTitleLabel { get; set; }
	
	private GameData _gameData;
	
	public override void _Ready()
	{
		_gameData = GetNode<GameData>("/root/GameData");
		
		BackButton.Pressed += this.OnBackButtonPressed;
		SaveNameButton.Pressed += this.OnSaveNamePressed;
		
		if (_gameData.PendingScoreEntry != null)
		{
			ShowNameEntryPopup();
		}
		else
		{
			NameEntryPopup.Hide();
			PopulateLeaderboard();
		}
	}
	
	private void ShowNameEntryPopup()
	{
		if (!string.IsNullOrEmpty(_gameData.WinnerString))
		{
			PopupTitleLabel.Text = $"{_gameData.WinnerString}\nEnter Your Name:";
		}
		else
		{
			PopupTitleLabel.Text = "New High Score!\nEnter Your Name:";
		}
		
		NameEntryPopup.Show();
		NameLineEdit.GrabFocus();
		
		if (ScoreListContainer.GetParent() is ScrollContainer scroll)
		{
			scroll.Hide();
		}
	}
	
	private void OnSaveNamePressed()
	{
		GD.Print("Save button was pressed!");
		string playerName = NameLineEdit.Text.Trim();
		if (string.IsNullOrEmpty(playerName))
		{
			playerName = _gameData.WinnerString.Contains("Tie") ? "Winner" : _gameData.WinnerString.Split(' ')[0];
		}
		
		_gameData.PendingScoreEntry.Name = playerName;
		LeaderboardManager.AddScore(_gameData.PendingScoreEntry);
		
		_gameData.PendingScoreEntry = null;
		_gameData.WinnerString = null;
		
		NameEntryPopup.Hide();
		if (ScoreListContainer.GetParent() is ScrollContainer scroll)
		{
			scroll.Show();
		}
		PopulateLeaderboard();
	}
	
	private void OnBackButtonPressed()
	{
		GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://Scenes/MainMenu/MainMenu.tscn"));
	}
	private void PopulateLeaderboard()
	{
		GD.Print("Refreshing the leaderboard display");
		foreach (Node child in this.ScoreListContainer.GetChildren()) { child.QueueFree();}
		List<LeaderboardEntry> scores = LeaderboardManager.LoadScores();
		GD.Print($"Scores loaded: {scores.Count}");
		if (scores.Count == 0)
		{
			var emptyLabel = new Label { Text = "No scores yet!", HorizontalAlignment = HorizontalAlignment.Center };
			this.ScoreListContainer.AddChild(emptyLabel);
			return;
		}
		for (int i = 0; i < scores.Count; i++)
		{
			LeaderboardEntry entry = scores[i];
			int index = i;
			var entryRow = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
			var rankLabel = new Label { Text = $"{index + 1}.", CustomMinimumSize = new Vector2(50, 0) };
			var nameLabel = new Label { Text = entry.Name, SizeFlagsHorizontal = SizeFlags.ExpandFill };
			var scoreLabel = new Label { Text = entry.Score.ToString(), CustomMinimumSize = new Vector2(100, 0), HorizontalAlignment = HorizontalAlignment.Right };
			var deleteButton = new Button { Text = "X", CustomMinimumSize = new Vector2(30, 0) };
			deleteButton.Pressed += () => { LeaderboardManager.DeleteScore(index); PopulateLeaderboard(); };
			entryRow.AddChild(rankLabel);
			entryRow.AddChild(nameLabel);
			entryRow.AddChild(scoreLabel);
			entryRow.AddChild(deleteButton);
			this.ScoreListContainer.AddChild(entryRow);
		}
	}
}
