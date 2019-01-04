using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShopData.Models;
using ShapeShopData.Util;
using System.Text;

namespace ShapeShop.UI
{
    public class PuzzleCog
    {
        public enum CogMovementType
        {
            Slide,
            Rotate,
        }

        public enum CogRotationDirection
        {
            CCW,
            CW,
        }

        public enum CogSlideDirection
        {
            Up,
            Down,
            Left,
            Right,
        }

        private const float SLIDE_RATE = 5f;
        private const float RESIZE_RATE = .005f;
        private const float COG_ROTATION_RATE_SLOW = .005f;
        private const float COG_ROTATION_RATE_MEDIUM = .02f;
        private const float COG_ROTATION_RATE_FAST = .04f;

        private const float ARM_ROTATION_RATE = .05f;
        private readonly float ROTATION_START = MathHelper.ToRadians(0); // MathHelper.ToRadians(0);
        private readonly float ROTATION_CW_MAX = MathHelper.ToRadians(180);
        private readonly float ROTATION_CCW_MAX = MathHelper.ToRadians(-180);
        private readonly Rectangle IND_OFF_SRC = new Rectangle(0, 0, 24, 24);
        private readonly Rectangle IND_SELECTED_SRC = new Rectangle(24, 0, 24, 24);
        private readonly Rectangle IND_IN_USE_SRC = new Rectangle(72, 0, 24, 24);

        private readonly CogMovementType movementType;
        public CogMovementType MovementType
        {
            get { return movementType; }
        }

        private Sprite cogSprite;
        private Sprite cogArmSprite;
        private Sprite armSprite;
        private Sprite shapeSprite;

        private readonly int shapeKey;
        private Color shapeColor;
        private Vector2 startPosition;
        private Vector2 phase1Position;
        private Vector2 endPosition;
        private readonly float startScale = 0f;
        private readonly float endScale = 1f;

        private readonly CogSlideDirection slideDirection;
        private readonly CogRotationDirection cogDirection = CogRotationDirection.CW;
        private readonly CogRotationDirection cogArmDirection = CogRotationDirection.CW;
        private Vector2 cogArmOrigin;
        private Vector2 armPosition;

        private PuzzlePanel parent;

        private Circle circleGeom;

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        private bool isRotating = true;
        public bool IsRotating
        {
            get { return isRotating; }
            set { isRotating = value; }
        }

        // state flags
        private bool isPhase1Finished = false;
        public bool IsPhase1Finished 
        { 
            get { return isPhase1Finished; }
            set { isPhase1Finished = value; }
        }

        private bool isPhaseExtendFinished = false;
        public bool IsPhaseExtendFinished 
        { 
            get { return isPhaseExtendFinished; }
            set { isPhaseExtendFinished = value; }
        }
        
        private bool isPhase2Finished = false;
        public bool IsPhase2Finished 
        { 
            get { return isPhase2Finished; }
            set { isPhase2Finished = value; }
        }

        private bool isStartupRotationFinished = false;
        public bool IsStartupRotationFinished 
        { 
            get { return isStartupRotationFinished; }
            set { isStartupRotationFinished = false; }
        }

        private bool isPhase2RevFinished = false;
        public bool IsPhase2RevFinished 
        { 
            get { return isPhase2RevFinished; }
            set { isPhase2RevFinished = value; }
        }

        private bool isPhaseRetractFinished = false;
        public bool IsPhaseRetractFinished 
        { 
            get { return isPhaseRetractFinished; }
            set { isPhaseRetractFinished = value; }
        }

        private bool isPhase1RevFinished = false;
        public bool IsPhase1RevFinished 
        { 
            get { return isPhase1RevFinished; }
            set { isPhase1RevFinished = value; }
        }

        private bool isShutdownRotationFinished = false;
        public bool IsShutdownRotationFinished 
        { 
            get { return isShutdownRotationFinished; }
            set { isShutdownRotationFinished = false; }
        }

        public Vector2 Position
        {
            get { return cogSprite.Position; }
        }

        public Shape PShape
        {
            get { return PuzzleEngine.CurrentPuzzleSet.ShapesDict[shapeKey]; }
        }

        public float CogRotation
        {
            get { return cogSprite.Rotation; }
        }

        private Rectangle focusRectangle;

        public bool HasFocus(Vector2 cursorPosition)
        {
            if (circleGeom.IsPositionWithin(cursorPosition))
            {
                return true;
            }
            return false;
        }

