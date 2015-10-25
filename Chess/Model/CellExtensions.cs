using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	static class CellExtensions
	{
		public static Cell ToCell(int file, int rank)
		{
			return file >= 0 && file < 8 && rank >= 0 && rank < 8 ? (Cell)(rank * 8 + file) : Cell.None;
		}

		public static int ToFile(this Cell cell)
		{
			if (cell == Cell.None)
			{
				throw new InvalidOperationException("Nowhere has no file");
			}

			return (int)cell % 8;
		}

		public static int ToRank(this Cell cell)
		{
			if (cell == Cell.None)
			{
				throw new InvalidOperationException("Nowhere has no rank");
			}

			return (int)cell / 8;
		}

		public static char ToFileChar(this Cell cell)
		{
			return cell != Cell.None ? (char)('a' + cell.ToFile()) : '-';
		}

		public static char ToRankChar(this Cell cell)
		{
			return cell != Cell.None ? (char)('1' + cell.ToRank()) : '-';
		}
	}
}
