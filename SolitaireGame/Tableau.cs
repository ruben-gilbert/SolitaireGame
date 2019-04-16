using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Tableau : CardZone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Tableau"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="x">The x coordinate of this Zone.</param>
        /// <param name="y">The y coordinate of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Tableau(BackendGame game, int x, int y, int xSep, int ySep) :
            base(game, x, y, xSep, ySep)
        {
        }

        /// <summary>
        /// Adds cards to this Zone by using the base class method.  Also adjusts the
        /// height/y-separation of cards if the stack gets too large for the window.
        /// </summary>
        /// <param name="c">C.</param>
        public override void AddCards(List<Card> c)
        {
            base.AddCards(c);
            // TODO update the y-separation of this Tableau if it exceeds bounds of the window?
        }

        /// <summary>
        /// Simple cleanup function that ensures the top card of a Tableau is face-up.
        /// </summary>
        public void Cleanup()
        {
            if (!this.IsEmpty() && !this.TopCard().IsUp)
            {
                this.TopCard().Flip();
            }
        }

        /// <summary>
        /// Returns the position of a clicked card in this Tableau.
        /// </summary>
        /// <returns>The index of the clicked Card in this Tableau, -1 on failure.</returns>
        /// <param name="x">The x coordinate of the click.</param>
        /// <param name="y">The y coordinate of the click.</param>
        public override int GetClicked(int x, int y)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.cards[i].IsClicked(x, y, this.width, this.ySeparation))
                {
                    if (this.cards[i].IsUp)
                    {
                        return this.Size() - i;
                    }
                    else
                    {
                        return -1;
                    }
                    
                }
            }

            // Return -1 on failure
            return -1;
        }
    }
}
