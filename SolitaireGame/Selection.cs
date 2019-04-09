// Selection.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Selection : CardZone
    {
        protected CardZone sourceZone;
        protected int relativeXOffset;
        protected int relativeYOffset;

        public Selection(int x, int y, int xSep, int ySep, GraphicsDevice g) :
            base(x, y, xSep, ySep, g)
        {
            //this.relativeXOffset = 0;
            //this.relativeYOffset = 0;
        }

        // TODO -- any more to this method?
        public void CompleteMove(CardZone dst)
        {
            // Perform move as normal
            this.MoveCardsToZone(this.Size(), dst);

            // Take care of any cleanup after the move
            if (this.sourceZone is Tableau && !this.sourceZone.IsEmpty() 
                && this.sourceZone.TopCard().IsUp)
            {
                this.sourceZone.TopCard().Flip();
            }
            else if (this.sourceZone is Discard && !this.sourceZone.IsEmpty())
            {
                this.sourceZone.RealignCards(this.sourceZone.Size());
            }

            this.Reset();
        }

        /// <summary>
        /// Set the relative offset position for the Cards in this Selection.  When
        /// a selection is made, we want the Cards in the Selection to move with respect
        /// to where the mouse was placed on them when the user clicked.
        /// </summary>
        /// <param name="numToMove">The number of cards being moved from the source</param>
        /// <param name="clickX">Click x.</param>
        /// <param name="clickY">Click y.</param>
        public void SetRelativeOffsets(int numToMove, int clickX, int clickY)
        {
            this.x = this.sourceZone.Cards[this.sourceZone.Size() - numToMove].X;
            this.y = this.sourceZone.Cards[this.sourceZone.Size() - numToMove].Y;
            this.relativeXOffset = this.x - clickX;
            this.relativeYOffset = this.y - clickY;
        }

        public void SetSourceZone(CardZone source)
        {
            this.sourceZone = source;
            this.xSeparation = source.XSeparation;
            this.ySeparation = source.YSeparation;
        }

        /// <summary>
        /// Helper function to reset this Selection object.
        /// </summary>
        private void Reset()
        {
            this.x = 0;
            this.y = 0;
            this.xSeparation = 0;
            this.ySeparation = 0;
            this.relativeXOffset = 0;
            this.relativeYOffset = 0;
            this.sourceZone = null;
        }

        // TODO return to source method?
        public void ReturnToSource()
        {
            this.MoveCardsToZone(this.Size(), this.sourceZone);
            this.Reset();
        }

        /// <summary>
        /// Updates the position of this Selection based on the current (x, y) coordinates
        /// of the mouse.
        /// </summary>
        /// <param name="mouseX">Click x.</param>
        /// <param name="mouseY">Click y.</param>
        public void UpdatePosition(int mouseX, int mouseY)
        {
            this.x = mouseX + this.relativeXOffset;
            this.y = mouseY + this.relativeYOffset;

            for (int i = 0; i < this.Size(); i++)
            {
                this.cards[i].X = this.x + (this.xSeparation * i);
                this.cards[i].Y = this.y + (this.ySeparation * i);
            }
        }
    }
}

    /*
        private List<Card> cards;
        private bool valid;
        private int source;
        private int x;
        private int y;
        private readonly int w;
        private int h;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Selection"/> class.
        /// </summary>
        public Selection()
        {
            this.cards = new List<Card>();
            this.valid = false;
            this.source = -1;
            this.x = 0;
            this.y = 0;
            this.w = GameProperties.CARD_WIDTH;
            this.h = 0;
        }

        /// <summary>
        /// Gets the List of Card objects this Selection holds.
        /// </summary>
        public List<Card> Cards
        {
            get { return this.cards; }
        }

        /// <summary>
        /// Returns the source of this Selection object.
        /// -1 is invalid
        /// 0-6 is column number
        /// 7 is discard pile
        /// </summary>
        /// <value>The new source of this Selection</value>
        public int Source
        {
            get { return this.source; }
        }

        /// <summary>
        /// Gets or sets the x value of this Selection.
        /// </summary>
        /// <value>The new x value.</value>
        public int X
        {
            set { this.x = value; }
            get { return this.x; }
        }

        /// <summary>
        /// Gets or sets the y value of this Selection.
        /// </summary>
        /// <value>The new y value.</value>
        public int Y
        {
            set { this.y = value; }
            get { return this.y; }
        }

        /// <summary>
        /// Gets the width value of this Selection.
        /// </summary>
        /// <value>NULL</value>
        public int W
        { 
            get { return this.w; }
        }

        /// <summary>
        /// Gets or sets the height of this Selection.
        /// </summary>
        /// <value>NULL</value>
        public int H
        {
            set { this.h = value; }
            get { return this.h; }
        }

        /// <summary>
        /// Checks if this Selection is valid.
        /// </summary>
        /// <returns><c>true</c>, if the Selection is valid, <c>false</c> otherwise.</returns>
        public bool IsValid()
        {
            return this.valid;
        }

        /// <summary>
        /// Change what Cards this Selection refers to.
        /// </summary>
        /// <param name="c">A List of Cards</param>
        public void Change(List<Card> c, int src, int x, int y, int h)
        {
            this.cards = c;
            this.valid = true;
            this.source = src;
            this.x = x;
            this.y = y;
            this.h = h;
        }

        /// <summary>
        /// Gets the size of this Selection (i.e. the number of Cards it refers to).
        /// </summary>
        /// <returns>The number of Cards the Selection holds</returns>
        public int Size()
        {
            return this.cards.Count;
        }

        /// <summary>
        /// Compares the Cards in this Selection to some other List of Cards.
        /// </summary>
        /// <returns><c>true</c>, if the two Lists are the same, <c>false</c> otherwise.</returns>
        /// <param name="other">Some other List of Cards</param>
        public bool CompareCards(List<Card> other)
        {
            if (this.cards.Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < other.Count; i++)
            {
                if (!other[i].Equals(this.cards[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears this Selection by clearing the List of Cards, setting valid to False,
        /// and resetting x and y.
        /// </summary>
        public void Clear()
        {
            this.cards.Clear();
            this.valid = false;
            this.source = -1;
            this.x = 0;
            this.y = 0;
        }
    }
}
*/
