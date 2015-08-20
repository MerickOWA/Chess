using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	[Flags]
	enum Castling
	{
		None,
		q,
		k,
		kq = k | q,
		Q,
		Qq = Q | q,
		Qk = Q | k,
		Qkq = Q | k | q,
		K,
		Kq = K | q,
		Kk = K | k,
		Kkq = K | k | q,
		KQ = K | Q,
		KQq = K | Q | q,
		KQk = K | Q | k,
		KQkq = K | Q | k | q,
	}
}
