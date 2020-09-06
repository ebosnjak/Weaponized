using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace test_game {
    public struct Animation {
        public int frameCount;
        public Vector2 frameSize;
        public float fps;
        public int index;

        public Animation(int fcnt, Vector2 fsz, float f, int i) {
            frameCount = fcnt;
            frameSize = fsz;
            fps = f;
            index = i;
        }
    }

    public class AnimatedSprite : Sprite {
        private float lastFrameTime;
        private int currentFrame;
        private bool isRepeating;
        public bool isPlaying;
        private Animation currentAnim;

        public Rectangle defaultFrame;

        public AnimatedSprite(Texture2D tex, Vector2 pos, float rot) : base(tex, pos, rot) {
            lastFrameTime = 0.0f;
            currentFrame = 0;
            defaultFrame = texture.Bounds;
            currentAnim = new Animation();
            isPlaying = false;
        }

        public void PlayAnimation(Animation anim, bool repeat = false) {
            currentAnim = anim;
            isRepeating = repeat;
            isPlaying = true;
            currentFrame = 0;
            lastFrameTime = -1.0f;
        }

        public void StopAnimation() {
            isRepeating = false;
            isPlaying = false;
            currentFrame = 0;
            lastFrameTime = -1.0f;
        }

        public void Update(GameTime gameTime) {
            if (!isPlaying) return;

            if (lastFrameTime < 0.0f) {
                lastFrameTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            float currentT = (float)gameTime.TotalGameTime.TotalSeconds;
            float delta = currentT - lastFrameTime;
            delta *= Game1.timeScale;

            if (delta > 1.0f / currentAnim.fps) {
                currentFrame++;
                if (isRepeating) {
                    currentFrame %= currentAnim.frameCount;
                }
                else {
                    if (currentFrame >= currentAnim.frameCount) {
                        currentFrame = 0;
                        isPlaying = false;
                    }
                }
                lastFrameTime = currentT;
            }
        }

        public override void Draw(SpriteBatch batch, float scale = 1.0f, bool ignoreCamera = false) {
            if (parent != null) {
                position = parent.position + anchor;
            }

            Vector2 topLeft = position - (ignoreCamera ? Vector2.Zero : (Game1.cameraTarget - Game1.screenSize / 2.0f));
            Rectangle frame = defaultFrame;
            if (isPlaying) {
                frame = new Rectangle((int)(currentFrame * currentAnim.frameSize.X),
                    (int)(currentAnim.index * currentAnim.frameSize.Y),
                    (int)(currentAnim.frameSize.X),
                    (int)(currentAnim.frameSize.Y));
            }

            batch.Draw(texture, topLeft, frame, color, MathHelper.ToRadians(rotation), origin, new Vector2(scale), SpriteEffects.None, 1);
        }
    }
}
