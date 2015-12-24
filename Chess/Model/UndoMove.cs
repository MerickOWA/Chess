namespace Chess.Model
{
	struct UndoMove
	{
		public UndoMove(Cell from, Cell to, Piece promotion, Piece capture, Castling castling, Cell enpassant, int drawClock)
		{
			From = from;
			To = to;
			Promotion = promotion;
			Capture = capture;
			Castling = castling;
			Enpassant = enpassant;
			DrawClock = drawClock;
		}

		public Cell From { get; }
		public Cell To { get; }
		public Piece Promotion { get; }
		public Piece Capture { get; }
		public Castling Castling { get; }
		public Cell Enpassant { get; }
		public int DrawClock { get; }
	}
}