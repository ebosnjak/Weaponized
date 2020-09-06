using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test_game {
    public abstract class Projectile {
        public Sprite sprite;
        public Vector2 velocity;

        public bool isFriendly;
        public bool isActive;
        public int damage;
        public float range;

        protected float distanceTraveled;

        public Weapon weapon;

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch batch);
        public abstract void OnHit(Car car);
        public abstract void OnHit(Enemy enemy);
        public abstract void OnMaxRange();
    }

    public class Projectile_Bullet : Projectile {
        public Projectile_Bullet(Weapon shotBy, Vector2 pos, float rot, Vector2 vel, int dmg, float r = 800.0f, bool fr = true) {
            sprite = new Sprite(Game1.textureMap["bullet2"], pos, rot);
            velocity = vel;
            damage = dmg;
            isFriendly = fr;
            isActive = true;
            range = r;
            weapon = shotBy;
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) return;

            if (distanceTraveled > range) {
                isActive = false;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            distanceTraveled += (velocity * deltaT).Length();
            sprite.position += velocity * deltaT;
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch);
        }

        public override void OnHit(Car car) {
            if (isFriendly || car.isInvincible) return;
            isActive = false;

            car.health -= damage;
        }

        public override void OnHit(Enemy enemy) {
            if (!isFriendly) return;
            isActive = false;

            enemy.health -= damage;
        }

        public override void OnMaxRange() {
            return;
        }
    }

    public class Projectile_ExplosiveRound : Projectile {
        public float dropoffStart;
        public float dropoffEnd;
        public float explosionScale;

        public Projectile_ExplosiveRound(Weapon shotBy, Vector2 pos, float rot, Vector2 vel, int dmg, float r = 800.0f, bool fr = true) {
            sprite = new Sprite(Game1.textureMap["bullet3"], pos, rot);
            velocity = vel;
            damage = dmg;
            isFriendly = fr;
            isActive = true;
            range = r;

            dropoffStart = 50.0f;
            dropoffEnd = 125.0f;
            explosionScale = 1.5f;

            weapon = shotBy;
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) return;

            if (distanceTraveled > range) {
                isActive = false;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            distanceTraveled += (velocity * deltaT).Length();
            sprite.position += velocity * deltaT;
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch);
        }

        public override void OnHit(Car car) {
            if (isFriendly || car.isInvincible) return;
            isActive = false;

            Effect_Explosion ex = new Effect_Explosion(sprite.position);
            ex.scale = explosionScale;
            Game1.effects.Add(ex);

            foreach (Car e in Game1.trainCars) {
                float dist = (e.sprite.position - sprite.position).Length();
                if (dist <= dropoffStart || e.sprite.GetAABB().Intersects(sprite.GetAABB())) {
                    e.health -= damage;
                }
                else if (dist <= dropoffEnd) {
                    float factor = damage / (float)Math.Pow((dropoffEnd - dropoffStart), 2.0f);
                    e.health -= Math.Max(0, damage - (int)(Math.Pow(dist - dropoffStart, 2.0f) * factor));
                }
            }
        }

        public override void OnHit(Enemy enemy) {
            if (!isFriendly) return;
            isActive = false;

            Effect_Explosion ex = new Effect_Explosion(sprite.position);
            ex.scale = explosionScale;
            Game1.effects.Add(ex);

            foreach (Enemy e in Game1.enemies) {
                float dist = (e.sprite.position - sprite.position).Length();
                if (dist <= dropoffStart || e.sprite.GetAABB().Intersects(sprite.GetAABB())) {
                    e.health -= damage;
                }
                else if (dist <= dropoffEnd) {
                    float factor = damage / (float)Math.Pow((dropoffEnd - dropoffStart), 2.0f);
                    e.health -= Math.Max(0, damage - (int)(Math.Pow(dist - dropoffStart, 2.0f) * factor));
                }
            }
        }

        public override void OnMaxRange() {
            return;
        }
    }

    public class Projectile_ExplosiveShell : Projectile {
        public float dropoffStart;
        public float dropoffEnd;
        public float explosionScale;

        public Projectile_ExplosiveShell(Weapon shotBy, Vector2 pos, float rot, Vector2 vel, int dmg, float r = 800.0f, bool fr = true) {
            sprite = new Sprite(Game1.textureMap["bullet1"], pos, rot);
            velocity = vel;
            damage = dmg;
            isFriendly = fr;
            isActive = true;
            range = r;

            weapon = shotBy;

            dropoffStart = 50.0f + (2.5f * (weapon.tier - 1));
            dropoffStart = Math.Min(dropoffStart, 200.0f);
            dropoffEnd = 75.0f + (5.0f * (weapon.tier - 1));
            dropoffEnd = Math.Min(dropoffEnd, 400.0f);
            explosionScale = 3.0f;
        }

        public override void Update(GameTime gameTime) {
            if (!isActive) return;

            if (distanceTraveled > range) {
                isActive = false;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            distanceTraveled += (velocity * deltaT).Length();
            sprite.position += velocity * deltaT;
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch);
        }

        public override void OnHit(Car car) {
            if (isFriendly || car.isInvincible) return;
            isActive = false;

            Effect_Explosion ex = new Effect_Explosion(sprite.position);
            ex.scale = explosionScale;
            Game1.effects.Add(ex);

            foreach (Car e in Game1.trainCars) {
                float dist = (e.sprite.position - sprite.position).Length();
                if (dist <= dropoffStart || e.sprite.GetAABB().Intersects(sprite.GetAABB())) {
                    e.health -= damage;
                }
                else if (dist <= dropoffEnd) {
                    float factor = damage / (float)Math.Pow((dropoffEnd - dropoffStart), 2.0f);
                    e.health -= Math.Max(0, damage - (int)(Math.Pow(dist - dropoffStart, 2.0f) * factor));
                }
            }
        }

        public override void OnHit(Enemy enemy) {
            if (!isFriendly) return;
            isActive = false;

            Effect_Explosion ex = new Effect_Explosion(sprite.position);
            ex.scale = explosionScale;
            Game1.effects.Add(ex);

            foreach (Enemy e in Game1.enemies) {
                float dist = (e.sprite.position - sprite.position).Length();
                if (dist <= dropoffStart || e.sprite.GetAABB().Intersects(sprite.GetAABB())) {
                    e.health -= damage;
                }
                else if (dist <= dropoffEnd) {
                    float factor = damage / (float)Math.Pow((dropoffEnd - dropoffStart), 2.0f);
                    e.health -= Math.Max(0, damage - (int)(Math.Pow(dist - dropoffStart, 2.0f) * factor));
                }
            }
        }

        public override void OnMaxRange() {
            return;
        }
    }
}
