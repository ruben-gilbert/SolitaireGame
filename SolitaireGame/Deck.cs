// Deck.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;

namespace SolitaireGame
{
    public class Deck
    {
        // Instance Variables
        private List<Card> cards;

        // Constructors
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

        // *******************************
        // Methods
        // *******************************

        public ref List<Card> GetCards()
        {
            return ref this.cards;
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

        // Use simple Fisher-Yates shuffle
        public void Shuffle()
        {
            Random random = new Random();
            int n = this.cards.Count - 1;

            int choice;
            Card temp;

            while (n > 1)
            {
                choice = random.Next(n);
                temp = this.cards[choice];
                this.cards[choice] = this.cards[n];
                this.cards[n] = temp;
                n--;
            }
        }

        // Take a slice off the front of the List
        public List<Card> Deal(int num)
        {
            List<Card> c = this.cards.GetRange(0, num);
            this.cards.RemoveRange(0, num);
            return c;
        }

        public override string ToString()
        {
            return "[" + String.Join(", ", this.cards) + "]";
        }

    }
}