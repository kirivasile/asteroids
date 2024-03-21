using System;

namespace Asteroids.Utils {
    // Custom simple implementation of the optional type
    public struct Option<T> {
        public static Option<T> Some(T value) => new Option<T>(value);

        public static implicit operator Option<T>(None none) => default;

        private readonly bool _isSome;
        private readonly T _unsafeValue;

        private Option(T value) {
            _unsafeValue = value;
            _isSome = value is not null;
        }

        public bool Value(out T value) {
            value = _unsafeValue;
            return _isSome;
        }

        public bool IsSome => _isSome;
        public bool IsNone => !_isSome;

        // Convertes `Option<T>` to `Option<U>` with provided `mapper`
        public Option<U> Map<U>(Func<T, U> mapper) => 
            _isSome ? new Option<U>(mapper(_unsafeValue)) : None._;
    }

    public struct None {
        public static None _;
    }

    public struct Some {
        // Shortcut for the Option<T>.Some method
        public static Option<T> _<T>(T value) => Option<T>.Some(value);
    }

    public static class BoolExts {
        // Allows you to construct `Option` base on the boolean value `flag`
        public static Option<T> ToOption<T>(this bool flag, T value) => flag ? Some._(value) : None._;
    }

    // public static class OptionExts {
    //     // public static Option<U> Upcast<T, U>(this Option<T> opt) where T: U => opt.Map<U>(t => t);
    // }
}
