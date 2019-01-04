using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShopData.Models;
using System;

namespace ShapeShop.UI
{
    public class StorageDeviceIndicator
    {
        private static readonly Vector2 POSITION = new Vector2(1071, 638);

        private Sprite sdSprite;

        public StorageDeviceIndicator() { }

        public void LoadContent(ContentManager content)
        {
            sdSprite = new Sprite(content.Load<Texture2D>("Textures/GameScreens/disk"), Color.White, POSITION, null, 1, 0);
        }

        public void Update(GameTime gameTime)
        {
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            sdSprite.Scale = 1 + pulsate * 0.05f;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (sdSprite != null)
            {
                sdSprite.Draw(spriteBatch, color);
            }
        }

    }
}
