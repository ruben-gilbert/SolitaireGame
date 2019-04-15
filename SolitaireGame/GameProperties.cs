// Constants.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;

namespace SolitaireGame
{
    public static class GameProperties
    {
        // use a HashSet since we want to check containment quickly, can build it from an array
        public readonly static string[] VALID_SUITS_ARRAY = { "H", "D", "C", "S" };
        public readonly static HashSet<string> VALID_SUITS = new HashSet<string>(VALID_SUITS_ARRAY);

        //public readonly static int WINDOW_WIDTH = 1280;
        //public readonly static int WINDOW_HEIGHT = 1280;
        public static int WINDOW_WIDTH;
        public static int WINDOW_HEIGHT;

        public readonly static int CARD_WIDTH = 131;
        public readonly static int CARD_HEIGHT = 200;

        public readonly static int DECK_XCOR = 10;
        public readonly static int DECK_YCOR = 10;

        public readonly static int DISCARD_XCOR = DECK_XCOR + CARD_WIDTH + 30;
        public readonly static int DISCARD_YCOR = DECK_YCOR;
        public readonly static int DISCARD_SEPARATION = 20;

        public readonly static int FOUNDATION_YCOR = DECK_YCOR;

        //public readonly static int TABLE_START = WINDOW_HEIGHT / 3;
        public static int TABLE_START;
        public readonly static int TABLE_CARD_SEPARATION = 30;

        public static int DEAL_MODE = 3;
        public static string CARD_COLOR = "purple";

        public static int DOUBLE_CLICK_SPEED = 300;

        public static float ANIMATION_SPEED = 0.1f;
    }
}