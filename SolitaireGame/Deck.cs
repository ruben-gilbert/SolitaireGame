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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Deck"/> class.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.Deck"/> class.  Uses a 
        /// specific set of cards, instead of generating from scratch.
        /// </summary>
        /// <param name="c">Some List of Cards</param>
        public Deck(List<Card> c)
        {
            this.cards = c;
        }

        /// <summary>
        /// Gets the List of Card objects this Deck holds.
        /// </summary>
        /// <returns>A List of Cards</returns>
        public List<Card> GetCards()
        {
            return this.cards;
        }

        /// <summary>
        /// Checks to see if this Deck is empty
        /// </summary>
        /// <returns><c>true</c>, if the Deck is empty, <c>false</c> otherwise.</returns>
        public bool IsEmpty()
        {
            return this.cards.Count == 0;
        }

        /// <summary>
        /// Adds a Card to this Deck.
        /// </summary>
        /// <param name="c">Some Card object.</param>
        public void AddCard(Card c)
        {
            this.cards.Add(c);
        }

        /// <summary>
        /// Gets the size of this Deck (i.e. the number of Cards in it).
        /// </summary>
        /// <returns>The number of Cards in this Deck.</returns>
        public int Size()
        {
            return this.cards.Count;
        }

        /// <summary>
        /// Shuffles the Deck 3 times using Fisher-Yates shuffle
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < 3; i++)
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