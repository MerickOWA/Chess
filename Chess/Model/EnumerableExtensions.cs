using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
	internal static class EnumerableExtensions
	{
		public static TValue? GetNullable<TKey,TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? (TValue?)value : null;
		}
	}
}
