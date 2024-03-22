using System;

public static class ArrayExts {
    public static B[] Map<A, B>(this A[] arr, Func<A, B> mapper) {
        var result = new B[arr.Length];
        for (var i = 0; i < arr.Length; ++i) {
            result[i] = mapper(arr[i]);
        }
        return result;
    }
}