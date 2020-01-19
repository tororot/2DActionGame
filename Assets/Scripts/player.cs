using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // インスペクターで設定する
    public float speed; // 速度
    public float gravity; // 重力
    public float jumpSpeed; //ジャンプする速度
    public float jumpHeight; //高さ制限
    public float jumpLimitTime; //ジャンプ制限時間
    public GroundCheck ground; //接地判定
    public GroundCheck head; //頭ぶつけた判定
    public AnimationCurve dashCurve;
    public AnimationCurve jumpCurve;

    // プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false;
    private bool isJump = false;
    private float jumpPos = 0.0f;
    private float dashTime, jumpTime;
    private float beforeKey; //New

    // Start is called before the first frame update
    void Start () {
        anim = GetComponent<Animator> ();
        rb = GetComponent<Rigidbody2D> ();
        speed = 12.0f;
        gravity = 10.0f;
        jumpSpeed = 10.0f;
        jumpHeight = 5.0f;
        jumpLimitTime = 5.0f;
    }

    // Update is called once per frame
    void FixedUpdate () {
        // 接地判定を得る
        isGround = ground.IsGround ();

        // アニメーション設定
        anim.SetBool ("jump", isJump);
        anim.SetBool ("ground", isGround);

        // キー入力されたら行動する
        float xSpeed = 0.0f;
        float ySpeed = -gravity;
        float horizontalKey = Input.GetAxis ("Horizontal");
        float verticalKey = Input.GetAxis ("Vertical");

        // 上下方向
        if (isGround) {
            if (verticalKey > 0 && jumpTime < jumpLimitTime) {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
            } else {
                isJump = false;
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

        // 左右方向
        if (horizontalKey > 0) {
            transform.localScale = new Vector3 (1, 1, 1);
            anim.SetBool ("run", true);
            dashTime += Time.deltaTime;
            xSpeed = speed;
        } else if (horizontalKey < 0) {
            transform.localScale = new Vector3 (-1, 1, 1);
            anim.SetBool ("run", true);
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        } else {
            anim.SetBool ("run", false);
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える New
        if (horizontalKey > 0 && beforeKey < 0) {
            dashTime = 0.0f;
        } else if (horizontalKey < 0 && beforeKey > 0) {
            dashTime = 0.0f;
        }
        beforeKey = horizontalKey;

        //アニメーションカーブを速度に適用 New
        xSpeed *= dashCurve.Evaluate (dashTime);
        if (isJump) {
            ySpeed *= jumpCurve.Evaluate (jumpTime);
        }

        // 速さ更新
        rb.velocity = new Vector2 (xSpeed, ySpeed);
    }
}