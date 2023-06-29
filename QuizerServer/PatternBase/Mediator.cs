namespace QuizerServer.PatternBase
{
    public abstract class Mediator
    {
        public abstract void Send(string message, MediatorRequests request);
    }
}
