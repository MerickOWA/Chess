using System;
using Chess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
	[TestClass]
	public class GameStateTests
	{
		[TestMethod]
		public void GameState_FromForsythEdwardsNotation_Parses_Board_Successfully()
		{
			var expectedBoard = new Board(new[]
			{
				Piece.WhiteRook, Piece.WhiteKnight, Piece.WhiteBishop, Piece.WhiteQueen, Piece.WhiteKing, Piece.WhiteBishop, Piece.WhiteKnight, Piece.WhiteRook,
				Piece.WhitePawn, Piece.WhitePawn,   Piece.WhitePawn,   Piece.WhitePawn,  Piece.WhitePawn, Piece.WhitePawn,   Piece.WhitePawn,   Piece.WhitePawn,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.None,      Piece.None,        Piece.None,        Piece.None,       Piece.None,      Piece.None,        Piece.None,        Piece.None,
				Piece.BlackPawn, Piece.BlackPawn,   Piece.BlackPawn,   Piece.BlackPawn,  Piece.BlackPawn, Piece.BlackPawn,   Piece.BlackPawn,   Piece.BlackPawn,
				Piece.BlackRook, Piece.BlackKnight, Piece.BlackBishop, Piece.BlackQueen, Piece.BlackKing, Piece.BlackBishop, Piece.BlackKnight, Piece.BlackRook,
			});

			var state = GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

			Assert.IsNotNull(state);

			for (Cell i = Cell.a1; i <= Cell.h8; i++)
			{
				Assert.AreEqual(expectedBoard[i], state.Board[i]);
			}
		}

		[TestMethod]
		public void GameState_FromForsythEdwardsNotation_Parses_Active_Successfully()
		{
			Assert.AreEqual(Color.White, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1").Active);
			Assert.AreEqual(Color.Black, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1").Active);
		}

		[TestMethod]
		public void FromForsythEdwardsNotation_Parses_Castling_Successfully()
		{
			Assert.AreEqual(Castling.None, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1").Castling);
			Assert.AreEqual(Castling.q, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w q - 0 1").Castling);
			Assert.AreEqual(Castling.k, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w k - 0 1").Castling);
			Assert.AreEqual(Castling.kq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w kq - 0 1").Castling);
			Assert.AreEqual(Castling.Q, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Q - 0 1").Castling);
			Assert.AreEqual(Castling.Qq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qq - 0 1").Castling);
			Assert.AreEqual(Castling.Qk, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qk - 0 1").Castling);
			Assert.AreEqual(Castling.Qkq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qkq - 0 1").Castling);
			Assert.AreEqual(Castling.K, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w K - 0 1").Castling);
			Assert.AreEqual(Castling.Kq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kq - 0 1").Castling);
			Assert.AreEqual(Castling.Kk, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kk - 0 1").Castling);
			Assert.AreEqual(Castling.Kkq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kkq - 0 1").Castling);
			Assert.AreEqual(Castling.KQ, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1").Castling);
			Assert.AreEqual(Castling.KQq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq - 0 1").Castling);
			Assert.AreEqual(Castling.KQk, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQk - 0 1").Castling);
			Assert.AreEqual(Castling.KQkq, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1").Castling);
		}

		[TestMethod]
		public void GameState_FromForsythEdwardsNotation_Parses_EnPassant_Successfully()
		{
			Assert.AreEqual(Cell.e3, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e3 0 1").Enpassant);
			Assert.AreEqual(Cell.b6, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq b6 0 1").Enpassant);
		}

		[TestMethod]
		public void GameState_FromForsythEdwardsNotation_Parses_DrawClock_Successfully()
		{
			Assert.AreEqual(0, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e3 0 1").DrawClock);
			Assert.AreEqual(2, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e3 2 1").DrawClock);
			Assert.AreEqual(15, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e3 15 1").DrawClock);
		}

		[TestMethod]
		public void GameState_FromForsythEdwardsNotation_Parses_Move_Successfully()
		{
			Assert.AreEqual(1, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1").Move);
			Assert.AreEqual(23, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 23").Move);
			Assert.AreEqual(456, GameState.FromForsythEdwardsNotation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 456").Move);
		}

		[TestMethod]
		public void GameState_FromForsythEdwardsNotation_Parses_Successfully_With_No_DrawClock_And_Move()
		{
			var target = GameState.FromForsythEdwardsNotation("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");

			Assert.AreEqual(0, target.DrawClock);
			Assert.AreEqual(1, target.Move);
		}

		[TestMethod]
		public void GameState_ToString_Formats_Successfully()
		{
			var expected = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
			var actual = GameState.FromForsythEdwardsNotation(expected).ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}
