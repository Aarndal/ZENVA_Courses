
public interface ISpawner<T>
{
    void Configure(ISpawnerInstruction<T> instructions);
    void StartSpawning();
    void StopSpawning();
}
