using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Utils {
    public interface IPoolView {
        void OnRelease();
    }

    // Wrapper for `GameObjectPool`. Allows you to track active objects.
    public abstract class PooledObjectsController<TView, TInit, TData> where TView : MonoBehaviour, IPoolView  {
        protected readonly GameObjectPool<TView> _viewPool;
        protected readonly HashSet<TInit> _activeObjects;
        protected readonly CreateInit _createInit;
        protected readonly GetView _getView;
        protected readonly GetPosition _getPosition;

        public HashSet<TInit> ActiveObjects => _activeObjects;

        public PooledObjectsController(
            int poolInitialSize, CreateView createView, CreateInit createInit, GetView getView, GetPosition getPosition
        ) {
            _viewPool = new(poolInitialSize, () => createView());
            _activeObjects = new();
            _createInit = createInit;
            _getView = getView;
            _getPosition = getPosition;
        }

        protected void AddNewObjectToActive(TData data) {
            var view = _viewPool.Borrow(_getPosition(data));
            var init = _createInit(view, data);
            _activeObjects.Add(init).ForSideEffects();
        }

        public void Release(TInit obj) {
            var view = _getView(obj);
            view.OnRelease();

            if (_activeObjects.Remove(obj)) {
                _viewPool.Release(view);
            }
        }

        public virtual void Disable() {
            foreach (var obj in _activeObjects) {
                var view = _getView(obj);
                view.OnRelease();
                _viewPool.Release(view);
            }

            _activeObjects.Clear();
        }

        public delegate TView CreateView();
        public delegate TInit CreateInit(TView view, TData data);
        public delegate TView GetView(TInit init);
        public delegate Vector3 GetPosition(TData data);
    }

    // Version of `PooledObjectsController` where objects are spawned on timer.
    public class PooledObjectsOnTimer<TView, TInit, TData> : PooledObjectsController<TView, TInit, TData> where TView : MonoBehaviour, IPoolView {
        readonly float _spawnPeriod;
        // We use fixed data for the objects that were spawned on timer
        readonly TData _fixedData;

        bool _isEnabled;
        Option<float> _lastTimeSpawned;

        public PooledObjectsOnTimer(
            int poolInitialSize, float spawnPeriod, CreateView createView, CreateInit createInit, GetView getView, 
            GetPosition getPosition, TData data
        ) : base(poolInitialSize, createView, createInit, getView, getPosition) {
            _spawnPeriod = spawnPeriod;
            _fixedData = data;
        }

        public void Enable() {
            _isEnabled = true;
            _lastTimeSpawned = Some._(Time.time);
        }

        public void OnUpdate() {
            if (!_isEnabled) return;

            if (_lastTimeSpawned.Value(out var lastTimeSpawned) && Time.time >= lastTimeSpawned + _spawnPeriod) {
                AddNewObjectToActive(_fixedData);
                _lastTimeSpawned = Some._(Time.time);
            }
        }

        public override void Disable() {
            base.Disable();
            _isEnabled = false;
            _lastTimeSpawned = None._;
        }
    }

    // Version of `PooledObjectsController` where objects are spawned on event.
    public class PooledObjectsOnEvent<TView, TInit, TData> : PooledObjectsController<TView, TInit, TData> where TView : MonoBehaviour, IPoolView {
        public PooledObjectsOnEvent(
            int poolInitialSize, CreateView createView, CreateInit createInit, GetView getView, GetPosition getPosition
        ) : base(poolInitialSize, createView, createInit, getView, getPosition) {
        }

        public void SpawnObject(TData data) => AddNewObjectToActive(data);
    }
}