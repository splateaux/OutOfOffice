using Microsoft.Xna.Framework.Graphics;

namespace SoftProGame2014
{
    public class AtlasInfo
    {
        public Texture2D Texture;
        public int Columns;
        public int Rows;
        public int RightMovingAnimationStartFrame;
        public int RightMovingAnimationEndFrame;
        public int LeftMovingAnimationStartFrame;
        public int LeftMovingAnimationEndFrame;
        public int JumpingLeftFrame;
        public int JumpingRightFrame;

        public AtlasInfo(
            Texture2D texture,
            int columns,
            int rows,
            int rightMovingStartFrame,
            int rightMovingEndFrame,
            int leftMovingStartFrame,
            int leftMovingEndFrame,
            int jumpLeftFrame,
            int jumpRightFrame)
        {
            Texture = texture;
            Columns = columns;
            Rows = rows;
            RightMovingAnimationStartFrame = rightMovingStartFrame;
            RightMovingAnimationEndFrame = rightMovingEndFrame;
            LeftMovingAnimationStartFrame = leftMovingStartFrame;
            LeftMovingAnimationEndFrame = leftMovingEndFrame;
            JumpingLeftFrame = jumpLeftFrame;
            JumpingRightFrame = jumpRightFrame;
        }
    }
}