using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Prokaryotic_Showdown
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D cursorTex;
        TBSManager manager;

        enum gameState
        {
            Splash,
            Start,
            Playing,
            Options,
            OptionsDifficulty,
            OptionsResolution,
            Paused,
            GameOver
        }

        const int BUTTON_HEIGHT = 50;
        //start menu
        Button start, exit, options, resume;

        //options menu
        Button difficulty, easy, medium, hard;
        Button resolution, small, mediumRes, large;
        Button fullScreen;

        //game over menu
        Button PlayAgain, mainMenu, showScore, win;

        SoundEffect menuSound;
        SoundEffect backgroundMusic;
        SpriteFont font;
        Texture2D bckTex;
        Rectangle backg;
        MouseState oldMouse;
        KeyboardState oldKeys;
        Vector2 cursorPos;
        gameState curState = gameState.Start;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = false;

            GameOptions.setGraphics(graphics);
            GameOptions.setScreen(800, 600);
            GameOptions.setDifficulty(GameOptions.GameDifficulties.Easy);

            backg = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bckTex = Content.Load<Texture2D>("alienmetal2");
            menuSound = Content.Load<SoundEffect>("select");
            backgroundMusic = Content.Load<SoundEffect>("backgroundMusic");

            start = new Button(0, Window.ClientBounds.Height / 2, Window.ClientBounds.Width, BUTTON_HEIGHT, 3);
            resume = new Button(0, Window.ClientBounds.Height / 2, Window.ClientBounds.Width, BUTTON_HEIGHT, 3);
            exit = new Button(0, (Window.ClientBounds.Height / 2) + 400, Window.ClientBounds.Width, BUTTON_HEIGHT, 5);
            options = new Button(0, (Window.ClientBounds.Height / 2) + 100, Window.ClientBounds.Width, BUTTON_HEIGHT, 4);

            start.setFont(Content.Load<SpriteFont>("MenuFont"));
            resume.setFont(Content.Load<SpriteFont>("MenuFont"));
            exit.setFont(Content.Load<SpriteFont>("MenuFont"));
            options.setFont(Content.Load<SpriteFont>("MenuFont"));

            start.setTexture(Content.Load<Texture2D>("button"));
            resume.setTexture(Content.Load<Texture2D>("button"));
            exit.setTexture(Content.Load<Texture2D>("button"));
            options.setTexture(Content.Load<Texture2D>("button"));

            start.setText("Start Game");
            resume.setText("Resume");
            exit.setText("Exit Game");
            options.setText("Options");

            difficulty = new Button(0, 200, GameOptions.getWidth(), BUTTON_HEIGHT, 1);
            resolution = new Button(0, 300, GameOptions.getWidth(), BUTTON_HEIGHT, 2);
            fullScreen = new Button(0, 400, GameOptions.getWidth(), BUTTON_HEIGHT, 3);

            difficulty.setFont(Content.Load<SpriteFont>("MenuFont"));
            resolution.setFont(Content.Load<SpriteFont>("MenuFont"));
            fullScreen.setFont(Content.Load<SpriteFont>("MenuFont"));

            difficulty.setTexture(Content.Load<Texture2D>("button"));
            resolution.setTexture(Content.Load<Texture2D>("button"));
            fullScreen.setTexture(Content.Load<Texture2D>("button"));

            difficulty.setText("Difficulty");
            resolution.setText("Resolution");
            fullScreen.setText("Toggle Fullscreen");

            easy = new Button(0, 200, GameOptions.getWidth(), BUTTON_HEIGHT, 1);
            medium = new Button(0, 300, GameOptions.getWidth(), BUTTON_HEIGHT, 2);
            hard = new Button(0, 400, GameOptions.getWidth(), BUTTON_HEIGHT, 3);

            easy.setFont(Content.Load<SpriteFont>("MenuFont"));
            medium.setFont(Content.Load<SpriteFont>("MenuFont"));
            hard.setFont(Content.Load<SpriteFont>("MenuFont"));

            easy.setTexture(Content.Load<Texture2D>("button"));
            medium.setTexture(Content.Load<Texture2D>("button"));
            hard.setTexture(Content.Load<Texture2D>("button"));

            easy.setText("Easy");
            medium.setText("Medium");
            hard.setText("Hard");

            small = new Button(0, 200, GameOptions.getWidth(), BUTTON_HEIGHT, 1);
            mediumRes = new Button(0, 300, GameOptions.getWidth(), BUTTON_HEIGHT, 2);
            large = new Button(0, 400, GameOptions.getWidth(), BUTTON_HEIGHT, 3);

            small.setFont(Content.Load<SpriteFont>("MenuFont"));
            mediumRes.setFont(Content.Load<SpriteFont>("MenuFont"));
            large.setFont(Content.Load<SpriteFont>("MenuFont"));

            small.setTexture(Content.Load<Texture2D>("button"));
            mediumRes.setTexture(Content.Load<Texture2D>("button"));
            large.setTexture(Content.Load<Texture2D>("button"));

            small.setText("800x600");
            mediumRes.setText("1280x1024");
            large.setText("1600x1200");

            win = new Button(0, 200, GameOptions.getWidth(), BUTTON_HEIGHT, 3);
            PlayAgain = new Button(0, 400, GameOptions.getWidth(), BUTTON_HEIGHT, 1);
            mainMenu = new Button(0, 500, GameOptions.getWidth(), BUTTON_HEIGHT, 2);
            showScore = new Button(0, 300, GameOptions.getWidth(), BUTTON_HEIGHT, 0);

            PlayAgain.setFont(Content.Load<SpriteFont>("MenuFont"));
            mainMenu.setFont(Content.Load<SpriteFont>("MenuFont"));
            showScore.setFont(Content.Load<SpriteFont>("MenuFont"));
            win.setFont(Content.Load<SpriteFont>("MenuFont"));

            PlayAgain.setTexture(Content.Load<Texture2D>("button"));
            mainMenu.setTexture(Content.Load<Texture2D>("button"));
            showScore.setTexture(Content.Load<Texture2D>("button"));
            win.setTexture(Content.Load<Texture2D>("button"));

            PlayAgain.setText("Play Again");
            mainMenu.setText("Back to Main Menu");
            showScore.setText(TBSManager.getScore().ToString());
            win.setText("WE HAVE A WINNER!");

            oldMouse = Mouse.GetState();
            oldKeys = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("ScoreFont");
            menuSound = Content.Load<SoundEffect>("Select");
            backgroundMusic = Content.Load<SoundEffect>("backgroundMusic");
            cursorTex = Content.Load<Texture2D>("Cursor");
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MouseState mouse = Mouse.GetState();
            KeyboardState keys = Keyboard.GetState();
            cursorPos = new Vector2(mouse.X, mouse.Y);
            switch (curState)
            {
                case gameState.Start:
                    gameStart(gameTime, ref mouse, ref keys);
                    break;
                case gameState.Playing:
                    gamePlaying(gameTime, ref mouse, ref keys, ref oldMouse, ref oldKeys);
                    break;
                case gameState.Options:
                    Options(gameTime, ref mouse, ref keys);
                    break;
                case gameState.OptionsResolution:
                    resOptions(gameTime, ref mouse, ref keys);
                    break;
                case gameState.OptionsDifficulty:
                    diffOptions(gameTime, ref mouse, ref keys);
                    break;
                case gameState.Paused:
                    gamePaused(gameTime, ref mouse, ref keys);
                    break;
                case gameState.GameOver:
                    gameOver(gameTime, ref mouse, ref keys);
                    break;
                default:
                    break;
            }

            oldMouse = mouse;
            oldKeys = keys;

            base.Update(gameTime);
        }

        private void gameOver(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys)
        {
            PlayAgain.mouseOver(mouse);
            mainMenu.mouseOver(mouse);
            showScore.setText("Your Score: " + TBSManager.getScore().ToString());
            if (GameOptions.m_win == true)
                win.setText("We Have Winner!");
            else
                win.setText("GAME OVER!");

            if (PlayAgain.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                newGame();
            }

            if (mainMenu.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                curState = gameState.Start;
            }
        }

        private void diffOptions(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys)
        {
            easy.mouseOver(mouse);
            medium.mouseOver(mouse);
            hard.mouseOver(mouse);

            if (easy.pressed(mouse, oldMouse))
            {
                GameOptions.setDifficulty(GameOptions.GameDifficulties.Easy);
                menuSound.Play();
                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }

           if (medium.pressed(mouse, oldMouse))
            {
                GameOptions.setDifficulty(GameOptions.GameDifficulties.Medium);
                menuSound.Play();
                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }

            if (hard.pressed(mouse, oldMouse))
            {
                GameOptions.setDifficulty(GameOptions.GameDifficulties.Hard);
                menuSound.Play();
                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }
        }
        private void resOptions(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys)
        {
            small.mouseOver(mouse);
            mediumRes.mouseOver(mouse);
            large.mouseOver(mouse);

            if (small.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                GameOptions.setScreen(800, 600);
                
                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }

            if (mediumRes.pressed(mouse, oldMouse))
            {
                menuSound.Play();
               
                GameOptions.setScreen(800, 600);
                GameOptions.toggleFullScreen();
                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }

            if (large.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                GameOptions.setScreen(800, 600);
                GameOptions.toggleFullScreen();
                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }
        }

        private void Options(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys)
        {
            if (!GameOptions.isGamePlaying())
            {
                difficulty.mouseOver(mouse);
            }

            resolution.mouseOver(mouse);
            fullScreen.mouseOver(mouse);

            if (difficulty.pressed(mouse, oldMouse) && !GameOptions.isGamePlaying())
            {
                menuSound.Play();
                curState = gameState.OptionsDifficulty;
            }

            if (resolution.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                curState = gameState.OptionsResolution;
            }

            if (fullScreen.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                GameOptions.toggleFullScreen();

                if (GameOptions.isGamePlaying())
                {
                    curState = gameState.Paused;
                }
                else
                {
                    curState = gameState.Start;
                }
            }
        }

        private void gameStart(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys)
        {
            start.mouseOver(mouse);
            exit.mouseOver(mouse);
            options.mouseOver(mouse);

            if (start.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                newGame();
            }

           if (exit.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                this.Exit();
            }

            if (options.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                curState = gameState.Options;
            }
        }

        private void newGame()
        {
            manager = null;
            manager = TBSManager.Instance(Window.ClientBounds.Width, Window.ClientBounds.Height, Content);
            TBSManager.NewGame();

            SoundEffectInstance instance = backgroundMusic.CreateInstance();
            instance.IsLooped = true;
            backgroundMusic.Play();

            curState = gameState.Playing;
        }

        private void gamePaused(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys)
        {
            resume.mouseOver(mouse);
            exit.mouseOver(mouse);
            options.mouseOver(mouse);

            if (resume.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                curState = gameState.Playing;
            }

            if (exit.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                this.Exit();
            }

            if (options.pressed(mouse, oldMouse))
            {
                menuSound.Play();
                curState = gameState.Options;
            }
        }

        const int SLOWCAM = 100;
        const int FASTCAM = 50;
        const int FASTCAM_SPEED = 15;
        const int SLOWCAM_SPEED = 5;

        private void gamePlaying(GameTime gameTime, ref MouseState mouse, ref KeyboardState keys, ref MouseState oldmouse, ref KeyboardState oldkeys)
        {
            if (keys.IsKeyDown(Keys.Escape) && oldKeys.IsKeyUp(Keys.Escape))
            {
                curState = gameState.Paused;
            }
          
            if (mouse.X < SLOWCAM)
            {
                if (mouse.X < FASTCAM)
                {
                    manager.setCameraSpeed(FASTCAM_SPEED);
                    manager.moveCamLeft();
                }
                else
                {
                    manager.setCameraSpeed(SLOWCAM_SPEED);
                    manager.moveCamLeft();
                }
            }
            else if (mouse.X > (GameOptions.getWidth() - SLOWCAM))
            {
                if (mouse.X > (GameOptions.getWidth() - FASTCAM))
                {
                    manager.setCameraSpeed(FASTCAM_SPEED);
                    manager.moveCamRight();
                }
                else
                {
                    manager.setCameraSpeed(SLOWCAM_SPEED);
                    manager.moveCamRight();
                }
            }

            if (mouse.Y < SLOWCAM)
            {
                if (mouse.Y < FASTCAM)
                {
                    manager.setCameraSpeed(FASTCAM_SPEED);
                    manager.moveCamUp();
                }
                else
                {
                    manager.setCameraSpeed(SLOWCAM_SPEED);
                    manager.moveCamUp();
                }
            }
            else if (mouse.Y > (GameOptions.getHeight() - SLOWCAM))
            {
                if (mouse.Y > (GameOptions.getHeight() - FASTCAM))
                {
                    manager.setCameraSpeed(FASTCAM_SPEED);
                    manager.moveCamDown();
                }
                else
                {
                    manager.setCameraSpeed(SLOWCAM_SPEED);
                    manager.moveCamDown();
                }
            }

            manager.update(new Vector2(mouse.X, mouse.Y), mouse.LeftButton, mouse.RightButton, keys, oldmouse, oldkeys, gameTime);

            if (GameOptions.isGameOver())
            {
                curState = gameState.GameOver;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            switch (curState)
            {
                case gameState.Splash:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    break;
                case gameState.Playing:
                    manager.draw(spriteBatch);
                    break;
                case gameState.Start:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    start.draw(spriteBatch);
                    exit.draw(spriteBatch);
                    options.draw(spriteBatch);
                    break;
                case gameState.Options:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    if (!GameOptions.isGamePlaying())
                    {
                        difficulty.draw(spriteBatch);
                    }
                    resolution.draw(spriteBatch);
                    fullScreen.draw(spriteBatch);
                    break;
                case gameState.OptionsDifficulty:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    easy.draw(spriteBatch);
                    medium.draw(spriteBatch);
                    hard.draw(spriteBatch);
                    break;
                case gameState.OptionsResolution:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    small.draw(spriteBatch);
                    mediumRes.draw(spriteBatch);
                    large.draw(spriteBatch);
                    break;
                case gameState.Paused:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    resume.draw(spriteBatch);
                    exit.draw(spriteBatch);
                    options.draw(spriteBatch);
                    break;
                case gameState.GameOver:
                    spriteBatch.Begin();
                    spriteBatch.Draw(bckTex, backg, Color.White);
                    spriteBatch.End();
                    
                    PlayAgain.draw(spriteBatch);
                    mainMenu.draw(spriteBatch);
                    win.draw(spriteBatch);
                    showScore.draw(spriteBatch);
                    break;
            }

            spriteBatch.Begin();
            spriteBatch.Draw(cursorTex, new Rectangle((int)cursorPos.X,(int)cursorPos.Y,24,24), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}