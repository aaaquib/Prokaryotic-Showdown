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
    class Virus : Unit
    {
        Random rand = new Random(DateTime.Now.Millisecond);
        Rectangle m_scanArea;
        const int SCAN_RADIUS = 30;
        /// <summary>
        /// Creates a Virus with the specified size and position
        /// </summary>
        /// <param name="Width">Width of the Virus</param>
        /// <param name="Height">Height of the Virus</param>
        /// <param name="X"> X position</param>
        /// <param name="Y"> Y position</param>
        public Virus(float X, float Y, int Width, int Height)
        {
            m_position = new Vector2(X, Y);
            m_width = Width;
            m_height = Height;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, Width, Height);
            m_origin = new Vector2(Width / 2, Height / 2);
            m_speed = 1;
            m_unitHealth = 75;
            m_curDestination = new Vector2(0, 0);

            m_scanArea = new Rectangle((int)m_position.X - 50, (int)m_position.Y - 50, SCAN_RADIUS * 2, SCAN_RADIUS * 2);
        }

        public void setHealthBar(Texture2D texture)
        {
            healthBar = texture;
        }

        public void update(List<Unit> enemies, GameTime time)
        {
            Unit target = scan(enemies);

            m_rectangle.X = (int)m_position.X;
            m_rectangle.Y = (int)m_position.Y;

            if (target != null)
            {
                m_rotation = (float)Math.Atan2(m_position.X - target.X, target.Y - m_position.Y);
                performAction(time, target);
            }
            else
            {
                int mov = rand.Next(-100, 100);

                if (mov < 0)
                {
                    m_rotation -= 0.1f;
                }
                else
                {
                    m_rotation += 0.1f;
                }
            }

            if (!collisionDetection(enemies))
            {
                m_position.X -= (float)(m_speed * Math.Sin(m_rotation));
                m_position.Y += (float)(m_speed * Math.Cos(m_rotation));
            }

            if(m_position.X>TBSManager.getWorldWidth())
            {
                m_position.X -= 25;
                m_rotation -= 3.14f;
            }
            if (m_position.X < TBSManager.world.X)
            {
                m_position.X += 25;
                m_rotation -= 3.14f;
            }
            if (m_position.Y > TBSManager.getWorldHeight())
            {
                m_position.Y -= 25;
                m_rotation -= 3.14f;
            }
            if (m_position.Y < TBSManager.world.Y)
            {
                m_position.Y += 25;
                m_rotation -= 3.14f;
            }
        }

        public bool collisionDetection(List<Unit> collideables)
        {
            foreach (Unit curU in collideables)
            {
                if (curU.getRect().Intersects(m_rectangle))
                {
                    return true;
                }
            }

            return false;
        }

        public Unit scan(List<Unit> targets)
        {
            m_scanArea.X = (int)m_position.X - SCAN_RADIUS;
            m_scanArea.Y = (int)m_position.Y - SCAN_RADIUS;

            foreach (Unit sub in targets)
            {
                if (m_scanArea.Intersects(sub.getRect()))
                {
                    return sub;
                }
            }

            return null;
        }

        private TimeSpan elapsedTime = new TimeSpan();
        private TimeSpan oneSecond = new TimeSpan(0, 0, 1);
        const int ATTACK_RATE = 5;

        public void performAction(GameTime time, Unit target)
        {
            if (elapsedTime.Seconds > ATTACK_RATE)
            {
                elapsedTime = elapsedTime.Subtract(elapsedTime);
                target.doDamage(GameOptions.getVirusStrength());
            }

            elapsedTime += time.ElapsedGameTime;

            if (elapsedTime.Milliseconds > 900)
            {
                elapsedTime.Add(oneSecond);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, m_rectangle, null, Color.White, m_rotation, m_origin, SpriteEffects.None, 0);
                //Draw the negative space for the health bar
                sb.Draw(healthBar, new Rectangle((int)m_position.X, (int)m_position.Y - 15, (int)healthBar.Width / 13, 5), new Rectangle(0, 45, healthBar.Width, 44), Color.Red);

                //Draw the current health level based on the current Health
                sb.Draw(healthBar, new Rectangle((int)m_position.X, (int)m_position.Y - 15, (int)((healthBar.Width) * ((double)m_unitHealth / 75)) / 13, 5), new Rectangle(0, 45, healthBar.Width, 44), Color.Green);

                //Draw the box around the health bar
                sb.Draw(healthBar, new Rectangle((int)m_position.X, (int)m_position.Y - 15, healthBar.Width / 13, 5), new Rectangle(0, 0, healthBar.Width, 44), Color.White);

        }
    }
}