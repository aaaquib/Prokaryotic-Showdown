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
    public class GameOptions
    {
        private static int m_screenWidth = 800;
        private static int m_screenHeight = 600;
        public enum GameDifficulties
        {
            Easy,
            Medium,
            Hard
        }

        private static GameDifficulties curDifficulty = GameDifficulties.Easy;

        private static bool m_gamePlaying = false;
        public static bool m_gameOver = false;
        public static bool m_win = false;
        private static GraphicsDeviceManager m_graphics;

        public static void setWidth(int width)
        {
            m_screenWidth = width;
            m_graphics.PreferredBackBufferWidth = m_screenWidth;
            m_graphics.ApplyChanges();
        }

        public static void setHeight(int height)
        {
            m_screenHeight = height;
            m_graphics.PreferredBackBufferHeight = m_screenHeight;
            m_graphics.ApplyChanges();
        }

        public static void setScreen(int width, int height)
        {
            setHeight(height);
            setWidth(width);
        }

        public static int getWidth()
        {
            return m_screenWidth;
        }

        public static int getHeight()
        {
            return m_screenHeight;
        }

        public static void setDifficulty(GameDifficulties newDifficulty)
        {
            curDifficulty = newDifficulty;
        }

        public static GameDifficulties getDifficulty()
        {
            return curDifficulty;
        }

        public static GameOptions getInstance()
        {
            return new GameOptions();
        }

        public static int getNumOfVirusesToAdd()
        {
            switch (curDifficulty)
            {
                case GameDifficulties.Easy:
                    return 20;
                case GameDifficulties.Medium:
                    return 30;
                case GameDifficulties.Hard:
                    return 40;
            }

            return 10;
        }
        public static int getVirusStrength()
        {
            switch (curDifficulty)
            {
                case GameDifficulties.Easy:
                    return 20;
                case GameDifficulties.Medium:
                    return 30;
                case GameDifficulties.Hard:
                    return 40;
            }

            return 5;
        }

        public static int getVirusAddInterval()
        {
            switch (curDifficulty)
            {
                case GameDifficulties.Easy:
                    return 15;
                case GameDifficulties.Medium:
                    return 10;
                case GameDifficulties.Hard:
                    return 8;
            }

            return 2;
        }

        public static Resources getStartResources()
        {
            switch (curDifficulty)
            {
                case GameDifficulties.Easy:
                    return new Resources(10000, 10000);
                case GameDifficulties.Medium:
                    return new Resources(7000, 7000);
                case GameDifficulties.Hard:
                    return new Resources(3000, 3000);
            }

            return new Resources(100, 100);
        }

        public static void setGraphics(GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
        }

        public static GraphicsDeviceManager getGraphics()
        {
            return m_graphics;
        }

        public static void toggleFullScreen()
        {
            m_graphics.ToggleFullScreen();
        }
        

        public static bool isGameOver()
        {
            return m_gameOver;
        }

        public static void gameOver()
        {
            m_gameOver = true;
            m_gamePlaying = false;
        }

        public static void startGame()
        {
            m_gamePlaying = true;
            m_gameOver = false;
        }

        public static bool isGamePlaying()
        {
            return m_gamePlaying;
        }
    }
}