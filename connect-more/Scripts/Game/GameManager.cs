using Godot;

namespace ConnectMore.Scripts.Game;

public partial class GameManager : Node2D
{
    [Export] public Board Board { get; set; }

    [Export] public Label StatusLabel { get; set; }

    [Export] public Button RestartButton { get; set; }

    [Export] public Control Columns { get; set; }

    private int currentPlayer = 1;
    private bool gameOver;

    public override void _Ready()
    {
        this.RestartButton.Pressed += this.OnRestartPressed;

        int i = 0;

        foreach (Node n in this.Columns.GetChildren())
        {
            if (n is Button b)
            {
                int colIndex = i;
                b.Text = (i + 1).ToString();
                b.Pressed += () => this.OnColumnPressed(colIndex);
                i++;
            }
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

            return;
        }

        if (this.Board.CheckWin(row, column, this.currentPlayer))
        {
            this.StatusLabel.Text = $"Player {this.currentPlayer} wins!";
            this.gameOver = true;

            return;
        }

        if (this.Board.IsFull())
        {
            this.StatusLabel.Text = "Draw!";
            this.gameOver = true;

            return;
        }

        this.currentPlayer = this.currentPlayer == 1 ? 2 : 1;
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