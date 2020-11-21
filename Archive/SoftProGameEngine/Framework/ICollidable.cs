namespace SoftProGameEngine.Framework
{
    public interface ICollidable
    {
        bool HandlesCollisions { get; }

        void OnBottomCollision(object collider);

        void OnTopCollision(object collider);

        void OnLeftCollision(object collider);

        void OnRightCollision(object collider);
    }
}