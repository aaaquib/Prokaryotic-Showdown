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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Xml;

namespace HUD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HudUsingXml 
    {
        //GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont sf;
        string filename;
        List<ImageDisplay> imageDisplays;
        List<TextDisplay> textDisplays;
        List<SoundObject> soundPlays;

        public float timer;
        float secs, min, hours;
        public bool pauseTime;
        string gtime;
        public int score1,score2;
        public int resolution=1;

        public HudUsingXml(GraphicsDevice graphicsDevice)
        {
            //graphics = new GraphicsDeviceManager(this);
            spriteBatch = new SpriteBatch(graphicsDevice);
            score1 = 0;
            score2 = 0;
           // resolution = 1;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            // TODO: Add your initialization logic here
            if (resolution == 1)
            {
                filename = "Settings.xml";
            }
            else if (resolution == 2)
            {
                filename = "Settings2.xml";
            }
            imageDisplays = new List<ImageDisplay>();
            textDisplays = new List<TextDisplay>();
            soundPlays = new List<SoundObject>();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public  void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sf = content.Load<SpriteFont>("HUDfont");

            // TODO: use this.Content to load your game content here

            //try
            //{
                // open and read the XML file
                XmlDocument reader = new XmlDocument();
                reader.Load(filename);
                
                //from the read in - create an XmlNodeList 
                XmlNodeList allNodes = reader.ChildNodes;
                
                // iterate through the list, looking for the root tag, aka <HUDSettings>
                foreach (XmlNode rootNode in allNodes)
                {
                    if (rootNode.Name == "HUDSettings")
                    {
                        // make another XmlNodeList - this time pulling all the childnodes nested in <HUDSettings>
                        XmlNodeList rootChildren = rootNode.ChildNodes;
                        foreach (XmlNode node in rootChildren)
                        {
                            if (node.Name == "Image")
                            {
                                imageDisplays.Add(new ImageDisplay(node));
                            }

                            if (node.Name == "Text")
                            {
                                textDisplays.Add(new TextDisplay(node));
                            }
                            if (node.Name == "Sound")
                            {
                                soundPlays.Add(new SoundObject(node));
                            }
                        }
                    }
                }
            
            //}

            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine("Error reading xml");
            //    throw e;
            //}

            // once everything is loaded, call loadContent on your image Displays
            foreach (ImageDisplay id in imageDisplays)
            {
                id.LoadContent(content);
            }
            foreach (SoundObject snd in soundPlays)
            {
                snd.LoadContent(content);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void DisplayTime(GameTime gameTime, SpriteFont font, float positionX, float positionY, Color color)
        {
            
            if (!pauseTime)
            {
                secs += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timer += (int)secs;
                if (secs >= 1.0F)
                    secs = 0F;
                if (timer >= 60)
                {
                    min++;
                    timer = 0;
                }
                if (min >= 60)
                {
                    hours++;
                    min = 0;
                }
                if (hours >= 24)
                {
                    hours = min = timer = 0;
                }
            }
            gtime = hours.ToString() + ":" + min.ToString() + ":" + timer.ToString();

            spriteBatch.DrawString(font, gtime, new Vector2(positionX, positionY), color);

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void PlaySound(int index)
        {
            if(index>=1)
                soundPlays[index-1].sound.Play();
        }

        public void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            foreach (ImageDisplay id in imageDisplays)
            {
                if(id.nameID=="Map")
                    spriteBatch.Draw(id.texture,id.rectangle,new Color(200,200,200,100));
                else if(id.nameID=="PlayerHealth")
                    spriteBatch.Draw(id.texture, id.rectangle, new Color(200, 200, 200, 120));
                else
                    spriteBatch.Draw(id.texture, id.rectangle, Color.White);
            }

            foreach (TextDisplay td in textDisplays)
            {
                if(td.nameID=="Player1Score")
                    spriteBatch.DrawString(sf, score1.ToString(), td.position, Color.Blue);
                else if (td.nameID == "Player2Score")
                    spriteBatch.DrawString(sf, score2.ToString(), td.position, Color.Blue);
                else if(td.nameID=="Timer" && pauseTime == false)
                    DisplayTime(gameTime, sf, td.position.X,td.position.Y, Color.Blue);
                else
                    spriteBatch.DrawString(sf, td.text, td.position, Color.Blue);
            }
            spriteBatch.End();
        }
    }
}
