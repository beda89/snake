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

        public List<Vector2> Body{get;set;}
        public Vector2 Head { get; set; }
        public Direction SnakeDirection;
        public Boolean IsGameOver { get; set; }
        public int Priority { get; set; }

        public Color SnakeColor { get; private set; }


        //last really moved direction (needed if snake directions are modified fast (fast presses of cursor) it is possible to go back on its own body)
        //for example currentDirection is UP and the user presses direction left and then direction down fast
        public Direction LastMovedDirection { get; set; }

        private Vector2 oldLastPart;

        private Texture2D Texture;
        private Boolean addPart = false;

        //only called by server
        public void Initialize(Texture2D texture, Vector2 position,Direction SnakeDirection,int priority,Color snakeColor)
        {
            this.Position = position;
            this.Texture = texture;

            this.SnakeDirection = SnakeDirection;
            this.IsGameOver = false;
            this.Priority = priority;
            this.SnakeColor = snakeColor;

            buildSnake();

        }

        //only called by client
        public void Initialize(Texture2D texture, Vector2 head, List<Vector2> body,int priority,Color snakeColor)
        {
            this.Head = head;
            this.Body = body;
            this.Texture = texture;
            this.Priority = priority;
            this.SnakeColor = snakeColor;
        }

        //snake is initialized with 5 segments
        private void buildSnake()
        {
            Head = Position;

            Body = new List<Vector2>();
            Vector2 tempPosition = Position;

            for (int i = 0; i < 4; i++)
            {
                switch (SnakeDirection)
                {
                    case Direction.Up:
                        tempPosition.Y -= 16;
                        break;
                    case Direction.Left:
                        tempPosition.X += 16;
                        break;
                    case Direction.Right:
                        tempPosition.X -= 16;
                        break;
                    case Direction.Down:
                        tempPosition.Y += 16;
                        break;
                }

                Body.Add(tempPosition);
            }
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

                if (Body.Count() != 0)
                {

                    oldLastPart = Body.Last();

                    for (int i = 0; i < Body.Count - 1; i++)
                    {
                        Body[Body.Count - 1 - i] = Body[Body.Count - 2 - i];
                    }

                    Body[0] = Head;
                }
                else
                {
                    oldLastPart = Head;
                }



                Head = Position;

                if (addPart == true)
                {
                    Body.Add(oldLastPart);
                    addPart = false;
                }
        }

        public void AddPart()
        {
            addPart = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Drawing head
            spriteBatch.Draw(Texture, Head, Color.White);

            //Drawing Body
            foreach (Vector2 part in Body)
            {
                spriteBatch.Draw(Texture, part, Color.White);
            }
        }

        public Boolean CollidesWithItself()
        {
            //iterator over body
            foreach(Vector2 part in Body)
            {
                if (part.Equals(Head))
                {
                    return true;
                }
            }

            return false;
        }

        public void CheckIfEatenByEnemy(List<Snake> snakes)
        {
            foreach (Snake enemy in snakes)
            {
                if (enemy.Equals(this))
                {
                    continue;
                }

                if (enemy.IsGameOver)
                {
                    continue;
                }

                int bodyIndex=0;

                foreach (Vector2 part in Body)
                {
                    if (part.Equals(enemy.Head))
                    {
                        //TODO could be changed
                        enemy.AddPart();

                        //remove the eaten part and all following
                        Body.RemoveRange(bodyIndex, Body.Count() - bodyIndex);
                        break;
                    }

                    bodyIndex++;
                }
            }
        }
    }
}
