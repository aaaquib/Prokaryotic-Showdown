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

namespace HudUsingXml
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HudUsingXml : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont sf;
        string filename = "Settings.xml";
        List<ImageDisplay> imageDisplays;
        List<TextDisplay> textDisplays;
        float timer, secs, min, hours;
        public bool pauseTime;
        string gtime;
        public int score1,score2;

        public HudUsingXml()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //this.Window.AllowUserResizing=true;
            this.graphics.PreferredBackBufferWidth = 720;
            this.graphics.PreferredBackBufferHeight = 480;
            score1 = 4;
            score2 = 8;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            imageDisplays = new List<ImageDisplay>();
            textDisplays = new List<TextDisplay>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sf = Content.Load<SpriteFont>("HUDfont");

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
                id.LoadContent(this.Content);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        public void DisplayTime(GameTime gameTime, SpriteFont font, float positionX, float positionY, Color color)
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
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            foreach (ImageDisplay id in imageDisplays)
            {
                spriteBatch.Draw(id.texture, id.rectangle, Color.White);
            }

            foreach (TextDisplay td in textDisplays)
            {
                if(td.nameID=="Player1Score")
                    spriteBatch.DrawString(sf, score1.ToString(), td.position, Color.White);
                else if (td.nameID == "Player2Score")
                    spriteBatch.DrawString(sf, score2.ToString(), td.position, Color.White);
                else if(td.nameID=="Timer")
                    DisplayTime(gameTime, sf, td.position.X,td.position.Y, Color.Magenta);
                else
                spriteBatch.DrawString(sf, td.text, td.position, Color.White);
            }
            spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
