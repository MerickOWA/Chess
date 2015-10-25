using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	static class CellExtensions
	{
		private static Cell ToCell(int file, int rank)
		{
			return file >= 0 && file < 8 && rank >= 0 && rank < 8 ? (Cell)(rank * 8 + file) : Cell.None;
		}

		private static int ToFile(this Cell cell)
		{
			return (int)cell % 8;
		}

		private static int ToRank(this Cell cell)
		{
			return (int)cell / 8;
		}

		public static Cell Move(this Cell cell, Tuple<int, int> direction)
		{
			if (cell == Cell.None)
			{
				throw new InvalidOperationException("Can't move from nowhere");
			}

			return ToCell(cell.ToFile() + direction.Item1, cell.ToRank() + direction.Item2);
		}
	}
}
