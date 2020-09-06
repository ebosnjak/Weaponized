using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test_game {
    public class Car {
        public Sprite sprite;
        public int index;
        public int health;
        public int maxHealth;
        public Car pulledBy;
        public float acceleration;
        public float speed;
        public float topSpeed;
        public float topSpeedReverse;
        public bool isDestroyed;
        public bool isPowered;
        public bool isBoosted;
        public bool isInvincible;
        public bool isAttached;

        public Weapon weapon;

        public Vector2 weaponPos;

        private Healthbar hpBar;
        private Healthbar reloadBar;
        public float defaultTopSpeed;
        public float defaultTopSpeedRev;

        public float remainingBoost;
        public float remainingInv;

        public Car(Sprite spr, int idx, Car pull, int hp = 500) {
            sprite = spr;
            index = idx;
            pulledBy = pull;
            health = hp;
            maxHealth = hp;
            isDestroyed = false;
            isBoosted = false;
            isInvincible = false;
            isAttached = false;

            hpBar = new Healthbar(new Vector2(100.0f, 8.0f), Color.SpringGreen, Color.DarkGray);
            hpBar.health = health;

            reloadBar = new Healthbar(new Vector2(80.0f, 8.0f), Color.Yellow, Color.DarkGray);
            reloadBar.health = 0;
            reloadBar.maxHealth = 100;

            weaponPos = Vector2.Zero;

            weapon = new Weapon_MachineGun();
            weapon.sprite.anchor = weaponPos;
            weapon.sprite.parent = sprite;
            weapon.owner = this;

            defaultTopSpeed = 300.0f;
            defaultTopSpeedRev = 150.0f;
            topSpeed = 300.0f;
            topSpeedReverse = 150.0f;
            acceleration = 0.0f;
            speed = 0.0f;
            remainingBoost = 0.0f;
            remainingInv = 0.0f;

            isPowered = false;
        }

        public void Equip(Weapon w) {
            weapon = w;
            weapon.sprite.anchor = weaponPos;
            weapon.sprite.parent = sprite;
            weapon.owner = this;
        }

        public void Update(GameTime gameTime) {
            if (isDestroyed) {
                return;
            }

            if (health <= 0) {
                isDestroyed = true;
                return;
            }

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            if (isBoosted) {
                remainingBoost -= deltaT;
                if (remainingBoost <= 0.0f) {
                    remainingBoost = 0.0f;
                    isBoosted = false;
                    topSpeed = defaultTopSpeed;
                    topSpeedReverse = defaultTopSpeedRev;
                    if (speed < 0.0f) {
                        if (speed < -topSpeedReverse) {
                            speed = -topSpeedReverse;
                        }
                    }
                    else {
                        if (speed > topSpeed) {
                            speed = topSpeed;
                        }
                    }
                }
            }

            if (isInvincible) {
                remainingInv -= deltaT;
                if (remainingInv <= 0.0f) {
                    remainingInv = 0.0f;
                    isInvincible = false;
                }
            }

            if (pulledBy == null || pulledBy.isDestroyed) {
                if (acceleration == 0.0f || !isPowered) {
                    speed -= speed * 0.93f * deltaT;
                    if (Math.Abs(speed) < 5.0f) speed = 0.0f;
                }
                else {
                    speed += acceleration * deltaT;
                    if (speed < 0) {
                        if (speed < -topSpeedReverse) speed = -topSpeedReverse;
                    }
                    else {
                        if (speed > topSpeed) speed = topSpeed;
                    }
                }

            }
            else {
                acceleration = pulledBy.acceleration;
                speed = pulledBy.speed;
            }
            
            sprite.position.X += speed * deltaT;

            hpBar.position = sprite.position + new Vector2(0.0f, -50.0f);
            hpBar.health += ((float)health - hpBar.health) * 8.0f * deltaT;
            hpBar.maxHealth = maxHealth;
            
            reloadBar.position = sprite.position + new Vector2(0.0f, 50.0f);

            weapon.sprite.anchor = weaponPos;

            weapon.Update(gameTime);
        }

        public void Draw(SpriteBatch batch) {
            if (isDestroyed) return;

            hpBar.Draw(batch);
            if (weapon.isReloading) {
                reloadBar.health = (weapon.reloadTime - weapon.remainingReloadT) / weapon.reloadTime * 100.0f;
                reloadBar.Draw(batch);
            }

            sprite.Draw(batch);
            weapon.sprite.Draw(batch);
        }
    }
}
