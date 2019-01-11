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

        public Selection()
        {
            this.cards = new List<Card>();
            this.valid = false;
            this.x = 0;
            this.y = 0;
            this.w = Constants.CARD_WIDTH;
            this.h = 0;
        }

        public int X
        {
            set { this.x = value; }
            get { return this.x; }
        }

        public int Y
        {
            set { this.y = value; }
            get { return this.y; }
        }

        public int W
        { 
            get { return this.w; }
        }

        public int H
        {
            set { this.h = value; }
            get { return this.h; }
        }

        public bool IsValid()
        {
            return this.valid;
        }

        public void Change(List<Card> c)
        {
            this.cards = c;
            this.valid = true;
        }

        public int Size()
        {
            return this.cards.Count;
        }

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

        public void Clear()
        {
            this.cards.Clear();
            this.valid = false;
            this.x = 0;
            this.y = 0;
        }
    }
}
