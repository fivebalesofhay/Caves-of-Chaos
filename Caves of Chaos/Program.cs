using System;
using SadConsole;
using SadRogue.Primitives;

namespace Caves_of_Chaos
{
    public static class Program
    {
        static void Main()
        {
            // Setup the engine and create the main window.
            Game.Create(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT, "Cheepicus12.font");
            
            Game.Instance.OnStart = Init;
            
            // Start the game.
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        static void Init()
        {
            var font = Game.Instance.LoadFont("Cheepicus12.font");
            Game.Instance.DefaultFont = font;

            Windows.Init();
        }
    }
}