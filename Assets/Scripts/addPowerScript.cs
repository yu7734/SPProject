using System;
using UnityEngine;

public class addPowerScript : MonoBehaviour
{
    int[] PlayerPowerInventory = new int[5];
    int[] Level = new int[5];
    bool[] evolution = new bool[5];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < PlayerPowerInventory.Length; i++)
        {
            PlayerPowerInventory[i] = 0;
        }
        for(int i=0;i < Level.Length; i++)
        {
            Level[i] = 1;
        }
        for (int i = 0; i < evolution.Length; i++)
        {
            evolution[i] = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;i < PlayerPowerInventory.Length; i++)
        {
            if (PlayerPowerInventory[i] == 0) return ;
            if (PlayerPowerInventory[i] == 1) return ;
        }
    }
}

/*  能力アイデア
    ファンネルを召喚、ファンネルは1番近い対象を攻撃する
    ファンネルの射撃する弾の種類を変える
    Array.Resize(ref name,name.Length+1);
        name[name.Length - 1] = 1;
前提能力があるタイプの強化は配列を追加して
 */
