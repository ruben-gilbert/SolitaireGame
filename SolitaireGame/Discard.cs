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

        public new void Draw(SpriteBatch s)
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

        // TODO -- Discard should add cards to front instead of back so
        // deck remains in correct order every time??? Maybe not?
    }
}
