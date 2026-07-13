using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテム選択ボタンのカード型表示（アイテム名 / Lv表記 / アイコン / 説明文）。
/// ボタン本体にアタッチし、各表示要素への参照を持つ。
/// 表示内容の差し替えは ItemSelectRandomizer が Apply() を呼んで行う。
/// </summary>
public class ItemButtonView : MonoBehaviour
{
    [Tooltip("アイテム名（上段）")]
    public TextMeshProUGUI nameLabel;
    [Tooltip("段階表示（名前の下、例: Lv 0/4）")]
    public TextMeshProUGUI levelLabel;
    [Tooltip("アイテムのアイコン画像（中央）。スプライト未設定のアイテムでは自動で非表示になる")]
    public Image iconImage;
    [Tooltip("説明文（下段）")]
    public TextMeshProUGUI descLabel;

    public void Apply(string itemName, string levelText, Sprite icon, string description)
    {
        if (nameLabel != null) nameLabel.text = itemName;
        if (levelLabel != null) levelLabel.text = levelText;
        if (descLabel != null) descLabel.text = description;
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = (icon != null); // 画像未設定なら白い四角を出さない
        }
    }
}
