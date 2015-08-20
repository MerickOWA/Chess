using Chess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
	[TestClass]
	public class BoardTests
	{
		[TestMethod]
		public void Board_Is_Immutable_After_Construction()
		{
			var array = new[]
			{
				Piece.WhiteRook, Piece.WhiteKnight, Piece.WhiteBishop, Piece.WhiteQueen, Piece.WhiteKing, Piece.WhiteBishop, Piece.WhiteKnight, Piece.WhiteRook,
				Piece.WhitePawn, Piece.WhitePawn,   Piece.WhitePawn,   Piece.WhitePawn,  Piece.WhitePawn, Piece.WhitePawn,   Piece.WhitePawn,   Piece.WhitePawn,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.BlackPawn, Piece.BlackPawn,   Piece.BlackPawn,   Piece.BlackPawn,  Piece.BlackPawn, Piece.BlackPawn,   Piece.BlackPawn,   Piece.BlackPawn,
				Piece.BlackRook, Piece.BlackKnight, Piece.BlackBishop, Piece.BlackQueen, Piece.BlackKing, Piece.BlackBishop, Piece.BlackKnight, Piece.BlackRook,
			};

			var target = new Board(array);

			//*** Initial state: a1 should have a WhiteRook
			Assert.AreEqual(Piece.WhiteRook, target[Cell.a1]);

			//*** Outside array is mutated
			array[0] = Piece.BlackRook;

			//*** Expected: board a1 should maintain its inital state
			Assert.AreEqual(Piece.WhiteRook, target[Cell.a1]);
		}
	}
}
