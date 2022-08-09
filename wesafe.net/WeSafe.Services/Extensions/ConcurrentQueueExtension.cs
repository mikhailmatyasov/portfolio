using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeSafe.Services.Extensions
{
    public static class ConcurrentQueueExtension
    {
        public static void EnqueueRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
            {
                queue.Enqueue(element);
            }
        }

        public static IEnumerable<T> DequeueRange<T>(this ConcurrentQueue<T> queue)
        {
            List<T> list = new List<T>();

            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out T item);
                list.Add(item);
            }

            return list;
        }
    }
}
