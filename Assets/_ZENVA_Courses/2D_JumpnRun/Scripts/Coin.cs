using CollectibleSystem;
using InteractableSystem;
using SpawnSystem;
using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour, ICollectible, ISpawnable, IInteractable
{
    [SerializeField]
    private SpawnableDataSO coinData = null;

    public event Func<ISpawnable, bool> DespawnRequested;

    public IDataProvider<ISpawnable> Data => coinData;

    public GameObject GameObject => this.gameObject;

    public string SpawnableType { get; private set; }

    ISpawnableData ISpawnable.Data => throw new System.NotImplementedException();

    IToggleable.ToggleState IToggleable.State => throw new System.NotImplementedException();

    bool IInteractable.CanBeInteractedWith => throw new System.NotImplementedException();

    public void Despawn()
    {
        //coinData.Pool.TryReturn(this);
        this.gameObject.SetActive(false);
    }

    public void Spawn(Vector3 spawnPosition)
    {
        this.transform.position = spawnPosition;
        this.gameObject.SetActive(true);
    }

    public bool TryInitialize(IDataProvider<ISpawnable> data)
    {
        if (data == null)
        {
            return false;
        }

        coinData = data as SpawnableDataSO;
        if (coinData == null)
        {
            return false;
        }

        return true;
    }

    public bool TryInteract<T>(IInteractor interactor, T data = default)
    {
        if ((interactor.InteractorLayer & (1 << LayerMask.NameToLayer("PlayerCharacter"))) == 0)
        {
            return false;
        }

        return TryCollect();
    }

    public bool TryCollect()
    {
        Debug.Log("Coin collected!");
        return true;
    }

    public bool TryCollect<T>(ICollector<T> collector)
    {
        throw new System.NotImplementedException();
    }

    void ISpawnable.Spawn(Vector3 spawnPosition, ISpawnContext context)
    {
        throw new System.NotImplementedException();
    }

    bool ISpawnable.TryInitialize(ISpawnableData data)
    {
        throw new System.NotImplementedException();
    }

    bool IToggleable.TryToggle()
    {
        throw new System.NotImplementedException();
    }

    bool IInteractable.TryInteract(IInteractor interactor)
    {
        throw new System.NotImplementedException();
    }
}