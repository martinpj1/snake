using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XInputDotNetPure;
using static System.Console;
using static ConsoleExtender.ConsoleHelper;

namespace Snake
{
    class Program
    {
        static long ClockTick;
        static Timer GameClock;

        static Random Rand = new Random();

        static void OnGameClock_Tick(object stateInfo)
        {
            Interlocked.Exchange(ref ClockTick, 1);
        }

        public static Player Player1 = new Player(PlayerIndex.One);
        public static Player Player2 = new Player(PlayerIndex.Two);
        public static Player Player3 = new Player(PlayerIndex.Three);
        public static Player Player4 = new Player(PlayerIndex.Four);

        public static readonly List<Player> Players = new List<Player>
        {
            Player1, Player2, Player3, Player4
        };

        public static bool InGame => Players.Any(p => p.InGame);

        //TODO: main menu
        //TODO: music
        //TODO: controller support
        //TODO: poison apples
        //TODO: choose your own color
        static void Main(string[] args)
        {
            DisableQuickEdit();
            SetConsoleFont(16);
            //GetCurrentFontSize();
            SetFullScreen();

            GameClock = new Timer(OnGameClock_Tick, null, 500, 200);

            Board.BuildBorder();

            bool playing = true;
            while (playing)
            {
                Players.ForEach(p => p.ReadInput());

                foreach (var player in Players.Where(p => !p.InGame && p.Select).ToList())
                    player.JoinGame();

                if (InGamePlayers.Any(p => p.Start))
                    GameLoop();

                playing = InGamePlayers.Count == 0 || !InGamePlayers.Any(p => p.Quit);

                Board.Draw();
            }
        }

        static bool InSession;

        static List<Player> InGamePlayers => Players.Where(p => p.InGame).ToList();
        static List<Player> AlivePlayers => Players.Where(p => p.IsAlive).ToList();
        static void GameLoop()
        {
            Board.Clear();
            Board.BuildBorder();
            InGamePlayers.ForEach(p => p.Reset());

            InSession = true;
            while (AlivePlayers.Any())
            {
                AlivePlayers.ForEach(p => p.ReadInput());

                if (Interlocked.Read(ref ClockTick) == 1)
                {
                    Interlocked.Exchange(ref ClockTick, 0);

                    //about once every four seconds
                    if (Rand.Next(12) == 0)
                    {
                        new Apple(Board.OpenTile());
                    }

                    AlivePlayers.ForEach(p => p.UpdateState());
                   
                    Board.Draw();
                }

            }
            InSession = false; 
        }
    }
}
