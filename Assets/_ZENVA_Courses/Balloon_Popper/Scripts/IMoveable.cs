using UnityEngine;

public interface IMoveable
{
    void Move(Vector3 direction);
}


public interface IHorizontalMoveable : IMoveable
{
    void MoveHorizontal(Vector2 horizontalDirection);
}

public interface IVerticalMoveable : IMoveable
{
    void MoveVertical(Vector2 verticalDirection);
}