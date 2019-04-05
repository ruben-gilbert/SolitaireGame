// Deck.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Deck : CardZone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Deck"/> class.
        /// </summary>
        public Deck(int x, int y, int xSep, int ySep, GraphicsDevice g, MainGame game) : base(x, y, xSep, ySep, g)
        { 

            foreach (string suit in GameProperties.VALID_SUITS_ARRAY)
            {
                List<Card> currentSuit = new List<Card>();
                for (int i = 1; i < 14; i++)
                {
                    Card c = new Card(i, suit);
                    c.LoadImage(game, GameProperties.CARD_COLOR);
                    currentSuit.Add(c);
                }

                this.AddCards(currentSuit);
            }
        }

        /// <summary>
        /// Deals a certain number of cards from the Deck
        /// </summary>
        /// <param name="num">The number of Cards to deal</param>
        /// <returns>A List of Cards of size num</returns>
        public void Deal(int num, CardZone dst)
        {
            Debug.Assert(dst is Discard || dst is Tableau);

            // Nothing to do if deck is empty
            if (this.IsEmpty())
            {
                return;
            }

            // If we have less cards than we are being asked for, give them all we have
            if (this.Size() < num)
            {
                num = this.Size();
            }

            List<Card> c = this.RemoveCards(num);

            // Face cards the proper direction and nest all of them
            foreach (Card card in c)
            {
                if (dst is Discard)
                {
                    card.MakeFaceUp();
                }
                else
                {
                    card.MakeFaceDown();
                }

                card.Nest();
            }

            // Unnest the last card
            c[c.Count - 1].UnNest();

            // Flip the last card of a tableau
            if (dst is Tableau)
            {
                c[c.Count - 1].Flip();
            }

            dst.AddCards(c);
        }

        /// <summary>
        /// Checks if the Deck was clicked
        /// </summary>
        /// <returns><c>true</c>, if deck was clicked, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the click</param>
        /// <param name="y">The y coordinate of the click</param>
        public new bool IsClicked(int x, int y)
        {
            return ((this.x <= x && x <= this.x + this.width) 
                   && (this.y <= y && y <= this.y + this.height))
                   || 
                   this.cards[this.Size() - 1].IsClicked(x, y, 0, 0);
        }

        /// <summary>
        /// Shuffles the Deck 3 times using Fisher-Yates shuffle
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < 3; i++)
            {
                Random random = new Random();
                int n = this.Size() - 1;

                while (n > 1)
                {
                    int choice = random.Next(n);
                    Card temp = this.cards[choice];
                    this.cards[choice] = this.cards[n];
                    this.cards[n] = temp;
                    n--;
                }
            }
        }

        /// <summary>
        /// String representation of this Deck
        /// </summary>
        /// <returns>A string with comma separated Card representations</returns>
        public override string ToString()
        {
            return "[" + String.Join(", ", this.cards) + "]";
        }

    }
}