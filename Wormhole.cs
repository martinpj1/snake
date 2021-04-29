using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Wormhole : Item
    {
        public Wormhole(Tile tile) : base(tile)
        {
            Color = ConsoleColor.Magenta;
            Icon = 'Θ';
        }
    }
}
