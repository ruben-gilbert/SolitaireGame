// Discard.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame

{
    public class Discard : CardZone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Discard"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="x">The x coordinate of this Zone.</param>
        /// <param name="y">The y coordinate of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Discard(BackendGame game, int x, int y, int xSep, int ySep) :
            base(game, x, y, xSep, ySep)
        {
        }

        /// <summary>
        /// Adds a list of Card objects to this Discard zone.
        /// </summary>
        /// <param name="c">Some List of Cards</param>
        public override void AddCards(List<Card> c)
        {
            
            if (this.Size() > GameProperties.DEAL_MODE)
            {
                for (int i = this.Size() - 1; i <= this.Size() - GameProperties.DEAL_MODE; i--)
                {
                    this.cards[i].X = this.x;
                    this.cards[i].Y = this.y;
                }
            }
            
            base.AddCards(c);
            this.RealignCards(GameProperties.DEAL_MODE);
        }

        /// <summary>
        /// Draws this Discard object (overrides the CardZone Draw method)
        /// </summary>
        /// <param name="s">The SpriteBatch object used for drawing.</param>
        public override void Draw(SpriteBatch s)
        {
            if (this.IsEmpty())
            {
                this.DrawEmptyZone(this.blankBox, s, 2, Color.Black);
            }
            else if (this.Size() < GameProperties.DEAL_MODE)
            {
                foreach (Card c in this.cards)
                {
                    c.Draw(s, Color.White);
                }
            }
            else
            {
                for (int i = this.Size() - GameProperties.DEAL_MODE; i < this.Size(); i++)
                {
                    this.cards[i].Draw(s, Color.White);
                }
            }
        }

        /// <summary>
        /// Moves num number of cards to CardZone dst.  Relies on the base CardZone method, but
        /// also handles moving cards back to the Deck and realigning the cards that should be
        /// shown after removing them from the discard pile.
        /// </summary>
        /// <param name="num">The number of cards to be moved.</param>
        /// <param name="dst">The destination CardZone object.</param>
        public override void MoveCardsToZone(int num, CardZone dst)
        {
            // If the destination is the deck, we need to make sure all cards are face down
            if (dst is Deck)
            {
                foreach (Card card in this.cards)
                {
                    card.MakeFaceDown();
                }
            }

            base.MoveCardsToZone(num, dst);

            // Cleanup
            if (dst is Selection)
            {
                this.RealignCards(GameProperties.DEAL_MODE - 1);
            }
        }

        /// <summary>
        /// Realign num number of cards at the back of the Discard pile.  Since we should only show
        /// a specific number of cards, useful for making sure cards draw correctly.  Relies on the
        /// base method to realign the remaining cards that are not the top few that should show.
        /// </summary>
        /// <param name="num">The number of cards that need realignment.</param>
        public override void RealignCards(int num)
        {
            for (int i = 0; i < this.Size() - num; i++)
            {
                this.cards[i].X = this.x;
                this.cards[i].Y = this.y;
            }

            base.RealignCards(num);
        }
    }
}
