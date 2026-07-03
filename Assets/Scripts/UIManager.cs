using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private GameObject selectItemImage;
    [SerializeField] private PlayerManager player;
    //[SerializeField] private BulletManagert bullet;
    [SerializeField] private int _experiencePoint;
    [SerializeField, Header("現在のレベル（初期値1）")] private int _level = 1;

    // 外部からの読み取り用プロパティ（HUDPanel / EXPBar からアクセスされる）
    public int experiencePoint => _experiencePoint;
    public int level => _level;

    public bool bSelect;

    // ===== アイテム選択肢用 =====
    [Header("=== Item: LaserCannon ===")]
    [SerializeField, Tooltip("シーンに配置済みのLaserCannonオブジェクト（初期は非アクティブ推奨）")]
    private GameObject laserCannonObject;
    [SerializeField, Tooltip("LaserCannonコンポーネント参照（laserCannonObject内）")]
    private LaserCannon laserCannon;
    [Tooltip("LaserCannonの最大強化段階（1回押したらPowerUpにフォールバック）")]
    private const int laserMaxLevel = 1;
    [SerializeField, Tooltip("現在のレーザーキャノン段階。0=未取得, 1〜3=取得済み段階")]
    private int laserLevel = 0;

    [Header("=== Item: Fanel ===")]
    [SerializeField, Tooltip("ファンネルのPrefab")]
    private GameObject fanelPrefab;
    [SerializeField, Tooltip("シーンに配置済みのFanelManager")]
    private FanelManager fanelManager;
    [Tooltip("ファンネルの最大数")]
    private const int fanelMaxCount = 4;

    [Header("=== ボタン表示ラベル ===")]
    [SerializeField, Tooltip("Itemボタン(Laser)内のTextMeshPro")]
    private TextMeshProUGUI laserButtonLabel;
    [SerializeField, Tooltip("Fanelボタン内のTextMeshPro")]
    private TextMeshProUGUI fanelButtonLabel;
    [SerializeField, Tooltip("通常状態のItemボタン表示")] private string laserLabelNormal = "Item";
    [SerializeField, Tooltip("最大強化後のItemボタン表示")] private string laserLabelPower = "Power";
    [SerializeField, Tooltip("通常状態のFanelボタン表示")] private string fanelLabelNormal = "Fanel";
    [SerializeField, Tooltip("最大装備後のFanelボタン表示")] private string fanelLabelPower = "Power";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectItemImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HPUI();
        ExperienceUI();
        SelectItem();
    }

    //HPのUI
    private void HPUI()
    {
        // HPText が未設定でも落ちないようにする（HPBar.cs 側で数値表示する場合は未設定でOK）
        if (HPText == null || player == null) return;
        HPText.text = "HP : " + player.playerHP + " / " + player.MaxPlayerHP;
    }

    //経験値UI
    private void ExperienceUI()
    {
        // experienceText が未設定でも落ちないようにする（EXPBar.cs 側で数値表示する場合は未設定でOK）
        if (experienceText == null) return;
        experienceText.text = "EX : " + _experiencePoint + " / 100";
    }

    //経験値加算
    public void Experience(int point)
    {
        _experiencePoint += point;
    }

    //アイテム選択シーン
    private void SelectItem()
    {
        if (_experiencePoint >= 100)
        {
            bSelect = true;
            // 経験値が 100 を超えた回数だけレベルアップ
            _level += _experiencePoint / 100;
            _experiencePoint %= 100;
            selectItemImage.SetActive(true);
            Time.timeScale = 0;

            // ボタン表示ラベルを状態に応じて更新
            UpdateButtonLabels();
        }
    }

    // 各アイテムボタンの表示文字を状態に応じて切り替える
    private void UpdateButtonLabels()
    {
        // Itemボタン(Laser): 最大強化済みなら Power 表記に
        if (laserButtonLabel != null)
        {
            laserButtonLabel.text = (laserLevel >= laserMaxLevel) ? laserLabelPower : laserLabelNormal;
        }
        // Fanelボタン: ファンネル最大装備済みなら Power 表記に
        if (fanelButtonLabel != null)
        {
            int currentFanel = (fanelManager != null) ? fanelManager.Fanelcount : 0;
            fanelButtonLabel.text = (currentFanel >= fanelMaxCount) ? fanelLabelPower : fanelLabelNormal;
        }
    }

    //攻撃力アップ
    public void PowerUp()
    {
        BulletManagert.bulletPower += 5;
        Time.timeScale = 1;
        bSelect = false;
        selectItemImage.SetActive(false);
    }

    //体力アップ
    public void HPUp()
    {
        player.MaxPlayerHP += 10;
        player.playerHP += 10;
        Time.timeScale = 1;
        bSelect = false;
        selectItemImage.SetActive(false);
    }

    // === 3つ目のボタン用：レーザーキャノン優位、最大強化後はPowerUpへフォールバック ===
    // 呼び出しルール:
    //   laserLevel == 0  → LaserCannonを有効化（LaserNum=0 / LaserSphere1）
    //   laserLevel == 1  → LaserNum=1 / LaserSphere2 に進化
    //   laserLevel == 2  → LaserNum=2 / LaserSphere3 に進化（最終強化）
    //   laserLevel >= 3  → PowerUp（攻撃力アップ）にフォールバック
    public void LaserOrPower()
    {
        if (laserLevel < laserMaxLevel)
        {
            UpgradeLaser();
        }
        else
        {
            // 最大強化に達していたらPowerUpに置き換え
            PowerUp();
        }
    }

    // レーザーキャノンの取得 / 強化
    private void UpgradeLaser()
    {
        Debug.Log($"[UpgradeLaser] called. laserLevel(before) = {laserLevel}, laserCannonObject = {(laserCannonObject == null ? "NULL" : laserCannonObject.name)}");

        if (laserCannonObject != null && !laserCannonObject.activeSelf)
        {
            laserCannonObject.SetActive(true);
            Debug.Log("[UpgradeLaser] laserCannonObject SetActive(true)");
        }

        if (laserCannon == null && laserCannonObject != null)
        {
            laserCannon = laserCannonObject.GetComponent<LaserCannon>();
            Debug.Log($"[UpgradeLaser] laserCannon component GetComponent result = {(laserCannon == null ? "NULL" : "OK")}");
        }

        // laserLevel: 0→1 (LaserNum=0), 1→2 (LaserNum=1), 2→3 (LaserNum=2)
        if (laserCannon != null)
        {
            int newIndex = Mathf.Clamp(laserLevel, 0, laserMaxLevel - 1);
            laserCannon.LaserNum = newIndex;
            Debug.Log($"[UpgradeLaser] laserCannon.LaserNum set to {newIndex}. Actual field value now = {laserCannon.LaserNum}, instanceID = {laserCannon.GetInstanceID()}");
        }
        else
        {
            Debug.LogWarning("[UpgradeLaser] laserCannon is NULL — cannot set LaserNum!");
        }

        laserLevel++;
        Debug.Log($"[UpgradeLaser] laserLevel(after) = {laserLevel}");

        CloseSelectUI();
    }

    // === ファンネル1個追加（最大4個、上限後はPowerUpにフォールバック） ===
    public void AddFanel()
    {
        if (fanelManager == null || fanelPrefab == null)
        {
            Debug.LogWarning("UIManager.AddFanel: fanelManager or fanelPrefab is not assigned.");
            CloseSelectUI();
            return;
        }

        if (fanelManager.Fanelcount >= fanelMaxCount)
        {
            // 最大数に達したらPowerUpにフォールバック（Itemボタンと同じ挙動）
            PowerUp();
            return;
        }

        // Fanelのインスタンスを生成（newFanelScript.Start内でFanelManager.Fanelcountをインクリメント）
        Instantiate(fanelPrefab);

        CloseSelectUI();
    }

    // 選択UIを閉じてゲーム再開
    private void CloseSelectUI()
    {
        Time.timeScale = 1;
        bSelect = false;
        if (selectItemImage != null) selectItemImage.SetActive(false);
    }
}
