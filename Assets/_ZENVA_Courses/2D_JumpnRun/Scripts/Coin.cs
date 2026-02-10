using BalloonPopper;
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectible, ISpawnable, IInteractable
{
    [SerializeField]
    private SpawnableDataProviderSO coinData = null;

    public IDataProvider<ISpawnable> Data => coinData;

    public GameObject GameObject => this.gameObject;

    public string SpawnableType { get; private set; }

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

        coinData = data as SpawnableDataProviderSO;
        if (coinData == null)
        {
            return false;
        }

        return true;
    }

    public bool TryInteract<T>(IInteractor interactor, T data = default)
    {
        if ((interactor.InteractorLayer & (1 << LayerMask.NameToLayer("Player"))) == 0)
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
}