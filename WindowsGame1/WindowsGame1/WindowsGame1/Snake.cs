using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    class Snake
    {
        public Vector2 Position;
        public enum Direction { Up, Right, Down, Left };

        //the parts of the snake as a list (every snake segment)
        public List<Vector2> parts{get;set;}
        public Direction SnakeDirection;


        //last really moved direction (needed if snake directions are modified fast (fast presses of cursor) it is possible to go back on its own body)
        //for example currentDirection is UP and the user presses direction left and then direction down fast
        public Direction LastMovedDirection { get; set; }

        private Vector2 oldLastPart;

        private Texture2D Texture;
        private Boolean addPart = false;

        //only called by server
        public void Initialize(Texture2D texture, Vector2 position,Direction SnakeDirection)
        {
            this.Position = position;
            this.Texture = texture;

            this.SnakeDirection = SnakeDirection;

            parts = new List<Vector2>();
            parts.Add(position);

            Vector2 tempPosition = position;

            for (int i = 0; i < 4; i++)
            {
                switch (SnakeDirection)
                {
                    case Direction.Up:
                        tempPosition.Y-=16;
                        break;
                    case Direction.Left:
                        tempPosition.X += 16;
                        break;
                    case Direction.Right:
                        tempPosition.X-= 16;
                        break;
                    case Direction.Down:
                        tempPosition.Y += 16;
                        break;
                }

                parts.Add(tempPosition);
            }

        }

        //only called by client
        public void Initialize(Texture2D texture, List<Vector2> parts)
        {
            this.parts = parts;
            this.Texture = texture;
        }

        //only called by server
        public void Update(GameField gameField)
        {
                Vector2 tempPosition = Position;

                switch (SnakeDirection)
                {
                    case Direction.Up:
                        tempPosition.Y -= 16;
                        break;
                    case Direction.Down:
                        tempPosition.Y += 16;
                        break;
                    case Direction.Left:
                        tempPosition.X -= 16;
                        break;
                    case Direction.Right:
                        tempPosition.X += 16;
                        break;
                }

                if (gameField.Collides(tempPosition))
                {
                    return;
                }

                LastMovedDirection = SnakeDirection;

                Position = tempPosition;

                oldLastPart = parts.Last();

                for (int i = 0; i < parts.Count - 1; i++)
                {
                    parts[parts.Count - 1 - i] = parts[parts.Count - 2 - i];
                }

                parts[0] = Position;

                if (addPart == true)
                {
                    parts.Add(oldLastPart);
                    addPart = false;
                }
        }

        public void AddPart()
        {
            addPart = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Vector2 part in parts)
            {
                spriteBatch.Draw(Texture, part, Color.White);
            }
        }
    }
}
