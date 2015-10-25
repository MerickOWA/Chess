using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	class MoveGenerator
	{
		Board _board;
		Color _active;
		Castling _castling;
		Cell _enpassant;
		int _drawClock;
		int _move;

		static readonly IDictionary<Piece, Func<Cell, Board, IEnumerable<Cell>>> pieceMoveGenerator = new Dictionary<Piece, Func<Cell, Board, IEnumerable<Cell>>>
		{
			{ Piece.WhitePawn, WhitePawnMoves },
			{ Piece.BlackPawn, BlackPawnMoves },
			{ Piece.WhiteKnight, KnightMoves },
			{ Piece.BlackKnight, KnightMoves },
			{ Piece.WhiteBishop, BishopMoves },
			{ Piece.BlackBishop, BishopMoves },
			{ Piece.WhiteRook, RookMoves },
			{ Piece.BlackRook, RookMoves },
			{ Piece.WhiteQueen, QueenMoves },
			{ Piece.BlackQueen, QueenMoves },
			{ Piece.WhiteKing, KingMoves },
			{ Piece.BlackKing, KingMoves },
		};

		static readonly IEnumerable<Tuple<int, int>> rookDirections = new[]
		{
			Tuple.Create(0, -1), //down
			Tuple.Create(-1, 0), //left
			Tuple.Create(1, 0),  //right
			Tuple.Create(0, 1),  //up
		};

		static readonly IEnumerable<Tuple<int, int>> bishopDirections = new[]
		{
			Tuple.Create(-1,-1), // down-left
			Tuple.Create(1, -1), // down-right
			Tuple.Create(-1, 1), // up-left
			Tuple.Create(1, 1),  // up-right
		};

		static readonly IEnumerable<Tuple<int, int>> queenDirections = rookDirections.Concat(bishopDirections);

		static readonly IEnumerable<Tuple<int, int>> knightDirections = new[]
		{
			Tuple.Create(-1, -2), //down-down-left,
			Tuple.Create(1, -2),  //down-down-right,
			Tuple.Create(-2, -1), //down-left-left
			Tuple.Create(2, -1),  //down-right-right
			Tuple.Create(-2, 1),  //up-left-left
			Tuple.Create(2, 1),   //up-right-right
			Tuple.Create(-1, 2),  //up-up-left
			Tuple.Create(1, 2),   //up-up-right
		};

		private static IEnumerable<Cell> WhitePawnMoves(Cell cell, Board board)
		{
			yield break;
		}

		private static IEnumerable<Cell> BlackPawnMoves(Cell cell, Board board)
		{
			yield break;
		}

		private static IEnumerable<Cell> KnightMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, knightDirections, limited: true);
		}

		private static IEnumerable<Cell> BishopMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, bishopDirections, limited: false);
		}

		private static IEnumerable<Cell> RookMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, rookDirections, limited: false);
		}

		private static IEnumerable<Cell> QueenMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, queenDirections, limited: false);
		}

		private static IEnumerable<Cell> KingMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, queenDirections, limited: true);
		}

		private static IEnumerable<Cell> SlidingMoves(Cell from, Board board, IEnumerable<Tuple<int, int>> directions, bool limited)
		{
			var color = board[from].ToColor();

			//*** Generate moves in every direction
			foreach (var direction in directions)
			{
				var to = from.Move(direction);

				while (to != Cell.None)
				{
					var capture = board[to];
					if (capture == Piece.None)
					{
						//*** Allowed to move to a cell if there is no piece there
						yield return to;
					}
					else
					{
						if (capture.ToColor() != color)
						{
							//*** Allowed to capture a piece if its not my color
							yield return to;
						}

						//*** Stop moving in this direction once we've hit another piece
						break;
					}

					if (limited)
					{
						//*** Limited movement pieces can only move in a given direction once
						break;
					}

					//*** Move to the next cell along the "direction"
					to = to.Move(direction);
				}
			}
		}

		public MoveGenerator(GameState state)
		{
			_board = state.Board;
			_active = state.Active;
			_castling = state.Castling;
			_enpassant = state.Enpassant;
			_drawClock = state.DrawClock;
			_move = state.Move;
		}

		public void MakeMove(Move move)
		{
			throw new NotImplementedException();
		}

		public void UndoMove(Move move)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Move> GetMoves()
		{
			return
				from cell in BoardCells()
				let piece = _board[cell]
				where piece != Piece.None && piece.ToColor() == _active
				let generator = pieceMoveGenerator[piece]
				from move in generator(cell, _board)
				select new Move(cell, move);
		}

		private static IEnumerable<Cell> BoardCells()
		{
			for(Cell i = Cell.a1; i <= Cell.h8; i++)
			{
				yield return i;
			}
		}

		public static implicit operator GameState(MoveGenerator obj)
		{
			throw new NotImplementedException();
		}
	}
}
