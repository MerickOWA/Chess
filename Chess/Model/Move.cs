using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	struct Move
	{
		public Move(Cell from, Cell to, Piece promotion = Piece.None)
		{
			From = from;
			To = to;
			Promotion = promotion;
		}

		public Cell From { get; }

		public Cell To { get; }

		public Piece Promotion { get; }

		public override string ToString()
		{
			var chars = new char[Promotion != Piece.None ? 5 : 4];

			chars[0] = From.ToFileChar();
			chars[1] = From.ToRankChar();
			chars[2] = To.ToFileChar();
			chars[3] = To.ToRankChar();

			if (Promotion != Piece.None)
			{
				chars[4] = Promotion.ToChar();
			}

			return new string(chars);
		}
	}
}
