using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _132134412312
{
    public class Player : Form
    {
        public Point Position = new Point();
        public readonly Bitmap PlayerStandinOnPlace = Resource1.PlayerStands;
        public static System.Windows.Forms.Timer ShootTimer = new System.Windows.Forms.Timer();
        public void AdaptPosition(int x, int y)
        {
            Position = new Point(x / 2-PlayerStandinOnPlace.Width/2, y-PlayerStandinOnPlace.Height);
        }
        public void Movement(int mouse)
        {
            Position.X = mouse;
        }
        public List<Bullet> Shoot(List<Bullet> bullets)
        {
            if (bullets.Count < ClientSize.Height / 32)
            {
                bullets.Add(new Bullet()
                { Position = new Point(Position.X + PlayerStandinOnPlace.Width / 2, Position.Y) });
            }            
            bullets.RemoveAll(bullet => bullet.Position.Y < 0);
            return bullets;
        }
       
    }
}
