using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Snake
{
    class GameUtils
    {
        public static Boolean IsDirectionValid(Snake snake, Snake.Direction direction)
        {
            int distance = Math.Abs((int)direction - (int)snake.LastMovedDirection);
            if (distance != 2)
                return true;

            return false;
        }

    }
}
