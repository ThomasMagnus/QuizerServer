namespace QuizerServer.PatternBase
{
    public abstract class MediatorRequests
    {
        protected Mediator _mediator;
        public MediatorRequests(Mediator mediator) => _mediator = mediator;
        public virtual void Send(string message) { _mediator.Send(message, this); }
        public abstract void Notify(string message);
    }
}
