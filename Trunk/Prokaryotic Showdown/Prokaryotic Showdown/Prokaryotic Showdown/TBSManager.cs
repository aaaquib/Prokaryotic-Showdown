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
using System.Threading;

namespace Prokaryotic_Showdown
{
    public class TBSManager
    {
        private static ContentManager m_content;
        static TBSManager instance = null;
        static Texture2D mDottedLine;
        private static Rectangle mSelectionBox = new Rectangle(0, 0, 0, 0);

        private static List<Bacterium> bacteria = new List<Bacterium>();
        private static List<BacteriumSoldier> soldier = new List<BacteriumSoldier>();
        private static List<BacteriumRanger> rangers = new List<BacteriumRanger>();
        private static List<Material> worldResources = new List<Material>();
        private static List<Material> worldTrees = new List<Material>();
        private static List<Virus> viruses = new List<Virus>();
        private static List<Unit> units = new List<Unit>();
        private static List<Unit> virusTargets = new List<Unit>();

        private static Random rand = new Random(DateTime.Now.Millisecond);
        public static Resources curResources = GameOptions.getStartResources();
        private static int m_score = 0;

        private static int m_worldWidth, m_worldHeight;
        public static Rectangle world;
        public static Camera2d cam;

        private static int m_windowWidth, m_windowHeight;

        public void setWindowLimits(int Width, int Height)
        {
            m_windowWidth = Width;
            m_windowHeight = Height;
        }

        public void setContentManger(ContentManager content)
        {
            m_content = content;
        }

        public static int getWorldWidth()
        {
            return world.Width;
        }

        public static int getWorldHeight()
        {
            return world.Height;
        }

        public static void NewGame()
        {
            bacteria = new List<Bacterium>();
            soldier = new List<BacteriumSoldier>();

            worldResources = new List<Material>();

            viruses = new List<Virus>();
            units = new List<Unit>();
            virusTargets = new List<Unit>();

            rand = new Random(DateTime.Now.Millisecond);

            setFont(m_content.Load<SpriteFont>("ScoreFont"));
            setWorld(2000, 2000);
            setBackground(m_content.Load<Texture2D>("background2"));
            mDottedLine = m_content.Load<Texture2D>("DottedLine");

            curResources = GameOptions.getStartResources();

            generateWorld(5, 5, 5);
            m_score = 0;
            GameOptions.startGame();
        }

        public static TBSManager Instance(int WindowWidth, int WindowHeight, ContentManager content)
        {
            instance = new TBSManager();
            instance.setContentManger(content);
            instance.setWindowLimits(WindowWidth, WindowHeight);

            cam = new Camera2d();
            cam.Pos = new Vector2(GameOptions.getWidth()/2, GameOptions.getHeight()/2);

            return instance;
        }

        public static void generateWorld(int numOfFood, int numOfWater,int numOfTrees)
        {
            for (int i = 0; i < 10; i++)
            {
                generateFood(numOfFood);
                generateWater(numOfWater);
                generateTrees(numOfTrees);
            }

            createSoldier(200, 200, 1);
            createBacteria(150, 150, 1);
            createBacteria(200, 150,1);
            createRanger(100, 100);
            createRanger(250, 100);

        }

        public static void setWorld(int Width, int Height)
        {
            m_worldWidth = Width;
            m_worldHeight = Height;
            world = new Rectangle(0, 0, Width, Height);
        }

        
        private static void generateFood(int numOfFood)
        {
            Material curMat;
            Rectangle range = new Rectangle(rand.Next(0, m_worldWidth-100), rand.Next(0, m_worldHeight-100), 100, 100);

            for (int i = 0; i < numOfFood; i++)
            {
                curMat = new Material(rand.Next((int)range.X, (int)range.X + range.Width), rand.Next((int)range.Y, (int)range.Y + range.Height), 5, 5, Material.type.Food);
                curMat.setTexture(m_content.Load<Texture2D>("food"));
                worldResources.Add(curMat);
            }
        }

        private static void generateWater(int numOfWater)
        {
            Material curMat;
            Rectangle range = new Rectangle(rand.Next(0, m_worldWidth-100), rand.Next(0, m_worldHeight-100), 100, 100);

            for (int i = 0; i < numOfWater; i++)
            {
                curMat = new Material(rand.Next((int)range.X, (int)range.X + range.Width), rand.Next((int)range.Y, (int)range.Y + range.Height), 20, 20, Material.type.Water);
                curMat.setTexture(m_content.Load<Texture2D>("water"));
                worldResources.Add(curMat);
            }
        }

