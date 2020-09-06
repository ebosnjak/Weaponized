using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test_game {
    public abstract class Powerup {
        public Vector2 position;
        public Sprite sprite;
        public bool isActive;

        public abstract bool OnPickup(Car c);
        public abstract void Draw(SpriteBatch batch);
    }

    public class Powerup_InstaHealth : Powerup {
        public int hpGain;

        public Powerup_InstaHealth(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["powerups"], pos, 0.0f);
            sprite.origin = new Vector2(25.0f, 25.0f);
            sprite.textureRect = new Rectangle(0, 0, 50, 50);
            position = pos;
            isActive = true;
            hpGain = 300;
        }

        public override bool OnPickup(Car c) {
            if (!isActive) return false;
            if (c.health == c.maxHealth) return false;
            c.health = Math.Min(c.maxHealth, c.health + hpGain);
            isActive = false;
            Game1.effects.Add(new Effect_PowerupParticles(sprite.position));
            return true;
        }

        public override void Draw(SpriteBatch batch) {
            if (!isActive) return;
            sprite.Draw(batch);
        }
    }

    public class Powerup_Boost : Powerup {
        public float topSpeedIncrease;
        public float instantSpeed;
        public float boostDuration;
        
        public Powerup_Boost(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["powerups"], pos, 0.0f);
            sprite.origin = new Vector2(25.0f, 25.0f);
            sprite.textureRect = new Rectangle(50, 0, 50, 50);
            position = pos;
            isActive = true;
            topSpeedIncrease = 350.0f;
            instantSpeed = 225.0f;
            boostDuration = 4.0f;
        }

        public override bool OnPickup(Car c) {
            if (!isActive) return false;
            foreach (Car ca in Game1.trainCars) {
                if (!ca.isPowered && ca.pulledBy == null) continue;
                ca.isBoosted = true;
                ca.topSpeed += topSpeedIncrease;
                ca.topSpeedReverse += topSpeedIncrease;
                ca.speed += (ca.speed < 0.0f ? -1.0f : 1.0f) * instantSpeed;
                ca.remainingBoost = boostDuration;
                Effect_RailSparks s = new Effect_RailSparks(ca.sprite.position);
                s.parent = ca;
                s.sprite.parent = ca.sprite;
                s.sprite.anchor = Vector2.Zero;
                Game1.effects.Add(s);
            }

            isActive = false;
            Game1.effects.Add(new Effect_PowerupParticles(sprite.position));

            return true;
        }

        public override void Draw(SpriteBatch batch) {
            if (!isActive) return;
            sprite.Draw(batch);
        }
    }

    public class Powerup_Invincibility : Powerup {
        public float duration;

        public Powerup_Invincibility(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["powerups"], pos, 0.0f);
            sprite.origin = new Vector2(25.0f, 25.0f);
            sprite.textureRect = new Rectangle(100, 0, 50, 50);
            position = pos;
            isActive = true;
            duration = 5.0f;
        }

        public override bool OnPickup(Car c) {
            if (!isActive) return false;
            foreach (Car ca in Game1.trainCars) {
                ca.isInvincible = true;
                ca.remainingInv = duration;
            }

            isActive = false;
            Game1.effects.Add(new Effect_PowerupParticles(sprite.position));
            return true;
        }

        public override void Draw(SpriteBatch batch) {
            if (!isActive) return;
            sprite.Draw(batch);
        }
    }

    public class Powerup_Slowmo : Powerup {
        public float duration;

        public Powerup_Slowmo(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["powerups"], pos, 0.0f);
            sprite.origin = new Vector2(25.0f, 25.0f);
            sprite.textureRect = new Rectangle(150, 0, 50, 50);
            position = pos;
            isActive = true;
            duration = 8.0f;
        }

        public override bool OnPickup(Car c) {
            if (!isActive) return false;
            Game1.isSlowed = true;
            Game1.remainingSlow = duration;
            Game1.timeScale = 0.2f;
            isActive = false;
            Game1.effects.Add(new Effect_PowerupParticles(sprite.position));
            return true;
        }

        public override void Draw(SpriteBatch batch) {
            if (!isActive) return;
            sprite.Draw(batch);
        }
    }

    public class Powerup_Weapon : Powerup {
        public Powerup_Weapon(Vector2 pos) {
            sprite = new Sprite(Game1.textureMap["powerups"], pos, 0.0f);
            sprite.origin = new Vector2(25.0f, 25.0f);
            sprite.textureRect = new Rectangle(200, 0, 50, 50);
            position = pos;
            isActive = true;
        }

        public override bool OnPickup(Car c) {
            if (!isActive) return false;

            //if (Game1.availableWeapons.Count == Game1.MAX_WEAPONS) return false;
            //Random rnd = new Random();
            //int idx = rnd.Next(1, 6);
            //if (idx == 1) {
            //    Game1.availableWeapons.Add(new Weapon_MachineGun());
            //}
            //else if (idx == 2) {
            //    Game1.availableWeapons.Add(new Weapon_MGx2());
            //}
            //else if (idx == 3) {
            //    Game1.availableWeapons.Add(new Weapon_ExplosiveAutoCannon());
            //}
            //else if (idx == 4) {
            //    Game1.availableWeapons.Add(new Weapon_Cannon());
            //}
            //else if (idx == 5) {
            //    Game1.availableWeapons.Add(new Weapon_Minigun());
            //}

            if (c.weapon.OnUpgrade()) {
                isActive = false;
                Game1.effects.Add(new Effect_PowerupParticles(sprite.position));
                return true;
            }
            else {
                return false;
            }
        }

        public override void Draw(SpriteBatch batch) {
            if (!isActive) return;
            sprite.Draw(batch);
        }
    }
}
