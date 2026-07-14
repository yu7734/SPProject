using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Imageに付けるだけでグラデーションが掛かるシンプルなスクリプト。
/// 縦・横・斜めの4方向に対応。
/// 既存のImageコンポーネントを置き換えるのではなく、上から重ねて頂点色を変更する仕組み。
/// 他のスクリプト（HPBar、UIEffectなど）と併用可能。
/// </summary>
[RequireComponent(typeof(Graphic))]
[AddComponentMenu("UI/Effects/Simple UI Gradient")]
public class SimpleUIGradient : BaseMeshEffect
{
    public enum Direction
    {
        Vertical,           // 上→下
        Horizontal,         // 左→右
        DiagonalLeftToRight,// 左下→右上
        DiagonalRightToLeft // 右下→左上
    }

    [SerializeField, Header("方向")] private Direction direction = Direction.Horizontal;
    [SerializeField, Header("開始色")] private Color startColor = Color.white;
    [SerializeField, Header("終了色")] private Color endColor = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0) return;

        var vertices = new System.Collections.Generic.List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        // バウンディングボックスを計算
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        for (int i = 0; i < vertices.Count; i++)
        {
            var p = vertices[i].position;
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        float width = Mathf.Max(0.0001f, maxX - minX);
        float height = Mathf.Max(0.0001f, maxY - minY);

        // 各頂点の色を補間
        for (int i = 0; i < vertices.Count; i++)
        {
            var v = vertices[i];
            float t = 0f;
            switch (direction)
            {
                case Direction.Vertical:
                    t = 1f - (v.position.y - minY) / height; // 上が startColor
                    break;
                case Direction.Horizontal:
                    t = (v.position.x - minX) / width; // 左が startColor
                    break;
                case Direction.DiagonalLeftToRight:
                    t = ((v.position.x - minX) / width + (v.position.y - minY) / height) * 0.5f;
                    break;
                case Direction.DiagonalRightToLeft:
                    t = ((maxX - v.position.x) / width + (v.position.y - minY) / height) * 0.5f;
                    break;
            }
            v.color = Color.Lerp(startColor, endColor, t) * v.color;
            vertices[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }
}
