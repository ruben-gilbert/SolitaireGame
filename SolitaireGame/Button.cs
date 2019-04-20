using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolitaireGame
{
    class Button
    {
        private Action action;
        private Texture2D blankBox;
        private Texture2D down;
        private SpriteFont font;
        private Vector2 fontSize;
        private BackendGame game;
        private int height;
        private string label;
        private Vector2 position;
        private bool pressed;
        private double scale;        
        private Vector2 textPadding;
        private Texture2D up;        
        private int width;

        public Button(BackendGame game, string label, Vector2 position, double scale = 1.2)
        {
            this.label = label;
            this.position = position;
            this.scale = scale;

            this.pressed = false;
            this.game = game;

            this.blankBox = new Texture2D(this.game.Game.GraphicsDevice, 1, 1);
            this.blankBox.SetData(new[] { Color.White });

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
                s.Draw(this.down, this.position, Color.White);
                s.DrawString(this.font,
                             this.label,
                             this.position + this.textPadding,
                             Color.White);
            }
            else
            {
                s.Draw(this.up, this.position, Color.White);
                DrawBorder(s, 2);
                s.DrawString(this.font,
                             this.label,
                             this.position + this.textPadding,
                             Color.Black);

            }

        }

        protected void DrawEmptyZone(Texture2D tex, SpriteBatch s, int thickness, Color c)
        {
            //s.Draw(tex, new Rectangle(this.x, this.y, thickness, this.height), c);
            s.Draw(tex, 
                   new Rectangle(
                       (int)this.position.X, 
                       (int)this.position.Y, 
                       thickness, 
                       this.height), 
                   c);
            s.Draw(tex, 
                new Rectangle(
                    (int)this.position.X + this.width, 
                    (int)this.position.Y, 
                    thickness, 
                    this.height), 
                c);
            s.Draw(tex, 
                new Rectangle(
                    (int)this.position.X, 
                    (int)this.position.Y, 
                    this.width, 
                    thickness), 
                c);
            s.Draw(tex, 
                new Rectangle(
                    (int)this.position.X, 
                    (int)this.position.Y + this.height, 
                    this.width, 
                    thickness), 
                c);
        }

        // TODO -- edges aren't tight?
        protected void DrawBorder(SpriteBatch s, int thickness)
        {
            Vector2 tRight = new Vector2(this.position.X + this.width, this.position.Y);
            Vector2 bRight = new Vector2(this.position.X + this.width, this.position.Y + this.height);
            Vector2 bLeft = new Vector2(this.position.X, this.position.Y + this.height);

            this.DrawLine(s, this.position, tRight, thickness);
            this.DrawLine(s, tRight, bRight, thickness);
            this.DrawLine(s, this.position, bLeft, thickness);
            this.DrawLine(s, bLeft, bRight, thickness);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="s">SpriteBatch for drawing</param>
        /// <param name="start">The starting coordinates of the line</param>
        /// <param name="end">The ending coordinates of the line</param>
        /// <param name="lineWidth">How thick the line should be</param>
        private void DrawLine(SpriteBatch s, Vector2 start, Vector2 end, int lineWidth)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            s.Draw(this.blankBox,
                   new Rectangle(
                       (int)start.X,
                       (int)start.Y,
                       (int)edge.Length(),
                       lineWidth),
                   null,
                   Color.Black,
                   angle,
                   new Vector2(0, 0),
                   SpriteEffects.None,
                   0);
        }

        public bool IsClicked(int x, int y)
        {
            return this.position.X <= x && x <= this.position.X + this.width &&
                   this.position.Y <= y && y <= this.position.Y + this.height;
        }

        public void LoadFont(string fontName)
        {
            this.font = this.game.Game.Content.Load<SpriteFont>(fontName);
            this.fontSize = this.font.MeasureString(this.label);
            this.width = (int)(this.fontSize.X * this.scale);
            this.height = (int)(this.fontSize.Y * this.scale);
            this.textPadding = new Vector2((this.width - (int)this.fontSize.X) / 2,
                                           (this.height - (int)this.fontSize.Y) / 2);

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
