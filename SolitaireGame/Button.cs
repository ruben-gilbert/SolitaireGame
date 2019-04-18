using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolitaireGame
{
    class Button
    {
        private string label;
        private int x;
        private int y;
        private int width;
        private int height;
        private bool pressed;
        private Texture2D up;
        private Texture2D down;
        private SpriteFont font;
        private BackendGame game;
        private Action action;

        public Button(BackendGame game, string label, int x, int y, int width, int height)
        {
            this.label = label;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            this.pressed = false;
            this.game = game;

            this.up = new Texture2D(this.game.Game.GraphicsDevice, 1, 1);
            this.up.SetData(new[] { Color.White });

            this.down = new Texture2D(this.game.Game.GraphicsDevice, 1, 1);
            this.down.SetData(new[] { Color.Black });
        }

        // -----------------------------------------------------------------------------------------
        // Getters / Setters

        public bool Pressed
        {
            get { return this.pressed; }
            set { this.pressed = value; }
        }

        // -----------------------------------------------------------------------------------------
        // Methods

        public void Draw(SpriteBatch s)
        {
            // TODO -- don't just draw empty boxes, draw the text and colors?
            if (this.pressed)
            {
                this.DrawEmptyZone(this.down, s, 2, Color.White);
            }
            else
            {
                this.DrawEmptyZone(this.up, s, 2, Color.White);
            }

        }

        protected void DrawEmptyZone(Texture2D tex, SpriteBatch s, int thickness, Color c)
        {
            s.Draw(tex, new Rectangle(this.x, this.y, thickness, this.height), c);
            s.Draw(tex, new Rectangle(this.x + this.width, this.y, thickness, this.height), c);
            s.Draw(tex, new Rectangle(this.x, this.y, this.width, thickness), c);
            s.Draw(tex, new Rectangle(this.x, this.y + this.height, this.width, thickness), c);
        }

        public bool IsClicked(int x, int y)
        {
            return this.x <= x && x <= this.x + this.width &&
                   this.y <= y && y <= this.y + this.height;
        }

        public void OnPress()
        {
            this.pressed = true;
        }

        public void OnRelease()
        {
            if (this.pressed)
            {
                this.action();
            }

            this.pressed = false;
        }

        public void SetAction(Action action)
        {
            this.action = action;
        }
    }
}
