using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonController : MonoBehaviour
{

    public static ButtonController instant;
    public event Action onClickBtn;
    // Start is called before the first frame update
    private void Awake()
    {
        instant = this;
        var btn = this.GetComponent<Button>();
        btn.onClick.AddListener(UpdateGPs);
    }

    private void UpdateGPs()
    {
        if (onClickBtn != null)
            onClickBtn();
    }
}
