namespace SoftProGameEngine.Framework
{
    public class EnemyStatus
    {
        public bool IsLeft { get; set; }

        public bool IsRight { get; set; }

        public bool IsTop { get; set; }

        public bool IsBottom { get; set; }

        public EnemyStatus()
        {
            IsBottom = false;
            IsLeft = false;
            IsRight = false;
            IsTop = false;
        }

        public static EnemyStatus TotalEnemy()
        {
            return new EnemyStatus { IsBottom = true, IsLeft = true, IsRight = true, IsTop = true };
        }

        public static EnemyStatus AllBut(Direction direction)
        {
            EnemyStatus enemyStatus = TotalEnemy();

            switch (direction)
            {
                case Direction.Down:
                    enemyStatus.IsBottom = false;
                    break;

                case Direction.Up:
                    enemyStatus.IsTop = false;
                    break;

                case Direction.Right:
                    enemyStatus.IsRight = false;
                    break;

                case Direction.Left:
                    enemyStatus.IsLeft = false;
                    break;
            }

            return enemyStatus;
        }
    }
}