using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tougouyou : MonoBehaviour
{
    public AudioSetting_Script audioSettingScript;
    // Start is called before the first frame update
    void Start()
    {
        audioSettingScript.Play_BGM(1); // 最初にBGMを再生
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
