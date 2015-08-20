using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	class BiDictionary<TFirst,TSecond> : IEnumerable
	{
		Dictionary<TFirst, TSecond> _toSecond = new Dictionary<TFirst, TSecond>();
		Dictionary<TSecond, TFirst> _toFirst = new Dictionary<TSecond, TFirst>();

		public void Add(TFirst first, TSecond second)
		{
			_toSecond.Add(first, second);
			_toFirst.Add(second, first);
		}

		public bool ContainsFirst(TFirst first)
		{
			return _toSecond.ContainsKey(first);
		}

		public bool ContainsSecond(TSecond second)
		{
			return _toFirst.ContainsKey(second);
		}

		public bool TryGetSecond(TFirst first, out TSecond second)
		{
			return _toSecond.TryGetValue(first, out second);
		}

		public bool TryGetFirst(TSecond second, out TFirst first)
		{
			return _toFirst.TryGetValue(second, out first);
		}

		public TSecond GetSecond(TFirst key, TSecond notFound = default(TSecond))
		{
			TSecond found;
			return TryGetSecond(key, out found) ? found : notFound;
		}

		public TFirst GetFirst(TSecond key, TFirst notFound = default(TFirst))
		{
			TFirst found;
			return TryGetFirst(key, out found) ? found : notFound;
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
