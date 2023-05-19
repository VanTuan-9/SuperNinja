using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bullet;
    public GameObject dich;
    public int speed = 1;
    public int doCao = 0;
    private Rigidbody2D rigid;
    private Animator anim;
    private int diChuyen = 0;
    private float oldScale = 0;
    private bool isRun = false;
    private bool isJump = false;

    private bool isStart = true;

    private bool isPause = false;

    private float curTime = 0;

    private bool isButtonMoveLeft = false, isButtonMoveRight = false, isButtonJump = false, isButtonAttack = false;
    private bool isAttack = false, isBoss = false;
    private int idBoss = -1;

    private bool isBeAttack = false;
    private GameObject newBullet;
    private bool isMove = false, isActiveAnim = false;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        transform.GetChild(GlobalInstance.Instance.gameManagerInstance.playerChoose).gameObject.SetActive(true);
        anim = transform.GetChild(GlobalInstance.Instance.gameManagerInstance.playerChoose).GetComponent<Animator>();
        this.RegisterListener(EventID.isEndGame, (param) => {
            isStart = false;
            if(this != null) {
                StartCoroutine(DelayDie());
            }
        });
        this.RegisterListener(EventID.isPause, (param) => {
            if((int) param == 0)
                isPause = false;
            else    
                isPause = true;
            ChangeAnim(1);
        });
        this.RegisterListener(EventID.isStartGame, (param) => {
            isStart = true;
        });
        this.RegisterListener(EventID.isDich, (param) => {
            if(dich.gameObject != null) {
                dich.GetComponent<BoxCollider2D>().enabled = true;
                dich.transform.GetChild(0).gameObject.SetActive(true);
            }
        });
        this.RegisterListener(EventID.isMove, (param) => {
            if((int) param == 1) {
                isButtonMoveRight = false;
                isButtonMoveLeft = true;
            } else if((int) param == 2) {
                isButtonMoveLeft = false;
                isButtonMoveRight = true;
            } else if((int) param == 3) {
                isButtonJump = true;
            } else {
                isButtonAttack = true;
            }
        });
        this.RegisterListener(EventID.isNotMove, (param) => {
            isButtonMoveLeft = false;
            isButtonMoveRight = false;
        });
        this.RegisterListener(EventID.beAttack, (param) => {
            if(this != null) {
                StartCoroutine(DelayBeAttack());
            }
        });
        oldScale = transform.localScale.x;
    }
    void OnDisable() {
        transform.GetChild(GlobalInstance.Instance.gameManagerInstance.playerChoose).gameObject.SetActive(false);
    }
    IEnumerator DelayDie()
    {
        yield return new WaitForSeconds(0.3f);
        ChangeAnim(5);
    }

    IEnumerator DelayBeAttack()
    {
        yield return new WaitForSeconds(0.3f);
        if(GlobalInstance.Instance.gameManagerInstance.isAttackSound)
            GlobalInstance.Instance.gameManagerInstance.soundManager.PlayBeAttackPlayer();
        GetComponent<Animator>().SetBool("isBeAttack", true);
        isBeAttack = true;
        yield return new WaitForSeconds(1);
        isBeAttack = false;
        GetComponent<Animator>().SetBool("isBeAttack", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isMove && !isActiveAnim) {
            isActiveAnim = true;
            GetComponent<Animator>().enabled = true;
        }
        if(isStart && !isPause) {
            float a = UltimateJoystick.GetHorizontalAxis( "JoyStick" );
            float b = UltimateJoystick.GetVerticalAxis( "JoyStick" );
            if((isButtonMoveLeft || Input.GetKey(KeyCode.LeftArrow) || a < 0) && !isAttack) {
                isMove = true;
                if(!isRun) {
                    isRun = true;
                    ChangeAnim(2);
                }
                diChuyen = -1;
                transform.localScale = new Vector3(-oldScale, transform.localScale.y, transform.localScale.z);
            } 
            else if((isButtonMoveRight || Input.GetKey(KeyCode.RightArrow) || a > 0) && !isAttack) {
                isMove = true;
                if(!isRun) {
                    isRun = true;
                    ChangeAnim(2);
                }
                diChuyen = 1;
                transform.localScale = new Vector3(oldScale, transform.localScale.y, transform.localScale.z);
            } 
            else if(!isAttack && !isJump) {
                isRun = false;
                ChangeAnim(1);
                diChuyen = 0;
            }
            transform.Translate(Vector2.right * diChuyen * speed * Time.deltaTime);
            if((isButtonJump || Input.GetKeyDown(KeyCode.UpArrow)) && !isJump) {
                isMove = true;
                isJump = true;
                ChangeAnim(3);
                StartCoroutine(DelayJump());
                rigid.AddForce(Vector2.up * doCao, ForceMode2D.Impulse);
            } 
            if((Input.GetKeyDown(KeyCode.Space) || isButtonAttack) && !isAttack) {
                isMove = true;
                isAttack = true;
                diChuyen = 0;
                ChangeAnim(4);
                if(GlobalInstance.Instance.gameManagerInstance.isAttackSound) {
                    if(GlobalInstance.Instance.gameManagerInstance.playerChoose < 2)
                        GlobalInstance.Instance.gameManagerInstance.soundManager.PlayAttackKiem();
                    else
                        GlobalInstance.Instance.gameManagerInstance.soundManager.PlayAttackSung();
                }
                StartCoroutine(DelayAttack());
                newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
                if(transform.localScale.x < 0) {
                    newBullet.transform.localScale = new Vector3(-newBullet.transform.localScale.x, newBullet.transform.localScale.y, newBullet.transform.localScale.z);
                    newBullet.transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y);
                } else {
                    newBullet.transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y);
                }
            }

        } if(isPause) {
            ChangeAnim(1);
        }
    }
    IEnumerator DelayAttack() {
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
        isButtonAttack = false;
        isRun = false;
        ChangeAnim(1);
    }
    IEnumerator DelayJump() {
        yield return new WaitForSeconds(0.7f);
        isJump = false;
        isRun = false;
        isButtonJump = false;
    }
    private void ChangeAnim(int typeAnim) {
        // 1: idle, 2: run, 3: jump, 4: attack, 5: die
        if(anim != null) {
            anim.SetBool("isIdle", false);
            anim.SetBool("isRun", false);
            anim.SetBool("isJump", false);
            anim.SetBool("isAttack", false);
            anim.SetBool("isDie", false);
            if(typeAnim == 1)
                anim.SetBool("isIdle", true);
            if(typeAnim == 2)
                anim.SetBool("isRun", true);
            if(typeAnim == 3)
                anim.SetBool("isJump", true);
            if(typeAnim == 4)
                anim.SetBool("isAttack", true);
            if(typeAnim == 5)
                anim.SetBool("isDie", true);
        }
    }

    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("dich")) {
            obj.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            isPause = true;
            this.PostEvent(EventID.isVeDich);
        }
        if(obj.gameObject.layer == LayerMask.NameToLayer("nuoc")) {
            this.PostEvent(EventID.beAttack, 1);
        }
        if(obj.gameObject.layer == LayerMask.NameToLayer("bacDie"))
            this.PostEvent(EventID.beAttack);
    }
    
    void OnTriggerStay2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("boss")) {
            isBoss = true;
            idBoss = obj.gameObject.GetComponent<BossScript>().idBoss;
        }
    }
    void OnTriggerExit2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("boss"))
            idBoss = -1;
    }
}
