using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    class Button
    {
        #region Members
        private Action m_action;

        private readonly BackendGame m_game;

        private readonly double m_scale;

        private Point m_location;

        private SpriteFont m_font;

        private readonly string m_label;

        private Texture2D m_blankBox; // TODO -- define this in the BackendGame class?
        private Texture2D m_down;
        private Texture2D m_up;

        private Vector2 m_fontSize;
        private Vector2 m_size;
        private Vector2 m_textPadding;
        #endregion

        #region Properties
        public bool Pressed { get; set; }
        #endregion

        public Button(BackendGame game, string label, Point location, double scale = 1.2)
        {
            m_label = label;
            m_location = location;
            m_scale = scale;

            Pressed = false;
            m_game = game;

            m_blankBox = new Texture2D(game.Game.GraphicsDevice, 1, 1);
            m_blankBox.SetData(new[] { Color.White });

        }

        #region Methods
        public void Draw(SpriteBatch s)
        {
            if (Pressed)
            {
                s.Draw(m_down, new Rectangle(m_location, m_size.ToPoint()), Color.White);
                s.DrawString(m_font, m_label, new Vector2(m_location.X, m_location.Y) + m_textPadding, Color.White);
            }
            else
            {
                s.Draw(m_up, new Rectangle(m_location, m_size.ToPoint()), Color.White);
                DrawBorder(s, 2);
                s.DrawString(m_font, m_label, new Vector2(m_location.X, m_location.Y) + m_textPadding, Color.Black);

            }

        }

        protected void DrawEmptyZone(Texture2D tex, SpriteBatch s, int thickness, Color c)
        {
            //s.Draw(tex, new Rectangle(x, y, thickness, height), c);
            s.Draw(tex, new Rectangle(m_location.X, m_location.Y, thickness, m_size.ToPoint().Y), c);
            s.Draw(tex, new Rectangle(m_location.X + m_size.ToPoint().X, m_location.Y, thickness, m_size.ToPoint().Y), c);
            s.Draw(tex, new Rectangle(m_location.X, m_location.Y, m_size.ToPoint().X, thickness), c);
            s.Draw(tex, new Rectangle(m_location.X, m_location.Y + m_size.ToPoint().Y, m_size.ToPoint().X, thickness), c);
        }

        // TODO -- edges aren't tight?
        protected void DrawBorder(SpriteBatch s, int thickness)
        {
            Point topRight = new Point(m_location.X + m_size.ToPoint().X, m_location.Y);
            Point bottomRight = new Point(m_location.X + m_size.ToPoint().X, m_location.Y + m_size.ToPoint().Y);
            Point bottomLeft = new Point(m_location.X, m_location.Y + m_size.ToPoint().Y);

            m_game.DrawLine(s, m_location, topRight, thickness);
            m_game.DrawLine(s, topRight, bottomRight, thickness);
            m_game.DrawLine(s, m_location, bottomLeft, thickness);
            m_game.DrawLine(s, bottomLeft, bottomRight, thickness);
        }

        public bool IsClicked(int x, int y)
        {
            return m_location.X <= x && x <= m_location.X + m_size.X &&
                   m_location.Y <= y && y <= m_location.Y + m_size.Y;
        }

        public void LoadFont(string fontName)
        {
            m_font = m_game.Game.Content.Load<SpriteFont>(fontName);
            m_fontSize = m_font.MeasureString(m_label);
            m_size.X = (int)(m_fontSize.X * m_scale);
            m_size.Y = (int)(m_fontSize.Y * m_scale);
            m_textPadding = new Vector2((m_size.X - (int)m_fontSize.X) / 2, (m_size.Y - (int)m_fontSize.Y) / 2);

            m_up = new Texture2D(m_game.Game.GraphicsDevice, m_size.ToPoint().X, m_size.ToPoint().Y);
            m_down = new Texture2D(m_game.Game.GraphicsDevice, m_size.ToPoint().X, m_size.ToPoint().Y);
            Color[] upData = new Color[m_size.ToPoint().X * m_size.ToPoint().Y];
            Color[] downData = new Color[m_size.ToPoint().X * m_size.ToPoint().Y];

            for (int i = 0; i < upData.Length; i++)
            {
                upData[i] = Color.White;
                downData[i] = Color.Black;
            }

            m_up.SetData(upData);
            m_down.SetData(downData);
        }

        public void OnPress()
        {
            Pressed = true;
        }

        public void OnRelease()
        {
            if (Pressed)
            {
                m_action();
            }

            Pressed = false;
        }

        public void SetAction(Action action)
        {
            m_action = action;
        }
        #endregion
    }
}
