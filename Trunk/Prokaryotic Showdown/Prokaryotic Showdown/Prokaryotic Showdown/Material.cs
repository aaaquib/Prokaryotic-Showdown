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
    class Material : Unit
    {
        public enum type
        {
            Food,
            Water,
            Tree
        }

        private type curType;

        public type getType()
        {
            return curType;
        }
        /// <summary>
        /// Creates a matType Resource with the specified size and position
        /// </summary>
        /// <param name="Width">Width of the Resource</param>
        /// <param name="Height">Height of the Resource</param>
        /// <param name="matType">Type of resource to create (food, water or tree)</param>
        /// <param name="x"> X position</param>
        /// <param name="y"> Y position</param>
        public Material(float X, float Y, int Width, int Height, type matType)
        {
            m_position = new Vector2(X, Y);
            m_width = Width;
            m_height = Height;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, Width, Height);
            curType = matType;
            m_origin = new Vector2(Width / 2, Height / 2);

            switch (curType)
            {
                case type.Food:
                    m_unitHealth = 75;
                    break;
                case type.Water:
                    m_unitHealth = 50;
                    break;
            }
        }

        public override void draw(SpriteBatch sb)
        {

            if (m_unitHealth > 0)
            {
                switch (curType)
                {
                    case type.Food: sb.Draw(m_texture, m_position, null, Color.White, m_rotation, m_origin, 0.15f, SpriteEffects.None, 0); break;

                    case type.Water: sb.Draw(m_texture, m_position, null, Color.White, m_rotation, m_origin, 0.35f, SpriteEffects.None, 0); break;

                    case type.Tree: sb.Draw(m_texture, m_position, null, Color.White, m_rotation, m_origin, 0.4f, SpriteEffects.None, 0); break;

                    default:
                        sb.Draw(m_texture, m_position, null, Color.White, m_rotation, m_origin, 0.5f, SpriteEffects.None, 0);
                        break;
                }
            }
        }
    }
}