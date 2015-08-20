using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	struct Move
	{
		public Move(Cell from, Cell to)
		{
			From = from;
			To = to;
		}

		public Cell From { get; }

		public Cell To { get; }
	}
}
