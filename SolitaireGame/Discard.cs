// Discard.cs
// Author: Ruben Gilbert
// 2019

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame

{
    public class Discard : CardZone
    {
        #region Constants
        public readonly static int X = Deck.X + Card.Width + 30;
        public readonly static int Y = Deck.Y;
        public readonly static int Separation = 20;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Discard"/> class.
        /// </summary>
        /// <param name="game">The BackendGame this Zone belongs to.</param>
        /// <param name="location">The coordinates of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Horizontal separation of cards in this Zone.</param>
        public Discard(BackendGame game, Point location, int xSep, int ySep) :
            base(game, location, xSep, ySep)
        {
        }

        #region Methods
        /// <summary>
        /// Adds a list of Card objects to this Discard zone.
        /// </summary>
        /// <param name="cards">Some List of Cards</param>
        public override void AddCards(List<Card> cards)
        { 
            if (Count() > Properties.DealMode)
            {
                for (int i = Count() - 1; i <= Count() - Properties.DealMode; i--)
                {
                    m_cards[i].Location = Location;
                }
            }
            
            base.AddCards(cards);
            RealignCards(Properties.DealMode);
        }

        /// <summary>
        /// Draws this Discard object (overrides the CardZone Draw method)
        /// </summary>
        /// <param name="s">The SpriteBatch object used for drawing.</param>
        public override void Draw(SpriteBatch s)
        {
            if (IsEmpty())
            {
                DrawEmptyZone(m_game.Game.BlankBox, s, 2, Color.Black);
            }
            else if (Count() < Properties.DealMode)
            {
                foreach (Card c in m_cards)
                {
                    c.Draw(s, Color.White);
                }
            }
            else
            {
                for (int i = Count() - Properties.DealMode; i < Count(); i++)
                {
                    m_cards[i].Draw(s, Color.White);
                }
            }
        }

        /// <summary>
        /// Moves num number of cards to CardZone dst.  Relies on the base CardZone method, but
        /// also handles moving cards back to the Deck and realigning the cards that should be
        /// shown after removing them from the discard pile.
        /// </summary>
        /// <param name="num">The number of cards to be moved.</param>
        /// <param name="dst">The destination CardZone object.</param>
        public override void MoveCardsToZone(int num, CardZone dst)
        {
            // If the destination is the deck, we need to make sure all cards are face down
            if (dst is Deck)
            {
                foreach (Card card in m_cards)
                {
                    card.MakeFaceDown();
                }
            }

            base.MoveCardsToZone(num, dst);

            // Cleanup
            if (dst is Selection)
            {
                RealignCards(Properties.DealMode - 1);
            }
        }

        /// <summary>
        /// Realign num number of cards at the back of the Discard pile.  Since we should only show
        /// a specific number of cards, useful for making sure cards draw correctly.  Relies on the
        /// base method to realign the remaining cards that are not the top few that should show.
        /// </summary>
        /// <param name="num">The number of cards that need realignment.</param>
        public override void RealignCards(int num)
        {
            for (int i = 0; i < Count() - num; i++)
            {
                m_cards[i].Location = Location;
            }

            base.RealignCards(num);
        }
        #endregion
    }
}
