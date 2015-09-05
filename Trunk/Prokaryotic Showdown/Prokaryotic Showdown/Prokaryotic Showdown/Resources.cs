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
    public class Resources
    {
        private int m_food = 0;
        private int m_water = 0;

        public int Food
        {
            get { return m_food; }
            set { m_food = value; }
        }

        public int Water
        {
            get { return m_water; }
            set { m_water = value; }
        }
        /// <summary>
        /// Stores the amount of food and water the player currently has.
        /// </summary>
        public Resources(int Food, int Water)
        {
            m_food = Food;
            m_water = Water;
        }
        public static bool operator >=(Resources r1, Resources r2)
        {
            if (r1.Food >= r2.Food)
            {
                 if (r1.Water >= r2.Water)
                    {
                        return true;
                    }
            }

            return false;
        }

        public static bool operator <=(Resources r1, Resources r2)
        {
            if (r1.Food <= r2.Food)
            {
                    if (r1.Water <= r2.Water)
                    {
                            return true;
                    }
            }

            return false;
        }
        public static Resources operator -(Resources r1, Resources r2)
        {
            return new Resources(r1.Food - r2.Food, r1.Water - r2.Water);
        }

        public override string ToString()
        {
            return "\nFood: " + m_food + "\nWater: " + m_water;
        }
    }
}