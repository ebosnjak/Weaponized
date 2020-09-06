using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace test_game {
    public abstract class Effect {
        public AnimatedSprite sprite;
        public Vector2 position;
        public bool isActive;
        public float scale;

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch batch);
    }

    public class Effect_Explosion : Effect {
        public Effect_Explosion(Vector2 pos) {
            position = pos;
            isActive = true;
            scale = 2.0f;
            sprite = new AnimatedSprite(Game1.textureMap["explosion1"], position, 0.0f);
            sprite.origin = new Vector2(30.0f, 30.0f);
            sprite.PlayAnimation(new Animation(10, new Vector2(60.0f, 60.0f), 30.0f, 0));
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) {
                return;
            }

            if (!sprite.isPlaying) {
                isActive = false;
                return;
            }

            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch, scale);
        }
    }

    public class Effect_Shatter : Effect {
        public Effect_Shatter(Vector2 pos) {
            position = pos;
            isActive = true;
            scale = 2.0f;
            sprite = new AnimatedSprite(Game1.textureMap["shatter"], position, 0.0f);
            sprite.origin = new Vector2(30.0f, 30.0f);
            sprite.PlayAnimation(new Animation(5, new Vector2(60.0f, 60.0f), 30.0f, 0));
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) {
                return;
            }

            if (!sprite.isPlaying) {
                isActive = false;
                return;
            }

            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch, scale);
        }
    }

    public class Effect_MuzzleFlash : Effect {
        public Effect_MuzzleFlash(Vector2 pos) {
            position = pos;
            isActive = true;
            scale = 1.0f;
            sprite = new AnimatedSprite(Game1.textureMap["flash"], position, 0.0f);
            sprite.origin = new Vector2(0.0f, 22.0f);
            sprite.PlayAnimation(new Animation(1, new Vector2(45.0f, 45.0f), 60.0f, 0));
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) {
                return;
            }

            if (!sprite.isPlaying) {
                isActive = false;
                return;
            }

            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch, scale);
        }
    }

    public class Effect_PowerupParticles : Effect {
        public Effect_PowerupParticles(Vector2 pos) {
            position = pos;
            isActive = true;
            scale = 1.0f;
            sprite = new AnimatedSprite(Game1.textureMap["powerup_particles"], position, 0.0f);
            sprite.origin = new Vector2(25.0f, 25.0f);
            sprite.PlayAnimation(new Animation(5, new Vector2(50.0f, 50.0f), 30.0f, 0));
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) {
                return;
            }

            if (!sprite.isPlaying) {
                isActive = false;
                return;
            }

            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch, scale);
        }
    }

    public class Effect_RailSparks : Effect {
        public Car parent;

        public Effect_RailSparks(Vector2 pos) {
            position = pos;
            isActive = true;
            scale = 1.0f;
            sprite = new AnimatedSprite(Game1.textureMap["sparks"], position, 0.0f);
            sprite.origin = new Vector2(90.0f, 45.0f);
            sprite.PlayAnimation(new Animation(5, new Vector2(180.0f, 90.0f), 30.0f, 0), true);
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) {
                return;
            }

            if (!sprite.isPlaying) {
                isActive = false;
                return;
            }

            if (parent.speed <= parent.defaultTopSpeed && parent.speed >= -parent.defaultTopSpeedRev) {
                isActive = false;
                return;
            }

            sprite.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch, scale);
        }
    }
}
