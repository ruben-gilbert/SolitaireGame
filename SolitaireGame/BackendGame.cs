// BackendGame.cs
// Author: Ruben Gilbert
// 2019

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolitaireGame
{
    // TODO -- UPDATE EVERYTHING TO BE VECTORS
    // TODO -- MAKE BLANK BOX TEXTURE ONCE AND ALLOW COMPONENTS TO ACCESS IT
    // TODO -- START OF GAME DEALING ANIMATION?
    // TODO -- auto-winning game breaks if there is an empty foundation
    public class BackendGame
    {
        #region Members
        private Button m_button;
        private Deck m_deck;
        private Discard m_discard;
        private List<Foundation> m_foundations;
        private List<Tableau> m_tableaus;
        private List<CardZone> m_board;
        #endregion

        #region Properties
        public List<Tweener> Animations { get; private set; }

        public MainGame Game { get; }

        public bool IsWinnable { get; private set; }

        public Selection Selection { get; private set; }
        #endregion

        public BackendGame(MainGame game)
        {
            Game = game;
            NewGame();
        }

        #region Methods
        public void AnimationAdd(Tweener tween)
        {
            Animations.Add(tween);
        }

        public void AnimationRemove(Tweener tween)
        {
            Animations.Remove(tween);
        }

        /// <summary>
        /// Autos scores the top card of some source CardZone
        /// </summary>
        /// <param name="c">The card to be played</param>
        private void AutoPlayCard(Card c)
        {
            foreach (Foundation f in m_foundations)
            {
                // If the foundation is empty and we are trying to play an Ace OR the foundation
                // is not empty and we are playing the next card that belongs in it, then succeed.
                //if ((f.IsEmpty() && src.TopCard().Val == 1) ||
                //    !f.IsEmpty() && src.TopCard().IsSameSuit(f.TopCard()) 
                //    && src.TopCard().Val == f.TopCard().Val + 1)
                if (f.IsEmpty() && c.Value == 1 ||
                    !f.IsEmpty() && c.IsSameSuit(f.TopCard()) && c.Value == f.TopCard().Value + 1)
                {
                    //Tweener tween = new Tweener(this, src, f, 1);
                    Tweener tween = new Tweener(this, c.Source, f, 1);
                    AnimationAdd(tween);

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
            if (m_deck.IsEmpty() && m_discard.IsEmpty())
            {
                foreach (Tableau t in m_tableaus)
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
            foreach (CardZone cz in m_board)
            {
                cz.Draw(s);
            }

            // Only draw the selection if there is something in it 
            if (!Selection.IsEmpty())
            {
                Selection.Draw(s);
            }

            // Draw anything that is currently animating
            foreach (Tweener tween in Animations)
            {
                if (tween.Valid)
                {
                    tween.Draw(s);
                }
            }

            m_button.Draw(s);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="s">SpriteBatch for drawing</param>
        /// <param name="start">The starting coordinate of the line</param>
        /// <param name="end">The ending coordinate of the line</param>
        /// <param name="thickness">How thick the line should be</param>
        public void DrawLine(SpriteBatch s, Point start, Point end, int thickness)
        {
            Vector2 edge = end.ToVector2() - start.ToVector2();
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            s.Draw(Game.BlankBox,
                   new Rectangle(start.X, start.Y, (int)edge.Length(), thickness),
                   null,
                   Color.Black,
                   angle,
                   new Vector2(0, 0),
                   SpriteEffects.None,
                   0);
        }

        /// <summary>
        /// Checks if the game is over.
        /// </summary>
        /// <returns><c>true</c>, if all Foundations are full, <c>false</c> otherwise.</returns>
        public bool GameOver()
        {
            foreach (Foundation f in m_foundations) {
                if (f.Count() != 13)
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

            if (m_button.IsClicked(x, y))
            {
                m_button.OnPress();
                return;
            }

            CardZone clicked = null;
            foreach (CardZone zone in m_board)
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
                        Selection.SetSourceZone(clicked);
                        Selection.SetRelativeOffsets(numToMove, x, y);
                        clicked.MoveCardsToZone(numToMove, Selection);
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
            if (m_button.IsClicked(x, y))
            {
                m_button.OnRelease();
            }
            else if (m_button.Pressed)
            {
                m_button.Pressed = false;
            }

            // If the selection isn't empty, it's valid and we should consider playing it
            if (!Selection.IsEmpty())
            {
                // Check if the move was played into one of the foundations
                foreach (Foundation f in m_foundations)
                {
                    if (f.IsDroppedOn(x, y) && Selection.IsValidMove(f))
                    {
                        Selection.CompleteMove(f);
                        return;
                    }
                }

                // Check if the move was played into one of the tableaus
                foreach (Tableau t in m_tableaus)
                {
                    if (t.IsDroppedOn(x, y) && Selection.IsValidMove(t))
                    {
                        Selection.CompleteMove(t);
                        return;
                    }
                }

                // If invalid move, return Cards to source Zone
                Selection.ReturnToSource();
            }
            else
            {
                // If there's no selection to play, we only need to consider if the deck was clicked
                if (m_deck.IsSelected)
                {
                    if (m_deck.IsClicked(x, y))
                    {
                        if (m_deck.IsEmpty())
                        {
                            m_discard.MoveCardsToZone(m_discard.Count(), m_deck);
                        }
                        else
                        {
                            m_deck.MoveCardsToZone(Properties.DealMode, m_discard);
                        }
                    }

                    m_deck.Deselect();
                }
            }
        }

        /// <summary>
        /// Handles right-click events for this game.
        /// </summary>
        public void HandleRightClick(int x, int y)
        {
            // If the game is winnable (and isn't already being auto-won), set it for completion
            if (!IsWinnable && CanAutoWin())
            {
                IsWinnable = true;
            }
            // Otherwise, if there are no animations being processed, try to auto play a card
            else if (Animations.Count == 0)
            {
                if (!m_discard.IsEmpty() &&
                m_discard.TopCard().IsClicked(x, y, Card.Width, Card.Height))
                {
                    AutoPlayCard(m_discard.TopCard());
                    return;
                }

                foreach (Tableau t in m_tableaus)
                {
                    if (!t.IsEmpty() && t.TopCard().IsClicked(x, y, Card.Width, Card.Height))
                    {
                        AutoPlayCard(t.TopCard());
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
            Foundation target = m_foundations[0];
            for (int i = 1; i < m_foundations.Count; i++)
            {
                if (m_foundations[i].Count() < target.Count())
                {
                    target = m_foundations[i];
                }
            }

            // Search the Tableaus for the card that belongs on the smallest foundation
            foreach (Tableau source in m_tableaus)
            {
                if (!source.IsEmpty()
                    && source.TopCard().IsSameSuit(target.TopCard())
                    && source.TopCard().Value == target.TopCard().Value + 1)
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
            m_button = new Button(this, "New Game", new Point(Properties.WindowWidth / 2 - 100, 50));
            m_button.SetAction(NewGame);
            m_button.LoadFont("Button"); 
            
            // TODO make new font for buttons (smaller)
            // TODO add button popup when game is winnable?
            // TODO undo/redo functionality?
            // TODO resolution button/dropdown?
            // TODO cards per deal dropdown?
            // TODO card color drop down?


            m_board = new List<CardZone>();
            m_deck = new Deck(this, new Point(Deck.X, Deck.Y), 1, 1);
            m_deck.Shuffle();
            m_board.Add(m_deck);

            m_discard = new Discard(this, new Point(Discard.X, Discard.Y), Discard.Separation, 0);
            m_board.Add(m_discard);

            m_foundations = new List<Foundation>();
            int foundX = Properties.WindowWidth / 2;
            int foundSpace = (foundX - (Card.Width * 4)) / 4;
            for (int i = 0; i < 4; i++)
            {
                Foundation f = new Foundation(this, new Point(foundX, Foundation.Y), 0, 0);
                m_foundations.Add(f);
                m_board.Add(f);
                foundX += foundSpace + Card.Width;
            }

            m_tableaus = new List<Tableau>();

            int tableSpace = (Properties.WindowWidth - (Card.Width * 7)) / 8;
            int tabX = tableSpace;
            for (int j = 1; j < 8; j++)
            {
                Tableau t = new Tableau(this, new Point(tabX, Properties.TableStart), 0, Properties.TableCardSeparation);
                m_deck.MoveCardsToZone(j, t);
                m_tableaus.Add(t);
                m_board.Add(t);
                tabX += tableSpace + Card.Width;
            }

            Selection = new Selection(this, new Point(0, 0), 0, 0);

            Animations = new List<Tweener>();

            IsWinnable = false;
        }

        /// <summary>
        /// Public facing method that allows this Game's Selection to update it's position
        /// based on the (x, y) coordinates of the mouse.
        /// </summary>
        /// <param name="x">The mouse's x coordinate.</param>
        /// <param name="y">The mouse's y coordinate.</param>
        public void UpdateSelection(int x, int y)
        {
            Selection.UpdatePosition(x, y);
        }
        #endregion
    }
}

