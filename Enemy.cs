using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _132134412312
{
    public class Enemy :Form
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
            Position.Offset(dx, 0);
        }
        public List<Bullet> Shoot(int count)
        {
            if (Bullets.Count < 4)
            {
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X + EnemyImg.Width / 2, Position.Y+EnemyImg.Height) });
            }
            Bullets.RemoveAll(bullet => bullet.Position.Y > Form1.FormHeight);
            return Bullets;
        }

    }
}
