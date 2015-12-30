﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	internal class MoveGenerator
	{
		InternalBoard _board;
		Piece _active;
		Castling _castling;
		Cell _enpassant;
		int _drawClock;
		int _move;

		public MoveGenerator(GameState state)
		{
			_board = new InternalBoard(state.Board);
			_active = state.Active;
			_castling = state.Castling;
			_enpassant = state.Enpassant;
			_drawClock = state.DrawClock;
			_move = state.Move;
		}

		public IBoard Board => _board;
		public Piece Active => _active;
		public Castling Castling => _castling;
		public Cell Enpassant => _enpassant;
		public int DrawClock => _drawClock;
		public int Move => _move;

		public UndoMove MakeMove(Move move)
		{
			var from = (int)move.From;
			var to = (int)move.To;

			//*** Move the piece on the board from the 'from' cell to the 'to' cell
			var capture = _board[move.To];
			var piece = _board[move.From];
			var pieceType = piece & Piece.Type;

			var retval = new UndoMove(move.From, move.To, move.Promotion, capture, _castling, _enpassant, _drawClock);

			if (move.Promotion != Piece.None)
			{
				piece = _active | move.Promotion;
			}

			_board[to] = piece;
			_board[from] = Piece.None;

			//*** Remove captured pawn in enpassant moves
			if (move.To == _enpassant && pieceType == Piece.Pawn)
			{
				_board[(from & 56) | (to & 7)] = Piece.None;
			}

			//*** If we've castled the king, move the corrisponding rook as well
			if (Math.Abs(from - to) == 2 && pieceType == Piece.King)
			{
				var rook = GetCastlingRookCell(move.To);

				_board[(from + to) / 2] = _board[rook];
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
			if (_active == Piece.Black)
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
				_enpassant = (Cell)((from + to) / 2);
			}
			else
			{
				_enpassant = Cell.None;
			}

			//*** Update the active color state
			_active ^= Piece.Color;

			return retval;
		}

		public void UndoMove(UndoMove move)
		{
			var from = (int)move.From;
			var to = (int)move.To;

			var piece = _board[to];

			if (move.Promotion != Piece.None)
			{
				piece = (piece & Piece.Color) | Piece.Pawn;
			}

			var pieceType = piece & Piece.Type;

			_board[from] = piece;
			_board[to] = move.Capture;

			if (move.To == move.Enpassant && pieceType == Piece.Pawn)
			{
				_board[(from & 56) | (to & 7)] = Piece.Pawn | _active;
			}

			if (Math.Abs(from - to) == 2 && pieceType == Piece.King)
			{
				var mid = (from + to) / 2;
				var rook = GetCastlingRookCell(move.To);
				_board[rook] = _board[mid];
				_board[mid] = Piece.None;
			}

			_active ^= Piece.Color;
			_enpassant = move.Enpassant;
			_castling = move.Castling;
			_drawClock = move.DrawClock;
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

		public IEnumerable<Move> GetMoves()
		{
			Func<Cell, bool> isOccupied = cell => _board[cell] != Piece.None;

			//*** Generate all movements of opponent pieces which could capture the current player's king
			//*** NOTE: these are not ordinary moves, as the opponent can't "capture" their own pieces
			//***       however, squares which contain an opponents piece are still "protected" from capture by
			//***       the active player's king due to other pieces of the opponent "attacking" the square
			//***       containing their own piece
			//*** NOTE: only pawn "attack" moves are only generated by passing a "fully" occupied board state
			//***       thus generating moves regardless of the actual occupied state of the neighboring cells
			//***       and preventing the cells immediately infront of the pawn from being considered as "attacked"
			var attacks =
				from src in _board
				let piece = _board[src]
				where (piece & Piece.Color) == (_active ^ Piece.Color)
				from dst in CellMoves(src, piece, (piece & Piece.Type) != Piece.Pawn ? isOccupied : _ => true)
				select new Move(src, dst);

			var kingCell = _board.First(cell => _board[cell] == (Piece.King | _active));

			var checks = attacks.Where(o => o.To == kingCell);
			var attacked = new HashSet<Cell>(attacks.Select(o => o.To));

			var promotionRank = _active == Piece.White ? 0 : 7;

			foreach (var from in _board)
			{
				var piece = _board[from];
				if ((piece & Piece.Color) != _active) continue; // skip empty & opponent squares

				var pieceType = piece & Piece.Type;

				foreach (var to in CellMoves(from, piece, isOccupied))
				{
					if ((_board[to] & Piece.Color) == _active) continue; // skip captures of player's own pieces

					if (pieceType == Piece.King)
					{
						//*** Ensure king doesn't move to sqaure where he could be captured
						if (!attacked.Contains(to))
						{
							yield return new Move(from, to);
						}
					}
					else if (pieceType == Piece.Pawn)
					{
						if (to.ToRank() == promotionRank)
						{
							yield return new Move(from, to, Piece.Queen);
							yield return new Move(from, to, Piece.Rook);
							yield return new Move(from, to, Piece.Bishop);
							yield return new Move(from, to, Piece.Knight);
						}
						else
						{
							yield return new Move(from, to);
						}
					}
					else
					{
						yield return new Move(from, to);
					}
				}

				if (pieceType == Piece.King && !attacked.Contains(from))
				{
					var queenSide = _active == Piece.White ? Castling.Q : Castling.q;
					var to = from + Direction.LeftLeft;
					if ((_castling & queenSide) == queenSide && !attacked.Contains(from + Direction.Left) && !attacked.Contains(to))
					{
						yield return new Move(from, to);
					}

					var kingSide = _active == Piece.White ? Castling.K : Castling.k;
					to = from + Direction.RightRight;
					if ((_castling & kingSide) == kingSide && !attacked.Contains(from + Direction.Right) && !attacked.Contains(to))
					{
						yield return new Move(from, to);
					}
				}
			}

			//*** Generate enpassant moves if available
			if (_enpassant != Cell.None)
			{
				var backward = _active == Piece.White ? Direction.Down : Direction.Up;
				var playerPawn = _active | Piece.Pawn;

				var from = _enpassant + (backward + Direction.Left);
				if (from != Cell.None && _board[from] == playerPawn)
				{
					yield return new Move(from, _enpassant);
				}

				from = _enpassant + (backward + Direction.Right);
				if (from != Cell.None && _board[from] == playerPawn)
				{
					yield return new Move(from, _enpassant);
				}
			}
		}

		private static IEnumerable<Cell> CellMoves(Cell cell, Piece piece, Func<Cell,bool> isOccupied)
		{
			switch (piece)
			{
				case Piece.WhitePawn:
					return PawnMoves(cell, Direction.Up, isOccupied);

				case Piece.BlackPawn:
					return PawnMoves(cell, Direction.Down, isOccupied);

				case Piece.WhiteKnight:
				case Piece.BlackKnight:
					return KnightMoves(cell, isOccupied);

				case Piece.WhiteBishop:
				case Piece.BlackBishop:
					return BishopMoves(cell, isOccupied);

				case Piece.WhiteRook:
				case Piece.BlackRook:
					return RookMoves(cell, isOccupied);

				case Piece.WhiteQueen:
				case Piece.BlackQueen:
					return QueenMoves(cell, isOccupied);

				case Piece.WhiteKing:
				case Piece.BlackKing:
					return KingMoves(cell, isOccupied);

				default:
					throw new InvalidOperationException("Can't generate moves for unknown piece");
			}
		}

		private static IEnumerable<Cell> PawnMoves(Cell cell, Direction forward, Func<Cell, bool> isOccupied)
		{
			var to = cell + forward;
			if (!isOccupied(to))
			{
				yield return to;

				to += forward;
				if (cell.ToRank() == 1 && !isOccupied(to))
				{
					yield return to;
				}
			}

			to = cell + (forward + Direction.Left);
			if (to != Cell.None && isOccupied(to))
			{
				yield return to;
			}

			to = cell + (forward + Direction.Right);
			if (to != Cell.None && isOccupied(to))
			{
				yield return to;
			}
		}

		private static IEnumerable<Cell> KnightMoves(Cell cell, Func<Cell, bool> isOccupied)
		{
			return SlidingMoves(cell, Direction.Knight, isOccupied, limited: true);
		}

		private static IEnumerable<Cell> BishopMoves(Cell cell, Func<Cell, bool> isOccupied)
		{
			return SlidingMoves(cell, Direction.Bishop, isOccupied, limited: false);
		}

		private static IEnumerable<Cell> RookMoves(Cell cell, Func<Cell, bool> isOccupied)
		{
			return SlidingMoves(cell, Direction.Rook, isOccupied, limited: false);
		}

		private static IEnumerable<Cell> QueenMoves(Cell cell, Func<Cell,bool> isOccupied)
		{
			return SlidingMoves(cell, Direction.Queen, isOccupied, limited: false);
		}

		private static IEnumerable<Cell> KingMoves(Cell cell, Func<Cell, bool> isOccupied)
		{
			return SlidingMoves(cell, Direction.Queen, isOccupied, limited: true);
		}

		private static IEnumerable<Cell> SlidingMoves(Cell from, IEnumerable<Direction> directions, Func<Cell, bool> isOccupied, bool limited)
		{
			//*** Generate moves in every direction
			foreach (var direction in directions)
			{
				var to = from + direction;

				while (to != Cell.None)
				{
					if (!isOccupied(to))
					{
						//*** Allowed to move to a cell if there is no piece there
						yield return to;
					}
					else
					{
						//*** Allowed to capture an opponent's piece
						yield return to;

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

		private struct InternalBoard : IBoard, IEnumerable<Cell>
		{
			private Piece[] _pieces;

			public InternalBoard(Board board)
			{
				_pieces = board.ToArray();
			}

			public Piece this[int cell]
			{
				get { return _pieces[cell]; }
				set { _pieces[cell] = value; }
			}

			public Piece this[Cell cell]
			{
				get { return _pieces[(int)cell]; }
				set { _pieces[(int)cell] = value; }
			}

			public IEnumerator<Cell> GetEnumerator()
			{
				for(Cell i = Cell.a1; i <= Cell.h8; i++)
				{
					yield return i;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<Cell>)this).GetEnumerator();
			}
		}
	}
}
