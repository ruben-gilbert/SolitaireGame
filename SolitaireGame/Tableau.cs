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

        public override List<Card> RemoveCards(int num)
        {
            List<Card> baseRemoved = base.RemoveCards(num);

            if (!this.TopCard().IsUp)
            {
                this.TopCard().Flip();
            }

            return baseRemoved;
        }
    }
}
