using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	static class PieceExtensions
	{
		public static char ToChar(this Piece piece)
		{
			switch (piece)
			{
				case Piece.WhitePawn:   return 'P';
				case Piece.WhiteKnight: return 'N';
				case Piece.WhiteBishop: return 'B';
				case Piece.WhiteRook:   return 'R';
				case Piece.WhiteQueen:  return 'Q';
				case Piece.WhiteKing:   return 'K';
				case Piece.BlackPawn:   return 'p';
				case Piece.BlackKnight: return 'n';
				case Piece.BlackBishop: return 'b';
				case Piece.BlackRook:   return 'r';
				case Piece.BlackQueen:  return 'q';
				case Piece.BlackKing:   return 'k';
				case Piece.Knight:      return 'n';
				case Piece.Bishop:      return 'b';
				case Piece.Rook:        return 'r';
				case Piece.Queen:       return 'q';

				default:
					throw new InvalidOperationException("Unknown piece");
			}
		}
	}
}
