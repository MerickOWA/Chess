using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	struct Direction
	{
		Direction(int file, int rank)
		{
			File = file;
			Rank = rank;
		}

		public int File { get; }

		public int Rank { get; }

		public static Cell operator +(Cell cell, Direction direction)
		{
			if (cell == Cell.None)
			{
				throw new InvalidOperationException("Can't move from nowhere");
			}

			return CellExtensions.ToCell(cell.ToFile() + direction.File, cell.ToRank() + direction.Rank);
		}

		public static Direction operator +(Direction a, Direction b)
		{
			return new Direction(a.File + b.File, a.Rank + b.Rank);
		}

		public static readonly Direction Down = new Direction(0, -1);
		public static readonly Direction Left = new Direction(-1, 0);
		public static readonly Direction Right = new Direction(1, 0);
		public static readonly Direction Up = new Direction(0, 1);
		public static readonly Direction DownDownLeft = Down + Down + Left;
		public static readonly Direction DownDownRight = Down + Down + Right;
		public static readonly Direction DownLeftLeft = Down + Left + Left;
		public static readonly Direction DownLeft = Down + Left;
		public static readonly Direction DownRight = Down + Right;
		public static readonly Direction DownRightRight = Down + Right + Right;
		public static readonly Direction LeftLeft = Left + Left;
		public static readonly Direction RightRight = Right + Right;
		public static readonly Direction UpLeftLeft = Up + Left + Left;
		public static readonly Direction UpLeft = Up + Left;
		public static readonly Direction UpRight = Up + Right;
		public static readonly Direction UpRightRight = Up + Right + Right;
		public static readonly Direction UpUpLeft = Up + Up + Left;
		public static readonly Direction UpUpRight = Up + Up + Right;

		public static readonly IEnumerable<Direction> Knight = new[]
		{
			DownDownLeft,
			DownDownRight,
			DownLeftLeft,
			DownRightRight,
			UpLeftLeft,
			UpRightRight,
			UpUpLeft,
			UpUpRight,
		};

		public static readonly IEnumerable<Direction> Bishop = new[]
		{
			DownLeft,
			DownRight,
			UpLeft,
			UpRight,
		};

		public static readonly IEnumerable<Direction> Rook = new[]
		{
			Down,
			Left,
			Right,
			Up,
		};

		public static readonly IEnumerable<Direction> Queen = Rook.Concat(Bishop);
	}
}
