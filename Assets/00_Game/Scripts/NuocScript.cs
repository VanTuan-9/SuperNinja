using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NuocScript : MonoBehaviour
{
    public int limitTime = 60;
    private object idDT;
    private bool isRun = true;
    void Start()
    {
        DOTween.Init();
        idDT = transform.DOMoveY(25, limitTime).SetEase(Ease.Linear).target;
        this.RegisterListener(EventID.isEndGame, (param) => {
            if(this != null)
                DOTween.Pause(idDT);
        });
        this.RegisterListener(EventID.isVeDich, (param) => {
            if(this != null)
                DOTween.Pause(idDT);
        });
        this.RegisterListener(EventID.isPause, (param) => {
            if(this != null) {
                if((int) param == 1)
                    DOTween.Pause(idDT);
                else
                    DOTween.Play(idDT);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
