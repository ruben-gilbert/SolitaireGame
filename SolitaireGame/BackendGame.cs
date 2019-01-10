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
        private Selection sel;

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

            // init Selection object
            this.sel = new Selection();

        }

        /*
         * *******************************
         * Methods
         * *******************************
         */
        public Deck GetDeck()
        {
            return this.d;
        }

        public List<List<Card>> GetBoard()
        {
            return this.board;
        }


        /* 
         * Fills the board with Card objects by
         * dealing Cards from the Deck.  Flips
         * the last Card in each column face up
         */
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
                Console.WriteLine(string.Format("{0,-3} {1,-3} {2,-3} " +
                	    "{3,-3} {4,-3} {5,-3} {6,-3}", current));
                Array.Clear(current, 0, current.Length);
            }
        }


        /*
         * Method for drawing the board (gets called from
         * MainGame.Draw() on every frame)
         */        
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
            int row_start = Constants.ROW_START;
            int hspace = col_space - width;
            int vspace = Constants.VSPACE;
            int buf = -(hspace / 2);

            List<Card> cards = this.d.GetCards();

            // Empty border texture (for drawing the empty squares)
            Texture2D tx = new Texture2D(g, 1, 1);
            tx.SetData(new[] { Color.White });

            // Draw the Deck
            if (cards.Count == 0)
            {
                DrawBorder(s, tx, Color.Black, dkx, dkx + width, dky, dky +
                    height, width, height);
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
                DrawBorder(s, tx, Color.Black, dix, dix + width, diy, diy + 
                    height, width, height);
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
                
                if (this.fd[i].Count == 0)
                {
                    DrawBorder(s, tx, Color.Black, fdx, fdx + width, dky, dky + 
                        height, width, height);
                }
                else
                {
                    Card c4 = this.fd[i].Peek();
                    c4.Draw(g, s, fdx, dky, Color.White);
                }

                fd_col++;
            }

            // Draw border for currently selected Card(s)
            if (this.sel.IsValid())
            {
                // TODO if the selection is valid, draw a border around it
                DrawBorder(s, tx, Color.Red, this.sel.X, this.sel.X + 
                    this.sel.W, this.sel.Y, this.sel.Y + this.sel.H, this.sel.W,
                    this.sel.H);
            }

        }


        /* 
         * Method for drawing a border either:
         * 1) for a placeholder for where Cards could go
         * 2) around current selected Cards
         */        
        private void DrawBorder(SpriteBatch s, Texture2D tx, Color c, int l, 
            int r, int t, int b, int w, int h)
        {
            int bw = 2; // border width
            s.Draw(tx, new Rectangle(l, t, bw, h), c);
            s.Draw(tx, new Rectangle(r, t, bw, h), c);
            s.Draw(tx, new Rectangle(l, t, w, bw), c);
            s.Draw(tx, new Rectangle(l, b, w, bw), c);
        }


        /*
         * Mouse Handler for mouse click events
         */        
        public void MouseClicked(int x, int y)
        {
            // check for deck click
            if (DeckClicked(x, y))
            {
                // clear the current selection
                this.sel.Clear();

                // if the deck space is clicked with no cards,
                // reset the deck and empty discard pile
                if (this.d.IsEmpty())
                {
                    foreach (Card dcard in this.discard)
                    {
                        dcard.Flip();
                        this.d.AddCard(dcard);
                    }
                    this.discard.Clear();
                }
                // the deck still has cards
                else
                {
                    // hardcoded one for now, but allows extension to other #'s
                    List<Card> drawn = this.d.Deal(1);

                    foreach (Card c in drawn)
                        c.Flip();

                    this.discard.AddRange(drawn);
                }

            }
            // check for discard pile click
            else if (DiscardClicked(x, y))
            {
                // clear the current selection
                this.sel.Clear();

                // If there are cards in discard pile,
                // make the top card the selection
                if (this.discard.Count != 0)
                {
                    List<Card> cur_sel = this.discard.GetRange(this.discard.Count - 1, 1);
                    this.sel.Change(cur_sel);
                    this.sel.X = Constants.DISCARD_XCOR;
                    this.sel.Y = Constants.DISCARD_YCOR;
                    this.sel.H = Constants.CARD_HEIGHT;
                }
            }
            // if not deck or discard, check foundation or tableu
            else
            {
                // if the top part of the screen is clicked, and there is a 
                // selection currently active, check for click on foundations
                if (y > Constants.ROW_START && this.sel.IsValid())
                {
                    //  -1: invalid clicked
                    // 0-3: number of foundation, left-to-right
                    int fd_clicked = CheckFoundations(x, y);
                    if (fd_clicked != -1)
                    {
                        // TODO if a selection is made, check if the selection
                        // can go in the foundation
                        // If no selection is made, don't do anything for now
                    }
                    else
                    {
                        this.sel.Clear();
                    }
                }
                // otherwise, check for columns
                else
                {
                    //  -1: invalid click
                    // 0-6: number of column left-to-right
                    int tab_clicked = CheckColumns(x, y);

                    if (tab_clicked != -1)
                    {
                        // TODO the column space was clicked, figure out
                        // which card was clicked (and if valid)
                    }
                    else
                    {
                        this.sel.Clear();
                    }
                }
            }
        }

        /*
         * Check if the deck is clicked
         */        
        public bool DeckClicked(int x, int y)
        {
            if (Constants.DECK_XCOR <= x && x <= Constants.DECK_XCOR + 
                Constants.CARD_WIDTH)
            {
                if (y <= Constants.DECK_YCOR + Constants.CARD_HEIGHT && 
                    Constants.DECK_YCOR <= y)
                {
                    return true;
                }
            }

            return false;
        }

        /* 
         * Check if the discard pile is clicked
         */
         public bool DiscardClicked(int x, int y)
        {
            if (Constants.DISCARD_XCOR <= x && x <= Constants.DISCARD_XCOR + 
                Constants.CARD_WIDTH)
            {
                if (y <= Constants.DECK_YCOR + Constants.CARD_HEIGHT && 
                    Constants.DECK_YCOR <= y)
                {
                    return true;
                }
            }

            return false;
        }

        /* 
         * Check if a column was clicked -- only gets called if a click was
         * in the bottom section of the screen        
         */
         public int CheckColumns(int x, int y)
        {
            // TODO figure out if column was clicked
            return -1;
        }

        /*
         * Check if a foundation was clicked -- only gets called if a click
         * was in the top section of the screen
         */
         public int CheckFoundations(int x, int y)
        {
            // TODO figure out if foundation was clicked
            return -1;
        }
    }
}