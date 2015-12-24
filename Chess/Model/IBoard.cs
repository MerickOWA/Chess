namespace Chess.Model
{
	interface IBoard
	{
		Piece this[Cell cell] { get; }
	}
}