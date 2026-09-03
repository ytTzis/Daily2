using System.Collections.Generic;
using UnityEngine;

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
    }

    private void OnDisable()
    {
        service.CollectionChanged -= Refresh;
    }

    private void CreateItems()
    {
        foreach (ItemDefinition item in service.Items)
        {
            GalleryItemView view =
                Instantiate(itemPrefab, content);

            views.Add(view);
        }
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