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
		public void GetMoves_Depth_1_Tests()
		{
			var moves = target.GetMoves();

			var totalMoves = 0;
			foreach (var move in moves)
			{
				var undo = target.MakeMove(move);
				totalMoves += target.GetMoves().Count() + 1;
				target.UndoMove(undo);
			}

			Assert.AreEqual(2039, totalMoves);
		}
	}
}
