using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class BossScript : MonoBehaviour
{
    public Image thanhhp;
    public List<Vector3> listPoints;
    public GameObject sach;
    public float timeRun = 0;
    private float oldX;
    private object idDT;
    private Animator animator;
    private bool isPlayer = false;
    private bool isAttack = false;
    public int hpBoss = 2;
    private int limitHpBoss = 2;
    public int idBoss = 1;
    private bool isStartAttack = false;
    private bool isDie = false;
    void Start()
    {
        limitHpBoss = hpBoss;
        oldX = transform.position.x;
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        this.RegisterListener(EventID.attackBoss, (param) => {
            if(this != null) {
                if((int) param == idBoss && hpBoss > 0) {
                    hpBoss-= GlobalInstance.Instance.gameManagerInstance.atk;
                    thanhhp.fillAmount -= GlobalInstance.Instance.gameManagerInstance.atk * (1.0f/limitHpBoss);
                    if(hpBoss > 0 && !isStartAttack) {
                        ChangeAnim(4);
                        StartCoroutine(DelayBeAttack());
                    }
                    if(hpBoss <= 0) {
                        isDie = true;
                        if(GlobalInstance.Instance.gameManagerInstance.isAttackSound)
                            GlobalInstance.Instance.gameManagerInstance.soundManager.PlayBeAttackBoss();
                        thanhhp.transform.parent.parent.gameObject.SetActive(false);
                        ChangeAnim(5);
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        StartCoroutine(DelayDie());
                    }
                    DOTween.Pause(idDT);
                    StartCoroutine(DelayMove());
                }
            }
        });
        this.RegisterListener(EventID.isEndGame, (param) => {
            if(this != null) {
                this.GetComponent<CircleCollider2D>().enabled = false;
            }
        });
        Run();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > oldX) {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            thanhhp.rectTransform.localScale = new Vector3(- Mathf.Abs(thanhhp.rectTransform.localScale.x), thanhhp.rectTransform.localScale.y, thanhhp.rectTransform.localScale.z);
            oldX = transform.position.x;
        } else if(transform.position.x < oldX) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            thanhhp.rectTransform.localScale = new Vector3(Mathf.Abs(thanhhp.rectTransform.localScale.x), thanhhp.rectTransform.localScale.y, thanhhp.rectTransform.localScale.z);
            oldX = transform.position.x;
        }
    }
    private void Run()
    {
        ChangeAnim(2);
        idDT = transform.DOPath(listPoints.ToArray(), timeRun, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear).OnComplete(() =>
        {
            Run();
        }).target;
    }
    IEnumerator DelayBeAttack() {
        yield return new WaitForSeconds(0.5f);
        if(!isStartAttack)
            ChangeAnim(1);
    }
    void OnTriggerStay2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("player")) {
            if((transform.position.x < obj.transform.position.x && transform.localScale.x > 0) ||
                (transform.position.x > obj.transform.position.x && transform.localScale.x < 0))
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if(obj.gameObject.layer == LayerMask.NameToLayer("player") && !isAttack) {
            isAttack = true;
            ChangeAnim(1);
            DOTween.Pause(idDT);
            StartCoroutine(DelayStartAttack());
            isPlayer = true;
        }
    }
    IEnumerator DelayDie() {
        yield return new WaitForSeconds(1);
        sach.GetComponent<BoxCollider2D>().enabled = true;
        Destroy(this.gameObject);
    }
    IEnumerator DelayStartAttack() {
        yield return new WaitForSeconds(0.9f);
        isAttack = false;
        if(isPlayer && !isDie) {
            isStartAttack = true;
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(DelayEndAttack());
            ChangeAnim(3);
            
            if(GlobalInstance.Instance.gameManagerInstance.isAttackSound)
                GlobalInstance.Instance.gameManagerInstance.soundManager.PlayBossAttack();
            this.PostEvent(EventID.beAttack);
        }
    }

    IEnumerator DelayEndAttack()
    {
        yield return new WaitForSeconds(0.6f);
        isStartAttack = false;
    }

    void OnTriggerExit2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("player")) {
            isAttack = false;
            isPlayer = false;
            StartCoroutine(DelayMove());
        }
    }
    IEnumerator DelayMove() {
        yield return new WaitForSeconds(1);
        ChangeAnim(2);
        DOTween.Play(idDT);
    }
    void ChangeAnim(int typeAnim) {
        //1: idle, 2: run, 3: attack, 4: beAttack, 5: die
        if(animator != null) {
            animator.SetBool("isIdle", false);
            animator.SetBool("isRun", false);
            animator.SetBool("isAttack", false);
            animator.SetBool("isBeAttack", false);
            animator.SetBool("isDie", false);
            if(typeAnim == 1)
                animator.SetBool("isIdle", true);
            else if(typeAnim == 2)
                animator.SetBool("isRun", true);
            else if(typeAnim == 3)
                animator.SetBool("isAttack", true);
            else if(typeAnim == 4)
                animator.SetBool("isBeAttack", true);
            else if(typeAnim == 5)
                animator.SetBool("isDie", true);
        }
    }
}
