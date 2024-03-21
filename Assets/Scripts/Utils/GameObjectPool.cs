using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Utils {
    public class GameObjectPool<T> where T: MonoBehaviour {
        readonly Stack<T> _pool;
        readonly Func<T> _createFunction;

        public GameObjectPool(int poolInitialSize, Func<T> createFunction) {
            _createFunction = createFunction;

            _pool = new Stack<T>(poolInitialSize);

            for (var i = 0; i < poolInitialSize; ++i) {
                Release(createFunction());
            }
        }

        public T Borrow(Vector3 position) {
            var value = _pool.Count > 0 ? _pool.Pop() : _createFunction();
            
            value.transform.position = position;
            value.gameObject.SetActive(true);
            return value;
        }

        public void Release(T value) {
            value.gameObject.SetActive(false);
            _pool.Push(value);
        }
    }
}