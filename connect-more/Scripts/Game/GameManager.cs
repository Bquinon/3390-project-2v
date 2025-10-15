using System.Linq;
using Godot;

namespace ConnectMore.Scripts.Game;

public partial class GameManager : Node2D
{
    public const int RowFull = -1;
    
    [Export] public Board Board { get; set; }

    [Export] public Label StatusLabel { get; set; }
    
    [Export] public Label ScoresLabel { get; set; }

    [Export] public Button RestartButton { get; set; }

    [Export] public Control Columns { get; set; }

    [Export] public int Players { get; set; } = 2;
    
    public int[] PlayersScores { get; set; }
    
    private int currentPlayer = 1;
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
        this.currentPlayer = 1;
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

        if (this.TryGetRow(column, out int row))
        {
            return;
        }

        if (this.CheckMatch(column, row))
        {
            return;
        }
        
        if (this.CheckFull())
        {
            this.gameOver = true;
            this.OnGameEnd();
         
            return;
        }

        this.NextPlayer();
    }

    private void NextPlayer()
    {
        this.currentPlayer = (this.currentPlayer % this.Players) + 1;
        this.ResetStatusMessage();
    }

    private void ResetStatusMessage()
    {
        this.StatusLabel.Text = $"Player {this.currentPlayer}'s turn";
    }

    private bool CheckFull()
    {
        if (!this.Board.IsFull())
        {
            return false;
        }

        int bestPlayer = this.PlayersScores.ElementAt(this.PlayersScores.Max());
        this.StatusLabel.Text = $"Game Over! Player{bestPlayer} wins!";

        return true;
    }

    private bool TryGetRow(int column, out int row)
    {
        row = this.Board.DropDisc(column, this.currentPlayer);

        if (row != RowFull)
        {
            return false;
        }

        this.StatusLabel.Text = $"Column {column + 1} is full!  (player{this.currentPlayer}'s turn)";

        return true;
    }

    private bool CheckMatch(int column, int row)
    {
        int matches = this.Board.CheckMatch(row, column, this.currentPlayer);

        if (matches <= 0)
        {
            return false;
        }

        this.PlayersScores[this.currentPlayer - 1] += matches;
        this.UpdateScore();

        string plurality = matches == 1 
            ? "a point" 
            : $"{matches} points";
        
        this.StatusLabel.Text = $"Player {this.currentPlayer} gains {plurality}! Player {this.currentPlayer + 1}'s turn";
        this.currentPlayer = (this.currentPlayer % this.Players) + 1;

        return true;
    }

    private void UpdateScore()
    {
        this.ScoresLabel.Text = string.Join(" | ", 
                                            this.PlayersScores.Select((score, index) => $"P{index + 1} = {score}"));
    }

    private void OnGameEnd()
    {
        //TODO: add win screen here (where we will put scores and stuff)
    }
}