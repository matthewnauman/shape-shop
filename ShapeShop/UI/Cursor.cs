using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using ShapeShopData.Models;
using ShapeShopData.Util;
using System;

namespace ShapeShop.UI
{
    public enum CursorState
    {
        Free,
        Grab,
        Reset,
        Hidden,
    }

    public class Cursor
    {
        private readonly Point cursorSize = new Point(64, 64);   // magic number

        private Texture2D cursorTexture;
        private bool isMoving = false;

        public static readonly int snapPadding = 2; //12; // 5, 16
        public static readonly float SPEED = 12.5f;
        public static readonly Vector2 SPEEDV = new Vector2(SPEED, -SPEED);

        public Vector2 ScreenPosition = PuzzleEngine.ViewportCenter;

        public Direction Direction = Direction.South;

        private CursorState cursorMode;
        public CursorState CursorMode
        {
            get { return cursorMode; }
            set { cursorMode = value; }
        }

        public Cursor() 
        { 
            CursorMode = CursorState.Free;
        }

        public void LoadContent(ContentManager content)
        {
            cursorTexture = content.Load<Texture2D>(@"Textures\cursor");
            ScreenPosition = PuzzleEngine.ViewportCenter;
        }


        private Vector2 boundMovement(Vector2 movement)
        {
            if (!PuzzleEngine.IsAShapeSelected)
            {
                Vector2 newScreenPosition = ScreenPosition + movement;
                if (newScreenPosition.X < PuzzleEngine.Viewport.X + MainGameScreen.MARGIN_LEFTRIGHT)
                {
                    movement.X += ((PuzzleEngine.Viewport.X + MainGameScreen.MARGIN_LEFTRIGHT) - newScreenPosition.X);
                }
                else if (newScreenPosition.X > ShapeShop.PREFERRED_WIDTH - cursorTexture.Width - MainGameScreen.MARGIN_LEFTRIGHT)
                {
                    movement.X -= (newScreenPosition.X - (ShapeShop.PREFERRED_WIDTH - cursorTexture.Width - MainGameScreen.MARGIN_LEFTRIGHT));
                }

                if (newScreenPosition.Y < PuzzleEngine.Viewport.Y)
                {
                    movement.Y += ((PuzzleEngine.Viewport.Y) - newScreenPosition.Y);
                }
                else if (newScreenPosition.Y > ShapeShop.PREFERRED_HEIGHT - cursorTexture.Height)
                {
                    movement.Y -= (newScreenPosition.Y - (ShapeShop.PREFERRED_HEIGHT - cursorTexture.Height));
                }
            }
            else  // a shape is selected, bound screen on shape position and shape origin instead of cursor position and offset
            {
                Vector2 newScreenPosition = PuzzleEngine.SelectedShape.ScreenPosition + movement;
                if (newScreenPosition.X < PuzzleEngine.Viewport.X + PuzzleEngine.SelectedShape.ShapeOrigin.X + MainGameScreen.MARGIN_LEFTRIGHT)
                {
                    movement.X += ((PuzzleEngine.Viewport.X + PuzzleEngine.SelectedShape.ShapeOrigin.X + MainGameScreen.MARGIN_LEFTRIGHT) - newScreenPosition.X);
                }
                else if (newScreenPosition.X > ShapeShop.PREFERRED_WIDTH - PuzzleEngine.SelectedShape.ShapeOrigin.X - MainGameScreen.MARGIN_LEFTRIGHT)
                {
                    movement.X -= (newScreenPosition.X - (ShapeShop.PREFERRED_WIDTH - PuzzleEngine.SelectedShape.ShapeOrigin.X - MainGameScreen.MARGIN_LEFTRIGHT));
                }

                if (newScreenPosition.Y < PuzzleEngine.Viewport.Y + PuzzleEngine.SelectedShape.ShapeOrigin.Y)
                {
                    movement.Y += ((PuzzleEngine.Viewport.Y + PuzzleEngine.SelectedShape.ShapeOrigin.Y) - newScreenPosition.Y);
                }
                else if (newScreenPosition.Y > ShapeShop.PREFERRED_HEIGHT - PuzzleEngine.SelectedShape.ShapeOrigin.Y)
                {
                    movement.Y -= (newScreenPosition.Y - (ShapeShop.PREFERRED_HEIGHT - PuzzleEngine.SelectedShape.ShapeOrigin.Y));
                }
            }

            return movement;
        }

