// Animation.cs
// Author: Ruben Gilbert
// 2019

using System;
using Microsoft.Xna.Framework;

namespace SolitaireGame
{
    public class Tweener : CardZone
    {
        #region Members
        private bool m_valid;

        private CardZone m_source;
        private CardZone m_target;

        private float m_percentChange;

        private int m_numToMove;
        private int targetX;
        private int targetY;
        #endregion

        #region Properties
        public bool Valid
        {
            get => m_valid;
        }
        #endregion

        public Tweener(BackendGame game) : 
            base(game, new Point(0, 0), 0, 0)
        {
            m_valid = false;
        }

        public Tweener(BackendGame game, CardZone source, CardZone target, int numToMove) :
            base(game, source.Location, source.XSeparation, source.YSeparation)
        {
            NewAnimation(source, target, numToMove);
        }

        #region Methods
        private void Cleanup()
        {
            if (m_source is Tableau)
            {
                ((Tableau)m_source).Cleanup();
            }
            else if (m_source is Discard)
            {
                ((Discard)m_source).RealignCards(Properties.DealMode);
            }
        }

        private float Lerp(float start, float end, float by)
        {
            return start * (1 - by) + (end * by);
        }

        public void NewAnimation(CardZone source, CardZone target, int numToMove)
        {
            m_valid = true;
            m_source = source;
            m_target = target;
            m_numToMove = numToMove;
            m_percentChange = 0;

            m_location = new Point(source.Cards[source.Count() - numToMove].Location.X,
                source.Cards[source.Count() - numToMove].Location.Y);

            targetX = target.TopCard() == null ? target.Location.X : target.TopCard().Location.X + target.XSeparation;
            targetY = target.TopCard() == null ? target.Location.Y : target.TopCard().Location.Y + target.YSeparation;

            source.MoveCardsToZone(numToMove, this);
        }

        private void TestCompletion()
        {
            if (Location.X == targetX && Location.Y == targetY)
            {
                MoveCardsToZone(m_numToMove, m_target);
                Cleanup();
                m_valid = false;
            }
        }

        public void Update(GameTime time)
        {
            float delta = (float)time.ElapsedGameTime.TotalSeconds;
            float rate = 1.0f / Properties.AnimationSpeed;
            m_percentChange += delta * rate;

            m_location = new Point((int)Lerp(Location.X, targetX, m_percentChange), 
                (int)Lerp(Location.Y, targetY, m_percentChange));

            UpdateCardPositions();
            TestCompletion();
        }

        private void UpdateCardPositions()
        {
            for (int i = 0; i < Count(); i++)
            {
                m_cards[i].Location = new Point(Location.X + (m_xSeparation * i), Location.Y + (m_ySeparation * i));
            }
        }
        #endregion
    }
}
