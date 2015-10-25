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

		private static IEnumerable<Cell> WhitePawnMoves(Cell cell, Board board)
		{
			return PawnMoves(cell, board, Direction.Up);
		}

		private static IEnumerable<Cell> BlackPawnMoves(Cell cell, Board board)
		{
			return PawnMoves(cell, board, Direction.Down);
		}

		private static IEnumerable<Cell> PawnMoves(Cell cell, Board board, Direction forward)
		{
			var mycolor = board[cell].ToColor();
			var to = cell + forward;
			if (board[to] == Piece.None)
			{
				yield return to;

				to += forward;
				if (cell.ToRank() == 1 && board[to] == Piece.None)
				{
					yield return to;
				}
			}

			Piece capture;
			to = cell + (forward + Direction.Left);
			if (to != Cell.None && (capture = board[to]) != Piece.None && capture.ToColor() != mycolor)
			{
				yield return to;
			}

			to = cell + (forward + Direction.Right);
			if (to != Cell.None && (capture = board[to]) != Piece.None && capture.ToColor() != mycolor)
			{
				yield return to;
			}
		}

		private static IEnumerable<Cell> KnightMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, Direction.Knight, limited: true);
		}

		private static IEnumerable<Cell> BishopMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, Direction.Bishop, limited: false);
		}

		private static IEnumerable<Cell> RookMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, Direction.Rook, limited: false);
		}

		private static IEnumerable<Cell> QueenMoves(Cell cell, Board board)
		{
			return SlidingMoves(cell, board, Direction.Queen, limited: false);
		}

		private static IEnumerable<Cell> KingMoves(Cell cell, Board board)
		{
			foreach (var move in SlidingMoves(cell, board, Direction.Queen, limited: true))
			{
				yield return move;
			}
		}

		private static IEnumerable<Cell> SlidingMoves(Cell from, Board board, IEnumerable<Direction> directions, bool limited)
		{
			var color = board[from].ToColor();

			//*** Generate moves in every direction
			foreach (var direction in directions)
			{
				var to = from + direction;

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
					to = to + direction;
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
