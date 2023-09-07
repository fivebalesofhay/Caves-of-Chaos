using System;
using Caves_of_Chaos.CreatureScripts;
using Caves_of_Chaos.GridScripts;
using SadConsole;
using SadRogue.Primitives;

namespace Caves_of_Chaos
{
    public static class Program
    {
        public static GameContainer container = new GameContainer();
        public static Random random = new Random();

        static void Main()
        {
            // Setup the engine and create the main window.
            Game.Create(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT, "Cheepicus12.font");

            Game.Instance.OnStart = Init;
            
            // Start the game.
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        public static void Init()
        {
            // Change font:
            var font = Game.Instance.LoadFont("Cheepicus12.font");
            Game.Instance.DefaultFont = font;

            Settings.WindowTitle = "Caves of Chaos";

            // Fill Palette dictionary:
            Palette.Init();

            // Make the GameContainer the game's Screen
            Game.Instance.Screen = container;
            Game.Instance.DestroyDefaultStartingConsole();

            // Initialize grid and generate layout
            GridManager.Init();

            // Spawn player
            PlayerManager.Init();
        }
    }
}