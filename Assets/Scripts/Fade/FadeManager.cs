using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    enum Mode
    {
        FadeIn,
        FadeOut
    }

    [SerializeField, Tooltip("フェードの時間")] private float fadeTime;
    [SerializeField, Tooltip("フェードの種類")] private Mode mode;

    private Image fadeImage;
    private bool bFade;//フェードするかしないか
    private float fadeCount;
    private UnityEvent onFadeComplete =  new UnityEvent();//変数の中にメソッドを入れられる

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (mode)
        {
            case Mode.FadeIn: fadeCount = fadeTime; break;
            case Mode.FadeOut: 
                fadeCount = 0;
                this.gameObject.SetActive(false); 
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _Fade();
    }

    private void _Fade()
    {
        if (!bFade) return;

        switch (mode)
        {
            case Mode.FadeIn: FadeIn(); break;
            case Mode.FadeOut: FadeOut(); break;
        }

        float alpha = fadeCount / fadeTime;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
    }

    private void FadeIn()
    {
        fadeCount -= Time.deltaTime;//徐々に透明になる

        if (fadeCount <= 0)
        {
            mode = Mode.FadeOut;
            bFade = false;
            this.gameObject.SetActive(false);
            onFadeComplete.Invoke();
        }
    }

    private void FadeOut()
    {
        fadeCount += Time.deltaTime;//徐々に暗くなる

        if (fadeCount >= fadeTime)
        {
            mode = Mode.FadeIn;
            bFade = false;
            onFadeComplete.Invoke();
        }
    }

    public void FadeStart(UnityAction action)
    {
        if (bFade) return;

        bFade = true;
        onFadeComplete.AddListener(action);
    }

    public void ChangeGameScene() { SceneManager.LoadScene("Game"); }//ゲームシーンに遷移
    public void ChangeTutorialScene() { SceneManager.LoadScene("Tutorial"); }//チュートリアルシーンに遷移
    public void ChangeTitleScene() { SceneManager.LoadScene("Game"); }//タイトルシーンに遷移
    public void GameStart() { }

    public bool GetSetBfade {  get { return bFade; } set { bFade = value; } }//bFadeのアクセッサ
}
