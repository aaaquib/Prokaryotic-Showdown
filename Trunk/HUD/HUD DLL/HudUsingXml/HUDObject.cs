using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace HUD
{
    public class HUDObject
    {
        public string nameID;

        public HUDObject(XmlNode node)
        {
        }
    }

    /// <summary>
    /// Used to display Text.
    /// </summary>
    public class TextDisplay : HUDObject
    {
        public string text;
        public Vector2 position;

        public TextDisplay(XmlNode node)
            : base(node)
        {
            this.nameID = node.Attributes["nameID"].Value;
            this.position = new Vector2(int.Parse(node.Attributes["posX"].Value), int.Parse(node.Attributes["posY"].Value));
            if(node.Attributes["text"]!=null)
                this.text = node.Attributes["text"].Value;
       }

    }

    /// <summary>
    /// Used to display Images.
    /// </summary>
    public class ImageDisplay : HUDObject
    {
        string texturePath;
        public Texture2D texture;
        public Rectangle rectangle;

        public ImageDisplay(XmlNode node)
            : base(node)
        {
            this.nameID = node.Attributes["nameID"].Value;
            this.rectangle = new Rectangle(int.Parse(node.Attributes["posX"].Value), int.Parse(node.Attributes["posY"].Value), int.Parse(node.Attributes["width"].Value),int.Parse(node.Attributes["height"].Value));
            this.texturePath = node.Attributes["texture"].Value;

        }

        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.texture = content.Load<Texture2D>(this.texturePath);
        }
    }
    public class SoundObject : HUDObject
    {
        string texturePath;
        public SoundEffect sound;

        public SoundObject(XmlNode node)
            : base(node)
        {
            this.nameID = node.Attributes["nameID"].Value;
            this.texturePath = node.Attributes["effect"].Value;
        }

        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.sound = content.Load<SoundEffect>(this.texturePath);
        }
    }
}
