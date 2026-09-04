using TMPro;
using UnityEngine;

/// <summary>
/// シーン上の TextMeshPro にアタッチすると、Localization のキーに対応する文言を
/// 自動で表示し、設定で言語が切り替わったときにも自動で書き換わる。
///
/// 使い方：
///   1. 文字を出している TextMeshProUGUI と同じオブジェクトにこのコンポーネントを付ける
///   2. Key に Localization の table にあるキー（例：result.killCount）を入れる
///   3. 必要なら日本語用 / 英語用のフォントを入れる（空なら今のフォントのまま）
///
/// キーを使わず、Inspector に日本語と英語を直接書きたい場合は
/// Key を空にして Japanese / English 欄に文言を入れる。
/// </summary>
public class LocalizedText : MonoBehaviour
{
    [SerializeField, Tooltip("Localization の table にあるキー。空なら下の日本語/英語欄を使う")]
    private string key = "";

    [SerializeField, TextArea(1, 3), Tooltip("キーを使わない場合の日本語の文言")]
    private string japanese = "";

    [SerializeField, TextArea(1, 3), Tooltip("キーを使わない場合の英語の文言。空なら日本語がそのまま出る")]
    private string english = "";

    [Header("フォント（空なら変更しない）")]
    [SerializeField, Tooltip("日本語表示のときに使うフォント")]
    private TMP_FontAsset japaneseFont;

    [SerializeField, Tooltip("英語表示のときに使うフォント")]
    private TMP_FontAsset englishFont;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Localization.OnLanguageChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        Localization.OnLanguageChanged -= Refresh;
    }

    /// <summary>今の言語で表示を更新する</summary>
    public void Refresh()
    {
        if (text == null) text = GetComponent<TMP_Text>();
        if (text == null) return;

        string value = !string.IsNullOrEmpty(key)
            ? Localization.Get(key)
            : Localization.Pick(japanese, english);
        text.text = value;

        TMP_FontAsset font = Localization.IsEnglish ? englishFont : japaneseFont;
        if (font != null)
        {
            text.font = font;
        }
    }

    /// <summary>コードからキーを差し替えたいとき用</summary>
    public void SetKey(string newKey)
    {
        key = newKey;
        Refresh();
    }
}
