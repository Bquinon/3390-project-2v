using Godot;

namespace ConnectMore.Scripts.Game;

public partial class Board : Node2D
{
	[Export] public int Rows { get; set; } = 6;

	[Export] public int Columns { get; set; } = 7;

	[Export] public PackedScene DiscScene { get; set; }

	[Export] public int CellSize { get; set; }
	
	[Export] public Node2D DiscsContainer { get; set; }
	
	private int[,] grid; // Rows x Columns

	public override void _Ready()
	{
		this.grid = new int[this.Rows, this.Columns];
		this.ClearBoard();
	}

	public void ClearBoard()
	{
		for (int r = 0; r < this.Rows; r++)
		{
			for (int c = 0; c < this.Columns; c++)
			{
				this.grid[r, c] = 0;
			}
		}

		foreach (Node child in this.DiscsContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	public int DropDisc(int column, int playerId)
	{
		if (column < 0
		 || column >= this.Columns)
		{
			return -1;
		}

		for (int r = 0; r < this.Rows; r++)
		{
			if (this.grid[r, column] == 0)
			{
				this.grid[r, column] = playerId;
				this.SpawnDiscVisual(r, column, playerId);

				return r;
			}
		}

		return -1; // full
	}

	private void SpawnDiscVisual(int row, int column, int playerId)
	{
		Disc disc = this.DiscScene.Instantiate<Disc>();
		disc.Name = $"Disc_{row}_{column}";
		disc.TargetSize = this.CellSize;
		disc.Position = this.GridToLocalPosition(row, column);
		disc.PlayerId = playerId;
		disc.UpdateVisual();

		this.DiscsContainer.AddChild(disc);
	}

	// acts as bottom center anchor
	private Vector2 GridToLocalPosition(int row, int column)
	{
		float boardWidth = this.Columns * this.CellSize;
		float boardHeight = this.Rows * this.CellSize;

		float x = ((column + 0.5f) * this.CellSize) - (boardWidth / 2f);
		float y = -((row + 0.5f) * this.CellSize);

		return new Vector2(x, y);
	}

	public int GetCell(int row, int column)
	{
		if (!row.IsBetween(0, this.Rows) ||
			!column.IsBetween(0, this.Columns))
		{
			return -1;
		}

		return this.grid[row, column];
	}

	public bool CheckWin(int row, int col, int playerId)
	{
		if (playerId == 0)
		{
			return false;
		}

		(int deltaRow, int deltaCol)[] directions =
		[
			(0, 1),  // horizontal
			(1, 0),  // vertical
			(1, 1),  // diagonal
			(1, -1), // anti-diagonal
		];

		foreach ((int deltaRow, int deltaCol) in directions)
		{
			int count = 1;

			count += this.CountInDirection(row, col, deltaRow, deltaCol, playerId);
			count += this.CountInDirection(row, col, -deltaRow, -deltaCol, playerId);

			if (count >= 4)
			{
				return true;
			}
		}

		return false;
	}

	private int CountInDirection(int startRow, int startCol, int deltaRow, int deltaCol, int playerId)
	{
		int count = 0;
		int row = startRow + deltaRow;
		int col = startCol + deltaCol;
		
		while (row.IsBetween(0, this.Rows) &&
			   col.IsBetween(0, this.Columns) && 
			   this.grid[row, col] == playerId)
		{
			count++;
			row += deltaRow;
			col += deltaCol;
		}

		return count;
	}
	
	public bool IsFull()
	{
		for (int c = 0; c < this.Columns; c++)
		{
			if (this.grid[this.Rows - 1, c] == 0)
			{
				return false;
			}
		}

		for (int r = 0; r < this.Rows; r++)
		{
			for (int c = 0; c < this.Columns; c++)
			{
				if (this.grid[r, c] == 0)
				{
					return false;
				}
			}
		}

		return true;
	}
}