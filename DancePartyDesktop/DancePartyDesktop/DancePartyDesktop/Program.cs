using System;

using DanceParty;

namespace DancePartyDesktop
{
#if WINDOWS_DESKTOP || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (DancePartyGame game = new DancePartyGame())
            {
                game.Run();
            }
        }
    }
#endif
}

