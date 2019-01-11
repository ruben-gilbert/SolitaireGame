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
        private Deck d;
        private List<Card> discard;
        private List<Stack<Card>> foundations;
        private List<List<Card>> board;
        private Selection sel;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.BackendGame"/> class.
        /// </summary>
        public BackendGame()
        {
            // Create Deck and shuffle it
            this.d = new Deck();
            d.Shuffle();

            // Discard pile starts empty
            this.discard = new List<Card>();

            // 4 Foundations -- start as empty stacks
            this.foundations = new List<Stack<Card>>();
            for (int i = 0; i < 4; i++)
            {
                this.foundations.Add(new Stack<Card>());
            }

            // Init the board as empty, will be built later
            this.board = new List<List<Card>>();

            // Init Selection object
            this.sel = new Selection();

        }

        /// <summary>
        /// Gets the Deck object associated with this Game
        /// </summary>
        /// <returns>The Deck</returns>
        public Deck GetDeck()
        {
            return this.d;
        }

        /// <summary>
        /// Gets the board associated with this Game
        /// </summary>
        /// <returns>The board (a List of Lists of Cards)</returns>
        public List<List<Card>> GetBoard()
        {
            return this.board;
        }


        /// <summary>
        /// Fills the board with Card objects by dealing Cards from the Deck.  
        /// Flips the last Card in each column face up.  The board is 7 columns
        /// with 1, 2, 3, etc cards in each column, left-to-right.
        /// </summary>
        public void BuildBoard()
        {
            for (int col = 0; col < 7; col++)
            {
                this.board.Add(new List<Card>());
                this.board[col].AddRange(this.d.Deal(col + 1));
                this.board[col][col].Flip();
            }
        }

        /// <summary>
        /// Prints the board in a nice format to the console (text-based)
        /// </summary>
        public void PrintBoard()
        {
            int colNum = this.board.Count;
            int maxCards = 14;
            string[] current = new string[7];

            for (int row = 0; row <= maxCards; row++)
            {
                for (int col = 0; col < colNum; col++)
                {
                    int size = this.board[col].Count;

                    // If there are still cards in this column
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

        /// <summary>
        /// Draws the game board.  Called upon from MainGame.Draw().  The game
        /// is broken up into 4 structures: the deck, the discard pile, the
        /// foundations (scoring areas), and the tableaus (7 columns).  We also want to
        /// highlight the current selection (either a single card or multiple cards in a column)
        /// in a red border.
        /// </summary>
        /// <param name="g">The graphics object driving the game</param>
        /// <param name="s">The 2D drawing object</param>
        public void DrawGame(GraphicsDevice g, SpriteBatch s)
        {
            int width = Constants.CARD_WIDTH;
            int height = Constants.CARD_HEIGHT;
            int deckX = Constants.DECK_XCOR;
            int deckY = Constants.DECK_YCOR;
            int discardX = Constants.DISCARD_XCOR;
            int discardY = Constants.DISCARD_YCOR;
            int deckOffset = 0; 

            int colSpace = (Constants.WINDOW_WIDTH / 7) - width;
            int row_start = Constants.TABLE_START;
            int sep = Constants.TABLE_CARD_SEPARATION;
            int buf = -(colSpace / 2);

            List<Card> cards = this.d.GetCards();

            // Empty border texture (for drawing the empty squares)
            Texture2D tx = new Texture2D(g, 1, 1);
            tx.SetData(new[] { Color.White });

            // Draw the Deck
            if (cards.Count == 0)
            {
                DrawBorder(s, tx, Color.Black, deckX, deckY, width, height);
            }
            else
            {
                foreach (Card c1 in cards)
                {
                    c1.Draw(s, deckX + deckOffset, deckY + deckOffset, Color.White);
                    deckOffset++;
                }
            }

            // Draw the Discard pile
            if (this.discard.Count == 0)
            {
                DrawBorder(s, tx, Color.Black, discardX, discardY, width, height);
            }
            else
            {
                Card c2 = this.discard[this.discard.Count - 1];
                c2.Draw(s, discardX, discardY, Color.White);
            }

            // Draw Tableus (7 columns)
            for (int col = 0; col < this.board.Count; col++)
            {
                for (int row = 0; row < this.board[col].Count; row++)
                {
                    int colX = buf + (colSpace * (col + 1)) + (width * col);
                    int colY = row_start + (sep * row);

                    Card c3 = this.board[col][row];
                    c3.Draw(s, colX, colY, Color.White);
                }
            }

            // Draw Foundations (scoring piles)
            int foundCol = 4;

            for (int i = 0; i < this.foundations.Count; i++)
            {
                int foundX = buf + (colSpace * foundCol) + (width * (foundCol - 1));
                
                if (this.foundations[i].Count == 0)
                {
                    DrawBorder(s, tx, Color.Black, foundX, deckY, width, height);
                }
                else
                {
                    Card c4 = this.foundations[i].Peek();
                    c4.Draw(s, foundX, deckY, Color.White);
                }

                foundCol++;
            }

            // Draw border for currently selected Card(s)
            if (this.sel.IsValid())
            {
                DrawBorder(s, tx, Color.Red, this.sel.X, this.sel.Y, this.sel.W, this.sel.H);
            }

        }

        /// <summary>
        /// Draws a border.  Assumes x, y is the top-left corner.
        /// </summary>
        /// <param name="s">SpriteBatch object for drawing 2D textures</param>
        /// <param name="tx">Some 2D texture</param>
        /// <param name="c">The color of the border</param>
        /// <param name="x">The X-coordinate of the border</param>
        /// <param name="y">The Y-coordinate of the border</param>
        /// <param name="w">The width of the border (left-to-right, NOT thickness)</param>
        /// <param name="h">The height of the border</param>
        private void DrawBorder(SpriteBatch s, Texture2D tx, Color c, int x, int y, int w, int h)
        {
            int thickness = 2;

            s.Draw(tx, new Rectangle(x, y, thickness, h), c);
            s.Draw(tx, new Rectangle(x + w, y, thickness, h), c);
            s.Draw(tx, new Rectangle(x, y, w, thickness), c);
            s.Draw(tx, new Rectangle(x, y + h, w, thickness), c);
        }

        /// <summary>
        /// Mouse handler for the game.  Determines what, if anything, was clicked on.  Called from 
        /// MainGame.Update() every time a mouse click is detected.
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse click</param>
        /// <param name="y">The Y-coordinate of the mouse click</param>        
        public void MouseClicked(int x, int y)
        {
            // Check for deck click
            if (DeckClicked(x, y))
            {
                this.sel.Clear();

                // If the deck has no cards, reset the deck and empty discard pile
                if (this.d.IsEmpty())
                {
                    foreach (Card c in this.discard)
                    {
                        c.Flip();
                        this.d.AddCard(c);
                    }
                    this.discard.Clear();
                }
                // Otherwise, the deck still has cards
                else
                {
                    // Deal one card into the discard pile.  Hardcoded to 1 for now, can extend to
                    // multiple card deals at some point
                    // TODO multiple card deals
                    List<Card> drawn = this.d.Deal(1);

                    foreach (Card c in drawn)
                        c.Flip();

                    this.discard.AddRange(drawn);
                }

            }
            // Check for discard pile click
            else if (DiscardClicked(x, y))
            {
                // If there are cards in discard pile, make the top card the selection unless
                // it is already selected, in which case, unselect it
                if (this.discard.Count > 0)
                {
                    List<Card> test = new List<Card>(
                        this.discard.GetRange(this.discard.Count - 1, 1));

                    if (this.sel.Size() == 1 && this.sel.IsValid() && this.sel.CompareCards(test))
                    {
                        this.sel.Clear();
                    }
                    else
                    {
                        List<Card> cur_sel = this.discard.GetRange(this.discard.Count - 1, 1);
                        this.sel.Change(cur_sel);
                        this.sel.X = Constants.DISCARD_XCOR;
                        this.sel.Y = Constants.DISCARD_YCOR;
                        this.sel.H = Constants.CARD_HEIGHT;
                    }
                }
            }
            // If the click is not the deck or discard pile, check the foundations or table
            else
            {
                // If the top part of the screen is clicked, AND there is a 
                // selection currently active, check for click on foundations
                if (y > Constants.TABLE_START && this.sel.IsValid())
                {
                    //  -1: invalid clicked
                    // 0-3: number of foundation, left-to-right
                    int fd_clicked = CheckFoundationClick(x, y);

                    if (fd_clicked != -1)
                    {
                        // TODO check if the selection can go in the foundation
                        // If no selection is made, don't do anything for now
                    }
                    else
                    {
                        this.sel.Clear();
                    }
                }
                // Otherwise, check if one of the columns was clicked
                else
                {
                    //  -1: invalid click
                    // 0-6: number of column left-to-right
                    int tab_clicked = CheckColumnClick(x, y);

                    if (tab_clicked != -1)
                    {
                        // TODO the column space was clicked, figure out which card was clicked (and if valid)

                        // TODO If there is already a valid selection, see if it applies to this column

                        // TODO Otherwise, select the column if a flipped Card was clicked
                    }
                    else
                    {
                        this.sel.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the deck's area was clicked
        /// </summary>
        /// <param name="x">X-coordinate of the click</param>
        /// <param name="y">Y-coordinate of the click</param>
        /// <returns>True if inside the bounds of the deck was clicked, False otherwise</returns>        
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

        /// <summary>
        /// Checks if the discard pile's area was clicked
        /// </summary>
        /// <param name="x">X-coordinate of the click</param>
        /// <param name="y">Y-coordinate of the click</param>
        /// <returns>True if inside the bounds of the discard pile was clicked, 
        /// False otherwise</returns> 
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

        // TODO think of the best way to do this
        public int CheckColumnClick(int x, int y)
        {
            // TODO figure out if column was clicked
            return -1;
        }

        // TODO think of the best way to do this
        public int CheckFoundationClick(int x, int y)
        {
            // TODO figure out if foundation was clicked
            return -1;
        }
    }
}