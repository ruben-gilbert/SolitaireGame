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
        private int x;
        private int y;
        private int height;
        private int width;
        private readonly string suit;
        private readonly int val;
        private bool isUp;
        private bool isNested;
        private Texture2D front;
        private Texture2D back;
        private CardZone source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Card"/> class.
        /// </summary>
        /// <param name="val">The Card's value.</param>
        /// <param name="suit">The Card's suit.</param>
        public Card(int val, string suit)
        {
            this.isUp = false;
            this.isNested = true;
            this.width = GameProperties.CARD_WIDTH;
            this.height = GameProperties.CARD_HEIGHT;

            if (!GameProperties.VALID_SUITS.Contains(suit))
                throw new ArgumentException(String.Format("{0} is not a valid suit", "suit"));
            else
                this.suit = suit;

            if (val < 1 || val > 13)
                throw new ArgumentException(String.Format("{0} is not a valid card val", "val"));
            else
                this.val = val;
        }

        // -----------------------------------------------------------------------------------------
        // Getters / Setters

        /// <summary>
        /// Gets the Texture of the back of this Card.
        /// </summary>
        /// <value>The back Texture.</value>
        public Texture2D Back
        {
            get { return this.back; }
        }

        /// <summary>
        /// Gets the Texture of the front of this Card.
        /// </summary>
        /// <value>The front Texture</value>
        public Texture2D Front
        {
            get { return this.front; }
        }

        /// <summary>
        /// Gets or sets the height of this Card.
        /// </summary>
        /// <value>The new height value.</value>
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:SolitaireGame.Card"/> is nested.
        /// </summary>
        /// <value><c>true</c> if the card is nested; otherwise, <c>false</c>.</value>
        public bool IsNested
        {
            get { return this.isNested; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:SolitaireGame.Card"/> is up.
        /// </summary>
        /// <value><c>true</c> if up; otherwise, <c>false</c>.</value>
        public bool IsUp
        {
            get { return this.isUp; }
        }

        public CardZone Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        /// <summary>
        /// Gets the suit of this Card.
        /// </summary>
        /// <value>NULL</value>
        public string Suit
        {
            get { return this.suit; }
        }

        /// <summary>
        /// Gets the value of this Card.
        /// </summary>
        /// <value>NULL</value>
        public int Val
        {
            get { return this.val; }
        }

        /// <summary>
        /// Gets or sets the width of this card.
        /// </summary>
        /// <value>The new width value.</value>
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        /// <summary>
        /// Gets or sets the x coordinate of this Card.
        /// </summary>
        /// <value>The new x value</value>
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets or sets the y coordinate of this Card.
        /// </summary>
        /// <value>The new y value</value>
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        // -----------------------------------------------------------------------------------------
        // Methods

        /// <summary>
        /// Card objects draw themselves
        /// </summary>
        /// <param name="s">SpriteBatch can draw 2D textures</param>
        /// <param name="c">Color of the texture (will generally be White)</param>
        public void Draw(SpriteBatch s, Color c)
        {
            Rectangle r = new Rectangle(this.x, this.y, this.width, this.height);

            if (this.isUp)
                s.Draw(this.front, r, c);
            else
                s.Draw(this.back, r, c);
        }

        /// <summary>
        /// Flip this Card.
        /// </summary>
        public void Flip()
        {
            this.isUp = !this.isUp;
        }

        /// <summary>
        /// Checks if this Card was hit by a click (x, y coordinate)
        /// </summary>
        /// <returns><c>true</c>, if Card was clicked, <c>false</c> otherwise.</returns>
        /// <param name="clickX">X coordinate of click</param>
        /// <param name="clickY">Y coordinate of click</param>
        /// <param name="xSep">Horizontal separation between stacked cards</param>
        /// <param name="ySep">Vertical separation between stacked cards</param>
        public bool IsClicked(int clickX, int clickY, int xSep, int ySep)
        {
            // If the card is nested, only allow a subsection of it to be clicked
            if (this.IsNested)
            {
                return (this.x <= clickX && clickX <= this.x + xSep)
                    && (this.y <= clickY && clickY <= this.y + ySep);
            }
            else
            {
                return (this.x <= clickX && clickX <= this.x + this.width)
                    && (this.y <= clickY && clickY <= this.y + this.height);
            }
        }

        /// <summary>
        /// Checks if this card and some other card are opposite suits
        /// </summary>
        /// <returns><c>true</c>, if the suits are opposite, <c>false</c> otherwise.</returns>
        /// <param name="other">Some other Card object</param>
        public bool IsOppositeSuit(Card other)
        {
            if (this.suit.Equals("H") || this.suit.Equals("D"))
            {
                return other.Suit.Equals("S") || other.Suit.Equals("C");
            }
            else
            {
                return other.Suit.Equals("H") || other.Suit.Equals("D");
            }
        }

        /// <summary>
        /// Compares the suit of this Card with some other Card.
        /// </summary>
        /// <param name="other">Some other Card object</param>
        /// <returns>true if the suits are the same, false otherwise</returns>
        public bool IsSameSuit(Card other)
        {
            return this.suit.Equals(other.Suit);
        }

        /// <summary>
        /// Loads the images for this card.  The front image depends on the 
        /// value and suit of the Card (files are named the same as the string
        /// representation).  There are multiple possible card-back colors, so
        /// we allow for that customization.
        /// </summary>
        /// <param name="game">The MainGame object this Card is attached to</param>
        /// <param name="color">The color of the card-back</param>
        public void LoadImage(MainGame game, string color)
        {
            this.front = game.Content.Load<Texture2D>("images/" + this.ToString());
            this.back = game.Content.Load<Texture2D>("images/back_" + color);
        }

        /// <summary>
        /// Makes the card face down
        /// </summary>
        public void MakeFaceDown()
        {
            this.isUp = false;
        }

        /// <summary>
        /// Makes the card face up.
        /// </summary>
        public void MakeFaceUp()
        {
            this.isUp = true;
        }

        /// <summary>
        /// Nest this Card
        /// </summary>
        public void Nest()
        {
            this.isNested = true;
        }

        /// <summary>
        /// Representation of a Card as a string
        /// </summary>
        /// <returns>A string of the Card's value and suit</returns>
        public override string ToString()
        {
            switch (this.val)
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
                    return this.val + this.suit;
            }
        }

        /// <summary>
        /// Unnest this card
        /// </summary>
        public void UnNest()
        {
            this.isNested = false;
        }
    }
}
