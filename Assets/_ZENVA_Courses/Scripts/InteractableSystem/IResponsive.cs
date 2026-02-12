namespace TriggerResponderSystem
{
    internal interface IResponsive
    {
        bool TryGetResponse(ITriggerable trigger);
    }
}
