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

			var state = target.ToState();

			Assert.AreEqual(Piece.None, state.Board[Cell.a1]);
			Assert.AreEqual(Piece.WhiteRook, state.Board[Cell.b1]);
		}

		[TestMethod]
		public void Ensure_castling_white_king_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.e1, Cell.g1));

			var state = target.ToState();

			Assert.AreEqual(Piece.None, state.Board[Cell.h1]);
			Assert.AreEqual(Piece.WhiteRook, state.Board[Cell.f1]);
		}

		[TestMethod]
		public void Ensure_castling_white_queen_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.e1, Cell.c1));

			var state = target.ToState();

			Assert.AreEqual(Piece.None, state.Board[Cell.a1]);
			Assert.AreEqual(Piece.WhiteRook, state.Board[Cell.d1]);
		}

		[TestMethod]
		public void Ensure_castling_black_king_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.e8, Cell.g8));

			var state = target.ToState();

			Assert.AreEqual(Piece.None, state.Board[Cell.h8]);
			Assert.AreEqual(Piece.BlackRook, state.Board[Cell.f8]);
		}

		[TestMethod]
		public void Ensure_castling_black_queen_side_moves_rook()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.e8, Cell.c8));

			var state = target.ToState();

			Assert.AreEqual(Piece.None, state.Board[Cell.a8]);
			Assert.AreEqual(Piece.BlackRook, state.Board[Cell.d8]);
		}

		[TestMethod]
		public void Ensure_EnPassant_Move_Captures_Appropriate_Pawn()
		{
			target.MakeMove(new Move(Cell.a2, Cell.a4));
			target.MakeMove(new Move(Cell.b4, Cell.a3));

			var state = target.ToState();

			Assert.AreEqual(Piece.None, state.Board[Cell.a4]);
		}

		[TestMethod]
		public void Ensure_Move_By_Non_Pawn_To_EnPassant_Square_Doesnt_Capture()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.c7, Cell.c5));
			target.MakeMove(new Move(Cell.e5, Cell.c6));

			var state = target.ToState();

			Assert.AreEqual(Piece.BlackPawn, state.Board[Cell.c5]);
		}

		[TestMethod]
		public void Ensure_Pawn_Promotions_Correctly()
		{
			target.MakeMove(new Move(Cell.d5, Cell.d6));
			target.MakeMove(new Move(Cell.a8, Cell.b8));
			target.MakeMove(new Move(Cell.d6, Cell.c7));
			target.MakeMove(new Move(Cell.b8, Cell.a8));
			target.MakeMove(new Move(Cell.c7, Cell.c8, Piece.Queen));

			var state = target.ToState();

			Assert.AreEqual(Piece.WhiteQueen, state.Board[Cell.c8]);
		}

		[TestMethod]
		public void Ensure_Active_Changes()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			var state1 = target.ToState();

			Assert.AreNotEqual(initial.Active, state1.Active);

			target.MakeMove(new Move(Cell.a8, Cell.b8));

			var state2 = target.ToState();

			Assert.AreNotEqual(state1.Active, state2.Active);
			Assert.AreEqual(initial.Active, state2.Active);
		}

		[TestMethod]
		public void Ensure_Move_Count_Doesnt_Increment_On_White_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			var state = target.ToState();

			Assert.AreEqual(initial.Move, state.Move);
		}

		[TestMethod]
		public void Ensure_Move_Count_Does_Increment_On_Black_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.a8, Cell.b8));

			var state = target.ToState();

			Assert.AreEqual(initial.Move + 1, state.Move);
		}

		[TestMethod]
		public void Ensure_Updates_Enpassant_State_Correctly()
		{
			target.MakeMove(new Move(Cell.a2, Cell.a4));

			Assert.AreEqual(Cell.a3, target.ToState().Enpassant);

			target.MakeMove(new Move(Cell.c7, Cell.c5));

			Assert.AreEqual(Cell.c6, target.ToState().Enpassant);

			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(Cell.None, target.ToState().Enpassant);
		}

		[TestMethod]
		public void Ensure_DrawClock_Increments_On_Non_Capture_Non_Pawn_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			var state = target.ToState();

			Assert.AreEqual(initial.DrawClock + 1, state.DrawClock);
		}

		[TestMethod]
		public void Ensure_DrawClock_Resets_On_Capture()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.b6, Cell.d5));

			var state = target.ToState();

			Assert.AreEqual(0, state.DrawClock);
		}

		[TestMethod]
		public void Ensure_DrawClock_Resets_On_Pawn_Move()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.MakeMove(new Move(Cell.c7, Cell.c5));

			var state = target.ToState();

			Assert.AreEqual(0, state.DrawClock);
		}

		[TestMethod]
		public void Ensure_Castling_Flags_Are_Cleared_On_Move_Of_King()
		{
			target.MakeMove(new Move(Cell.e1, Cell.f1));

			Assert.AreEqual(Castling.kq, target.ToState().Castling);

			target.MakeMove(new Move(Cell.e8, Cell.f8));

			Assert.AreEqual(Castling.None, target.ToState().Castling);
		}

		[TestMethod]
		public void Ensure_Castling_Flags_Are_Cleared_On_Move_Of_Rook_On_A_File()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));

			Assert.AreEqual(Castling.Kkq, target.ToState().Castling);

			target.MakeMove(new Move(Cell.a8, Cell.b8));

			Assert.AreEqual(Castling.Kk, target.ToState().Castling);
		}

		[TestMethod]
		public void Ensure_Castling_Flags_Are_Cleared_On_Move_Of_Rook_On_H_File()
		{
			target.MakeMove(new Move(Cell.h1, Cell.g1));

			Assert.AreEqual(Castling.Qkq, target.ToState().Castling);

			target.MakeMove(new Move(Cell.h8, Cell.g8));

			Assert.AreEqual(Castling.Qq, target.ToState().Castling);
		}
	}
}