        private void move(Vector2 movement)
        {
            isMoving = (movement != Vector2.Zero);

            // if moving, update screen positions and direction
            if (isMoving)
            {
                // bind movement to edges of puzzle background
                movement = boundMovement(movement);

                if (PuzzleEngine.IsCursorModeActive && !PuzzleEngine.IsAShapeSelected)
                {
                    ScreenPosition += movement;
                }

                // if a shape is selected move it along with the cursor
                if (PuzzleEngine.IsAShapeSelected)
                {
                    if (PuzzleEngine.SelectedShape.State != Shape.ShapeState.TransitionIn &&
                        PuzzleEngine.SelectedShape.State != Shape.ShapeState.TransitionOut)
                    {
                        ScreenPosition += movement;
                        PuzzleEngine.SelectedShape.ScreenPosition += movement;
                        PuzzleEngine.SelectedShape.IsSnapped = false;
                    }
                }
 
                Direction = CalculateDirection(movement);
            }
            else
            {
                // if the shape is not moving, try to snap to closest tile
                if (PuzzleEngine.IsAShapeSelected && !PuzzleEngine.SelectedShape.IsSnapped)
                {
                    snapToTile();
                }
            }
        }

        private void snapToTile()
        {
            bool isXSnappable = false;
            bool isYSnappable = false;

            // snap to tile if its over puzzle grid and within snap padding of x || y, or x && y
            if (PuzzleEngine.IsSelectedShapeOverPuzzle && 
                PuzzleEngine.CurrentPuzzle.CanSnapShape(PuzzleEngine.SnapTilePosition, PuzzleEngine.SelectedShape))
            {
                Vector2 snapToDelta = Vector2.Zero;

                if (PuzzleEngine.PuzzleTileOffset.X > snapPadding)
                {
                    snapToDelta.X += (PuzzleEngine.CurrentPuzzle.TileSize.X / 2) - PuzzleEngine.PuzzleTileOffset.X;
                    isXSnappable = true;
                }
                else if (PuzzleEngine.PuzzleTileOffset.X < -snapPadding)
                {
                    snapToDelta.X -= (PuzzleEngine.CurrentPuzzle.TileSize.X / 2) + PuzzleEngine.PuzzleTileOffset.X;
                    isXSnappable = true;
                }

                if (PuzzleEngine.PuzzleTileOffset.Y > snapPadding)
                {
                    snapToDelta.Y += (PuzzleEngine.CurrentPuzzle.TileSize.Y / 2) - PuzzleEngine.PuzzleTileOffset.Y;
                    isYSnappable = true;
                }
                else if (PuzzleEngine.PuzzleTileOffset.Y < -snapPadding)
                {
                    snapToDelta.Y -= (PuzzleEngine.CurrentPuzzle.TileSize.Y / 2) + PuzzleEngine.PuzzleTileOffset.Y;
                    isYSnappable = true;
                }

                if (isXSnappable && isYSnappable)
                {
                    PuzzleEngine.SelectedShape.ScreenPosition += snapToDelta;
                    PuzzleEngine.Cursor.ScreenPosition += snapToDelta;
                    PuzzleEngine.SelectedShape.IsSnapped = true;

                    if (PuzzleEngine.SelectedShape.IsFlipping || PuzzleEngine.SelectedShape.IsRotating)
                    {
                        if (PuzzleEngine.SelectedShape.IsFlipping)
                        {
                            PuzzleEngine.SelectedShape.IsSnapCueOnFlipFinish = true;
                        }
                        else
                        { // (PuzzleEngine.SelectedShape.IsRotating)
                            PuzzleEngine.SelectedShape.IsSnapCueOnRotateFinish = true;
                        }
                    }
                    else
                    {
                        AudioManager.PlayCue("snap");
                    }
                }
            }
            else
            {
                PuzzleEngine.SelectedShape.IsSnapCueOnFlipFinish = false;
                PuzzleEngine.SelectedShape.IsSnapCueOnRotateFinish = false;
            }

        }

        public void HandleInput()
        {
            if (!PuzzleEngine.CurrentPuzzle.IsCleared)
            {
                handleCursorMovement();
            }
        }

