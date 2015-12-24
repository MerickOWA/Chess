using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests.MoveGeneratorTests
{
	[TestClass]
	public class UndoMoveTests
	{
		private readonly MoveGenerator target;

		public UndoMoveTests()
		{
			target = new MoveGenerator(GameState.FromForsythEdwardsNotation("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -"));
		}

		[TestMethod]
		public void Ensure_UndoMove_moves_piece_back()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.a1, Cell.b1)));

			Assert.AreEqual(Piece.WhiteRook, target.Board[Cell.a1]);
		}

		[TestMethod]
		public void Ensure_UndoMove_restores_captured_piece()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.f3, Cell.f6)));

			Assert.AreEqual(Piece.BlackKnight, target.Board[Cell.f6]);
		}

		[TestMethod]
		public void Ensure_UndoMove_restores_enpassent_captured_pawn()
		{
			target.MakeMove(new Move(Cell.a2, Cell.a4));
			target.UndoMove(target.MakeMove(new Move(Cell.b4, Cell.a3)));

			Assert.AreEqual(Piece.WhitePawn, target.Board[Cell.a4]);
		}

		[TestMethod]
		public void Ensure_UndoMove_moves_white_king_side_rook_back()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.e1, Cell.g1)));

			Assert.AreEqual(Piece.WhiteRook, target.Board[Cell.h1]);
		}

		[TestMethod]
		public void Ensure_UndoMove_moves_white_queeen_side_rook_back()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.e1, Cell.c1)));

			Assert.AreEqual(Piece.WhiteRook, target.Board[Cell.a1]);
		}

		[TestMethod]
		public void Ensure_UndoMove_moves_black_king_side_rook_back()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.UndoMove(target.MakeMove(new Move(Cell.e8, Cell.g8)));

			Assert.AreEqual(Piece.BlackRook, target.Board[Cell.h8]);
		}

		[TestMethod]
		public void Ensure_UndoMove_moves_black_queeen_side_rook_back()
		{
			target.MakeMove(new Move(Cell.a1, Cell.b1));
			target.UndoMove(target.MakeMove(new Move(Cell.e8, Cell.c8)));

			Assert.AreEqual(Piece.BlackRook, target.Board[Cell.a8]);
		}

		[TestMethod]
		public void Ensure_UndoMove_unpromotes_pawn()
		{
			target.MakeMove(new Move(Cell.d5, Cell.d6));
			target.MakeMove(new Move(Cell.a8, Cell.b8));
			target.MakeMove(new Move(Cell.d6, Cell.c7));
			target.MakeMove(new Move(Cell.b8, Cell.a8));
			target.UndoMove(target.MakeMove(new Move(Cell.c7, Cell.c8, Piece.Queen)));

			Assert.AreEqual(Piece.WhitePawn, target.Board[Cell.c7]);
		}

		[TestMethod]
		public void Ensure_UndoMove_restores_castling_state()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.e1, Cell.f1)));

			Assert.AreEqual(Castling.KQkq, target.Castling);
		}

		[TestMethod]
		public void Ensure_UndoMove_restores_enpassant_state()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.a2, Cell.a4)));

			Assert.AreEqual(Cell.None, target.Enpassant);
		}

		[TestMethod]
		public void Ensure_UndoMove_restores_draw_clock_state()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.a1, Cell.b1)));

			Assert.AreEqual(0, target.DrawClock);
		}

		[TestMethod]
		public void Ensure_UndoMove_restores_active_color()
		{
			target.UndoMove(target.MakeMove(new Move(Cell.a1, Cell.b1)));

			Assert.AreEqual(Piece.White, target.Active);
		}
	}
}
