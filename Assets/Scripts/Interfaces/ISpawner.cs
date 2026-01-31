
public interface ISpawner<T> where T : IDataProvider
{
    void Configure(ISpawnInstruction<T> instructions);
    void StartSpawning();
    void StopSpawning();
}
