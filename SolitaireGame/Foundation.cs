// Foundation.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Foundation : CardZone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Foundation"/> class.
        /// </summary>
        /// <param name="x">The x coordinate of this Zone.</param>
        /// <param name="y">The y coordinate of this Zone.</param>
        /// <param name="xSep">X sep of cards in this Zone.</param>
        /// <param name="ySep">Y sep of cards in this Zone.</param>
        /// <param name="g">A GraphicsDevice that can produce Texture2D's.</param>
        public Foundation(int x, int y, int xSep, int ySep, GraphicsDevice g) : 
            base(x, y, xSep, ySep, g)
        {

        }

        public new List<Card> GetClicked(int x, int y)
        {
            if (this.TopCard().IsClicked(x, y, 0, 0))
            {
                return this.RemoveCards(1);
            }

            return null;
        }

    }
}
