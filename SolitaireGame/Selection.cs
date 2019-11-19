// Selection.cs
// Author: Ruben Gilbert
// 2019

using Microsoft.Xna.Framework;

namespace SolitaireGame
{
    public class Selection : CardZone
    {
        #region Members
        protected CardZone m_sourceZone;
        protected int m_relativeXOffset;
        protected int m_relativeYOffset;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Selection"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="location">The coordinates of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Selection(BackendGame game, Point location, int xSep, int ySep) :
            base(game, location, xSep, ySep)
        {
        }

        #region Methods
        /// <summary>
        /// Completes a move from this Selection to a target CardZone and handles any cleanup
        /// required after the move.
        /// </summary>
        /// <param name="dst">The target CardZone of the move.</param>
        public void CompleteMove(CardZone dst)
        {
            // Perform move as normal
            MoveCardsToZone(Count(), dst);

            // Take care of any cleanup after the move
            if (m_sourceZone is Tableau)
            {
                ((Tableau)m_sourceZone).Cleanup();
            }
            else if (m_sourceZone is Discard)
            {
                m_sourceZone.RealignCards(Properties.DealMode);
            }

            Reset();
        }

        /// <summary>
        /// Checks if moving this selection to a target CardZone is valid.
        /// </summary>
        /// <param name="dst">The target CardZone.</param>
        /// <returns>True if the move is valid, false otherwise.</returns>
        public bool IsValidMove(CardZone dst)
        {
            if (dst is Foundation)
            {
                if (Count() == 1)
                {
                    if (dst.IsEmpty())
                    {
                        return TopCard().Value == 1;
                    }
                    else
                    {
                        return TopCard().IsSameSuit(dst.TopCard())
                           && TopCard().Value == dst.TopCard().Value + 1;
                    }  
                }

                return false;
            }
            else if (dst is Tableau)
            {
                if (dst.IsEmpty())
                {
                    return BottomCard().Value == 13;
                }
                else
                {
                    return BottomCard().IsOppositeSuit(dst.TopCard())
                           && BottomCard().Value == dst.TopCard().Value - 1;
                }
            }

            return false;
        }

        /// <summary>
        /// Set the relative offset position for the Cards in this Selection.  When
        /// a selection is made, we want the Cards in the Selection to move with respect
        /// to where the mouse was placed on them when the user clicked.
        /// </summary>
        /// <param name="numToMove">The number of cards being moved from the source</param>
        /// <param name="clickX">Click x.</param>
        /// <param name="clickY">Click y.</param>
        public void SetRelativeOffsets(int numToMove, int clickX, int clickY)
        {
            m_location = new Point(m_sourceZone.Cards[m_sourceZone.Count() - numToMove].Location.X,
                m_sourceZone.Cards[m_sourceZone.Count() - numToMove].Location.Y);
            m_relativeXOffset = m_location.X - clickX;
            m_relativeYOffset = m_location.Y - clickY;
        }

        /// <summary>
        /// Properly sets this Selection's properties based on the CardZone is was selected from.
        /// </summary>
        /// <param name="source">The CardZone where this selection was taken from.</param>
        public void SetSourceZone(CardZone source)
        {
            m_sourceZone = source;
            m_xSeparation = source.XSeparation;
            m_ySeparation = source.YSeparation;
        }

        /// <summary>
        /// Helper function to reset this Selection object.
        /// </summary>
        private void Reset()
        {
            m_location = new Point(0, 0);
            m_xSeparation = 0;
            m_ySeparation = 0;
            m_relativeXOffset = 0;
            m_relativeYOffset = 0;
            m_sourceZone = null;
        }

        /// <summary>
        /// Returns the Cards from this Selection to the source CardZone and clears this Selection.
        /// </summary>
        public void ReturnToSource()
        {
            if (!IsEmpty())
            {
                //MoveCardsToZone(Size(), sourceZone);
                Tweener tween = new Tweener(m_game, this, m_sourceZone, Count());
                m_game.AnimationAdd(tween);
                Reset();
            }
        }

        /// <summary>
        /// Updates the position of this Selection based on the current (x, y) coordinates
        /// of the mouse.
        /// </summary>
        /// <param name="mouseX">Click x.</param>
        /// <param name="mouseY">Click y.</param>
        public void UpdatePosition(int mouseX, int mouseY)
        {
            m_location = new Point(mouseX + m_relativeXOffset, mouseY + m_relativeYOffset);

            for (int i = 0; i < Count(); i++)
            {
                m_cards[i].Location = new Point(m_location.X + (m_xSeparation * i), m_location.Y + (m_ySeparation * i));
            }
        }
        #endregion
    }
}