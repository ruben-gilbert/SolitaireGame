// BackendGame.cs
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
        private Deck d;
        private List<Card> discard;
        private List<Stack<Card>> fd;
        private List<List<Card>> board;

        // Constructor
        public BackendGame()
        {
            // create Deck and shuffle it
            this.d = new Deck();
            d.Shuffle();

            // discard pile starts empty
            this.discard = new List<Card>();

            // 4 Foundations -- start as empty stacks
            this.fd = new List<Stack<Card>>();
            for (int i = 0; i < 4; i++)
            {
                this.fd.Add(new Stack<Card>());
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


        // Fills the board with Card objects by
        // dealing Cards from the Deck.  Flips
        // the last Card in each column face up
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
                            current[col] = "*";
                        else
                            current[col] = c.ToString();
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


        // Method for drawing the board (gets called from
        // MainGame.Draw() on every frame)
        public void DrawBoard(GraphicsDevice g, SpriteBatch s)
        {

            // local variables for shortness
            int width = Constants.CARD_WIDTH;
            int height = Constants.CARD_HEIGHT;
            int dkx = Constants.DECK_XCOR;
            int dky = Constants.DECK_YCOR;
            int dix = Constants.DISCARD_XCOR;
            int diy = Constants.DISCARD_YCOR;
            int dko = 0; // how much to offset each Card in the Deck

            int col_space = Constants.WINDOW_WIDTH / 7;
            int row_start = Constants.WINDOW_HEIGHT / 3;
            int hspace = col_space - width;
            int vspace = 20;
            int buf = -(hspace / 2);

            List<Card> cards = this.d.GetCards();

            // Empty border texture (for drawing the empty squares)
            Texture2D tx = new Texture2D(g, 1, 1);
            tx.SetData(new[] { Color.White });

            // Draw the Deck
            if (cards.Count == 0)
            {
                this.DrawBorder(s, tx, dkx, dkx + width, dky, dky + height);
            }
            else
            {
                foreach (Card c1 in cards)
                {
                    c1.Draw(g, s, dkx + dko, dky + dko, Color.White);
                    dko++; // draw the next Card 1 pixel down and right
                }
            }


            // Draw the Discard pile
            if (this.discard.Count == 0)
            {
                this.DrawBorder(s, tx, dix, dix + width, diy, diy + height);
            }
            else
            {
                Card c2 = this.discard[this.discard.Count - 1];
                c2.Draw(g, s, dix, diy, Color.White);
            }

            // Draw Tableus (7 columns)
            for (int col = 0; col < this.board.Count; col++)
            {
                for (int row = 0; row < this.board[col].Count; row++)
                {
                    int col_x = buf + (hspace * (col + 1)) + (width * col);
                    int col_y = row_start + (vspace * row);

                    Card c3 = this.board[col][row];
                    c3.Draw(g, s, col_x, col_y, Color.White);
                }
            }

            // Draw Foundations (scoring piles)
            int fd_col = 4; // starting column

            for (int i = 0; i < this.fd.Count; i++)
            {
                int fdx = buf + (hspace * fd_col) + (width * (fd_col - 1));
                Console.WriteLine(i);
                if (this.fd[i].Count == 0)
                {
                    this.DrawBorder(s, tx, fdx, fdx + width, dky, dky + height);
                }
                else
                {
                    Card c4 = this.fd[i].Peek();
                    c4.Draw(g, s, fdx, dky, Color.White);
                }

                fd_col++;
            }              
        }


        // Method for drawing a black border as a place holder
        // for where Cards should go (for empty piles)
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