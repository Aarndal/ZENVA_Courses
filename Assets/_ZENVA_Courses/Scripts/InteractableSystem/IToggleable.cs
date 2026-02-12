public interface IToggleable
{
    public enum ToggleState : byte
    {
        Active = 0,
        Inactive = 1,
        Cooldown = 2,
    }

    ToggleState State { get; }

    bool TryToggle();
}