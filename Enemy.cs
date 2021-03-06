using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _132134412312
{
    public class Enemy
    {
        public Bitmap EnemyImg = Resource1.Enemy;
        public Point Position = new Point();
        public List<Bullet> Bullets = new List<Bullet>();
        public Enemy()
        {
            EnemyImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }
        public void Movement(int dx)
        {
            if (Position.X + dx <= 0)
                dx = dx * -1;
            if (Position.X + dx >= Form1.FormWidth - EnemyImg.Width)
                dx = dx * -1;
            Position.Offset(dx, 0);
        }
        public List<Bullet> Shoot()
        {
            if (Bullets.Count < 5)
            {
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X + EnemyImg.Width / 2, Position.Y+EnemyImg.Height) });
            }
            Bullets.RemoveAll(bullet => bullet.Position.Y > Form1.FormHeight);
            return Bullets;
        }

    }
}
