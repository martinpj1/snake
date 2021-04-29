using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    //TODO: multiple snakes
    public class Snake
    {
        public Snake(Tile tile)
        {
            startingTile = tile;    
            Signals.Add("Kill", Kill);
            Signals.Add("Eat", Eat);
            Signals.Add("Move", Move);
        }

        public void Reset()
        {
            Score = 0; 
            IsAlive = true;
            while (Tail != null)
                RemoveTail();
            HeadPosition = startingTile.Position;
            Eat();
        }

        public ConsoleColor BackgroundColor;

        public bool IsAlive = false;
        public int Score;
        Tile startingTile;

        LinkedList<SnakePart> Parts = new LinkedList<SnakePart>();
        public SnakePart Head => Parts.First?.Value;
        SnakePart Tail => Parts.Last?.Value;

        public Position HeadPosition;
        
        public void Kill()
        {
            IsAlive = false;
            BackgroundColor = ConsoleColor.DarkRed;
            Death();
        }

        public void Death()
        {
            foreach (var part in Parts)
            {
                part.Color = ConsoleColor.DarkRed;
                Board.Draw();
                Thread.Sleep(75);
            }

            for(int i = 0; i < 3; i++)
            {
                foreach (var part in Parts)
                    part.Color = ConsoleColor.Black;
                Board.Draw();
                Thread.Sleep(150);

                foreach (var part in Parts)
                    part.Color = ConsoleColor.DarkRed;
                Board.Draw();
                Thread.Sleep(250);
            }

        }

        public void Eat()
        {
            Score++;
            AddHead(new SnakePart(HeadPosition));
            BackgroundColor = Head.Color;
        }

        public void Move()
        {
            RemoveTail();
            AddHead(new SnakePart(HeadPosition));
            BackgroundColor = ConsoleColor.Black;
        }

        public void RemoveTail()
        {
            Tail.Tile.Item = null;
            Parts.Remove(Tail);
        }

        public void AddHead(SnakePart part)
        {
            part.Signal += Part_Signal;
            Parts.AddFirst(part);
        }

        Dictionary<string, Action> Signals = new Dictionary<string, Action>();

        private void Part_Signal(object sender, string e)
        {
            Signals[e].Invoke();
        }
    }

    public class SnakePart : Item
    {
        public SnakePart(Position pos) : this(Board.Tiles[pos]) { }

        public SnakePart(Tile tile) :base(tile)
        {
            Color = ConsoleColor.Green;
            Icon = ' ';
            
            CollissionEffects.Add(typeof(Bound), Kill);
            CollissionEffects.Add(typeof(SnakePart), Kill);
            CollissionEffects.Add(typeof(Apple), Eat);
            NoCollisionAction = Move;
            
        }

        public void Kill()
        {
            Signal.Invoke(this, "Kill");
        }

        public void Eat()
        {
            Signal.Invoke(this, "Eat");
        }

        public void Move()
        {
            Signal.Invoke(this, "Move");
        }

        public event EventHandler<string> Signal;



    }

}
