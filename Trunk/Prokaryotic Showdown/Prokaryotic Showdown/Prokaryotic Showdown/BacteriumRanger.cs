using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Prokaryotic_Showdown
{
    class BacteriumRanger : Unit
    {
        const int ATTACK_RATE = 500;
        Rectangle m_scanArea;
        const int SCAN_RADIUS = 200;
        public List<Bullet> bullets = new List<Bullet>();
        private Texture2D m_bulletTexture;

        /// <summary>
        /// Creates a Ranger Bacteria with the specified size and position
        /// </summary>
        /// <param name="x"> X position</param>
        /// <param name="y"> Y position</param>
        /// /// <param name="bulletTex"> texture for the bullet</param>
        public BacteriumRanger(float X, float Y, Texture2D bulletTex)
        {
            m_position = new Vector2(X, Y);
            m_width = 25;
            m_height = 25;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, m_width, m_height);
            m_origin = new Vector2(m_width / 2, m_height / 2);
            area = new Rectangle(0, 0, 50, 50);
            m_bulletTexture = bulletTex;

            m_scanArea = new Rectangle((int)m_origin.X - SCAN_RADIUS, (int)m_origin.Y - SCAN_RADIUS, SCAN_RADIUS * 2, SCAN_RADIUS * 2);
        }

        public static Resources getRequiredResources()
        {
            return new Resources(500, 500);
        }

        public void setHealthBar(Texture2D texture)
        {
            healthBar = texture;
        }

        public override void draw(SpriteBatch sb)
        {
            if (m_unitHealth > 0)
            {
                foreach (Bullet curB in bullets)
                {
                    curB.draw(sb);
                }

                sb.Draw(m_texture, m_position,null, Color.White, m_rotation, m_origin,1.1f, SpriteEffects.None, 0);

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

        Rectangle area;

        private TimeSpan consumedTime = new TimeSpan();
        private TimeSpan animTime = new TimeSpan();
        int animateSprite = 0;

        public void update(ContentManager content, List<Virus> viruses, GameTime time)
        {
            m_unitHealth = (int)MathHelper.Clamp(m_unitHealth, 0, 100);

            if (scan(viruses))
            {
                performAction(time);
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(viruses);

                if (bullets[i].isVisible == false)
                {
                    bullets[i] = null;
                    bullets.RemoveAt(i);
                    GC.Collect();
                }
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
                    case 0: setTexture(content.Load<Texture2D>("RBacteria2_0"));
                        break;
                    case 1: setTexture(content.Load<Texture2D>("RBacteria2_1"));
                        break;
                    case 2: setTexture(content.Load<Texture2D>("RBacteria2_2"));
                        break;
                    case 3: setTexture(content.Load<Texture2D>("RBacteria2_3"));
                        break;
                    case 4: setTexture(content.Load<Texture2D>("RBacteria2_4"));
                        break;
                    case 5: setTexture(content.Load<Texture2D>("RBacteria2_5"));
                        break;
                    case 6: setTexture(content.Load<Texture2D>("RBacteria2_6"));
                        break;
                    case 7: setTexture(content.Load<Texture2D>("RBacteria2_7"));
                        break;
                }
            }

            if (consumedTime.Seconds > 35)
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

        public bool scan(List<Virus> targets)
        {
            m_scanArea.X = (int)m_position.X - SCAN_RADIUS;
            m_scanArea.Y = (int)m_position.Y - SCAN_RADIUS;

            foreach (Virus sub in targets)
            {
                if (m_scanArea.Contains(sub.getRect()))
                {
                    m_rotation = (float)Math.Atan2(m_position.X - sub.X, sub.Y - m_position.Y);
                    return true;
                }
            }

            return false;
        }

        private TimeSpan elapsedTime = new TimeSpan();
        private TimeSpan oneSecond = new TimeSpan(0, 0, 1);

        public void performAction(GameTime time)
        {
            if (elapsedTime.Milliseconds > ATTACK_RATE)
            {
                elapsedTime = elapsedTime.Subtract(elapsedTime);
                bullets.Add(new Bullet(m_bulletTexture));
                bullets.Last().FireBullet(m_rotation, m_position.X, m_position.Y);
            }

            elapsedTime += time.ElapsedGameTime;
        }

        public void moveBulletsDown(int amount)
        {
            foreach (Bullet b in bullets)
            {
                b.Y -= amount;
            }
        }

        public void moveBulletsUp(int amount)
        {
            foreach (Bullet b in bullets)
            {
                b.Y += amount;
            }
        }

        public void moveBulletsRight(int amount)
        {
            foreach (Bullet b in bullets)
            {
                b.X -= amount;
            }
        }

        public void moveBulletsLeft(int amount)
        {
            foreach (Bullet b in bullets)
            {
                b.X += amount;
            }
        }
    }
}