// Selection.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Selection : CardZone
    {
        protected CardZone sourceZone;
        protected int relativeXOffset;
        protected int relativeYOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Selection"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="x">The x coordinate of this Zone.</param>
        /// <param name="y">The y coordinate of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Selection(BackendGame game, int x, int y, int xSep, int ySep) :
            base(game, x, y, xSep, ySep)
        {
        }

        /// <summary>
        /// Completes a move from this Selection to a target CardZone and handles any cleanup
        /// required after the move.
        /// </summary>
        /// <param name="dst">The target CardZone of the move.</param>
        public void CompleteMove(CardZone dst)
        {
            // Perform move as normal
            this.MoveCardsToZone(this.Size(), dst);

            // Take care of any cleanup after the move
            if (this.sourceZone is Tableau)
            {
                ((Tableau)this.sourceZone).Cleanup();
            }
            else if (this.sourceZone is Discard)
            {
                this.sourceZone.RealignCards(GameProperties.DEAL_MODE);
            }

            this.Reset();
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
                if (this.Size() == 1)
                {
                    if (dst.IsEmpty())
                    {
                        return this.TopCard().Val == 1;
                    }
                    else
                    {
                        return this.TopCard().IsSameSuit(dst.TopCard())
                           && this.TopCard().Val == dst.TopCard().Val + 1;
                    }  
                }

                return false;
            }
            else if (dst is Tableau)
            {
                if (dst.IsEmpty())
                {
                    return this.BottomCard().Val == 13;
                }
                else
                {
                    return this.BottomCard().IsOppositeSuit(dst.TopCard())
                           && this.BottomCard().Val == dst.TopCard().Val - 1;
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
            this.x = this.sourceZone.Cards[this.sourceZone.Size() - numToMove].X;
            this.y = this.sourceZone.Cards[this.sourceZone.Size() - numToMove].Y;
            this.relativeXOffset = this.x - clickX;
            this.relativeYOffset = this.y - clickY;
        }

        /// <summary>
        /// Properly sets this Selection's properties based on the CardZone is was selected from.
        /// </summary>
        /// <param name="source">The CardZone where this selection was taken from.</param>
        public void SetSourceZone(CardZone source)
        {
            this.sourceZone = source;
            this.xSeparation = source.XSeparation;
            this.ySeparation = source.YSeparation;
        }

        /// <summary>
        /// Helper function to reset this Selection object.
        /// </summary>
        private void Reset()
        {
            this.x = 0;
            this.y = 0;
            this.xSeparation = 0;
            this.ySeparation = 0;
            this.relativeXOffset = 0;
            this.relativeYOffset = 0;
            this.sourceZone = null;
        }

        /// <summary>
        /// Returns the Cards from this Selection to the source CardZone and clears this Selection.
        /// </summary>
        public void ReturnToSource()
        {
            if (!this.IsEmpty())
            {
                // TODO -- make positions more accurate so cards don't go to top of zone then scroll down

                //this.MoveCardsToZone(this.Size(), this.sourceZone);
                Tweener tween = new Tweener(this.game, this, this.sourceZone, this.Size());
                this.game.AnimationAdd(tween);
                this.Reset();
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
            this.x = mouseX + this.relativeXOffset;
            this.y = mouseY + this.relativeYOffset;

            for (int i = 0; i < this.Size(); i++)
            {
                this.cards[i].X = this.x + (this.xSeparation * i);
                this.cards[i].Y = this.y + (this.ySeparation * i);
            }
        }
    }
}