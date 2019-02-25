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

            Constants.WINDOW_WIDTH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Constants.WINDOW_HEIGHT = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Constants.TABLE_START = Constants.WINDOW_HEIGHT / 3;

            graphics.PreferredBackBufferWidth = Constants.WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.WINDOW_HEIGHT;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initializes the backend game object, turn the mouse on, and 
        /// initializes the mouse state vars
        /// </summary>
        protected override void Initialize()
        {
            // TODO set screen size to be full screen based on the monitor?
            // TODO main menu (number of cards to draw, instructions, etc)

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
