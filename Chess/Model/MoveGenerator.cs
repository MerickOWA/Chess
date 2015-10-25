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
			throw new NotImplementedException();
		}

		public void UndoMove(Move move)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Move> GetMoves()
		{
			throw new NotImplementedException();
		}

		public static implicit operator GameState(MoveGenerator obj)
		{
			throw new NotImplementedException();
		}
	}
}
