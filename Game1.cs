using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace test_game {
    public class Game1 : Game {
        public enum GameState {
            Start,
            Menu,
            Running,
            Paused,
            GameOver,
            Help
        };

        public static Dictionary< string, Texture2D > textureMap;
        public static Vector2 cameraTarget;
        public static Vector2 screenSize;
        public static GraphicsDevice gDevice;
        public static List< Projectile > projectiles;
        public static List< Car > trainCars;
        public static List< Enemy > enemies;
        public static List< Effect > effects;
        public static List< Powerup > powerups;
        public static List< Weapon > availableWeapons;
        public static float timeScale;
        public static int playerScore;
        public static int MAX_WEAPONS;

        public static bool isSlowed;
        public static float remainingSlow;

        public static bool survival = false;

        public bool canShoot;

        public GameState state;

        private Vector2 prevCamTarget;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D kekw;
        private Sprite test;

        private Texture2D background;
        private Texture2D bush;

        private SpriteFont font;

        private Sprite rails1;
        private Sprite rails2;

        private int selectedCar = 0;

        private float lastSpawnTime = -100000.0f;
        private float spawnRate = 1.3f;

        private float maxX = 0.0f;
        private float lastHealthIncreaseX = 0.0f;

        private float trackY;
        private Vector2 carSize;

        private const int MAX_CARS = 3;
        private const int MAX_ENEMIES = 30;

        public static bool[] keyState;
        public static bool[] prevKeyState;
        public static bool[] mouseState;
        public static bool[] prevMouseState;

        public string helpText =
            "Use W or A to accelerate\n" +
            "Use S or D to reverse\n" +
            "Left Mouse to shoot, Right Mouse to pick up powerups\n" +
            "1, 2, 3, 4 switch between train cars (1 is the rightmost one)\n" +
            "R - reload, Tab - Weapon Inventory, L - End game, Esc - Pause\n" +
            "\n" +
            "There are 5 powerups: \n" +
            "Health, Boost, Invincibility, Slow-mo and Weapon Upgrade\n" +
            "\n" +
            "Weapons can be upgraded up to Tier 25, gaining better stats\n" +
            "with each level. Enemies become stronger and spawn more often\n" +
            "the further you travel. When your locomotive is destroyed, \n" +
            "you can't move, but each kill is worth 3x the points, so do\n" +
            "your best to hold out. GL HF\n" +
            "\n" +
            "\n" +
            "Press Esc to go back.";

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // for key presses
        private static bool IsKeyHeld(Keys key) {
            return keyState[(int)key];
        }

        private static bool IsKeyPressed(Keys key) {
            return keyState[(int)key] && !prevKeyState[(int)key];
        }

        private static bool IsKeyUp(Keys key) {
            return !keyState[(int)key];
        }

        private static bool IsKeyReleased(Keys key) {
            return !keyState[(int)key] && prevKeyState[(int)key];
        }

        // for mouse button presses
        private static bool IsButtonHeld(int idx) {
            return mouseState[idx];
        }

        private static bool IsButtonPressed(int idx) {
            return mouseState[idx] && !prevMouseState[idx];
        }

        private static bool IsButtonUp(int idx) {
            return !mouseState[idx];
        }

        private static bool IsButtonReleased(int idx) {
            return !mouseState[idx] && prevMouseState[idx];
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SynchronizeWithVerticalRetrace = false;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            gDevice = graphics.GraphicsDevice;

            IsFixedTimeStep = false;

            screenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            keyState = new bool[1024];
            prevKeyState = new bool[1024];
            mouseState = new bool[16];
            prevMouseState = new bool[16];

            trainCars = new List< Car >();
            projectiles = new List< Projectile >();
            enemies = new List< Enemy >();
            effects = new List< Effect >();
            powerups = new List< Powerup >();
            availableWeapons = new List< Weapon >();

            timeScale = 1.0f;
            state = GameState.Start;

            isSlowed = true;
            remainingSlow = 0.0f;

            playerScore = 0;

            MAX_WEAPONS = 5;

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load< SpriteFont >("mainfont");
            //font = Content.Load< SpriteFont >("font2");

            textureMap = new Dictionary< string, Texture2D >();

            background = Content.Load<Texture2D>("backgroundlarge");
            bush = Content.Load<Texture2D>("bush");

            textureMap["pause_screen"] = Content.Load< Texture2D >("pause");
            textureMap["gameover_screen"] = Content.Load< Texture2D >("gameover");
            textureMap["inv_screen"] = Content.Load< Texture2D >("inventory");
            textureMap["start_screen"] = Content.Load< Texture2D >("startscreen");
            textureMap["help_screen"] = Content.Load< Texture2D >("helpscreen");

            textureMap["rails"] = Content.Load< Texture2D >("rails");

            textureMap["powerups"] = Content.Load< Texture2D >("powerups");
            
            textureMap["weapon_mg"] = Content.Load< Texture2D >("mg_basic");
            textureMap["weapon_mg_mk2"] = Content.Load< Texture2D >("mg_basic_mk2");
            textureMap["weapon_mg_ex"] = Content.Load< Texture2D >("mg_explosive");
            textureMap["weapon_cannon"] = Content.Load< Texture2D >("cannon");
            textureMap["weapon_cannon_mk2"] = Content.Load< Texture2D >("cannon_mk2");
            textureMap["weapon_cannon_mk3"] = Content.Load< Texture2D >("cannon_mk3");
            textureMap["weapon_mgx2"] = Content.Load< Texture2D >("mg_x2");
            textureMap["weapon_mgx2_mk2"] = Content.Load< Texture2D >("mg_x2_mk2");
            textureMap["weapon_mgx2_mk3"] = Content.Load< Texture2D >("mg_x2_mk3");
            textureMap["weapon_minigun"] = Content.Load< Texture2D >("minigun");

            textureMap["bullet_default"] = Content.Load< Texture2D >("bullet0");
            textureMap["bullet1"] = Content.Load< Texture2D >("bullet1");
            textureMap["bullet2"] = Content.Load< Texture2D >("bullet2");
            textureMap["bullet3"] = Content.Load< Texture2D >("bullet3");

            textureMap["enemy1"] = Content.Load< Texture2D >("enemy1");
            textureMap["enemy2"] = Content.Load< Texture2D >("enemy2");
            textureMap["enemy3"] = Content.Load< Texture2D >("enemy3");
            textureMap["enemy4"] = Content.Load< Texture2D >("enemy4");

            textureMap["shatter"] = Content.Load< Texture2D >("shatter");
            textureMap["powerup_particles"] = Content.Load< Texture2D >("particles");
            textureMap["flash"] = Content.Load< Texture2D >("flash");
            textureMap["sparks"] = Content.Load< Texture2D >("sparks");
            textureMap["explosion0"] = Content.Load< Texture2D >("explosion");
            textureMap["explosion1"] = Content.Load< Texture2D >("explosion1");

            textureMap["locomotive"] = Content.Load< Texture2D >("locomotive");

            carSize = new Vector2(textureMap["locomotive"].Width, textureMap["locomotive"].Height);
            trackY = graphics.GraphicsDevice.Viewport.Height / 2.0f - carSize.Y / 2.0f;

            for (int i = 1; i <= MAX_CARS; i++) {
                string name = "Car" + i.ToString();
                textureMap[name] = Content.Load< Texture2D >(name);
            }

            kekw = Content.Load< Texture2D >("kekw");
            test = new Sprite(kekw, new Vector2(700.0f, 200.0f), 0.0f);
            //test.rotation = 45.0f;
            //test.origin = new Vector2(kekw.Width / 2.0f, kekw.Height / 2.0f);

            trainCars.Add(new Car(new Sprite(textureMap["locomotive"], new Vector2(0.0f, trackY), 0.0f), 0, null, 5000));
            trainCars[0].weaponPos = new Vector2(18.0f, 0.0f);
            trainCars[0].sprite.origin = new Vector2(trainCars[0].sprite.texture.Width / 2.0f, trainCars[0].sprite.texture.Height / 2.0f);
            trainCars[0].isPowered = true;

            for (int i = 1; i <= MAX_CARS; i++) {
                string name = "Car" + i.ToString();
                trainCars.Add(new Car(new Sprite(textureMap[name], new Vector2(-carSize.X * i, trackY), 0.0f), i, trainCars[i - 1]));
                trainCars[i].sprite.origin = new Vector2(trainCars[i].sprite.texture.Width / 2.0f, trainCars[i].sprite.texture.Height / 2.0f);
            }

            rails1 = new Sprite(textureMap["rails"], new Vector2(-100.0f, trackY), 0.0f);
            rails1.origin = new Vector2(50.0f, rails1.texture.Height / 2.0f);

            rails2 = new Sprite(textureMap["rails"], new Vector2(-100.0f + rails1.texture.Width, trackY), 0.0f);
            rails2.origin = new Vector2(50.0f, rails2.texture.Height / 2.0f);

            cameraTarget = new Vector2(screenSize.X / 2.0f, screenSize.Y / 2.0f);
            prevCamTarget = cameraTarget;

            //explosion = new AnimatedSprite(textureMap["explosion0"], new Vector2(300.0f, 300.0f), 0.0f);
            //explosion.defaultFrame = new Rectangle(2496, 0, 64, 64);
            //explosion.PlayAnimation(new Animation(40, new Vector2(64.0f, 64.0f), 30.0f, 0));

            availableWeapons.Add(new Weapon_ExplosiveAutoCannon());
            availableWeapons.Add(new Weapon_MGx2());
            availableWeapons.Add(new Weapon_Cannon());
            availableWeapons.Add(new Weapon_Minigun());
        }

        protected override void Update(GameTime gameTime) {
            // TODO: Add your update logic here
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            // process keyboard input
            foreach (Keys key in Enum.GetValues(typeof(Keys))) {
                if (Keyboard.GetState().IsKeyDown(key))
                    keyState[(int)key] = true;
                else
                    keyState[(int)key] = false;
            }

            // process mouse input
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                mouseState[0] = true;
            else
                mouseState[0] = false;

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                mouseState[1] = true;
            else
                mouseState[1] = false;

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                mouseState[2] = true;
            else
                mouseState[2] = false;

            #region Start
            if (state == GameState.Start) {
                Rectangle playButton = new Rectangle(380, 420, 440, 100);
                Rectangle helpButton = new Rectangle(380, 560, 440, 100);
                if (IsButtonPressed(0) && playButton.Contains(mousePos)) {
                    state = GameState.Running;
                }
                else if (IsButtonPressed(0) && helpButton.Contains(mousePos)) {
                    state = GameState.Help;
                }
            }
            #endregion
            #region Help
            else if (state == GameState.Help) {
                if (IsKeyPressed(Keys.Escape)) {
                    state = GameState.Start;
                }
            }
            #endregion
            #region Running
            else if (state == GameState.Running) {
                if (isSlowed) {
                    remainingSlow -= deltaT;
                    if (remainingSlow <= 0.0f) {
                        remainingSlow = 0.0f;
                        timeScale = 1.0f;
                        isSlowed = false;
                    }
                }

                deltaT *= timeScale;

                if (trainCars.Count == 0) {
                    state = GameState.GameOver;
                }
                else if (IsKeyPressed(Keys.Escape)) {
                    state = GameState.Paused;
                }
                else if (IsKeyPressed(Keys.Tab)) {
                    state = GameState.Menu;
                }
                else {
                    float mass = 0.0f;
                    
                    // player train movement
                    if (trainCars.Count > 0) {
                        if (trainCars[0].isPowered && !trainCars[0].isDestroyed) {
                            mass = 1.0f;
                            for (int i = 1; i < trainCars.Count; i++) {
                                if (trainCars[i].pulledBy == trainCars[i - 1]) {
                                    mass += 1.0f;
                                }
                                else break;
                            }

                            if (IsKeyHeld(Keys.D) || IsKeyHeld(Keys.W)) {
                                if (trainCars[0].speed > 0) {
                                    trainCars[0].acceleration = 180.0f / mass;
                                }
                                else {
                                    trainCars[0].acceleration = 450.0f / mass;
                                }
                            }
                            else if (IsKeyHeld(Keys.A) || IsKeyHeld(Keys.S)) {
                                if (trainCars[0].sprite.position.X > 0.0f) {
                                    trainCars[0].acceleration = -180.0f / mass;
                                }

                                if (trainCars[0].speed > 0) {
                                    trainCars[0].acceleration = -450.0f / mass;
                                }
                            }
                            else {
                                trainCars[0].acceleration = 0.0f;
                            }

                            if (trainCars[0].sprite.position.X < -100.0f) {
                                trainCars[0].sprite.position.X = -100.0f;
                                trainCars[0].speed = 0.0f;
                                trainCars[0].acceleration = 0.0f;
                            }
                        }
                    }

                    maxX = Math.Max(maxX, trainCars[0].sprite.position.X);
                    if (maxX - lastHealthIncreaseX >= 15000.0f) {
                        lastHealthIncreaseX = maxX;
                        foreach (Car c in trainCars) {
                            c.maxHealth += 500;
                            c.health = c.maxHealth;
                        }

                        spawnRate *= 0.9f;
                    }

                    // we can ram enemies
                    if (trainCars[0].speed >= 200) {
                        foreach (Enemy e in enemies) {
                            if (trainCars[0].sprite.GetAABB().Intersects(e.sprite.GetAABB())) {
                                trainCars[0].health -= 500;
                                e.OnDeath();
                            }
                        }
                    }
                    

                    // switch between active train cars
                    if (IsKeyPressed(Keys.D1)) {
                        if (!trainCars[0].isDestroyed) selectedCar = 0;
                    }
                    if (IsKeyPressed(Keys.D2) && trainCars.Count >= 2) {
                        if (!trainCars[1].isDestroyed && trainCars[1].isAttached) selectedCar = 1;
                    }
                    if (IsKeyPressed(Keys.D3) && trainCars.Count >= 3) {
                        if (!trainCars[2].isDestroyed && trainCars[2].isAttached) selectedCar = 2;
                    }
                    if (IsKeyPressed(Keys.D4) && trainCars.Count >= 4) {
                        if (!trainCars[3].isDestroyed && trainCars[3].isAttached) selectedCar = 3;
                    }
                    if (IsKeyPressed(Keys.D5) && trainCars.Count >= 5) {
                        if (!trainCars[4].isDestroyed && trainCars[4].isAttached) selectedCar = 4;
                    }

                    // shooting
                    if (!trainCars[selectedCar].isDestroyed && canShoot) {
                        if (IsButtonHeld(0)) {
                            trainCars[selectedCar].weapon.OnPrimaryFire(gameTime);
                        }
                        else {
                            trainCars[selectedCar].weapon.OnPrimaryFireReleased(gameTime);
                        }     
                    }

                    // reloading
                    if (IsKeyPressed(Keys.R)) {
                        trainCars[selectedCar].weapon.OnReload(gameTime);
                    }

                    // check if we picked up a powerup
                    if (IsButtonPressed(2)) {
                        foreach (Powerup p in powerups) {
                            if (p.sprite.GetAABB().Contains(mousePos + cameraTarget - screenSize / 2.0f)) {
                                p.OnPickup(trainCars[selectedCar]);
                            }
                        }
                    }

                    // to make sure it doesn't automatically shoot after unpausing
                    if (!canShoot && IsButtonUp(0)) {
                        canShoot = true;
                    }

                    // for debugging
                    if (IsKeyPressed(Keys.L)) {
                        state = GameState.GameOver;
                    }

                    // aim towards the mouse
                    if (selectedCar < trainCars.Count) {
                        trainCars[selectedCar].weapon.sprite.RotateTowards(mousePos + cameraTarget - screenSize / 2.0f);
                    }

                    // can't use cars that are left behind until we pick them up
                    bool flag = true;
                    for (int i = 1; i < trainCars.Count; i++) {
                        if (trainCars[i].pulledBy == null) {
                            flag = false;
                        }

                        trainCars[i].isAttached = flag;
                    }

                    // check for projectile hits
                    foreach (Projectile p in projectiles) {
                        foreach (Enemy e in enemies) {
                            if (e.sprite.GetAABB().Intersects(p.sprite.GetAABB())) {
                                p.OnHit(e);
                            }
                        }

                        foreach (Car c in trainCars) {
                            if (c.sprite.GetAABB().Intersects(p.sprite.GetAABB())) {
                                p.OnHit(c);
                            }
                        }
                    }

                    // spawn enemies off screen
                    if (enemies.Count < MAX_ENEMIES) {
                        Random rng = new Random();
                        Vector2 pos = Vector2.Zero;
                        bool super = false;

                        float spawnDelta = (float)gameTime.TotalGameTime.TotalSeconds - lastSpawnTime;
                        spawnDelta *= timeScale;

                        if (trainCars[0].isPowered) {
                            int t = rng.Next(0, 3);
                            if (t == 0) {
                                int x = rng.Next(0, (int)(1.5f * screenSize.X));
                                int y = rng.Next(0, (int)screenSize.Y);

                                pos.X = maxX + screenSize.X + 100 + x;
                                pos.Y = y;
                            }
                            else if (t == 1) {
                                int x = (int)cameraTarget.X + rng.Next(-500, 300);
                                int y = -150;

                                pos.X = x;
                                pos.Y = y;
                            }
                            else if (t == 2) {
                                int x = (int)cameraTarget.X + rng.Next(-500, 300);
                                int y = (int)screenSize.Y + 150;

                                pos.X = x;
                                pos.Y = y;
                            }
                        }
                        else {
                            super = true;
                            survival = true;

                            for (int i = enemies.Count - 1; i >= 0; i--) {
                                if ((enemies[i].sprite.position - cameraTarget).Length() > 1200.0f &&
                                    (enemies[i].attackTarget == null)) {
                                    enemies.RemoveAt(i);
                                }
                            }

                            int side = rng.Next(0, 4);

                            if (side == 0) {
                                pos.X = rng.Next(-(int)screenSize.X / 2, (int)screenSize.X / 2) + cameraTarget.X;
                                pos.Y = -200.0f;
                            }
                            else if (side == 1) {
                                pos.X = rng.Next(-(int)screenSize.X / 2, (int)screenSize.X / 2) + cameraTarget.X;
                                pos.Y = screenSize.Y + 200.0f;
                            }
                            else if (side == 2) {
                                pos.Y = rng.Next(-(int)screenSize.Y / 2, (int)screenSize.Y / 2);
                                pos.X = cameraTarget.X - screenSize.X / 2.0f - 200.0f;
                            }
                            else if (side == 3) {
                                pos.Y = rng.Next(-(int)screenSize.Y / 2, (int)screenSize.Y / 2);
                                pos.X = cameraTarget.X + screenSize.X / 2.0f + 200.0f;
                            }
                        }

                        int type = rng.Next(0, 2);
                        if (type == 0) {
                            Enemy_Exploder t = new Enemy_Exploder(pos);
                            if (super) {
                                if (spawnDelta > spawnRate / 1.5f) {
                                    t.visionRange = 1800.0f;
                                    t.maxHealth += (int)(maxX / 10000.0f) * 50;
                                    t.health = t.maxHealth;
                                    t.damage += (int)(maxX / 10000.0f) * 50;
                                    t.value *= (int)(maxX / 10000.0f) + 1;
                                    enemies.Add(t);
                                    lastSpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
                                }
                            }
                            else {
                                if (spawnDelta > spawnRate) {
                                    t.maxHealth += (int)(maxX / 10000.0f) * 50;
                                    t.health = t.maxHealth;
                                    t.damage += (int)(maxX / 10000.0f) * 50; 
                                    t.value *= (int)(maxX / 10000.0f) + 1;
                                    enemies.Add(t);
                                    lastSpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
                                }
                            }
                        }
                        else if (type == 1) {
                            Enemy_Shooter t = new Enemy_Shooter(pos);
                            if (super) {
                                if (spawnDelta > spawnRate / 1.5f) {
                                    t.visionRange = 1800.0f;
                                    t.maxHealth += (int)(maxX / 10000.0f) * 50;
                                    t.health = t.maxHealth;
                                    t.damage += (int)(maxX / 10000.0f) * 50;
                                    t.value *= (int)(maxX / 10000.0f) + 1;
                                    enemies.Add(t);
                                    lastSpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
                                }
                            }
                            else {
                                if (spawnDelta > spawnRate) {
                                    t.maxHealth += (int)(maxX / 10000.0f) * 50;
                                    t.health = t.maxHealth;
                                    t.damage += (int)(maxX / 10000.0f) * 50;
                                    t.value *= (int)(maxX / 10000.0f) + 1;
                                    enemies.Add(t);
                                    lastSpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;
                                }
                            }
                        }
                    }



                    // update everything
                    foreach (Car car in trainCars) {
                        car.Update(gameTime);
                    }

                    foreach (Enemy e in enemies) {
                        e.Update(gameTime);
                    }

                    foreach (Effect fx in effects) {
                        fx.Update(gameTime);
                    }

                    foreach (Projectile p in projectiles) {
                        p.Update(gameTime);
                    }

                    // create the illusion of infinite tracks
                    if (rails2.position.X > rails1.position.X) {
                        if (cameraTarget.X - screenSize.X / 2.0f - 100.0f > rails1.position.X + rails1.texture.Width) {
                            rails1.position.X = rails2.position.X + rails2.texture.Width;
                        }
                    }
                    else {
                        if (cameraTarget.X - screenSize.X / 2.0f - 100.0f > rails2.position.X + rails2.texture.Width) {
                            rails2.position.X = rails1.position.X + rails1.texture.Width;
                        }
                    }

                    // delete destroyed cars from the list
                    for (int i = trainCars.Count - 1; i >= 0; i--) {
                        if (trainCars[i].isDestroyed) {
                            trainCars.RemoveAt(i);
                        }
                    }

                    for (int i = 0; i < trainCars.Count; i++) {
                        // change indices of cars that aren't destroyed
                        if (selectedCar == trainCars[i].index)
                            selectedCar = i;
                        trainCars[i].index = i;

                        // we can couple with cars that are left behind
                        if (trainCars[i].pulledBy != null) {
                            if (trainCars[i].pulledBy.isDestroyed) {
                                trainCars[i].pulledBy = null;
                            }
                            else {
                                continue;
                            }
                        }

                        for (int j = 0; j < trainCars.Count; j++) {
                            if (i == j) continue;
                            if (trainCars[j].pulledBy == trainCars[i]) continue;
                            if (trainCars[i].sprite.GetAABB().Intersects(trainCars[j].sprite.GetAABB()) &&
                                trainCars[i].sprite.position.X < trainCars[j].sprite.position.X) {
                                trainCars[i].pulledBy = trainCars[j];
                                trainCars[i].sprite.position.X = trainCars[j].sprite.position.X - trainCars[j].sprite.texture.Width;
                            }
                        }
                    }

                    // delete destroyed projectiles
                    for (int i = projectiles.Count - 1; i >= 0; i--) {
                        if (!projectiles[i].isActive) {
                            projectiles.RemoveAt(i);
                        }
                    }

                    // delete destroyed enemies
                    for (int i = enemies.Count - 1; i >= 0; i--) {
                        if (!enemies[i].isAlive) {
                            enemies[i].OnDeath();
                            enemies.RemoveAt(i);
                        }
                        else if ((enemies[i].sprite.position - cameraTarget).Length() > 3000.0f) {
                            enemies.RemoveAt(i);
                        }
                    }

                    // delete finished effect animations
                    for (int i = effects.Count - 1; i >= 0; i--) {
                        if (!effects[i].isActive) {
                            effects.RemoveAt(i);
                        }
                    }

                    // if the selected car is destroyed, select another one
                    if (selectedCar >= trainCars.Count ||
                        trainCars[selectedCar].isDestroyed) {
                        for (int i = 0; i < trainCars.Count; i++) {
                            if (!trainCars[i].isDestroyed) {
                                selectedCar = i;
                                break;
                            }
                        }
                    }

                    // move camera
                    Vector2 nextCamTarget = cameraTarget;
                    if (trainCars.Count > 0) {
                        Vector2 target = trainCars[0].sprite.position;
                        cameraTarget.X += (target.X - cameraTarget.X) * 7.0f * deltaT;
                        cameraTarget.Y += (target.Y - cameraTarget.Y) * 7.0f * deltaT;
                    }

                    if (cameraTarget.X - screenSize.X / 2.0f < 0.0f) {
                        cameraTarget.X = screenSize.X / 2.0f;
                    }
                }
            }
            #endregion
            #region Paused
            else if (state == GameState.Paused) {
                if ((IsButtonPressed(0) && (mousePos - new Vector2(600.0f, 514.0f)).Length() < 40.0f) || 
                    IsKeyPressed(Keys.Escape)) {
                    state = GameState.Running;
                    canShoot = false;
                }
            }
            #endregion
            #region GameOver 
            else if (state == GameState.GameOver) {
                if ((IsButtonPressed(0) && (mousePos - new Vector2(680.0f, 514.0f)).Length() < 40.0f) ||
                    IsKeyPressed(Keys.Escape)) {
                    // exit
                    Exit();
                }
                else if ((IsButtonPressed(0) && (mousePos - new Vector2(520.0f, 514.0f)).Length() < 40.0f) ||
                    // reset the game
                    IsKeyPressed(Keys.Enter)) {
                    keyState = new bool[1024];
                    prevKeyState = new bool[1024];
                    mouseState = new bool[16];
                    prevMouseState = new bool[16];

                    trainCars = new List< Car >();
                    projectiles = new List< Projectile >();
                    enemies = new List< Enemy >();
                    effects = new List< Effect >();
                    powerups = new List< Powerup >();
                    availableWeapons = new List< Weapon >();

                    playerScore = 0;

                    state = GameState.Running;

                    canShoot = false;

                    timeScale = 1.0f;
                    isSlowed = false;
                    remainingSlow = 0.0f;

                    trainCars.Add(new Car(new Sprite(textureMap["locomotive"], new Vector2(0.0f, trackY), 0.0f), 0, null, 5000));
                    trainCars[0].weaponPos.X = 18.0f;
                    trainCars[0].sprite.origin = new Vector2(trainCars[0].sprite.texture.Width / 2.0f, trainCars[0].sprite.texture.Height / 2.0f);
                    trainCars[0].isPowered = true;

                    for (int i = 1; i <= MAX_CARS; i++) {
                        string name = "Car" + i.ToString();
                        trainCars.Add(new Car(new Sprite(textureMap[name], new Vector2(-carSize.X * i, trackY), 0.0f), i, trainCars[i - 1]));
                        trainCars[i].sprite.origin = new Vector2(trainCars[i].sprite.texture.Width / 2.0f, trainCars[i].sprite.texture.Height / 2.0f);
                    }

                    rails1 = new Sprite(textureMap["rails"], new Vector2(-100.0f, trackY), 0.0f);
                    rails1.origin = new Vector2(50.0f, rails1.texture.Height / 2.0f);

                    rails2 = new Sprite(textureMap["rails"], new Vector2(-100.0f + rails1.texture.Width, trackY), 0.0f);
                    rails2.origin = new Vector2(50.0f, rails2.texture.Height / 2.0f);

                    cameraTarget = new Vector2(screenSize.X / 2.0f, screenSize.Y / 2.0f);
                    prevCamTarget = cameraTarget;
                }
            }
            #endregion
            #region Menu
            else if (state == GameState.Menu) {
                if (IsKeyPressed(Keys.Escape) || IsKeyPressed(Keys.Tab)) {
                    canShoot = false;
                    state = GameState.Running;
                }
                else {
                    if (IsKeyPressed(Keys.D1)) {
                        if (!trainCars[0].isDestroyed) selectedCar = 0;
                    }
                    if (IsKeyPressed(Keys.D2) && trainCars.Count >= 2) {
                        if (!trainCars[1].isDestroyed) selectedCar = 1;
                    }
                    if (IsKeyPressed(Keys.D3) && trainCars.Count >= 3) {
                        if (!trainCars[2].isDestroyed) selectedCar = 2;
                    }
                    if (IsKeyPressed(Keys.D4) && trainCars.Count >= 4) {
                        if (!trainCars[3].isDestroyed) selectedCar = 3;
                    }
                    if (IsKeyPressed(Keys.D5) && trainCars.Count >= 5) {
                        if (!trainCars[4].isDestroyed) selectedCar = 4;
                    }

                    float y = 140.0f;

                    for (int i = 0; i < availableWeapons.Count; i++) {
                        if (IsButtonPressed(0) && availableWeapons[i].sprite.GetAABB(2.5f).Contains(mousePos) &&
                            !trainCars[selectedCar].weapon.isReloading) {
                            Weapon tmp = trainCars[selectedCar].weapon;
                            trainCars[selectedCar].Equip(availableWeapons[i]);
                            availableWeapons[i] = tmp;
                        }

                        availableWeapons[i].sprite.position = new Vector2(845.0f, y);
                        availableWeapons[i].sprite.rotation = 0.0f;
                        availableWeapons[i].sprite.parent = null;
                        availableWeapons[i].sprite.anchor = Vector2.Zero;

                        y += 100.0f;
                    }
                }
            }
            #endregion

            // set previous key state to current
            foreach (Keys key in Enum.GetValues(typeof(Keys))) {
                prevKeyState[(int)key] = keyState[(int)key];
            }

            // same for mouse state
            for (int i = 0; i <= 2; i++) {
                prevMouseState[i] = mouseState[i];
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (state == GameState.Start) {
                spriteBatch.Draw(textureMap["start_screen"], Vector2.Zero, Color.White);
                spriteBatch.DrawString(font, "Play", new Vector2(530.0f, 445.0f), Color.Black, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 1);
                spriteBatch.DrawString(font, "Help", new Vector2(530.0f, 585.0f), Color.Black, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 1);
                spriteBatch.DrawString(font, "Weaponized Express", new Vector2(325.0f, 270.0f), Color.Black, 0.0f, Vector2.Zero, 1.7f, SpriteEffects.None, 1);
            }
            else if (state == GameState.Help) {
                spriteBatch.Draw(textureMap["help_screen"], Vector2.Zero, Color.White);
                spriteBatch.DrawString(font, helpText, new Vector2(80.0f, 85.0f), Color.Black);
            }
            else if (state == GameState.Menu) {
                spriteBatch.Draw(textureMap["inv_screen"], Vector2.Zero, Color.White);
                spriteBatch.DrawString(font, "MODIFY YOUR MACHINE", new Vector2(400.0f, 20.0f), Color.Black);
                float y = 110.0f;
                for (int i = 0; i < trainCars.Count; i++) {
                    Color col = (i == selectedCar ? Color.Red : Color.Black);

                    string text = (trainCars[i].isPowered ? "Locomotive (" : "Car (") + (i + 1).ToString() + ")";
                    if (!trainCars[i].isAttached && !trainCars[i].isPowered) {
                        text += " (NOT ATTACHED)";
                    }

                    spriteBatch.DrawString(font, text, new Vector2(50.0f, y), col);
                    y += 40.0f;

                    text = "Health: " + trainCars[i].health.ToString() + "/" + trainCars[i].maxHealth.ToString();
                    spriteBatch.DrawString(font, text, new Vector2(55.0f, y), col);
                    y += 40.0f;

                    text = "Weapon: " + trainCars[i].weapon.name + " (Ammo " + trainCars[i].weapon.currentAmmo +
                        "/" + trainCars[i].weapon.maxAmmo + ") (" + trainCars[i].weapon.tier + ")";
                    spriteBatch.DrawString(font, text, new Vector2(55.0f, y), col);
                    y += 40.0f;

                    y += 40.0f;
                }

                y = 110.0f;
                spriteBatch.DrawString(font, "Available weapons: ", new Vector2(830.0f, y), Color.DarkGreen);
                y += 40.0f;

                for (int i = 0; i < availableWeapons.Count; i++) {
                    Vector2 tmp = availableWeapons[i].sprite.origin;
                    availableWeapons[i].sprite.origin = Vector2.Zero;
                    float scale = 2.5f;

                    // - no you can't just check if a gun is big using an if,
                    //   you should set the icon scale in every Weapon_xxx class!!
                    //
                    // - haha spaghetti code go brrrrr
                    if (availableWeapons[i].internalName == "minigun" ||
                        (availableWeapons[i].internalName == "mg_x2" && availableWeapons[i].tier >= 3) ||
                        (availableWeapons[i].internalName == "cannon" && availableWeapons[i].tier >= 3))
                        scale = 1.0f;

                    availableWeapons[i].sprite.Draw(spriteBatch, scale, true);
                    availableWeapons[i].sprite.origin = tmp;

                    y += 100.0f;
                }
            }
            else {
                int startX = 100 * (((int)(cameraTarget.X - screenSize.X / 2.0f - 100.0f)) / 100);
                int startY = 0;

                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 15; j++) {
                        int x = startX + 100 * j;
                        int y = startY + 100 * i;
                        int idx = ((x * y + 69) % 17) % 4;
                        int bgType = (x / 10000) % 4;
                        //if (i < 2 || i > 6) {
                        //    Rectangle rect = new Rectangle(100 * bgType, 0, 100, 100);
                        //    spriteBatch.Draw(bush, new Vector2((float)x - 50.0f, (float)y - 50.0f) - cameraTarget + screenSize / 2.0f, rect, Color.White);
                        //}
                        //else {
                        Rectangle rect = new Rectangle(idx * 100, 100 * bgType, 100, 100);
                        spriteBatch.Draw(background, new Vector2((float)x - 50.0f, (float)y - 50.0f) - cameraTarget + screenSize / 2.0f, rect, Color.White);
                        //}
                    }
                }

                rails1.Draw(spriteBatch);
                rails2.Draw(spriteBatch);

                foreach (Car car in trainCars) {
                    car.Draw(spriteBatch);
                }

                foreach (Enemy e in enemies) {
                    e.Draw(spriteBatch);
                }

                foreach (Projectile proj in projectiles) {
                    proj.Draw(spriteBatch);
                }

                foreach (Powerup p in powerups) {
                    p.Draw(spriteBatch);
                }

                foreach (Effect e in effects) {
                    e.Draw(spriteBatch);
                }

                string scoreText = "Score: " + playerScore.ToString();
                spriteBatch.DrawString(font, scoreText, new Vector2(20.0f, 20.0f), Color.Black);
                string distText = "Distance: " + ((int)(trainCars[0].sprite.position.X / 10.0f)).ToString() + " m";
                spriteBatch.DrawString(font, distText, new Vector2(20.0f, 50.0f), Color.Black);
                if (selectedCar < trainCars.Count) {
                    spriteBatch.DrawString(font, trainCars[selectedCar].weapon.name, new Vector2(20.0f, 700.0f), Color.Black);
                    string ammoText = trainCars[selectedCar].weapon.currentAmmo.ToString() + " / " +
                        trainCars[selectedCar].weapon.maxAmmo;
                    if (trainCars[selectedCar].weapon.isReloading)
                        ammoText = "(RELOADING)";

                    spriteBatch.DrawString(font, ammoText, new Vector2(80.0f, 740.0f), Color.Black, 0.0f,
                        new Vector2(ammoText.Length * 10.0f / 2.0f, 0.0f), 1.0f, SpriteEffects.None, 1);
                }

                if (state == GameState.Paused) {
                    spriteBatch.Draw(textureMap["pause_screen"], Vector2.Zero, Color.White);
                    spriteBatch.DrawString(font, "P A U S E D", new Vector2(435.0f, 330.0f), Color.Black, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 1);
                }
                else if (state == GameState.GameOver) {
                    spriteBatch.Draw(textureMap["gameover_screen"], Vector2.Zero, Color.White);
                    spriteBatch.DrawString(font, "G A M E   O V E R", new Vector2(350.0f, 280.0f), Color.Black, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 1);
                    string text = "Final score: " + playerScore.ToString();
                    spriteBatch.DrawString(font, text, new Vector2(460.0f, 360.0f), Color.Black);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
