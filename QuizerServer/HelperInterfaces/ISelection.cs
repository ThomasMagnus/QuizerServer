namespace QuizerServer.HelperInterfaces
{
    public interface ISelection<T> where T : class
    {
        public List<T> SelectionValues(Dictionary<string, object> value);
    }
}
