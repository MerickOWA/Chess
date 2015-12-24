using System;
using Chess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests.MoveGeneratorTests
{
	[TestClass]
	public class MakeMoveTests
	{
		private readonly GameState initial = GameState.FromForsythEdwardsNotation("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -");
		private readonly MoveGenerator target;

		public MakeMoveTests()
		{
			target = new MoveGenerator(initial);
		}

		[TestMethod]
		public void Ensure_Piece_Is_Moved()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(Piece.None, target.Board[Cell.a1]);
			Assert.AreEqual(Piece.WhiteRook, target.Board[Cell.b1]);
		}

		[TestMethod]
		public void Ensure_castling_white_king_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.e1, Cell.g1));

			Assert.AreEqual(Piece.None, target.Board[Cell.h1]);
			Assert.AreEqual(Piece.WhiteRook, target.Board[Cell.f1]);
		}

		[TestMethod]
		public void Ensure_castling_white_queen_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.e1, Cell.c1));

			Assert.AreEqual(Piece.None, target.Board[Cell.a1]);
			Assert.AreEqual(Piece.WhiteRook, target.Board[Cell.d1]);
		}

		[TestMethod]
		public void Ensure_castling_black_king_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.e8, Cell.g8));

			Assert.AreEqual(Piece.None, target.Board[Cell.h8]);
			Assert.AreEqual(Piece.BlackRook, target.Board[Cell.f8]);
		}

		[TestMethod]
		public void Ensure_castling_black_queen_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.e8, Cell.c8));

			Assert.AreEqual(Piece.None, target.Board[Cell.a8]);
			Assert.AreEqual(Piece.BlackRook, target.Board[Cell.d8]);
		}

		[TestMethod]
		public void Ensure_EnPassant_Move_Captures_Appropriate_Pawn()
		{
			target.MakeMove(new Move(Cell.a2, Cell.a4));
			target.MakeMove(new Move(Cell.b4, Cell.a3));

			Assert.AreEqual(Piece.None, target.Board[Cell.a4]);
		}

		[TestMethod]
		public void Ensure_Move_By_Non_Pawn_To_EnPassant_Square_Doesnt_Capture()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.c7, Cell.c5));
			target.MakeMove(new Move(Cell.e5, Cell.c6));

			Assert.AreEqual(Piece.BlackPawn, target.Board[Cell.c5]);
		}

		[TestMethod]
		public void Ensure_Pawn_Promotions_Correctly()
		{
			target.MakeMove(new Move(Cell.d5, Cell.d6));
			target.MakeMove(new Move(Cell.a8, Cell.b8));
			target.MakeMove(new Move(Cell.d6, Cell.c7));
			target.MakeMove(new Move(Cell.b8, Cell.a8));
			target.MakeMove(new Move(Cell.c7, Cell.c8, Piece.Queen));

			Assert.AreEqual(Piece.WhiteQueen, target.Board[Cell.c8]);
		}

		[TestMethod]
		public void Ensure_Active_Changes()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(Piece.Black, target.Active);

			target.MakeMove(new Move(Cell.a8, Cell.b8));

			Assert.AreEqual(Piece.White, target.Active);
		}

		[TestMethod]
		public void Ensure_Move_Count_Doesnt_Increment_On_White_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(1, target.Move);
		}

		[TestMethod]
		public void Ensure_Move_Count_Does_Increment_On_Black_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.a8, Cell.b8));

			Assert.AreEqual(2, target.Move);
		}

		[TestMethod]
		public void Ensure_Updates_Enpassant_State_Correctly()
		{
			target.MakeMove(new Move(Cell.a2, Cell.a4));

			Assert.AreEqual(Cell.a3, target.Enpassant);

			target.MakeMove(new Move(Cell.c7, Cell.c5));

			Assert.AreEqual(Cell.c6, target.Enpassant);

			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(Cell.None, target.Enpassant);
		}

		[TestMethod]
		public void Ensure_DrawClock_Increments_On_Non_Capture_Non_Pawn_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(1, target.DrawClock);
		}

		[TestMethod]
		public void Ensure_DrawClock_Resets_On_Capture()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.b6, Cell.d5));

			Assert.AreEqual(0, target.DrawClock);
		}

		[TestMethod]
		public void Ensure_DrawClock_Resets_On_Pawn_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.c7, Cell.c5));

			Assert.AreEqual(0, target.DrawClock);
		}

		[TestMethod]
		public void Ensure_Castling_Flags_Are_Cleared_On_Move_Of_King()
		{
			target.MakeMove(new Move(Cell.e1, Cell.f1));

			Assert.AreEqual(Castling.kq, target.Castling);

			target.MakeMove(new Move(Cell.e8, Cell.f8));

			Assert.AreEqual(Castling.None, target.Castling);
		}

		[TestMethod]
		public void Ensure_Castling_Flags_Are_Cleared_On_Move_Of_Rook_On_A_File()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(Castling.Kkq, target.Castling);

			target.MakeMove(new Move(Cell.a8, Cell.b8));

			Assert.AreEqual(Castling.Kk, target.Castling);
		}

		[TestMethod]
		public void Ensure_Castling_Flags_Are_Cleared_On_Move_Of_Rook_On_H_File()
		{
			target.MakeMove(new Move(Cell.h1, Cell.g1));

			Assert.AreEqual(Castling.Qkq, target.Castling);

			target.MakeMove(new Move(Cell.h8, Cell.g8));

			Assert.AreEqual(Castling.Qq, target.Castling);
		}
	}
}
