// Animation.cs
// Author: Ruben Gilbert
// 2019

using System;
using Microsoft.Xna.Framework;

namespace SolitaireGame
{
    public class Animation
    {
        private CardZone source;
        private CardZone target;
        private Selection tweener;
        private BackendGame game;
        private int numToMove;
        private int dx;
        private int dy;
        private int lastUpdateTime;

        // TODO -- make this derived from CardZones to make life easy?
        public Animation(BackendGame game, CardZone source, CardZone target, int numToMove)
        {
            this.game = game;
            this.source = source;
            this.target = target;
            this.tweener = new Selection(this.game,
                                        this.source.X,
                                        this.source.Y,
                                        this.source.XSeparation,
                                        this.source.YSeparation);
            this.numToMove = numToMove;
            this.lastUpdateTime = 0;
        }

        // -----------------------------------------------------------------------------------------
        // Methods

        public void Complete()
        {
            this.tweener.MoveCardsToZone(this.numToMove, target);
        }

        public bool Done()
        {
            return true;
        }

        public void Start()
        {

        }

        public void Update(double time)
        {

        }
    }
}
