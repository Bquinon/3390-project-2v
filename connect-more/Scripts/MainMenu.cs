using Godot;
using System;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
		GetNode<Button>("VBoxContainer/Button").Pressed += OnStartPressed;
		GetNode<Button>("VBoxContainer/Button2").Pressed += OnLeaderboardPressed;
	}

	private void OnStartPressed()
	{
		GD.Print("Start button pressed!");
		GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
	}

	private void OnLeaderboardPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Leaderboard.tscn");
	}
}
