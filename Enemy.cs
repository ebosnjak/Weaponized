using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test_game {
    public abstract class Enemy {
        public Sprite sprite;
        public Healthbar hpBar;
        public bool ranged;
        public bool isAlive;
        public int health;
        public int maxHealth;
        public float moveSpeed;
        public bool isChasing;
        public float visionRange;
        public float attackRange;

        public int damage;

        public int value;

        public Car attackTarget;

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch batch);
        public abstract bool OnAttack(Car target);
        public abstract bool OnDeath();
    }

    public class Enemy_Melee : Enemy {
        public Enemy_Melee(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["enemy3"], pos, 0.0f);
            sprite.origin = new Vector2(sprite.texture.Width / 2.0f, sprite.texture.Height / 2.0f);
            ranged = false;
            isAlive = true;
            health = 120;
            maxHealth = 120;
            moveSpeed = 400.0f;
            isChasing = false;
            hpBar = new Healthbar(new Vector2(40.0f, 10.0f), Color.DarkRed, Color.DarkGray);
            hpBar.health = health;
            attackTarget = null;
            value = 100;
        }

        public override void Update(GameTime gameTime) {
            if (!isAlive) {
                return;
            }

            if (health <= 0) {
                isAlive = false;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            hpBar.health += ((float)health - hpBar.health) * 8.0f * deltaT;
            hpBar.maxHealth = maxHealth;
            hpBar.position = sprite.position - new Vector2(0.0f, 50.0f);
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch);
            hpBar.Draw(batch);
        }

        public override bool OnAttack(Car target) {
            throw new NotImplementedException();
        }

        public override bool OnDeath() {
            isAlive = false;

            Random rng = new Random();
            int powerup = rng.Next(0, 100);
            if (powerup < 60) {
                // don't drop anything
            }
            else if (powerup < 75) {
                Game1.powerups.Add(new Powerup_InstaHealth(sprite.position));
            }
            else if (powerup < 80) {
                Game1.powerups.Add(new Powerup_Boost(sprite.position));
            }
            else if (powerup < 90) {
                Game1.powerups.Add(new Powerup_Invincibility(sprite.position));
            }
            else if (powerup < 95) {
                Game1.powerups.Add(new Powerup_Slowmo(sprite.position));
            }
            else if (powerup < 100) {
                Game1.powerups.Add(new Powerup_Weapon(sprite.position));
            }

            Game1.playerScore += value * (Game1.survival ? 3 : 1);

            Game1.effects.Add(new Effect_Shatter(sprite.position));
            return true;
        }
    }

    public class Enemy_Exploder : Enemy {
        private bool suicide;
        public float dropoffStart = 75.0f;

        public Enemy_Exploder(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["enemy4"], pos, 0.0f);
            sprite.origin = new Vector2(sprite.texture.Width / 2.0f, sprite.texture.Height / 2.0f);
            ranged = false;
            isAlive = true;
            isChasing = false;
            health = 150;
            maxHealth = 150;
            moveSpeed = 250.0f;
            hpBar = new Healthbar(new Vector2(40.0f, 10.0f), Color.DarkRed, Color.DarkGray);
            hpBar.health = health;
            attackTarget = null;
            attackRange = 100.0f;
            visionRange = 500.0f;
            value = 150;
            suicide = false;

            damage = 200;
        }

        public override void Update(GameTime gameTime) {
            if (!isAlive) {
                return;
            }

            if (health <= 0) {
                isAlive = false;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            hpBar.health += ((float)health - hpBar.health) * 8.0f * deltaT;
            hpBar.maxHealth = maxHealth;
            hpBar.position = sprite.position - new Vector2(0.0f, 50.0f);

            if (attackTarget == null) {
                float minDist = -1.0f;
                foreach (Car c in Game1.trainCars) {
                    if (minDist < 0.0f) {
                        minDist = (c.sprite.position - sprite.position).Length();
                        if (minDist <= visionRange) {
                            attackTarget = c;
                        }
                    }
                    else {
                        float d = (c.sprite.position - sprite.position).Length();
                        if (d < minDist) {
                            minDist = d;
                            attackTarget = c;
                        }
                    }
                }
            }

            if (attackTarget != null) {
                float dist = (attackTarget.sprite.position - sprite.position).Length();
                if (dist > visionRange) {
                    attackTarget = null;
                    isChasing = false;
                }
                else {
                    isChasing = true;
                    sprite.RotateTowards(attackTarget.sprite.position);
                    Vector2 dir = attackTarget.sprite.position - sprite.position;
                    dir.Normalize();

                    sprite.position += dir * moveSpeed * deltaT;
                }
            }

            if (isChasing && attackTarget != null &&
                ((attackTarget.sprite.position - sprite.position).Length() <= 75.0f || attackTarget.sprite.GetAABB().Intersects(sprite.GetAABB()))) {
                OnAttack(attackTarget);
                return;
            }

            foreach (Car c in Game1.trainCars) {
                if ((c.sprite.position - sprite.position).Length() <= 75.0f ||
                    c.sprite.GetAABB().Intersects(sprite.GetAABB())) {
                    OnAttack(c);
                    return;
                }
            }
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch);
            hpBar.Draw(batch);
        }

        public override bool OnAttack(Car target) {
            suicide = true;
            Game1.effects.Add(new Effect_Explosion(sprite.position));
            foreach (Car c in Game1.trainCars) {
                if (c.isInvincible) continue;

                float dist = (sprite.position - c.sprite.position).Length();
                if (dist <= dropoffStart || c.sprite.GetAABB().Intersects(sprite.GetAABB())) {
                    c.health -= damage;
                }
                else if (dist <= attackRange) {
                    float factor = damage / (float)Math.Pow((attackRange - dropoffStart), 2.0f);
                    c.health -= Math.Max(0, damage - (int)(Math.Pow(dist - dropoffStart, 2.0f) * factor));
                }
            }

            isAlive = false;

            return true;
        }

        public override bool OnDeath() {
            isAlive = false;

            if (!suicide) {
                Game1.playerScore += value * (Game1.survival ? 3 : 1);

                Random rng = new Random();
                int powerup = rng.Next(0, 100);

                if (powerup < 12) {
                    Game1.powerups.Add(new Powerup_InstaHealth(sprite.position));
                }
                else if (powerup < 17) {
                    Game1.powerups.Add(new Powerup_Boost(sprite.position));
                }
                else if (powerup < 27) {
                    Game1.powerups.Add(new Powerup_Invincibility(sprite.position));
                }
                else if (powerup < 29) {
                    Game1.powerups.Add(new Powerup_Slowmo(sprite.position));
                }
                else if (powerup < 30) {
                    Game1.powerups.Add(new Powerup_Weapon(sprite.position));
                }
                else {
                    // drop nothing
                }
            }

            Game1.effects.Add(new Effect_Shatter(sprite.position));
            return true;
        }
    }

    public class Enemy_Shooter : Enemy {
        public Weapon weapon;
        public float lastFired;
        public float fireRate;
        public float currentT;

        public Enemy_Shooter(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["enemy1"], pos, 0.0f);
            sprite.origin = new Vector2(sprite.texture.Width / 2.0f, sprite.texture.Height / 2.0f);
            ranged = true;
            isAlive = true;
            isChasing = false;
            health = 300;
            maxHealth = 300;
            moveSpeed = 250.0f;
            hpBar = new Healthbar(new Vector2(40.0f, 10.0f), Color.DarkRed, Color.DarkGray);
            hpBar.health = health;
            attackTarget = null;
            attackRange = 400.0f;
            visionRange = 550.0f;
            value = 200;

            damage = 30;
            fireRate = 2.0f;
            lastFired = -100000.0f;
        }

        public override void Update(GameTime gameTime) {
            if (!isAlive) {
                return;
            }

            if (health <= 0) {
                isAlive = false;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            currentT = (float)gameTime.TotalGameTime.TotalSeconds;

            hpBar.health += ((float)health - hpBar.health) * 8.0f * deltaT;
            hpBar.maxHealth = maxHealth;
            hpBar.position = sprite.position - new Vector2(0.0f, 50.0f);

            if (attackTarget == null || attackTarget.isDestroyed) {
                float minDist = -1.0f;
                foreach (Car c in Game1.trainCars) {
                    if (minDist < 0.0f) {
                        minDist = (c.sprite.position - sprite.position).Length();
                        if (minDist <= visionRange) {
                            attackTarget = c;
                        }
                    }
                    else {
                        float d = (c.sprite.position - sprite.position).Length();
                        if (d < minDist) {
                            minDist = d;
                            attackTarget = c;
                        }
                    }
                }
            }

            if (attackTarget != null) {
                float dist = (attackTarget.sprite.position - sprite.position).Length();
                if (dist > visionRange) {
                    attackTarget = null;
                    isChasing = false;
                }
                else {
                    isChasing = true;
                    sprite.RotateTowards(attackTarget.sprite.position);
                    Vector2 dir = attackTarget.sprite.position - sprite.position;
                    dir.Normalize();

                    if (dist > attackRange) {
                        sprite.position += dir * moveSpeed * deltaT;
                    }
                }
            }

            if (isChasing && attackTarget != null &&
                (attackTarget.sprite.position - sprite.position).Length() <= attackRange) {
                OnAttack(attackTarget);
                return;
            }

            foreach (Car c in Game1.trainCars) {
                if ((c.sprite.position - sprite.position).Length() <= attackRange) {
                    OnAttack(c);
                    return;
                }
            }
        }

        public override void Draw(SpriteBatch batch) {
            sprite.Draw(batch);
            hpBar.Draw(batch);
        }

        public override bool OnAttack(Car target) {
            sprite.RotateTowards(target.sprite.position);

            float deltaT = currentT - lastFired;
            deltaT *= Game1.timeScale;
            if (deltaT < 1.0f / fireRate) {
                return false;
            }

            float angle = MathHelper.ToRadians(sprite.rotation);

            lastFired = currentT;
            Projectile_Bullet bullet = new Projectile_Bullet(
                null, sprite.position, sprite.rotation,
                1000.0f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)),
                damage);

            bullet.sprite.position += 35.0f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            bullet.isFriendly = false;
            bullet.sprite.color = Color.Red;
            bullet.range = attackRange;
            Game1.projectiles.Add(bullet);

            Effect_MuzzleFlash flash = new Effect_MuzzleFlash(bullet.sprite.position);
            flash.sprite.rotation = bullet.sprite.rotation;
            Game1.effects.Add(flash);

            return true;
        }

        public override bool OnDeath() {
            isAlive = false;

            Game1.playerScore += value * (Game1.survival ? 3 : 1);

            Random rng = new Random();
            int powerup = rng.Next(0, 100);
            if (powerup < 5) {
                Game1.powerups.Add(new Powerup_InstaHealth(sprite.position));
            }
            else if (powerup < 10) {
                Game1.powerups.Add(new Powerup_Boost(sprite.position));
            }
            else if (powerup < 15) {
                Game1.powerups.Add(new Powerup_Invincibility(sprite.position));
            }
            else if (powerup < 17) {
                Game1.powerups.Add(new Powerup_Slowmo(sprite.position));
            }
            else if (powerup < 20) {
                Game1.powerups.Add(new Powerup_Weapon(sprite.position));
            }
            else {
                // drop nothing
            }

            Game1.effects.Add(new Effect_Shatter(sprite.position));
            return true;
        }
    }
}
