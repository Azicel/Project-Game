using System.Drawing;

namespace _132134412312
{
    public partial class Form1 : Form
    {
        public Label GameName = new Label();
        public Label GameStatus = new Label();
        public Label GameOver = new Label();
        public Label ScoreLabel = new Label();
        public Label LivesLabel = new Label();
        public Button CloseGame = new Button();
        public Button StartGame = new Button();
        public Button PauseGame = new Button();
        public Button EasyDiffculty = new Button();
        public Button MediumDiffculty = new Button();
        public Button HardDiffculty = new Button();
        public Button RestartGame = new Button();
        public Bitmap BackGround = Resource1.BackGround;

        public int Score = 0;
        public int EnemyCount = 4;
        public int MaxLives = 5;
        public int Lives = 0;
        public Player PlayerPlane = new Player();
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Boss> Bosses = new List<Boss>();
        public Boolean GameStarted = false;
        public static int FormWidth;
        public static int FormHeight;

        public static System.Windows.Forms.Timer ShootTimer = new System.Windows.Forms.Timer() { Interval = 350};
        public static System.Windows.Forms.Timer SpawnEnemy = new System.Windows.Forms.Timer() { Interval = 3000 };
        public static System.Windows.Forms.Timer EnemyShot = new System.Windows.Forms.Timer() { Interval = 500 };
        public static System.Windows.Forms.Timer EnemyMove = new System.Windows.Forms.Timer() { Interval = 250 };
        public static System.Windows.Forms.Timer CheckEnemyCount = new System.Windows.Forms.Timer() { Interval = 10 };
        public static System.Windows.Forms.Timer BossNeeded = new System.Windows.Forms.Timer() { Interval = 30000 };
        public static System.Windows.Forms.Timer BossShoot = new System.Windows.Forms.Timer() { Interval = 500 };
        public static System.Windows.Forms.Timer BossMove = new System.Windows.Forms.Timer() { Interval = 500 };
        public Form1()
        {
            var rand = new Random();
            var enemy = new Enemy();
            var boss = new Boss() { Position = new Point(-256,256)};
            InitializeComponent();
            InitializeMenu();
            Controls.Add(GameName);
            Controls.Add(StartGame);
            Controls.Add(CloseGame);
            PlayerPlane.AdaptPosition(ClientSize.Width, ClientSize.Height);
            Score = 0;
            Lives = MaxLives;
            SizeChanged += (sender, args) =>
            {
                ChangeSize();
            };
            StartGame.Click += (sender, args) =>
            {
                Controls.Add(EasyDiffculty);
                Controls.Add(MediumDiffculty);
                Controls.Add(HardDiffculty);
                Controls.Remove(CloseGame);
                Controls.Remove(StartGame);
            };
            EasyDiffculty.Click += (sender, args) =>
            {
                MaxLives = 7;
                Lives = MaxLives;
                InitializeGame();
            };
            MediumDiffculty.Click += (sender, args) =>
            {
                MaxLives = 5;
                Lives = MaxLives;
                InitializeGame();
            };
            HardDiffculty.Click += (sender, args) =>
            {
                MaxLives = 3;
                Lives = MaxLives;
                InitializeGame();
            };
            PauseGame.Click += (sender, args) =>
            {
                InitializeGame();                
            };
            CloseGame.Click += (sender, args) =>
            {
                Close();
            };
            RestartGame.Click += (sender, args) =>
            {
                Score = 0;
                Lives = MaxLives;
                Restart();
            };
            ShootTimer.Tick += (sender, args) =>
            {
                if (GameStarted)
                {
                    ShootBullet(PlayerPlane, boss, enemy);
                }
            };
            SpawnEnemy.Tick += (sender, args) =>
            {
                if (GameStarted)
                {
                    if (Enemies.Count < EnemyCount)
                    {
                        Enemies.Add(new Enemy() { Position = new Point(rand.Next(32, ClientSize.Width - 32), 30) });
                    }
                    foreach (var enemy in Enemies)
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
            CheckEnemyCount.Tick += (sender, args) =>
            {
                if (Enemies.Count < EnemyCount)
                {
                    SpawnEnemy.Start();
                }
            };
            EnemyMove.Tick += (sender, args) =>
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Movement(rand.Next(-10, 10));
                }
            };
            EnemyShot.Tick += (sender, args) =>
            {
                if (GameStarted)
                {
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        ShootBullet(Enemies[i]);
                    }
                }
            };
            BossNeeded.Tick += (sender, args) =>
            {
                if (GameStarted && !boss.IsSpawned)
                {
                    if (Score >= 10)
                    {
                        Bosses.Add(new Boss() { Position = new Point(rand.Next(0, ClientSize.Width - 256), 30) });
                        BossShoot.Start();
                        BossMove.Start();
                        boss.IsSpawned = true;
                        BossNeeded.Interval = 30000;
                        Invalidate();
                        Paint += (sender, args) =>
                        {
                            var g = args.Graphics;
                            if (Bosses.Count > 0)
                                g.DrawImage(boss.BossImg, Bosses.Last().Position);
                            else
                                g.DrawImage(boss.BossImg, boss.Position);
                        };
                    }
                }
            };
            BossMove.Tick += (sender, args) =>
            {
                if(boss.IsSpawned)
                    Bosses.Last().Movement(rand.Next(-20, 20));
            };
            BossShoot.Tick += (sender, args) =>
            {
                if (GameStarted && boss.IsSpawned)
                {
                    ShootBullet(Bosses.Last());
                }                                  
            };
        }

