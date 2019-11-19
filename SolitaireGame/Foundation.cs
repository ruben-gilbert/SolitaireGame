// Foundation.cs
// Author: Ruben Gilbert
// 2019

using Microsoft.Xna.Framework;

namespace SolitaireGame
{
    public class Foundation : CardZone
    {
        #region Constants
        public readonly static int Y = Deck.Y;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Foundation"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="location">The coordinates of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Foundation(BackendGame game, Point location, int xSep, int ySep) : 
            base(game, location, xSep, ySep)
        {
        }

        #region Methods
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
            if (TopCard().IsClicked(x, y, 0, 0))
            {
                return 1;
            }

            // Return -1 on failure (shouldn't actually happen since this only gets called if 
            // the foundation was clicked in the first place, which would be the top card).
            return -1;
        }
        #endregion

    }
}
