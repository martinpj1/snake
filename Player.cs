using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XInputDotNetPure;

namespace Snake
{
    public class Player
    {
        static Dictionary<PlayerIndex, Tile> startingPositions = new Dictionary<PlayerIndex, Tile>
        {
            {PlayerIndex.One, Board.Q1_Tile },
            {PlayerIndex.Two, Board.Q4_Tile },
            {PlayerIndex.Three, Board.Q2_Tile },
            {PlayerIndex.Four, Board.Q3_Tile }
        };

        static Dictionary<PlayerIndex, string> names = new Dictionary<PlayerIndex, string>
        {
            {PlayerIndex.One, "Mom" },
            {PlayerIndex.Two, "Phil" },
            {PlayerIndex.Three, string.Empty },
            {PlayerIndex.Four, string.Empty }
        };

        public PlayerIndex Idx;
        public Player(PlayerIndex playerIdx)
        {
            Idx = playerIdx;
            Snake = new Snake(startingPositions[Idx]);
            Controller = new Controller(Idx);
            Name = names[Idx];
        }

        Snake Snake;
        Controller Controller;

        public string Name;
        public bool InGame;
        public bool IsAlive => Snake.IsAlive;
        public bool Quit => Controller.Quit;
        public int Score => Snake.Score;
        public ConsoleColor BackgroundColor => Snake.BackgroundColor;


        public void JoinGame()
        {
            InGame = true;
            Reset();
        }

        public void Reset() => Snake.Reset();

        public bool Start => Controller.Start;
        public bool Select => Controller.Select;

        public void ReadInput() => Controller.ReadInput();

        public void UpdateState()
        {
            Snake.HeadPosition += Controller.GetDirectionVector();
            Board.CollisionCheck(Snake.HeadPosition, Snake.Head);
        }
    }
}
