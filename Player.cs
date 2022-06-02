using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _132134412312
{
    public class Player
    {
        public Point Position = new Point();
        public readonly Bitmap PlayerImg = Resource1.PlayerStands;
        public static System.Windows.Forms.Timer ShootTimer = new System.Windows.Forms.Timer();
        public List<Bullet> Bullets = new List<Bullet>();
        public int MaxBullets = 10;
        public void AdaptPosition(int x, int y)
        {
            Position = new Point(x / 2-PlayerImg.Width/2, y-PlayerImg.Height);
        }
        public void Movement(int mouse)
        {
            Position.X = mouse;
        }
        public List<Bullet> Shoot()
        {
            if (Bullets.Count < MaxBullets)
            {
                Bullets.Add(new Bullet()
                { Position = new Point(Position.X + PlayerImg.Width / 2, Position.Y) });
            }            
            Bullets.RemoveAll(bullet => bullet.Position.Y < 0);
            return Bullets;
        }
       
    }
}
