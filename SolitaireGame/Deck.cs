// Deck.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class Deck : CardZone
    {
        #region Constants
        public readonly static int X = 10;
        public readonly static int Y = 10;
        #endregion

        #region Members
        private bool m_isSelected;
        #endregion

        #region Properties
        public bool IsSelected
        {
            get => m_isSelected;
            set => m_isSelected = value;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Deck"/> class.
        /// </summary>
        /// <param name="game">The game this object belongs to.</param>
        /// <param name="location">The coordinates of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Vertical separation of cards in this Zone.</param>
        public Deck(BackendGame game, Point location, int xSep, int ySep) : 
            base(game, location, xSep, ySep)
        { 

            foreach (Card.Suits suit in Enum.GetValues(typeof(Card.Suits)))
            {
                List<Card> currentSuit = new List<Card>();
                for (int i = 1; i < 14; i++)
                {
                    Card c = new Card(i, suit);
                    c.LoadImage(game.Game, Properties.CardColor);
                    currentSuit.Add(c);
                }

                AddCards(currentSuit);
            }

            m_isSelected = false;
        }

        #region Methods
        /// <summary>
        /// Draws this Deck.</summary>
        /// <param name="s">A SpriteBatch object used for drawing in MonoGame.</param>
        public override void Draw(SpriteBatch s)
        {
            // If no cards, draw a black box
            if (IsEmpty())
            {
                int lineWidth = 2;
                
                DrawEmptyZone(m_game.Game.BlankBox, s, lineWidth, Color.Black);
                m_game.DrawLine(s, Location, Size.ToPoint(), lineWidth);
                m_game.DrawLine(s, new Point(Location.X, Location.Y + Size.ToPoint().Y), 
                    new Point(Location.X + Size.ToPoint().X, Location.Y), lineWidth);
            }
            else
            {
                foreach (Card card in m_cards)
                {
                    card.Draw(s, Color.White);
                }
            }
        }

        /// <summary>
        /// Deselect this instance.
        /// </summary>
        public void Deselect()
        {
            m_isSelected = false;
        }

        /// <summary>
        /// Checks if the Deck was clicked
        /// </summary>
        /// <returns><c>true</c>, if deck was clicked, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the click</param>
        /// <param name="y">The y coordinate of the click</param>
        public override bool IsClicked(int x, int y)
        {
            return ((Location.X <= x && x <= Location.X + Size.ToPoint().X) 
                   && (Location.Y <= y && y <= Location.Y + Size.ToPoint().Y))
                   || (!IsEmpty() && m_cards[Count() - 1].IsClicked(x, y, 0, 0));
        }

        /// <summary>
        /// Deals a certain number of cards from the Deck
        /// </summary>
        /// <param name="num">The number of Cards to deal</param>
        /// <param name="dst">The destination CardZone</param>
        public override void MoveCardsToZone(int num, CardZone dst)
        {
            Debug.Assert(dst is Discard || dst is Tableau);

            // Nothing to do if deck is empty
            if (IsEmpty())
            {
                return;
            }

            // If we have less cards than we are being asked for, give them all we have
            if (Count() < num)
            {
                num = Count();
            }

            List<Card> removed = RemoveCards(num);

            // Face cards the proper direction and nest all of them
            foreach (Card card in removed)
            {
                if (dst is Discard)
                {
                    card.MakeFaceUp();
                }
                else
                {
                    card.MakeFaceDown();
                }

                card.Nest();
            }

            // Unnest the last card
            removed[removed.Count - 1].UnNest();

            // Flip the last card of a tableau
            if (dst is Tableau)
            {
                removed[removed.Count - 1].Flip();
            }

            dst.AddCards(removed);
        }

        /// <summary>
        /// Remove some number of cards from the FRONT of this Deck (overrides default Zone
        /// behavior of removing from the back).
        /// </summary>
        /// <param name="num">The number of cards to be removed</param>
        /// <returns>A list of cards removed from the front</returns>
        public override List<Card> RemoveCards(int num, bool fromFront = true)
        {
            List<Card> baseRemoved = base.RemoveCards(num, fromFront);

            if (!IsEmpty())
            {
                RealignCards(Count());
            }

            return baseRemoved;
        }

        /// <summary>
        /// Select this instance.
        /// </summary>
        public void Select()
        {
            m_isSelected = true;
        }

        /// <summary>
        /// Shuffles the Deck 3 times using Fisher-Yates shuffle
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < 3; i++)
            {
                Random random = new Random();
                int n = Count() - 1;

                while (n > 1)
                {
                    int choice = random.Next(n);
                    Card temp = m_cards[choice];
                    m_cards[choice] = m_cards[n];
                    m_cards[n] = temp;
                    n--;
                }
            }

            RealignCards(Count());
        }

        #endregion
    }
}