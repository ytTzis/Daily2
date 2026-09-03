using UnityEngine;

public class PageController : MonoBehaviour
{
    [SerializeField] private GameObject drawPanel;
    [SerializeField] private GameObject galleryPanel;

    public void ShowDraw()
    {
        drawPanel.SetActive(true);
        galleryPanel.SetActive(false);
    }

    public void ShowGallery()
    {
        drawPanel.SetActive(false);
        galleryPanel.SetActive(true);
    }
}