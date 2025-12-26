using System.Collections.Generic;

public static class ProvinceViewRegistry
{
    private static readonly Dictionary<ProvinceData, List<ProvinceView>> map = new();

    public static void Register(ProvinceView view)
    {
        if (view.Data == null)
            return;

        if (!map.ContainsKey(view.Data))
            map[view.Data] = new List<ProvinceView>();

        map[view.Data].Add(view);
    }

    public static void Unregister(ProvinceView view)
    {
        if (view.Data == null)
            return;

        if (map.TryGetValue(view.Data, out var list))
            list.Remove(view);
    }

    public static IEnumerable<ProvinceView> GetViews(ProvinceData data)
    {
        if (data == null || !map.ContainsKey(data))
            yield break;

        foreach (var view in map[data])
            yield return view;
    }
}