        private static void generateTrees(int numOfTrees)
        {
            Material curMat;
            Rectangle range = new Rectangle(rand.Next(200, m_worldWidth-200), rand.Next(200, m_worldHeight-200), 200, 200);

            for (int i = 0; i < numOfTrees; i++)
            {
                curMat = new Material(rand.Next((int)range.X, (int)range.X + range.Width), rand.Next((int)range.Y, (int)range.Y + range.Height), 20, 20, Material.type.Tree);
                curMat.setTexture(m_content.Load<Texture2D>("tree"));
                worldTrees.Add(curMat);
            }
        }

        private static void generateViruses(int numOfViruses)
        {
            Virus curVirus;
            Rectangle range = new Rectangle(rand.Next(0, m_worldWidth), rand.Next(0, m_worldHeight), 100, 100);

            for (int i = 0; i < numOfViruses; i++)
            {
                curVirus = new Virus(rand.Next((int)range.X, (int)range.X + range.Width), rand.Next((int)range.Y, (int)range.Y + range.Height), 20, 20);
                curVirus.setTexture(m_content.Load<Texture2D>("virus"));
                curVirus.setHealthBar(m_content.Load<Texture2D>("HealthBar"));
                viruses.Add(curVirus);

                Thread.Sleep(10);
            }
        }

        private static Texture2D m_texture;

        public static void setBackground(Texture2D texture)
        {
            m_texture = texture;
        }

        private static GameTime gameTime;
        private static TimeSpan elapsedTime;
        private static TimeSpan oneSecond = new TimeSpan(0, 0, 1);

