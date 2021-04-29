using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XInputDotNetPure;

namespace Snake
{
    public class Controller
    {
        static readonly Dictionary<Direction, Position> DirectionVectors = new Dictionary<Direction, Position>()
        {
            {  Direction.Up, Position.Create(0,-1)},
            {  Direction.Right, Position.Create(1,0)},
            {  Direction.Down, Position.Create(0,1)},
            {  Direction.Left, Position.Create(-1,0)},
        };

        static Dictionary<PlayerIndex, Direction> defaultDirections = new Dictionary<PlayerIndex, Direction>
        {
            {PlayerIndex.One, Direction.Right },
            {PlayerIndex.Two, Direction.Left },
            {PlayerIndex.Three, Direction.Down },
            {PlayerIndex.Four, Direction.Up }
        };

        public Controller(PlayerIndex player)
        {
            Player = player;
            nextDirection = defaultDirections[player];
        }

        PlayerIndex Player;
        GamePadState State;

        public void GetState()
        {
            State = GamePad.GetState(Player);
        }

        public void ReadInput()
        {
            GetState();
            if (KeyAvailable)
            {
                nextDirection = GetDirection(prevDirection);
            }
        }
        
        Direction prevDirection;
        Direction nextDirection;

        public Position GetDirectionVector()
        {
            prevDirection = nextDirection;
            return DirectionVectors[nextDirection];
        }

        public bool IsConnected => State.IsConnected; 

        public bool Start =>
            State.Buttons.Start == ButtonState.Pressed;

        public bool Select =>
           State.Buttons.Back == ButtonState.Pressed;


        public bool Quit =>
            State.Buttons.B == ButtonState.Pressed;

        public bool KeyAvailable =>
            State.DPad.Up == ButtonState.Pressed ||
            State.DPad.Right == ButtonState.Pressed ||
            State.DPad.Down == ButtonState.Pressed ||
            State.DPad.Left == ButtonState.Pressed;

        public Direction GetDirection(Direction prevDirection)
        {
            Direction nextDirection = prevDirection;

            if (State.DPad.Up == ButtonState.Pressed && prevDirection != Direction.Down)
                nextDirection = Direction.Up;
            else if (State.DPad.Right == ButtonState.Pressed && prevDirection != Direction.Left)
                nextDirection = Direction.Right;
            else if (State.DPad.Down == ButtonState.Pressed && prevDirection != Direction.Up)
                nextDirection = Direction.Down;
            else if (State.DPad.Left == ButtonState.Pressed && prevDirection != Direction.Right)
                nextDirection = Direction.Left;

            return nextDirection;
        }
    }
}
