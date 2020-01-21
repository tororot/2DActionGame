using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // インスペクターで設定する
    [Header ("移動速度")] public float speed;
    [Header ("重力")] public float gravity;
    [Header ("ジャンプ速度")] public float jumpSpeed;
    [Header ("ジャンプする高さ")] public float jumpHeight;
    [Header ("ジャンプする長さ")] public float jumpLimitTime;
    [Header ("接地判定")] public GroundCheck ground;
    [Header ("天井判定")] public GroundCheck head;
    [Header ("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header ("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    [Header ("踏みつけ判定の高さの割合(%)")] public float stepOnRate;
    [Header ("ジャンプする時に鳴らすSE")] public AudioClip jumpSE;
    [Header ("やられた鳴らすSE")] public AudioClip downSE;
    [Header ("コンティニュー時に鳴らすSE")] public AudioClip continueSE;

    // プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    private bool isGround = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isDown = false;
    private bool isOtherJump = false;
    private float jumpPos = 0.0f;
    private float dashTime, jumpTime;
    private float beforeKey;
    private float otherJumpHeight = 0.0f;
    private string enemyTag = "Enemy";
    private bool isContinue = false;
    private float continueTime, blinkTime;
    private SpriteRenderer sr = null;

    // Start is called before the first frame update
    void Start () {
        anim = GetComponent<Animator> ();
        rb = GetComponent<Rigidbody2D> ();
        capcol = GetComponent<CapsuleCollider2D> ();
        sr = GetComponent<SpriteRenderer> ();
    }

    private void Update () {
        if (isContinue) {
            //明滅 ついている時に戻る
            if (blinkTime > 0.2f) {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅 消えているとき
            else if (blinkTime > 0.1f) {
                sr.enabled = false;
            }
            //明滅 ついているとき
            else {
                sr.enabled = true;
            }

            //1秒たったら明滅終わり
            if (continueTime > 1.0f) {
                isContinue = false;
                blinkTime = 0f;
                continueTime = 0f;
                sr.enabled = true;
            } else {
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (!isDown && !GManager.instance.isGameOver) {
            // 接地判定を得る
            isGround = ground.IsGround ();

            //各種座標軸の速度を求める
            float xSpeed = GetXSpeed ();
            float ySpeed = GetYSpeed ();

            // アニメーションを適用
            SetAnimation ();

            // 移動速度を設定
            rb.velocity = new Vector2 (xSpeed, ySpeed);
        } else {
            rb.velocity = new Vector2 (0, -gravity);
        }
    }

    // X成分で必要な計算をし、速度を返す。
    private float GetXSpeed () {
        // キー入力されたら行動する
        float xSpeed = 0.0f;
        float horizontalKey = Input.GetAxis ("Horizontal");

        // 左右方向
        if (horizontalKey > 0) {
            transform.localScale = new Vector3 (1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = speed;
        } else if (horizontalKey < 0) {
            transform.localScale = new Vector3 (-1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        } else {
            isRun = false;
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える
        if (horizontalKey > 0 && beforeKey < 0) {
            dashTime = 0.0f;
        } else if (horizontalKey < 0 && beforeKey > 0) {
            dashTime = 0.0f;
        }
        beforeKey = horizontalKey;

        //アニメーションカーブを速度に適用
        xSpeed *= dashCurve.Evaluate (dashTime);

        return xSpeed;
    }

    // Y成分で必要な計算をし、速度を返す。
    private float GetYSpeed () {

        float verticalKey = Input.GetAxis ("Vertical");
        float ySpeed = -gravity;

        if (isGround) {
            if (verticalKey > 0 && jumpTime < jumpLimitTime) {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
                GManager.instance.PlaySE (jumpSE);
            } else {
                isJump = false;
            }
        }
        //何かを踏んだ際のジャンプ
        else if (isOtherJump) {
            //現在の高さがジャンプした位置から自分の決めた位置より下ならジャンプを継続する
            if (jumpPos + otherJumpHeight > transform.position.y &&
                jumpTime < jumpLimitTime && !head.IsGround ()) {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
                GManager.instance.PlaySE (jumpSE);
            } else {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        } else if (isJump) {
            // 上ボタンを押されている。かつ、現在の高さがジャンプした位置から
            // 自分の決めた位置より下ならジャンプを継続する
            if (verticalKey > 0 &&
                jumpPos + jumpHeight > transform.position.y &&
                jumpTime < jumpLimitTime &&
                !head.IsGround ()
            ) {

                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            } else {
                isJump = false;
                jumpTime = 0.0f;
            }
        }

        if (isJump || isOtherJump) {
            ySpeed *= jumpCurve.Evaluate (jumpTime);
        }

        return ySpeed;

    }

    // アニメーションを設定する
    private void SetAnimation () {
        anim.SetBool ("jump", isJump || isOtherJump);
        anim.SetBool ("ground", isGround);
        anim.SetBool ("run", isRun);
    }

    // 接触判定
    private void OnCollisionEnter2D (Collision2D collision) {
        if (collision.collider.tag == enemyTag) {
            //踏みつけ判定になる高さ
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
            //踏みつけ判定のワールド座標
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D p in collision.contacts) {
                if (p.point.y < judgePos) {
                    ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision> ();
                    if (o != null) {
                        //踏んづけたものから跳ねる高さを取得する
                        otherJumpHeight = o.boundHeight;
                        //踏んづけたものに対して踏んづけた事を通知する
                        o.playerStepOn = true;
                        isOtherJump = true;
                        isJump = false;
                        jumpTime = 0.0f;
                    } else {
                        Debug.Log ("ObjectCollisionが付いてないよ!");
                    }
                } else {
                    anim.Play ("player_lose");
                    isDown = true;
                    GManager.instance.SubHeartNum ();
                    GManager.instance.PlaySE (downSE);
                    break;
                }
            }
        }
    }

    // ダウンアニメーションが終わっているかどうか
    public bool IsDownAnimEnd () {
        if (isDown && anim != null) {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo (0);
            if (currentState.IsName ("player_lose")) {
                if (currentState.normalizedTime >= 1) {
                    return true;
                }
            }
        }
        return false;
    }

    // コンティニューする
    public void ContinuePlayer () {
        isDown = false;
        anim.Play ("player_stand");
        isJump = false;
        isOtherJump = false;
        isRun = false;
        isContinue = true;
        GManager.instance.PlaySE (continueSE);
    }

}