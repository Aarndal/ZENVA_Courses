namespace TriggerResponderSystem
{
    public interface ITriggerable
    {
        bool TryTrigger(ITriggerContext context);
    }
}
