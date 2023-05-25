using Quizer.Context;

namespace Quizer.IServices
{
    public interface ICollectionProperty<T, K> where T : class where K : class
    {
        public T? _applicationContext { get; }
        public List<K> GetCollectionList(Func<T, List<K>> func);
    }
}
