using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Dodge_Game
{
    public partial class Form1 : Form
    {
        // draw a hero character and initialize speed

        Rectangle hero = new Rectangle(300, 300, 20, 20);

        int heroSpeed = 4;
        // lists to keep ball values and brushes to paint the screen

        List<Rectangle> ballList = new List<Rectangle>();
        List<int> ballSpeedX = new List<int>();
        List<int> ballSpeedY = new List<int>();
        List<string> ballSizes = new List<string>();
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush heroBrush = new SolidBrush(Color.Orange);


        // random value to determine ball size

        int randValue = 0;
        
        // new random generator

        Random randGen = new Random();

        // bools to check for keyboard presses

        bool aDown = false;
        bool dDown = false;
        bool wDown = false;
        bool sDown = false;

        new Stopwatch stopWatch = new Stopwatch();

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Space:
                    // send player to start game method if game timer is deactivated
                    if (gameTimer.Enabled == false)
                    {
                        startGame();
                    }
                    break;
                case Keys.Escape:
                    // close application if gametimer is deactivated

                    if (gameTimer.Enabled == false)
                    {
                        Application.Exit();
                    }
                    break;
            }
        }
        //code for start screen
        private void startGame()
        {
            stopWatch.Reset();
            stopWatch.Restart();
            ballList.Clear();
            hero.X = 300;
            hero.Y = 300;
            startLabel.Visible = false;
            startLabel2.Visible = false;
            gameTimer.Start();
            
        }
        // void for when the player collides with a ball
        private void retryGame()
        {
            stopWatch.Stop();
            hero.X = 600;
            hero.Y = 600;
            ballList.Clear();
            gameTimer.Stop();
            startLabel.Visible = true;
            startLabel2.Visible = true;
            startLabel.Text = "You Lost! Try Again?";
            
        }
        // void for code once game is won
        private void winGame()
        {
            stopWatch.Stop();
            hero.X = 600;
            hero.Y = 600;
            ballList.Clear();
            gameTimer.Stop();
            startLabel.Visible = true;
            startLabel2.Visible = true;
            startLabel.Text = "You Escaped! Play Again?";
        }
        public Form1()
        {
            InitializeComponent();

            randValue = randGen.Next(1, 4);

        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //timer in top right corner
            timeLabel.Text = stopWatch.Elapsed.ToString(@"ss");

            //if 20 seconds pass, you win
            if (stopWatch.ElapsedMilliseconds > 20000)
            {
                winGame();
            }
            //ball generator
            randValue = randGen.Next(0, 100);
            int randBallValue = randGen.Next(5, 25);
            for (int i = 0; i < ballList.Count; i++)
            {
                
                int y = ballList[i].Y + ballSpeedY[i];
                int x = ballList[i].X + ballSpeedX[i];
                ballList[i] = new Rectangle(x, y, ballList[i].Width, ballList[i].Height );
                
            }
            //determine ball size
            if (randValue <= 50)
            {

                randValue = randGen.Next(1, 6);

                switch (randValue)
                {
                    case 1:
                        //large ball
                        randBallValue = 20;
                        break;
                    case 2:
                        //smallish ball
                        randBallValue = 7;
                        break;
                    case 3:
                        //medium ball
                        randBallValue = 10;
                        break;
                    case 4:
                        //medium+ ball
                        randBallValue = 12;
                        break;
                    case 5:
                        //larger- ball
                        randBallValue = 17;
                        break;
                }

                //after determine size, generate ball with random speed and determined size

                randValue = randGen.Next(10, this.Width - 10);
                Rectangle ball = new Rectangle(randValue, 0, randBallValue, randBallValue);
                ballList.Add(ball);
                ballSpeedX.Add(randGen.Next(-7, 7));
                ballSpeedY.Add(randGen.Next(4, 10));

            }
            //when ball leaves screen (y = 600)

            for (int i = 0; i < ballList.Count; i++)
            {
                if (ballList[i].Y >= this.Height )
                {
                    ballList.RemoveAt(i);
                    ballSpeedX.RemoveAt(i);
                    ballSpeedY.RemoveAt(i);
                    
                }
            }
            // intersect with player

            for (int i = 0; i < ballList.Count; i++)
            {
                if (ballList[i].IntersectsWith(hero))
                {
                    ballList.RemoveAt(i);
                    ballSpeedX.RemoveAt(i);
                    ballSpeedY.RemoveAt(i);

                    retryGame();
                }
                
            }

            //move player
            if (dDown == true && hero.X < (this.Width - hero.Width))
            {
                hero.X += heroSpeed;
            }

            if (aDown == true && hero.X > 0)
            {
                hero.X -= heroSpeed;
            }

            if (sDown == true && hero.Y < (this.Height - hero.Height))
            {
                hero.Y += heroSpeed;
            }

            if (wDown == true && hero.Y > 3)
            {
                hero.Y -= heroSpeed;
            }

            Refresh();

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //draw heros
            e.Graphics.FillRectangle(heroBrush, hero);

            //draw balls
            for (int i = 0; i < ballList.Count; i++)
            {
                e.Graphics.FillEllipse(ballBrush, ballList[i]);
            }
        }
    }
}

