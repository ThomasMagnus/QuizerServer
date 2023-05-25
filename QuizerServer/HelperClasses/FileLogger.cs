namespace QuizerServer.HelperClasses
{
    public class FileLogger : ILogger, IDisposable
    {
        private string filePath;
        private static object _lock = new object();

        public FileLogger(string filePath) => this.filePath = filePath;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => this;

        public void Dispose() { }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (_lock)
            {
                File.AppendAllText(filePath, formatter(state, exception) + Environment.NewLine);
            }
        }
    }
}
