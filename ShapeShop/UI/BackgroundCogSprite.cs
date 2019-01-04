using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeShopData.Models;
using System.Collections.Generic;

namespace ShapeShop.UI
{
    class BackgroundCogSprite : Sprite
    {
        private static readonly List<float> rotationRatesList = new List<float> { .001f, .005f, .010f, .015f, .020f, .025f, .030f, .035f, .040f, .045f };

        public enum RotationDirection
        {
            CCW,
            CW,
        }

        private enum RotationMode
        {
            SpeedConstant,
            SpeedIncreasing,
            SpeedDecreasing,
        }

        private readonly RotationDirection rotDirection = RotationDirection.CW;
        private readonly int currentRotationRateLevel = 1;
        private float currentRotationRate = 0f;
        private readonly Texture2D boltTex;

        private float currentRotationRateLevelValue
        {
            get { return rotationRatesList[currentRotationRateLevel - 1]; }
        }

        private bool isMoving = true;
        public bool IsMoving
        {
            get { return isMoving; }
        }

        public BackgroundCogSprite(Texture2D texture, Texture2D boltTex, Color color, Vector2 position, Vector2? origin, float scale, float rotation, int rotationRateLevel, RotationDirection rotDirection)
            : base(texture, color, position, origin, scale, rotation)
        {
            this.boltTex = boltTex;
            this.currentRotationRateLevel = rotationRateLevel;
            this.rotDirection = rotDirection;
        }

        public void Update(GameTime gameTime)
        {
            if (isMoving)
            {
                currentRotationRate = currentRotationRateLevelValue;
                Rotation = MathHelper.WrapAngle(Rotation + (currentRotationRate * (rotDirection == RotationDirection.CW ? 1 : -1)));
            }
        }

        public void Stop(bool isHardStop)
        {
            if (isHardStop)
            {
                isMoving = false;
            }
            else
            {
                // implement gradual stop
            }
        }

        public void Start(bool isQuickStart)
        {
            if (isQuickStart)
            {
                isMoving = true;
            }
            else
            {
                // implement gradual stop
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color tmpColor)
        {
            base.Draw(spriteBatch, tmpColor);

            if (boltTex != null)
            {
                spriteBatch.Draw(boltTex,
                                 position + origin,
                                 null,
                                 tmpColor,
                                 0,
                                 origin,
                                 scale,
                                 SpriteEffects.None,
                                 0);
            }
        }

    }
}
