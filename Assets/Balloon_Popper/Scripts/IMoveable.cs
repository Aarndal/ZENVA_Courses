public interface IMoveable
{
    void Move();
}


public interface IHorizontalMoveable : IMoveable
{
    void MoveHorizontal();
}

public interface IVerticalMoveable : IMoveable
{
    void MoveVertical();
}