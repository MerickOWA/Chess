using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	[Flags]
	enum Piece
	{
		None = 0,

		Type = 7,
		Color = 24,

		Pawn = 1,
		Knight = 2,
		Bishop = 3,
		Rook = 4,
		Queen = 5,
		King = 6,

		White = 8,
		Black = 16,

		WhitePawn = White | Pawn,
		WhiteKnight = White | Knight,
		WhiteBishop = White | Bishop,
		WhiteRook = White | Rook,
		WhiteQueen = White | Queen,
		WhiteKing = White | King,
		BlackPawn = Black | Pawn,
		BlackKnight = Black | Knight,
		BlackBishop = Black | Bishop,
		BlackRook = Black | Rook,
		BlackQueen = Black | Queen,
		BlackKing = Black | King,
	}
}
