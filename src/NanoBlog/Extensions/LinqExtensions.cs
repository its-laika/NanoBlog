namespace NanoBlog.Extensions;

using System.Linq;

public static class LinqExtensions
{
    public static IDictionary<TKey, TElement> ToDictionary<TKey, TElement, TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, int, TKey> keySelector,
        Func<TSource, int, TElement> valueSelector
    )
        where TKey : notnull
    {
        return source
            .Select((element, index) => (index, element))
            .ToDictionary(
                map => keySelector(map.element, map.index),
                map => valueSelector(map.element, map.index)
            );
    }
}
