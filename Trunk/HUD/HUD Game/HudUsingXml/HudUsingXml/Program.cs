using System;

namespace HudUsingXml
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HudUsingXml game = new HudUsingXml())
            {
                game.Run();
            }
        }
    }
#endif
}

