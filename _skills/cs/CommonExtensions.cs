using System;
using System.Collections.Generic;
using System.Linq;

public static class RangeExtensions {
    public static IEnumerator<int> GetEnumerator(this Range range) => range.Start.IsFromEnd || range.End.IsFromEnd
        ? throw new ArgumentException("Range bounds count from the start here.", nameof(range))
        : Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value).GetEnumerator();
}

public static class IntExtensions {
    extension(int n) {
        public void Times(Action action) => n.Times(_ => action());

        public void Times(Action<int> action) {
            foreach (int i in ..n) {
                action(i);
            }
        }
    }
}

public static class EnumerableExtensions {
    extension<T>(IEnumerable<T> enumerable) {
        public void ForEach(Action<T> action) {
            foreach (T t in enumerable) {
                action(t);
            }
        }

        public void ForEach(Action<T, int> action) {
            int i = 0;
            foreach (T t in enumerable) {
                action(t, i);
                i++;
            }
        }
    }
}
