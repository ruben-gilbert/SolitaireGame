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
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BackendGame backendGame;
        private MouseState curState;
        private MouseState oldState;
        
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
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
            this.backendGame = new BackendGame();

            this.IsMouseVisible = true;
            this.curState = Mouse.GetState();
            this.oldState = Mouse.GetState();

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == 
                ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Get the current state of the mouse.  If it's currently pressed
            // and it used to be released, it must be a "click" we should
            // attempt to handle
            this.curState = Mouse.GetState();

            if (this.curState.LeftButton == ButtonState.Pressed &&
                this.oldState.LeftButton == ButtonState.Released)
            {
                // The backend game handles if the click is valid
                this.backendGame.MouseClicked(curState.X, curState.Y);
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
    }
}
