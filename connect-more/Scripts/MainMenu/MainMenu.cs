using Godot;

namespace ConnectMore.Scripts.MainMenu;

public partial class MainMenu : Control
{
	[Export] public PackedScene GameScene { get; set; }
	
	[Export] public PackedScene LeaderboardScene { get; set; }
	
	public override void _Ready()
	{
		this.GetNode<Button>("VBoxContainer/Button").Pressed += this.OnStartPressed;
		this.GetNode<Button>("VBoxContainer/Button2").Pressed += this.OnLeaderboardPressed;
	}

	private void OnStartPressed()
	{
		GD.Print("Start button pressed!");
		this.GetTree().ChangeSceneToPacked(this.GameScene);
	}

	private void OnLeaderboardPressed()
	{
		this.GetTree().ChangeSceneToPacked(this.LeaderboardScene);
	}
}