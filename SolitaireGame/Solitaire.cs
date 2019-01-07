using System;

namespace SolitaireGame
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Solitaire
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
