using System;

namespace SoftProGame2014
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (OutOfOfficeGame game = new OutOfOfficeGame())
            {
                game.Run();
            }
        }
    }
#endif
}

