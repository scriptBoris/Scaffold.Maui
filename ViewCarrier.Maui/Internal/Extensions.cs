using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffold.Maui.Internal
{
    internal static class Extensions
    {
        public static T? ItemOrDefault<T>(this IList<T> self, int index)
        {
            if (self.Count == 0)
                return default;

            if (index < 0 || index > self.Count - 1)
                return default;

            return self[index];
        }

        public static Task<bool> EasyAnimate(this View v, string name, double start, double end, ushort length, Action<double> callback)
        {
            var tsc = new TaskCompletionSource<bool>();
            var anim = new Animation(callback, start, end);
            anim.Commit(v, "Anim", length:length, finished: (v, b) =>
            {
                tsc.TrySetResult(b);
            });
            return tsc.Task;
        }
    }
}
