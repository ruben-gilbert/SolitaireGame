// Constants.cs
// Author: Ruben Gilbert
// 2019

namespace SolitaireGame
{
    public static class Properties
    {
        // use a HashSet since we want to check containment quickly, can build it from an array
        //public readonly static string[] VALID_SUITS_ARRAY = { "H", "D", "C", "S" };
        //public readonly static HashSet<string> VALID_SUITS = new HashSet<string>(VALID_SUITS_ARRAY);

        //public readonly static int WINDOW_WIDTH = 1280;
        //public readonly static int WINDOW_HEIGHT = 1280;
        public static int WindowWidth;
        public static int WindowHeight;

        //public readonly static int TABLE_START = WINDOW_HEIGHT / 3;
        public static int TableStart;
        public readonly static int TableCardSeparation = 30;

        public static int DealMode = 3;
        public static string CardColor = "purple";

        public static int DoubleClickSpeed = 300;

        public static float AnimationSpeed = 0.1f;
    }
}