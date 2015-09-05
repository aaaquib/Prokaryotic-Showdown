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
    class Unit
    {
        protected int m_unitHealth = 100;
        protected Vector2 m_position;
        protected int m_width, m_height;
        protected Vector2 m_origin;

        protected float m_rotation;
        protected SpriteFont m_font;
        protected Texture2D m_texture;
        protected Rectangle m_rectangle;
        protected Texture2D healthBar;

        protected bool m_selected = false;
        protected int SPEED = 3;
        protected Vector2 m_curDestination;

        protected enum State
        {
            Idle,
            Walking,
            Attacking,
        }

        protected State curState = State.Idle;
        protected int m_speed = 0;

        public float X
        {
            get { return m_position.X; }
            set
            {
                m_position.X = value;
                m_rectangle.X = (int)value;
            }
        }

        public float Y
        {
            get { return m_position.Y; }
            set
            {
                m_position.Y = value;
                m_rectangle.Y = (int)value;
            }
        }

        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public void setTexture(Texture2D texture)
        {
            m_texture = texture;
        }

        public Texture2D getTexture()
        {
            return m_texture;
        }

        public void setFont(SpriteFont font)
        {
            m_font = font;
        }

        public Rectangle getRect()
        {
            return m_rectangle;
        }

        public void selectUnit()
        {
            m_selected = true;
            m_speed = SPEED;
        }

        public void deselectUnit()
        {
            m_selected = false;
            if (curState != State.Walking)
                m_curDestination = m_position;
        }        

        public void setSpeed(int Speed)
        {
            m_speed = Speed;
        }

        public int getSpeed()
        {
            return m_speed;
        }

        public void moveCurDestLeft(float amount)
        {
            m_curDestination.X -= amount;
        }

        public void moveCurDestRight(float amount)
        {
            m_curDestination.X += amount;
        }

        public void moveCurDestUp(float amount)
        {
            m_curDestination.Y -= amount;
        }

        public void moveCurDestDown(float amount)
        {
            m_curDestination.Y += amount;
        }

        public void move(List<Unit> units, Vector2 destination)
        {
            m_rotation = (float)Math.Atan2(m_position.X - destination.X, destination.Y - m_position.Y);
                m_curDestination = destination;

            float moveX = m_speed * (float)Math.Sin(m_rotation);
            float moveY = m_speed * (float)Math.Cos(m_rotation);
            if (curState == State.Walking && !arrivedAtDestination() && !collisionDetection(units, moveX, moveY))
            {
                    m_position.X -= moveX;
                    m_position.Y += moveY;
            }
        }

        public bool collisionDetection(List<Unit> collideables, float moveX, float moveY)
        {
            Rectangle colRect = new Rectangle((int)(m_position.X - moveX), (int)(m_position.Y + moveY), m_width, m_height);
            foreach (Unit curU in collideables)
            {
                if (!this.Equals(curU))
                {
                    if (curU.getRect().Intersects(colRect))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        Rectangle arrivedArea = new Rectangle(0, 0, 30, 30);

        private bool arrivedAtDestination()
        {
            arrivedArea.X = (int)m_curDestination.X - 15;
            arrivedArea.Y = (int)m_curDestination.Y - 15;

            if (arrivedArea.Intersects(m_rectangle))
            {
                curState = State.Idle;
                return true;
            }

            return false;
        }

        public void setHealth(int health)
        {
            m_unitHealth = health;
        }

        public int getHealth()
        {
            return m_unitHealth;
        }

        public void doDamage(int amount)
        {
            m_unitHealth -= amount;
        }

        public virtual int getFullHealth()
        {
            return 100;
        }

        public virtual void performAction()
        {
            Console.WriteLine("performing action");
        }

        Rectangle area = new Rectangle(0, 0, 20, 20);
        Vector2 dest;

        public virtual void update(List<Unit> units, Vector2 worldMousePos, ButtonState RbtnState, ButtonState LbtnState, ButtonState oldRLbtnState, ButtonState oldLBtnState)
        {

            area.X = (int)worldMousePos.X - 10;
            area.Y = (int)worldMousePos.Y - 10;
            
            m_rectangle.X = (int)m_position.X;
            m_rectangle.Y = (int)m_position.Y;


            if (RbtnState == ButtonState.Released && oldRLbtnState == ButtonState.Pressed)
            {
                if (m_selected)
                {
                    dest = worldMousePos;
                    curState = State.Walking;
                }

            }
            if (LbtnState == ButtonState.Pressed)
            {
                if (!area.Intersects(m_rectangle))
                {
                    deselectUnit();
                }
            }
            if(curState == State.Walking)
                move(units, dest);
        }

        protected Rectangle m_displayArea;
        protected Vector2 m_displayTextPos;
        protected Texture2D m_displayTexture;
        protected bool m_displayInfo = false;

        public void setDisplayTexture(Texture2D texture)
        {
            m_displayTexture = texture;
        }
        public void setDisplayArea(Rectangle area)
        {
            m_displayArea = area;
            m_displayTextPos = new Vector2(area.X + 100, area.Y);
        }

        protected string m_displayText = "i need to be changed";

        public void setDisplayText(string text)
        {
            m_displayText = text;
        }
        protected virtual void drawDisplayInfo(SpriteBatch sb)
        {
            sb.DrawString(m_font, m_displayText + "\nHealth: " + m_unitHealth, m_displayTextPos, Color.Black);
        }
        public virtual void draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, m_rectangle, null, Color.White, m_rotation, m_origin, SpriteEffects.None, 0);
        }

    }
}