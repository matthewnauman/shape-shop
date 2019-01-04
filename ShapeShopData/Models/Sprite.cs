using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ShapeShopData.Models
{
    public class Sprite : ICloneable
    {
        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
        }

        public float Width
        {
            get { return texture.Width * scale; }
        }

        public float Height
        {
            get { return texture.Height * scale; }
        }

        protected Vector2 position = Vector2.Zero;
        public Vector2 Position
        {
            set { position = value; }
            get { return position; }
        }

        protected Vector2 origin = Vector2.Zero;
        public Vector2 Origin
        {
            set { origin = value; }
            get { return origin; }
        }

        protected float rotation = 0.0f;
        public float Rotation
        {
            set { rotation = value; }
            get { return rotation; }
        }

        protected float scale = 1f;
        public float Scale
        {
            set { scale = value; }
            get { return scale; }
        }

        protected Color color = Color.White;
        public Color Color
        {
            set { color = value; }
            get { return color; }
        }

        public Sprite(Texture2D texture)
        {
            this.texture = texture ?? throw new ArgumentNullException("texture");
        }

        public Sprite(Texture2D texture, Color color, Vector2 position, Vector2? origin, float scale, float rotation)
            : this(texture)
        {
            this.color = color;
            this.position = position;

            if (origin == null) { this.origin = new Vector2(texture.Width / 2, texture.Height / 2); }
            else { this.origin = (Vector2)origin; }
            
            this.rotation = rotation;
            this.scale = scale;
        }

        protected Sprite() { /* empty constructor set to protected so AnimatedSprite can create without a texture */ }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             position + origin,
                             null,
                             color,
                             rotation,
                             origin,
                             scale,
                             SpriteEffects.None,
                             0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(texture,
                             position + origin,
                             null,
                             color,
                             rotation,
                             origin,
                             scale,
                             spriteEffects,
                             0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            spriteBatch.Draw(texture,
                             position + origin + drawOffset,
                             null,
                             color,
                             rotation,
                             origin,
                             scale,
                             SpriteEffects.None,
                             0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 drawOffset, Color tmpColor)
        {
            spriteBatch.Draw(texture,
                             position + origin + drawOffset,
                             null,
                             tmpColor,
                             rotation,
                             origin,
                             scale,
                             SpriteEffects.None,
                             0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 drawOffset, Rectangle srcRect)
        {
            spriteBatch.Draw(texture,
                             position + origin + drawOffset,
                             srcRect,
                             color,
                             rotation,
                             origin,
                             scale,
                             SpriteEffects.None,
                             0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color tmpColor)
        {
//            this.color = color;
            spriteBatch.Draw(texture,
                             position + origin,
                             null,
                             tmpColor,
                             rotation,
                             origin,
                             scale,
                             SpriteEffects.None,
                             0);
        }

        public object Clone()
        {
            Sprite sprite = new Sprite();

            sprite.Color = Color;
            sprite.Origin = Origin;
            sprite.Position = Position;
            sprite.Rotation = Rotation;
            sprite.Scale = Scale;
            sprite.texture = texture;

            return sprite;
        }


    }
}
