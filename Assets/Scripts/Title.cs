using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    [Header ("フェード")] public FadeImage fade;
    [Header ("ゲームスタート時に鳴らすSE")] public AudioClip startSE;

    private bool firstPush = false;

    void Update () {
        if (fade.compFadeOut) {
            SceneManager.LoadScene ("stage1");
        }
    }

    public void PressStart () {
        if (!firstPush) {
            if (fade != null) {
                GManager.instance.PlaySE (startSE);
                fade.StartFadeOut ();
                firstPush = true;
            }
        }
    }
}