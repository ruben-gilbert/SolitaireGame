// MainGame.cs
// Author: Ruben Gilbert
// 2019

using System;
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
        #region Members
        BackendGame m_backendGame;

        private bool m_gameOver;
        private bool m_isWrittenToFile;

        GraphicsDeviceManager m_graphics;
        
        private MouseState m_currentState;
        private MouseState m_oldState;

        SpriteBatch m_spriteBatch;

        private string m_endTime;

        private Texture2D m_blankBox;
        #endregion

        #region Properties
        public Texture2D BlankBox
        {
            get => m_blankBox;
        }
        #endregion

        public MainGame()
        {
            m_graphics = new GraphicsDeviceManager(this);

            //GameProperties.WINDOW_WIDTH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //GameProperties.WINDOW_HEIGHT = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            int monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Properties.WindowWidth = monitorWidth > 1600 ? 1600 : monitorWidth;
            Properties.WindowHeight = monitorHeight > 900 ? 900 : monitorHeight;

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

            Properties.TableStart = Properties.WindowHeight / 3;

            m_graphics.PreferredBackBufferWidth = Properties.WindowWidth;
            m_graphics.PreferredBackBufferHeight = Properties.WindowHeight;
            m_graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initializes the backend game object, turn the mouse on, and 
        /// initializes the mouse state vars
        /// </summary>
        protected override void Initialize()
        {

            m_backendGame = new BackendGame(this);
            IsMouseVisible = true;
            m_currentState = Mouse.GetState();
            m_oldState = Mouse.GetState();
            m_gameOver = false;
            m_isWrittenToFile = false;

            base.Initialize();
        }

        /// <summary>
        /// Creates the SpriteBatch for this game and initializes and content we need.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            m_blankBox = new Texture2D(GraphicsDevice, 1, 1);
            m_blankBox.SetData(new[] { Color.White });

            // Create the SpriteFont object
            //SpriteFont victoryFont = Content.Load<SpriteFont>("Victory");
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
            // Check keyboard input
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                NewGame();
            }

            // Check to see if we have any animations that need to be updated
            if (m_backendGame.Animations.Count != 0)
            {
                List<Tweener> toRemove = new List<Tweener>();

                foreach (Tweener tween in m_backendGame.Animations)
                {
                    if (tween.Valid)
                    {
                        tween.Update(gameTime);
                    }
                    else
                    {
                        toRemove.Add(tween);
                    }
                }

                if (toRemove.Count != 0)
                {
                    foreach (Tweener tween in toRemove)
                    {
                        m_backendGame.AnimationRemove(tween);
                    }
                }
            }

            // Main game check -- if game is not over or being auto-won, continue to allow playing.
            if (!m_backendGame.IsWinnable && !m_backendGame.GameOver())
            {
                m_currentState = Mouse.GetState();

                // If there is a valid selection, make sure we update its location
                if (!m_backendGame.Selection.IsEmpty())
                {
                    m_backendGame.UpdateSelection(m_currentState.X, m_currentState.Y);
                }

                // If the left-mouse button is pushed down, prepare for a click 
                if (m_currentState.LeftButton == ButtonState.Pressed &&
                    m_oldState.LeftButton == ButtonState.Released)
                {
                    m_backendGame.HandleMouseDown(m_currentState.X, m_currentState.Y);
                }
                // If the left mouse button is released, handle it
                else if (m_currentState.LeftButton == ButtonState.Released &&
                    m_oldState.LeftButton == ButtonState.Pressed)
                {
                    m_backendGame.HandleMouseUp(m_currentState.X, m_currentState.Y);
                }
                // If the right mouse button is pressed AND the left mouse button isn't down
                else if (m_currentState.RightButton == ButtonState.Pressed &&
                        m_oldState.RightButton == ButtonState.Released &&
                        m_currentState.LeftButton != ButtonState.Pressed)
                {
                    m_backendGame.HandleRightClick(m_currentState.X, m_currentState.Y);
                }

                m_oldState = m_currentState;
            }
            else if (m_backendGame.IsWinnable && !m_backendGame.GameOver())
            {
                // If we are able to auto-win and there are no cards currently being processed
                // for winning, grad another card and start it animating
                if (m_backendGame.Animations.Count == 0)
                {
                    Tuple<Tableau, Foundation> step = m_backendGame.NextAutoWinStep();
                    Tweener tween = new Tweener(m_backendGame, step.Item1, step.Item2, 1);
                    m_backendGame.AnimationAdd(tween);
                }
            }
            else
            {
                // TODO -- do the card "spilling" out of foundations animation?
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

            m_spriteBatch.Begin();
            m_backendGame.Draw(m_spriteBatch);

            /*
            if (gameOver)
            {
                Vector2 vSize = font.MeasureString("VICTORY!");
                Vector2 sSize = font.MeasureString("Final Score: " + m_backendGame.GetScore());
                int x = GameProperties.WINDOW_WIDTH / 2;
                int y = GameProperties.WINDOW_HEIGHT / 2;

                spriteBatch.DrawString(font, 
                    "VICTORY!", 
                    new Vector2(x - (vSize.X / 2), y), 
                    Color.Black);
                spriteBatch.DrawString(font,
                    "Final Score: " + m_backendGame.GetScore(),
                    new Vector2(x - (sSize.X / 2), y + vSize.Y),
                    Color.Black);

                // TODO display the top scores from the file after winning a game?  Or menu option?
            }
            */
            m_spriteBatch.End();

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
            if (!writtenToFile && gameOver)
            {
                WriteScoreToFile();
            }
            */

            m_backendGame.NewGame();
            m_gameOver = false;
            m_isWrittenToFile = false;

            /*
            // Load the image textures for each Card
            foreach (Card c in m_backendGame.GetDeck().GetCards())
            {
                c.LoadImages(this, GameProperties.CARD_COLOR);
            }

            // Build the board now that textures have been loaded
            m_backendGame.BuildBoard();
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
            scores.Add(new HighScore(m_backendGame.GetScore(), endTime));
            scores.Sort();
            scores.Reverse();

            // If there are more than 5 scores, cut off any but the top 5
            if (scores.Count > 5)
            {
                scores = scores.GetRange(0, 5);
            }

            File.WriteAllLines(path, 
                Array.ConvertAll<HighScore, string>(scores.ToArray(), HighScore.ConvertHighScoreToString));

            writtenToFile = true;

        }
        */

        protected override void OnExiting(Object sender, EventArgs args)
        {
            /*
            // If the current game is won AND we haven't written to the file yet, check if 
            // we should write the score to the high score file before exiting
            if (!writtenToFile && gameOver)
            {
                WriteScoreToFile();
            }
            */

            base.OnExiting(sender, args);
        }
    }
}
