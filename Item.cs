using System;
using System.Collections.Generic;
using static System.Console;
namespace Snake
{
    public abstract class Item : ICollisionCheck
    {
        public Item(Tile tile)
        {
            Tile = tile;
            if (Tile.Item == null)
            {
                Tile.Item = this;
            }
            else
                tile.ThrowOccupiedException();
        }

        public Tile Tile { get; set; }

        private ConsoleColor _color;
        public ConsoleColor Color
        {
            get { return _color; }
            set {
                if(_color != value)
                {
                    _color = value;
                    Tile.IsDirty = true;
                }
            }
        }

        private char _icon;
        public char Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    Tile.IsDirty = true;
                }
            }
        }

        public virtual void Draw()
        {
            if (Icon == ' ')
                BackgroundColor = Color;
            else
                ForegroundColor = Color;

            Write(Icon);
        }

        #region ICollision

        private Dictionary<Type, Action> _collisionEffects = new Dictionary<Type, Action>();
        public Dictionary<Type, Action> CollissionEffects
        {
            get { return _collisionEffects; }
            set { _collisionEffects = value; }
        }

        private Dictionary<Type, Action> _collisionResponses = new Dictionary<Type, Action>();
        public Dictionary<Type, Action> CollisionResponses
        {
            get { return _collisionResponses; }
            set { _collisionResponses = value; }
        }

        public Action NoCollisionAction { get; set; }        
        #endregion
    }
}
