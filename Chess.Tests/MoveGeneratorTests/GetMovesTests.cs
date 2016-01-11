using System;
using System.Linq;
using Chess.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests.MoveGeneratorTests
{
	[TestClass]
	public class GetMovesTests
	{
		//From http://chessprogramming.wikispaces.com/Perft+Results
		//Depth  Nodes      Captures  E.p.   Castles  Promotions  Checks   Checkmates
		//1      48         8         0      2        0           0        0
		//2      2039       351       1      91       0           3        0
		//3      97862      17102     45     3162     0           993      1
		//4      4085603    757163    1929   128013   15172       25523    43
		//5      193690690  35043416  73365  4993637  8392        3309887  30171
		private readonly MoveGenerator target = new MoveGenerator(GameState.FromForsythEdwardsNotation("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -"));

		[TestMethod]
		public void Test_MoveGenerator_GetMoves()
		{
			var actual = target.GetMoves()
				.OrderBy(o => o.From)
				.ThenBy(o => o.To)
				.ToList();

			CollectionAssert.AreEqual(new[]
			{
				new Move(Cell.a1, Cell.b1),
				new Move(Cell.a1, Cell.c1),
				new Move(Cell.a1, Cell.d1),
				new Move(Cell.e1, Cell.c1),
				new Move(Cell.e1, Cell.d1),
				new Move(Cell.e1, Cell.f1),
				new Move(Cell.e1, Cell.g1),
				new Move(Cell.h1, Cell.f1),
				new Move(Cell.h1, Cell.g1),
				new Move(Cell.a2, Cell.a3),
				new Move(Cell.a2, Cell.a4),
				new Move(Cell.b2, Cell.b3),
				new Move(Cell.d2, Cell.c1),
				new Move(Cell.d2, Cell.e3),
				new Move(Cell.d2, Cell.f4),
				new Move(Cell.d2, Cell.g5),
				new Move(Cell.d2, Cell.h6),
				new Move(Cell.e2, Cell.d1),
				new Move(Cell.e2, Cell.f1),
				new Move(Cell.e2, Cell.d3),
				new Move(Cell.e2, Cell.c4),
				new Move(Cell.e2, Cell.b5),
				new Move(Cell.e2, Cell.a6),
				new Move(Cell.g2, Cell.g3),
				new Move(Cell.g2, Cell.h3),
				new Move(Cell.g2, Cell.g4),
				new Move(Cell.c3, Cell.b1),
				new Move(Cell.c3, Cell.d1),
				new Move(Cell.c3, Cell.a4),
				new Move(Cell.c3, Cell.b5),
				new Move(Cell.f3, Cell.d3),
				new Move(Cell.f3, Cell.e3),
				new Move(Cell.f3, Cell.g3),
				new Move(Cell.f3, Cell.h3),
				new Move(Cell.f3, Cell.f4),
				new Move(Cell.f3, Cell.g4),
				new Move(Cell.f3, Cell.f5),
				new Move(Cell.f3, Cell.h5),
				new Move(Cell.f3, Cell.f6),
				new Move(Cell.d5, Cell.d6),
				new Move(Cell.d5, Cell.e6),
				new Move(Cell.e5, Cell.d3),
				new Move(Cell.e5, Cell.c4),
				new Move(Cell.e5, Cell.g4),
				new Move(Cell.e5, Cell.c6),
				new Move(Cell.e5, Cell.g6),
				new Move(Cell.e5, Cell.d7),
				new Move(Cell.e5, Cell.f7),
			}, actual);
		}

		[TestMethod]
		public void Expect_white_pawn_can_make_double_move_from_2th_rank()
		{
			var target = new MoveGenerator(GameState.FromForsythEdwardsNotation("4k3/8/8/8/8/8/P7/4K3 w - -"));

			var moves = target.GetMoves();

			Assert.IsTrue(moves.Contains(new Move(Cell.a2, Cell.a4)));
		}

		[TestMethod]
		public void Expect_black_pawn_can_make_double_move_from_7th_rank()
		{
			var target = new MoveGenerator(GameState.FromForsythEdwardsNotation("4k3/p7/8/8/8/8/8/4K3 b - -"));

			var moves = target.GetMoves();

			Assert.IsTrue(moves.Contains(new Move(Cell.a7, Cell.a5)));
		}

		[TestMethod]
		public void Expect_king_doesnt_move_to_attacked_cell()
		{
			target.MakeMove(new Move(Cell.e5, Cell.g6));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.f8)));
		}

		[TestMethod]
		public void Expect_king_cant_castle_through_check_kingside()
		{
			target.MakeMove(new Move(Cell.e5, Cell.g6));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.g8)));
		}

		[TestMethod]
		public void Expect_king_cant_castle_into_check_kingside()
		{
			target.MakeMove(new Move(Cell.e5, Cell.g4));
			target.MakeMove(new Move(Cell.a8, Cell.b8));
			target.MakeMove(new Move(Cell.g4, Cell.h6));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.g8)));
		}

		[TestMethod]
		public void Expect_king_cant_castle_through_check_queenside()
		{
			target.MakeMove(new Move(Cell.e5, Cell.c6));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.c8)));
		}

		[TestMethod]
		public void Expect_king_cant_castle_into_check_queenside()
		{
			target.MakeMove(new Move(Cell.e2, Cell.a6));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.c8)));
		}

		[TestMethod]
		public void Expect_king_cant_castle_while_in_check()
		{
			target.MakeMove(new Move(Cell.d5, Cell.e6));
			target.MakeMove(new Move(Cell.a6, Cell.b7));
			target.MakeMove(new Move(Cell.e6, Cell.f7));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.c8)));
			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.g8)));
		}

		[TestMethod]
		public void Expect_king_cant_capture_protected_piece()
		{
			target.MakeMove(new Move(Cell.d5, Cell.e6));
			target.MakeMove(new Move(Cell.a6, Cell.b7));
			target.MakeMove(new Move(Cell.e6, Cell.f7));
			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e8, Cell.f7)));
		}

		[TestMethod]
		public void Ensure_moves_in_check_block_eliminate_attacker()
		{
			target.MakeMove(new Move(Cell.e1, Cell.f1));
			target.MakeMove(new Move(Cell.h3, Cell.g2));

			var moves = target.GetMoves();

			//*** Queen on f3 only has one move
			Assert.AreEqual(1, moves.Count(o => o.From == Cell.f3));
			//*** ... to take the pawn on g2
			Assert.AreEqual(Cell.g2, moves.First(o => o.From == Cell.f3).To);
		}

		[TestMethod]
		public void Ensure_pinned_piece_doesnt_expose_king()
		{
			target.MakeMove(new Move(Cell.e1, Cell.f1));
			target.MakeMove(new Move(Cell.a8, Cell.b8));

			var moves = target.GetMoves();

			Assert.IsFalse(moves.Contains(new Move(Cell.e2, Cell.d1)));
		}

		[TestMethod]
		public void Ensure_pinned_piece_can_move_along_pinning_diagonal()
		{
			target.MakeMove(new Move(Cell.e1, Cell.f1));
			target.MakeMove(new Move(Cell.a8, Cell.b8));

			var moves = target.GetMoves();

			Assert.IsTrue(moves.Contains(new Move(Cell.e2, Cell.d3)));
		}

		[TestMethod]
		public void Ensure_pinned_piece_can_capture_the_pinning_piece()
		{
			target.MakeMove(new Move(Cell.e1, Cell.f1));
			target.MakeMove(new Move(Cell.a8, Cell.b8));

			var moves = target.GetMoves();

			Assert.IsTrue(moves.Contains(new Move(Cell.e2, Cell.a6)));
		}

		[TestMethod]
		public void GetMoves_Depth_0_Tests()
		{
			var totalMoves = 0;
			var totalCaptures = 0;
			var totalEnpassants = 0;
			var totalCastles = 0;

			var moves = target.GetMoves();

			totalMoves += moves.Count();
			totalCaptures += moves.Count(o => target.Board[o.To] != Piece.None);
			totalCastles += moves.Count(o => (target.Board[o.From] & Piece.Type) == Piece.King && Math.Abs(o.From.File() - o.To.File()) == 2);
			totalEnpassants += moves.Count(o => (target.Board[o.From] & Piece.Type) == Piece.Pawn && o.To == target.Enpassant);

			Assert.AreEqual(48, totalMoves, "Incorrect # of moves");
			Assert.AreEqual(8, totalCaptures, "Incorrect # of captures");
			Assert.AreEqual(0, totalEnpassants, "Incorrect # of enpassants");
			Assert.AreEqual(2, totalCastles, "Incorrect # of castling");
		}

		[TestMethod]
		public void GetMoves_Depth_1_Tests()
		{
			var totalMoves = 0;
			var totalCaptures = 0;
			var totalEnpassants = 0;
			var totalCastles = 0;

			foreach (var move0 in target.GetMoves())
			{
				var undo0 = target.MakeMove(move0);

				var moves = target.GetMoves();

				totalMoves += moves.Count();
				totalCaptures += moves.Count(o => target.Board[o.To] != Piece.None);
				totalCastles += moves.Count(o => (target.Board[o.From] & Piece.Type) == Piece.King && Math.Abs(o.From.File() - o.To.File()) == 2);
				totalEnpassants += moves.Count(o => (target.Board[o.From] & Piece.Type) == Piece.Pawn && o.To == target.Enpassant);

				target.UndoMove(undo0);
			}

			Assert.AreEqual(2039, totalMoves, "Incorrect # of moves");
			Assert.AreEqual(351, totalCaptures, "Incorrect # of captures");
			Assert.AreEqual(1, totalEnpassants, "Incorrect # of enpassants");
			Assert.AreEqual(91, totalCastles, "Incorrect # of castling");
		}

		[TestMethod]
		public void GetMoves_Depth_2_Tests()
		{
			var totalMoves = 0;
			var totalCaptures = 0;
			var totalEnpassants = 0;
			var totalCastles = 0;
			foreach (var move0 in target.GetMoves())
			{
				//totalMoves++;
				var undo0 = target.MakeMove(move0);
				foreach (var move1 in target.GetMoves())
				{
					var undo1 = target.MakeMove(move1);

					var moves = target.GetMoves();

					totalMoves += moves.Count();
					totalCaptures += moves.Count(o => target.Board[o.To] != Piece.None);
					totalCastles += moves.Count(o => (target.Board[o.From] & Piece.Type) == Piece.King && Math.Abs(o.From.File() - o.To.File()) == 2);
					totalEnpassants += moves.Count(o => (target.Board[o.From] & Piece.Type) == Piece.Pawn && o.To == target.Enpassant);

					target.UndoMove(undo1);
				}
				target.UndoMove(undo0);
			}

			Assert.AreEqual(3162, totalCastles);
			Assert.AreEqual(45, totalEnpassants);
			Assert.AreEqual(17102, totalCaptures);
			Assert.AreEqual(97862, totalMoves);
		}
	}
}
