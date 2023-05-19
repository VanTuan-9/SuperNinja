using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemScript : MonoBehaviour
{
    public int typeVatPham = 1; //1: vang, 2: kimCuong, 3: cauHoi, 4: hp

    void Start()
    {
        DOTween.Init();
    }

    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("player")) {
            GetComponent<BoxCollider2D>().enabled = false;
            if(typeVatPham == 1) {
                this.PostEvent(EventID.isAnVang);
                GlobalInstance.Instance.gameManagerInstance.soundManager.PlayAnVP();
            } else if(typeVatPham == 2) {
                this.PostEvent(EventID.isAnKimCuong);
                GlobalInstance.Instance.gameManagerInstance.soundManager.PlayAnVP();
            } else if(typeVatPham == 3) {
                this.PostEvent(EventID.isAnCauHoi);
                if(GlobalInstance.Instance.gameManagerInstance.isAttackSound)
                    GlobalInstance.Instance.gameManagerInstance.soundManager.PlayAnCauHoi();
            }
            else if(typeVatPham == 4) {
                this.PostEvent(EventID.isAnHP);
                GlobalInstance.Instance.gameManagerInstance.soundManager.PlayAnVP();
            }
            transform.GetChild(0).GetComponent<Animator>().enabled = false;
            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear).OnComplete(() => {
                Destroy(this.gameObject);
            });
        }
    }
}
