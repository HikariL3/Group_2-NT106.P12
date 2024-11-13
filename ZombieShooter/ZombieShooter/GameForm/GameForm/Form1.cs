﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Net.Sockets;
using GameForm;
using System.Net.NetworkInformation;
using System.Diagnostics;
using Client;
using Server_ShootOutGame;

namespace GameForm
{
    public partial class MainGame : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "right";
        double wallHealth = 250;
        int speed = 10;
        Random randNum = new Random();
        int kill, score;
        List<Zombie> zombiesList = new List<Zombie>();
        private List<Gun> guns = new List<Gun>();
        private Gun currentGun;
        private Gun gunForOther;
        private bool canFire = true;
        int offset = 20;
        Random ranSpawn = new Random();
        int timeLeft = 120;
        private SoundManager soundManager = new SoundManager();
        private bool finalWave = false;
        private Random rand = new Random();

        private Dictionary<string, PictureBox> playerPictureBoxes = new Dictionary<string, PictureBox>();
        private Dictionary<string, Label> playerLabels = new Dictionary<string, Label>();
        PictureBox myPlayer;
        Label myName;

        private DateTime lastSentPositionTime = DateTime.Now;
        private const int POSITION_UPDATE_INTERVAL_MS = 35;

        

        public MainGame()
        {
            InitializeComponent();
            InitializeGuns();
            RestartGame();

            GameClient.OnPlayerShoot += HandleOtherPlayerShoot;
            GameClient.OnPlayerPositionUpdated += UpdatePlayerVisual;
            GameClient.OnMakeZombies += DisplayZombies;
        }

        private void HandleOtherPlayerShoot(Player player, string direction, string gunName)
        {
            // Find the appropriate gun for the player
            Gun shootingGun = guns.FirstOrDefault(g => g.Name == gunName) ?? guns[0];

            // Trigger shooting logic with the correct gun and direction
            ShootBulletForOtherPlayer(player, direction, shootingGun);
        }

        private void ShootBulletForOtherPlayer(Player player, string direction, Gun gun)
        {
            int bulletSpeed = gun.BulletSpeed;
            int bulletRange = gun.Range;
            soundManager.PlaySound(gun.Name.ToLower());

            // Instantiate bullets based on gun type
            switch (gun.Name)
            {
                case "Pistol":
                    Bullet pistolBullet = new Bullet(bulletSpeed, bulletRange);
                    pistolBullet.direction = direction;
                    pistolBullet.bulletLeft = playerPictureBoxes[player.Name].Left + (playerPictureBoxes[player.Name].Width / 2);
                    pistolBullet.bulletTop = playerPictureBoxes[player.Name].Top + (playerPictureBoxes[player.Name].Height / 2);
                    pistolBullet.MakeBullet(this);
                    break;

                case "Shotgun":
                    Random rand = new Random();
                    for (int i = 0; i < 8; i++)
                    {
                        Bullet shotgunPellet = new Bullet(bulletSpeed, bulletRange);
                        shotgunPellet.direction = direction;
                        int spreadAngle = rand.Next(-60, 61);

                        shotgunPellet.bulletLeft = playerPictureBoxes[player.Name].Left + (playerPictureBoxes[player.Name].Width / 2);
                        shotgunPellet.bulletTop = playerPictureBoxes[player.Name].Top + (playerPictureBoxes[player.Name].Height / 2);

                        ApplySpread(shotgunPellet, spreadAngle);
                        shotgunPellet.MakeBullet(this);
                    }
                    break;

                case "Sniper":
                    Bullet sniperBullet = new Bullet(bulletSpeed, bulletRange);
                    sniperBullet.direction = direction;
                    sniperBullet.bulletLeft = playerPictureBoxes[player.Name].Left + (playerPictureBoxes[player.Name].Width / 2);
                    sniperBullet.bulletTop = playerPictureBoxes[player.Name].Top + (playerPictureBoxes[player.Name].Height / 2);
                    sniperBullet.MakeBulletSniper(this);
                    break;
            }
        }

