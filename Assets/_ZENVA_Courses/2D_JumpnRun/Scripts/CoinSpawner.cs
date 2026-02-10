using UnityEngine;

public class CoinSpawner : MonoBehaviour, ISpawner, IInteractable
{
    [SerializeField]
    private Coin coin = null;

    public void StartSpawning()
    {
        coin.Spawn(this.transform.position + Vector3.up);
    }

    public void StopSpawning()
    {
        throw new System.NotImplementedException();
    }

    public bool TryInteract<T>(IInteractor interactor, T data = default)
    {
        if ((interactor.InteractorLayer & (1 << LayerMask.NameToLayer("PlayerCharacter"))) == 0)
        {
            return false;
        }
        StartSpawning();
        return true;
    }
}

