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
        // Instance Variables
        private string suit;
        private int value;
        private bool up;
        private Texture2D front;
        private Texture2D back;

        // Constructor
        public Card(int value, string suit)
        {

            up = false;

            if (!Constants.VALID_SUITS.Contains(suit))
            {
                throw new ArgumentException(String.Format("{0} is not a valid suit", "suit"));
            }
            else
            {
                this.suit = suit;
            }

            if (value < 1 || value > 13)
            {
                throw new ArgumentException(String.Format("{0} is not a valid card value", "value"));
            }
            else
            {
                this.value = value;
            }
        }

        // *******************************
        // Methods
        // *******************************

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

        public void LoadImages(MainGame g, string back_color)
        {
            this.front = g.Content.Load<Texture2D>("images/" + this.ToString());
            this.back = g.Content.Load<Texture2D>("images/back_" + back_color);
        }

        public override string ToString()
        {
            if (this.value == 1)
            {
                return "A" + this.suit;
            }
            else if (this.value == 11)
            {
                return "J" + this.suit;
            }
            else if (this.value == 12)
            {
                return "Q" + this.suit;
            }
            else if (this.value == 13)
            {
                return "K" + this.suit;
            }
            else
            {
                return this.value.ToString() + this.suit;
            }
        }
    }
}
