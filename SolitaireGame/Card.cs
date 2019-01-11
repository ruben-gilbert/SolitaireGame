// Card.cs
// Author: Ruben Gilbert
// 2019

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolitaireGame
{
    public class Card
    {
        private string suit;
        private int value;
        private bool up;
        private Texture2D front;
        private Texture2D back;

        public Card(int value, string suit)
        {
            up = false;

            if (!Constants.VALID_SUITS.Contains(suit))
                throw new ArgumentException(String.Format("{0} is not a valid suit", "suit"));
            else
                this.suit = suit;

            if (value < 1 || value > 13)
                throw new ArgumentException(String.Format("{0} is not a valid card value", "value"));
            else
                this.value = value;
        }

        public Texture2D Front
        {
            get { return this.front; }
        }

        public Texture2D Back
        {
            get { return this.back; }
        }

        public bool Up
        {
            get { return this.up; }
        }

        public void Flip()
        {
            this.up = !this.up;
        }

        /// <summary>
        /// Loads the images for this card.  The front image depends on the 
        /// value and suit of the Card (files are named the same as the string
        /// representation).  There are multiple possible card-back colors, so
        /// we allow for that customization.
        /// </summary>
        /// <param name="game">The MainGame object this Card is attached to</param>
        /// <param name="color">The color of the card-back</param>
        public void LoadImages(MainGame game, string color)
        {
            this.front = game.Content.Load<Texture2D>("images/" + this.ToString());
            this.back = game.Content.Load<Texture2D>("images/back_" + color);
        }

        /// <summary>
        /// Card objects draw themselves
        /// </summary>
        /// <param name="s">SpriteBatch can draw 2D textures</param>
        /// <param name="x">X-coordinate of this Card</param>
        /// <param name="y">Y-coordinate of this Card</param>
        /// <param name="c">Color of the texture (will generally be White)</param>
        public void Draw(SpriteBatch s, int x, int y, Color c)
        {                                                                                           
            Rectangle r = new Rectangle(x, y, Constants.CARD_WIDTH, Constants.CARD_HEIGHT);

            if (this.up)
                s.Draw(this.front, r, c);
            else
                s.Draw(this.back, r, c);
        }

        /// <summary>
        /// Representation of a Card as a string
        /// </summary>
        /// <returns>A string of the Card's value and suit</returns>
        public override string ToString()
        {
            switch (this.value)
            {
                case 1:
                    return "A" + this.suit;
                case 11:
                    return "J" + this.suit;
                case 12:
                    return "Q" + this.suit;
                case 13:
                    return "K" + this.suit;
                default:
                    return this.value + this.suit;
            }
        }
    }
}
