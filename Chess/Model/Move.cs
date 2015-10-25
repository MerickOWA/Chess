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

		public override string ToString()
		{
			var chars = new char[4];

			return new string(new[]
			{
				this.From.ToFileChar(),
				this.From.ToRankChar(),
				this.To.ToFileChar(),
				this.To.ToRankChar(),
			});
		}
	}
}
