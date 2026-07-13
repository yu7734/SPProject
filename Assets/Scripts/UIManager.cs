using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private GameObject selectItemImage;
    [SerializeField] private PlayerManager player;
    //[SerializeField] private BulletManagert bullet;
    [SerializeField] private int _experiencePoint;
    [SerializeField, Header("現在のレベル（初期値1）")] private int _level = 1;

    ///<summary>次のレベルまでに必要な経験値数の初期値</summary>
    [SerializeField, Header("レベルアップに必要な経験値初期値")] private int exprrienceMax = 100;
    /// <summary>レベルが上がった回数</summary>
    private int levelUpCount = 0;
    /// <summary>必要な経験値数の増加量</summary>
    [SerializeField, Header("必要な経験値増加量")] private int exprrienceMaxUp = 10;

    /// <summary>計算後の必要経験値</summary>
    private int _maxExprrience = 100;

    // 外部からの読み取り用プロパティ（HUDPanel / EXPBar からアクセスされる）
    public int experiencePoint => _experiencePoint;
    public int level => _level;

    public bool bSelect;

    public int maxExprrience => _maxExprrience;

    // ===== アイテム段階の外部参照用（ItemSelectRandomizerなどが使用） =====
    public int LaserLevel => laserLevel;
    public int LaserMaxLevel => laserMaxLevel;
    public int FanelMaxCount => fanelMaxCount;
    public int GunMaxCount => gunMaxCount;
    public int SyabonMaxCount => syabonMaxCount;

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

    [Header("=== Item: Gun / Syabon ===")]
    [SerializeField, Tooltip("シーンに配置済みのGunManager")]
    private GunManagerScript gunManager;
    [SerializeField, Tooltip("シーンに配置済みのSyabonManager")]
    private SyabonManagerScript syabonManager;
    /// <summary>ガンの最大段階数（GunManagerScript.SerectGunの4段階に対応）</summary>
    private const int gunMaxCount = 4;
    /// <summary>シャボンの最大段階数（SyabonManagerScript.SerectSyabonの4段階に対応）</summary>
    private const int syabonMaxCount = 4;

    [SerializeField] private PlaySound playSound;

    private void Awake()
    {
        _maxExprrience = exprrienceMax;
    }

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
        // 必要経験値数が増える都合上レベルアップ処理を変更
        if (_experiencePoint >= _maxExprrience&&!bSelect)
        {
            bSelect = true;
            ++_level;
            _experiencePoint -= _maxExprrience;
            _maxExprrience += exprrienceMaxUp;
            playSound.PlaySE(0);
            selectItemImage.SetActive(true);
            Time.timeScale = 0;
            // ラベル・色・段階表示はパネル側の ItemSelectRandomizer が OnEnable で更新する
        }
    }

    //攻撃力アップ
    public void PowerUp()
    {
        BulletManagert.bulletPower += 5;
        Time.timeScale = 1;
        playSound.PlaySE(1);
        bSelect = false;
        selectItemImage.SetActive(false);
    }

    //体力アップ
    public void HPUp()
    {
        player.MaxPlayerHP += 10;
        player.playerHP += 10;
        Time.timeScale = 1;
        playSound.PlaySE(1);
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

    // === ガン追加（最大4段階、上限後はPowerUpにフォールバック） ===
    // 選択パネルのボタンOnClickに割り当てる
    public void AddGun()
    {
        if (gunManager == null)
        {
            Debug.LogWarning("UIManager.AddGun: gunManager is not assigned.");
            CloseSelectUI();
            return;
        }

        if (gunManager.GunCount >= gunMaxCount)
        {
            // 最大段階に達したらPowerUpにフォールバック
            PowerUp();
            return;
        }

        gunManager.SerectGun();
        CloseSelectUI();
    }

    // === シャボン追加（最大4段階、上限後はPowerUpにフォールバック） ===
    // 選択パネルのボタンOnClickに割り当てる
    public void AddSyabon()
    {
        if (syabonManager == null)
        {
            Debug.LogWarning("UIManager.AddSyabon: syabonManager is not assigned.");
            CloseSelectUI();
            return;
        }

        if (syabonManager.SyabonCount >= syabonMaxCount)
        {
            // 最大段階に達したらPowerUpにフォールバック
            PowerUp();
            return;
        }

        syabonManager.SerectSyabon();
        CloseSelectUI();
    }

    // 選択UIを閉じてゲーム再開
    private void CloseSelectUI()
    {
        Time.timeScale = 1;
        playSound.PlaySE(1);
        bSelect = false;
        if (selectItemImage != null) selectItemImage.SetActive(false);
    }
}
