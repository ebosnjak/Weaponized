using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace test_game {
    public class Sprite {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 origin = Vector2.Zero;
        public float rotation = 0.0f;
        public Color color = Color.White;

        public Sprite parent = null;
        public Vector2 anchor = Vector2.Zero;

        public Rectangle? textureRect = null;

        public Sprite(Texture2D tex, Vector2 pos, float rot) {
            texture = tex;
            position = pos;
            rotation = rot;
        }

        public void RotateTowards(Vector2 target) {
            Vector2 myPos;
            if (parent != null) {
                myPos = parent.position + anchor;
            }
            else {
                myPos = position;
            }

            rotation = (float)MathHelper.ToDegrees((float)Math.Atan2(target.Y - myPos.Y, target.X - myPos.X));
        }

        public Rectangle GetAABB(float scale = 1.0f) {
            float w = (textureRect == null ? texture.Width : textureRect.Value.Width) * scale;
            float h = (textureRect == null ? texture.Height : textureRect.Value.Height) * scale;

            Vector2[] vertices = new Vector2[4];
            vertices[0] = position - origin;
            vertices[1] = position - origin + new Vector2(w, 0.0f);
            vertices[2] = position - origin + new Vector2(0.0f, h);
            vertices[3] = position - origin + new Vector2(w, h);

            bool first = true;
            float top = 0.0f;
            float bottom = 0.0f;
            float left = 0.0f;
            float right = 0.0f;

            for (int i = 0; i < 4; i++) {
                float x0 = vertices[i].X - position.X, y0 = vertices[i].Y - position.Y;
                float angle = MathHelper.ToRadians(rotation);
                float x = x0 * (float)Math.Cos(angle) - y0 * (float)Math.Sin(angle) + position.X;
                float y = y0 * (float)Math.Cos(angle) + x0 * (float)Math.Sin(angle) + position.Y;

                if (first) {
                    left = x;
                    right = x;
                    top = y;
                    bottom = y;
                    first = false;
                }

                if (x < left) left = x;
                if (x > right) right = x;
                if (y < top) top = y;
                if (y > bottom) bottom = y;
            }

            return new Rectangle((int)left, (int)top,
                (int)(right - left), (int)(bottom - top));
        }

        public virtual void Draw(SpriteBatch batch, float scale = 1.0f, bool ignoreCamera = false) {
            if (parent != null) {
                position = parent.position + anchor;
            }

            batch.Draw(texture, position - (ignoreCamera ? Vector2.Zero : (Game1.cameraTarget - Game1.screenSize / 2.0f)), textureRect, color, MathHelper.ToRadians(rotation), origin, new Vector2(scale), SpriteEffects.None, 1);
        }
    }
}
