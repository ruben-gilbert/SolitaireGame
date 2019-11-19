// CardZone.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    public class CardZone
    {
        #region Members
        protected BackendGame m_game;

        protected List<Card> m_cards;

        protected Point m_location;

        protected int m_xSeparation;
        protected int m_ySeparation;

        protected Vector2 m_size;
        #endregion

        #region Properties
        public List<Card> Cards
        {
            get { return m_cards; }
        }

        public Point Location
        {
            get => m_location;

        }

        public Vector2 Size
        {
            get => m_size;
            set => m_size = value;
        }

        public int XSeparation
        {
            get => m_xSeparation;
        }

        public int YSeparation
        {
            get => m_ySeparation;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.CardZone"/> class.
        /// </summary>
        /// <param name="game">The game this object belongs to.</param>
        /// <param name="location">The coordinates of this Zone.</param>
        /// <param name="xSep">Horizontal separation of cards in this Zone.</param>
        /// <param name="ySep">Vertical separation of cards in this Zone.</param>
        protected CardZone(BackendGame game, Point location, int xSep, int ySep)
        {
            m_cards = new List<Card>();
            m_location = location;
            m_size = new Vector2(Card.Width, Card.Height);
            m_xSeparation = xSep;
            m_ySeparation = ySep;
            m_game = game;
        }

        #region Methods
        /// <summary>
        /// Add cards to this Zone.  Ensures that inner-cards become nested and update their
        /// x, y locations.  Also updates the width/height of this Zone.
        /// </summary>
        /// <param name="cards">A List of one or more cards to be added to this Zone.</param>
        public virtual void AddCards(List<Card> cards)
        {

            // Make sure to nest the top card
            if (!IsEmpty())
            {
                m_cards[Count() - 1].Nest();
            }

            foreach (Card card in cards)
            {
                // Update the card's position
                card.Location = new Point(Location.X + (Count() * m_xSeparation), Location.Y + (Count() * m_ySeparation));
                //card.X = m_x + (Size() * xSeparation);
                //card.Y = m_y + (Size() * ySeparation);

                // Update it's source
                card.Source = this;

                // Only update the width/height of the Zone when we start nesting cards
                if (Count() > 0)
                {
                    m_size.X += m_xSeparation;
                    m_size.Y += m_ySeparation;
                }

                m_cards.Add(card);
            }
        }

        /// <summary>
        /// Shorthand method for getting the card at position 0.
        /// </summary>
        /// <returns>The Card object at position 0.</returns>
        public Card BottomCard()
        {
            if (!IsEmpty())
            {
                return m_cards[0];
            }

            return null;
        }

        /// <summary>
        /// Clears this Zone.
        /// </summary>
        public void Clear()
        {
            m_cards.Clear();
        }

        /// <summary>
        /// Gets the number of cards in this CardZone
        /// </summary>
        /// <returns>The number of cards in this CardZone.</returns>
        public int Count()
        {
            return m_cards.Count;
        }

        /// <summary>
        /// Draws this Zone.  Either a blank box if it is empty, or have each card draw itself.
        /// </summary>
        /// <param name="s">A SpriteBatch object used for drawing in MonoGame.</param>
        public virtual void Draw(SpriteBatch s)
        {
            // If no cards, draw a black box
            if (IsEmpty())
            {
                DrawEmptyZone(m_game.Game.BlankBox, s, 2, Color.Black);
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
        /// Helper method to draw a black box for an empty Zone.
        /// </summary>
        /// <param name="tex">The texture for the inside of the empty box (typically blank)</param>
        /// <param name="s">A SpriteBatch object used for drawing in MonoGame</param>
        /// <param name="thickness">How thick the border of the box should be</param>
        /// <param name="c">The color of the border of the box</param>
        protected void DrawEmptyZone(Texture2D tex, SpriteBatch s, int thickness, Color c)
        {
            s.Draw(tex, new Rectangle(Location.X, Location.Y, thickness, Size.ToPoint().Y), c);
            s.Draw(tex, new Rectangle(Location.X + Size.ToPoint().X, Location.Y, thickness, Size.ToPoint().Y), c);
            s.Draw(tex, new Rectangle(Location.X, Location.Y, Size.ToPoint().X, thickness), c);
            s.Draw(tex, new Rectangle(Location.X, Location.Y + Size.ToPoint().Y, Size.ToPoint().X, thickness), c);
        }

        /// <summary>
        /// Get the position of a clicked card in this CardZone.
        /// </summary>
        /// <returns>An integer representing the position of the clicked card with respect to the
        /// end of the Zone.  Returns -1 if no valid card clicked.</returns>
        /// <param name="x">The x coordinate of the click.</param>
        /// <param name="y">The y coordinate of the click.</param>
        public virtual int GetClicked(int x, int y)
        {
            for (int i = 0; i < Count(); i++)
            {
                if (m_cards[i].IsClicked(x, y, m_xSeparation, m_ySeparation))
                {
                    return Count() - i;
                }
            }

            // Return -1 on failure
            return -1;
        }

        /// <summary>
        /// Checks to see if this CardZone was clicked
        /// </summary>
        /// <returns><c>true</c>, if this Zone was clicked, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the click</param>
        /// <param name="y">The y coordinate of the click</param>
        public virtual bool IsClicked(int x, int y)
        {
            return Location.X <= x && x <= Location.X + Size.ToPoint().X
                && Location.Y <= y && y <= Location.Y + Size.ToPoint().Y;
        }

        /// <summary>
        /// Checks to see if a coordinate is within a valid range of the Zone.  Useful for
        /// determining if a selection can be placed into the Zone.
        /// </summary>
        /// <returns><c>true</c>, if the coordinates are in range, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate of the mouse</param>
        /// <param name="y">The y coordinate of the mouse</param>
        public bool IsDroppedOn(int x, int y)
        {
            Debug.Assert(this is Tableau || this is Foundation);

            if (IsEmpty())
            {
                return Location.X <= x && x <= Location.X + Size.ToPoint().X
                       && y <= Location.Y && y <= Location.Y + Size.ToPoint().Y;
            }
            else
            {
                return TopCard().Location.X <= x && x <= TopCard().Location.X + TopCard().Size.ToPoint().X
                   && TopCard().Location.Y <= y && y <= TopCard().Location.Y + TopCard().Size.ToPoint().Y;
            }
        }

        /// <summary>
        /// Checks if this Zone is empty.
        /// </summary>
        /// <returns><c>true</c>, if the Zone is empty, <c>false</c> otherwise.</returns>
        public bool IsEmpty()
        {
            return m_cards.Count == 0;
        }

        /// <summary>
        /// Move num cards from this Zone to the destination Zone dst
        /// </summary>
        /// <param name="num">The number of cards to be moved.</param>
        /// <param name="dst">The destination CardZone to put the cards.</param>
        public virtual void MoveCardsToZone(int num, CardZone dst)
        {
            Debug.Assert(num >= 0 && num <= Count());

            dst.AddCards(RemoveCards(num));
        }

        /// <summary>
        /// Realign num Card's positions in this Zone based on this Zone's (x,y)
        /// coordinate and x and y separation values.  Realignment is done with respect to the back
        /// of the Zone.
        /// <param name="num">The number of cards to realign</param>
        /// </summary>
        public virtual void RealignCards(int num)
        {
            if (Count() < num)
            {
                num = Count();
            }

            for (int i = 0; i < num; i++)
            {
                int location = Count() + i - num;
                m_cards[location].Location = new Point(m_location.X + (i * m_xSeparation), m_location.Y + (i * m_ySeparation));
            }
        }

        /// <summary>
        /// Remove some number of cards from this Zone.  Always removes from the back, that is, 
        /// from the last index backwards (no CardZone should be able to remove from the front).
        /// Updates the height and width of the Zone and unnests the top card of the remaining
        /// Zone (if applicable).  Does NOT update the locations of the cards -- cards should be 
        /// moving from one Zone to another, therefore the AddCards method is in charge of location.
        /// It is the job of the derived method to update card positions if it pulls from the 
        /// front of the Zone.
        /// </summary>
        /// <returns>Some List of cards</returns>
        /// <param name="num">The number of cards to be removed from the back of the Zone.</param>
        /// <param name="fromFront">If true, remove from the front of the zone, defaults to false.</param>
        public virtual List<Card> RemoveCards(int num, bool fromFront = false)
        {
            int location;
            if (fromFront)
            {
                location = 0;
            } 
            else
            {
                location = Count() - num;
            }

            List<Card> removed = m_cards.GetRange(location, num);
            m_cards.RemoveRange(location, num);
            m_size.X -= num * m_xSeparation;
            m_size.Y -= num * m_ySeparation;

            if (!IsEmpty())
            {
                TopCard().UnNest();
            }
            else
            {
                m_size = new Vector2(Card.Width, Card.Height);
            }

            return removed;
        }

        /// <summary>
        /// Shorthand method for getting the last card in a Zone
        /// </summary>
        /// <returns>The card in the last position of the Zone</returns>
        public Card TopCard()
        {
            if (!IsEmpty())
            {
                return m_cards[Count() - 1];
            }


            return null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the 
        /// current <see cref="T:SolitaireGame.CardZone"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current 
        /// <see cref="T:SolitaireGame.CardZone"/>.</returns>
        public override string ToString()
        {
            return "[" + String.Join(", ", m_cards) + "]";
        }
        #endregion
    }
}
