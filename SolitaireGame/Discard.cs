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
        public Discard(int x, int y, int xSep, int ySep, GraphicsDevice g) :
            base(x, y, xSep, ySep, g)
        {
        }

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
