public interface IToggleable
{
    public enum ToggleState : byte
    {
        Off = 0,
        On = 1,
        Pending = 2,
    }

    ToggleState State { get; }

    bool TryToggle();
}
