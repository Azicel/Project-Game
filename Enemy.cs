using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _132134412312
{
    internal class Enemy : Form
    {
        public Bitmap EnemyImg = Resource1.Enemy;
        public Point Position = new Point();
        public Enemy()
        {
            EnemyImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }
        public void Movement(int dx)
        {          
            Position.Offset(dx, 0);
        }
        public List<Bullet> Shoot(List<Bullet> bullets, int count)
        {
            if (bullets.Count < count*3)
            {
                bullets.Add(new Bullet()
                { Position = new Point(Position.X + EnemyImg.Width / 2, Position.Y+EnemyImg.Height) });
            }
            bullets.RemoveAll(bullet => bullet.Position.Y > Form1.FormHeight);
            return bullets;
        }

    }
}
