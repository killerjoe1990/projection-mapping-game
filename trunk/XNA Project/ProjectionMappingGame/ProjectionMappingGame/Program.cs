using System;

namespace ProjectionMappingGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameDriver game = new GameDriver())
            {
                game.Run();
            }
        }
    }
#endif
}