        public void InitializeMenu()
        {
            InitializeLabels();
            InitializeButtons();
            FormWidth = ClientSize.Width;
            FormHeight = ClientSize.Height;        
        }

        public void InitializeLabels()
        {
            GameName.Location = new Point(StartGame.Left - 10, StartGame.Top - 100);
            GameName.Size = new Size(ClientSize.Width / 4 + 40, ClientSize.Height / 16);
            GameName.Text = "Defeat the Aliens";
            GameName.TextAlign = ContentAlignment.TopCenter;
            ScoreLabel.Location = new Point(0, 0);
            ScoreLabel.Size = new Size(100, 20);
            ScoreLabel.Text = "Очки: " + Score.ToString();
            LivesLabel.Location = new Point(ClientSize.Width - 100, 0);
            LivesLabel.Size = new Size(100, 20);
            LivesLabel.Text = "Жизни: " + Lives.ToString();
            GameStatus.Text = "Игра приостановлена";
            GameStatus.Location = new Point(GameName.Location.X, GameName.Location.Y);
            GameStatus.Size = GameName.Size;
            GameStatus.TextAlign = ContentAlignment.TopCenter;
            GameOver.Text = "Игра окончена. Вы проиграли!";
            GameOver.Location = new Point(GameName.Left, GameName.Top);
            GameOver.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            GameOver.TextAlign = ContentAlignment.TopCenter;
        }
        
