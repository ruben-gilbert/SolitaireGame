// Foundation.cs
// Author: Ruben Gilbert
// 2019

using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Foundation : CardZone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Foundation"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="x">The x coordinate of this Zone.</param>
        /// <param name="y">The y coordinate of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Foundation(BackendGame game, int x, int y, int xSep, int ySep) : 
            base(game, x, y, xSep, ySep)
        {

        }

        /// <summary>
        /// Get the position of a clicked card in this Foundation.  Since only
        /// the top card in a foundation is showing, only need to check the one card for
        /// a click.
        /// </summary>
        /// <returns>1 if the top card was clicked, -1 otherwise.</returns>
        /// <param name="x">The x coordinate of the click.</param>
        /// <param name="y">The y coordinate of the click.</param>
        public override int GetClicked(int x, int y)
        {
            if (this.TopCard().IsClicked(x, y, 0, 0))
            {
                return 1;
            }

            // Return -1 on failure (shouldn't actually happen since this only gets called if 
            // the foundation was clicked in the first place, which would be the top card).
            return -1;
        }

    }
}
