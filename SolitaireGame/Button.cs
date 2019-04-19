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
        private Vector2 fontSize;
        private BackendGame game;
        private Action action;

        public Button(BackendGame game, string label, int x, int y)
        {
            this.label = label;
            this.x = x;
            this.y = y;

            this.pressed = false;
            this.game = game;

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
            if (this.pressed)
            {
                s.Draw(this.down, new Vector2(this.x, this.y), Color.White);
                s.DrawString(this.font,
                             this.label,
                             new Vector2(this.x, this.y),
                             Color.White);
                //this.DrawEmptyZone(this.down, s, 2, Color.White);
            }
            else
            {
                s.Draw(this.up, new Vector2(this.x, this.y), Color.White);
                s.DrawString(this.font,
                             this.label,
                             new Vector2(this.x, this.y),
                             Color.Black);
                //this.DrawEmptyZone(this.up, s, 2, Color.White);
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

        public void LoadFont(string fontName)
        {
            this.font = this.game.Game.Content.Load<SpriteFont>(fontName);
            this.fontSize = this.font.MeasureString(this.label);
            this.width = (int)this.fontSize.X;
            this.height = (int)this.fontSize.Y;

            this.up = new Texture2D(this.game.Game.GraphicsDevice, this.width, this.height);
            this.down = new Texture2D(this.game.Game.GraphicsDevice, this.width, this.height);
            Color[] upData = new Color[this.width * this.height];
            Color[] downData = new Color[this.width * this.height];

            for (int i = 0; i < upData.Length; i++)
            {
                upData[i] = Color.White;
                downData[i] = Color.Black;
            }

            this.up.SetData(upData);
            this.down.SetData(downData);
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
