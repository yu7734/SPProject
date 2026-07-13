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
    }

    [Header("抽選候補（Inspectorで追加・変更可）")]
    [SerializeField]
    private ItemOption[] itemPool =
    {
        new ItemOption { label = "Gun",    color = new Color(0.45f, 0.85f, 0.45f), action = ItemAction.AddGun },       // 緑
        new ItemOption { label = "Fanel",  color = new Color(0.90f, 0.40f, 0.40f), action = ItemAction.AddFanel },     // 赤
        new ItemOption { label = "Laser",  color = new Color(0.45f, 0.75f, 0.95f), action = ItemAction.LaserOrPower }, // 青
        new ItemOption { label = "Syabon", color = new Color(0.70f, 0.45f, 0.90f), action = ItemAction.AddSyabon },    // 紫
    };

    [Header("対象ボタン（空のままなら子から自動取得）")]
    [SerializeField] private Button[] buttons;

    [Header("段階表示")]
    [SerializeField, Tooltip("段階表示付きラベルの書式。{0}=アイテム名, {1}=現在段階, {2}=最大段階")]
    private string levelFormat = "{0}\n<size=55%>Lv {1}/{2}</size>";
    [SerializeField, Tooltip("最大段階に達したアイテムの表示（選んだときの効果はPowerUpになる）")]
    private string maxedLabel = "Power";
    [SerializeField, Tooltip("Laserの表示上の最大段階。実際の上限(UIManagerのlaserMaxLevel)より大きい場合、表記だけこの値になる（将来の3段階化を見込んだ表示用）")]
    private int laserDisplayMax = 3;

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

            // ラベル（段階のあるアイテムは「名前 + Lv 現在/最大」、最大到達済みはmaxedLabel）
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null)
            {
                string text = option.label;
                if (TryGetStage(option.action, out int cur, out int max))
                {
                    // Laserだけ表示上の最大値を上書きできる（実際の上限に達したらPower表記になるのは変わらない）
                    int dispMax = (option.action == ItemAction.LaserOrPower && laserDisplayMax > max) ? laserDisplayMax : max;
                    text = (cur >= max) ? maxedLabel : string.Format(levelFormat, option.label, cur, dispMax);
                }
                tmp.text = text;
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
