using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _132134412312
{


    public class Bullet
    {
        public enum Directions
        {
            Up = 1,
            UpRight = 2,
            Right = 3,
            DownRight = 4,
            Down = 5,
            LeftDown = 6,
            Left = 7,
            UpLeft = 8,
        }
        public Bitmap PlayerBulletImg = Resource1.Bullet;
        public Bitmap EnemyBulletImg = Resource1.Bullet_Enemy;
        public Point Position = new Point();
        public Boolean IsActive = true;
        public void Shoot(Point startPos, Directions dir)
        {
            var direction = new Point();
            switch (dir)
            {
                case Directions.Up:
                    direction = new Point(0, -30);
                    break;
                case Directions.UpRight:
                    direction = new Point(30, -30);
                    break;
                case Directions.Right:
                    direction = new Point(30, 0);
                    break;
                case Directions.DownRight:
                    direction = new Point(30, 30);
                    break;
                case Directions.Down:
                    direction = new Point(0, 30);
                    break;
                case Directions.LeftDown:
                    direction = new Point(-30, 30);
                    break;
                case Directions.Left:
                    direction = new Point(-30, 0);
                    break;
                case Directions.UpLeft:
                    direction = new Point(-30, -30);
                    break;
            }
            Position = startPos;
            Position.Offset(direction);
        } 
        public bool IsInsideTarget(Bitmap target, Point targetPosition)
        {
            if (Position.X >= targetPosition.X && Position.X <= targetPosition.X + target.Width
                && Position.Y >= targetPosition.Y && Position.Y <= targetPosition.Y + target.Height)
                return true;
            return false;
        }
    }
}

