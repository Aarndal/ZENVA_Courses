using Cysharp.Threading.Tasks;
using EventSystem;
using InteractableSystem;
using SpawnSystem;
using System;
using UnityEngine;

using static IToggleable;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour, ISpawnable, IClickable, IScoreChanger, IPublisher
    {
        // Private Member Variables
        private int _counter = 0;
        private bool _isInitialized = false;
        private uint _eventID = 0;
        private IEventChannel<ScoreChangedEventArgs> _scoreChangedChannel = null;

        private BalloonDataSO _data = null;
        private Renderer _renderer = null;
        private ToggleState _toggleState = ToggleState.On;

        // Properties
        public ISpawnableData Data => _data;
        public GameObject GameObject => this.gameObject;
        public string SpawnableType => _data != null ? _data.InstanceName : string.Empty;
        public ToggleState State => _toggleState;
        public int ScoreChangeValue => _data != null ? _data.ScoreValue : 0;

        public uint EventID => _eventID;
        public bool IsAnonymous => false;
        public string UniqueKey => this.gameObject.name;


        public event Func<ISpawnable, bool> DespawnRequested;


        #region Unity Lifecycle Methods
        private void Awake()
        {
            _eventID = EventSystemIDManager.GetParticipantID(this);

            if (!this.transform.TryGetComponentInChildren(out _renderer))
            {
                Debug.LogErrorFormat("Renderer component not found on Spawnable: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }
        #endregion


        #region Private Methods
        private async UniTask AnimatePopAsync()
        {
            _toggleState = ToggleState.Pending;

            Vector3 popScale = this.transform.localScale * 1.25f;
            const float scaleSpeed = 0.75f;
            const float sqrEpsilon = 0.0001f;

            while (Vector3.SqrMagnitude(this.transform.localScale - popScale) > sqrEpsilon)
            {
                this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, popScale, scaleSpeed * Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            this.transform.localScale = popScale;
        }

        private bool TryPublishScoreChanged()
        {
            if (_scoreChangedChannel == null)
            {
                if (!EventTransmitter.TryGetChannel(this, out _scoreChangedChannel))
                {
                    return false;
                }
            }

            return _scoreChangedChannel.TryPublish(new ScoreChangedEventArgs(this, EventFlag.None, this));
        }

        private async UniTask PopBalloonAsync()
        {
            await AnimatePopAsync();
            Despawn();
            TryPublishScoreChanged();
        }

        private bool TryResetCounter()
        {
            if (_data == null)
            {
                Debug.LogErrorFormat("Balloon cannot reset counter because data is missing: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (_data.ClicksToPop <= 0)
            {
                Debug.LogErrorFormat("Data has invalid ClicksToPop value in Balloon script: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());

                return false;
            }

            _counter = _data.ClicksToPop;
            return true;
        }


        #endregion


        #region Public Methods
        public async void Click()
        {
            // Check if the balloon is active and initialized
            if (!this.gameObject.activeInHierarchy || !_isInitialized)
                return;

            if (!TryToggle())
                return;

            // Check if the balloon should start popping
            if (_counter <= 0)
            {
                _toggleState = ToggleState.On;
                await PopBalloonAsync();
                return;
            }

            _toggleState = ToggleState.Off;
        }

        public void Despawn()
        {
            if (DespawnRequested?.Invoke(this) == true)
            {
                _toggleState = ToggleState.On;
                this.gameObject.SetActive(false);
            }
        }

        public void Spawn(Vector3 spawnPosition, ISpawnContext context = null)
        {
            _toggleState = ToggleState.Off;

            this.gameObject.SetActive(true);

            // If we fail to reset the counter, return the balloon to the pool
            if (!TryResetCounter())
            {
                Despawn();
                return;
            }

            this.transform.position = spawnPosition;

            // Reset scale to initial value
            if (_data != null && _isInitialized)
            {
                this.transform.localScale = Vector3.one * _data.InitialScale;
            }
        }

        public bool TryInitialize(ISpawnableData dataProvider)
        {
            // Don't allow re-initialization
            if (_isInitialized)
            {
                Debug.LogWarningFormat("Balloon is already initialized: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (dataProvider == null)
            {
                Debug.LogError("Balloon initialization failed, missing data provider.");
                return false;
            }

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing Renderer: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            return TryInitializeBalloon(dataProvider);
        }

        private bool TryInitializeBalloon(ISpawnableData data)
        {
            _data = data as BalloonDataSO;

            if (_data == null)
            {
                Debug.LogErrorFormat(
                    "Spawnable initialization failed: {0} | ID: {1}\nIncorrect data type: {2} | ID: {3}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId(),
                    data.InstanceName,
                    data.ID);
                return false;
            }

            if (_renderer != null)
                _renderer.material = _data.Material;

            this.transform.localScale = Vector3.one * _data.InitialScale;

            return _isInitialized = true;
        }

        public bool TryToggle()
        {
            // Can only be toggled if currently in the Off state
            if (_toggleState != ToggleState.Off)
                return false;

            _toggleState = ToggleState.Pending;

            _counter--;

            this.transform.localScale += Vector3.one * _data.ScaleFactor;

            return true;
        }

        public bool Equals(IEventParticipant other)
        {
            if (other == null) return false;

            return this.EventID == other.EventID;
        }
        #endregion
    }
}
