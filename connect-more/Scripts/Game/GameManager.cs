using System.Linq;
using ConnectMore.Scripts.Data;
using ConnectMore.Scripts.UI;
using Godot;

namespace ConnectMore.Scripts.Game;

public partial class GameManager : Node2D
{
    [Export] public Board Board { get; set; }

    [Export] public Label StatusLabel { get; set; }

    [Export] public Label ScoresLabel { get; set; }

    [Export] public Button RestartButton { get; set; }

    [Export] public Control Columns { get; set; }

    [Export] public PackedScene LeaderboardScene { get; set; }
    
    public GameSetup Settings { get; set; }

    public int[] PlayersScores { get; set; }

    private int currentPlayer;
    private bool gameOver;

    public override void _Ready()
    {
        this.RestartButton.Pressed += this.ResetGame;

        this.SetupColumnButtons();
        this.ResetGame();
    }

    private void SetupColumnButtons()
    {
        for (int column = 0; column < this.Settings.Columns; column++)
        {
            Button button = new();
            button.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
            int columnIndex = column;
            button.Pressed += () => this.OnColumnPressed(columnIndex);
            int size = this.Board.CellSize;
            button.CustomMinimumSize = new Vector2(size, size * this.Settings.Rows);
            button.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            button.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            this.Columns.AddChild(button);
        }
    }

    private void ResetGame()
    {
        this.Board.ClearBoard();
        this.currentPlayer = 0;
        this.gameOver = false;
        this.PlayersScores = new int[this.Settings.Players]; // lazy oneliner
        this.UpdateScore();
        this.UpdateStatus();
    }

    private void OnColumnPressed(int column)
    {
        if (this.gameOver)
        {
            this.StatusLabel.Text = "Game is over. Please reset to try again.";

            return;
        }

        int row = this.Board.DropDisc(column, this.currentPlayer);

        if (row == Board.ColumnFull)
        {
            this.StatusLabel.Text = $"Column {column + 1} is full! (Player {this.currentPlayer}'s turn)";

            return;
        }

        int matches = this.Board.CheckMatch(row, column, this.currentPlayer);

        if (matches > 0)
        {
            this.PlayersScores[this.currentPlayer] += matches;
            this.UpdateScore();

            string plurality = matches == 1
                ? "a point"
                : $"{matches} points";

            this.StatusLabel.Text = $"Player {this.currentPlayer} gains {plurality}!";
        }

        if (this.Board.IsFull())
        {
            this.OnGameEnd();

            return;
        }

        this.NextPlayer();
    }

    private void NextPlayer()
    {
        this.currentPlayer += 1;
        this.currentPlayer %= this.Settings.Players;
        this.UpdateStatus();
    }

    private void UpdateStatus()
    {
        this.StatusLabel.Text = $"Player {this.currentPlayer}'s turn";
    }

    private void UpdateScore()
    {
        this.ScoresLabel.Text = string.Join(" | ",
                                            this.PlayersScores.Select((score, index) => $"P{index + 1} = {score}"));
    }

    private void OnGameEnd()
    {
        this.gameOver = true;

        int maxScore = this.PlayersScores.Any() ? this.PlayersScores.Max() : 0;

        int[] winnerIndexes = this.PlayersScores
                                  .Select((score, index) => new
                                  {
                                      PlayerIndex = index,
                                      Score = score,
                                  })
                                  .Where(p => p.Score == maxScore)
                                  .Select(p => p.PlayerIndex)
                                  .ToArray();

        string[] winnerNames = winnerIndexes.Select(i => $"Player {i + 1}").ToArray();

        string winnerText = winnerNames.Length > 1
            ? $"{string.Join(" & ", winnerNames)} Tie!"
            : $"{winnerNames[0]} Wins!";

        this.ScoresLabel.Text = winnerText;
        
        GameData gameData = new();
        gameData.WinnerString = winnerText;
        gameData.PendingScoreEntry = new LeaderboardEntry(string.Empty, maxScore);

        Leaderboard leaderboard = this.LeaderboardScene.Instantiate<Leaderboard>();
        leaderboard._gameData = gameData;
        this.GetTree().Root.AddChild(leaderboard);
        this.GetTree().CurrentScene = leaderboard;
        this.GetTree().Root.RemoveChild(this);
    }
}