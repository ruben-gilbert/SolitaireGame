// Animation.cs
// Author: Ruben Gilbert
// 2019

using System;
using Microsoft.Xna.Framework;

namespace SolitaireGame
{
    public class Tweener : CardZone
    {
        private CardZone source;
        private CardZone target;
        private bool valid;
        private int numToMove;
        public int xDistance;
        public int yDistance;
        private float percentChange;

        public Tweener(BackendGame game) : 
            base(game, 0, 0, 0, 0)
        {
            this.game = game;
            this.valid = false;
        }

        public Tweener(BackendGame game, CardZone source, CardZone target, int numToMove) :
            base(game, source.X, source.Y, source.XSeparation, source.YSeparation)
        {
            this.game = game;
            this.NewAnimation(source, target, numToMove);
        }

        // -----------------------------------------------------------------------------------------
        // Getters / Setters

        public bool Valid
        {
            get { return this.valid; }
        }

        // -----------------------------------------------------------------------------------------
        // Methods

        private void Cleanup()
        {
            if (this.source is Tableau)
            {
                ((Tableau)this.source).Cleanup();
            }
            else if (this.source is Discard)
            {
                ((Discard)this.source).RealignCards(GameProperties.DEAL_MODE);
            }
        }

        private float Lerp(float start, float end, float by)
        {
            return start * (1 - by) + (end * by);
        }

        public void NewAnimation(CardZone source, CardZone target, int numToMove)
        {
            this.valid = true;
            this.source = source;
            this.target = target;
            this.numToMove = numToMove;
            this.percentChange = 0;

            this.x = this.source.Cards[this.source.Size() - numToMove].X;
            this.y = this.source.Cards[this.source.Size() - numToMove].Y;

            this.xDistance = this.target.X - this.X;
            this.yDistance = this.target.Y - this.Y;

            this.source.MoveCardsToZone(numToMove, this);
        }

        private void TestCompletion()
        {
            if (this.X == this.target.X && this.Y == this.target.Y)
            {
                this.MoveCardsToZone(this.numToMove, target);
                this.Cleanup();
                this.valid = false;
            }
        }

        public void Update(GameTime time)
        {
            float delta = (float)time.ElapsedGameTime.TotalSeconds;
            float rate = 1.0f / GameProperties.ANIMATION_SPEED;
            this.percentChange += delta * rate;

            this.x = (int)Lerp(this.x, this.target.X, this.percentChange);
            this.y = (int)Lerp(this.y, this.target.Y, this.percentChange);

            this.UpdateCardPositions();

            this.TestCompletion();
        }

        private void UpdateCardPositions()
        {
            for (int i = 0; i < this.Size(); i++)
            {
                this.cards[i].X = this.x + (this.xSeparation * i);
                this.cards[i].Y = this.Y + (this.ySeparation * i);
            }
        }
    }
}
