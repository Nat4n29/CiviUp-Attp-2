using UnityEngine;

public class MapWrapVisual : MonoBehaviour
{
    [Header("References")]
    public Transform baseMapLayer;
    public HexMapGenerator generator;

    private Transform leftClone;
    private Transform rightClone;
    private float mapWidthWorld;

    public void BuildVisualWrap()
    {
        ClearClones();

        if (baseMapLayer == null || generator == null)
            return;

        float hexWidth = generator.HexWidth;

        mapWidthWorld =
            (generator.width - 1) * hexWidth * 0.985f + hexWidth;

        leftClone = Instantiate(baseMapLayer, transform);
        rightClone = Instantiate(baseMapLayer, transform);

        leftClone.name = "Map_Left";
        rightClone.name = "Map_Right";

        leftClone.position = baseMapLayer.position + Vector3.left * mapWidthWorld;
        rightClone.position = baseMapLayer.position + Vector3.right * mapWidthWorld;

        LinkCloneData(leftClone);
        LinkCloneData(rightClone);
    }

    private void ClearClones()
    {
        if (leftClone != null) Destroy(leftClone.gameObject);
        if (rightClone != null) Destroy(rightClone.gameObject);
    }

    private void LinkCloneData(Transform cloneRoot)
    {
        ProvinceView[] originalViews =
            baseMapLayer.GetComponentsInChildren<ProvinceView>();

        ProvinceView[] cloneViews =
            cloneRoot.GetComponentsInChildren<ProvinceView>();

        for (int i = 0; i < cloneViews.Length; i++)
        {
            cloneViews[i].Init(originalViews[i].Data);
        }
    }
}
