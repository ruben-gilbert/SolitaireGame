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
    // TODO -- UPDATE EVERYTHING TO BE VECTORS
    // TODO -- MAKE BLANK BOX TEXTURE ONCE AND ALLOW COMPONENTS TO ACCESS IT
    // TODO -- START OF GAME DEALING ANIMATION?
    // TODO -- auto-winning game breaks if there is an empty foundation
    public class BackendGame
    {
        private Deck deck;
        private Discard discard;
        private Selection selection;
        private List<Foundation> foundations;
        private List<Tableau> tableaus;
        private List<CardZone> board;
        private List<Tweener> animations;
        private int score;
        private MainGame mainGame;
        private bool isWinnable;

        private Button b;

        public BackendGame(MainGame game)
        {
            this.mainGame = game;
            this.NewGame();
        }

        // -----------------------------------------------------------------------------------------
        // Getters / Setters

        public List<Tweener> Animations
        {
            get { return this.animations; }
        }

        public MainGame Game
        {
            get { return this.mainGame; }
        }

        public bool IsWinnable
        {
            get { return this.isWinnable; }
        }

        public Selection Selection
        {
            get { return this.selection; }
        }

        // -----------------------------------------------------------------------------------------
        // Methods

        public void AnimationAdd(Tweener tween)
        {
            this.animations.Add(tween);
        }

        public void AnimationRemove(Tweener tween)
        {
            this.animations.Remove(tween);
        }

        /// <summary>
        /// Autos scores the top card of some source CardZone
        /// </summary>
        /// <param name="c">The card to be played</param>
        private void AutoPlayCard(Card c)
        {
            foreach (Foundation f in this.foundations)
            {
                // If the foundation is empty and we are trying to play an Ace OR the foundation
                // is not empty and we are playing the next card that belongs in it, then succeed.
                //if ((f.IsEmpty() && src.TopCard().Val == 1) ||
                //    !f.IsEmpty() && src.TopCard().IsSameSuit(f.TopCard()) 
                //    && src.TopCard().Val == f.TopCard().Val + 1)
                if (f.IsEmpty() && c.Val == 1 ||
                    !f.IsEmpty() && c.IsSameSuit(f.TopCard()) && c.Val == f.TopCard().Val + 1)
                {
                    //Tweener tween = new Tweener(this, src, f, 1);
                    Tweener tween = new Tweener(this, c.Source, f, 1);
                    this.AnimationAdd(tween);

                    return;
                }
            }
        }

        /// <summary>
        /// Checks to see if the game is currently in a state that can be auto-completed.
        /// </summary>
        /// <returns><c>true</c>, if game can be auto-won, <c>false</c> otherwise.</returns>
        private bool CanAutoWin()
        {
            // Only bother checking the Tableus if there are no cards in deck or discard
            if (this.deck.IsEmpty() && this.discard.IsEmpty())
            {
                foreach (Tableau t in this.tableaus)
                {
                    foreach (Card c in t.Cards)
                    {
                        // If there is any card that is not face-up, can't safely win game
                        if (!c.IsUp)
                        {
                            return false;
                        }
                    }
                }

                // If we make it through all cards without failing, must be we can win
                return true;
            }

            return false;
        }

        /// <summary>
        /// Draw the game.  Ask each CardZone in the board to draw itself and, if there is a valid
        /// selection, draw it as well.
        /// </summary>
        /// <param name="s">The SpriteBatch object that will handle drawing.</param>
        public void Draw(SpriteBatch s)
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

            // Draw anything that is currently animating
            foreach (Tweener tween in this.animations)
            {
                if (tween.Valid)
                {
                    tween.Draw(s);
                }
            }

            this.b.Draw(s);
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

        /// <summary>
        /// Handles mouse down events for this game.
        /// </summary>
        /// <param name="x">The x coordinate of the click.</param>
        /// <param name="y">The y coordinate of the click.</param>
        public void HandleMouseDown(int x, int y)
        {

            if (this.b.IsClicked(x, y))
            {
                this.b.OnPress();
                return;
            }

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

        /// <summary>
        /// Handles mouse up events for this game.
        /// </summary>
        /// <param name="x">The x coordinate of the click.</param>
        /// <param name="y">The y coordinate of the click.</param>
        public void HandleMouseUp(int x, int y)
        {
            if (this.b.IsClicked(x, y))
            {
                this.b.OnRelease();
            }
            else if (this.b.Pressed)
            {
                this.b.Pressed = false;
            }

            // If the selection isn't empty, it's valid and we should consider playing it
            if (!this.selection.IsEmpty())
            {
                // Check if the move was played into one of the foundations
                foreach (Foundation f in this.foundations)
                {
                    if (f.IsDroppedOn(x, y) && this.selection.IsValidMove(f))
                    {
                        this.selection.CompleteMove(f);
                        return;
                    }
                }

                // Check if the move was played into one of the tableaus
                foreach (Tableau t in this.tableaus)
                {
                    if (t.IsDroppedOn(x, y) && this.selection.IsValidMove(t))
                    {
                        this.selection.CompleteMove(t);
                        return;
                    }
                }

                // If invalid move, return Cards to source Zone
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

        /// <summary>
        /// Handles right-click events for this game.
        /// </summary>
        public void HandleRightClick(int x, int y)
        {
            // If the game is winnable (and isn't already being auto-won), set it for completion
            if (!this.isWinnable && this.CanAutoWin())
            {
                this.isWinnable = true;
            }
            // Otherwise, if there are no animations being processed, try to auto play a card
            else if (this.animations.Count == 0)
            {
                if (!this.discard.IsEmpty() &&
                this.discard.TopCard().IsClicked(x, y, GameProperties.CARD_WIDTH,
                                                 GameProperties.CARD_HEIGHT))
                {
                    this.AutoPlayCard(this.discard.TopCard());
                    return;
                }

                foreach (Tableau t in this.tableaus)
                {
                    if (!t.IsEmpty() && t.TopCard().IsClicked(x, y, GameProperties.CARD_WIDTH,
                                                              GameProperties.CARD_HEIGHT))
                    {
                        this.AutoPlayCard(t.TopCard());
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the next step in auto-completing the current game.
        /// </summary>
        /// <returns>A 2-item Tuple containing the source Tableau and the 
        /// target Foundation.</returns>
        public Tuple<Tableau, Foundation> NextAutoWinStep()
        {
            // Find the foundation with the least cards
            Foundation target = this.foundations[0];
            for (int i = 1; i < this.foundations.Count; i++)
            {
                if (this.foundations[i].Size() < target.Size())
                {
                    target = this.foundations[i];
                }
            }

            // Search the Tableaus for the card that belongs on the smallest foundation
            foreach (Tableau source in this.tableaus)
            {
                if (!source.IsEmpty()
                    && source.TopCard().IsSameSuit(target.TopCard())
                    && source.TopCard().Val == target.TopCard().Val + 1)
                {
                    return new Tuple<Tableau, Foundation>(source, target);
                }
            }

            // Shouldn't ever happen
            return new Tuple<Tableau, Foundation>(null, null);
        }

        /// <summary>
        /// Creates a new game within this BackendGame object.
        /// </summary>
        public void NewGame()
        {
            // TODO add buttons and menus, etc?
            this.b = new Button(this, 
                                "New Game", 
                                new Vector2(GameProperties.WINDOW_WIDTH / 2 - 100, 50));
            this.b.SetAction(this.NewGame);
            this.b.LoadFont("Button"); 
            
            // TODO make new font for buttons (smaller)
            // TODO add button popup when game is winnable?
            // TODO undo/redo functionality?
            // TODO resolution button/dropdown?
            // TODO cards per deal dropdown?
            // TODO card color drop down?


            this.board = new List<CardZone>();
            this.deck = new Deck(this,
                                GameProperties.DECK_XCOR,
                                GameProperties.DECK_YCOR,
                                1,
                                1);
            this.deck.Shuffle();
            this.board.Add(this.deck);

            this.discard = new Discard(this,
                                GameProperties.DISCARD_XCOR,
                                GameProperties.DISCARD_YCOR,
                                GameProperties.DISCARD_SEPARATION,
                                0);
            this.board.Add(this.discard);

            foundations = new List<Foundation>();
            int foundX = GameProperties.WINDOW_WIDTH / 2;
            int foundSpace = (foundX - (GameProperties.CARD_WIDTH * 4)) / 4;
            for (int i = 0; i < 4; i++)
            {
                Foundation f = new Foundation(this,
                                    foundX,
                                    GameProperties.FOUNDATION_YCOR,
                                    0,
                                    0);
                this.foundations.Add(f);
                this.board.Add(f);
                foundX += foundSpace + GameProperties.CARD_WIDTH;
            }

            tableaus = new List<Tableau>();

            int tableSpace = (GameProperties.WINDOW_WIDTH - (GameProperties.CARD_WIDTH * 7)) / 8;
            int tabX = tableSpace;
            for (int j = 1; j < 8; j++)
            {
                Tableau t = new Tableau(this,
                                    tabX,
                                    GameProperties.TABLE_START,
                                    0,
                                    GameProperties.TABLE_CARD_SEPARATION);
                this.deck.MoveCardsToZone(j, t);
                this.tableaus.Add(t);
                this.board.Add(t);
                tabX += tableSpace + GameProperties.CARD_WIDTH;
            }

            this.selection = new Selection(this, 0, 0, 0, 0);

            this.animations = new List<Tweener>();

            this.score = 0;
            this.isWinnable = false;
        }

        /// <summary>
        /// Public facing method that allows this Game's Selection to update it's position
        /// based on the (x, y) coordinates of the mouse.
        /// </summary>
        /// <param name="x">The mouse's x coordinate.</param>
        /// <param name="y">The mouse's y coordinate.</param>
        public void UpdateSelection(int x, int y)
        {
            this.selection.UpdatePosition(x, y);
        }
    }
}

