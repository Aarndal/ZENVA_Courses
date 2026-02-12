namespace TriggerResponderSystem
{
    public interface ITriggerable
    {
        ITriggerContext Context { get; }

        bool TryTrigger(ITriggerContext context = default);
    }
}