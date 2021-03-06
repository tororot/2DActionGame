﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    private string groundTag = "Ground";
    public bool isGround = false;
    public bool isGroundEnter, isGroundStay, isGroundExit;

    //接地判定を返すメソッド
    public bool IsGround () {
        if (isGroundEnter || isGroundStay) {
            isGround = true;
        }
        if (isGroundExit) {
            isGround = false;
        }

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag == groundTag) {
            isGroundEnter = true;
        }
    }

    private void OnTriggerStay2D (Collider2D collision) {
        if (collision.tag == groundTag) {
            isGroundStay = true;
        }
    }

    private void OnTriggerExit2D (Collider2D collision) {
        if (collision.tag == groundTag) {
            isGroundExit = true;
        }
    }
}