public interface IRandomSpawnInterval : ISpawnInterval
{
    float MinInterval { get; }
    float MaxInterval { get; }
}