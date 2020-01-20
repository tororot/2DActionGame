using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour {

    [Header ("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header ("コンティニュー位置")] public GameObject[] continuePoint;
    [Header ("ゲームオーバー")] public GameObject GameOverObj;
    [Header ("フェード")] public FadeImage fade;

    private Player p;
    private int nextStageNum;
    private bool startFade = false;
    private bool doGameOver = false;

    void Start () {
        if (playerObj != null &&
            continuePoint != null &&
            continuePoint.Length > 0) {
            GameOverObj.SetActive (false);
            playerObj.transform.position = continuePoint[0].transform.position;
            p = playerObj.GetComponent<Player> ();
            if (p == null) {
                Debug.Log ("プレイヤーが設定されていません");
                Destroy (this);
            }
        } else {
            Debug.Log ("ステージコントローラーの設定が足りていません");
            Destroy (this);
        }
    }

    void Update () {
        // ゲームオーバー
        if (GManager.instance.isGameOver && !doGameOver) {
            GameOverObj.SetActive (true);
            doGameOver = true;
        }

        // プレイヤーがダメージを受けた
        if (p.IsDownAnimEnd ()) {
            PlayerSetContinuePoint ();
        }

        // ステージを切り替える
        if (fade != null && startFade) {
            if (fade.compFadeOut) {
                GManager.instance.stageNum = nextStageNum;
                SceneManager.LoadScene ("stage" + nextStageNum);
            }
        }
    }

    // プレイヤーをコンティニューポイントへ移動する
    public void PlayerSetContinuePoint () {
        playerObj.transform.position = continuePoint[GManager.instance.continueNum].transform.position;
        p.ContinuePlayer ();
    }

    // 最初から始める
    public void Retry () {
        GManager.instance.RetryGame ();
        ChangeScene (1); //最初のステージに戻るので１
    }

    // ステージを切り替えます。
    // <param name="num">ステージ番号</param>
    public void ChangeScene (int num) {
        if (fade != null) {
            nextStageNum = num;
            fade.StartFadeOut ();
            startFade = true;
        }
    }
}