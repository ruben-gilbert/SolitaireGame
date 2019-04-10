// MainGame.cs
// Adapted from Monogame default
// Author: Ruben Gilbert
// 2019

using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SolitaireGame
{
    /// <summary>
    /// The MainGame class inherits the Monogame Game class.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BackendGame backendGame;
        private SpriteFont font;
        private MouseState curState;
        private MouseState oldState;
        private double clickTimer;
        private bool gameOver;
        private string endTime;
        private bool writtenToFile;
        
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);

            //GameProperties.WINDOW_WIDTH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //GameProperties.WINDOW_HEIGHT = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            int monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            GameProperties.WINDOW_WIDTH = monitorWidth > 1600 ? 1600 : monitorWidth;
            GameProperties.WINDOW_HEIGHT = monitorHeight > 900 ? 900 : monitorHeight;

            // TODO make card sizes dynamic based on window size?
            // TODO make window size dynamic based on height of screen
            // TODO make UI location dynamic based on window size?
            /*
            if (GameProperties.WINDOW_HEIGHT > 1440)
            {
                GameProperties.WINDOW_WIDTH = 1280;
                GameProperties.WINDOW_HEIGHT = 1280;
            }
            else if (GameProperties.WINDOW_HEIGHT > 720)
            {
                GameProperties.WINDOW_WIDTH = 1000;
                GameProperties.WINDOW_HEIGHT = 1000;
            }
            */

            /*
            if (GameProperties.WINDOW_WIDTH > 1920 && GameProperties.WINDOW_WIDTH > 1080)
            {
                GameProperties.WINDOW_WIDTH = 1920;
                GameProperties.WINDOW_HEIGHT = 1080;
            } 
            else if (GameProperties.WINDOW_WIDTH > 1280 && GameProperties.WINDOW_HEIGHT > 720)
            {
                GameProperties.WINDOW_WIDTH = 1280;
                GameProperties.WINDOW_HEIGHT = 720;
            }
            */

            GameProperties.TABLE_START = GameProperties.WINDOW_HEIGHT / 3;

            graphics.PreferredBackBufferWidth = GameProperties.WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = GameProperties.WINDOW_HEIGHT;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initializes the backend game object, turn the mouse on, and 
        /// initializes the mouse state vars
        /// </summary>
        protected override void Initialize()
        {
            // TODO Use Myra UI (or some UI builder? emptykeys?) for the following:
                // add resolution selections (720p, 1080p, fullscreen?)
                // add new game button
                // add cards per deal option
                // main menu (number of cards to draw, instructions, etc)  -- or no?
                // add card color selection (and unload the card backs of other color?)
                // button to show top scores?

            this.backendGame = new BackendGame(GraphicsDevice, this);

            this.IsMouseVisible = true;
            this.curState = Mouse.GetState();
            this.oldState = Mouse.GetState();
            this.clickTimer = 0;
            this.gameOver = false;
            this.writtenToFile = false;

            base.Initialize();
        }

        /// <summary>
        /// Loads the Card textures and finishes building the board
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create the SpriteFont object
            this.font = Content.Load<SpriteFont>("Victory");

            /*
            // Load the image textures for each Card
            foreach (Card c in this.backendGame.GetDeck().GetCards())
            {
                c.LoadImages(this, GameProperties.CARD_COLOR);
            }
            */

            // Build the board now that textures have been loaded
            //this.backendGame.BuildBoard();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // No content to unload
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                NewGame();
            }


            if (!this.backendGame.GameOver())
            {
                this.curState = Mouse.GetState();
                this.clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                // If there is a valid selection, make sure we update its location
                if (!this.backendGame.Selection.IsEmpty())
                {
                    this.backendGame.UpdateSelection(this.curState.X, this.curState.Y);
                }

                // If the left-mouse button is pushed down, prepare for a click 
                if (this.curState.LeftButton == ButtonState.Pressed &&
                    this.oldState.LeftButton == ButtonState.Released)
                {
                    this.backendGame.HandleMouseDown(this.curState.X, this.curState.Y);
                }

                // If the mouse button is released, handle either single- or double-click
                if (this.curState.LeftButton == ButtonState.Released &&
                    this.oldState.LeftButton == ButtonState.Pressed)
                {
                    if (this.clickTimer <= GameProperties.DOUBLE_CLICK_SPEED)
                    {
                        this.backendGame.Selection.ReturnToSource();
                        this.backendGame.HandleDoubleClick(this.curState.X, this.curState.Y);
                    }
                    else
                    {
                        this.backendGame.HandleMouseUp(this.curState.X, this.curState.Y);
                    }

                    this.clickTimer = 0;
                }

                /*
                else if (this.curState.RightButton == ButtonState.Pressed &&
                        this.oldState.RightButton == ButtonState.Released)
                {

                    if (this.backendGame.CanAutoComplete())
                    {
                        Thread t = new Thread(this.backendGame.AutoComplete);
                        t.Start();
                    }

                }
                */

                this.oldState = curState;
            }
            else
            {
                this.gameOver = true;
                //this.endTime = DateTime.Now.ToShortDateString();
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            spriteBatch.Begin();
            this.backendGame.Draw(GraphicsDevice, this.spriteBatch);

            /*
            if (this.gameOver)
            {
                Vector2 vSize = font.MeasureString("VICTORY!");
                Vector2 sSize = font.MeasureString("Final Score: " + this.backendGame.GetScore());
                int x = GameProperties.WINDOW_WIDTH / 2;
                int y = GameProperties.WINDOW_HEIGHT / 2;

                this.spriteBatch.DrawString(font, 
                    "VICTORY!", 
                    new Vector2(x - (vSize.X / 2), y), 
                    Color.Black);
                this.spriteBatch.DrawString(font,
                    "Final Score: " + this.backendGame.GetScore(),
                    new Vector2(x - (sSize.X / 2), y + vSize.Y),
                    Color.Black);

                // TODO display the top scores from the file after winning a game?  Or menu option?
            }
            */
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Reinitializes the properties of the Backend Game object
        /// </summary>
        protected void NewGame()
        {
            /*
            // If the current game is won AND we haven't written to the file yet, check if 
            // we should write the score to the high score file
            if (!this.writtenToFile && this.gameOver)
            {
                this.WriteScoreToFile();
            }
            */

            this.backendGame.NewGame(GraphicsDevice, this);
            this.gameOver = false;
            this.writtenToFile = false;

            /*
            // Load the image textures for each Card
            foreach (Card c in this.backendGame.GetDeck().GetCards())
            {
                c.LoadImages(this, GameProperties.CARD_COLOR);
            }

            // Build the board now that textures have been loaded
            this.backendGame.BuildBoard();
            */
        }

        /*
        /// <summary>
        /// Writes the top 5 scores to the highscores.txt file.  If there are less than 5
        /// scores, all scores are re-appended.  If there are 5 scores, determines if the current 
        /// score is a new top 5 score, and then appends in the top 5 scores.
        /// </summary>
        protected void WriteScoreToFile()
        {
            string path = "./highscores.txt";

            List<HighScore> scores = new List<HighScore>();

            // Create the file if it doesn't exist
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            // Read in all the high scores from the file
            using(StreamReader r = new StreamReader(path, true))
            {
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    string[] elements = line.Split(',');

                    int s = Int32.Parse(elements[0]);
                    string date = elements[1];

                    scores.Add(new HighScore(s, date));
                }
            }

            // Add the current score to the list of high scores and sort them (and reverse order)
            scores.Add(new HighScore(this.backendGame.GetScore(), this.endTime));
            scores.Sort();
            scores.Reverse();

            // If there are more than 5 scores, cut off any but the top 5
            if (scores.Count > 5)
            {
                scores = scores.GetRange(0, 5);
            }

            File.WriteAllLines(path, 
                Array.ConvertAll<HighScore, string>(scores.ToArray(), HighScore.ConvertHighScoreToString));

            this.writtenToFile = true;

        }
        */

        protected override void OnExiting(Object sender, EventArgs args)
        {
            /*
            // If the current game is won AND we haven't written to the file yet, check if 
            // we should write the score to the high score file before exiting
            if (!this.writtenToFile && this.gameOver)
            {
                this.WriteScoreToFile();
            }
            */

            base.OnExiting(sender, args);
        }
    }
}
