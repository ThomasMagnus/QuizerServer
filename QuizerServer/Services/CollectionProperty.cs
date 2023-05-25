using Quizer.IServices;
using Quizer.Context;

namespace Quizer.Services
{
    public class CollectionProperty<T, K> : ICollectionProperty<T, K> where T : class where K : class
    {
        public T? _applicationContext { get; set; }

        public List<K> GetCollectionList(Func<T, List<K>> func) {
            List<K> result = func(_applicationContext!);
            return result;
        }
    }
}
