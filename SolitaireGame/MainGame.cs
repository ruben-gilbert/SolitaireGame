// MainGame.cs
// Adapted from Monogame default
// Author: Ruben Gilbert
// 2019

using System;
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
        private MouseState curState;
        private MouseState oldState;
        private double clickTimer;
        const double timerDelay = 500;
        
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);

            GameProperties.WINDOW_WIDTH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            GameProperties.WINDOW_HEIGHT = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // TODO assign starting window size based on monitor size
            //Console.WriteLine(GameProperties.WINDOW_WIDTH + "x" + GameProperties.WINDOW_HEIGHT);
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

            this.backendGame = new BackendGame();

            this.IsMouseVisible = true;
            this.curState = Mouse.GetState();
            this.oldState = Mouse.GetState();
            this.clickTimer = 0;

            base.Initialize();
        }

        /// <summary>
        /// Loads the Card textures and finishes building the board
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the image textures for each Card
            foreach (Card c in this.backendGame.GetDeck().GetCards())
            {
                c.LoadImages(this, "purple");
            }

            // Build the board now that textures have been loaded
            this.backendGame.BuildBoard();
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
            if (!this.backendGame.GameOver())
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    NewGame();
                }

                // Get the current state of the mouse.  If it's currently pressed
                // and it used to be released, it must be a "click" we should
                // attempt to handle
                this.curState = Mouse.GetState();

                this.clickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (this.curState.LeftButton == ButtonState.Pressed &&
                    this.oldState.LeftButton == ButtonState.Released)
                {
                    if (this.clickTimer < timerDelay)
                    {
                        // Double Click Detected
                        this.backendGame.MouseClicked(curState.X, curState.Y, true);
                    }
                    else
                    {
                        // Single click detected
                        this.backendGame.MouseClicked(curState.X, curState.Y, false);
                    }

                    this.clickTimer = 0;

                }

                this.oldState = curState;

                base.Update(gameTime);
            }
            else
            {
                // TODO should display game over or something on screen (Sprites and Fonts)
                Console.WriteLine("Game over -- Final Score: " + this.backendGame.GetScore());

                // TODO high scores file?
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            spriteBatch.Begin();
            this.backendGame.DrawGame(GraphicsDevice, this.spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Reinitializes the properties of the Backend Game object
        /// </summary>
        protected void NewGame()
        {
            this.backendGame.NewGame();

            // Load the image textures for each Card
            foreach (Card c in this.backendGame.GetDeck().GetCards())
            {
                c.LoadImages(this, "purple");
            }

            // Build the board now that textures have been loaded
            this.backendGame.BuildBoard();
        }
    }
}
