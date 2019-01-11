// Deck.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;

namespace SolitaireGame
{
    public class Deck
    {
        private List<Card> cards;

        public Deck()
        {
            this.cards = new List<Card>();

            foreach (string suit in Constants.VALID_SUITS_ARRAY)
            {
                for (int i = 1; i < 14; i++)
                {
                    cards.Add(new Card(i, suit));
                }
            }
        }

        public Deck(List<Card> c)
        {
            this.cards = c;
        }

        public List<Card> GetCards()
        {
            return this.cards;
        }

        public bool IsEmpty()
        {
            return this.cards.Count == 0;
        }

        public void AddCard(Card c)
        {
            this.cards.Add(c);
        }

        public int Size()
        {
            return this.cards.Count;
        }

        /// <summary>
        /// Shuffles the Deck using Fisher-Yates shuffle
        /// </summary>
        public void Shuffle()
        {
            Random random = new Random();
            int n = this.cards.Count - 1;

            while (n > 1)
            {
                int choice = random.Next(n);
                Card temp = this.cards[choice];
                this.cards[choice] = this.cards[n];
                this.cards[n] = temp;
                n--;
            }
        }

        /// <summary>
        /// Deals a certain number of cards from the Deck
        /// </summary>
        /// <param name="num">The number of Cards to deal</param>
        /// <returns>A List of Cards of size num</returns>
        public List<Card> Deal(int num)
        {
            List<Card> c = this.cards.GetRange(0, num);
            this.cards.RemoveRange(0, num);
            return c;
        }

        /// <summary>
        /// String representation of this Deck
        /// </summary>
        /// <returns>A string with comma separated Card representations</returns>
        public override string ToString()
        {
            return "[" + String.Join(", ", this.cards) + "]";
        }

    }
}