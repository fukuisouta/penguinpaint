using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーの自動前進、走りアニメーション、および各種トリガー（アイテム・敵・ゴール・落下）を制御するクラス
/// </summary>
public class Player : MonoBehaviour
{
    // --- オーディオ関連 ---
    private AudioSource audioSource;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioClip deathSE;
    [SerializeField] private AudioClip fallSE;
    [SerializeField] private AudioClip clearSE;

    // --- 移動・物理・グラフィック ---
    private Rigidbody2D rigid2D;
    [SerializeField] public float speed = 0.1f;       // 自動前進の移動速度
    [SerializeField] public float anima = 0.08f;       // アニメーションの切り替え速度
    public Sprite[] slide;                            // 走る（スライド）アニメーション用のスプライト配列

    private float time = 0;                           // アニメーションの時間計測用カウンター
    private int idx = 0;                              // 現在表示しているアニメーションの配列インデックス
    private SpriteRenderer spriteRenderer;

    // --- 線（Line）との接触管理（必要であれば残す、不要ならインク消費側だけで管理してもOK） ---
    [SerializeField] private int lineContactCount = 0;
    [SerializeField] string lineTag = "line";

    // --- シーン遷移と演出 ---
    [SerializeField, Header("フェード演出の参照")]
    private Fade m_fade = null;
    private bool isChangingScene = false;

    [SerializeField] private DamageEffect m_damageEffect;

    [SerializeField, Header("スコアマネージャーの参照")]
    private ScoreManager scoreManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        lineContactCount = 0;
    }

    void Update()
    {
        // シーン遷移中、またはゲームのカウントダウン中は動かさない
        if (isChangingScene) return;
        if (!gameManager.m_play) return;

        // 1. 自動前進（毎フレーム右へ移動）
        transform.Translate(this.speed, 0, 0);

        // 2. 落下判定
        if (transform.position.y < -10)
        {
            gameManager.StopBGM();
            if (audioSource != null && fallSE != null)
            {
                audioSource.PlayOneShot(fallSE);
            }
            DetermineGameOverScene();
        }

        // 3. アニメーション制御（余計な条件を無くし、常にパラパラ漫画風に走らせる）
        this.time += Time.deltaTime;
        if (this.time > this.anima)
        {
            this.time = 0;
            this.idx++;
            if (this.idx >= this.slide.Length)
            {
                this.idx = 0;
            }
            this.spriteRenderer.sprite = this.slide[this.idx];
        }
    }

  
    void OnCollisionEnter2D(Collision2D collision)
    {
        // マジックペンの線の上を走っているカウント（他スクリプトへの影響用）だけ残しています
        if (collision.collider.CompareTag(lineTag))
        {
            lineContactCount++;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(lineTag))
        {
            lineContactCount = Mathf.Max(0, lineContactCount - 1);
        }
    }

    // --- トリガー（重なり）判定 ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isChangingScene) return;

        // インクアイテムの回収
        if (other.CompareTag("InkItem"))
        {
            InkRefillItem item = other.GetComponent<InkRefillItem>();
            InkPenScript ink = GetComponentInChildren<InkPenScript>();

            if (item != null && ink != null)
            {
                ink.RefillInk(item.refillAmount);
                Destroy(other.gameObject);
                return;
            }
        }

        // ゴール判定
        if (other.CompareTag("goal"))
        {
            gameManager.StopBGM();
            if (audioSource != null && clearSE != null)
            {
                audioSource.PlayOneShot(clearSE);
            }
            ChangeScene("3,1GameClear");
        }
        // 敵との衝突判定
        else if (other.CompareTag("enemy"))
        {
            if (m_damageEffect != null) m_damageEffect.Flash();

            if (audioSource != null && deathSE != null)
            {
                gameManager.StopBGM();
                audioSource.PlayOneShot(deathSE);
            }

            Invoke("DetermineGameOverScene", 0.2f);
        }
        // トゲとの衝突判定
        else if (other.CompareTag("Spike"))
        {
            gameManager.StopBGM();
            if (audioSource != null && deathSE != null)
            {
                audioSource.PlayOneShot(deathSE);
            }
            DetermineGameOverScene();
        }
    }

    /// <summary>
    /// ゲームオーバー時の遷移先を判定し、スコアを保存してシーンを切り替える
    /// </summary>
    private void DetermineGameOverScene()
    {
        if (isChangingScene) return;

        // シーンが切り替わる前にスコアをセーブ
        if (scoreManager != null)
        {
            scoreManager.GameOver();
        }

        ButtonManager.SaveCurrentScene();

        // 現在のシーン名を取得して分岐
        string currentSceneName = SceneManager.GetActiveScene().name;

        // もし現在のシーン名に「Endless」が含まれていたらリザルトへ
        if (currentSceneName.Contains("2,4ENDLESS"))
        {
            ChangeScene("3,3ResultScene");
        }
        else
        {
            ChangeScene("3,2GameOver");
        }
    }

    private void ChangeScene(string sceneName)
    {
        if (isChangingScene) return;
        isChangingScene = true;

        if (m_fade != null)
        {
            m_fade.FadeOut(1.5f, () =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}