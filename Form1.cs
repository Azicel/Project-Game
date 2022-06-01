using System.Drawing;

namespace _132134412312
{
    partial class Form1 : Form
    {
        public static Label GameName = new Label();
        public static Label GameOver = new Label();
        public static Label ScoreLabel = new Label();
        public static Label LivesLabel = new Label();
        public static int Score = 0;
        public static int EnemyCount = 4;
        public static int Lives = 5;
        public static Button CloseGame = new Button();
        public static Button StartGame = new Button();
        public static Player PlayerPlane = new Player();
        public static Boolean IsGameEndedOrStopped = false;
        public static int FormWidth;
        public static int FormHeight;
        public static Bitmap BackGround = Resource1.BackGround;

        public static System.Windows.Forms.Timer ShootTimer = new System.Windows.Forms.Timer();
        public static System.Windows.Forms.Timer SpawnEnemy = new System.Windows.Forms.Timer();
        public Form1()
        {
            var rand = new Random();
            var enemys = new List<Enemy>();
            var enemy = new Enemy();
            var playerBullets = new List<Bullet>();
            var gameStarted = false;
            InitializeComponent();
            InitializeMenu();
            PlayerPlane.AdaptPosition(ClientSize.Width, ClientSize.Height);
            SizeChanged += (sender, args) =>
            {
                ChangeSize();
            };
            StartGame.Click += (sender, args) =>
            {
                Controls.Remove(StartGame);
                Controls.Remove(CloseGame);
                Controls.Remove(GameName);
                Controls.Add(ScoreLabel);
                Controls.Add(LivesLabel);
                Paint += (sender, args) =>
                {
                    var g = args.Graphics;
                    g.DrawImage(PlayerPlane.PlayerStandinOnPlace, PlayerPlane.Position);
                };
                gameStarted = true;
            };
            CloseGame.Click += (sender, args) =>
            {
                Close();
            };           
            ShootTimer.Interval = 500;
            ShootTimer.Start();
            ShootTimer.Tick += (sender, args) =>
            {
                if (gameStarted)
                {
                    PlayerPlane.Shoot(playerBullets);
                    foreach (var bull in playerBullets)
                    {
                        bull.Shoot(bull.Position, Bullet.Directions.Up);
                        Invalidate();
                        Paint += (sender, args) =>
                        {

                            var g = args.Graphics;
                            g.Clip = new Region(new Rectangle(0, 30, ClientSize.Width, ClientSize.Height - 10));
                            g.DrawImage(bull.PlayerBulletImg, bull.Position);
                        };
                        for (int i = 0; i < enemys.Count; i++)
                        {
                            if (bull.IsInsideTarget(enemy.EnemyImg, enemys[i].Position))
                            {
                                var enemyShotX = enemys[i].Position.X;
                                var enemyShotY = enemys[i].Position.Y;
                                Invalidate();
                                Paint += (sender, args) =>
                                {
                                    var g = args.Graphics;
                                    g.DrawImage(BackGround, enemyShotX, enemyShotY);
                                };
                                enemys.RemoveAt(i);
                                Score++;
                                ScoreLabel.Text = "Очки: " + Score.ToString();
                                if (Score % 5 == 0 && Score != 0)
                                    EnemyCount++;
                            }
                        }
                    }
                }
            };          
            SpawnEnemy.Interval = 3000;
            SpawnEnemy.Start();
            SpawnEnemy.Tick += (sender, args) =>
            {
                if (gameStarted)
                {
                    if (enemys.Count < EnemyCount)
                    {
                        enemys.Add(new Enemy() { Position = new Point(rand.Next(32, ClientSize.Width - 32), 30) });
                    }
                    else
                        SpawnEnemy.Stop();
                    foreach (var enemy in enemys)
                    {
                        Invalidate();
                        Paint += (sender, args) =>
                        {
                            var g = args.Graphics;
                            g.DrawImage(enemy.EnemyImg, enemy.Position);
                        };
                    }
                }
            };
            var checkEnemyCount = new System.Windows.Forms.Timer();
            checkEnemyCount.Interval = 10;
            checkEnemyCount.Start();
            checkEnemyCount.Tick += (sender, args) =>
            {
                if (enemys.Count < EnemyCount)
                {
                    SpawnEnemy.Start();
                }
            };
            var enemyMove = new System.Windows.Forms.Timer();
            enemyMove.Interval = 250;
            enemyMove.Start();
            enemyMove.Tick += (sender, args) =>
            {
                foreach(var enemy in enemys)
                {
                    enemy.Movement(rand.Next(-10, 10));
                }
            };
            var enemyShot = new System.Windows.Forms.Timer();
            enemyShot.Interval = 1000;
            enemyShot.Start();
            var enemyBullets = new List<Bullet>();
            enemyShot.Tick += (sender, args) =>
            {
                if (gameStarted)
                {
                    for(int i=0;i<enemys.Count;i++)
                    {
                        enemys[i].Shoot(enemyBullets,EnemyCount);
                        foreach (var bull in enemyBullets)
                        {
                            bull.Shoot(bull.Position, Bullet.Directions.Down);
                            Invalidate();
                            Paint += (sender, args) =>
                            {

                                var g = args.Graphics;
                                g.Clip = new Region(new Rectangle(0, 30, ClientSize.Width, ClientSize.Height - 10));
                                g.DrawImage(bull.EnemyBulletImg, bull.Position);
                            };
                            if (bull.IsInsideTarget(PlayerPlane.PlayerStandinOnPlace, PlayerPlane.Position))
                            {                               
                                Lives--;
                                LivesLabel.Text = "Жизни: " + Lives.ToString();
                            }
                            if(Lives<=0)
                            {
                                GameOver.Text = "Игра окончена. Вы проиграли!";
                                GameOver.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8,
                                    ClientSize.Height / 2 - ClientSize.Height / 8);
                                GameOver.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 16);
                                IsGameEndedOrStopped = true;
                                Controls.Add(GameOver);
                                Controls.Add(CloseGame);
                                ShootTimer.Stop();
                                SpawnEnemy.Stop();
                                enemyShot.Stop();
                                enemyMove.Stop();
                                checkEnemyCount.Stop();
                            }
                        }
                    }
                }
            };
        }
        public void InitializeMenu()
        {
            StartGame.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 - ClientSize.Height / 8);
            StartGame.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            StartGame.Text = "Начать игру";
            GameName.Location = new Point(StartGame.Left - 10, StartGame.Top - 40);
            GameName.Size = new Size(ClientSize.Width / 4 + 40, ClientSize.Height / 16);
            GameName.Text = "Defeat the Aliens";
            GameName.TextAlign = ContentAlignment.TopCenter;
            CloseGame.Location = new Point(StartGame.Left, StartGame.Bottom + 10);
            CloseGame.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            CloseGame.Text = "Выйти из игры";
            ScoreLabel.Location = new Point(0, 0);
            ScoreLabel.Size = new Size(100, 20);
            ScoreLabel.Text = "Очки: " + Score.ToString();
            LivesLabel.Location = new Point(ClientSize.Width - 100, 0);
            LivesLabel.Size = new Size(100, 20);
            LivesLabel.Text = "Жизни: " + Lives.ToString();
            Controls.Add(GameName);
            Controls.Add(StartGame);
            Controls.Add(CloseGame);            
        }
        public void ChangeSize()
        {           
            GameOver.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8,
                ClientSize.Height / 2 - ClientSize.Height / 8);
            GameOver.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 16);
            StartGame.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 - ClientSize.Height / 8);
            StartGame.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            CloseGame.Location = new Point(StartGame.Left, StartGame.Bottom + 10);
            CloseGame.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            GameName.Location = new Point(StartGame.Left - 10, StartGame.Top - 40);
            GameName.Size = new Size(ClientSize.Width / 4 + 20, ClientSize.Height / 16);
            LivesLabel.Location = new Point(ClientSize.Width - 100, 0);
            FormWidth = ClientSize.Width;
            FormHeight = ClientSize.Height;
            PlayerPlane.AdaptPosition(ClientSize.Width, ClientSize.Height);
            if (!Controls.Contains(StartGame))
            {
                Invalidate();
                Paint += (sender, args) =>
                {
                    var g = args.Graphics;
                    g.DrawImage(PlayerPlane.PlayerStandinOnPlace, PlayerPlane.Position);
                };
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1024, 768);
            FormHeight = ClientSize.Height;
            FormWidth = ClientSize.Width;
            this.DoubleBuffered = true;
            this.Name = "Defeat the Aliens"; 
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.ResumeLayout(false);

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsGameEndedOrStopped)
            {
                if (e.X > PlayerPlane.PlayerStandinOnPlace.Width / 2 && e.X < ClientSize.Width - PlayerPlane.PlayerStandinOnPlace.Width / 2)
                {
                    PlayerPlane.Movement(e.X - PlayerPlane.PlayerStandinOnPlace.Width / 2);
                    Invalidate();
                }
            }
        }
    }
}