        public PuzzleCog(PuzzlePanel parent, int shapeKey, Color shapeColor,
                         Vector2 cogPosition, 
                         Vector2 cogArmPosition, Vector2 cogArmOrigin, Vector2 armPosition,
                         CogRotationDirection cogArmDirection,
                         CogRotationDirection cogDirection)
        {
            movementType = CogMovementType.Rotate;

            this.parent = parent;
            this.shapeKey = shapeKey;
            this.shapeColor = shapeColor;
            this.startPosition = cogArmPosition;
            this.endPosition = cogPosition;
            this.cogArmOrigin = cogArmOrigin;
            this.armPosition = armPosition;
            this.cogArmDirection = cogArmDirection;
            this.cogDirection = cogDirection;
            this.startScale = 1f;
        }

        public PuzzleCog(PuzzlePanel parent, int shapeKey, Color shapeColor,
                         Vector2 startPosition, Vector2 phase1Position, 
                         Vector2 endPosition, Vector2 armPosition,
                         float startScale, float endScale,
                         CogSlideDirection slideDirection,
                         CogRotationDirection cogDirection)
        {
            movementType = CogMovementType.Slide;

            this.parent = parent;
            this.shapeKey = shapeKey;
            this.shapeColor = shapeColor;
            this.startPosition = startPosition;
            this.phase1Position = phase1Position;
            this.endPosition = endPosition;
            this.armPosition = armPosition;
            this.startScale = startScale;
            this.endScale = endScale;
            this.cogDirection = cogDirection;
            this.slideDirection = slideDirection;
        }

        public void LoadContent(ContentManager content)
        {
            cogSprite = new Sprite(content.Load<Texture2D>("Textures/Cogs/pCog" + shapeKey),
                                   Color.White,
                                   startPosition,
                                   null,
                                   startScale,
                                   0f);

            shapeSprite = new Sprite(content.Load<Texture2D>("Textures/Shapes/smShape" + shapeKey),
                                     Color.White,
                                     startPosition,
                                     null,
                                     startScale,
                                     0f);

            armSprite = new Sprite(content.Load<Texture2D>("Textures/Cogs/pArm"),
                                   Color.White,
                                   armPosition,
                                   null,
                                   startScale,
                                   0f);

            switch (movementType)
            {
                case CogMovementType.Rotate:
                    cogArmSprite = new Sprite(content.Load<Texture2D>("Textures/Cogs/pCogArm" + shapeKey),
                          Color.White,
                          startPosition,
                          cogArmOrigin,
                          1f,
                          ROTATION_START);

                    cogSprite.Position = endPosition;
                    shapeSprite.Position = endPosition;

                    break;
                case CogMovementType.Slide:
                    switch (slideDirection)
                    {
                        case CogSlideDirection.Up:
                            break;
                        case CogSlideDirection.Down:
                            break;
                        case CogSlideDirection.Left:
                            armSprite.Rotation += MathHelper.ToRadians(-90);
                            break;
                        case CogSlideDirection.Right:
                            armSprite.Rotation += MathHelper.ToRadians(90);
                            break;
                    }
                    break;
            }

            circleGeom = new Circle(cogSprite.Texture.Width / 2, endPosition + cogSprite.Origin);
            focusRectangle = new Rectangle((int)endPosition.X, (int)endPosition.Y, (int)cogSprite.Texture.Width, (int)cogSprite.Texture.Height);

        }

        private void updatePositions(Vector2 rate)
        {
            armSprite.Position += rate;
            cogSprite.Position += rate;
            shapeSprite.Position += rate;
        }

        private void setPositions(Vector2 value, Vector2 thisArmOffset)
        {
            armSprite.Position = value + thisArmOffset;
            cogSprite.Position = value;
            shapeSprite.Position = value;
        }

