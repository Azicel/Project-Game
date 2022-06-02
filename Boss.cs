using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _132134412312
{
    public class Boss 
    {
        public Bitmap BossImg = Resource1.Boss;
        public Point Position = new Point();
        public List<Bullet> Bullets = new List<Bullet>();
        public Boolean IsSpawned = false;
        public int Lives = 15;
        public Boss()
        {
            BossImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }
        public void Movement(int dx)
        {
            Position.Offset(dx, 0);
        }
        public List<Bullet> Shoot()
        {
            if (Bullets.Count < 16)
            {
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X, Position.Y + BossImg.Height) });
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X + BossImg.Width / 4, Position.Y + BossImg.Height) });
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X + 3 * BossImg.Width / 4, Position.Y + BossImg.Height) });
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X + BossImg.Width, Position.Y + BossImg.Height) });
            }
            Bullets.RemoveAll(bullet => bullet.Position.Y > Form1.FormHeight);
            return Bullets;
        }
    }
}
