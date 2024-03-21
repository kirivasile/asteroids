namespace Asteroids.Utils {
    // Mark the value of the class `T` to remind you that the value is normalized.
    // `Normalized` means different value based on the type `T`
    // e.g. normalized versions of the `Vector3`
    public readonly struct Normalized<T> {
        public readonly T value;

        public Normalized(T value) {
            this.value = value;
        }
    }
}