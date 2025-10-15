using Godot;

namespace ConnectMore.Scripts.Game;

public partial class GameManager : Node2D
{
    [Export] public Board Board { get; set; }

    [Export] public Label StatusLabel { get; set; }

    [Export] public Button RestartButton { get; set; }

    [Export] public Control Columns { get; set; }

    public int Players { get; set; } = 2;
    
    public int[] PlayersScores { get; set; }
    
    private int currentPlayer = 1;
    private bool gameOver;

    public override void _Ready()
    {
        this.RestartButton.Pressed += this.OnRestartPressed;

        for (int i = 0; i < this.Board.Columns; i++)
        {
            Button button = new();
            button.Text = (i + 1).ToString();
            int colIndex = i;
            button.Pressed += () => this.OnColumnPressed(colIndex);
            int size = this.Board.CellSize - 5;
            button.CustomMinimumSize = new Vector2(size, size);
            button.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            button.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            this.Columns.AddChild(button);
        }

        this.UpdateStatus();
    }

    private void OnRestartPressed()
    {
        this.Board.ClearBoard();
        this.currentPlayer = 1;
        this.gameOver = false;
        this.UpdateStatus();
    }

    private void OnColumnPressed(int column)
    {
        if (this.gameOver)
        {
            return;
        }

        int row = this.Board.DropDisc(column, this.currentPlayer);

        if (row == -1)
        {
            this.StatusLabel.Text = $"Column {column + 1} is full!";
            // add win screen here once scores are setup

            return;
        }

        if (this.Board.CheckWin(row, column, this.currentPlayer))
        {
            this.StatusLabel.Text = $"Player {this.currentPlayer} wins!";
            this.gameOver = true;
            // add win screen here for now

            return;
        }

        if (this.Board.IsFull())
        {
            this.StatusLabel.Text = "Draw!";
            this.gameOver = true;

            return;
        }

        this.currentPlayer = (this.currentPlayer % this.Players) + 1;
        this.UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (this.gameOver)
        {
            return;
        }

        this.StatusLabel.Text = $"Player {this.currentPlayer}'s turn";
    }
}