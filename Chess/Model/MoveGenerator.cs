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

		public MoveGenerator(GameState state)
		{
			_board = state.Board;
			_active = state.Active;
			_castling = state.Castling;
			_enpassant = state.Enpassant;
			_drawClock = state.DrawClock;
			_move = state.Move;
		}

		private IEnumerable<Cell> CellMoves(Cell cell)
		{
			var piece = _board[cell];
			if (piece == Piece.None || piece.ToColor() != _active)
			{
				//*** No moves for an empty cell or an opponent's piece
				return Enumerable.Empty<Cell>();
			}

			switch (piece)
			{
				case Piece.WhitePawn:
					return PawnMoves(cell, Direction.Up);

				case Piece.BlackPawn:
					return PawnMoves(cell, Direction.Down);

				case Piece.WhiteKnight:
				case Piece.BlackKnight:
					return KnightMoves(cell);

				case Piece.WhiteBishop:
				case Piece.BlackBishop:
					return BishopMoves(cell);

				case Piece.WhiteRook:
				case Piece.BlackRook:
					return RookMoves(cell);

				case Piece.WhiteQueen:
				case Piece.BlackQueen:
					return QueenMoves(cell);

				case Piece.WhiteKing:
				case Piece.BlackKing:
					return KingMoves(cell);

				default:
					throw new InvalidOperationException("Can't generate moves for unknown piece");
			}
		}

		private IEnumerable<Cell> WhitePawnMoves(Cell cell)
		{
			return PawnMoves(cell, Direction.Up);
		}

		private IEnumerable<Cell> BlackPawnMoves(Cell cell)
		{
			return PawnMoves(cell, Direction.Down);
		}

		private IEnumerable<Cell> PawnMoves(Cell cell, Direction forward)
		{
			var to = cell + forward;
			if (_board[to] == Piece.None)
			{
				yield return to;

				to += forward;
				if (cell.ToRank() == 1 && _board[to] == Piece.None)
				{
					yield return to;
				}
			}

			Piece capture;
			to = cell + (forward + Direction.Left);
			if (to != Cell.None && (to == _enpassant || ((capture = _board[to]) != Piece.None && capture.ToColor() != _active)))
			{
				//*** Allowed to capture an opponent's piece on the board, or the enpassant square
				yield return to;
			}

			to = cell + (forward + Direction.Right);
			if (to != Cell.None && (to == _enpassant || ((capture = _board[to]) != Piece.None && capture.ToColor() != _active)))
			{
				//*** Allowed to capture an opponent's piece on the board, or the enpassant square
				yield return to;
			}
		}

		private IEnumerable<Cell> KnightMoves(Cell cell)
		{
			return SlidingMoves(cell, Direction.Knight, limited: true);
		}

		private IEnumerable<Cell> BishopMoves(Cell cell)
		{
			return SlidingMoves(cell, Direction.Bishop, limited: false);
		}

		private IEnumerable<Cell> RookMoves(Cell cell)
		{
			return SlidingMoves(cell, Direction.Rook, limited: false);
		}

		private IEnumerable<Cell> QueenMoves(Cell cell)
		{
			return SlidingMoves(cell, Direction.Queen, limited: false);
		}

		private IEnumerable<Cell> KingMoves(Cell cell)
		{
			foreach (var move in SlidingMoves(cell, Direction.Queen, limited: true))
			{
				yield return move;
			}

			var queenSide = _active == Color.White ? Castling.Q : Castling.q;
			if ((_castling & queenSide) == queenSide)
			{
				yield return cell + Direction.LeftLeft;
			}

			var kingSide = _active == Color.White ? Castling.K : Castling.k;
			if ((_castling & kingSide) == kingSide)
			{
				yield return cell + Direction.RightRight;
			}
		}

		private IEnumerable<Cell> SlidingMoves(Cell from, IEnumerable<Direction> directions, bool limited)
		{
			//*** Generate moves in every direction
			foreach (var direction in directions)
			{
				var to = from + direction;

				while (to != Cell.None)
				{
					var capture = _board[to];
					if (capture == Piece.None)
					{
						//*** Allowed to move to a cell if there is no piece there
						yield return to;
					}
					else
					{
						if (capture.ToColor() != _active)
						{
							//*** Allowed to capture an opponent's piece
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
				from move in CellMoves(cell)
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
