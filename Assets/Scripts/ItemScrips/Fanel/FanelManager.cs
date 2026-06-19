using System;
using System.Linq;
using UnityEngine;

public class FanelManager : MonoBehaviour
{
    bool one=true, two=true, thr = true, fou = true;
    [HideInInspector] public int Fanelcount = 0;
    [SerializeField] GameObject[] Fanel;
    [SerializeField] newFanelScript[] fanelScripts;

    void Update()
    {
        if (Fanelcount == 1 && one) { FanelSetup(); one = false; }
        if (Fanelcount == 2 && two) { FanelSetup(); two = false; }
        if (Fanelcount == 3 && thr) { FanelSetup(); thr = false; }
        if (Fanelcount == 4 && fou) { FanelSetup(); fou = false; }
    }

    void FanelSetup()
    {
        Fanel = GameObject.FindGameObjectsWithTag("Fanel");
        Fanel = Fanel.OrderBy(f => f.name).ToArray();
        Array.Resize(ref fanelScripts , Fanel.Length);
        for (int i = 0; i < Fanel.Length; i++) fanelScripts[i] = Fanel[i].GetComponent<newFanelScript>();
        switch (Fanelcount)
        {
            default:
            case 1:
                fanelScripts[0].offset = new Vector3(0f, 0f, -2f);
                break;
            case 2:
                fanelScripts[0].offset = new Vector3(1f, 0f, -2f);
                fanelScripts[1].offset = new Vector3(-1f, 0f, -2f);
                break;
            case 3:
                fanelScripts[0].offset = new Vector3(1f, -0.5f, -2f);
                fanelScripts[1].offset = new Vector3(-1f, -0.5f, -2f);
                fanelScripts[2].offset = new Vector3(0f, 1f, -2f);
                break;
            case 4:
                fanelScripts[0].offset = new Vector3(1f, 0f, -2f);
                fanelScripts[1].offset = new Vector3(0f, 1f, -2f);
                fanelScripts[2].offset = new Vector3(-1f, 0f, -2f);
                fanelScripts[3].offset = new Vector3(0f, -1f, -2f);
                break;
        }
    }
}
