namespace TriggerResponderSystem
{
    internal interface IResponsive<T> where T : class
    {
        bool TryGetResponse(T trigger, IDataProvider<T> context);
    }
}
