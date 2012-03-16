using System;
using System.Windows.Forms;

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
               Form form = (Form)Form.FromHandle(game.Window.Handle);
               form.FormBorderStyle = FormBorderStyle.None;
                game.Run();
            }
        }
    }
#endif
}

