using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	class MoveGenerator
	{
		GameState _state;

		public MoveGenerator(GameState state)
		{
			throw new NotImplementedException();
		}

		public void Apply(Move move)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Move> Get()
		{
			throw new NotImplementedException();
		}

		public static implicit operator GameState(MoveGenerator obj)
		{
			throw new NotImplementedException();
		}
	}
}
