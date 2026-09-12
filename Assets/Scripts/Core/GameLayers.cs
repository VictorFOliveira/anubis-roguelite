namespace Anubis.Core
{
    public static class GameLayers
    {
        public const int Default = 0;
        public const int Player = 6;
        public const int Enemy = 7;
        public const int PlayerHurtbox = 8;
        public const int EnemyHurtbox = 9;
        public const int Projectile = 10;
        public const int Environment = 11;

        public static int PlayerMask => 1 << Player;
        public static int EnemyMask => 1 << Enemy;
        public static int EnvironmentMask => 1 << Environment;
    }

    public static class GameSorting
    {
        public const int Floor = -20;
        public const int Environment = -10;
        public const int Entities = 0;
        public const int Vfx = 10;
    }
}
