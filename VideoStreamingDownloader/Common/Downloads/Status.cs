namespace VideoStreamingDownloader.Common.Downloads
{
    internal class Status
    {
        private int _maxValue;
        internal int Progress { get; private set; } = 0;
        internal string Message { get; private set; } = string.Empty;

        internal Status(int maxValue)
        {
            _maxValue = maxValue;
        }

        internal void SetProgressValue(int actualValue)
        {
            Progress = actualValue >= _maxValue ? 99 : (int)(actualValue * 100.0 / _maxValue);
        }

        internal void Itereate()
        {
            if (Progress < 99)
                Progress++;
        }

        internal void SetMessage(string message)
        {
            Message = message;
        }
    }
}
