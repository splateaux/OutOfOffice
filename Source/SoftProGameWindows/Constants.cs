using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    public static class Constants
    {
        // Point value names, use these against the ObjectValueManager to get a point value for something
        public const string SOFTPRO_DOLLAR_VALUE = "SoftProDollarValue";
        public const string ENEMY_KILL_VALUE = "EnemyKillValue";
        public const string ENEMY_DAMAGE_VALUE_SECONDS = "EnemyDamageValueInSeconds";
        public const string PLAYER_INVINCIBILITY_VALUE_SECONDS = "PlayerInvisibilityValueInSeconds";
        public const string MAZE_GAME_COMPLETE = "CompleteMazeGame";
        public const string CLOCK_PER_SECOND = "SecondsLeftOnClock";
        public const string QUIZ_CORRECT_ANSWER = "QuizCorrectAnswer";
        public const string QUIZ_INCORRECT_ANSWER = "QuizIncorrectAnswer";
        public const string MAX_POINTS_AWARDED_FOR_TIME = "MaxPointsAwardedForTime";

        // content paths for shared items
        public const string SOFTPRODOLLAR_TEXTURE = "Textures/Objects/SoftProDollar";
        public const string SOFTPRODOLLAR_SOUND = "Audio/Effects/SoftProDollarCollected";
        public const string MAINGAME_BACKGROUND_TEXTURE = "Textures/Backgrounds/Opening/OpeningPage";

    }
}
