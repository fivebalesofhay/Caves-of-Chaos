public static class GameSettings
{
    // General settings:
    public const int GAME_WIDTH = 60;
    public const int GAME_HEIGHT = 60;
    public const int GRID_CONSOLE_WIDTH = 41;
    public const int GRID_CONSOLE_HEIGHT = 41;
    public const int LARGE_SCREEN_WIDTH = 40;
    public const int LARGE_SCREEN_HEIGHT = 40;
    public const int SMALL_SCREEN_WIDTH = 32;
    public const int SMALL_SCREEN_HEIGHT = 32;

    // Grid layout settings:
    public const int WALKER_STEPS = 500;

    // Player settings:
    public const int PLAYER_FOV_DISTANCE = 8;
    public const int EXP_COEFFICIENT = 5;
    public const int LEVELS_PER_STAT_INCREASE = 2;

    // Creature settings:
    public const double BASE_MOVEMENT_TIME = 10;
    public const double BASE_HEALING_RATE = 0.002; // In percent of max health per tu
    public const double STRENGTH_DAMAGE_MULTIPLIER = 0.1;
}
