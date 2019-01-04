using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ShapeShopData.Models
{
    /// a game piece on the board
    public class Shape : ICloneable
    {
        public enum ShapeState
        {
            Waiting,
            TransitionIn,
            Selected,
            Dropped,
            TransitionOut,
            TransitionOutForHint,
            WaitOnHint,
            TransitionInAfterHint,
        }

        public enum TransitionType
        {
            Fade,
//            TraveHome,
//            Both,
//            None,
        }

        public static readonly float[] ROTATIONS = { MathHelper.ToRadians(0), 
                                                     MathHelper.ToRadians(90), 
                                                     MathHelper.ToRadians(180), 
                                                     MathHelper.ToRadians(270)
                                                   };

        public static readonly float RotationSpeed = .1f;
        public static readonly float FlipSpeed = .1f;
        public static readonly float AutoMovementSpeed = 20;
        public static readonly float AutoRotationSpeed = .2f;
        public static readonly float AutoScaleMax = 1f;
        public static readonly float AutoScaleMin = 0f;
        public static readonly float AutoFadeDelta_Default = .075f;
        public static readonly float AutoFadeDelta_Hint = .075f; //.075f;
        public static readonly float MinAlpha_Hint = 0f;

        //        public static readonly Vector2 AutoScaleDelta = new Vector2(.02f, .02f);

        private bool isFlipMidway = false;

        private int currentRotationIdx = 0;
        [ContentSerializerIgnore]
        public int CurrentRotationIdx
        {
            get { return currentRotationIdx; }
        }

        private int destinationRotationIdx = 0;
        private float rotation = 0f;
        private float alpha = 0f;
        private float shapeResetDistance = 0f;
        private Vector2 scale = Vector2.One;

        private Vector2 horizontalFlipScale = Vector2.One;
        private Vector2 verticalFlipScale = Vector2.One;

        /// The name of the object.
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int key;
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        private Vector2 screenPosition;
        [ContentSerializerIgnore]
        public Vector2 ScreenPosition
        {
            get { return screenPosition; }
            set { screenPosition = value; }
        }

        private Vector2 cursorReturnOffset;
        [ContentSerializerIgnore]
        public Vector2 CursorReturnOffset
        {
            get { return cursorReturnOffset; }
            set { cursorReturnOffset = value; }
        }

        private Vector2 baseOrigin;
        [ContentSerializerIgnore]
        public Vector2 BaseOrigin
        {
            get { return baseOrigin; }
            set { baseOrigin = value; }
        }

        private Vector2 shapeOrigin;
        [ContentSerializerIgnore]
        public Vector2 ShapeOrigin
        {
            get { return shapeOrigin; }
            set { shapeOrigin = value; }
        }

        private bool isValid = false;
        [ContentSerializerIgnore]
        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        private bool isDrawHighlighted = false;
        [ContentSerializerIgnore]
        public bool IsDrawHighlighted
        {
            get { return isDrawHighlighted; }
            set { isDrawHighlighted = value; }
        }

        private bool isReadyToUnsnap = false;
        [ContentSerializerIgnore]
        public bool IsReadyToUnsnap
        {
            get { return isReadyToUnsnap; }
            set { isReadyToUnsnap = value; }
        }

        private bool isSnapCueOnRotateFinish = false;
        [ContentSerializerIgnore]
        public bool IsSnapCueOnRotateFinish
        {
            get { return isSnapCueOnRotateFinish; }
            set { isSnapCueOnRotateFinish = value; }
        }

        private bool isSnapCueOnFlipFinish = false;
        [ContentSerializerIgnore]
        public bool IsSnapCueOnFlipFinish
        {
            get { return isSnapCueOnFlipFinish; }
            set { isSnapCueOnFlipFinish = value; }
        }

        private bool isPlaySnapCue = false;
        [ContentSerializerIgnore]
        public bool IsPlaySnapCue
        {
            get { return isPlaySnapCue; }
            set { isPlaySnapCue = value; }
        }

        private TransitionType transition = TransitionType.Fade;
        [ContentSerializerIgnore]
        public TransitionType Transition
        {
            get { return transition; }
            set { transition = value; }
        }
        
        private bool isRotatingCW = false;
        private bool isRotatingCCW = false;
        public bool IsRotating
        {
            get { return isRotatingCW || isRotatingCCW; }
        }

        private bool isFlippingHorizontal = false;
        public bool IsFlippingHorizontal
        {
            get { return isFlippingHorizontal; }
        }

        private bool isFlippingVertical = false;
        public bool IsFlippingVertical
        {
            get { return isFlippingVertical; }
        }

        public bool IsFlipping
        {
            get { return isFlippingHorizontal || isFlippingVertical; }
        }

        // for sprite effects draw
        private bool isFlippedHorizontal = false;
        [ContentSerializerIgnore]
        public bool IsFlippedHorizontal
        {
            get { return isFlippedHorizontal; }
        }

        private bool isFlippedVertical = false;
        [ContentSerializerIgnore]
        public bool IsFlippedVertical
        {
            get { return isFlippedVertical; }
        }

        private string textureContentName;
        public string TextureContentName
        {
            get { return textureContentName; }
            set { textureContentName = value; }
        }

        private string musicCueName;
        public string MusicCueName
        {
            get { return musicCueName; }
            set { musicCueName = value; }
        }

        public string MusicCueName_Loud
        {
            get { return musicCueName+"L"; }
        }

        private Texture2D texture;
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return texture; }
        }

        private Texture2D textureHL;
        [ContentSerializerIgnore]
        public Texture2D TextureHL
        {
            get { return textureHL; }
        }

        /// The dimensions of the map, in tiles.
        private Point baseMatrixDimensions;
        public Point BaseMatrixDimensions
        {
            get { return baseMatrixDimensions; }
            set { baseMatrixDimensions = value; }
        }

        /// The dimensions of the map, in tiles.
        private Point currentMatrixDimensions;
        [ContentSerializerIgnore]
        public Point CurrentMatrixDimensions
        {
            get { return currentMatrixDimensions; }
            set { currentMatrixDimensions = value; }
        }

        private Point tileSize;
        public Point TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }

        private int[][] baseMatrix;
        public int[][] BaseMatrix
        {
            get { return baseMatrix; }
            set { baseMatrix = value; }
        }

        private int[][] currentMatrix;
        [ContentSerializerIgnore]
        public int[][] CurrentMatrix
        {
            get { return currentMatrix; }
            set { currentMatrix = value; }
        }

        private Vector2 baseScreenPosition;
        public Vector2 BaseScreenPosition
        {
            get { return baseScreenPosition; }
            set { baseScreenPosition = value; }
        }

        /// The state of this character.
        private ShapeState state = ShapeState.Waiting;
        [ContentSerializerIgnore]
        public ShapeState State
        {
            get { return state; }
            set { state = value; }
        }

        private float timeIdle = 0f;
        [ContentSerializerIgnore]
        public float TimeIdle
        {
            get { return timeIdle; }
            set { timeIdle = value; }
        }

        private bool isSnapped = false;
        [ContentSerializerIgnore]
        public bool IsSnapped
        {
            get { return isSnapped; }
            set { isSnapped = value; }
        }

        public bool IsTransitioning
        {
            get 
            {
                return (State == ShapeState.TransitionOut ||
                        State == ShapeState.TransitionIn ||
                        State == ShapeState.TransitionOutForHint ||
                        State == ShapeState.TransitionInAfterHint);
            }
        }

        public Vector2 Shape00Position
        {
            get { return (ScreenPosition - ShapeOrigin); }
        }

        public Rectangle DestRect
        {
            get
            {
                return new Rectangle((int)(ScreenPosition.X - shapeOrigin.X),
                                     (int)(ScreenPosition.Y - shapeOrigin.Y),
                                     CurrentMatrixDimensions.X * TileSize.X,
                                     CurrentMatrixDimensions.Y * TileSize.Y);
            }
        }

        private bool hasBroadFocus(Vector2 cursorPosition)
        {
            return DestRect.Contains(new Point((int)cursorPosition.X, (int)cursorPosition.Y));
        }

        private bool hasNarrowFocus(Vector2 cursorPosition)
        {
            Vector2 diff = cursorPosition - Shape00Position;
            Point dp = new Point((int)(diff.X / TileSize.X), (int)(diff.Y / TileSize.Y));
            return (CurrentMatrix[dp.Y][dp.X] == 1);
        }

        public bool HasFocus(Vector2 cursorPosition)
        {
            if (hasBroadFocus(cursorPosition) && hasNarrowFocus(cursorPosition))
            {
                return true;
            }
            return false;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(System.IO.Path.Combine(@"Textures\Shapes", textureContentName));
            textureHL = content.Load<Texture2D>(System.IO.Path.Combine(@"Textures\Shapes", textureContentName + "HL"));
            currentMatrixDimensions = baseMatrixDimensions;
            currentMatrix = baseMatrix.Clone() as int[][];
            baseOrigin = new Vector2(currentMatrixDimensions.X * tileSize.X / 2,
                                     currentMatrixDimensions.Y * tileSize.Y / 2);
            shapeOrigin = baseOrigin;
            state = ShapeState.Waiting;
        }

        // reset a shape in use
        public void Reset()
        {
            // reset position to base and change state
            ScreenPosition = BaseScreenPosition;
            CurrentMatrixDimensions = BaseMatrixDimensions;
            ShapeOrigin = new Vector2(CurrentMatrixDimensions.X * TileSize.X / 2,
                                      CurrentMatrixDimensions.Y * TileSize.Y / 2);
            CurrentMatrix = BaseMatrix.Clone() as int[][];
            currentRotationIdx = 0;
            cursorReturnOffset = Vector2.Zero;
            destinationRotationIdx = 0;
            isFlippedHorizontal = false;
            isFlippedVertical = false;
            isFlippingHorizontal = false;
            isFlippingVertical = false;
            isFlipMidway = false;
            isRotatingCW = false;
            isRotatingCCW = false;
            isValid = false;
            isSnapped = false;
            rotation = 0f;
            scale = Vector2.One;
            State = ShapeState.Waiting;
            alpha = 0f;
            timeIdle = 0f;
            isDrawHighlighted = false;
        }

        private void updateRotation(GameTime gameTime)
        {
            if (isRotatingCW)
            {
                if (rotation < ROTATIONS[destinationRotationIdx])
                {
                    rotation += RotationSpeed;
                }
                else
                {
                    rotation = ROTATIONS[destinationRotationIdx];
                    isRotatingCW = false;
                    currentRotationIdx = destinationRotationIdx;
                    if (isSnapCueOnRotateFinish)
                    {
                        isSnapCueOnRotateFinish = false;
                        isPlaySnapCue = true;
                    }
                }
            }
            else if (isRotatingCCW)
            {
                if (rotation > ROTATIONS[destinationRotationIdx])
                {
                    rotation -= RotationSpeed;
                }
                else
                {
                    rotation = ROTATIONS[destinationRotationIdx];
                    isRotatingCCW = false;
                    currentRotationIdx = destinationRotationIdx;
                    if (isSnapCueOnRotateFinish)
                    {
                        isSnapCueOnRotateFinish = false;
                        isPlaySnapCue = true;
                    }
                }
            }
        }

        private void updateFlip(GameTime gameTime)
        {
            if (isFlippingHorizontal)
            {
                if (!isFlipMidway)
                {
                    horizontalFlipScale.X -= FlipSpeed;

                    if (horizontalFlipScale.X <= 0)
                    {
                        horizontalFlipScale.X = FlipSpeed;
                        flipMatrix(true);
                        isFlipMidway = true;
                        IsReadyToUnsnap = true;
                    }
                }
                else
                {
                    horizontalFlipScale.X += FlipSpeed;

                    if (horizontalFlipScale.X >= 1)
                    {
                        horizontalFlipScale.X = 1;
                        isFlippingHorizontal = false;
                        isFlipMidway = false;
                        if (isSnapCueOnFlipFinish)
                        {
                            isSnapCueOnFlipFinish = false;
                            isPlaySnapCue = true;
                        }
                    }
                }
            }
            else if (isFlippingVertical)
            {
                if (!isFlipMidway)
                {
                    verticalFlipScale.Y -= FlipSpeed;

                    if (verticalFlipScale.Y <= 0)
                    {
                        verticalFlipScale.Y = FlipSpeed;
                        flipMatrix(false);
                        isFlipMidway = true;
                        IsReadyToUnsnap = true;
                    }
                }
                else
                {
                    verticalFlipScale.Y += FlipSpeed;

                    if (verticalFlipScale.Y >= 1)
                    {
                        verticalFlipScale.Y = 1;
                        isFlippingVertical = false;
                        isFlipMidway = false;
                        if (isSnapCueOnFlipFinish)
                        {
                            isSnapCueOnFlipFinish = false;
                            isPlaySnapCue = true;
                        }
                    }
                }
            }
            else
            {
                scale = Vector2.One;
            }
        }

        private void updateTransition(GameTime gameTime)
        {
            switch (State)
            {
                case ShapeState.TransitionIn:
                    alpha += AutoFadeDelta_Default;
                    if (alpha >= 1)
                    {
                        alpha = 1;
                        State = ShapeState.Selected;
                    }
                    break;
                case ShapeState.TransitionOut:
                    alpha -= AutoFadeDelta_Default;
                    if (alpha <= 0)
                    {
                        alpha = 0;
                        Reset();
                    }
                    break;
                case ShapeState.TransitionOutForHint:
                    alpha -= AutoFadeDelta_Hint;
                    if (alpha <= MinAlpha_Hint)
                    {
                        alpha = MinAlpha_Hint;
                        state = ShapeState.WaitOnHint;
                    }
                    break;
                case ShapeState.TransitionInAfterHint:
                    alpha += AutoFadeDelta_Hint;
                    if (alpha >= 1)
                    {
                        alpha = 1;
                        state = ShapeState.Dropped;
                    }
                    break;
            }

        }

        private void updateTimeIdle(GameTime gameTime)
        {
            switch (state)
            {
                case ShapeState.Selected:
                    timeIdle = 0f;
                    break;
                case ShapeState.Dropped:
                    timeIdle += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            updateTimeIdle(gameTime);

            if (IsTransitioning)
            {
                updateTransition(gameTime);
            }
            else
            {
                updateRotation(gameTime);
                updateFlip(gameTime);
            }
        }

        public void StartRotationCW()
        {
            isSnapped = false;
            isRotatingCW = true;
            destinationRotationIdx = currentRotationIdx + 1;
            if (destinationRotationIdx > ROTATIONS.Length - 1)
            {
                destinationRotationIdx = 0;
                rotation = MathHelper.ToRadians(-90);
            }
            rotateMatrix90(true);
        }

        public void StartRotationCCW()
        {
            isSnapped = false;
            isRotatingCCW = true;
            destinationRotationIdx = currentRotationIdx - 1;
            if (destinationRotationIdx < 0)
            {
                destinationRotationIdx = ROTATIONS.Length - 1;
                rotation = MathHelper.ToRadians(360);
            }
            rotateMatrix90(false);
        }

        public void StartFlipHorizontal()
        {
            isSnapped = false;
            isFlippingHorizontal = true;
            isFlipMidway = false;
        }

        public void StartFlipVertical()
        {
            isSnapped = false;
            isFlippingVertical = true;
            isFlipMidway = false;
        }

        public void flipMatrix(bool isHorizontal)
        {
            if (isHorizontal)
            {
                currentMatrix = Shape.flipMatrixHorizontal(currentMatrix, currentMatrixDimensions);
                // alternate between flip flags depending on odd or even rotation idx
                if (currentRotationIdx % 2 == 0)
                {
                    isFlippedHorizontal = !isFlippedHorizontal;
                }
                else
                {
                    isFlippedVertical = !isFlippedVertical;
                }
            }
            else
            {
                currentMatrix = Shape.flipMatrixVertical(currentMatrix, currentMatrixDimensions);
                if (currentRotationIdx % 2 == 0)
                {
                    isFlippedVertical = !isFlippedVertical;
                }
                else
                {
                    isFlippedHorizontal = !isFlippedHorizontal;
                }
            }
        }

        private static int[][] flipMatrixHorizontal(int[][] srcMatrix, Point dimensions)
        {
            int[][] retMatrix = srcMatrix;

            for (int y = 0; y < dimensions.Y; y++)
            {
                List<int> row = new List<int>(srcMatrix[y]);
                row.Reverse();
                retMatrix[y] = row.ToArray();
            }

            return retMatrix;
        }

        private static int[][] flipMatrixVertical(int[][] srcMatrix, Point dimensions)
        {
            int[][] retMatrix = new int[dimensions.Y][];
            for (int i = 0; i < dimensions.Y; i++)
            {
                retMatrix[i] = new int[dimensions.X];
            }

            // do top half then bottom half of matrix
            int halfHeight = dimensions.Y / 2;
            for (int y = 0; y < halfHeight; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    retMatrix[dimensions.Y - 1 - y][x] = srcMatrix[y][x];
                }
            }
            for (int y = halfHeight; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    retMatrix[dimensions.Y - 1 - y][x] = srcMatrix[y][x];
                }
            }
            
            return retMatrix;
        }

        private static int[][] rotateSquareMatrix90(int[][] srcMatrix, Point dimensions, bool isCW)
        {
            // init a temp 2d matrtix
            int[][] tmpMatrix = new int[dimensions.Y][];
            for (int i = 0; i < dimensions.Y; i++)
            {
                tmpMatrix[i] = new int[dimensions.X];
            }

            for (int y = 0; y < dimensions.Y; ++y)
            {
                for (int x = 0; x < dimensions.X; ++x)
                {
                    if (isCW)
                    {
                        tmpMatrix[y][dimensions.X - x - 1] = srcMatrix[x][y];
                    }
                    else
                    {
                        tmpMatrix[dimensions.X - y - 1][x] = srcMatrix[x][y];
                    }
                }
            }

            // do not have to switch currentMatDimensions because we are dealing with a square matrix
            return tmpMatrix;
        }

        private void rotateMatrix90(bool isCW)
        {

            if (CurrentMatrixDimensions.X == CurrentMatrixDimensions.Y)  // square, non-jagged matrix
            {
                currentMatrix = Shape.rotateSquareMatrix90(currentMatrix, currentMatrixDimensions, isCW);
            }

            else  // non-square, jagged matrix
            {
                if (CurrentMatrixDimensions.X > CurrentMatrixDimensions.Y)
                {
                    int rowDifference = Math.Abs(CurrentMatrixDimensions.X - CurrentMatrixDimensions.Y);

                    int[][] tmpMatrix = new int[CurrentMatrixDimensions.X][];
                    for (int i = 0; i < CurrentMatrixDimensions.X; i++)
                    {
                        tmpMatrix[i] = new int[CurrentMatrixDimensions.Y + rowDifference];
                    }

                    for (int y = 0; y < currentMatrixDimensions.Y; y++)
                    {
                        for (int x = 0; x < currentMatrixDimensions.X; x++)
                        {
                            tmpMatrix[y][x] = currentMatrix[y][x];
                        }
                    }

                    CurrentMatrix = new int[CurrentMatrixDimensions.X][];
                    for (int i = 0; i < CurrentMatrixDimensions.X; i++)
                    {
                        CurrentMatrix[i] = new int[CurrentMatrixDimensions.Y];
                    }

                    if (isCW)
                    {
                        tmpMatrix = Shape.rotateSquareMatrix90(tmpMatrix, new Point(currentMatrixDimensions.X, 
                                                                                    currentMatrixDimensions.Y + rowDifference), true);

                        Point previousMatrixDimensions = currentMatrixDimensions;
                        currentMatrixDimensions = new Point(previousMatrixDimensions.Y, previousMatrixDimensions.X);

                        for (int y = 0; y < previousMatrixDimensions.Y + rowDifference; y++)
                        {
                            for (int x = rowDifference; x < previousMatrixDimensions.X; x++)
                            {
                                CurrentMatrix[y][x - rowDifference] = tmpMatrix[y][x];
                            }
                        }
                    }
                    else
                    {
                        tmpMatrix = Shape.rotateSquareMatrix90(tmpMatrix, new Point(currentMatrixDimensions.X, 
                                                                                    currentMatrixDimensions.Y + rowDifference), false);

                        currentMatrixDimensions = new Point(currentMatrixDimensions.Y, currentMatrixDimensions.X);

                        for (int y = 0; y < currentMatrixDimensions.Y; y++)
                        {
                            for (int x = 0; x < currentMatrixDimensions.X; x++)
                            {
                                CurrentMatrix[y][x] = tmpMatrix[y][x];
                            }
                        }
                    }
                }
                else  // (CurrentMatrixDimensions.X < CurrentMatrixDimensions.Y)
                {
                    int colDifference = Math.Abs(CurrentMatrixDimensions.X - CurrentMatrixDimensions.Y);

                    int[][] tmpMatrix = new int[CurrentMatrixDimensions.Y][];
                    for (int i = 0; i < CurrentMatrixDimensions.Y; i++)
                    {
                        tmpMatrix[i] = new int[CurrentMatrixDimensions.X + colDifference];
                    }

                    for (int y = 0; y < currentMatrixDimensions.Y; y++)
                    {
                        for (int x = 0; x < currentMatrixDimensions.X; ++x)
                        {
                            tmpMatrix[y][x] = currentMatrix[y][x];
                        }
                    }

                    CurrentMatrix = new int[CurrentMatrixDimensions.X][];
                    for (int i = 0; i < CurrentMatrixDimensions.X; i++)
                    {
                        CurrentMatrix[i] = new int[CurrentMatrixDimensions.Y];
                    }

                    if (isCW)
                    {
                        tmpMatrix = Shape.rotateSquareMatrix90(tmpMatrix, new Point(currentMatrixDimensions.X + colDifference, 
                                                                                    currentMatrixDimensions.Y), true);

                        currentMatrixDimensions = new Point(currentMatrixDimensions.Y, currentMatrixDimensions.X);

                        for (int y = 0; y < currentMatrixDimensions.Y; ++y)
                        {
                            for (int x = 0; x < currentMatrixDimensions.X; ++x)
                            {
                                CurrentMatrix[y][x] = tmpMatrix[y][x];
                            }
                        }
                    }
                    else
                    {
                        tmpMatrix = Shape.rotateSquareMatrix90(tmpMatrix, new Point(currentMatrixDimensions.X + colDifference, 
                                                                                    currentMatrixDimensions.Y), false);

                        Point previousMatrixDimensions = currentMatrixDimensions;
                        currentMatrixDimensions = new Point(previousMatrixDimensions.Y, previousMatrixDimensions.X);

                        for (int y = colDifference; y < previousMatrixDimensions.Y; y++)
                        {
                            for (int x = 0; x < previousMatrixDimensions.X + colDifference; x++)
                            {
                                CurrentMatrix[y - colDifference][x] = tmpMatrix[y][x];
                            }
                        }
                    }
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, Color color, bool isSelected) //, float alpha)
        {
            // if the state is waiting dont draw large size shape
            if (State != ShapeState.Waiting)
            {
                SpriteEffects spriteEffect = SpriteEffects.None;
                float rotationDelta = 0;

                if (isFlippedHorizontal && isFlippedVertical)
                {
                    // rotate shape 180 degrees
                    rotationDelta = MathHelper.ToRadians(180);
                    spriteEffect = SpriteEffects.None;
                }
                else if (isFlippedHorizontal)
                {
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }
                else if (isFlippedVertical)
                {
                    spriteEffect = SpriteEffects.FlipVertically;
                }

                if (isFlippingHorizontal)
                {
                    if (currentRotationIdx % 2 == 0)
                    {
                        scale = horizontalFlipScale;
                    }
                    else
                    {
                        scale = new Vector2(horizontalFlipScale.Y, horizontalFlipScale.X);
                    }
                }
                else if (isFlippingVertical)
                {
                    if (currentRotationIdx % 2 == 0)
                    {
                        scale = verticalFlipScale;
                    }
                    else
                    {
                        scale = new Vector2(verticalFlipScale.Y, verticalFlipScale.X);
                    }
                }                

                spriteBatch.Draw((isDrawHighlighted ? textureHL : (isSelected ? textureHL : texture)),
                                 new Vector2((int)ScreenPosition.X, (int)ScreenPosition.Y),
                                 null,
                                 new Color(color.R, color.G, color.B, alpha),
                                 rotation + rotationDelta,
                                 BaseOrigin,
                                 scale,
                                 spriteEffect,
                                 0f);
            }
        }

        public void Render(SpriteBatch spriteBatch, Vector2 offset, PlayerSolutionShape pss, float renderScale)
        {
            SpriteEffects spriteEffect = SpriteEffects.None;
            float rotationDelta = 0;
            float rot = ROTATIONS[pss.RotationIdx];
            Vector2 position = new Vector2(pss.OriginPoint.X * TileSize.X, pss.OriginPoint.Y * TileSize.Y);
            Vector2 origin = baseOrigin;

            if (pss.IsFlippedHorizontal && pss.IsFlippedVertical)
            {
                // rotate shape 180 degrees
                rotationDelta = MathHelper.ToRadians(180);
                spriteEffect = SpriteEffects.None;
            }
            else if (pss.IsFlippedHorizontal)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else if (pss.IsFlippedVertical)
            {
                spriteEffect = SpriteEffects.FlipVertically;
            }

            // adjust position on odd rotation idx shapes
            if (baseMatrixDimensions.X != baseMatrixDimensions.Y)
            {
                if (pss.RotationIdx % 2 != 0)
                {
                    if (baseMatrixDimensions.X > baseMatrixDimensions.Y)
                    {
                        position = adjustRenderPosition(position, baseMatrixDimensions.X);
                    }
                    else
                    {
                        position = adjustRenderPosition(position, baseMatrixDimensions.Y);
                    }
                }
            }

            spriteBatch.Draw(texture,
                             (position + origin + offset) * new Vector2(renderScale, renderScale),
                             null,
                             Color.White,
                             rot + rotationDelta,
                             origin,
                             renderScale,
                             spriteEffect,
                             0f);
        }

        public void DrawForHint(SpriteBatch spriteBatch, Vector2 offset, ContentSolutionShape css, float pAlpha)
        {

            SpriteEffects spriteEffect = SpriteEffects.None;
            float rotationDelta = 0;
            float rot = ROTATIONS[css.RotationIdx];
            Vector2 position = new Vector2(css.OriginPoint.X * TileSize.X, css.OriginPoint.Y * TileSize.Y);
            Vector2 origin = baseOrigin;

            if (css.IsFlippedHorizontal && css.IsFlippedVertical)
            {
                // rotate shape 180 degrees
                rotationDelta = MathHelper.ToRadians(180);
                spriteEffect = SpriteEffects.None;
            }
            else if (css.IsFlippedHorizontal)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else if (css.IsFlippedVertical)
            {
                spriteEffect = SpriteEffects.FlipVertically;
            }

            // adjust position on odd rotation idx shapes
            if (baseMatrixDimensions.X != baseMatrixDimensions.Y)
            {
                if (css.RotationIdx % 2 != 0)
                {
                    if (baseMatrixDimensions.X > baseMatrixDimensions.Y)
                    {
                        position = adjustRenderPosition(position, baseMatrixDimensions.X);
                    }
                    else
                    {
                        position = adjustRenderPosition(position, baseMatrixDimensions.Y);
                    }
                }
            }

            spriteBatch.Draw(textureHL,
                             (position + origin + offset),
                             null,
                             new Color(1, 1, 1, pAlpha),
                             rot + rotationDelta,
                             origin,
                             1,
                             spriteEffect,
                             0f);
        }

        private Vector2 adjustRenderPosition(Vector2 position, int blockLength)
        {
            if (blockLength < 3 || blockLength > 5)
            {
                throw new ArgumentException("bad blockLength: " + blockLength);
            }

            switch (blockLength)
            {
                case 3:
                    return position + new Vector2(-TileSize.X / 2, TileSize.Y / 2);
                case 4:
                    return position + new Vector2(-TileSize.X, TileSize.Y);
                case 5:
                    return position + new Vector2(-TileSize.X * 2, TileSize.Y * 2);
                default:
                    return Vector2.Zero;
            }

        }

        public string GetDebugMatrix()
        {
            String returnString = "";
            for (int y = 0; y < CurrentMatrixDimensions.Y; y++)
            {
                for (int x = 0; x < CurrentMatrixDimensions.X; x++)
                {
                    returnString += CurrentMatrix[y][x] + " ";
                }
                returnString += "\n";
            }
            return returnString;
        }

        public string GetDebugInformation()
        {
            String returnString = "Name : " + name + " (Key: " + Key + ")" +
                                  "\n" + "" +
                                  "\nTile Size : " + tileSize.ToString() +
                                  "\nState : " + State.ToString() +
                                  "\nIs Valid : " + IsValid.ToString() +
                                  "\nBaseScrPosition : " + BaseScreenPosition.ToString() +
                                  "\nScreenPosition : " + ScreenPosition.ToString() +
                                  "\nOrigin : " + BaseOrigin.ToString() +
                                  "\nShape Origin : "+ShapeOrigin.ToString() +
                                  "\nCursor Offset : "+CursorReturnOffset.ToString() +
                                  "\nRotation : " + rotation + " radians" +
                                  "\nRotation idx : " + currentRotationIdx +
                                  "\nCurrent Dimensions : " + CurrentMatrixDimensions.ToString() +
                                  "\n[Flipping : " + IsFlipping + "] [Rotating : " + IsRotating + "]" +
                                  "\n[FlippedX : " + isFlippedHorizontal + "] [FlippedY : " + isFlippedVertical + "]";

            returnString += "\n\nCurrent Matrix: ";

            returnString += "\n-----\n";

            for (int y = 0; y < CurrentMatrixDimensions.Y; y++)
            {
                for (int x = 0; x < CurrentMatrixDimensions.X; x++)
                {
                    returnString += CurrentMatrix[y][x] + " ";
                }

                returnString += "\n";
            }

            return returnString;
        }

        public object Clone()
        {
            Shape shape = new Shape();

            shape.alpha = alpha;
//            shape.AssetName = AssetName;
            shape.BaseMatrix = (baseMatrix != null ? BaseMatrix.Clone() as int[][] : null);
            shape.BaseMatrixDimensions = BaseMatrixDimensions;
            shape.BaseOrigin = BaseOrigin;
            shape.BaseScreenPosition = BaseScreenPosition;
            shape.CurrentMatrix = (currentMatrix != null ? CurrentMatrix.Clone() as int[][] : null);
            shape.CurrentMatrixDimensions = CurrentMatrixDimensions;
            shape.currentRotationIdx = currentRotationIdx;
            shape.CursorReturnOffset = CursorReturnOffset;
            shape.destinationRotationIdx = destinationRotationIdx;
            shape.IsDrawHighlighted = IsDrawHighlighted;
            shape.horizontalFlipScale = horizontalFlipScale;
            shape.isFlipMidway = isFlipMidway;
            shape.isFlippedHorizontal = isFlippedHorizontal;
            shape.isFlippedVertical = isFlippedVertical;
            shape.isFlippingHorizontal = isFlippingHorizontal;
            shape.isFlippingVertical = isFlippingVertical;
            shape.isRotatingCW = isRotatingCW;
            shape.isRotatingCCW = isRotatingCCW;
            shape.IsSnapCueOnFlipFinish = IsSnapCueOnFlipFinish;
            shape.IsSnapCueOnRotateFinish = IsSnapCueOnRotateFinish;
            shape.IsValid = IsValid;
            shape.Key = Key;
            shape.MusicCueName = MusicCueName;
            shape.Name = Name;
            shape.rotation = rotation;
            shape.scale = scale;
            shape.ShapeOrigin = ShapeOrigin;
            shape.ScreenPosition = ScreenPosition;
            shape.shapeResetDistance = shapeResetDistance;
            shape.State = State;
            shape.texture = Texture;
            shape.textureHL = TextureHL;
            shape.TextureContentName = TextureContentName;
            shape.TileSize = TileSize;
            shape.TimeIdle = TimeIdle;
            shape.transition = transition;
            shape.verticalFlipScale = verticalFlipScale;

            return shape;
        }

        /*
        /// Reads a Character object from the content pipeline.
        public class ShapeReader : ContentTypeReader<Shape>
        {
            /// Reads a Character object from the content pipeline.
            protected override Shape Read(ContentReader input, Shape existingInstance)
            {
                Shape shape = existingInstance;
                if (shape == null)
                {
//                    throw new ArgumentNullException("existingInstance");
                    shape = new Shape();
                }

                shape.AssetName = input.AssetName;

                shape.Name = input.ReadString();
                shape.Key = input.ReadInt32();
                shape.TextureContentName = input.ReadString();
                shape.texture = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine(@"Textures\Shapes", shape.TextureContentName));
                shape.textureHL = input.ContentManager.Load<Texture2D>(System.IO.Path.Combine(@"Textures\Shapes", shape.TextureContentName + "HL"));
                shape.MusicCueName = input.ReadString();
                shape.BaseMatrixDimensions = input.ReadObject<Point>();
                shape.CurrentMatrixDimensions = shape.BaseMatrixDimensions;
                shape.TileSize = input.ReadObject<Point>();
                shape.BaseMatrix = input.ReadObject<int[][]>();
                shape.CurrentMatrix = shape.BaseMatrix.Clone() as int[][];
                shape.BaseScreenPosition = input.ReadObject<Vector2>();
                shape.BaseOrigin = new Vector2(shape.CurrentMatrixDimensions.X * shape.TileSize.X / 2, 
                                               shape.CurrentMatrixDimensions.Y * shape.TileSize.Y / 2);
                shape.ShapeOrigin = shape.BaseOrigin;
                shape.State = ShapeState.Waiting;

                return shape;
            }
        }
        */
    }
}
