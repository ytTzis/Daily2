using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryPanel : MonoBehaviour
{
    [SerializeField] private BlindBoxService service;
    [SerializeField] private GalleryItemView itemPrefab;
    [SerializeField] private Transform content;

    private readonly List<GalleryItemView> views = new();

    private void Start()
    {
        CreateItems();
        Refresh();
    }

    private void OnEnable()
    {
        service.CollectionChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        service.CollectionChanged -= Refresh;
    }

    private void CreateItems()
{
    views.Clear();

    GalleryItemView[] existingViews =
        content.GetComponentsInChildren<GalleryItemView>(true);

    views.AddRange(existingViews);
}

    private void Refresh()
    {
        if (views.Count == 0)
            return;

        for (int i = 0; i < service.Items.Count; i++)
        {
            ItemDefinition item = service.Items[i];

            views[i].Bind(
                item,
                service.IsUnlocked(item.itemId));
        }
    }
}