        public void InitializeButtons()
        {
            StartGame.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 - ClientSize.Height / 8);
            StartGame.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            StartGame.Text = "Начать игру";
            EasyDiffculty.Text = "Сложность: Легкая";
            EasyDiffculty.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 - ClientSize.Height / 6);
            EasyDiffculty.Size = StartGame.Size;
            MediumDiffculty.Text = "Сложность: Средняя";
            MediumDiffculty.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2);
            MediumDiffculty.Size = StartGame.Size;
            HardDiffculty.Text = "Сложность: Тяжелая";
            HardDiffculty.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 + ClientSize.Height / 6);
            HardDiffculty.Size = StartGame.Size;
            CloseGame.Location = new Point(StartGame.Left, StartGame.Bottom + 10);
            CloseGame.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 8);
            CloseGame.Text = "Выйти из игры";
            RestartGame.Text = "Начать заново?";
            RestartGame.Location = StartGame.Location;
            RestartGame.Size = StartGame.Size;
            PauseGame.Text = "Продолжить";
            PauseGame.Location = StartGame.Location;
            PauseGame.Size = StartGame.Size;
        }

        public void ChangeSize()
        {
            InitializeMenu();
            FormWidth = ClientSize.Width;
            FormHeight = ClientSize.Height;
            PlayerPlane.AdaptPosition(ClientSize.Width, ClientSize.Height);
            if (GameStarted)
            {
                Invalidate();
                Paint += (sender, args) =>
                {
                    var g = args.Graphics;
                    g.DrawImage(PlayerPlane.PlayerImg, PlayerPlane.Position);
                };
            }
        }

        public void InitializeGame()
        {
            Controls.Remove(StartGame);
            Controls.Remove(CloseGame);
            Controls.Remove(GameName);
            Controls.Remove(GameOver);
            Controls.Remove(RestartGame);
            Controls.Remove(EasyDiffculty);
            Controls.Remove(MediumDiffculty);
            Controls.Remove(HardDiffculty);
            Controls.Remove(PauseGame);
            Controls.Add(ScoreLabel);
            Controls.Add(LivesLabel);
            ScoreLabel.Text = "Очки: " + Score.ToString();
            LivesLabel.Text = "Жизни: " + Lives.ToString();
            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.DrawImage(PlayerPlane.PlayerImg, PlayerPlane.Position);
            };
            ShootTimer.Start();
            SpawnEnemy.Start();
            CheckEnemyCount.Start();
            EnemyMove.Start();
            EnemyShot.Start();
            BossNeeded.Start();
            GameStarted = true;
        }
        public void Restart()
        {
            InitializeGame();
            Enemies = new List<Enemy>();
            PlayerPlane.Bullets = new List<Bullet>();
            Invalidate();
            Paint += (sender, args) =>
            {
                var g = args.Graphics;
                g.Clear(BackColor);
                g.DrawImage(PlayerPlane.PlayerImg, PlayerPlane.Position);
            };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.ResumeLayout(false);

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (GameStarted)
            {
                if (e.X > PlayerPlane.PlayerImg.Width / 2 && e.X < ClientSize.Width - PlayerPlane.PlayerImg.Width / 2)
                {
                    PlayerPlane.Movement(e.X - PlayerPlane.PlayerImg.Width / 2);
                    Invalidate();
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Escape)
            {
                GameStarted = false;                                
                Controls.Add(PauseGame);
                Controls.Add(CloseGame);
            }
        }

        private void CheckBullet(Bullet bull, Bitmap bitmap, Enemy enemy)
        {
            if (bull.IsInsideTarget(bitmap, enemy.Position))
            {
                Invalidate();
                Paint += (sender, args) =>
                {
                    var g = args.Graphics;
                    g.DrawImage(BackGround, enemy.Position.X, enemy.Position.Y);
                    foreach (var bull in enemy.Bullets)
                    {
                        g.DrawImage(BackGround,
                            new Rectangle(bull.Position.X, bull.Position.Y,
                            bull.EnemyBulletImg.Width, bull.EnemyBulletImg.Height));
                    }
                };
                bull.IsActive = false;
                Enemies.Remove(enemy);
                Score++;
                ScoreLabel.Text = "Очки: " + Score.ToString();
                if (Score % 5 == 0 && Score != 0)
                    EnemyCount++;
            }
        }
        private void CheckBullet(Bullet bull, Bitmap bitmap, ref Boss boss)
        {
            if (bull.IsInsideTarget(boss.BossImg, Bosses.Last().Position))
            {
                Bosses.Last().Lives--;
                bull.IsActive = false;
                if (Bosses.Last().Lives <= 0)
                {
                    var bossData = Bosses.Last();
                    Invalidate();
                    Paint += (sender, args) =>
                    {
                        var g = args.Graphics;
                        foreach (var bull in bossData.Bullets)
                        {
                            g.DrawImage(BackGround,
                                new Rectangle(bull.Position.X, bull.Position.Y,
                                bull.EnemyBulletImg.Width, bull.EnemyBulletImg.Height));
                        }
                    };
                    Bosses.Clear();
                    boss.IsSpawned = false;
                    Score += 10;
                    Lives++;
                    LivesLabel.Text = "Жизни: " + Lives.ToString();
                    ScoreLabel.Text = "Очки: " + Score.ToString();
                    EnemyCount += 2;
                    CheckEnemyCount.Start();
                    EnemyMove.Start();
                    EnemyShot.Start();
                }
            }
        }
        private void CheckBullet(Bullet bull, Bitmap bitmap, Player player)
        {
            if (bull.IsInsideTarget(player.PlayerImg, player.Position))
            {
                Lives--;
                LivesLabel.Text = "Жизни: " + Lives.ToString();
            }
            if (Lives <= 0)
            {
                GameStarted = false;
                Controls.Add(RestartGame);
                Controls.Add(GameOver);
                Controls.Add(CloseGame);
                ShootTimer.Stop();
                SpawnEnemy.Stop();
                EnemyShot.Stop();
                EnemyMove.Stop();
                CheckEnemyCount.Stop();
                BossShoot.Stop();
                BossMove.Stop();
                BossNeeded.Stop();
            }
        }
        private void ShootBullet(Enemy enemy)
        {
            enemy.Shoot();
            foreach (var bull in enemy.Bullets)
            {
                bull.Shoot(bull.Position, Bullet.Directions.Down);
                Invalidate();
                Paint += (sender, args) =>
                {

                    var g = args.Graphics;
                    g.Clip = new Region(new Rectangle(0, 30, ClientSize.Width, ClientSize.Height - 10));
                    g.DrawImage(bull.EnemyBulletImg, bull.Position);
                };
                CheckBullet(bull, PlayerPlane.PlayerImg, PlayerPlane);
            }
        }
        private void ShootBullet(Player player, Boss boss, Enemy enemy)
        {
            player.Shoot();
            foreach (var bull in player.Bullets)
            {
                bull.Shoot(bull.Position, Bullet.Directions.Up);
                Invalidate();
                Paint += (sender, args) =>
                {

                    var g = args.Graphics;
                    g.Clip = new Region(new Rectangle(0, 30, ClientSize.Width, ClientSize.Height - 10));
                    g.DrawImage(bull.PlayerBulletImg, bull.Position);
                };
                if (bull.IsActive)
                {
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        CheckBullet(bull, enemy.EnemyImg, Enemies[i]);
                    }
                    if (boss.IsSpawned)
                    {
                        CheckBullet(bull, boss.BossImg, ref boss);
                    }
                }
            }
        }
        private void ShootBullet(Boss boss)
        {
            boss.Shoot();
            foreach (var bull in boss.Bullets)
            {
                bull.Shoot(bull.Position, Bullet.Directions.Down);
                Invalidate();
                Paint += (sender, args) =>
                {

                    var g = args.Graphics;
                    g.Clip = new Region(new Rectangle(0, 30, ClientSize.Width, ClientSize.Height - 10));
                    g.DrawImage(bull.EnemyBulletImg, bull.Position);
                };
                CheckBullet(bull, PlayerPlane.PlayerImg, PlayerPlane);
            }
        }
    }
}