// BackendGame.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SolitaireGame
{

    public class BackendGame
    {
        private Deck deck;
        private Discard discard;
        private Selection selection;
        private List<Foundation> foundations;
        private List<Tableau> tableaus;
        private List<CardZone> board;
        private int score;

        public BackendGame(GraphicsDevice g, MainGame game)
        {
            this.NewGame(g, game);
        }

        public void NewGame(GraphicsDevice g, MainGame game)
        {
            // TODO add buttons and menus, etc?

            this.board = new List<CardZone>();
            this.deck = new Deck(GameProperties.DECK_XCOR,
                                GameProperties.DECK_YCOR,
                                1,
                                1,
                                g,
                                game);
            //this.deck.Shuffle();
            this.board.Add(this.deck);

            this.discard = new Discard(GameProperties.DISCARD_XCOR,
                                GameProperties.DISCARD_YCOR,
                                GameProperties.DISCARD_SEPARATION,
                                0,
                                g);
            this.board.Add(this.discard);

            foundations = new List<Foundation>();
            int foundX = GameProperties.WINDOW_WIDTH / 2;
            int foundSpace = (foundX - (GameProperties.CARD_WIDTH * 4)) / 4;
            for (int i = 0; i < 4; i++)
            {
                Foundation f = new Foundation(foundX,
                                    GameProperties.FOUNDATION_YCOR,
                                    0,
                                    0,
                                    g);
                this.foundations.Add(f);
                this.board.Add(f);
                foundX += foundSpace + GameProperties.CARD_WIDTH;
            }

            tableaus = new List<Tableau>();

            int tableSpace = (GameProperties.WINDOW_WIDTH - (GameProperties.CARD_WIDTH * 7)) / 8;
            int tabX = tableSpace;
            for (int j = 1; j < 8; j++)
            {
                Tableau t = new Tableau(tabX,
                                    GameProperties.WINDOW_HEIGHT / 2,
                                    0,
                                    GameProperties.TABLE_CARD_SEPARATION,
                                    g);
                this.deck.MoveCardsToZone(j, t);
                this.tableaus.Add(t);
                this.board.Add(t);
                tabX += tableSpace + GameProperties.CARD_WIDTH;
            }

            this.selection = new Selection(0, 0, 0, 0, g);

            this.score = 0;
        }

        /// <summary>
        /// Draw the game.  Ask each CardZone in the board to draw itself and, if there is a valid
        /// selection, draw it as well.
        /// </summary>
        /// <param name="g">The GraphicsDevice for this game</param>
        /// <param name="s">The SpriteBatch object that will handle drawing.</param>
        public void Draw(GraphicsDevice g, SpriteBatch s)
        {
            foreach (CardZone cz in this.board)
            {
                cz.Draw(s);
            }

            // Only draw the selection if there is something in it 
            if (!this.selection.IsEmpty())
            {
                this.selection.Draw(s);
            }
        }

        /// <summary>
        /// Checks if the game is over.
        /// </summary>
        /// <returns><c>true</c>, if all Foundations are full, <c>false</c> otherwise.</returns>
        public bool GameOver()
        {
            foreach (Foundation f in this.foundations) {
                if (f.Size() != 13)
                {
                    return false;
                }
            }

            return true;
        }

        public void HandleDoubleClick(int x, int y)
        {
            // TODO -- auto score card if only one is double-clicked
        }

        public void HandleMouseDown(int x, int y)
        {
            
            //Console.WriteLine("MOUSE DOWN (" + x + ", " + y + ")");

            CardZone clicked = null;
            foreach (CardZone zone in this.board)
            {
                if (zone.IsClicked(x, y))
                {
                    clicked = zone;
                    break;
                }
            }

            if (clicked != null)
            {
                if (clicked is Deck)
                {
                    ((Deck)clicked).Select();
                }
                else
                {
                    // TODO -- make play into Selection?
                    int numToMove = clicked.GetClicked(x, y);
                    if (numToMove != -1)
                    {
                        this.selection.SetSourceZone(clicked);
                        this.selection.SetRelativeOffsets(numToMove, x, y);
                        clicked.MoveCardsToZone(numToMove, this.selection);
                    }
                }
            }
            
        }

        public void HandleMouseUp(int x, int y)
        {
            // If the selection isn't empty, it's valid and we should consider playing it
            if (!this.selection.IsEmpty())
            {
                // TODO handle playing the selection at the mouse's location

                // If placement is valid, play it

                // otherwise, return to source
                this.selection.ReturnToSource();
            }
            else
            {
                // If there's no selection to play, we only need to consider if the deck was clicked
                if (this.deck.IsSelected())
                {
                    if (this.deck.IsClicked(x, y))
                    {
                        if (this.deck.IsEmpty())
                        {
                            this.discard.MoveCardsToZone(this.discard.Size(), this.deck);
                        }
                        else
                        {
                            this.deck.MoveCardsToZone(GameProperties.DEAL_MODE, this.discard);
                        }
                    }

                    this.deck.Deselect();
                }
            }
        }

        public void UpdateSelection(int x, int y)
        {
            this.selection.UpdatePosition(x, y);
        }
    }
}

        /*
        private Deck d;
        private List<Card> discard;
        private List<Stack<Card>> foundations;
        private List<List<Card>> board;
        private Selection sel;
        private int score;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SolitaireGame.BackendGame"/> class.
        /// </summary>
        public BackendGame()
        {
            this.NewGame();
        }

        /// <summary>
        /// Called via the BackendGame constructor to make the object
        /// </summary>
        public void NewGame()
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

            // Init score
            this.score = 0;
 
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
        /// Gets the score of this Game
        /// </summary>
        /// <returns>The score of this Game</returns>
        public int GetScore()
        {
            return this.score;
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
            int width = GameProperties.CARD_WIDTH;
            int height = GameProperties.CARD_HEIGHT;
            int deckX = GameProperties.DECK_XCOR;
            int deckY = GameProperties.DECK_YCOR;
            int discardX = GameProperties.DISCARD_XCOR;
            int discardY = GameProperties.DISCARD_YCOR;
            int deckOffset = 0; 

            int colSpace = (GameProperties.WINDOW_WIDTH / 7) - width;
            int row_start = GameProperties.TABLE_START;
            int sep = GameProperties.TABLE_CARD_SEPARATION;
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
                if (GameProperties.DEAL_MODE == 1)
                {
                    Card c2 = this.discard[this.discard.Count - 1];
                    c2.Draw(s, discardX, discardY, Color.White);
                }
                else if (GameProperties.DEAL_MODE == 3)
                {
                    List<Card> discardToShow;
                    if (this.discard.Count < 3)
                    {
                        discardToShow = this.discard.GetRange(0, this.discard.Count);
                    }
                    else
                    {
                        discardToShow = this.discard.GetRange(this.discard.Count - 3, 3);
                    }

                    for (int i = 0; i < discardToShow.Count; i++)
                    {
                        Card c2 = discardToShow[i];
                        c2.Draw(s,
                            discardX + (GameProperties.DISCARD_SEPARATION * i),
                            discardY,
                            Color.White);
                    }
                }
                
            }

            // Draw Tableus (7 columns)
            for (int col = 0; col < this.board.Count; col++)
            {
                int colX = buf + (colSpace * (col + 1)) + (width * col);

                if (this.board[col].Count == 0)
                {
                    DrawBorder(s, tx, Color.Black, colX, row_start, width, height);
                }
                else
                {
                    for (int row = 0; row < this.board[col].Count; row++)
                    {
                        int colY = row_start + (sep * row);

                        Card c3 = this.board[col][row];
                        c3.Draw(s, colX, colY, Color.White);
                    }
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
        /// Mouse handler for the game.  Determines what, if anything, was clicked on.  Called from 
        /// MainGame.Update() every time a mouse click is detected.
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse click</param>
        /// <param name="y">The Y-coordinate of the mouse click</param> 
        /// <param name="doubleClick">Boolean signifying if the click was a double click</param>       
        public void MouseClicked(int x, int y, bool doubleClick)
        {
            // Check for deck click
            if (DeckClicked(x, y))
            {
                this.sel.Clear();

                // If the deck has no cards, reset the deck and empty discard pile,
                // and remove 100 points
                if (this.d.IsEmpty())
                {
                    foreach (Card c in this.discard)
                    {
                        c.Flip();
                        this.d.AddCard(c);
                    }
                    this.discard.Clear();

                    // Only remove points for a re-deal if we are drawing one card per flip
                    if (GameProperties.DEAL_MODE == 1)
                    {
                        // The min score allowed is 0, so be sure to cap it if we have less than 100 pts
                        this.score = this.score < 100 ? 0 : this.score - 100;
                    }

                }
                // Otherwise, the deck still has cards
                else
                {
                    // Deal card(s) into the discard pile.
                    List<Card> drawn = this.d.Deal(GameProperties.DEAL_MODE);

                    foreach (Card c in drawn)
                    {
                        c.Flip();
                    }
                        
                    this.discard.AddRange(drawn);
                }

            }
            // Check for discard pile click
            else if (DiscardClicked(x, y))
            {
                if (this.discard.Count > 0)
                {
                    // It was a valid click, but which card should we select?
                    int pos = this.discard.Count >= 3 ? 2 : (this.discard.Count == 2 ? 1 : 0);

                    if (doubleClick)
                    {
                        this.sel.Clear();
                        List<Card> cur_sel = this.discard.GetRange(this.discard.Count - 1, 1);

                        this.sel.Change(cur_sel, 7,
                            GameProperties.DISCARD_XCOR + (pos * GameProperties.DISCARD_SEPARATION),
                            GameProperties.DECK_YCOR,
                            GameProperties.CARD_HEIGHT);

                        // Try to score the top card of the discard pile.  If it can be scored,
                        // score move it to foundation and update the score
                        if (CanBeScored())
                        {
                            this.score += 10;
                        }
                    }
                    else
                    {
                        // Make the top card the selection unless
                        // it is already selected, in which case, unselect it
                        List<Card> test = new List<Card>(
                            this.discard.GetRange(this.discard.Count - 1, 1)
                        );

                        if (this.sel.Size() == 1 && this.sel.IsValid() && this.sel.CompareCards(test))
                        {
                            this.sel.Clear();
                        }
                        else
                        {
                            List<Card> cur_sel = this.discard.GetRange(this.discard.Count - 1, 1);
                            this.sel.Change(cur_sel, 7,
                                GameProperties.DISCARD_XCOR + (pos * GameProperties.DISCARD_SEPARATION),
                                GameProperties.DECK_YCOR,
                                GameProperties.CARD_HEIGHT);
                        }
                    }
                }
            }
            // If the click is not the deck or discard pile, check the foundations or table
            else
            {
                // If the top part of the screen is clicked, check for click on foundations
                if (y < GameProperties.TABLE_START)
                {
                    //  -1: invalid clicked
                    // 0-3: number of foundation, left-to-right
                    int fdNumClicked = CheckFoundationClick(x, y);

                    // If a foundation was clicked, try to handle it
                    if (fdNumClicked != -1)
                    {
                        HandleFoundationClick(fdNumClicked);
                    }
                    else
                    {
                        this.sel.Clear();
                    }
                }
                // Otherwise, check if one of the columns was clicked
                else
                {
                    // Tuple of structure (colNum, rowNum)
                    Tuple<int, int> locClicked = CheckColumnClick(x, y);

                    if (locClicked != null)
                    {
                        if (doubleClick)
                        {
                            HandleColumnClick(locClicked, true);
                        }
                        else
                        {
                            HandleColumnClick(locClicked, false);
                        }
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
            if (GameProperties.DECK_XCOR <= x && x <= GameProperties.DECK_XCOR + 
                GameProperties.CARD_WIDTH)
            {
                if (y <= GameProperties.DECK_YCOR + GameProperties.CARD_HEIGHT && 
                    GameProperties.DECK_YCOR <= y)
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
            // Can handle both single card mode AND three card mode with less than 2 cards
            if (GameProperties.DEAL_MODE == 1 || this.discard.Count < 2)
            {
                if (GameProperties.DISCARD_XCOR <= x && 
                    x <= GameProperties.DISCARD_XCOR + GameProperties.CARD_WIDTH)
                {
                    if (y <= GameProperties.DECK_YCOR + GameProperties.CARD_HEIGHT &&
                        GameProperties.DECK_YCOR <= y)
                    {
                        return true;
                    }
                }
            }
            else if (GameProperties.DEAL_MODE == 3)
            {
                // Do we need to offset by 1 or 2 cards?
                int position = this.discard.Count >= 3 ? 2 : 1;

                int cardX = GameProperties.DISCARD_XCOR + 
                            (position * GameProperties.DISCARD_SEPARATION);

                if (cardX <= x && x <= cardX + GameProperties.CARD_WIDTH)
                {
                    if (y <= GameProperties.DECK_YCOR + GameProperties.CARD_HEIGHT &&
                        GameProperties.DECK_YCOR <= y)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Checks if a column has been clicked.  This method is only ever invoked thru
        /// MouseClicked in response to a click below the halfway point of the board.
        /// </summary>
        /// <param name="x">The X-coordinate of the click.</param>
        /// <param name="y">The Y-coordinate of the click.</param>
        /// <returns>A 2 item Tuple containing (int, int).  First integer is the
        /// column number, the second integer is the row number.  Returns null if the click
        /// is not valid</returns>
        public Tuple<int, int> CheckColumnClick(int x, int y)
        {
            int sep = GameProperties.TABLE_CARD_SEPARATION;
            int w = GameProperties.CARD_WIDTH;
            int h = GameProperties.CARD_HEIGHT;            
            int colSpace = (GameProperties.WINDOW_WIDTH / 7) - w;
            int buf = -(colSpace / 2);
            int start = GameProperties.TABLE_START;

            for (int col = 0; col <= 6; col++)
            {
                int numCards = this.board[col].Count;

                int colHeight = (numCards - 1) * sep + h;
                int left = buf + (colSpace * (col + 1)) + (w * col);
                int right = left + w;

                // If the click is in the correct left-right bounds of the column
                if (left <= x && x <= right)
                {
                    // If the click is within the actual span of the cards
                    if (start <= y && y <= start + colHeight)
                    {
                        // Check the last card first since it is treated specially
                        if (start + colHeight - h <= y && y <= start + colHeight)
                        {
                            return new Tuple<int, int>(col, numCards - 1);
                        }
                        else
                        {
                            // For all cards except the last in the column
                            for (int row = 0; row < numCards - 1; row++)
                            {
                                // Check if the click was within the small bounds for it
                                if (start + (sep * row) <= y && y < start + (sep * (row + 1)))
                                {
                                    return new Tuple<int, int>(col, row);
                                }
                            }
                        }
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// Given the (x,y) coordinates of a click, properly handles
        /// the click if it was on a column of cards
        /// </summary>
        /// <param name="locClicked">Tuple storing the (x,y) location clicked.</param>
        public void HandleColumnClick(Tuple<int, int> locClicked, bool doubleClick)
        {
            int col = locClicked.Item1;
            int row = locClicked.Item2;
            int cardsInCol = this.board[col].Count;
            bool validMove = false;

            Card clicked = null;
            int count = -1;

            // If there is a currently valid selection, and we didn't enter this method
            // thinking it was a double click, check if it can be played in this column
            if (this.sel.IsValid() && !doubleClick)
            {
                // If the column is empty, we can only play a selection starting with a king
                if (cardsInCol == 0)
                { 
                    if (this.sel.Cards[0].Val == 13)
                    {
                        validMove = true;
                    }
                }
                // Otherwise, check if the last card in the column is one number greater AND the 
                // opposite suit of the first card in the selection
                else
                {
                    Card lastInCol = this.board[col][cardsInCol - 1];
                    Card firstInSel = this.sel.Cards[0];

                    if (lastInCol.OppositeSuits(firstInSel))
                    {
                        if (lastInCol.Val == firstInSel.Val + 1)
                        {
                            validMove = true;
                        }
                    }
                }

                if (validMove)
                {
                    // If selection is from discard, remove it from the pile and add 5 points
                    if (this.sel.Source == 7)
                    {
                        this.discard.Remove(this.sel.Cards[0]);
                        this.score += 5;
                    }
                    else if (this.sel.Source > 7)
                    {
                        this.foundations[this.sel.Source - 8].Pop();
                        this.score = this.score < 15 ? 0 : this.score - 15;
                    }
                    else
                    {
                        // Otherwise, the selection is from a column.  Remove it from the current
                        // column and place it in the new column
                        int cardsInSourceCol = this.board[this.sel.Source].Count;
                        int startIndex = cardsInSourceCol - this.sel.Size();
                        this.board[this.sel.Source].RemoveRange(startIndex, this.sel.Size());

                        // If the source column still has cards in it, and the last card is face
                        // down, flip it face up and add 5 points
                        int cardsRemaining = this.board[this.sel.Source].Count;
                        if (cardsRemaining > 0 &&
                            !this.board[this.sel.Source][cardsRemaining - 1].Up)
                        {
                            this.board[this.sel.Source][cardsRemaining - 1].Flip();
                            this.score += 5;
                        }
                    }

                    this.board[col].AddRange(this.sel.Cards);
                    
                }

                this.sel.Clear();

            }
            else
            {
                if (cardsInCol > 0)
                {
                    clicked = this.board[col][row];
                    count = cardsInCol - row;

                    if (clicked.Up)
                    {
                        int w = GameProperties.CARD_WIDTH;
                        int h = GameProperties.CARD_HEIGHT;
                        int sep = GameProperties.TABLE_CARD_SEPARATION;
                        int start = GameProperties.TABLE_START;
                        int colSpace = (GameProperties.WINDOW_WIDTH / 7) - w;
                        int buf = -(colSpace / 2);
                        int colHeight = (count - 1) * sep + h;

                        int selX = buf + (colSpace * (col + 1)) + (w * col);
                        int selY = start + (sep * row);
                        int selH = (sep * (count - 1)) + h;

                        this.sel.Change(this.board[col].GetRange(row, count), col, selX, selY, selH);

                        // Don't bother trying to score the selection if there is more than one 
                        // card selected
                        if (doubleClick && count == 1 && clicked != null)
                        {
                            if (CanBeScored())
                            {
                                this.score += 10;
                            }
                        }

                    }
                }
            }

            
        }

        /// <summary>
        /// Checks if a foundation has been clicked.  This method is only ever invoked thru
        /// MouseClicked in response to a click above the table section of the board.
        /// </summary>
        /// <param name="x">The X-coordinate of the click.</param>
        /// <param name="y">The Y-coordinate of the click.</param>
        /// <returns>An integer between 0 and 3, representing the foundation that was clicked
        /// (left-to-right.  Returns -1 if no valid foundation was clicked.</returns>
        public int CheckFoundationClick(int x, int y)
        {
            int w = GameProperties.CARD_WIDTH;
            int h = GameProperties.CARD_HEIGHT;
            int colSpace = (GameProperties.WINDOW_WIDTH / 7) - w;
            int buf = -(colSpace / 2);
            int top = GameProperties.DECK_YCOR;
            int bottom = GameProperties.DECK_YCOR + h;

            for (int fd = 0; fd <= 3; fd++)
            {
                int left = buf + (colSpace * (fd + 4)) + (w * ((fd + 4) - 1));
                int right = left + w;

                if (left <= x && x <= right)
                {
                    if (top <= y && y <= bottom)
                    {
                        return fd;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Given the number of which foundation was clicked, checks to see if the current
        /// selection can be applied to the foundation.  This method shouldn't get called
        /// if there is not a valid selection (i.e. clicking on foundations without something
        /// selected prior shouldn't do anything)
        /// </summary>
        /// <param name="fdNum">The number, 0-3, of the foundation clicked (left-to-right)</param>
        public void HandleFoundationClick(int fdNum)
        {
            bool validMove = false;

            // If the selection is already valid, try to play it into the foundation
            if (this.sel.IsValid())
            {
                // Selection's of more than one card can't be played into the foundation
                if (this.sel.Size() == 1)
                {
                    Card selCard = this.sel.Cards[0];

                    if (this.foundations[fdNum].Count == 0)
                    {
                        if (selCard.Val == 1)
                        {
                            validMove = true;
                        }
                    }
                    else
                    {
                        Card fdCard = this.foundations[fdNum].Peek();

                        if (selCard.SameSuit(fdCard))
                        {
                            if (selCard.Val == fdCard.Val + 1)
                            {
                                validMove = true;
                            }
                        }
                    }

                    // If the attempted move is valid, remove the Card from wherever it currently
                    // lives and add it to the foundation it belongs in
                    if (validMove)
                    {
                        if (this.sel.Source == 7)
                        {
                            this.discard.Remove(selCard);
                        }
                        else
                        {
                            this.board[this.sel.Source].Remove(selCard);

                            // If column still has cards AND the last card isn't face up, flip it
                            int size = this.board[this.sel.Source].Count;
                            if (size > 0 && !this.board[this.sel.Source][size - 1].Up)
                            {
                                this.board[this.sel.Source][size - 1].Flip();
                            }
                        }

                        this.foundations[fdNum].Push(selCard);
                        this.score += 10;
                    }

                    this.sel.Clear();
                
                }
            }
            else
            {
                // The foundation was clicked, but a selection did not already exist, so
                // select the top card of the current foundation
                Card c = this.foundations[fdNum].Peek();
                List<Card> toAdd = new List<Card> { c };

                int w = GameProperties.CARD_WIDTH;
                int h = GameProperties.CARD_HEIGHT;
                int colSpace = (GameProperties.WINDOW_WIDTH / 7) - w;
                int buf = -(colSpace / 2);

                int xLoc = buf + (colSpace * (fdNum + 4)) + (w * ((fdNum + 4) - 1));
                int yLoc = GameProperties.DECK_YCOR;

                this.sel.Change(toAdd, fdNum + 8, xLoc, yLoc, h);
            }
        }


        /// <summary>
        /// Attempts to score the current selection.  This method assumes it will only
        /// ever get called when the selection is one card (we can't attempt to score more
        /// than one card).  Returns true if the selection was scored, false otherwise.
        /// </summary>
        public bool CanBeScored()
        {
            Card selCard = this.sel.Cards[0];

            for (int i = 0; i < 4; i++)
            {
                if (this.foundations[i].Count > 0)
                {
                    Card fCard = this.foundations[i].Peek();

                    if (selCard.SameSuit(fCard) && selCard.Val == fCard.Val + 1)
                    {
                        if (this.sel.Source == 7)
                        {
                            this.discard.Remove(selCard);
                        }
                        else
                        {
                            this.board[this.sel.Source].Remove(selCard);

                            // If column still has cards AND the last card isn't face up, flip it
                            int size = this.board[this.sel.Source].Count;
                            if (size > 0 && !this.board[this.sel.Source][size - 1].Up)
                            {
                                this.board[this.sel.Source][size - 1].Flip();
                            }
                        }

                        this.foundations[i].Push(selCard);
                        this.sel.Clear();

                        return true;
                    }
                }
                else
                {
                    // if the foundation is empty and we are trying to play
                    // an ace, go ahead and play it in this foundation
                    if (selCard.Val == 1)
                    {
                        if (this.sel.Source == 7)
                        {
                            this.discard.Remove(selCard);
                        }
                        else
                        {
                            this.board[this.sel.Source].Remove(selCard);

                            // If column still has cards AND the last card isn't face up, flip it
                            int size = this.board[this.sel.Source].Count;
                            if (size > 0 && !this.board[this.sel.Source][size - 1].Up)
                            {
                                this.board[this.sel.Source][size - 1].Flip();
                            }
                        }

                        this.foundations[i].Push(selCard);
                        this.sel.Clear();

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the game can be auto-won.  Requires that the deck be empty,
        /// the discard pile be empty, and all cards in play be face-up.
        /// </summary>
        /// <returns>True if the game can be auto completed, False otherwise</returns>
        public bool CanAutoComplete()
        {
            // If there are no cards in the deck or discard pile
            if (this.d.Size() == 0 && this.discard.Count == 0)
            {
                // Quickly check if there are any face-down cards still in play
                for (int i = 0; i < this.board.Count; i++)
                {
                    // If the top card in any column is face down, we can't complete
                    if (this.board[i].Count > 0 && !this.board[i][0].Up)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Auto finishes the game.  
        /// </summary>
        public void AutoComplete()
        {
            while (!GameOver())
            {
                // While the game isn't over, check if the bottom card in each column
                // can be scored.  If it can, score it and count the points
                for (int i = 0; i < this.board.Count; i++)
                {
                    int cardsInCol = this.board[i].Count;

                    if (cardsInCol > 0)
                    {
                        List<Card> toAdd = this.board[i].GetRange(cardsInCol - 1, 1);

                        int w = GameProperties.CARD_WIDTH;
                        int h = GameProperties.CARD_HEIGHT;
                        int sep = GameProperties.TABLE_CARD_SEPARATION;
                        int start = GameProperties.TABLE_START;
                        int colSpace = (GameProperties.WINDOW_WIDTH / 7) - w;
                        int buf = -(colSpace / 2);
                        int colHeight = (cardsInCol - 1) * sep + h;

                        int selX = buf + (colSpace * (i + 1)) + (w * i);
                        int selY = start + (sep * (cardsInCol - 1));

                        this.sel.Change(toAdd, i, selX, selY, h);

                        // Sleep for 0.2 seconds so player can view the animation
                        System.Threading.Thread.Sleep(200);

                        if (CanBeScored())
                        {
                            this.score += 10;
                        }
                        else
                        {
                            this.sel.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the game is over
        /// </summary>
        /// <returns><c>true</c>, if all cards are in a foundation, <c>false</c> otherwise.</returns>
        public bool GameOver()
        {
            for (int i = 0; i < this.foundations.Count; i++)
            {
                Stack<Card> f = this.foundations[i];

                // If the top card of any foundation isn't a king, the game isn't over
                if (f.Count == 0 || f.Peek().Val != 13)
                {
                    return false;
                }
            }

            return true;
        }

        // TODO undo functionality?
    }
}
*/
  