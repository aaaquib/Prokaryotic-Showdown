using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Prokaryotic_Showdown
{
    class BacteriumSoldier : Unit
    {
        Rectangle m_scanArea;
        const int SCAN_RADIUS = 50;
        const int ATTACK_RATE = 2;

        /// <summary>
        /// Creates a Soldier Bacteria with the specified size and position
        /// </summary>
        /// <param name="Width">Width of the bacteria</param>
        /// <param name="Height">Height of the bacteria</param>
        /// <param name="x"> X position</param>
        /// <param name="y"> Y position</param>
        public BacteriumSoldier(float X, float Y, int Width, int Height, int level)
        {
            m_position = new Vector2(X, Y);
            m_width = 25;
            m_height = 25;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, m_width, m_height);
            m_origin = new Vector2(m_width / 2, m_height / 2);
            m_scanArea = new Rectangle((int)m_position.X - 70, (int)m_position.Y - 70, SCAN_RADIUS * 3, SCAN_RADIUS * 3);
            SPEED = 2;
            m_unitHealth = 100;
        }

        private static Resources requiredResources = new Resources(400, 400);

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

        public void update(ContentManager content, List<Virus> viruses, List<Bacterium> bacteria, List<Unit> units, List<BacteriumSoldier> soldiers, GameTime time)
        {
            //go through list
            //if one nearby go to it and chop it down
            //mark down index and remove from list after chop down
            m_unitHealth = (int)MathHelper.Clamp(m_unitHealth, 0, 100);

            Virus target = scan(viruses);

            if (target != null && curState != State.Walking)
            {
                curState = State.Attacking;

                m_rotation = (float)Math.Atan2(m_position.X - target.X, target.Y - m_position.Y);
                performAction(time, target);

                float moveX = m_speed * (float)Math.Sin(m_rotation);
                float moveY = m_speed * (float)Math.Cos(m_rotation);
                if (!collisionDetection(units, moveX, moveY))
                {
                    m_position.X -= moveX;
                    m_position.Y += moveY;
                }
            }
            else
            {
                m_rotation = (float)Math.Atan2(m_position.X - m_curDestination.X, m_curDestination.Y - m_position.Y);
            }

            animTime += time.ElapsedGameTime;
            if (animTime.Milliseconds % 20 == 0)
            {
                animTime = new TimeSpan();
                animateSprite++;
                if (animateSprite > 7)
                    animateSprite = 0;

                switch (animateSprite)
                {
                    case 0: setTexture(content.Load<Texture2D>("SBacteria2_0"));
                        break;
                    case 1: setTexture(content.Load<Texture2D>("SBacteria2_1"));
                        break;
                    case 2: setTexture(content.Load<Texture2D>("SBacteria2_2"));
                        break;
                    case 3: setTexture(content.Load<Texture2D>("SBacteria2_3"));
                        break;
                    case 4: setTexture(content.Load<Texture2D>("SBacteria2_4"));
                        break;
                    case 5: setTexture(content.Load<Texture2D>("SBacteria2_5"));
                        break;
                    case 6: setTexture(content.Load<Texture2D>("SBacteria2_6"));
                        break;
                    case 7: setTexture(content.Load<Texture2D>("SBacteria2_7"));
                        break;
                }
            }

            if (consumedTime.Seconds > 25)
            {
                consumedTime = new TimeSpan();

                if (TBSManager.curResources.Food > 0 && m_unitHealth > 0)
                {
                    TBSManager.curResources.Food-=10;
                    m_unitHealth += 5;
                }
                else
                {
                    m_unitHealth-=5;
                }

                if (TBSManager.curResources.Water > 0 && m_unitHealth > 0)
                {
                    TBSManager.curResources.Water-=20;
                    m_unitHealth += 10;

                }
                else
                {
                    m_unitHealth-=10;
                }
            }

            consumedTime += time.ElapsedGameTime;

            if (consumedTime.Milliseconds > 900)
            {
                consumedTime.Add(new TimeSpan(0, 0, 1));
            }
        }

        public Virus scan(List<Virus> targets)
        {

            m_scanArea.X = (int)m_position.X - SCAN_RADIUS;
            m_scanArea.Y = (int)m_position.Y - SCAN_RADIUS;

            foreach (Virus sub in targets)
            {
                if (m_scanArea.Intersects(sub.getRect()))
                {
                    if (sub.getHealth() > 0)
                    {
                        return sub;
                    }
                }
            }

            return null;
        }

        private TimeSpan elapsedTime = new TimeSpan();
        private TimeSpan oneSecond = new TimeSpan(0, 0, 1);

        public void performAction(GameTime time, Virus target)
        {
            if (elapsedTime.Seconds > ATTACK_RATE)
            {
                elapsedTime = elapsedTime.Subtract(elapsedTime);
                target.doDamage(20);
            }

            elapsedTime += time.ElapsedGameTime;

            if (elapsedTime.Milliseconds > 900)
            {
                elapsedTime.Add(oneSecond);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, m_position, null, Color.White, m_rotation, m_origin,1.4f, SpriteEffects.None, 0);
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