        public void update(Vector2 mousePos, ButtonState LbtnState, ButtonState RbtnState, KeyboardState keyState,MouseState oldmouse,KeyboardState oldkeyState, GameTime time)
        {
            Vector2 winMousePos = Vector2.Transform(mousePos, Matrix.Invert(cam.get_transformation(GameOptions.getGraphics().GraphicsDevice)));
            Rectangle aSelection = new Rectangle(0, 0, 0, 0);
            gameTime = time;

            elapsedTime += time.ElapsedGameTime;

            if (elapsedTime.Milliseconds > 900)
            {
                elapsedTime.Add(oneSecond);
            }
            
            if (elapsedTime.Seconds > GameOptions.getVirusAddInterval())
            {

                generateViruses(GameOptions.getNumOfVirusesToAdd());

                elapsedTime = elapsedTime.Subtract(elapsedTime);
            }

            if (oldkeyState.IsKeyDown(Keys.S) && keyState.IsKeyUp(Keys.S))
            {
                if(curResources >= Bacterium.getRequiredResources())
                    createSoldier(world.X + 100, world.Y + 100,1);
            }
           
            if (oldkeyState.IsKeyDown(Keys.C) && keyState.IsKeyUp(Keys.C))
            {
                if (curResources >= Bacterium.getRequiredResources())
                    createBacteria(world.X + 100, world.Y + 100, 1);
            }

            if (oldkeyState.IsKeyDown(Keys.R) && keyState.IsKeyUp(Keys.R))
            {
                if (curResources >= Bacterium.getRequiredResources())
                    createRanger(world.X + 100, world.Y + 100);
            }

            if (bacteria.Count <= 0 && soldier.Count <= 0)
                GameOptions.gameOver();

            if (LbtnState == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released)
            {
                //Set the starting location for the selectiong box to the current location
                //where the Left button was initially clicked.
                mSelectionBox = new Rectangle((int)winMousePos.X, (int)winMousePos.Y, 0, 0);
            }
            //If the user is still holding the Left button down, then continue to re-size the 
            //selection square based on where the mouse has currently been moved to.
            if (LbtnState == ButtonState.Pressed)
            {
                //The starting location for the selection box remains the same, but increase (or decrease)
                //the size of the Width and Height but taking the current location of the mouse minus the
                //original starting location.
                mSelectionBox = new Rectangle(mSelectionBox.X, mSelectionBox.Y, (int)winMousePos.X - mSelectionBox.X, (int)winMousePos.Y - mSelectionBox.Y);
            }
            if (LbtnState == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed)
            {

                if (mSelectionBox.Width >= 0)
                {
                    if (mSelectionBox.Height >= 0)
                    {
                        aSelection = new Rectangle(mSelectionBox.X, mSelectionBox.Y, mSelectionBox.Width, mSelectionBox.Height);
                    }
                    else
                        aSelection = new Rectangle(mSelectionBox.X, mSelectionBox.Y + mSelectionBox.Height, mSelectionBox.Width, -mSelectionBox.Height);
                }

                else
                {
                    if (mSelectionBox.Height >= 0)
                    {
                        aSelection = new Rectangle(mSelectionBox.X + mSelectionBox.Width, mSelectionBox.Y, -mSelectionBox.Width, mSelectionBox.Height);
                    }
                    else
                    {
                        aSelection = new Rectangle(mSelectionBox.X + mSelectionBox.Width, mSelectionBox.Y + mSelectionBox.Height, -mSelectionBox.Width, -mSelectionBox.Height);
                    }
                }

                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].getRect().Intersects(aSelection))
                    {
                        units[i].selectUnit();
                        units[i].move(units, new Vector2(units[i].X, units[i].Y));
                    }
                    else
                    {
                        units[i].deselectUnit();
                    }
                }
                mSelectionBox = new Rectangle(-1, -1, 0, 0);
            }
                
            for (int i = 0; i < units.Count; i++)
            {
                units[i].update(units, winMousePos, RbtnState, LbtnState, oldmouse.RightButton, oldmouse.LeftButton);

                if (units[i].getHealth() <= 0)
                {
                    units.RemoveAt(i);
                }
            }

                for (int i = 0; i < bacteria.Count; i++)
                {
                    bacteria[i].update(m_content, worldResources, time);

                    if (bacteria[i].getHealth() <= 0)
                    {
                        bacteria.RemoveAt(i);
                    }
                }
                for (int i = 0; i < soldier.Count; i++)
                {
                    soldier[i].update(m_content, viruses, bacteria,units,soldier, time);

                    if (soldier[i].getHealth() <= 0)
                    {
                        soldier.RemoveAt(i);
                    }
                }

            for (int i = 0; i < viruses.Count; i++)
            {
                if (virusTargets.Count > 0)
                {
                    viruses[i].update(virusTargets, time);
                }
                else
                {
                    viruses[i].update(units, time);
                }

                if (viruses[i].getHealth() <= 0)
                {
                    viruses.RemoveAt(i);
                    m_score += 500;
                }
            }

            for (int i = 0; i < virusTargets.Count; i++)
            {
                if (virusTargets[i].getHealth() <= 0)
                {
                    virusTargets.RemoveAt(i);
                }
            }

            for (int i = 0; i < worldResources.Count; i++)
            {
                if (worldResources[i].getHealth() <= 0)
                {
                    if (worldResources[i].getType() == Material.type.Food)
                    {
                        curResources.Food += 100;
                        incScore(1);
                    }
                    else if (worldResources[i].getType() == Material.type.Water)
                    {
                        curResources.Water += 200;
                        incScore(2);
                    }
                    
                    worldResources.RemoveAt(i);
                }
            }
            if (worldResources.Count <= 0)
            {
                GameOptions.m_win = true;
                GameOptions.gameOver();
            }

            for (int i = 0; i < rangers.Count; i++)
            {
                rangers[i].update(m_content, viruses, time);

                if (rangers[i].getHealth() <= 0)
                {
                    rangers.RemoveAt(i);
                }
            }

        }

        private static SpriteFont m_font;
        private static Vector2 virusPop = new Vector2(GameOptions.getWidth() - 300, 0);

        public static void setFont(SpriteFont font)
        {
            m_font = font;
        }

        public void draw(SpriteBatch sb)
        {            
            sb.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        cam.get_transformation(GameOptions.getGraphics().GraphicsDevice));

            sb.Draw(m_texture, world, Color.White);
 
            foreach (Material mat in worldResources)
            {
                mat.draw(sb);
            } 

            foreach (Bacterium bact in bacteria)
            {
                bact.draw(sb);
            }

            foreach (BacteriumRanger r in rangers)
            {
                r.draw(sb);
            }

            foreach (BacteriumSoldier s in soldier)
            {
                s.draw(sb);
            }

            foreach (Virus vir in viruses)
            {
                vir.draw(sb);
            }

            foreach (Material tree in worldTrees)
            {
                tree.draw(sb);
            }

            DrawHorizontalLine(sb, mSelectionBox.Y);
            DrawHorizontalLine(sb, mSelectionBox.Y + mSelectionBox.Height);

            //Draw the vertical portions of the selection box 
            DrawVerticalLine(sb, mSelectionBox.X);
            DrawVerticalLine(sb, mSelectionBox.X + mSelectionBox.Width);

            sb.End();

            virusPop.X = GameOptions.getWidth() - 200;
            virusPop.Y = 10;
            int bactPop = bacteria.Count + soldier.Count + rangers.Count;
            sb.Begin();

            sb.DrawString(m_font, "Virus Population: " + viruses.Count + "\nScore: " + m_score, virusPop, Color.White, 0, new Vector2(0, 0), (float)0.8, SpriteEffects.None, 0);
            sb.DrawString(m_font, "Bacteria Population: " + bactPop, new Vector2(15, 10), Color.White, 0, new Vector2(0, 0), (float)0.8, SpriteEffects.None, 0);
            sb.DrawString(m_font, "Food: " + curResources.Food.ToString(), new Vector2(15, 30), Color.White, 0, new Vector2(0, 0), (float)0.8, SpriteEffects.None, 0);
            sb.DrawString(m_font, "Water:" + curResources.Water.ToString(), new Vector2(15, 45), Color.White, 0, new Vector2(0, 0), (float)0.8, SpriteEffects.None, 0);
            sb.DrawString(m_font, "'C' - Spawn Bacteria worker to collect resources" , new Vector2(15, GameOptions.getHeight()-30), Color.White,0,new Vector2(0,0),(float)0.6,SpriteEffects.None,0);
            sb.DrawString(m_font, "'S' - Spawn Bacteria soldier to attack viruses", new Vector2(15, GameOptions.getHeight() - 50), Color.White, 0, new Vector2(0, 0), (float)0.6, SpriteEffects.None, 0);
            sb.DrawString(m_font, "'R' - Spawn Bacteria Ranger to attack viruses", new Vector2(15, GameOptions.getHeight() - 70), Color.White, 0, new Vector2(0, 0), (float)0.6, SpriteEffects.None, 0);
            sb.DrawString(m_font, "Hold LMB and drag mouse to select Bacteria, right click to move", new Vector2(15, GameOptions.getHeight() - 90), Color.White, 0, new Vector2(0, 0), (float)0.6, SpriteEffects.None, 0);

            sb.DrawString(m_font, "Collect all resources or kill all Viruses to win!", new Vector2(450, GameOptions.getHeight() - 20), Color.White, 0, new Vector2(0, 0), (float)0.6, SpriteEffects.None, 0);
            sb.DrawString(m_font, "Use workers to collect food and water", new Vector2(450, GameOptions.getHeight() - 40), Color.White, 0, new Vector2(0, 0), (float)0.6, SpriteEffects.None, 0);
            
            sb.End();
        }

        private void DrawHorizontalLine(SpriteBatch sb, int thePositionY)
        {
            //When the width is greater than 0, the user is selecting an area to the right of the starting point
            if (mSelectionBox.Width > 0)
            {
                //Draw the line starting at the startring location and moving to the right
                for (int aCounter = 0; aCounter <= mSelectionBox.Width - 10; aCounter += 10)
                {
                    if (mSelectionBox.Width - aCounter >= 0)
                    {
                        sb.Draw(mDottedLine, new Rectangle(mSelectionBox.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
            //When the width is less than 0, the user is selecting an area to the left of the starting point
            else if (mSelectionBox.Width < 0)
            {
                //Draw the line starting at the starting location and moving to the left
                for (int aCounter = -10; aCounter >= mSelectionBox.Width; aCounter -= 10)
                {
                    if (mSelectionBox.Width - aCounter <= 0)
                    {
                        sb.Draw(mDottedLine, new Rectangle(mSelectionBox.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
        }

        private void DrawVerticalLine(SpriteBatch sb, int thePositionX)
        {
            //When the height is greater than 0, the user is selecting an area below the starting point
            if (mSelectionBox.Height > 0)
            {
                //Draw the line starting at the starting loctino and moving down
                for (int aCounter = -2; aCounter <= mSelectionBox.Height; aCounter += 10)
                {
                    if (mSelectionBox.Height - aCounter >= 0)
                    {
                        sb.Draw(mDottedLine, new Rectangle(thePositionX, mSelectionBox.Y + aCounter, 10, 5), new Rectangle(0, 0, mDottedLine.Width, mDottedLine.Height), Color.White, MathHelper.ToRadians(90), new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }
            //When the height is less than 0, the user is selecting an area above the starting point
            else if (mSelectionBox.Height < 0)
            {
                //Draw the line starting at the start location and moving up
                for (int aCounter = 0; aCounter >= mSelectionBox.Height; aCounter -= 10)
                {
                    if (mSelectionBox.Height - aCounter <= 0)
                    {
                        sb.Draw(mDottedLine, new Rectangle(thePositionX - 10, mSelectionBox.Y + aCounter, 10, 5), Color.White);
                    }
                }
            }
        }

        public static void createBacteria(float X, float Y, int level)
        {
            int w = 30, h = 30;

            if (level == 1)
            {
                w = 30;
                h = 30;
            }
            foreach (Unit unit in units)
            {
                if (unit.getRect().Intersects(new Rectangle((int)X, (int)Y, 40, 40)))
                {
                    X = X + 40;
                    Y = Y + 40;
                }
            }
            Bacterium newBacteria = new Bacterium(X, Y, w, h , level);

            newBacteria.setTexture(m_content.Load<Texture2D>("SBacteria1_0"));
            newBacteria.setHealthBar(m_content.Load<Texture2D>("HealthBar"));

            curResources = curResources - Bacterium.getRequiredResources();

            bacteria.Add(newBacteria);
            units.Add(newBacteria);
            virusTargets.Add(newBacteria);
        }
        public static void createSoldier(float X, float Y, int level)
        {
            int w = 40, h = 40;

            foreach (Unit unit in units)
            {
                if (unit.getRect().Intersects(new Rectangle((int)X, (int)Y, 40, 40)))
                {
                    X = X + 40;
                    Y = Y + 40;
                }
            }

            BacteriumSoldier newBacteria = new BacteriumSoldier(X, Y, w, h, level);

            newBacteria.setTexture(m_content.Load<Texture2D>("SBacteria2_0"));
            newBacteria.setHealthBar(m_content.Load<Texture2D>("HealthBar"));

            curResources = curResources - BacteriumSoldier.getRequiredResources();

            soldier.Add(newBacteria);
            units.Add(newBacteria);
            virusTargets.Add(newBacteria);
        }

        public static void createRanger(float X, float Y)
        {
            foreach (Unit unit in units)
            {
                if (unit.getRect().Intersects(new Rectangle((int)X, (int)Y, 40, 40)))
                {
                    X = X + 40;
                    Y = Y + 40;
                }
            }

            BacteriumRanger newRanger = new BacteriumRanger(X, Y, m_content.Load<Texture2D>("bullet"));

            newRanger.setTexture(m_content.Load<Texture2D>("SBacteria2_0"));
            newRanger.setHealthBar(m_content.Load<Texture2D>("HealthBar"));

            curResources = curResources - BacteriumRanger.getRequiredResources();

            rangers.Add(newRanger);
            units.Add(newRanger);
            virusTargets.Add(newRanger);
        }

        public static void incScore(int type)
        {
            switch (GameOptions.getDifficulty())
            {
                case GameOptions.GameDifficulties.Easy: 
                    if (type == 1)
                        m_score += 100;
                    else
                        m_score += 50;
                    break;
                case GameOptions.GameDifficulties.Medium:
                    if (type == 1)
                        m_score += 200;
                    else
                        m_score += 100;
                    break;
                case GameOptions.GameDifficulties.Hard:
                    if (type == 1)
                        m_score += 500;
                    else
                        m_score += 300;
                    break;
                default:
                    m_score+=100;
                    break;
            }
        }
        public static int getScore()
        {
            return m_score;
        }

        private int m_cameraSpeed = 4;

        public void setCameraSpeed(int amount)
        {
            m_cameraSpeed = amount;
        }

        public void moveCamDown()
        {
            if(world.Y+m_worldHeight-15>cam.Pos.Y + GameOptions.getHeight()/2)
                cam.Move(new Vector2(0,m_cameraSpeed));
        }
        public void moveCamUp()
        {
            if (world.Y + 15 < cam.Pos.Y - GameOptions.getHeight() / 2)
                cam.Move(new Vector2(0, -m_cameraSpeed));
        }
        public void moveCamRight()
        {
            if (world.X + m_worldWidth - 15 > cam.Pos.X + GameOptions.getWidth() / 2)
                cam.Move(new Vector2(m_cameraSpeed,0));
        }
        public void moveCamLeft()
        {
            if (world.X + 15 < cam.Pos.X - GameOptions.getWidth() / 2)
                cam.Move(new Vector2(-m_cameraSpeed, 0));
        }
    }
}