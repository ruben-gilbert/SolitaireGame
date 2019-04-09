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
                    if (this.cards[i].IsUp)
                    {
                        Console.WriteLine(cards[i] + " is clicked");
                        return this.Size() - i;
                    }
                    else
                    {
                        Console.WriteLine(cards[i] + " is clicked, but facedown");
                        return -1;
                    }
                    
                }
            }

            // Return -1 on failure
            return -1;
        }
    }
}
