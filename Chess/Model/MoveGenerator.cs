using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	class MoveGenerator
	{
		Piece[] _board;
		Color _active;
		Castling _castling;
		Cell _enpassant;
		int _drawClock;
		int _move;

		public MoveGenerator(GameState state)
		{
			_board = state.Board.ToArray();
			_active = state.Active;
			_castling = state.Castling;
			_enpassant = state.Enpassant;
			_drawClock = state.DrawClock;
			_move = state.Move;
		}

		public void MakeMove(Move move)
		{
			var from = (int)move.From;
			var to = (int)move.To;
			var mid = (from + to) / 2;

			//*** Move the piece on the board from the 'from' cell to the 'to' cell
			var capture = _board[to];
			var piece = _board[from];
			var pieceType = piece & Piece.Type;

			if (move.Promotion == Piece.None)
			{
				_board[to] = piece;
			}
			else
			{
				_board[to] = (move.Promotion & Piece.Type) | (piece & Piece.Color);
			}

			_board[from] = Piece.None;

			//*** Remove captured pawn in enpassant moves
			if (move.To == _enpassant && pieceType == Piece.Pawn)
			{
				_board[(from & 56) | (to & 7)] = Piece.None;
			}

			//*** If we've castled the king, move the corrisponding rook as well
			if (Math.Abs(from - to) == 2 && pieceType == Piece.King)
			{
				var rook = (int)GetCastlingRookCell(move.To);

				_board[mid] = _board[rook];
				_board[rook] = Piece.None;
			}

			//*** Invalidate appropriate castling once the king or rook move
			if (pieceType == Piece.King)
			{
				if (move.From == Cell.e1)
				{
					_castling &= ~Castling.KQ;
				}
				else if (move.From == Cell.e8)
				{
					_castling &= ~Castling.kq;
				}
			}
			else if (pieceType == Piece.Rook)
			{
				if (move.From == Cell.a1)
				{
					_castling &= ~Castling.Q;
				}
				else if (move.From == Cell.h1)
				{
					_castling &= ~Castling.K;
				}
				else if (move.From == Cell.a8)
				{
					_castling &= ~Castling.q;
				}
				else if (move.From == Cell.h8)
				{
					_castling &= ~Castling.k;
				}
			}

			//*** Increment the move count if black just made a move
			if (_active == Color.Black)
			{
				_move++;
			}

			//*** If the move is a non-capture by a non-pawn piece, then the draw clock is incremented
			//*** otherwise it resets
			if (capture == Piece.None && pieceType != Piece.Pawn)
			{
				_drawClock++;
			}
			else
			{
				_drawClock = 0;
			}

			//*** Update enpassant state if a pawn has made a double move
			if (pieceType == Piece.Pawn && Math.Abs(from - to) == 16)
			{
				_enpassant = (Cell)mid;
			}
			else
			{
				_enpassant = Cell.None;
			}

			//*** Update the active color state
			_active = _active != Color.White ? Color.White : Color.Black;
		}

		private static Cell GetCastlingRookCell(Cell kingTo)
		{
			switch (kingTo)
			{
				case Cell.g1: return Cell.h1;
				case Cell.g8: return Cell.h8;
				case Cell.c1: return Cell.a1;
				case Cell.c8: return Cell.a8;
			}

			throw new InvalidOperationException("Invalid castling move");
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

		public  GameState ToState()
		{
			return new GameState(_board, _active, _castling, _enpassant, _drawClock, _move);
		}

		private IEnumerable<Cell> CellMoves(Cell cell)
		{
			var piece = _board[(int)cell];
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

		private IEnumerable<Cell> PawnMoves(Cell cell, Direction forward)
		{
			var to = cell + forward;
			if (_board[(int)to] == Piece.None)
			{
				yield return to;

				to += forward;
				if (cell.ToRank() == 1 && _board[(int)to] == Piece.None)
				{
					yield return to;
				}
			}

			Piece capture;
			to = cell + (forward + Direction.Left);
			if (to != Cell.None && (to == _enpassant || ((capture = _board[(int)to]) != Piece.None && capture.ToColor() != _active)))
			{
				//*** Allowed to capture an opponent's piece on the board, or the enpassant square
				yield return to;
			}

			to = cell + (forward + Direction.Right);
			if (to != Cell.None && (to == _enpassant || ((capture = _board[(int)to]) != Piece.None && capture.ToColor() != _active)))
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
					var capture = _board[(int)to];
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
	}
}
