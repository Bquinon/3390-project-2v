using System;
using Godot;

namespace ConnectMore.Scripts.Game;

public partial class Board : Node2D
{
	private const int OffsetFromTop = 125;

	[Export] public int Rows { get; set; } = 6;

	[Export] public int Columns { get; set; } = 7;

	[Export] public PackedScene DiscBackgroundScene { get; set; }

	[Export] public PackedScene DiscScene { get; set; }

	[Export] public Node2D DiscContainer { get; set; }

	[Export] public Node2D DiscBackgroundContainer { get; set; }

	[Export] public int ConnectLength { get; set; } = 4;

	public int CellSize { get; private set; }
	
	private int[,] grid; // Rows x Columns

	public override void _Ready()
	{
		int maxRowHeight = (1080 - OffsetFromTop) / this.Rows;
		int maxColumnWidth = 1920 / this.Columns;
		this.CellSize = Math.Min(maxRowHeight, maxColumnWidth);
		
		this.grid = new int[this.Rows, this.Columns];
		this.ClearBoard();
		this.SetupBackgrounds();
	}

	private void SetupBackgrounds()
	{
		for (int row = 0; row < this.Rows; row++)
		{
			for (int column = 0; column < this.Columns; column++)
			{
				this.SpawnDiscBackgroundVisual(row, column);
			}
		}
	}

	public void ClearBoard()
	{
		for (int row = 0; row < this.Rows; row++)
		{
			for (int column = 0; column < this.Columns; column++)
			{
				this.grid[row, column] = 0;
			}
		}

		foreach (Node child in this.DiscContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	public int DropDisc(int column, int playerId)
	{
		if (!column.IsBetween(0, this.Columns))
		{
			return GameManager.RowFull;
		}

		for (int row = 0; row < this.Rows; row++)
		{
			if (this.grid[row, column] == 0)
			{
				this.grid[row, column] = playerId;
				this.SpawnDiscVisual(row, column, playerId);

				return row;
			}
		}

		return GameManager.RowFull;
	}

	private void SpawnDiscBackgroundVisual(int row, int column)
	{
		DiscBackground discBackground = this.DiscBackgroundScene.Instantiate<DiscBackground>();
		discBackground.Name = $"DiscBackground_{row}_{column}";
		discBackground.TargetSize = this.CellSize;
		discBackground.Position = this.GridToLocalPosition(row, column);
		discBackground.UpdateVisual();

		this.DiscBackgroundContainer.AddChild(discBackground);
	}

	private void SpawnDiscVisual(int row, int column, int playerId)
	{
		Disc disc = this.DiscScene.Instantiate<Disc>();
		disc.Name = $"Disc_{row}_{column}";
		disc.TargetSize = this.CellSize;
		disc.Position = this.GridToLocalPosition(row, column);
		disc.PlayerId = playerId;
		disc.UpdateVisual();

		this.DiscContainer.AddChild(disc);
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

	public int CheckMatch(int row, int column, int playerId)
	{
		int matches = 0;
		
		if (playerId == 0)
		{
			return matches;
		}

		(int deltaRow, int deltaColumn)[] directions =
		[
			(0, 1),  // horizontal
			(1, 0),  // vertical
			(1, 1),  // diagonal
			(1, -1), // anti-diagonal
		];

		foreach ((int deltaRow, int deltaColumn) in directions)
		{
			int count = 1;

			count += this.CountInDirection(row, column, deltaRow, deltaColumn, playerId);
			count += this.CountInDirection(row, column, -deltaRow, -deltaColumn, playerId);

			if (count >= this.ConnectLength)
			{
				matches++;
			}
		}

		return matches;
	}

	private int CountInDirection(int startRow, int startColumn, int deltaRow, int deltaColumn, int playerId)
	{
		int count = 0;
		int row = startRow + deltaRow;
		int column = startColumn + deltaColumn;
		
		while (row.IsBetween(0, this.Rows) &&
			   column.IsBetween(0, this.Columns) && 
			   this.grid[row, column] == playerId)
		{
			count++;
			row += deltaRow;
			column += deltaColumn;
		}

		return count;
	}
	
	public bool IsFull()
	{
		// we don't have to check the whole board, only the top row
		for (int column = 0; column < this.Columns; column++)
		{
			if (this.grid[this.Rows - 1, column] == 0)

			{
				return false;
			}
		}

		return true;
	}
}