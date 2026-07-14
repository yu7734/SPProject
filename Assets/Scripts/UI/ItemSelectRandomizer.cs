using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// アイテム選択パネル（SerectItemImage）にアタッチして使う。
/// パネルが開く（SetActive(true)される）たびに、候補プールから重複なしで抽選し、
/// 各ボタンのラベル・色・押したときの処理を差し替える。
/// </summary>
public class ItemSelectRandomizer : MonoBehaviour
{
    public enum ItemAction { HPUp, PowerUp, LaserOrPower, AddFanel, AddGun, AddSyabon }

    [Serializable]
    public class ItemOption
    {
        public string label = "Item";
        public Color color = Color.white;
        public ItemAction action = ItemAction.PowerUp;
        [Tooltip("カード中央に表示するアイコン。未設定なら非表示")]
        public Sprite icon;
        [Tooltip("カード下段に表示する説明文"), TextArea(1, 3)]
        public string description = "";
    }

    [Header("抽選候補（Inspectorで追加・変更可）")]
    [SerializeField]
    private ItemOption[] itemPool =
    {
        new ItemOption { label = "Gun",    color = new Color(0.45f, 0.85f, 0.45f), action = ItemAction.AddGun,       description = "サブの銃を増設して\n同時に攻撃する" },       // 緑
        new ItemOption { label = "Fanel",  color = new Color(0.90f, 0.40f, 0.40f), action = ItemAction.AddFanel,     description = "自機の周囲にファンネルを展開\nレーザーで攻撃する" }, // 赤
        new ItemOption { label = "Laser",  color = new Color(0.45f, 0.75f, 0.95f), action = ItemAction.LaserOrPower, description = "一定間隔で前方に\n強力なレーザーを発射する" },  // 青
        new ItemOption { label = "Syabon", color = new Color(0.70f, 0.45f, 0.90f), action = ItemAction.AddSyabon,    description = "シャボン弾を広範囲に\nばらまいて攻撃する" },    // 紫
    };

    [Header("対象ボタン（空のままなら子から自動取得）")]
    [SerializeField] private Button[] buttons;

    [Header("段階表示")]
    [SerializeField, Tooltip("段階表示付きラベルの書式。{0}=アイテム名, {1}=現在段階, {2}=最大段階")]
    private string levelFormat = "{0}\n<size=55%>Lv {1}/{2}</size>";
    [Header("最大強化済みアイテムの差し替えカード")]
    [SerializeField, Tooltip("強化が終わったアイテムが抽選されたとき、代わりに表示されるカード（効果もこのActionになる）")]
    private ItemOption maxedReplacement = new ItemOption
    {
        label = "Power",
        color = new Color(0.95f, 0.75f, 0.30f), // オレンジ
        action = ItemAction.PowerUp,
        description = "弾の攻撃力を上げる",
    };
    [SerializeField, Tooltip("Laserの表示上の最大段階。実際の上限(UIManagerのlaserMaxLevel)より大きい場合、表記だけこの値になる（将来の3段階化を見込んだ表示用）")]
    private int laserDisplayMax = 3;
    [SerializeField, Tooltip("カード型表示(ItemButtonView)のLv行の書式。{0}=現在段階, {1}=最大段階")]
    private string cardLevelFormat = "Lv {0}/{1}";

    private UIManager ui;
    private GunManagerScript gunManager;
    private SyabonManagerScript syabonManager;
    private newFanelManager fanelManager;

    void Awake()
    {
        ui = FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
        gunManager = FindAnyObjectByType<GunManagerScript>(FindObjectsInactive.Include);
        syabonManager = FindAnyObjectByType<SyabonManagerScript>(FindObjectsInactive.Include);
        fanelManager = FindAnyObjectByType<newFanelManager>(FindObjectsInactive.Include);
        if (buttons == null || buttons.Length == 0)
        {
            buttons = GetComponentsInChildren<Button>(true);
        }
    }

