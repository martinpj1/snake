using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Snake.Program;
using XInputDotNetPure;

namespace Snake
{
    public static class Board
    {
        const int X_OFF = 40;
        const int Y_OFF = 10; 
        const int WIDTH = 32;
        const int HEIGHT = 32;

        static Dictionary<PlayerIndex, Position> ScoreBoardPositions = new Dictionary<PlayerIndex, Position>
        {
            {PlayerIndex.One, Position.Create(X_OFF - 10, Y_OFF + (HEIGHT / 4)) },
            {PlayerIndex.Two, Position.Create(X_OFF + WIDTH + 5, Y_OFF + (3 * HEIGHT / 4)) },
            {PlayerIndex.Three, Position.Create(X_OFF + WIDTH + 5, Y_OFF + (HEIGHT / 4)) },
            {PlayerIndex.Four, Position.Create(X_OFF - 10, Y_OFF + (3 * HEIGHT / 4)) }
        };

        static Random Rand = new Random();

        public static Dictionary<Position, Tile> Tiles = new Dictionary<Position, Tile>();

        static Board()
        {
            //populate all tiles
            for (int i = X_OFF; i < X_OFF + WIDTH; i++)
                for (int j = Y_OFF; j < Y_OFF + HEIGHT; j++)
                {
                    var pos = Position.Create(i, j);
                    Tiles[pos] = new Tile(pos);
                }
        }

        public static void BuildBorder()
        {
            //Top Row 0-Width+1,0      
            for (int i = X_OFF, y = Y_OFF; i < X_OFF + WIDTH; i++)
                new Bound(Tiles[Position.From(i, y)]);

            //Bottom Row 0-WIDTH+1,0
            for (int i = X_OFF, y = Y_OFF + HEIGHT - 1; i < X_OFF + WIDTH; i++)
                new Bound(Tiles[Position.From(i, y)]); ;

            //Left Sides(0,0-HEIGHT+1) and
            for (int i = X_OFF, j = Y_OFF + 1; j < Y_OFF + HEIGHT - 1; j++)
                new Bound(Tiles[Position.From(i, j)]);

            //Right Sides(WIDTH+1,0 - HEIGHT + 1)
            for (int i = X_OFF + WIDTH - 1, j = Y_OFF + 1; j < Y_OFF + HEIGHT - 1; j++)
                new Bound(Tiles[Position.From(i, j)]);
        }

        public static void Clear()
        {
            for (int i = X_OFF; i < X_OFF + WIDTH; i++)
                for (int j = Y_OFF; j < Y_OFF + HEIGHT; j++)
                {
                    Tiles[Position.From(i, j)].Item = null;
                }
        }

        public static Tile OpenTile()
        {
            Tile openTile;
            int x, y;
            do
            {
                x = Rand.Next(X_OFF + 1, X_OFF + WIDTH);
                y = Rand.Next(Y_OFF + 1, Y_OFF + HEIGHT);
                openTile = Tiles[Position.From(x, y)];

            } while (openTile.Item != null);

            return openTile;
        }

        public static Tile CenterTile => Tiles[Position.From(X_OFF + (WIDTH / 2), Y_OFF + (HEIGHT / 2))];

        public static Tile Q1_Tile => Tiles[Position.From(X_OFF + (WIDTH / 4), Y_OFF + (HEIGHT / 4))];
        public static Tile Q2_Tile => Tiles[Position.From(X_OFF + (3 * WIDTH / 4), Y_OFF + (HEIGHT / 4))];
        public static Tile Q3_Tile => Tiles[Position.From(X_OFF + (WIDTH / 4), Y_OFF + (3 * HEIGHT / 4))];
        public static Tile Q4_Tile => Tiles[Position.From(X_OFF + (3 * WIDTH / 4), Y_OFF + (3 * HEIGHT / 4))];


        public static void CollisionCheck(Position position, ICollisionCheck collider)
        {
            var collidee = Tiles[position].Item as ICollisionCheck;
            Action effect, response;

            if (collidee != null && collider.CollissionEffects.TryGetValue(collidee.GetType(), out effect))
            {
                if (collidee.CollisionResponses.TryGetValue(collider.GetType(), out response))
                    response.Invoke();
                effect.Invoke();
            }
            else
            {
                collider.NoCollisionAction.Invoke();
            }
        }

        public static void Draw()
        {
            for(int i = Tile.DirtyTiles.Count - 1; i >= 0; i--)
            {
                Tile.DirtyTiles[i].Draw();
            }

            SetCursorPosition(X_OFF + 5, Y_OFF + HEIGHT + 1);
            ForegroundColor = ConsoleColor.White;

            foreach(var player in Players.Where(p => p.InGame))
            {
                var pos = ScoreBoardPositions[player.Idx];
                SetCursorPosition(pos.X, pos.Y);
                BackgroundColor = player.BackgroundColor;
                int spaces = (int)Math.Ceiling(Math.Log10(player.Score + 0.1d));
                spaces = 3 - spaces;
                Write($"{player.Name}{new string(' ', spaces)}{player.Score}");
            }
        }
    }

    public class Tile
    {
        public static List<Tile> DirtyTiles = new List<Tile>();

        public Tile(Position position)
        {
            Position = position;
        }

        public Position Position;

        private Item _item;
        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;
                IsDirty = true;
            }
        }

        public virtual ConsoleColor Color { get; set; }

        private bool isDirty = false;
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    if (isDirty)                    
                        DirtyTiles.Add(this);                    
                    else
                        DirtyTiles.Remove(this);
                }
            }
        }

        public void Draw()
        {
            SetCursorPosition(Position.X, Position.Y);
            BackgroundColor = Color;

            if (Item != null)
                Item.Draw();
            else
                Write(' ');

            IsDirty = false;

        }

        public void ThrowOccupiedException()
        {
            throw new Exception($"Tile at {Position.X},{Position.Y} occupied by {Item}");
        }
    }

    public class Bound : Item
    {
        public Bound(Tile tile) : base(tile)
        {
            Color = ConsoleColor.Blue;
            Icon = ' ';
        }
    }

}