        private void updatePhase1(GameTime gameTime)
        {
            if (!isPhase1Finished)
            {
                switch (slideDirection)
                {
                    case CogSlideDirection.Up:
                        if (cogSprite.Position.Y > phase1Position.Y)
                        {
                            updatePositions(new Vector2(0, -SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(phase1Position, new Vector2(0, PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase1Finished = true;
                        }
                        break;
                    case CogSlideDirection.Down:
                        if (cogSprite.Position.Y < phase1Position.Y)
                        {
                            updatePositions(new Vector2(0, SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(phase1Position, new Vector2(0, -PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase1Finished = true;
                        }

                        break;
                    case CogSlideDirection.Left:
                        if (cogSprite.Position.X > phase1Position.X)
                        {
                            updatePositions(new Vector2(-SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(phase1Position, new Vector2(PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase1Finished = true;
                        }

                        break;
                    case CogSlideDirection.Right:
                        if (cogSprite.Position.X < phase1Position.X)
                        {
                            updatePositions(new Vector2(SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(phase1Position, new Vector2(-PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase1Finished = true;
                        }
                        break;
                }
            }
        }

        private void updatePhase2(GameTime gameTime)
        {
            if (!isPhase2Finished)
            {
                switch (slideDirection)
                {
                    case CogSlideDirection.Up:
                        if (cogSprite.Position.Y < endPosition.Y)
                        {
                            updatePositions(new Vector2(0, SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(new Vector2(cogSprite.Position.X, endPosition.Y), new Vector2(0, PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase2Finished = true;
                        }
                        break;
                    case CogSlideDirection.Down:
                        if (cogSprite.Position.Y > endPosition.Y)
                        {
                            updatePositions(new Vector2(0, -SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(new Vector2(cogSprite.Position.X, endPosition.Y), new Vector2(0, -PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase2Finished = true;
                        }
                        break;
                    case CogSlideDirection.Left:
                        if (cogSprite.Position.X < endPosition.X)
                        {
                            updatePositions(new Vector2(SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(new Vector2(endPosition.X, cogSprite.Position.Y), new Vector2(PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase2Finished = true;
                        }
                        break;
                    case CogSlideDirection.Right:
                        if (cogSprite.Position.X > endPosition.X)
                        {
                            updatePositions(new Vector2(-SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(new Vector2(endPosition.X, cogSprite.Position.Y), new Vector2(-PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase2Finished = true;
                        }
                        break;
                }
            }
        }

        private void updatePhase2Rev(GameTime gameTime)
        {
            if (!isPhase2RevFinished)
            {
                switch (slideDirection)
                {
                    case CogSlideDirection.Up:
                        if (cogSprite.Position.Y > phase1Position.Y)
                        {
                            updatePositions(new Vector2(0, -SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(new Vector2(cogSprite.Position.X, phase1Position.Y), new Vector2(0, PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase2RevFinished = true;
                        }
                        break;
                    case CogSlideDirection.Down:
                        if (cogSprite.Position.Y < phase1Position.Y)
                        {
                            updatePositions(new Vector2(0, SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(new Vector2(cogSprite.Position.X, phase1Position.Y), new Vector2(0, -PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase2RevFinished = true;
                        }
                        break;
                    case CogSlideDirection.Left:
                        if (cogSprite.Position.X > phase1Position.X)
                        {
                            updatePositions(new Vector2(-SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(new Vector2(phase1Position.X, cogSprite.Position.Y), new Vector2(PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase2RevFinished = true;
                        }
                        break;
                    case CogSlideDirection.Right:
                        if (cogSprite.Position.X < phase1Position.X)
                        {
                            updatePositions(new Vector2(SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(new Vector2(phase1Position.X, cogSprite.Position.Y), new Vector2(-PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase2RevFinished = true;
                        }
                        break;
                }
            }
        }

        private void updatePhase1Rev(GameTime gameTime)
        {
            if (!isPhase1RevFinished)
            {
                switch (slideDirection)
                {
                    case CogSlideDirection.Up:
                        if (cogSprite.Position.Y < startPosition.Y)
                        {
                            updatePositions(new Vector2(0, SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(startPosition, new Vector2(0, PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase1RevFinished = true;
                        }
                        break;
                    case CogSlideDirection.Down:
                        if (cogSprite.Position.Y > startPosition.Y)
                        {
                            updatePositions(new Vector2(0, -SLIDE_RATE));
                        }
                        else
                        {
                            setPositions(startPosition, new Vector2(0, -PuzzlePanel.SLIDE_ARM_OFFSET));
                            isPhase1RevFinished = true;
                        }

                        break;
                    case CogSlideDirection.Left:
                        if (cogSprite.Position.X < startPosition.X)
                        {
                            updatePositions(new Vector2(SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(startPosition, new Vector2(PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase1RevFinished = true;
                        }

                        break;
                    case CogSlideDirection.Right:
                        if (cogSprite.Position.X > startPosition.X)
                        {
                            updatePositions(new Vector2(-SLIDE_RATE, 0));
                        }
                        else
                        {
                            setPositions(startPosition, new Vector2(-PuzzlePanel.SLIDE_ARM_OFFSET, 0));
                            isPhase1RevFinished = true;
                        }
                        break;
                }
            }
        }

        private void updateExtend(GameTime gameTime)
        {
            if (!isPhaseExtendFinished)
            {
                if (cogSprite.Scale < endScale)
                {
                    armSprite.Scale += RESIZE_RATE;
                    cogSprite.Scale += RESIZE_RATE;
                    shapeSprite.Scale += RESIZE_RATE;
                }
                else
                {
                    armSprite.Scale = endScale;
                    cogSprite.Scale = endScale;
                    shapeSprite.Scale = endScale;
                    isPhaseExtendFinished = true;
                }
            }
        }

        private void updateRetract(GameTime gameTime)
        {
            if (!isPhaseRetractFinished)
            {
                if (cogSprite.Scale > startScale)
                {
                    armSprite.Scale -= RESIZE_RATE;
                    cogSprite.Scale -= RESIZE_RATE;
                    shapeSprite.Scale -= RESIZE_RATE;
                }
                else
                {
                    armSprite.Scale = startScale;
                    cogSprite.Scale = startScale;
                    shapeSprite.Scale = startScale;
                    isPhaseRetractFinished = true;
                }
            }
        }

        private void updateStartupRotation(GameTime gameTime)
        {
            switch (cogArmDirection)
            {
                case CogRotationDirection.CW:
                    if (cogArmSprite.Rotation < ROTATION_CW_MAX)
                    {
                        cogArmSprite.Rotation += ARM_ROTATION_RATE;
                    }
                    else
                    {
                        cogArmSprite.Rotation = ROTATION_CW_MAX;
                    }
                    break;
                case CogRotationDirection.CCW:
                    if (cogArmSprite.Rotation > ROTATION_CCW_MAX)
                    {
                        cogArmSprite.Rotation -= ARM_ROTATION_RATE;
                    }
                    else
                    {
                        cogArmSprite.Rotation = ROTATION_CCW_MAX;
                    }
                    break;
            }

        }

        private void updateShutdownRotation(GameTime gameTime)
        {
            switch (cogArmDirection)
            {
                case CogRotationDirection.CW:
                    if (cogArmSprite.Rotation > ROTATION_START)
                    {
                        cogArmSprite.Rotation -= ARM_ROTATION_RATE;
                    }
                    else
                    {
                        cogArmSprite.Rotation = ROTATION_START;
                    }
                    break;
                case CogRotationDirection.CCW:
                    if (cogArmSprite.Rotation < ROTATION_START)
                    {
                        cogArmSprite.Rotation += ARM_ROTATION_RATE;
                    }
                    else
                    {
                        cogArmSprite.Rotation = ROTATION_START;
                    }
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (parent.PanelState)
            {
                case PuzzlePanel.PuzzlePanelState.Startup:
                    switch (parent.StartupState)
                    {
                        case PuzzlePanel.PanelStartupState.Phase1:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updatePhase1(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateStartupRotation(gameTime);
                                    break;
                            }
                            break;
                        case PuzzlePanel.PanelStartupState.Extend:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updateExtend(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateStartupRotation(gameTime);
                                    break;
                            }
                            break;
                        case PuzzlePanel.PanelStartupState.Phase2:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updatePhase2(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateStartupRotation(gameTime);
                                    break;
                            }
                            break;
                    }
                    break;
                case PuzzlePanel.PuzzlePanelState.Running:
                    if (PShape.State != Shape.ShapeState.Waiting)
                    {
                        if (isRotating)
                        {
                            if (PShape.IsValid)
                            {
                                cogSprite.Rotation = MathHelper.WrapAngle(cogSprite.Rotation + (COG_ROTATION_RATE_MEDIUM * (cogDirection == CogRotationDirection.CW ? 1 : -1)));
                                shapeSprite.Rotation = MathHelper.WrapAngle(shapeSprite.Rotation + (COG_ROTATION_RATE_MEDIUM * (cogDirection == CogRotationDirection.CW ? 1 : -1)));
                            }
                            else
                            {
                                cogSprite.Rotation = MathHelper.WrapAngle(cogSprite.Rotation + (COG_ROTATION_RATE_SLOW * (cogDirection == CogRotationDirection.CW ? 1 : -1)));
                                shapeSprite.Rotation = MathHelper.WrapAngle(shapeSprite.Rotation + (COG_ROTATION_RATE_SLOW * (cogDirection == CogRotationDirection.CW ? 1 : -1)));
                            }
                        }
                    }
                    break;
                case PuzzlePanel.PuzzlePanelState.Cleared:
                    break;
                case PuzzlePanel.PuzzlePanelState.PreShutdown:
                    if (cogSprite.Rotation == 0)
                    {
                    }
                    else if (cogSprite.Rotation > 0)
                    {
                        cogSprite.Rotation -= COG_ROTATION_RATE_FAST;
                        shapeSprite.Rotation -= COG_ROTATION_RATE_FAST;
                        if (cogSprite.Rotation <= 0)
                        {
                            cogSprite.Rotation = 0;
                            shapeSprite.Rotation = 0;
                        }
                    }
                    else if (cogSprite.Rotation < 0)
                    {
                        cogSprite.Rotation += COG_ROTATION_RATE_FAST;
                        shapeSprite.Rotation += COG_ROTATION_RATE_FAST;
                        if (cogSprite.Rotation >= 0)
                        {
                            cogSprite.Rotation = 0;
                            shapeSprite.Rotation = 0;
                        }
                    }
                    break;
                case PuzzlePanel.PuzzlePanelState.Shutdown:
                    switch (parent.ShutdownState)
                    {
                        case PuzzlePanel.PanelShutdownState.Phase2Rev:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updatePhase2Rev(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateShutdownRotation(gameTime);
                                    break;
                            }
                            break;
                        case PuzzlePanel.PanelShutdownState.Retract:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updateRetract(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateShutdownRotation(gameTime);
                                    break;
                            }
                            break;
                        case PuzzlePanel.PanelShutdownState.Phase1Rev:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updatePhase1Rev(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateShutdownRotation(gameTime);
                                    break;
                            }
                            break;
                    }
                    break;
                case PuzzlePanel.PuzzlePanelState.Ending:
                    switch (parent.EndingState)
                    {
                        case PuzzlePanel.GameEndingState.PanelPhase2Rev:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updatePhase2Rev(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateShutdownRotation(gameTime);
                                    break;
                            }
                            break;
                        case PuzzlePanel.GameEndingState.PanelRetract:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updateRetract(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateShutdownRotation(gameTime);
                                    break;
                            }
                            break;
                        case PuzzlePanel.GameEndingState.PanelPhase1Rev:
                            switch (movementType)
                            {
                                case CogMovementType.Slide:
                                    updatePhase1Rev(gameTime);
                                    break;
                                case CogMovementType.Rotate:
                                    updateShutdownRotation(gameTime);
                                    break;
                            }
                            break;
                    }
                    break;

            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            switch (movementType)
            {
                case CogMovementType.Slide:
                    armSprite.Draw(spriteBatch, drawOffset);
                    cogSprite.Draw(spriteBatch, drawOffset);
                    if (PShape.State == Shape.ShapeState.Waiting || PShape.State == Shape.ShapeState.TransitionOut)
                    {
                        shapeSprite.Draw(spriteBatch, drawOffset);
                    }
                    break;
                case CogMovementType.Rotate:
                    if (parent.IsRunning)
                    {
                        armSprite.Draw(spriteBatch);
                        cogSprite.Draw(spriteBatch);
                        if (PShape.State == Shape.ShapeState.Waiting || PShape.State == Shape.ShapeState.TransitionOut)
                        {
                            shapeSprite.Draw(spriteBatch);
                        }
                    }
                    else
                    {
                        cogArmSprite.Draw(spriteBatch);
                    }
                    break;
            }

        }

        public string GetDebugInformation()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append("#Cog" + shapeKey + "# [cogDirection:"+cogDirection+"]")
                  .Append('\n');
                  
            switch (movementType)
            {
                case CogMovementType.Rotate:
                    retStr.Append(" [moveType:"+movementType+"] [armDir:" + cogArmDirection+"] [armRot:"+cogArmSprite.Rotation+"]")
                          .Append('\n');
                    break;
                case CogMovementType.Slide:
                    retStr.Append(" [movementType: "+movementType+"] [slideDirection : " + slideDirection+"]")
                          .Append('\n');
                    break;
            }

            retStr.Append(" [position:" + cogSprite.Position + "] [rotation:"+cogSprite.Rotation+"] [scale:"+cogSprite.Scale+"]")
                  .Append('\n');

            return retStr.ToString();
        }

    }
}
