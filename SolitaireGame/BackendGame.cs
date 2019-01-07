// Game.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolitaireGame
{

    public class BackendGame
    {

        // Instance Variables
        Deck d;
        List<Card> discard;
        List<Stack<Card>> piles;
        List<List<Card>> board;

        // Constructor
        public BackendGame()
        {
            // create Deck and shuffle it
            this.d = new Deck();
            d.Shuffle();

            // discard pile starts empty
            this.discard = new List<Card>();

            // 4 piles for each suit -- start as empty stacks
            // using stacks since we only really care about the top 
            // element at any given time AND remembering previous cards
            // will allow for an undo method
            this.piles = new List<Stack<Card>>();
            for (int i = 0; i < 4; i++)
            {
                this.piles.Add(new Stack<Card>());
            }

            // init board, will be built later
            this.board = new List<List<Card>>();
            

        }

        // *******************************
        // Methods
        // *******************************

        public Deck GetDeck()
        {
            return this.d;
        }

        public List<List<Card>> GetBoard()
        {
            return this.board;
        }

        public void BuildBoard()
        {

            // build the board -- 7 columns, each column, left to right,
            // gets one more card than the column before it.  The last
            // card in each col should be face up
            for (int col = 0; col < 7; col++)
            {
                this.board.Add(new List<Card>());
                this.board[col].AddRange(this.d.Deal(col + 1));

                int size = this.board[col].Count;
                this.board[col][size - 1].Flip();
            }

        }

        // Prints the board in a nice format to the console (text-based)
        public void PrintBoard()
        {
            int colNum = this.board.Count;
            int maxRow = 14; // max number of cards in a stack is 14
            string[] current = new string[7];

            for (int row = 0; row <= maxRow; row++)
            {
                for (int col = 0; col < colNum; col++)
                {
                    int size = this.board[col].Count;

                    // if there are still cards in this col
                    if (row <= size - 1)
                    {
                        Card c = this.board[col][row];
                        if (!c.Up)
                        {
                            current[col] = "*";
                        }
                        else
                        {
                            current[col] = c.ToString();
                        }
                    }
                    else
                    {
                        current[col] = " ";
                    }
                }
                Console.WriteLine(string.Format("{0,-3} {1,-3} {2,-3} {3,-3} {4,-3} {5,-3} {6,-3}", current));
                Array.Clear(current, 0, current.Length);
            }
        }


        public void DrawBoard(GraphicsDevice g, SpriteBatch s)
        {
            int width = Constants.CARD_WIDTH;
            int height = Constants.CARD_HEIGHT;
            int deck_xcor = 10;
            int deck_ycor = 10;
            int deck_offset = 0;

            List<Card> cards = this.GetDeck().GetCards();

            //Console.WriteLine(cards.Count.ToString());


            // Draw Deck
            foreach (Card c in cards)
            {
                s.Draw(c.Back,
                    new Rectangle(deck_xcor + deck_offset, 
                        deck_ycor + deck_offset, 
                        width, 
                        height),
                    Color.White);
                deck_offset++;
            }


            // Draw playing area of board
            int col_space = Constants.WINDOW_WIDTH / 7;
            int hspace = col_space - width;
            int y_loc = Constants.WINDOW_HEIGHT / 3;
            int row_offset = 20;

            for (int col = 0; col < this.board.Count; col++)
            {
                for (int row = 0; row < this.board[col].Count; row++)
                {
                    Card current = this.board[col][row];
                    Texture2D t = current.Up ? t = current.Front : t = current.Back;
                    
                    s.Draw(t,
                            new Rectangle(-(hspace / 2) + (hspace * (col + 1)) + (width * col), 
                                y_loc + (row_offset * row), 
                                width, 
                                height),
                            Color.White);
                }
            }

            // Draw scoring piles
            Texture2D tx = new Texture2D(g, 1, 1);
            tx.SetData(new[] { Color.White });

            int p0x = -(hspace / 2) + (hspace * 4) + (width * 3);
            int p1x = -(hspace / 2) + (hspace * 5) + (width * 4);
            int p2x = -(hspace / 2) + (hspace * 6) + (width * 5);
            int p3x = -(hspace / 2) + (hspace * 7) + (width * 6);

            if (this.piles[0].Count == 0)
                this.DrawBorder(s, tx, p0x, p0x + width, deck_ycor, deck_ycor + height);
            else
                s.Draw(this.piles[0].Peek().Front, new Rectangle(p0x, deck_ycor, width, height), Color.White);

            if (this.piles[1].Count == 0)
                this.DrawBorder(s, tx, p1x, p1x + width, deck_ycor, deck_ycor + height);
            else
                s.Draw(this.piles[1].Peek().Front, new Rectangle(p1x, deck_ycor, width, height), Color.White);

            if (this.piles[2].Count == 0)
                this.DrawBorder(s, tx, p2x, p2x + width, deck_ycor, deck_ycor + height);
            else
                s.Draw(this.piles[2].Peek().Front, new Rectangle(p2x, deck_ycor, width, height), Color.White);

            if (this.piles[3].Count == 0)
                this.DrawBorder(s, tx, p3x, p3x + width, deck_ycor, deck_ycor + height);
            else
                s.Draw(this.piles[3].Peek().Front, new Rectangle(p3x, deck_ycor, width, height), Color.White);


        }

        public void DrawBorder(SpriteBatch s, Texture2D tx, int l, int r, int t, int b)
        {
            int bw = 2; // border width
            s.Draw(tx, new Rectangle(l, t, bw, Constants.CARD_HEIGHT), Color.Black);
            s.Draw(tx, new Rectangle(r, t, bw, Constants.CARD_HEIGHT), Color.Black);
            s.Draw(tx, new Rectangle(l, t, Constants.CARD_WIDTH, bw), Color.Black);
            s.Draw(tx, new Rectangle(l, b, Constants.CARD_WIDTH, bw), Color.Black);
        }
    }
}