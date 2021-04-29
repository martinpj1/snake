using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Apple : Item
    {
        public Apple(Position pos) : this(Board.Tiles[pos]) { }
        public Apple(Tile tile) : base(tile)
        {
            Color = ConsoleColor.DarkRed;
            Icon = 'Ö';

            
            CollisionResponses.Add(typeof(SnakePart), Destroy);
        }

        public void Destroy()
        {
            Tile.Item = null;
        }
    }
}
