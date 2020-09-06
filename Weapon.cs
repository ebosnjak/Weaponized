using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace test_game {
    public abstract class Weapon {
        public enum Mode {
            Semi,
            Auto
        }

        protected float lastFired;

        public Sprite sprite;
        public int damage;
        public float range;
        public float fireRate;
        public float bulletSpeed;
        public Mode fireMode;
        public int currentAmmo;
        public int maxAmmo;

        public bool isReloading;
        public float reloadTime;
        public float remainingReloadT;

        public string name;
        public string internalName;

        public int tier;

        public Car owner = null;

        public abstract void Update(GameTime gameTime);
        public abstract bool OnUpgrade();
        public abstract bool OnReload(GameTime gameTime);
        public abstract bool OnPrimaryFire(GameTime gameTime);
        public abstract bool OnPrimaryFireReleased(GameTime gameTime);
    }

    public class Weapon_MachineGun : Weapon {
        public Weapon_MachineGun() {
            sprite = new Sprite(Game1.textureMap["weapon_mg"], Vector2.Zero, 0.0f);
            damage = 60;
            fireRate = 6.5f;
            bulletSpeed = 1800.0f;
            sprite.origin = new Vector2(15.0f, 15.0f);
            fireMode = Weapon.Mode.Auto;

            currentAmmo = 40;
            maxAmmo = 40;
            reloadTime = 3.0f;

            lastFired = -100000.0f;

            tier = 1;

            name = "M85 Mounted MG";
            internalName = "mg_basic";
        }

        public override void Update(GameTime gameTime) {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            if (isReloading) {
                remainingReloadT -= deltaT;
                if (remainingReloadT <= 0.0f) {
                    remainingReloadT = 0.0f;
                    isReloading = false;
                    currentAmmo = maxAmmo;
                }
            }
        }

        public override bool OnReload(GameTime gameTime) {
            if (currentAmmo == maxAmmo || isReloading)
                return false;

            isReloading = true;
            remainingReloadT = reloadTime;

            return true;
        }

        public override bool OnUpgrade() {
            if (tier == 25)
                return false;

            tier++;
            if (tier == 2) {
                sprite.texture = Game1.textureMap["weapon_mg_mk2"];
            }

            damage += 100;
            fireRate = Math.Min(fireRate + 1.0f, 9.0f);
            reloadTime = Math.Max(reloadTime - 0.3f, 1.2f);
            maxAmmo = Math.Min(maxAmmo + 50, 500);
            currentAmmo = maxAmmo;
            isReloading = false;
            remainingReloadT = 0.0f;
            lastFired = -100000.0f;

            return true;
        }

        public override bool OnPrimaryFire(GameTime gameTime) {
            float deltaT = (float)gameTime.TotalGameTime.TotalSeconds - lastFired;
            deltaT *= Game1.timeScale;

            if (deltaT < 1.0f / fireRate) {
                return false;
            }

            if (isReloading) {
                return false;
            }

            if (currentAmmo == 0) {
                OnReload(gameTime);
                return false;
            }

            currentAmmo--;

            lastFired = (float)gameTime.TotalGameTime.TotalSeconds;
            float angle = MathHelper.ToRadians(sprite.rotation);
            Projectile_Bullet bullet = new Projectile_Bullet(
                this,
                sprite.position, 0.0f,
                bulletSpeed * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)),
                damage);
            bullet.sprite.position += 45.0f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            bullet.sprite.rotation = MathHelper.ToDegrees(angle);

            Game1.projectiles.Add(bullet);
            Effect_MuzzleFlash flash = new Effect_MuzzleFlash(bullet.sprite.position);
            flash.sprite.rotation = bullet.sprite.rotation;
            Game1.effects.Add(flash);

            return true;
        }

        public override bool OnPrimaryFireReleased(GameTime gameTime) {
            return true;
        }
    }

    public class Weapon_MGx2 : Weapon {
        private int barrel;
        private float length;
        private float barrelSpacing;

        public Weapon_MGx2() {
            sprite = new Sprite(Game1.textureMap["weapon_mgx2"], Vector2.Zero, 0.0f);
            damage = 40;
            fireRate = 9.0f;
            bulletSpeed = 1800.0f;
            sprite.origin = new Vector2(15.0f, 15.0f);
            fireMode = Weapon.Mode.Auto;
            barrel = 0;

            currentAmmo = 90;
            maxAmmo = 90;
            reloadTime = 4.0f;

            tier = 1;

            lastFired = -100000.0f;

            name = "MK74 Twin MG";
            internalName = "mg_x2";

            barrelSpacing = 8.0f;
            length = 35.0f;
        }

        public override void Update(GameTime gameTime) {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            if (isReloading) {
                remainingReloadT -= deltaT;
                if (remainingReloadT <= 0.0f) {
                    remainingReloadT = 0.0f;
                    isReloading = false;
                    currentAmmo = maxAmmo;
                }
            }
        }

        public override bool OnUpgrade() {
            if (tier == 25)
                return false;

            tier++;
            if (tier == 2) {
                sprite.texture = Game1.textureMap["weapon_mgx2_mk2"];
            }
            if (tier == 3) {
                sprite.texture = Game1.textureMap["weapon_mgx2_mk3"];
                sprite.origin = new Vector2(38.0f, 38.0f);
                length = 150.0f - 38.0f;
                barrelSpacing = 34.0f;
            }
            
            damage += 100;
            fireRate = Math.Min(fireRate + 1.0f, 18.0f);
            reloadTime = Math.Max(reloadTime - 0.4f, 1.6f);
            maxAmmo = Math.Min(maxAmmo + 50, 1200);
            currentAmmo = maxAmmo;
            isReloading = false;
            remainingReloadT = 0.0f;
            lastFired = -100000.0f;

            return true;
        }

        public override bool OnReload(GameTime gameTime) {
            if (currentAmmo == maxAmmo || isReloading)
                return false;

            isReloading = true;
            remainingReloadT = reloadTime;

            return true;
        }

        public override bool OnPrimaryFire(GameTime gameTime) {
            float deltaT = (float)gameTime.TotalGameTime.TotalSeconds - lastFired;
            deltaT *= Game1.timeScale;

            if (deltaT < 1.0f / fireRate) {
                return false;
            }

            if (isReloading) {
                return false;
            }

            if (currentAmmo == 0) {
                OnReload(gameTime);
                return false;
            }

            currentAmmo--;

            lastFired = (float)gameTime.TotalGameTime.TotalSeconds;
            float angle = MathHelper.ToRadians(sprite.rotation);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            Vector2 perpend = new Vector2(-direction.Y, direction.X);
            if (barrel == 1) perpend *= -1.0f;
            barrel++;
            barrel %= 2;

            Projectile_Bullet bullet = new Projectile_Bullet(
                this,
                sprite.position, 0.0f,
                bulletSpeed * direction,
                damage);

            bullet.sprite.position += length * direction;
            bullet.sprite.position += (barrelSpacing / 2.0f) * perpend;
            bullet.sprite.rotation = MathHelper.ToDegrees(angle);

            Game1.projectiles.Add(bullet);
            Effect_MuzzleFlash flash = new Effect_MuzzleFlash(bullet.sprite.position);
            flash.sprite.rotation = bullet.sprite.rotation;
            Game1.effects.Add(flash);

            return true;
        }

        public override bool OnPrimaryFireReleased(GameTime gameTime) {
            return true;
        }
    }

    public class Weapon_ExplosiveAutoCannon : Weapon {
        public Weapon_ExplosiveAutoCannon() {
            sprite = new Sprite(Game1.textureMap["weapon_mg_ex"], Vector2.Zero, 0.0f);
            damage = 150;
            fireRate = 4.0f;
            bulletSpeed = 1000.0f;
            sprite.origin = new Vector2(15.0f, 15.0f);
            fireMode = Weapon.Mode.Auto;

            currentAmmo = 35;
            maxAmmo = 35;
            reloadTime = 3.5f;

            tier = 1;

            lastFired = -100000.0f;

            name = "Explosive MG";
            internalName = "mg_explosive";
        }

        public override void Update(GameTime gameTime) {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            if (isReloading) {
                remainingReloadT -= deltaT;
                if (remainingReloadT <= 0.0f) {
                    remainingReloadT = 0.0f;
                    isReloading = false;
                    currentAmmo = maxAmmo;
                }
            }
        }

        public override bool OnUpgrade() {
            if (tier == 25)
                return false;

            tier++;

            damage += 100;
            fireRate = Math.Min(fireRate + 1.0f, 9.0f);
            reloadTime = Math.Max(reloadTime - 0.5f, 1.0f);
            maxAmmo = Math.Min(maxAmmo + 50, 450);
            currentAmmo = maxAmmo;
            isReloading = false;
            remainingReloadT = 0.0f;
            lastFired = -100000.0f;

            return true;
        }

        public override bool OnReload(GameTime gameTime) {
            if (currentAmmo == maxAmmo || isReloading)
                return false;

            isReloading = true;
            remainingReloadT = reloadTime;

            return true;
        }

        public override bool OnPrimaryFire(GameTime gameTime) {
            float deltaT = (float)gameTime.TotalGameTime.TotalSeconds - lastFired;
            deltaT *= Game1.timeScale;

            if (deltaT < 1.0f / fireRate) {
                return false;
            }

            if (isReloading) {
                return false;
            }

            if (currentAmmo == 0) {
                OnReload(gameTime);
                return false;
            }

            currentAmmo--;

            lastFired = (float)gameTime.TotalGameTime.TotalSeconds;
            float angle = MathHelper.ToRadians(sprite.rotation);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Projectile_ExplosiveRound bullet = new Projectile_ExplosiveRound(
                this,
                sprite.position, 0.0f,
                bulletSpeed * direction,
                damage);

            bullet.sprite.position += 45.0f * direction;
            bullet.sprite.rotation = MathHelper.ToDegrees(angle);

            Game1.projectiles.Add(bullet);
            Effect_MuzzleFlash flash = new Effect_MuzzleFlash(bullet.sprite.position);
            flash.sprite.rotation = bullet.sprite.rotation;
            Game1.effects.Add(flash);

            return true;
        }

        public override bool OnPrimaryFireReleased(GameTime gameTime) {
            return true;
        }
    }

    public class Weapon_Cannon : Weapon {
        private bool primaryHeld = false;
        private float length;

        public Weapon_Cannon() {
            sprite = new Sprite(Game1.textureMap["weapon_cannon"], Vector2.Zero, 0.0f);
            damage = 300;
            fireRate = 20.0f;
            bulletSpeed = 1000.0f;
            sprite.origin = new Vector2(15.0f, 15.0f);
            fireMode = Weapon.Mode.Auto;

            currentAmmo = 1;
            maxAmmo = 1;
            reloadTime = 2.0f;

            length = 45.0f;

            tier = 1;

            lastFired = -100000.0f;

            name = "115mm Heavy Cannon";
            internalName = "cannon";
        }

        public override void Update(GameTime gameTime) {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            if (isReloading) {
                remainingReloadT -= deltaT;
                if (remainingReloadT <= 0.0f) {
                    remainingReloadT = 0.0f;
                    isReloading = false;
                    currentAmmo = maxAmmo;
                }
            }
        }

        public override bool OnUpgrade() {
            if (tier == 25)
                return false;

            tier++;
            if (tier == 2) {
                sprite.texture = Game1.textureMap["weapon_cannon_mk2"];
            }
            if (tier == 3) {
                sprite.texture = Game1.textureMap["weapon_cannon_mk3"];
                sprite.origin = new Vector2(30.0f, 30.0f);
                length = 120.0f - 30.0f;
            }

            damage += 300;
            bulletSpeed = Math.Min(1800.0f, bulletSpeed + 150.0f);
            range = Math.Min(1200.0f, range + 75.0f);
            reloadTime = Math.Max(reloadTime - 0.2f, 1.0f);
            currentAmmo = maxAmmo;
            isReloading = false;
            remainingReloadT = 0.0f;
            lastFired = -100000.0f;

            return true;
        }

        public override bool OnReload(GameTime gameTime) {
            if (currentAmmo == maxAmmo || isReloading)
                return false;

            isReloading = true;
            remainingReloadT = reloadTime;

            return true;
        }

        public override bool OnPrimaryFire(GameTime gameTime) {
            float deltaT = (float)gameTime.TotalGameTime.TotalSeconds - lastFired;
            deltaT *= Game1.timeScale;

            if (deltaT < 1.0f / fireRate) {
                return false;
            }

            if (isReloading) {
                return false;
            }

            if (currentAmmo == 0) {
                OnReload(gameTime);
                return false;
            }

            currentAmmo--;

            lastFired = (float)gameTime.TotalGameTime.TotalSeconds;
            float angle = MathHelper.ToRadians(sprite.rotation);
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Projectile_ExplosiveShell bullet = new Projectile_ExplosiveShell(
                this,
                sprite.position, 0.0f,
                bulletSpeed * direction,
                damage);

            bullet.sprite.position += length * direction;
            bullet.sprite.rotation = MathHelper.ToDegrees(angle);

            Game1.projectiles.Add(bullet);
            Effect_MuzzleFlash flash = new Effect_MuzzleFlash(bullet.sprite.position);
            flash.sprite.rotation = bullet.sprite.rotation;
            Game1.effects.Add(flash);

            OnReload(gameTime);

            return true;
        }

        public override bool OnPrimaryFireReleased(GameTime gameTime) {
            return true;
        }
    }

    public class Weapon_Minigun : Weapon {
        private bool isSpinning = false; // I live in Spain without the "a"
        private int frame = 0;
        private float lastFrameTime = -1.0f;
        private float speed = 60.0f;

        public Weapon_Minigun() {
            sprite = new Sprite(Game1.textureMap["weapon_minigun"], Vector2.Zero, 0.0f);
            sprite.color = Color.White;
            damage = 40;
            fireRate = 20f;
            bulletSpeed = 2300.0f;
            sprite.origin = new Vector2(50.0f, 30.0f);
            sprite.textureRect = new Rectangle(0, 0, 150, 75);
            fireMode = Weapon.Mode.Auto;

            currentAmmo = 200;
            maxAmmo = 200;
            reloadTime = 7.0f;

            tier = 1;

            lastFired = -100000.0f;

            name = "G80 Gatling Gun";
            internalName = "minigun";
        }

        public override void Update(GameTime gameTime) {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaT *= Game1.timeScale;

            if (isReloading) {
                remainingReloadT -= deltaT;
                if (remainingReloadT <= 0.0f) {
                    remainingReloadT = 0.0f;
                    isReloading = false;
                    currentAmmo = maxAmmo;
                }
            }
        }

        public override bool OnUpgrade() {
            if (tier == 25)
                return false;

            tier++;

            damage += 100;
            fireRate = Math.Min(fireRate + 1.0f, 30.0f);
            reloadTime = Math.Max(reloadTime - 0.5f, 3.5f);
            maxAmmo = Math.Min(maxAmmo + 200, 4200);
            currentAmmo = maxAmmo;
            isReloading = false;
            remainingReloadT = 0.0f;
            lastFired = -100000.0f;

            return true;
        }

        public override bool OnReload(GameTime gameTime) {
            if (currentAmmo == maxAmmo || isReloading)
                return false;

            isReloading = true;
            remainingReloadT = reloadTime;

            return true;
        }

        public override bool OnPrimaryFire(GameTime gameTime) {
            float deltaT = (float)gameTime.TotalGameTime.TotalSeconds - lastFired;
            deltaT *= Game1.timeScale;

            if (deltaT < 1.0f / fireRate) {
                return false;
            }

            if (isReloading) {
                return false;
            }

            if (currentAmmo == 0) {
                OnReload(gameTime);
                return false;
            }

            currentAmmo--;

            if (!isSpinning) {
                isSpinning = true;
                if (lastFrameTime < 0.0f) {
                    lastFrameTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
            }

            float frameDeltaT = (float)gameTime.TotalGameTime.TotalSeconds - lastFrameTime;
            frameDeltaT *= Game1.timeScale;
            if (frameDeltaT >= 1.0f / speed) {
                lastFrameTime = (float)gameTime.TotalGameTime.TotalSeconds;
                frame++;
                frame %= 4;
            }

            sprite.textureRect = new Rectangle(150 * frame, 0, 150, 75);

            lastFired = (float)gameTime.TotalGameTime.TotalSeconds;
            float angle = MathHelper.ToRadians(sprite.rotation);
            Projectile_Bullet bullet = new Projectile_Bullet(
                this,
                sprite.position, 0.0f,
                bulletSpeed * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)),
                damage);
            bullet.sprite.position += 100.0f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            bullet.sprite.rotation = MathHelper.ToDegrees(angle);

            Game1.projectiles.Add(bullet);

            Effect_MuzzleFlash flash = new Effect_MuzzleFlash(bullet.sprite.position);
            flash.sprite.rotation = bullet.sprite.rotation;
            Game1.effects.Add(flash);

            return true;
        }

        public override bool OnPrimaryFireReleased(GameTime gameTime) {
            isSpinning = false;
            return true;
        }
    }
}
