using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Utils
    {

    }

    public class Position
    {

        static Dictionary<int, Dictionary<int, Position>> Positions = new Dictionary<int, Dictionary<int, Position>>();

        Position(int x, int y)
        {
            X = x;
            Y = y;
            Positions.GetOrCreate(X)[Y] = this;
        }

        public static Position Create(int x, int y)
        {
            return new Position(x, y);
        }

        public static Position From(int x, int y)
        {
            return Positions[x][y];
        }

        public int X;
        public int Y;

        public static Position operator +(Position a, Position b)
        {
            return From(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator -(Position a, Position b)
        {
            return From(a.X - b.X, a.Y - b.Y);
        }
    }

    public static class DictionaryExt
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
          where TValue : new()
        {
            TValue val;

            if (!dict.TryGetValue(key, out val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }
    }
}
