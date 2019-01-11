// Selection.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections;
using System.Collections.Generic;

namespace SolitaireGame
{
    public class Selection
    {
        private List<Card> cards;
        private bool valid;
        private int x;
        private int y;
        private int w;
        private int h;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Selection"/> class.
        /// </summary>
        public Selection()
        {
            this.cards = new List<Card>();
            this.valid = false;
            this.x = 0;
            this.y = 0;
            this.w = Constants.CARD_WIDTH;
            this.h = 0;
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
        public void Change(List<Card> c)
        {
            this.cards = c;
            this.valid = true;
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
            this.x = 0;
            this.y = 0;
        }
    }
}
