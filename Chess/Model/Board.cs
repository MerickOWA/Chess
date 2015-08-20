using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	struct Board
	{
		private Piece[] _pieces;

		public Board(Piece[] pieces)
		{
			_pieces = pieces;
		}

		public Piece this[Cell cell] => _pieces[(int)cell];
		public Piece this[int file, int rank] => _pieces[8*rank + file];
	}
}
