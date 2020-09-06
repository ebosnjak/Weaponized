using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace test_game {
    public class Healthbar {
        public float maxHealth;
        public float health;
        public Vector2 position;

        private Texture2D fgTex;
        private Texture2D bgTex;
        private Vector2 size;
        private Color fgColor = Color.DarkGreen;
        private Color bgColor = Color.DarkGray;

        public Healthbar(Vector2 sz, Color fgCol, Color bgCol) {
            maxHealth = 1;
            health = 1;
            position = Vector2.Zero;
            size = sz;
            fgColor = fgCol;
            bgColor = bgCol;

            Color[] data = new Color[(int)(sz.X * sz.Y)];
            for (int i = 0; i < data.Length; i++) {
                data[i] = Color.White;
            }

            fgTex = new Texture2D(Game1.gDevice, (int)sz.X, (int)sz.Y);
            fgTex.SetData(data);

            for (int i = 0; i < data.Length; i++) {
                data[i] = bgColor;
            }

            bgTex = new Texture2D(Game1.gDevice, (int)sz.X, (int)sz.Y);
            bgTex.SetData(data);
        }

        public void Draw(SpriteBatch batch, bool ignoreCamera = false) {
            Vector2 pos = position;
            if (!ignoreCamera) {
                pos -= Game1.cameraTarget - Game1.screenSize / 2.0f;
            }

            batch.Draw(bgTex, pos - size / 2.0f, Color.White);
            int w = (int)(health / maxHealth * size.X);
            Rectangle fgRect = new Rectangle((int)(pos.X - size.X / 2.0f),
                (int)(pos.Y - size.Y / 2.0f),
                w + 1,
                (int)size.Y + 1);

            batch.Draw(fgTex, fgRect, fgColor);
        }
    }
}
