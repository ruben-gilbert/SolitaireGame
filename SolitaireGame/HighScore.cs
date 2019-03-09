// HighScore.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;


namespace SolitaireGame
{
    public class HighScore : IComparable
    {
        private int score;
        private string date;

        public HighScore(int score, string date)
        {
            this.score = score;
            this.date = date;
        }

        public int Score
        {
            set { this.score = value; }
            get { return this.score; }
        }

        public string Date
        {
            set { this.date = value; }
            get { return this.date; }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            HighScore other = obj as HighScore;

            if (other != null)
            {
                return this.score.CompareTo(other.Score);
            } else
            {
                throw new ArgumentException("Object is not a HighScore object");
            }
        }

        public override string ToString()
        {
            return this.score + "," + this.date;
        }

        public static string ConvertHighScoreToString(HighScore h)
        {
            return h.ToString();
        }
    }
}
