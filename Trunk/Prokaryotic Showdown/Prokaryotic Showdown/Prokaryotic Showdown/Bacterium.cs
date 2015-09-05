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
    class Bacterium : Unit
    {
        private static Resources requiredResources = new Resources(300, 300);
        private int m_level = 1;
        private int WORK_RATE = 5;
        const int SCAN_RADIUS = 50;

        /// <summary>
        /// Creates a Bacteria with the specified size and position
        /// </summary>
        /// <param name="Width">Width of the bacteria</param>
        /// <param name="Height">Height of the bacteria</param>
        /// <param name="x"> X position</param>
        /// <param name="y"> Y position</param>
        public Bacterium(float X, float Y, int Width, int Height, int level)
        {
            m_position = new Vector2(X, Y);
            m_width = 25;
            m_height = 25;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, m_width, m_height);
            m_origin = new Vector2(m_width / 2, m_height / 2);
            m_level = level;

            m_unitHealth = 75;
            SPEED = 3;
            WORK_RATE = 7;
        }
        
        public static Resources getRequiredResources()
        {
            return requiredResources;
        }

        public void setHealthBar(Texture2D texture)
        {
            healthBar = texture;
        }

        private TimeSpan consumedTime = new TimeSpan();
        private TimeSpan animTime = new TimeSpan();
        int animateSprite = 0;


        public void update(ContentManager content, List<Material> worldResources, GameTime time)
        {

            m_unitHealth = (int)MathHelper.Clamp(m_unitHealth, 0, 100);

            Rectangle scanArea = new Rectangle((int)m_position.X - 25, (int)m_position.Y - 25, 100, 100);

            foreach (Material curM in worldResources)
                {
                    if (scanArea.Intersects(curM.getRect()))
                    {
                        performAction(time, curM);
                    }
                }

            animTime += time.ElapsedGameTime;
            
            if (animTime.Milliseconds % 22 == 0)
            {
                animTime = new TimeSpan();
                animateSprite++;
                if (animateSprite > 7)
                    animateSprite = 0;

                switch (animateSprite)
                {
                    case 0: setTexture(content.Load<Texture2D>("SBacteria1_0"));
                        break;
                    case 1: setTexture(content.Load<Texture2D>("SBacteria1_1"));
                        break;
                    case 2: setTexture(content.Load<Texture2D>("SBacteria1_2"));
                        break;
                    case 3: setTexture(content.Load<Texture2D>("SBacteria1_3"));
                        break;
                    case 4: setTexture(content.Load<Texture2D>("SBacteria1_4"));
                        break;
                    case 5: setTexture(content.Load<Texture2D>("SBacteria1_5"));
                        break;
                    case 6: setTexture(content.Load<Texture2D>("SBacteria1_6"));
                        break;
                    case 7: setTexture(content.Load<Texture2D>("SBacteria1_7"));
                        break;
                }
            }

            if (consumedTime.Seconds > 30)
            {
                consumedTime = new TimeSpan();
                if (TBSManager.curResources.Food > 0 && m_unitHealth > 0)
                {
                    TBSManager.curResources.Food -= 10;
                    m_unitHealth += 5;
                }
                else
                {
                    m_unitHealth -= 5;
                }

                if (TBSManager.curResources.Water > 0 && m_unitHealth > 0)
                {
                    TBSManager.curResources.Water -= 20;
                    m_unitHealth += 10;
                }
                else
                {
                    m_unitHealth -= 10;
                }
            }

            consumedTime += time.ElapsedGameTime;

            if (consumedTime.Milliseconds > 900)
            {
                consumedTime.Add(new TimeSpan(0, 0, 1));
            }
        }
        
        private TimeSpan elapsedTime = new TimeSpan();
        private TimeSpan oneSecond = new TimeSpan(0, 0, 1);

        public void performAction(GameTime time, Material mat)
        {

            if (elapsedTime.Seconds > WORK_RATE)
            {
                elapsedTime = elapsedTime.Subtract(elapsedTime);
                mat.doDamage(20);
                m_rotation = (float)Math.Atan2(m_position.X - mat.X, mat.Y - m_position.Y);
            }

            elapsedTime += time.ElapsedGameTime;

            if (elapsedTime.Milliseconds > 900)
            {
                elapsedTime.Add(oneSecond);
            }
        }


        public override void draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, m_position, null, Color.White, m_rotation, m_origin,0.8f, SpriteEffects.None, 0);
            if (m_selected)
            {
                //Draw the negative space for the health bar
                sb.Draw(healthBar, new Rectangle((int)m_position.X, (int)m_position.Y - 15, (int)healthBar.Width / 11, 6), new Rectangle(0, 45, healthBar.Width, 44), Color.Red);

                //Draw the current health level based on the current Health
                sb.Draw(healthBar, new Rectangle((int)m_position.X, (int)m_position.Y - 15, (int)((healthBar.Width) * ((double)m_unitHealth / 75)) / 11, 6), new Rectangle(0, 45, healthBar.Width, 44), Color.Green);

                //Draw the box around the health bar
                sb.Draw(healthBar, new Rectangle((int)m_position.X, (int)m_position.Y - 15, healthBar.Width / 11, 6), new Rectangle(0, 0, healthBar.Width, 44), Color.White);
            }

        }
    }
}