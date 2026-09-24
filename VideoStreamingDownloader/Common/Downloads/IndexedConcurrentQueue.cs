using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal class IndexedConcurrentQueue : IEnumerable<Downloader>
    {
        ConcurrentQueue<Downloader> stack = new ConcurrentQueue<Downloader>();
        HashSet<string> ids = new HashSet<string>();
        internal int Count => stack.Count;

        internal bool TryPush(Downloader item)
        {
            bool IsAdded = ids.Add(item.FileId);

            if (IsAdded)
                stack.Enqueue(item);

            return IsAdded;
        }

        internal bool TryPop(out Downloader item)
        {
            if (stack.TryDequeue(out item))
            {
                ids.Remove(item.FileId);
                return true;
            }

            item = null;
            return false;
        }

        public IEnumerator<Downloader> GetEnumerator()
        {
            return stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