    // パネルがSetActive(true)されるたびに抽選
    void OnEnable()
    {
        if (ui == null || buttons == null || buttons.Length == 0 || itemPool.Length == 0) return;

        // シャッフル（Fisher-Yates）して先頭からボタン数ぶん採用 → 重複なし
        var pool = new List<ItemOption>(itemPool);
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i];
            var option = pool[i % pool.Count]; // 候補がボタン数より少ない場合はループ

            // 強化が終わったアイテムはPowerカードに丸ごと差し替える（色・アイコン・説明・効果すべて）
            if (maxedReplacement != null
                && TryGetStage(option.action, out int curCheck, out int maxCheck)
                && curCheck >= maxCheck)
            {
                option = maxedReplacement;
            }

            // 表示内容の反映
            bool hasStage = TryGetStage(option.action, out int cur, out int max);
            // Laserだけ表示上の最大値を上書きできる（将来の3段階化を見込んだ表示用）
            int dispMax = (option.action == ItemAction.LaserOrPower && laserDisplayMax > max) ? laserDisplayMax : max;

            var view = btn.GetComponentInChildren<ItemButtonView>(true);
            if (view != null)
            {
                // カード型（名前 / Lv / アイコン / 説明文）
                view.Apply(
                    option.label,
                    hasStage ? string.Format(cardLevelFormat, cur, dispMax) : "",
                    option.icon,
                    option.description);
            }
            else
            {
                // 従来のシンプル表示（TMP1個だけのボタン）
                var tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
                if (tmp != null)
                {
                    tmp.text = hasStage ? string.Format(levelFormat, option.label, cur, dispMax) : option.label;
                }
            }

            // 色（アイテムに応じたイメージカラー）
            // ButtonHoverEffectが毎フレーム基準色で上書きするため、
            // ある場合は基準色ごと差し替える（ホバーで光る演出とも両立する）
            var hover = btn.GetComponent<ButtonHoverEffect>();
            if (hover != null)
            {
                hover.SetBaseColor(option.color);
            }
            else if (btn.image != null)
            {
                btn.image.color = option.color;
            }

            // Inspector側のOnClick（固定割り当て）を無効化して二重発動を防ぐ
            for (int p = 0; p < btn.onClick.GetPersistentEventCount(); p++)
            {
                btn.onClick.SetPersistentListenerState(p, UnityEventCallState.Off);
            }

            // ランタイムのリスナーを張り替え
            btn.onClick.RemoveAllListeners();
            var captured = option;
            btn.onClick.AddListener(() => Execute(captured.action));
        }
    }

    /// <summary>
    /// アイテムの現在段階と最大段階を取得する。
    /// HPUp / PowerUp のような無制限アイテムは false を返す（段階表示なし）。
    /// </summary>
    private bool TryGetStage(ItemAction action, out int current, out int max)
    {
        current = 0;
        max = 0;
        switch (action)
        {
            case ItemAction.AddGun:
                if (gunManager == null || ui == null) return false;
                current = gunManager.GunCount;
                max = ui.GunMaxCount;
                return true;
            case ItemAction.AddSyabon:
                if (syabonManager == null || ui == null) return false;
                current = syabonManager.SyabonCount;
                max = ui.SyabonMaxCount;
                return true;
            case ItemAction.AddFanel:
                if (fanelManager == null || ui == null) return false;
                current = fanelManager.FanelCount;
                max = ui.FanelMaxCount;
                return true;
            case ItemAction.LaserOrPower:
                if (ui == null) return false;
                current = ui.LaserLevel;
                max = ui.LaserMaxLevel;
                return true;
            default:
                return false;
        }
    }

    private void Execute(ItemAction action)
    {
        switch (action)
        {
            case ItemAction.HPUp:        ui.HPUp(); break;
            case ItemAction.PowerUp:     ui.PowerUp(); break;
            case ItemAction.LaserOrPower: ui.LaserOrPower(); break;
            case ItemAction.AddFanel:    ui.AddFanel(); break;
            case ItemAction.AddGun:      ui.AddGun(); break;
            case ItemAction.AddSyabon:   ui.AddSyabon(); break;
        }
    }
}
