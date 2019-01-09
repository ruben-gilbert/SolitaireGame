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

        public Selection()
        {
            this.cards = new List<Card>();
            this.valid = false;
            this.x = 0;
            this.y = 0;
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

        public bool IsValid()
        {
            return this.valid;
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
