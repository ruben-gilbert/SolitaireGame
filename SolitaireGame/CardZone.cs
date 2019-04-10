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
            get { return this.height; }
        }

        public int XSeparation
        {
            get { return this.xSeparation; }
        }

        public int YSeparation
        {
            get { return this.ySeparation; }
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
        /// Shorthand method for getting the card at position 0.
        /// </summary>
        /// <returns>The Card object at position 0.</returns>
        public Card BottomCard()
        {
            if (!this.IsEmpty())
            {
                return this.cards[0];
            }

            return null;
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

        /// <summary>
        /// Get the position of a clicked card in this CardZone.
        /// </summary>
        /// <returns>An integer representing the position of the clicked card with respect to the
        /// end of the Zone.  Returns -1 if no valid card clicked.</returns>
        /// <param name="x">The x coordinate of the click.</param>
        /// <param name="y">The y coordinate of the click.</param>
        public virtual int GetClicked(int x, int y)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.cards[i].IsClicked(x, y, this.xSeparation, this.ySeparation))
                {
                    return this.Size() - i;
                }
            }

            // Return -1 on failure
            return -1;
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
        /// Checks to see if a coordinate is within a valid range of the Zone.  Useful for
        /// determining if a selection can be placed into the Zone.
        /// </summary>
        /// <returns><c>true</c>, if the coordinates are in range, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the mouse</param>
        /// <param name="y">The y coordinate of the mouse</param>
        public bool IsDroppedOn(int x, int y)
        {
            Debug.Assert(this is Tableau || this is Foundation);

            if (this.IsEmpty())
            {
                return this.x <= x
                       && x <= this.x + this.width
                       && this.y <= y
                       && y <= this.y + this.height;
            }
            else
            {
                return this.TopCard().X <= x
                   && x <= this.TopCard().X + this.TopCard().Width
                   && this.TopCard().Y <= y
                   && y <= this.TopCard().Y + this.TopCard().Height;
            }
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
        /// Move num cards from this Zone to the destination Zone dst
        /// </summary>
        /// <param name="num">The number of cards to be moved.</param>
        /// <param name="dst">The destination CardZone to put the cards.</param>
        public virtual void MoveCardsToZone(int num, CardZone dst)
        {
            Debug.Assert(num >= 0 && num <= this.Size());

            dst.AddCards(this.RemoveCards(num));
        }

        /// <summary>
        /// Realign num Card's positions in this Zone based on this Zone's (x,y)
        /// coordinate and x and y separation values.  Realignment is done with respect to the back
        /// of the Zone.
        /// <param name="num">The number of cards to realign</param>
        /// </summary>
        public virtual void RealignCards(int num)
        {
            if (this.Size() < num)
            {
                num = this.Size();
            }

            for (int i = 0; i < num; i++)
            {
                int location = this.Size() + i - num;
                this.cards[location].X = this.x + (i * this.xSeparation);
                this.cards[location].Y = this.y + (i * this.ySeparation);
            }
        }

        /// <summary>
        /// Remove some number of cards from this Zone.  Always removes from the back, that is, 
        /// from the last index backwards (no CardZone should be able to remove from the front).
        /// Updates the height and width of the Zone and unnests the top card of the remaining
        /// Zone (if applicable).  Does NOT update the locations of the cards -- cards should be 
        /// moving from one Zone to another, therefore the AddCards method is in charge of location.
        /// It is the job of the derived method to update card positions if it pulls from the 
        /// front of the Zone.
        /// </summary>
        /// <returns>Some List of cards</returns>
        /// <param name="num">The number of cards to be removed from the back of the Zone.</param>
        /// <param name="fromFront">If true, remove from the front of the zone, defaults to false.</param>
        public virtual List<Card> RemoveCards(int num, bool fromFront = false)
        {
            int location;
            if (fromFront)
            {
                location = 0;
            } 
            else
            {
                location = this.Size() - num;
            }

            List<Card> removed = this.cards.GetRange(location, num);
            this.cards.RemoveRange(location, num);
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

        /// <summary>
        /// Shorthand method for getting the last card in a Zone
        /// </summary>
        /// <returns>The card in the last position of the Zone</returns>
        public Card TopCard()
        {
            if (!this.IsEmpty())
            {
                return this.cards[this.Size() - 1];
            }


            return null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the 
        /// current <see cref="T:SolitaireGame.CardZone"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current 
        /// <see cref="T:SolitaireGame.CardZone"/>.</returns>
        public override string ToString()
        {
            return "[" + String.Join(", ", this.cards) + "]";
        }
    }
}
