// Deck.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Deck : CardZone
    {

        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Deck"/> class.
        /// </summary>
        public Deck(BackendGame game, int x, int y, int xSep, int ySep) : 
            base(game, x, y, xSep, ySep)
        { 

            foreach (string suit in GameProperties.VALID_SUITS_ARRAY)
            {
                List<Card> currentSuit = new List<Card>();
                for (int i = 1; i < 14; i++)
                {
                    Card c = new Card(i, suit);
                    c.LoadImage(game.Game, GameProperties.CARD_COLOR);
                    currentSuit.Add(c);
                }

                this.AddCards(currentSuit);
            }

            this.isSelected = false;
        }

        /// <summary>
        /// Deselect this instance.
        /// </summary>
        public void Deselect()
        {
            this.isSelected = false;
        }

        /// <summary>
        /// Checks if the Deck was clicked
        /// </summary>
        /// <returns><c>true</c>, if deck was clicked, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the click</param>
        /// <param name="y">The y coordinate of the click</param>
        public override bool IsClicked(int x, int y)
        {
            return ((this.x <= x && x <= this.x + this.width) 
                   && (this.y <= y && y <= this.y + this.height))
                   || 
                   (!this.IsEmpty() && this.cards[this.Size() - 1].IsClicked(x, y, 0, 0));
        }

        /// <summary>
        /// Checks if this Deck is selected.
        /// </summary>
        /// <returns><c>true</c>, if selected, <c>false</c> otherwise.</returns>
        public bool IsSelected()
        {
            return this.isSelected;
        }

        /// <summary>
        /// Deals a certain number of cards from the Deck
        /// </summary>
        /// <param name="num">The number of Cards to deal</param>
        /// <param name="dst">The destination CardZone</param>
        public override void MoveCardsToZone(int num, CardZone dst)
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
        /// Remove some number of cards from the FRONT of this Deck (overrides default Zone
        /// behavior of removing from the back).
        /// </summary>
        /// <param name="num">The number of cards to be removed</param>
        /// <returns>A list of cards removed from the front</returns>
        public override List<Card> RemoveCards(int num, bool fromFront = true)
        {
            List<Card> baseRemoved = base.RemoveCards(num, fromFront);

            if (!this.IsEmpty())
            {
                this.RealignCards(this.Size());
            }

            return baseRemoved;
        }

        /// <summary>
        /// Select this instance.
        /// </summary>
        public void Select()
        {
            this.isSelected = true;
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

            this.RealignCards(this.Size());
        }
    }
}