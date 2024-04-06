namespace ASA_Server_Manager.Extensions;

public static class ListExtensions
{
    public static void Clear<T>(this List<T> list, Action<T> action)
    {
        list.ForEach(action);

        list.Clear();
    }

    public static bool Remove<T>(this List<T> list, T item, Action<T> actionOnRemove)
    {
        var result = list.Remove(item);

        if (result)
        {
            actionOnRemove(item);
        }

        return result;
    }
}