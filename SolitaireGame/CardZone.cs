// CardZone.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class CardZone
    {
        protected List<Card> cards;
        protected int x;
        protected int y;
        protected int width;  // potentially updated via method?
        protected int height; // potentially updated via method?
        protected int xSeparation;
        protected int ySeparation;
        protected Texture2D blankBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.CardZone"/> class.
        /// </summary>
        /// <param name="x">The x coordinate of this Zone.</param>
        /// <param name="y">The y coordinate of this Zone.</param>
        /// <param name="xSep">X sep of cards in this Zone.</param>
        /// <param name="ySep">Y sep of cards in this Zone.</param>
        /// <param name="g">A GraphicsDevice that can produce Texture2D's.</param>
        protected CardZone(int x, int y, int xSep, int ySep, GraphicsDevice g)
        {
            this.cards = new List<Card>();
            this.x = x;
            this.y = y;
            this.width = GameProperties.CARD_WIDTH;
            this.height = GameProperties.CARD_HEIGHT;
            this.xSeparation = xSep;
            this.ySeparation = ySep;
            blankBox = new Texture2D(g, 1, 1);
            blankBox.SetData(new[] { Color.White });
        }

        // -----------------------------------------------------------------------------------------
        // Getters / Setters

        public List<Card> Cards
        {
            get { return this.cards; }
        }

        public int X
        {
            get { return this.x; }
        }

        public int Y
        {
            get { return this.y; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.Height; }
        }

        // -----------------------------------------------------------------------------------------
        // Methods

        /// <summary>
        /// Add cards to this Zone.  Ensures that inner-cards become nested and update their
        /// x, y locations.  Also updates the width/height of this Zone.
        /// </summary>
        /// <param name="c">A List of one or more cards to be added to this Zone.</param>
        public virtual void AddCards(List<Card> c)
        {

            // TODO update the separation of this Zone depending on the size (if it exceeds bounds?)

            // Make sure to nest the top card
            if (!this.IsEmpty())
            {
                this.cards[this.Size() - 1].Nest();
            }

            foreach (Card card in c)
            {
                // Update the card's position
                card.X = this.x + (this.Size() * this.xSeparation);
                card.Y = this.y + (this.Size() * this.ySeparation);

                // Only update the width/height of the Zone when we start nesting cards
                if (this.Size() > 0)
                {
                    this.height += this.ySeparation;
                    this.width += this.xSeparation;
                }

                this.cards.Add(card);
            }
        }

        /// <summary>
        /// Clears this Zone.
        /// </summary>
        public void Clear()
        {
            this.cards.Clear();
        }

        /// <summary>
        /// Draws this Zone.  Either a blank box if it is empty, or have each card draw itself.
        /// </summary>
        /// <param name="s">A SpriteBatch object used for drawing in MonoGame.</param>
        public virtual void Draw(SpriteBatch s)
        {
            // If no cards, draw a black box
            if (this.IsEmpty())
            {
                this.DrawEmptyZone(this.blankBox, s, 2, Color.Black);
            }
            else
            {
                foreach (Card card in this.cards) 
                {
                    card.Draw(s, Color.White);
                }
            }
        }

        /// <summary>
        /// Helper method to draw a black box for an empty Zone.
        /// </summary>
        /// <param name="tex">The texture for the inside of the empty box (typically blank)</param>
        /// <param name="s">A SpriteBatch object used for drawing in MonoGame</param>
        /// <param name="thickness">How thick the border of the box should be</param>
        /// <param name="c">The color of the border of the box</param>
        protected void DrawEmptyZone(Texture2D tex, SpriteBatch s, int thickness, Color c)
        {
            s.Draw(tex, new Rectangle(this.x, this.y, thickness, this.height), c);
            s.Draw(tex, new Rectangle(this.x + this.width, this.y, thickness, this.height), c);
            s.Draw(tex, new Rectangle(this.x, this.y, this.width, thickness), c);
            s.Draw(tex, new Rectangle(this.x, this.y + this.height, this.width, thickness), c);
        }

        // TODO use LINQ here to instantly grab position of the clicked card?
        public virtual List<Card> GetClicked(int x, int y)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.cards[i].IsClicked(x, y, this.xSeparation, this.ySeparation))
                {
                    int clickSize = this.Size() - i;
                    return this.RemoveCards(clickSize);
                }
            }

            // Return a small, empty List if nothing clicked (shouldn't actually happen)
            return new List<Card>(1);
        }

        /// <summary>
        /// Checks to see if this CardZone was clicked
        /// </summary>
        /// <returns><c>true</c>, if this Zone was clicked, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the click</param>
        /// <param name="y">The y coordinate of the click</param>
        public virtual bool IsClicked(int x, int y)
        {
            return this.x <= x
                   && x <= this.x + this.width
                   && this.y <= y
                   && y <= this.y + this.Height;
        }

        /// <summary>
        /// Checks if this Zone is empty.
        /// </summary>
        /// <returns><c>true</c>, if the Zone is empty, <c>false</c> otherwise.</returns>
        public bool IsEmpty()
        {
            return this.cards.Count == 0;
        }

        /// <summary>
        /// Checks to see if a coordinate is within a valid range of the Zone.  Useful for
        /// determining if a selection can be placed into the Zone.
        /// </summary>
        /// <returns><c>true</c>, if the coordinates are in range, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the mouse</param>
        /// <param name="y">The y coordinate of the mouse</param>
        /// <param name="hDisplacement">The acceptable horizontal offset of the mouse</param>
        /// <param name="vDisplacement">The acceptable vertical offset of the mouse</param>
        public bool IsWithinRange(int x, int y, int hDisplacement, int vDisplacement)
        {
            return (this.x - hDisplacement <= x) 
                   && (x <= this.x + this.width + hDisplacement) 
                   && (this.y - vDisplacement <= y)
                   && (y <= this.y + this.height);
        }

        public void MoveCardsToZone(int num, CardZone dst)
        {
            // TODO -- make this generic Deal method?
        }

        /// <summary>
        /// Remove some number of cards from this Zone.  Always removes from the back, that is, 
        /// from the last index backwards (no CardZone should be able to remove from the front).
        /// Updates the height and width of the Zone and unnests the top card of the remaining
        /// Zone (if applicable).  Does NOT update the locations of the cards -- cards should be 
        /// moving from one Zone to another, therefore the AddCards method is in charge of location.
        /// </summary>
        /// <returns>Some List of cards</returns>
        /// <param name="num">The number of cards to be removed from the back of the Zone.</param>
        public virtual List<Card> RemoveCards(int num)
        {
            Debug.Assert(this.Size() >= num);

            List<Card> removed = this.cards.GetRange(this.Size() - num, num);
            this.cards.RemoveRange(this.Size() - num, num);
            this.height -= num * this.ySeparation;
            this.width -= num * this.xSeparation;

            if (!this.IsEmpty())
            {
                this.TopCard().UnNest();
            }
            else
            {
                this.width = GameProperties.CARD_WIDTH;
                this.height = GameProperties.CARD_HEIGHT;
            }

            return removed;
        }

        /// <summary>
        /// Number of cards in this Zone
        /// </summary>
        /// <returns>The number of cards in this Zone.</returns>
        public int Size()
        {
            return this.cards.Count;
        }

        public Card TopCard()
        {
            return this.cards[this.Size() - 1];
        }

        public override string ToString()
        {
            // TODO -- format a nice string?
            return "";
        }

    }
}
