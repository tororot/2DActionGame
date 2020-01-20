using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    public FadeImage fade;

    private bool firstPush = false;

    void Update () {
        if (fade.compFadeOut) {
            SceneManager.LoadScene ("stage1");
        }
    }

    public void PressStart () {
        if (!firstPush) {
            if (fade != null) {
                fade.StartFadeOut ();
                firstPush = true;
            }
        }
    }
}