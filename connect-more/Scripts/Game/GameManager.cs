using System.Linq;
using Godot;

namespace ConnectMore.Scripts.Game;

public partial class GameManager : Node2D
{
    [Export] public Board Board { get; set; }

    [Export] public Label StatusLabel { get; set; }
    
    [Export] public Label ScoresLabel { get; set; }

    [Export] public Button RestartButton { get; set; }

    [Export] public Control Columns { get; set; }

    [Export] public int Players { get; set; } = 2;
    
    public int[] PlayersScores { get; set; }
    
    private int currentPlayer = 0;
    private bool gameOver;

    public override void _Ready()
    {
        this.PlayersScores = new int[this.Players];
        this.RestartButton.Pressed += this.OnRestartPressed;

        for (int i = 0; i < this.Board.Columns; i++)
        {
            Button button = new();
            button.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
            int colIndex = i;
            button.Pressed += () => this.OnColumnPressed(colIndex);
            int size = this.Board.CellSize;
            button.CustomMinimumSize = new Vector2(size, size * this.Board.Rows);
            button.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            button.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            this.Columns.AddChild(button);
        }

        this.ResetStatusMessage();
        this.UpdateScore();
    }

    private void OnRestartPressed()
    {
        this.Board.ClearBoard();
        this.currentPlayer = 0;
        this.gameOver = false;
        this.ResetStatusMessage();
        this.PlayersScores = new int[this.Players]; // lazy oneliner
        this.UpdateScore();
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
        this.currentPlayer %= this.Players;
        this.ResetStatusMessage();
    }

    private void ResetStatusMessage()
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
        
        int maxScore = this.PlayersScores.Max();
        
        string[] winners = this.PlayersScores
                               .Select((score, index) => new { Player = index + 1, Score = score })
                               .Where(p => p.Score == maxScore)
                               .Select(p => $"Player {p.Player}")
                               .ToArray();

        string winnerText = winners.Length > 1
            ? $"{string.Join(" & ", winners)} tie with {maxScore} points!"
            : $"{winners[0]} wins with {maxScore} points!";

        this.StatusLabel.Text = $"Game Over! {winnerText}";
        //TODO: add win screen here (where we will put scores and stuff)
    }
}