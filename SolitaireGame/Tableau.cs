using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Tableau : CardZone
    {
        public Tableau(int x, int y, int xSep, int ySep, GraphicsDevice g) :
            base(x, y, xSep, ySep, g)
        {
        }

        public override void AddCards(List<Card> c)
        {
            base.AddCards(c);
            // TODO update the y-separation of this Tableau if it exceeds bounds of the window?
        }

        public override int GetClicked(int x, int y)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.cards[i].IsClicked(x, y, this.width, this.ySeparation))
                {
                    return this.Size() - i;
                }
            }

            // Return -1 on failure (shouldn't actually happen)
            return -1;
        }

        public override List<Card> RemoveCards(int num, bool fromFront = false)
        {
            List<Card> baseRemoved = base.RemoveCards(num, fromFront);

            // TODO -- remove this, only flip top card if selection actually goes thorugh?
            if (!this.IsEmpty() && !this.TopCard().IsUp)
            {
                this.TopCard().Flip();
            }

            return baseRemoved;
        }
    }
}