        public void Update(GameTime gameTime)
        {
        }

        private void handleCursorMovement()
        {
            // handle user cursor movement
            Vector2 cursorMovement = Vector2.Zero;
            cursorMovement = updateCursorMovement();
            move(cursorMovement);
        }

        private Vector2 updateCursorMovement()
        {
            Vector2 desiredMovement = Vector2.Zero;

            if (InputManager.IsActionPressed(InputManager.Action.MovePuzzleCursor))
            {
                Vector2 cursorSpeed = Vector2.Zero;

                if (PuzzleEngine.IsAShapeSelected)
                {
                    cursorSpeed = InputManager.CurrentGamePadState.ThumbSticks.Left * (SPEEDV / 2);
                }
                else
                {
                    cursorSpeed = InputManager.CurrentGamePadState.ThumbSticks.Left * SPEEDV;
                }

                desiredMovement += cursorSpeed;

                if (InputManager.IsActionPressed(InputManager.Action.MoveCursorDown))
                {
                    desiredMovement.Y += SPEED;
                }
                if (InputManager.IsActionPressed(InputManager.Action.MoveCursorUp))
                {
                    desiredMovement.Y -= SPEED;
                }
                if (InputManager.IsActionPressed(InputManager.Action.MoveCursorLeft))
                {
                    desiredMovement.X -= SPEED;
                }
                if (InputManager.IsActionPressed(InputManager.Action.MoveCursorRight))
                {
                    desiredMovement.X += SPEED;
                }
            }

            return desiredMovement;
        }

        /// Draw the sprite at the given position.
        public void Draw(SpriteBatch spriteBatch)
        {

            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }

            if (cursorTexture != null && !PuzzleEngine.IsAShapeSelected)
            {
                spriteBatch.Draw(cursorTexture,
                                 ScreenPosition,
                                 Color.White);
            }

//            drawDebugPoints(spriteBatch);
        }

        private void drawDebugPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Session.ScreenManager.BlankTexture,
                             ScreenPosition,
                             null,
                             Color.Fuchsia,
                             0,
                             Vector2.Zero,
                             2,
                             SpriteEffects.None,
                             0);

            if (PuzzleEngine.SelectedShape != null)
            {
                spriteBatch.Draw(Session.ScreenManager.BlankTexture,
                                 PuzzleEngine.SelectedShape.ScreenPosition,
                                 null,
                                 Color.Cyan,
                                 0,
                                 Vector2.Zero,
                                 2,
                                 SpriteEffects.None,
                                 0);
            }
        }

        /// location the direction based on the given movement vector.
        /// <param name="vector">The vector that the player is moving.</param>
        /// <returns>The calculated direction.</returns>
        public static Direction CalculateDirection(Vector2 vector)
        {
            if (vector.X > 0)
            {
                if (vector.Y > 0)
                {
                    return Direction.SouthEast;
                }
                else if (vector.Y < 0)
                {
                    return Direction.NorthEast;
                }
                else // y == 0
                {
                    return Direction.East;
                }
            }
            else if (vector.X < 0)
            {
                if (vector.Y > 0)
                {
                    return Direction.SouthWest;
                }
                else if (vector.Y < 0)
                {
                    return Direction.NorthWest;
                }
                else // y == 0
                {
                    return Direction.West;
                }
            }
            else // x == 0
            {
                if (vector.Y > 0)
                {
                    return Direction.South;
                }
                else if (vector.Y < 0)
                {
                    return Direction.North;
                }
            }
            // x == 0 && y == 0, so... south?
            return Direction.South;
        }

        public String GetDebugInformation()
        {
            return "Cursor Screen Pos : " + ScreenPosition.ToString() +
                   "\nPuzzle Tile Pos : " + PuzzleEngine.CursorTilePosition.ToString() +
                   "\nDirection : " + Direction.ToString() +
                   "\nCursor Mode : " + PuzzleEngine.Cursor.CursorMode;
        }

        public override String ToString()
        {
            return "Cursor Screen Pos : " + ScreenPosition.ToString() +
                   "\nPuzzle Tile Pos : " + PuzzleEngine.CursorTilePosition.ToString() +
                   "\nDirection : " + Direction.ToString() +
                   "\nCursor Mode : " + PuzzleEngine.Cursor.CursorMode;
        }

    }
}
