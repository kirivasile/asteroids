using System;
using System.Collections.Generic;

namespace Asteroids.Utils {
    public interface IDisposableTracker {
        void Track(IDisposable disposable);

        void Track(Action action) => Track(new ActionDisposable(action));
    }

    public interface IDisposableController : IDisposableTracker, IDisposable {
    }

    public class DisposableController : IDisposableController {
        readonly List<IDisposable> _disposables;

        public DisposableController() {
            _disposables = new();
        }

        public void Dispose() {
            foreach (var disp in _disposables) disp.Dispose();

            _disposables.Clear();
        }

        public void Track(IDisposable disposable) {
            _disposables.Add(disposable);
        }
    }

    // Converter from `Action` to `Disposable`
    // Struct, because it can be created often.
    public struct ActionDisposable : IDisposable {
        readonly Action _callback;

        public ActionDisposable(Action callback) {
            _callback = callback;
        }

        public void Dispose() {
            _callback?.Invoke();
        }
    }

    // Used for objects that shouldn't be disposable.
    // e.g. subscriptions that should not be unsubscribed
    public class NoDisposableController : IDisposableController {
        public void Dispose() {
        }

        public void Track(IDisposable disposable) {
        }
    }
}