        private void InitializeGuns()
        {
            // Load images for the guns
            Gun pistol = new Gun("Pistol", 40, 12, 25, 500, 350, 1000,
                                Properties.Resources.pistolup, Properties.Resources.pistoldown,
                                Properties.Resources.pistolleft, Properties.Resources.pistolright);
            Gun shotgun = new Gun("Shotgun", 20, 3, 25, 350, 700, 1400,
                                Properties.Resources.shotgunup, Properties.Resources.shotgundown,
                                Properties.Resources.shotgunleft, Properties.Resources.shotgunright);
            Gun sniper = new Gun("Sniper", 100, 5, 50, 1200, 1000, 1600,
                                Properties.Resources.sniperup, Properties.Resources.sniperdown,
                                Properties.Resources.sniperleft, Properties.Resources.sniperright);

            guns.Add(pistol);
            guns.Add(shotgun);
            guns.Add(sniper);
            currentGun = guns[0];// Start with pistol
            txtGun.Text = "Current Gun: " + currentGun.Name;
            txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
        }

        private void MainGame_Load(object sender, EventArgs e)
        {
            soundManager.LoadSound("pistol", Properties.Resources.pistolshoot);
            soundManager.LoadSound("shotgun", Properties.Resources.shotgunshoot);
            soundManager.LoadSound("sniper", Properties.Resources.snipershoot);
            soundManager.LoadSound("reload", Properties.Resources.gunload);
            soundManager.LoadSound("switch", Properties.Resources.gswitch);
            soundManager.LoadSound("empty", Properties.Resources.empty);
            soundManager.LoadSound("groan0", Properties.Resources.Groan0);
            soundManager.LoadSound("groan1", Properties.Resources.Groan1);
            soundManager.LoadSound("finalwave", Properties.Resources.finalwave);
            soundManager.LoadSound("bzfinalwave", Properties.Resources.bzfinalwave);
            soundManager.LoadSound("brain", Properties.Resources.brain);
            soundManager.LoadSound("begin", Properties.Resources.begin);
            soundManager.PlaySound("begin");
        }
        private void UpdatePlayerVisual(Player player)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdatePlayerVisual(player)));
            }
            else
            {
                if (playerPictureBoxes.ContainsKey(player.Name))
                {
                    var playerPictureBox = playerPictureBoxes[player.Name];

                    // Update position
                    playerPictureBox.Left = (int)player.Position.X;
                    playerPictureBox.Top = (int)player.Position.Y;

                    // Update image based on current gun and direction

                    Debug.WriteLine($"Updating {player.Name}'s image to face {player.Direction}");
                    switch (player.CurrentGun.Name)
                    {
                        case "Pistol":
                            gunForOther = guns[0];
                            break;
                        case "Shotgun":
                            gunForOther = guns[1];
                            break;
                        case "Sniper":
                            gunForOther = guns[2];
                            break;
                    }

                    switch (player.Direction)
                    {
                        case "up":
                            playerPictureBox.Image = gunForOther.ImageUp;
                            break;
                        case "down":
                            playerPictureBox.Image = gunForOther.ImageDown;
                            break;
                        case "left":
                            playerPictureBox.Image = gunForOther.ImageLeft;
                            break;
                        case "right":
                            playerPictureBox.Image = gunForOther.ImageRight;
                            break;
                    }
                }
                else
                {
                    Debug.WriteLine($"No visual element found for player: {player.Name}");
                }
            }
            
        }

        private void InitializePositions()
        {
            int xOffset = 20;
            int initialY = wall.Top + 50;

            for (int i = 0; i < GameClient.joinedLobby.Players.Count; i++)
            {
                string playerName = GameClient.joinedLobby.Players[i].Name;
                int playerInitialX = wall.Left - 100 - (xOffset * i);
                int playerInitialY = initialY + (i * 100);
                gunForOther = guns[0];

                if (playerName == GameClient.localPlayer.Name)
                {
                    // Set local player position
                    player1.Location = new Point(playerInitialX, playerInitialY);
                    name1.Text = playerName;
                    name1.Location = Location = new Point(playerInitialX, playerInitialY - 15);
                    name1.AutoEllipsis = true;
                    currentGun=guns[0];
                    myPlayer = player1;
                    myName = name1;
                }
                else
                {
                    // Set position for other players
                    PictureBox playerPictureBox = new PictureBox
                    {
                        Size = new Size(56, 81),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Image = gunForOther.ImageRight,

                        Location = new Point(playerInitialX, playerInitialY)
                    };

                    Label playerLabel = new Label
                    {
                        Text = playerName,
                        BackColor = Color.FromArgb(0, 28, 32),
                        AutoSize = true,
                        AutoEllipsis = true,
                        ForeColor = SystemColors.ButtonHighlight,
                        Font = new Font("Courier New", 9F, FontStyle.Regular),
                        Location = new Point(playerInitialX, playerInitialY - 15)
                    };

                    this.Controls.Add(playerPictureBox);
                    this.Controls.Add(playerLabel);
                    playerLabel.BringToFront();
                    playerPictureBox.SendToBack();
                    playerPictureBoxes[playerName] = playerPictureBox;
                    playerLabels[playerName] = playerLabel;
                }
            }
        }

        #region CODE FOR HANDLING GAME EVENT
        //BEGIN OF-----------------------------------------------------------------------
        //----------------THESE LINES OF CODE ARE FOR HANDLING GAME EVENT----------------
        //-------------------------------------------------------------------------------

        private void ActualTime_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                txtTimer.Text = "Time: " + timeLeft;

                if (timeLeft % 7 == 0)
                {
                    int randSound = rand.Next(1, 4);
                    string sound;
                    if (randSound == 1)
                        sound = "groan0";
                    else if (randSound == 2)
                        sound = "groan1";
                    else
                        sound = "brain";

                    soundManager.PlaySound(sound);
                }
                if (timeLeft % 2 == 0 && timeLeft > 0)
                {
                    if (timeLeft <= 30)
                        GameClient.SendMakeZombies(myName.Text, "4", this.ClientSize.Height);
                    else if (timeLeft <= 60)
                        GameClient.SendMakeZombies(myName.Text, "3", this.ClientSize.Height);
                    else if (timeLeft <= 90)
                        GameClient.SendMakeZombies(myName.Text, "2", this.ClientSize.Height);
                    else if (timeLeft <= 120)
                        GameClient.SendMakeZombies(myName.Text, "1", this.ClientSize.Height);
                }
            }
            else if (!finalWave)
            {
                GameClient.SendMakeZombies(myName.Text, "FINAL_WAVE", this.ClientSize.Height);
                soundManager.PlaySound("finalwave");
                finalWave = true;
            }
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            bool hasMoved = false;

            if (timeLeft <= 0)
            {
                if (zombiesList.Count == 0)
                {
                    gameOver = true;
                    YouWin();
                }
            }

            if (wallHealth > 1)
            {
                healthBar.Value = (int)wallHealth;
            }
            else
            {
                gameOver = true;
                YouLose();
            }

            txtKill.Text = "Kills: " + kill;
            txtScore.Text = "Score: " + score;

            // Initialize movement flags
            bool canMoveLeft = goLeft && myPlayer.Left > 0;
            bool canMoveRight = goRight && myPlayer.Left + myPlayer.Width < this.ClientSize.Width;
            bool canMoveUp = goUp && myPlayer.Top > 45;
            bool canMoveDown = goDown && myPlayer.Top + myPlayer.Height < this.ClientSize.Height;


            // Check for collisions with obstacles
            foreach (Control obstacle in this.Controls)
            {
                if (obstacle is PictureBox &&
                    ((string)obstacle.Name == "wall"))
                {
                    if (myPlayer.Bounds.IntersectsWith(obstacle.Bounds))
                    {
                        if (goLeft && myPlayer.Left < obstacle.Right && myPlayer.Right > obstacle.Right)
                        {
                            canMoveLeft = false;
                        }
                        if (goRight && myPlayer.Right > obstacle.Left && myPlayer.Left < obstacle.Left)
                        {
                            canMoveRight = false;
                        }
                        if (goUp && myPlayer.Top < obstacle.Bottom && myPlayer.Bottom > obstacle.Bottom)
                        {
                            canMoveUp = false;
                        }
                        if (goDown && myPlayer.Bottom > obstacle.Top && myPlayer.Top < obstacle.Top)
                        {
                            canMoveDown = false;
                        }
                    }
                }
            }

            if (canMoveLeft)
            {
                myPlayer.Left -= speed;
                myName.Left -= speed;
                hasMoved = true;
            }
            if (canMoveRight)
            {
                myPlayer.Left += speed;
                myName.Left += speed;
                hasMoved = true;
            }
            if (canMoveUp)
            {
                myPlayer.Top -= speed;
                myName.Top -= speed;
                hasMoved = true;
            }
            if (canMoveDown)
            {
                myPlayer.Top += speed;
                myName.Top += speed;
                hasMoved = true;
            }

            if (hasMoved && (DateTime.Now - lastSentPositionTime).TotalMilliseconds >= POSITION_UPDATE_INTERVAL_MS)
            {
                GameClient.SendPlayerPosition(GameClient.localPlayer.Name, facing, myPlayer.Left, myPlayer.Top, currentGun.Name);
                lastSentPositionTime = DateTime.Now;
            }

            foreach (Zombie zombie in zombiesList.ToList())
            {
                ProcessZombie(zombie);
            }
        }
        private void ProcessZombie(Zombie zombie)
        {
            foreach (PictureBox bullet in this.Controls.OfType<PictureBox>().Where(b => b.Tag?.ToString() == "bullet").ToList())
            {
                if (zombie.ZombiePictureBox.Bounds.IntersectsWith(bullet.Bounds))
                {
                    this.Controls.Remove(bullet);
                    bullet.Dispose();

                    zombie.Health -= currentGun.Damage;
                    if (zombie.Health <= 0)
                    {
                        kill++;
                        score += zombie.Score;
                        this.Controls.Remove(zombie.ZombiePictureBox);
                        zombiesList.Remove(zombie);
                        break;
                    }
                }
            }

            // Move zombie towards the wall
            if (zombie.ZombiePictureBox.Left > wall.Right)
            {
                zombie.ZombiePictureBox.Left -= zombie.Speed;
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;
            }
            else
            {
                DamageWall(zombie);
            }
        }

        private void DamageWall(Zombie zombie)
        {
            wallHealth -= zombie.Damage * 0.01;
            //wallHealth -= 0;
            if (wallHealth <= 0)
            {
                gameOver = true;
                GameTimer.Stop();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver) return;

            switch (e.KeyCode)
            {
                case Keys.Left: goLeft = true; facing = "left"; myPlayer.Image = currentGun.ImageLeft; break;
                case Keys.Right: goRight = true; facing = "right"; myPlayer.Image = currentGun.ImageRight; break;
                case Keys.Up: goUp = true; facing = "up"; myPlayer.Image = currentGun.ImageUp; break;
                case Keys.Down: goDown = true; facing = "down"; myPlayer.Image = currentGun.ImageDown; break;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {

            if (gameOver) return;

            switch (e.KeyCode)
            {
                case Keys.Left: goLeft = false; break;
                case Keys.Right: goRight = false; break;
                case Keys.Up: goUp = false; break;
                case Keys.Down: goDown = false; break;
                case Keys.C: soundManager.PlaySound("switch"); SwitchGun(); break;
                case Keys.R: ReloadGun(); break;
                case Keys.Space: HandleShooting(); break;
                case Keys.Enter: if (gameOver) RestartGame(); break;
            }
        }
        //END OF-------------------------------------------------------------------------
        //----------------THESE LINES OF CODE ARE FOR HANDLING GAME EVENT----------------
        //-------------------------------------------------------------------------------
        #endregion

        #region CODE FOR GUN'S MECHANICS
        //BEGIN OF-----------------------------------------------------------------------
        //------------------THESE LINES OF CODE ARE FOR GUN'S MECHANICS------------------
        //-------------------------------------------------------------------------------
        private void HandleShooting()
        {
            if (!canFire) return;

            if (currentGun.CurrentAmmo > 0)
            {
                currentGun.CurrentAmmo--;
                txtAmmo.Text = $"Ammo: {currentGun.CurrentAmmo}";
                GameClient.SendShootBullet(GameClient.localPlayer.Name, facing, currentGun.Name);
                ShootBullet(facing);
                canFire = false;

                var fireRateTimer = new Timer { Interval = currentGun.FireRate };
                fireRateTimer.Tick += (s, evt) => { canFire = true; fireRateTimer.Stop(); };
                fireRateTimer.Start();

                if (currentGun.CurrentAmmo < 1)
                {
                    txtAmmo.Text = "Ammo: Out of ammo!";
                    txtState.Text = "Press R to reload!";
                }
            }
            else
            {
                soundManager.PlaySound("empty");
            }
        }

        private void SwitchGun()
        {
            int currentGunIndex = guns.IndexOf(currentGun);
            currentGunIndex = (currentGunIndex + 1) % guns.Count;
            currentGun = guns[currentGunIndex];
            txtGun.Text = "Current Gun: " + currentGun.Name;

            if (currentGun.CurrentAmmo > 0)
                txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
            else if (currentGun.CurrentAmmo < 1)
            {
                txtAmmo.Text = "Ammo: Out of ammo!";
                txtState.Text = "Press R to reload!";
            }

            GameClient.SendSwitchGun(GameClient.localPlayer.Name, currentGun.Name);

            switch (facing)
            {
                case "up":
                    myPlayer.Image = currentGun.ImageUp;
                    break;
                case "down":
                    myPlayer.Image = currentGun.ImageDown;
                    break;
                case "left":
                    myPlayer.Image = currentGun.ImageLeft;
                    break;
                case "right":
                    myPlayer.Image = currentGun.ImageRight;
                    break;
            }
        }

        private void ReloadGun()
        {
            soundManager.PlaySound("reload");
            txtGun.Text = "Reloading...";
            txtState.Text = "";

            canFire = false;

            Timer reloadTimer = new Timer();
            reloadTimer.Interval = currentGun.ReloadTime;
            reloadTimer.Tick += (s, evt) =>
            {
                currentGun.Reload();
                txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
                txtGun.Text = "Current Gun: " + currentGun.Name;
                canFire = true;
                reloadTimer.Stop();
            };
            reloadTimer.Start();
        }

        private void ShootBullet(string direction)
        {
            int bulletSpeed;
            int bulletRange;

            switch (currentGun.Name)
            {
                case "Pistol":
                    bulletSpeed = 25;
                    bulletRange = 500;
                    soundManager.PlaySound("pistol");
                    Bullet shootPistolBullet = new Bullet(bulletSpeed, bulletRange);
                    shootPistolBullet.direction = direction;
                    shootPistolBullet.bulletLeft = myPlayer.Left + (myPlayer.Width / 2);
                    shootPistolBullet.bulletTop = myPlayer.Top + (myPlayer.Height / 2);

                    shootPistolBullet.bulletTop = myPlayer.Top + (myPlayer.Height / 2) + offset;

                    shootPistolBullet.MakeBullet(this);
                    break;
                case "Shotgun":
                    bulletSpeed = 25;
                    bulletRange = 350;
                    Random rand = new Random();
                    soundManager.PlaySound("shotgun");

                    for (int i = 0; i < 8; i++)
                    {
                        Bullet shootShotgunPellet = new Bullet(bulletSpeed, bulletRange);
                        shootShotgunPellet.direction = direction;

                        int spreadAngle = rand.Next(-60, 61);

                        shootShotgunPellet.bulletLeft = myPlayer.Left + (myPlayer.Width / 2);
                        shootShotgunPellet.bulletTop = myPlayer.Top + (myPlayer.Height / 2);
                        shootShotgunPellet.bulletTop = myPlayer.Top + (myPlayer.Height / 2) + offset;

                        ApplySpread(shootShotgunPellet, spreadAngle);

                        shootShotgunPellet.MakeBullet(this);
                    }
                    break;
                case "Sniper":
                    bulletSpeed = 50;
                    bulletRange = 1200;
                    soundManager.PlaySound("sniper");
                    Bullet shootSniperBullet = new Bullet(bulletSpeed, bulletRange);
                    shootSniperBullet.direction = direction;
                    shootSniperBullet.bulletLeft = myPlayer.Left + (myPlayer.Width / 2);
                    shootSniperBullet.bulletTop = myPlayer.Top + (myPlayer.Height / 2);
                    shootSniperBullet.bulletTop = myPlayer.Top + (myPlayer.Height / 2) + offset;
                    shootSniperBullet.MakeBulletSniper(this);
                    break;
                default:
                    bulletSpeed = 20;
                    bulletRange = 500;
                    break;
            }

        }

        private void ApplySpread(Bullet bullet, int spreadAngle)
        {
            switch (bullet.direction)
            {
                case "up":
                    bullet.bulletLeft += (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
                case "down":
                    bullet.bulletLeft -= (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
                case "left":
                    bullet.bulletTop += (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
                case "right":
                    bullet.bulletTop -= (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
            }
        }

        //END OF-------------------------------------------------------------------------
        //------------------THESE LINES OF CODE ARE FOR GUN'S MECHANICS------------------
        //-------------------------------------------------------------------------------
        #endregion

        #region CODE FOR MAKING ZOMBIES SPAWN
        //BEGIN OF-----------------------------------------------------------------------
        //---------------THESE LINES OF CODE ARE FOR MAKING ZOMBIES SPAWN----------------
        //-------------------------------------------------------------------------------

        private void DisplayZombies(string[] a)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => DisplayZombies(a)));
            }
            else
            {
                if (a[3] != "NONE") soundManager.PlaySound(a[3]);
                int type = int.Parse(a[1]);
                Zombie zombie = Zombie.CreateZombie(type);

                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;
                zombie.ZombiePictureBox.Top = int.Parse(a[2]);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
        }

        //END OF-------------------------------------------------------------------------
        //---------------THESE LINES OF CODE ARE FOR MAKING ZOMBIES SPAWN----------------
        //-------------------------------------------------------------------------------
        #endregion

        private async void YouWin()
        {
            await Task.Delay(5);
            GameTimer.Stop();
            ActualTime.Stop();
            MessageBox.Show("Game Over! You defeated all the zombies!", "You win!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            GameClient.SendData($"STATS;{kill};{score}");
            Win winForm = new Win();
            winForm.ShowDialog();
            this.Hide();
        }

        private async void YouLose()
        {
            await Task.Delay(5);
            GameTimer.Stop();
            ActualTime.Stop();
            MessageBox.Show("Game Over! You are dead, the zombies destroyed your wall", "You lose!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            GameClient.SendData($"STATS;{kill};{score}");
            Lose loseForm = new Lose();
            loseForm.ShowDialog();
            this.Hide();
        }

        private void HandleServerMessage(string message)
        {
            string[] parts = message.Split(';');
            if (parts[0] == "SPAWN_ZOMBIE")
            {
                int id = int.Parse(parts[1]);
                int left = int.Parse(parts[2]);
                int top = int.Parse(parts[3]);

                Zombie zombie = Zombie.CreateZombie(4);
                zombie.ZombiePictureBox.Left = left;
                zombie.ZombiePictureBox.Top = top;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
        }
        private void OnMessageReceived(string message)
        {
            HandleServerMessage(message);
        }

        private void MainGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameClient.Disconnect();
            GameClient.ClearLobby();
            Login login = new Login();
            login.Show();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            GameClient.OnPlayerPositionUpdated -= UpdatePlayerVisual;
            GameClient.OnPlayerShoot -= HandleOtherPlayerShoot;
            GameClient.OnMakeZombies -= DisplayZombies;
        }

        private void RestartGame()
        {
            foreach (Zombie zombie in zombiesList)
            {
                this.Controls.Remove(zombie.ZombiePictureBox);
                zombie.ZombiePictureBox.Dispose();
            }
            zombiesList.Clear();

            timeLeft = 120;

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            wallHealth = 250;
            kill = 0;
            score = 0;
            currentGun.Reload();

            healthBar.Value = (int)wallHealth;
            txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;

            InitializePositions();

            canFire = true;

            GameTimer.Start();
            ActualTime.Start();
        }
    }
}