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
        }

        public override List<Card> RemoveCards(int num, bool fromFront = false)
        {
            List<Card> baseRemoved = base.RemoveCards(num, fromFront);

            if (!this.IsEmpty())
            {
                this.RealignCards(GameProperties.DEAL_MODE);
            }
            
            return baseRemoved;
        }

    }
}
