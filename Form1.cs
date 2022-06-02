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

        public static System.Windows.Forms.Timer ShootTimer = new System.Windows.Forms.Timer();
        public static System.Windows.Forms.Timer SpawnEnemy = new System.Windows.Forms.Timer();
        public Form1()
        {
            var rand = new Random();
            var enemy = new Enemy();
            var boss = new Boss() { Position = new Point(-256,256)};
            var checkEnemyCount = new System.Windows.Forms.Timer();
            var enemyMove = new System.Windows.Forms.Timer();
            var enemyShot = new System.Windows.Forms.Timer();
            var bossNeeded = new System.Windows.Forms.Timer() { Interval = 25000 };
            var bossShoot = new System.Windows.Forms.Timer();
            var bossMove = new System.Windows.Forms.Timer();
            InitializeComponent();
            InitializeMenu();
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
                checkEnemyCount.Start();
                enemyMove.Start();
                enemyShot.Start();
                bossNeeded.Start();
            };
            MediumDiffculty.Click += (sender, args) =>
            {
                MaxLives = 5;
                Lives = MaxLives;
                InitializeGame();
                checkEnemyCount.Start();
                enemyMove.Start();
                enemyShot.Start();
                bossNeeded.Start();
            };
            HardDiffculty.Click += (sender, args) =>
            {
                MaxLives = 3;
                Lives = MaxLives;
                InitializeGame();
                checkEnemyCount.Start();
                enemyMove.Start();
                enemyShot.Start();
                bossNeeded.Start();
            };
            PauseGame.Click += (sender, args) =>
            {
                InitializeGame();
                checkEnemyCount.Start();
                enemyMove.Start();
                enemyShot.Start();
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
                checkEnemyCount.Start();
                enemyMove.Start();
                enemyShot.Start();
            };
            ShootTimer.Interval = 500;
            ShootTimer.Tick += (sender, args) =>
            {
                if (GameStarted)
                {
                    PlayerPlane.Shoot();
                    foreach (var bull in PlayerPlane.Bullets)
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
                                if (bull.IsInsideTarget(enemy.EnemyImg, Enemies[i].Position))
                                {
                                    var enemyShot = Enemies[i];
                                    Invalidate();
                                    Paint += (sender, args) =>
                                    {
                                        var g = args.Graphics;
                                        g.DrawImage(BackGround, enemyShot.Position.X, enemyShot.Position.Y);
                                        foreach (var bull in enemyShot.Bullets)
                                        {
                                            g.DrawImage(BackGround,
                                                new Rectangle(bull.Position.X, bull.Position.Y,
                                                bull.EnemyBulletImg.Width, bull.EnemyBulletImg.Height));
                                        }
                                    };
                                    bull.IsActive = false;
                                    Enemies.RemoveAt(i);
                                    Score++;
                                    ScoreLabel.Text = "Очки: " + Score.ToString();
                                    if (Score % 5 == 0 && Score != 0)
                                        EnemyCount++;
                                    break;
                                }
                            }
                            if (boss.IsSpawned)
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
                                        checkEnemyCount.Start();
                                        enemyMove.Start();
                                        enemyShot.Start();
                                    }
                                }
                            }
                        }
                    }
                }
            };
            SpawnEnemy.Interval = 3000;
            SpawnEnemy.Tick += (sender, args) =>
            {
                if (GameStarted)
                {
                    if (Enemies.Count < EnemyCount)
                    {
                        Enemies.Add(new Enemy() { Position = new Point(rand.Next(32, ClientSize.Width - 32), 30) });
                    }
                    else
                        SpawnEnemy.Stop();
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
            checkEnemyCount.Interval = 10;
            checkEnemyCount.Tick += (sender, args) =>
            {
                if (Enemies.Count < EnemyCount)
                {
                    SpawnEnemy.Start();
                }
            };
            enemyMove.Interval = 250;
            enemyMove.Tick += (sender, args) =>
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Movement(rand.Next(-10, 10));
                }
            };
            enemyShot.Interval = 1000;
            enemyShot.Tick += (sender, args) =>
            {
                if (GameStarted)
                {
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        Enemies[i].Shoot(EnemyCount);
                        foreach (var bull in Enemies[i].Bullets)
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
                            if (Lives <= 0)
                            {
                                GameOver.Text = "Игра окончена. Вы проиграли!";
                                GameOver.Location = new Point(GameName.Left, GameName.Top);
                                GameOver.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 16);
                                GameStarted = false;
                                Controls.Add(RestartGame);
                                Controls.Add(GameOver);
                                Controls.Add(CloseGame);
                                ShootTimer.Stop();
                                SpawnEnemy.Stop();
                                enemyShot.Stop();
                                enemyMove.Stop();
                                checkEnemyCount.Stop();
                                bossShoot.Stop();
                                bossMove.Stop();
                                bossNeeded.Stop();
                            }
                        }
                    }
                }
            };
            bossNeeded.Tick += (sender, args) =>
            {
                if (GameStarted && !boss.IsSpawned)
                {
                    if (Score >= 0)
                    {
                        Bosses.Add(new Boss() { Position = new Point(rand.Next(256, ClientSize.Width - 256), 30) });
                        SpawnEnemy.Stop();
                        checkEnemyCount.Stop();
                        bossShoot.Start();
                        bossMove.Start();
                        boss.IsSpawned = true;
                        bossNeeded.Interval = 30000;
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
            bossMove.Interval = 500;
            bossMove.Tick += (sender, args) =>
            {
                if(boss.IsSpawned)
                    Bosses.Last().Movement(rand.Next(-20, 20));
            };
            bossShoot.Interval = 1000;
            bossShoot.Tick += (sender, args) =>
            {
                if (GameStarted && boss.IsSpawned)
                {
                    Bosses.Last().Shoot();
                    foreach (var bull in Bosses.Last().Bullets)
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
                        if (Lives <= 0)
                        {
                            GameOver.Text = "Игра окончена. Вы проиграли!";
                            GameOver.Location = new Point(GameName.Left, GameName.Top);
                            GameOver.Size = new Size(ClientSize.Width / 4, ClientSize.Height / 16);
                            GameStarted = false;
                            Controls.Add(RestartGame);
                            Controls.Add(GameOver);
                            Controls.Add(CloseGame);
                            ShootTimer.Stop();
                            SpawnEnemy.Stop();
                            enemyShot.Stop();
                            enemyMove.Stop();
                            checkEnemyCount.Stop();
                            bossShoot.Stop();
                            bossMove.Stop();
                            bossNeeded.Stop();
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
            EasyDiffculty.Text = "Сложность: Легкая";
            EasyDiffculty.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 - ClientSize.Height / 6);
            EasyDiffculty.Size = StartGame.Size;
            MediumDiffculty.Text = "Сложность: Средняя";
            MediumDiffculty.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2);
            MediumDiffculty.Size = StartGame.Size;
            HardDiffculty.Text = "Сложность: Тяжелая";
            HardDiffculty.Location = new Point(ClientSize.Width / 2 - ClientSize.Width / 8, ClientSize.Height / 2 + ClientSize.Height / 6);
            HardDiffculty.Size = StartGame.Size;
            GameName.Location = new Point(StartGame.Left - 10, StartGame.Top - 100);
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
            RestartGame.Text = "Начать занового?";
            RestartGame.Location = StartGame.Location;
            RestartGame.Size = RestartGame.Size;
            FormWidth = ClientSize.Width;
            FormHeight = ClientSize.Height;
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
                g.DrawImage(PlayerPlane.PlayerStandinOnPlace, PlayerPlane.Position);
            };
            ShootTimer.Start();
            SpawnEnemy.Start();
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
                g.DrawImage(PlayerPlane.PlayerStandinOnPlace, PlayerPlane.Position);
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
                if (e.X > PlayerPlane.PlayerStandinOnPlace.Width / 2 && e.X < ClientSize.Width - PlayerPlane.PlayerStandinOnPlace.Width / 2)
                {
                    PlayerPlane.Movement(e.X - PlayerPlane.PlayerStandinOnPlace.Width / 2);
                    Invalidate();
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Escape)
            {
                GameStarted = false;                
                PauseGame.Text = "Продолжить";
                PauseGame.Location = StartGame.Location;
                PauseGame.Size = StartGame.Size;
                GameStatus.Text = "Игра приостановлена";
                GameStatus.Location = new Point(GameName.Location.X, GameName.Location.Y);
                GameStatus.Size = GameName.Size;
                Controls.Add(PauseGame);
                Controls.Add(CloseGame);
            }
        }
    }
}