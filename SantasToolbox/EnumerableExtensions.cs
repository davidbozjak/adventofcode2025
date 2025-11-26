namespace SantasToolbox;

public static class EnumerableExtensions
{
    /// <summary>
    /// Provides all different sublists of k elements from enumerable
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ? new[] { Array.Empty<T>() } :
          elements.SelectMany((e, i) =>
            elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }

    public static T GetMostFrequentElement<T>(this IEnumerable<T> elements)
    {
        return elements.GroupBy(w => w).OrderByDescending(w => w.Count()).Select(w => w.Key).First();
    }

    public static Stack<T> ToStack<T>(this IEnumerable<T> elements)
    {
        var stack = new Stack<T>();

        foreach (var element in elements)
        {
            stack.Push(element);
        }

        return stack;
    }

    public static int IndexOfFirst<T>(this IEnumerable<T> elements, Func<T, bool> condition)
    {
        var index = 0;

        foreach (var element in elements)
        {
            if (condition(element))
            {
                return index;
            }
            index++;
        }

        return -1;
    }

    public static int IndexOfFirst<T>(this IEnumerable<T> elements, T element)
    {
        var index = 0;

        foreach (var e in elements)
        {
            if (element.Equals(e))
            {
                return index;
            }
            index++;
        }

        return -1;
    }
}
