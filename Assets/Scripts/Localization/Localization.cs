using System;
using System.Collections.Generic;

/// <summary>
/// 表示言語の種類。設定（GameSettings.Language）とチュートリアルで共通に使う。
/// PlayerPrefs に数値で保存されるので、並び順は変えないこと。
/// </summary>
public enum Language
{
    JP = 0,
    ENG = 1,
}

/// <summary>
/// 日本語 / 英語の文言をまとめて持つ翻訳テーブル。
///
/// 使い方：
///   string s = Localization.Get("result.killCount");            // 今の言語の文言
///   string s = Localization.Format("result.killCount", 12);     // {0} に値を入れる
///   Localization.OnLanguageChanged += Refresh;                   // 言語が変わったら表示を更新
///
/// 文言を足すときは下の table にキーと (日本語, 英語) の組を追加するだけでよい。
/// シーン上の TextMeshPro をキーで差し替えたい場合は LocalizedText コンポーネントを使う。
/// </summary>
public static class Localization
{
    /// <summary>現在の表示言語。設定がまだ無い（エディタで再生していない等）場合は日本語</summary>
    public static Language Current
    {
        get
        {
            GameSettings settings = GameSettings.Instance;
            return (settings != null) ? settings.Language : GameSettings.DefaultLanguage;
        }
    }

    /// <summary>英語表示かどうか</summary>
    public static bool IsEnglish => Current == Language.ENG;

    /// <summary>言語が切り替わったときに呼ばれる（GameSettings から発火される）</summary>
    public static event Action OnLanguageChanged;

    /// <summary>GameSettings が言語を変えたときに呼ぶ。他から直接呼ぶ必要はない</summary>
    public static void RaiseLanguageChanged()
    {
        OnLanguageChanged?.Invoke();
    }

    // ==================================================================
    // 翻訳テーブル（キー → 日本語, 英語）
    // ==================================================================

    private static readonly Dictionary<string, (string jp, string eng)> table = new Dictionary<string, (string jp, string eng)>
    {
        // ----- 設定メニュー -----
        ["settings.invertY"]  = ("上下反転",          "Invert Vertical"),
        ["settings.invertX"]  = ("左右反転",          "Invert Horizontal"),
        ["settings.language"] = ("言語 / Language",   "Language"),

        // ----- アイテム選択カードの説明（キーはアイテム名 label と同じ） -----
        ["item.desc.Gun"]    = ("自機の左右に銃を増設\n一定間隔で追加のショット",
                                "Adds guns on both sides of your ship\nFires extra shots at set intervals"),
        ["item.desc.Fanel"]  = ("自機の軌跡を追うファンネルを追加\nショットに合わせて援護射撃",
                                "Adds a funnel that follows your trail\nFires support shots along with yours"),
        ["item.desc.Laser"]  = ("前方へ強力な弾を\n一定間隔で自動発射",
                                "Auto-fires a powerful shot\nforward at set intervals"),
        ["item.desc.Syabon"] = ("シャボン弾を前方へ乱射\n広範囲をまとめて攻撃",
                                "Sprays bubble shots forward\nHits a wide area at once"),
        ["item.desc.Power"]  = ("すべての弾の攻撃力を強化",
                                "Boosts the power of all your shots"),
        // 名前を変えた後のアイテム（Fanel→Drone, Syabon→Bubble）。古いキーも念のため残している
        ["item.desc.Drone"]  = ("自機の軌跡を追うドローンを追加\nショットに合わせて援護射撃",
                                "Adds drone that follow your ship,\nfiring in sync with your shots."),
        ["item.desc.Bubble"] = ("シャボン弾を前方へ乱射\n広範囲をまとめて攻撃",
                                "Sprays bubble shots forward,\nhitting a wide area."),

        // ----- リザルト画面 -----
        ["result.killCount"] = ("撃墜した敵機の数: {0}", "Enemies Destroyed: {0}"),
    };

    /// <summary>キーに対応する今の言語の文言を返す。無ければキーをそのまま返す</summary>
    public static string Get(string key)
    {
        return Get(key, Current);
    }

    /// <summary>キーに対応する指定言語の文言を返す。無ければキーをそのまま返す</summary>
    public static string Get(string key, Language language)
    {
        if (TryGet(key, language, out string value)) return value;
        return key;
    }

    /// <summary>キーがあれば true と文言を返す（無い場合の分岐を自分で書きたいとき用）</summary>
    public static bool TryGet(string key, out string value)
    {
        return TryGet(key, Current, out value);
    }

    /// <summary>キーがあれば true と指定言語の文言を返す</summary>
    public static bool TryGet(string key, Language language, out string value)
    {
        value = null;
        if (string.IsNullOrEmpty(key)) return false;
        if (!table.TryGetValue(key, out var pair)) return false;

        value = (language == Language.ENG) ? pair.eng : pair.jp;
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>Get した文言を string.Format に通して返す</summary>
    public static string Format(string key, params object[] args)
    {
        return string.Format(Get(key), args);
    }

    /// <summary>
    /// 「日本語の文字列」と「英語の文字列」を直接渡して、今の言語の方を返す。
    /// Inspector に両方の文言を持たせているスクリプト用。
    /// 英語が空なら日本語をそのまま返す（英語未入力でも壊れない）。
    /// </summary>
    public static string Pick(string jp, string eng)
    {
        if (IsEnglish && !string.IsNullOrEmpty(eng)) return eng;
        return jp;
    }
}
