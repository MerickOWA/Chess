using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	class GameState
	{
		private readonly static BiDictionary<Piece, char> _PieceChar = new BiDictionary<Piece, char>
		{
			{ Piece.WhitePawn,   'P' },
			{ Piece.WhiteKnight, 'N' },
			{ Piece.WhiteBishop, 'B' },
			{ Piece.WhiteRook,   'R' },
			{ Piece.WhiteQueen,  'Q' },
			{ Piece.WhiteKing,   'K' },
			{ Piece.BlackPawn,   'p' },
			{ Piece.BlackKnight, 'n' },
			{ Piece.BlackBishop, 'b' },
			{ Piece.BlackRook,   'r' },
			{ Piece.BlackQueen,  'q' },
			{ Piece.BlackKing,   'k' },
		};

		private static readonly BiDictionary<Color, char> _ColorChar = new BiDictionary<Color, char>
		{
			{ Color.White, 'w' },
			{ Color.Black, 'b' },
		};

		private static readonly BiDictionary<Castling, char> _CastlingChar = new BiDictionary<Castling, char>
		{
			{ Castling.q, 'q' },
			{ Castling.k, 'k' },
			{ Castling.Q, 'Q' },
			{ Castling.K, 'K' },
		};

		public GameState(Piece[] pieces, Color active, Castling castling, Cell enpassant, int draw, int move)
		{
			Board = new Board(pieces);
			Active = active;
			Castling = castling;
			Enpassant = enpassant;
			DrawClock = draw;
			Move = move;
		}

		public Board Board { get; }
		public Color Active { get; }
		public Castling Castling { get; }
		public Cell Enpassant { get; }
		public int DrawClock { get; }
		public int Move { get; }

		public static GameState FromForsythEdwardsNotation(string state)
		{
			int i = 0;
			var board = ParseBoard(state, ref i);
			var active = ParseActive(state, ref i);
			var castling = ParseCastling(state, ref i);
			var enpassant = ParseEnpassant(state, ref i);
			var draw = ParseDrawClock(state, ref i);
			var move = ParseMove(state, ref i);

			return new GameState(board, active, castling, enpassant, draw, move);
		}

		private static Piece[] ParseBoard(string state, ref int i)
		{
			var retval = new Piece[64];

			var cell = 64 - 8;
			char c;
			while ((c = state[i]) != ' ')
			{
				if (c >= '1' && c <= '8')
				{
					i++;
					for (var j = c - '0'; j > 0; j--)
					{
						retval[cell++] = Piece.None;
					}
				}
				else if (c == '/')
				{
					i++;

					if (cell % 8 != 0)
					{
						throw new ArgumentException();
					}

					cell -= 16;
				}
				else if (_PieceChar.TryGetFirst(c, out retval[cell]))
				{
					i++;
					cell++;
				}
				else
				{
					throw new ArgumentException();
				}
			}
			i++;

			return retval;
		}

		private static Color ParseActive(string state, ref int i)
		{
			Color retval;

			if (!_ColorChar.TryGetFirst(state[i], out retval))
			{
				throw new ArgumentException();
			}
			i++;

			if (state[i] != ' ')
			{
				throw new ArgumentException();
			}
			i++;

			return retval;
		}

		private static Castling ParseCastling(string state, ref int i)
		{
			var retval = Castling.None;

			if (state[i] != '-')
			{
				while (state[i] != ' ')
				{
					Castling val;
					if (!_CastlingChar.TryGetFirst(state[i], out val))
					{
						throw new ArgumentException();
					}
					i++;

					retval |= val;
				}
				i++;
			}
			else
			{
				i++;

				if (state[i] != ' ')
				{
					throw new ArgumentException();
				}
				i++;
			}

			return retval;
		}

		private static Cell ParseEnpassant(string state, ref int i)
		{
			Cell retval;

			if (state[i] == '-')
			{
				i++;
				retval = Cell.None;
			}
			else if (state[i] >= 'a' && state[i] <= 'h')
			{
				var file = state[i] - 'a';
				i++;

				if (state[i] != '3' && state[i] != '6')
				{
					throw new ArgumentException();
				}

				var rank = state[i] - '1';
				i++;

				retval = (Cell)(8 * rank + file);
			}
			else
			{
				throw new ArgumentException();
			}

			if (state[i] != ' ')
			{
				throw new ArgumentException();
			}
			i++;

			return retval;
		}

		private static int ParseDrawClock(string state, ref int i)
		{
			int retval = 0;

			while (state[i] >= '0' && state[i] <= '9')
			{
				retval = 10 * retval + (state[i] - '0');
				i++;
			}

			if (state[i] != ' ')
			{
				throw new ArgumentException();
			}
			i++;

			return retval;
		}

		private static int ParseMove(string state, ref int i)
		{
			int retval = 0;

			while (i < state.Length)
			{
				if (state[i] < '0' || state[i] > '9')
				{
					throw new ArgumentException();
				}

				retval = 10 * retval + (state[i] - '0');
				i++;
			}

			return retval;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			AppendBoard(sb);
			AppendActive(sb);
			AppendCastling(sb);
			AppendEnpassant(sb);
			AppendDrawClock(sb);
			AppendMove(sb);

			return sb.ToString();
		}

		private void AppendBoard(StringBuilder sb)
		{
			char c;

			for (var rank = 7; rank >= 0; rank--)
			{
				var blank = 0;
				for (var file = 0; file < 8; file++)
				{
					var piece = Board[file, rank];
					if (piece == Piece.None)
					{
						blank++;
						continue;
					}

					if (blank > 0)
					{
						sb.Append((char)('0' + blank));
						blank = 0;
					}

					if (!_PieceChar.TryGetSecond(piece, out c))
					{
						throw new InvalidOperationException();
					}

					sb.Append(c);
				}

				if (blank > 0)
				{
					sb.Append((char)('0' + blank));
				}

				if (rank > 0)
				{
					sb.Append('/');
				}
			}

			sb.Append(' ');
		}

		private void AppendActive(StringBuilder sb)
		{
			char c;
			if (!_ColorChar.TryGetSecond(Active, out c))
			{
				throw new InvalidOperationException();
			}

			sb.Append(c);
			sb.Append(' ');
		}

		private void AppendCastling(StringBuilder sb)
		{
			if (Castling != Castling.None)
			{
				if ((Castling & Castling.K) != 0) sb.Append('K');
				if ((Castling & Castling.Q) != 0) sb.Append('Q');
				if ((Castling & Castling.k) != 0) sb.Append('k');
				if ((Castling & Castling.q) != 0) sb.Append('q');
			}
			else
			{
				sb.Append('-');
			}

			sb.Append(' ');
		}

		private void AppendEnpassant(StringBuilder sb)
		{
			if (Enpassant != Cell.None)
			{
				sb.Append((char)('a' + ((int)Enpassant % 8)));
				sb.Append((char)('1' + ((int)Enpassant / 8)));
			}
			else
			{
				sb.Append('-');
			}

			sb.Append(' ');
		}

		private void AppendDrawClock(StringBuilder sb)
		{
			sb.Append(DrawClock);
			sb.Append(' ');
		}

		private void AppendMove(StringBuilder sb)
		{
			sb.Append(Move);
		}
	}
}
