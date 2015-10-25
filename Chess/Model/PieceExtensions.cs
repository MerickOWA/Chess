using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	static class PieceExtensions
	{
		public static Color ToColor(this Piece piece)
		{
			switch (piece)
			{
				case Piece.None:
					throw new InvalidOperationException("None has no color");

				case Piece.WhitePawn:
				case Piece.WhiteKnight:
				case Piece.WhiteBishop:
				case Piece.WhiteRook:
				case Piece.WhiteQueen:
				case Piece.WhiteKing:
					return Color.White;

				case Piece.BlackPawn:
				case Piece.BlackKnight:
				case Piece.BlackBishop:
				case Piece.BlackRook:
				case Piece.BlackQueen:
				case Piece.BlackKing:
					return Color.Black;

				default:
					throw new InvalidOperationException("Unknown piece?");
			}
		}
	}
}
