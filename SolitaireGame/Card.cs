// Card.cs
// Author: Ruben Gilbert
// 2019

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{ 
    public class Card
    {
        #region Constants
        public readonly static int Width = 131;
        public readonly static int Height = 200;
        #endregion

        #region Enums
        [Flags] public enum Suits
        {
            Diamonds    = 1 << 0,
            Hearts      = 1 << 1,
            Clubs       = 1 << 2,
            Spades      = 1 << 3
        };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Texture of the back of this Card.
        /// </summary>
        /// <value>The back Texture.</value>
        public Texture2D Back { get; private set; }

        /// <summary>
        /// Gets the Texture of the front of this Card.
        /// </summary>
        /// <value>The front Texture</value>
        public Texture2D Front { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:SolitaireGame.Card"/> is nested.
        /// </summary>
        /// <value><c>true</c> if the card is nested; otherwise, <c>false</c>.</value>
        public bool IsNested { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:SolitaireGame.Card"/> is up.
        /// </summary>
        /// <value><c>true</c> if up; otherwise, <c>false</c>.</value>
        public bool IsUp { get; private set; }

        /// <summary>
        /// The coordinates of the top-left corner of the card.
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// The width and height of the card.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// The CardZone where this card lives.
        /// </summary>
        public CardZone Source { get; set; }

        /// <summary>
        /// Gets the suit of this Card.
        /// </summary>
        /// <value>NULL</value>
        public Suits Suit { get; }

        /// <summary>
        /// Gets the value of this Card.
        /// </summary>
        /// <value>NULL</value>
        public int Value { get; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Card"/> class.
        /// </summary>
        /// <param name="value">The Card's value.</param>
        /// <param name="suit">The Card's suit.</param>
        public Card(int value, Suits suit)
        {
            IsUp = false;
            IsNested = true;
            Size = new Vector2(Width, Height);

            if (!Enum.IsDefined(typeof(Suits), suit))
                throw new ArgumentException(String.Format("{0} is not a valid suit", suit));
            else
                Suit = suit;

            if (value < 1 || value > 13)
                throw new ArgumentException(String.Format("{0} is not a valid card val", value));
            else
                Value = value;
        }

        #region Methods
        /// <summary>
        /// Card objects draw themselves
        /// </summary>
        /// <param name="s">SpriteBatch can draw 2D textures</param>
        /// <param name="color">Color of the texture (will generally be White)</param>
        public void Draw(SpriteBatch s, Color color)
        {
            Rectangle rectangle = new Rectangle(Location, Size.ToPoint());

            if (IsUp)
                s.Draw(Front, rectangle, color);
            else
                s.Draw(Back, rectangle, color);
        }

        /// <summary>
        /// Flip this Card.
        /// </summary>
        public void Flip()
        {
            IsUp = !IsUp;
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
            if (IsNested)
            {
                return (Location.X <= clickX && clickX <= Location.X + xSep)
                    && (Location.Y <= clickY && clickY <= Location.Y + ySep);
            }
            else
            {
                return (Location.X <= clickX && clickX <= Location.X + Size.X)
                    && (Location.Y <= clickY && clickY <= Location.Y + Size.Y);
            }
        }

        /// <summary>
        /// Checks if this card and some other card are opposite suits
        /// </summary>
        /// <returns><c>true</c>, if the suits are opposite, <c>false</c> otherwise.</returns>
        /// <param name="other">Some other Card object</param>
        public bool IsOppositeSuit(Card other)
        {
            if (Suit.Equals(Suits.Hearts) || Suit.Equals(Suits.Diamonds))
            {
                return other.Suit.Equals(Suits.Spades) || other.Suit.Equals(Suits.Clubs);
            }
            else
            {
                return other.Suit.Equals(Suits.Hearts) || other.Suit.Equals(Suits.Diamonds);
            }
        }

        /// <summary>
        /// Compares the suit of this Card with some other Card.
        /// </summary>
        /// <param name="other">Some other Card object</param>
        /// <returns>true if the suits are the same, false otherwise</returns>
        public bool IsSameSuit(Card other)
        { 
            return other != null ? Suit.Equals(other.Suit) : false;
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
            Front = game.Content.Load<Texture2D>("images/" + ToString());
            Back = game.Content.Load<Texture2D>("images/back_" + color);
        }

        /// <summary>
        /// Makes the card face down
        /// </summary>
        public void MakeFaceDown()
        {
            IsUp = false;
        }

        /// <summary>
        /// Makes the card face up.
        /// </summary>
        public void MakeFaceUp()
        {
            IsUp = true;
        }

        /// <summary>
        /// Nest this Card
        /// </summary>
        public void Nest()
        {
            IsNested = true;
        }

        /// <summary>
        /// Representation of a Card as a string
        /// </summary>
        /// <returns>A string of the Card's value and suit</returns>
        public override string ToString()
        {
            string shortSuit = Suit.ToString().Substring(0, 1);
            string valueString;

            switch (Value)
            {
                case 1:
                    valueString = "A";
                    break;
                case 11:
                    valueString = "J";
                    break;
                case 12:
                    valueString = "Q";
                    break;
                case 13:
                    valueString = "K";
                    break;
                default:
                    valueString = Value.ToString();
                    break;
            }

            return valueString + shortSuit;
        }

        /// <summary>
        /// Unnest this card
        /// </summary>
        public void UnNest()
        {
            IsNested = false;
        }
        #endregion
    }
}
