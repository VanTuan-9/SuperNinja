using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public GameObject canvasBG;
    public SoundManager soundManager;
    //prefabs
    public GameObject level1, level2, level3;
    public GameObject khoaLV2, khoaLV3;

    public GameObject[] listPlayers;
    public bool[] unLookPlayers;
    public GameObject[] iconPlayers;
    public int playerChoose = 0;

    public GameObject[] listPlayerInShops;
    public Sprite newBGShop;
    //canvas
    public GameObject canvasLoading, 
    canvasTrangChu, 
    canvasChooseLevel, 
    canvasChoosePlayer, 
    canvasGamePlay, 
    canvasPause, 
    canvasLose, 
    canvasWin, 
    canvasThongBao, 
    canvasDieuKhien, 
    canvasShop;

    // so vang, kimCuong trong canvasTrangChu
    public Text countVangTrangChu, countKimCuongTrangChu;

    // so vang, kimCuong trong shop
    public Text countVangShop, countKimCuongShop;

    //so cau hoi canvasLose
    public Text countCauHoiLose, countVangLose, countKimCuongLose;

    //so cau hoi canvasWin
    public Text countCauHoiWin, countVangWin, countKimCuongWin;

    public GameObject canvasCauHoiChonAnh, canvasCauHoiLogic, canvasCauHoiTimSo, canvasChooseTypeQuestion;
    public Text countVangCanvasGamePlay, countKimCuongCanvasGamePlay, countCauHoiGamePlay;
    public GameObject tymGroup;
    private int countTym = 3, limitTym = 3;
    public int atk = 1;
    private int indexPlayerChooseInLevel = 0;
    private int level = 3, levelPlaying = -1;

    public List<int> countLuotLV1s, countLuotLV2s, countLuotLV3s;
    private List<int> countLuotLVInlevels;

    public List<Button> buttonChooseTypeQuestions;
    public Sprite[] buttonChooseQuests;
    //Cau hoi logic
    public string[] listCauHoiLogics;
    public string[] listDapAnLogicA;
    public string[] listDapAnLogicB;
    public string[] listDapAnLogicC;
    public string[] listDapAnLogicD;
    public int[] listDapAnDungLogic;
    public Text cauHoiLogic, dapAnLogicA, dapAnLogicB, dapAnLogicC, dapAnLogicD;
    private List<string> listCauHoiChonInlevels = new List<string>();

    //cau hoi chon anh
    public GameObject[] cauHoiChonAnhs;
    public GameObject[] dapAnChonAnhs;
    public int[] limitCountDAChonAnhs;  // số lượng vị trí sai trong câu hỏi

    // cau hoi tim so
    public Sprite[] listCauHoiTimSos;
    public string[] listDapAnTimSos;
    public Image imageCauHoi;
    private bool[] listIndexCauHoiDaChonChonAnh, listIndexCauHoiDaChonLogic, listIndexCauHoiDaChonTimSo;

    private int indexCauHoi = 0;
    private bool isClickDapAn = false;

    private int typeQuestion = -1;
    private GameObject curLevel1, curLevel2, curLevel3;

    //Score gamePlay
    private int countVangInLevel = 0;
    private int countKimCuongInLevel = 0;
    private int countCauHoiInLevel = 0;

    private bool isDich = false;

    // index thong bao: 0. đủ câu hỏi để hiển thị đích

    // setting
    private bool isAmThanh = true;

    // update level
    private bool isLevel2 = false, isLevel3 = false;

    // time cau hoi
    private float curTimeCauHoi = 0;
    private int limitTime = 20, limitTime2 = 20;
    private int countTimDA = 0;
    public Text timeDongHoChonAnh, timeDongHoLogic, timeDongHoTimSo;

    public GameObject cupWin;

    public GameObject[] cupLV;
    private bool anCauHoiFirstChonAnh = false, anCauHoiFirstLogic = false, anCauHoiFirstTimSo = false; 

    public bool isAttackSound = true;
    private bool isChangeSound = false;
    private bool isPlayBG = false;
    void Start()
    {
        countLuotLVInlevels = new List<int>();
        for(int i = 0 ; i < countLuotLV1s.Count ; i++){
            countLuotLVInlevels.Add(countLuotLV1s[i]);
        }
        listIndexCauHoiDaChonChonAnh = new bool[cauHoiChonAnhs.Length];
        listIndexCauHoiDaChonLogic = new bool[listCauHoiLogics.Length];
        listIndexCauHoiDaChonTimSo = new bool[listCauHoiTimSos.Length];
        for (int i = 0; i < listIndexCauHoiDaChonChonAnh.Length; i++)
            listIndexCauHoiDaChonChonAnh[i] = false;
        for(int i = 0 ; i < listIndexCauHoiDaChonLogic.Length ; i++)
            listIndexCauHoiDaChonLogic[i] = false;
        for(int i = 0 ; i < listIndexCauHoiDaChonTimSo.Length ; i++)
            listIndexCauHoiDaChonTimSo[i] = false;
        UpdateIconChoosePlayer();
        soundManager.PlayBG();
        StartCoroutine(DelayLoading());
        this.RegisterListener(EventID.beAttack, (param) => {
            if(countTym > 0) {
                countTym--;
                StartCoroutine(DelayRemoveTym());
                if(countTym == 0 || param != null) {
                    this.PostEvent(EventID.isEndGame);
                    StartCoroutine(DelayLose());
                    countCauHoiLose.text = countCauHoiInLevel.ToString();
                    countVangLose.text = countVangInLevel.ToString();
                    countKimCuongLose.text = countKimCuongInLevel.ToString();
                }
            }
        });
        this.RegisterListener(EventID.isAnVang, (param) => {
            countVangInLevel++;
            if(isAttackSound)
                soundManager.PlayAnVP();
            countVangCanvasGamePlay.text = countVangInLevel.ToString();
        });
        
        this.RegisterListener(EventID.isAnKimCuong, (param) => {
            countKimCuongInLevel++;
            if(isAttackSound)
                soundManager.PlayAnVP();
            countKimCuongCanvasGamePlay.text = countKimCuongInLevel.ToString();
        });
        this.RegisterListener(EventID.isAnCauHoi, (param) => {
            isClickDapAn = false;
            if(isAttackSound)
                soundManager.PlayAnCauHoi();
            soundManager.StopBG();
            canvasChooseTypeQuestion.SetActive(true);
            this.PostEvent(EventID.isPause, 1);
        });
        this.RegisterListener(EventID.isAnHP, (param) => {
            if(countTym < limitTym) {
                tymGroup.transform.GetChild(0).GetComponent<Image>().fillAmount += 1.0f/limitTym;
                countTym++;
            }
        });
        this.RegisterListener(EventID.isVeDich, (param) => {
            canvasWin.SetActive(true);
            if(levelPlaying == 1) {
                level = 2;
                cupLV[levelPlaying].SetActive(true);
            }
            if(levelPlaying == 2){
                level = 3;
                cupLV[levelPlaying].SetActive(true);
            }
            
            if(countCauHoiInLevel > 0) {
                cupLV[levelPlaying - 1].transform.GetChild(1).gameObject.SetActive(true);
                cupWin.transform.GetChild(1).gameObject.SetActive(true);
            }
            if(countCauHoiInLevel > 2) {
                cupLV[levelPlaying - 1].transform.GetChild(2).gameObject.SetActive(true);
                cupWin.transform.GetChild(2).gameObject.SetActive(true);
            }
            if(countCauHoiInLevel > 5) {
                cupLV[levelPlaying - 1].transform.GetChild(3).gameObject.SetActive(true);
                cupWin.transform.GetChild(3).gameObject.SetActive(true);
            }
            if(isAttackSound)
                soundManager.PlayWin();
            canvasDieuKhien.SetActive(false);
            countCauHoiWin.text = countCauHoiInLevel.ToString();
            countVangWin.text = countVangInLevel.ToString();
            countKimCuongWin.text = countKimCuongInLevel.ToString();
        });
    }

    IEnumerator DelayLose()
    {
        yield return new WaitForSeconds(1);
        if(isAttackSound)
            soundManager.PlayLose();
        canvasDieuKhien.SetActive(false);
        canvasLose.SetActive(true);
    }

    public void OnChooseTypeQuestion(int typeQuestion) {
        if(isAttackSound)
            soundManager.PlayRun();
        canvasChooseTypeQuestion.SetActive(false);
        this.typeQuestion = typeQuestion;
        if(typeQuestion == 2) {
            canvasCauHoiLogic.SetActive(true);
            if(!anCauHoiFirstLogic) {
                anCauHoiFirstLogic = true;
                indexCauHoi = Random.Range(0, listCauHoiLogics.Length);
                listIndexCauHoiDaChonLogic[indexCauHoi] = true;
            } else {
                do {
                    indexCauHoi = Random.Range(0, listCauHoiLogics.Length);
                    if(!listIndexCauHoiDaChonLogic[indexCauHoi]) {
                        listIndexCauHoiDaChonLogic[indexCauHoi] = true;
                        break;
                    }
                } while(true);
            }
            cauHoiLogic.text = listCauHoiLogics[indexCauHoi];
            dapAnLogicA.text = listDapAnLogicA[indexCauHoi];
            dapAnLogicB.text = listDapAnLogicB[indexCauHoi];
            dapAnLogicC.text = listDapAnLogicC[indexCauHoi];
            dapAnLogicD.text = listDapAnLogicD[indexCauHoi];
            limitTime = 30;
        } else if(typeQuestion == 1) {
            canvasCauHoiChonAnh.SetActive(true);
            // int limitCauHoi = 3;
            // if(levelPlaying == 2)
            //     limitCauHoi = 2;
            // else if(levelPlaying == 3)
            //     limitCauHoi = 1;
            if(!anCauHoiFirstChonAnh) {
                anCauHoiFirstChonAnh = true;
                indexCauHoi = Random.Range(0, cauHoiChonAnhs.Length);
                listIndexCauHoiDaChonChonAnh[indexCauHoi] = true;
            } else {
                do {
                    indexCauHoi = Random.Range(0, cauHoiChonAnhs.Length);
                    if(!listIndexCauHoiDaChonChonAnh[indexCauHoi]) {
                        listIndexCauHoiDaChonChonAnh[indexCauHoi] = true;
                        break;
                    }
                } while(true);
            }
            cauHoiChonAnhs[indexCauHoi].SetActive(true);
            countTimDA = 0;
            limitTime = 20;
        } else {
            canvasCauHoiTimSo.SetActive(true);
            if(!anCauHoiFirstTimSo) {
                anCauHoiFirstTimSo = true;
                indexCauHoi = Random.Range(0, listCauHoiTimSos.Length);
                listIndexCauHoiDaChonTimSo[indexCauHoi] = true;
            } else {
                do {
                    indexCauHoi = Random.Range(0, listCauHoiTimSos.Length);
                    if(!listIndexCauHoiDaChonTimSo[indexCauHoi]) {
                        listIndexCauHoiDaChonTimSo[indexCauHoi] = true;
                        break;
                    }
                } while(true);
            }
            imageCauHoi.sprite = listCauHoiTimSos[indexCauHoi];
            limitTime = 60;
        }
        countLuotLVInlevels[typeQuestion - 1]--;
        buttonChooseTypeQuestions[typeQuestion - 1].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = countLuotLVInlevels[typeQuestion - 1] + " lượt";
        if(countLuotLVInlevels[typeQuestion - 1] <= 0) {
            buttonChooseTypeQuestions[typeQuestion - 1].transform.GetChild(0).GetComponent<Image>().sprite = buttonChooseQuests[1];
            buttonChooseTypeQuestions[typeQuestion - 1].GetComponent<Button>().enabled = false;
        }
    }
    private void UpdateIconChoosePlayer()
    {
        for(int i = 1 ; i < unLookPlayers.Length ; i++)
            if(unLookPlayers[i])
                iconPlayers[i].transform.GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(canvasGamePlay.activeInHierarchy && !isPlayBG)
            isPlayBG = true;
        if(isPlayBG && !canvasGamePlay.activeInHierarchy && isAttackSound) {
            soundManager.PlayBG();
            isPlayBG = false;
        }
        if(isChangeSound) {
            isChangeSound = false;
            if(isAttackSound) {
                if(!canvasGamePlay.activeInHierarchy)
                    soundManager.PlayBG();
            } else {
                soundManager.StopBG();
            }
        }
        if((canvasCauHoiChonAnh.activeInHierarchy || canvasCauHoiLogic.activeInHierarchy || canvasCauHoiTimSo.activeInHierarchy) && limitTime > 0) {
            curTimeCauHoi += Time.deltaTime;
            if(curTimeCauHoi >= 1) {
                curTimeCauHoi = 0;
                limitTime--;
                timeDongHoChonAnh.text = (limitTime > 9 ? "00:" : "00:0") + limitTime.ToString();
                timeDongHoLogic.text = (limitTime > 9 ? "00:" : "00:0") + limitTime.ToString();
                timeDongHoTimSo.text = (limitTime > 9 ? "00:" : "00:0") + limitTime.ToString();
            }
            if(limitTime <= 0) {
                soundManager.StopRun();
                if(typeQuestion == 2) {
                    canvasCauHoiLogic.SetActive(false);
                    this.PostEvent(EventID.beAttack);
                } else if(typeQuestion == 1) {
                    canvasCauHoiChonAnh.SetActive(false);
                    for(int i = 0 ; i < limitCountDAChonAnhs[indexCauHoi] ; i++) {
                        dapAnChonAnhs[indexCauHoi].transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 1, 0, 0);
                        dapAnChonAnhs[indexCauHoi].transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().color = new Color(0.5f, 1, 0, 0);
                    }
                    cauHoiChonAnhs[indexCauHoi].SetActive(false);
                } else if(typeQuestion == 3) {
                    canvasCauHoiTimSo.SetActive(false);
                    this.PostEvent(EventID.beAttack);
                }
                if(!canvasGamePlay.activeInHierarchy && isAttackSound)
                    soundManager.PlayBG();
                this.PostEvent(EventID.isPause, 0);
                limitTime = 20;
            }
        } 
        if(level == 2 && !isLevel2) {
            isLevel2 = true;
            khoaLV2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            khoaLV2.transform.GetChild(1).gameObject.SetActive(false);
        }
        if(level == 3 && !isLevel3) {
            isLevel3 = true;
            khoaLV3.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            khoaLV3.transform.GetChild(1).gameObject.SetActive(false);
        }
        if(!isDich && countCauHoiInLevel >= 3) {
            isDich = true;
            // canvasThongBao.SetActive(true);    
            // canvasThongBao.transform.GetChild(0).gameObject.SetActive(true);
            this.PostEvent(EventID.isDich);
        }
    }
    IEnumerator DelayRemoveTym() {
        yield return new WaitForSeconds(0.5f);
        tymGroup.transform.GetChild(0).GetComponent<Image>().fillAmount -= 1.0f/limitTym;
    }
    IEnumerator DelayAnThongBao() {
        yield return new WaitForSeconds(5);
        canvasThongBao.transform.GetChild(0).gameObject.SetActive(false);
        canvasThongBao.SetActive(false);
    }
    public void HackVang() {
        if(int.Parse(countVangTrangChu.text) <= 50000)
            countVangInLevel = 50000 - int.Parse(countVangTrangChu.text);
        if(int.Parse(countKimCuongTrangChu.text) <= 5000)
            countKimCuongInLevel = 5000 - int.Parse(countKimCuongTrangChu.text);
        UpdateScore();
    }
    public void OnClickDapAn(GameObject parent) {   // chọn ảnh
        if(parent.transform.GetChild(0).GetComponent<Image>().color.a == 1)    return;
        countTimDA++;
        parent.transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 1, 0, 1);
        parent.transform.GetChild(1).GetComponent<Image>().color = new Color(0.5f, 1, 0, 1);
        if(countTimDA == 1) {
            countCauHoiInLevel++;
            if(isAttackSound)
                soundManager.PlayTraLoiDung();
            countCauHoiGamePlay.text = countCauHoiInLevel.ToString();
        } else {
            countVangInLevel += 10;
            if(isAttackSound)
                soundManager.PlayTraLoiDung();
            countVangCanvasGamePlay.text = countVangInLevel.ToString();
        }
        if(countTimDA >= limitCountDAChonAnhs[indexCauHoi]) {
            soundManager.StopRun();
            if(!canvasGamePlay.activeInHierarchy && isAttackSound)
                    soundManager.PlayBG();
            canvasCauHoiChonAnh.SetActive(false);
            this.PostEvent(EventID.isPause, 0);
            for(int i = 0 ; i < limitCountDAChonAnhs[indexCauHoi] ; i++) {
                dapAnChonAnhs[indexCauHoi].transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 1, 0, 0);
                dapAnChonAnhs[indexCauHoi].transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().color = new Color(0.5f, 1, 0, 0);
            }
            cauHoiChonAnhs[indexCauHoi].SetActive(false);
            limitTime = 20;
            timeDongHoChonAnh.text = "00:20";
        }
    }
    public void OnClickDapAnLogic(int indexDapAn) {   // logic
        if(indexDapAn == listDapAnDungLogic[indexCauHoi]) {
            countCauHoiInLevel++;
            if(isAttackSound)
                soundManager.PlayTraLoiDung();
            countCauHoiGamePlay.text = countCauHoiInLevel.ToString();
            countVangInLevel += 100 * ((int) limitTime/10);
            countVangCanvasGamePlay.text = countVangInLevel.ToString();
        } else {
            this.PostEvent(EventID.beAttack);
            if(isAttackSound)
                soundManager.PlayTraLoiSai();
        }
        soundManager.StopRun();
        if(!canvasGamePlay.activeInHierarchy && isAttackSound)
                    soundManager.PlayBG();
        canvasCauHoiLogic.SetActive(false);
        this.PostEvent(EventID.isPause, 0);
        limitTime = 20;
        timeDongHoLogic.text = "00:30";
    }
    public void OnClickDapAnTimSo(InputField input) {   // logic
        if(input.text.CompareTo(listDapAnTimSos[indexCauHoi]) == 0) {
            countCauHoiInLevel++;
            if(isAttackSound)
                soundManager.PlayTraLoiDung();
            countCauHoiGamePlay.text = countCauHoiInLevel.ToString();
            // >= 50s : 100, >= 40s: 80, >= 30s: 60, >= 20s: 40, >= 10s: 20, < 10s: 10
            int x = 10;
            if(limitTime >= 50)
                x = 100;
            else if(limitTime >= 40)
                x = 80;
            else if(limitTime >= 30)
                x = 60;
            else if(limitTime >= 20)
                x = 40;
            else if(limitTime >= 10)
                x = 20;
            countKimCuongInLevel += x;
            countKimCuongCanvasGamePlay.text = countKimCuongInLevel.ToString();
        } else {
            this.PostEvent(EventID.beAttack);
            if(isAttackSound)
                soundManager.PlayTraLoiSai();
        }
        soundManager.StopRun();
        if(!canvasGamePlay.activeInHierarchy && isAttackSound)
                    soundManager.PlayBG();
        canvasCauHoiTimSo.SetActive(false);
        this.PostEvent(EventID.isPause, 0);
        limitTime = 20;
        input.text = "";
        timeDongHoTimSo.text = "01:00";
    }

    IEnumerator DelayLoading() {
        yield return new WaitForSeconds(2.5f);
        canvasLoading.SetActive(false);
        canvasTrangChu.SetActive(true);
    }

    public void OnClickStartGame() {
        if(isAttackSound)
            soundManager.PlayClick();
        canvasTrangChu.SetActive(false);
        canvasChooseLevel.SetActive(true);
    }

    public void OnClickBackHome() {
        if(isAttackSound)
            soundManager.PlayClick();
        countVangInLevel = 0;
        countKimCuongInLevel = 0;
        countCauHoiInLevel = 0;
        canvasChooseLevel.SetActive(false);
        canvasTrangChu.SetActive(true);
    }

    public void OnClickChooseLevel(int level) {
        if(isAttackSound)
            soundManager.PlayClick();
        if(level <= this.level) {
            canvasChooseLevel.SetActive(false);
            canvasChoosePlayer.SetActive(true);
            levelPlaying = level;
        }
    }
    public void OnClickBackToChooseLevel() {
        if(isAttackSound)
            soundManager.PlayClick();
        canvasChoosePlayer.SetActive(false);
        canvasChooseLevel.SetActive(true);
    }

    public void OnClickChoosePlayer(int indexPlayer) {
        /*
            - ninjaBoy(0): hp - 3, atk - 1
            - ninjaGirl(1): hp - 2, atk - 2
            - caoBoiBoy(3): hp - 4, atk - 3
            - couBoiGirl(4): hp - 3, atk - 4
        */
        if(!unLookPlayers[indexPlayer]) return;
        UpdateInforPlayer(indexPlayer);
        playerChoose = indexPlayer;
        canvasChoosePlayer.SetActive(false);
        canvasBG.SetActive(false);
        canvasGamePlay.SetActive(true);
        canvasDieuKhien.SetActive(true);
        Replay(1, levelPlaying);
    }

    private void UpdateInforPlayer(int indexPlayer)
    {
        if(indexPlayer == 0) {
            countTym = 3;
            limitTym = 3;
            atk = 1;
            indexPlayerChooseInLevel = 0;
        } else if(indexPlayer == 1) {
            countTym = 2;
            limitTym = 2;
            atk = 2;
            indexPlayerChooseInLevel = 1;
        } else if(indexPlayer == 2) {
            countTym = 4;
            limitTym = 4;
            atk = 3;
            indexPlayerChooseInLevel = 2;
        } else if(indexPlayer == 3) {
            countTym = 3;
            limitTym = 3;
            atk = 4;
            indexPlayerChooseInLevel = 3;
        }
        tymGroup.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
    }

    public void OnClickPauseGame() {
        if(isAttackSound)
            soundManager.PlayClick();
        canvasPause.SetActive(true);
        this.PostEvent(EventID.isPause, 1);
    }
    public void OnClickReplay(int type) {
        if(isAttackSound)
            soundManager.PlayClick();
        //1: lose, 2: win
        if(type == 1) {
            canvasPause.SetActive(false);
            canvasLose.SetActive(false);
            Replay(1, levelPlaying);
        } else {
            canvasWin.SetActive(false);
            Replay(2, levelPlaying);
        }
    }
    public void OnContinue() {
        if(isAttackSound)
            soundManager.PlayClick();
        canvasPause.SetActive(false);
        this.PostEvent(EventID.isPause, 0);
    }
    public void OnClickGamePlayToHome() {
        if(isAttackSound)
            soundManager.PlayClick();
        canvasShop.SetActive(false);
        canvasPause.SetActive(false);
        canvasLose.SetActive(false);
        canvasWin.SetActive(false);
        canvasBG.SetActive(true);
        canvasGamePlay.SetActive(false);
        canvasDieuKhien.SetActive(false);
        canvasTrangChu.SetActive(true);
        UpdateScore();
        DestroyLevel();
    }
    private void Replay(int typeReplay, int level) {
        //1: lose, 2: win
        if(typeReplay == 2) {
            UpdateScore();
        }
        soundManager.StopBG();
        cupWin.transform.GetChild(3).gameObject.SetActive(false);
        cupWin.transform.GetChild(1).gameObject.SetActive(false);
        cupWin.transform.GetChild(2).gameObject.SetActive(false);
        listCauHoiChonInlevels.Clear();
        for (int i = 0; i < listIndexCauHoiDaChonChonAnh.Length; i++)
        {   
            listIndexCauHoiDaChonChonAnh[i] = false;
        }
        for (int i = 0; i < listIndexCauHoiDaChonLogic.Length; i++)
        {   
            listIndexCauHoiDaChonLogic[i] = false;
        }
        canvasDieuKhien.SetActive(true);
        countCauHoiInLevel = 0;
        countVangInLevel = 0;
        countKimCuongInLevel = 0;
        UpdateInforPlayer(indexPlayerChooseInLevel);
        countCauHoiGamePlay.text = countCauHoiInLevel.ToString();
        countVangCanvasGamePlay.text = countVangInLevel.ToString();
        countKimCuongCanvasGamePlay.text = countKimCuongInLevel.ToString();
        isDich = false;
        for (int i = 0; i < tymGroup.transform.childCount; i++)
        {
            tymGroup.transform.GetChild(i).gameObject.SetActive(true);
        }
        countLuotLVInlevels.Clear();
        for(int i = 0 ; i < countLuotLV1s.Count ; i++){
            if(levelPlaying == 1) 
                countLuotLVInlevels.Add(countLuotLV1s[i]);
            else if(levelPlaying == 2) 
                countLuotLVInlevels.Add(countLuotLV2s[i]);
            else if(levelPlaying == 3) 
                countLuotLVInlevels.Add(countLuotLV3s[i]);
        }
        for(int i = 0 ; i < 3 ; i++) {
            buttonChooseTypeQuestions[i].transform.GetChild(0).GetComponent<Image>().sprite = buttonChooseQuests[0];
            buttonChooseTypeQuestions[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = countLuotLVInlevels[i] + " lượt";
            buttonChooseTypeQuestions[i].GetComponent<Button>().enabled = true;
        }
        if(level == 1) {
            DestroyLevel();
            curLevel1 = Instantiate(level1, Vector3.zero, Quaternion.identity);
        } else if(level == 2) {
            DestroyLevel();
            curLevel2 = Instantiate(level2, Vector3.zero, Quaternion.identity);
        } else if(level == 3) {
            DestroyLevel();
            curLevel3 = Instantiate(level3, Vector3.zero, Quaternion.identity);
        }
        this.PostEvent(EventID.isStartGame);
    }
    public void SetAmThanh() {
        // if(isAmThanh) {
        //     soundManager.StopBG();
        //     isAmThanh = false;
        // } else {
        //     soundManager.PlayBG();
        //     isAmThanh = true;
        // }
        isAttackSound = !isAttackSound;
        isChangeSound = true;
    }
    public void OnNextLevel() {
        canvasWin.SetActive(false);
        canvasChooseLevel.SetActive(true);
        UpdateScore();
    }
    private void DestroyLevel() {
        if(curLevel1 != null)
            Destroy(curLevel1.gameObject);
        if(curLevel2 != null)
            Destroy(curLevel2.gameObject);
        if(curLevel3 != null)
            Destroy(curLevel3.gameObject);
    }
    private void UpdateScore() {
        var countVang = int.Parse(countVangTrangChu.text);
        countVangTrangChu.text = (countVang + countVangInLevel).ToString();
        var countKimCuong = int.Parse(countKimCuongTrangChu.text);
        countKimCuongTrangChu.text = (countKimCuong + countKimCuongInLevel).ToString();
        countVangInLevel = 0;
        countKimCuongInLevel = 0;
        countCauHoiInLevel = 0;
    }
    public void OnClickShop() {
        canvasTrangChu.SetActive(false);
        countVangShop.text = countVangTrangChu.text;
        countKimCuongShop.text = countKimCuongTrangChu.text;
        canvasShop.SetActive(true);
    }
    public void OnClickBuy(int indexPlayerBuy) {
        int vang = 10000;
        int kimCuong = 0;
        if(indexPlayerBuy == 2) {
            vang = 0;
            kimCuong = 1000;
        }
        if(indexPlayerBuy == 3) {
            vang = 0;
            kimCuong = 2000;
        }
        // 1: 200 - 2: 9999 - 3: 9999
        if(indexPlayerBuy < 2) {
            if(int.Parse(countVangShop.text) < vang)    return;
            countVangTrangChu.text = (int.Parse(countVangTrangChu.text) - vang).ToString();
            countVangShop.text = countVangTrangChu.text;
        }
        else {
            if(int.Parse(countKimCuongShop.text) < kimCuong)    return;
            countKimCuongTrangChu.text = (int.Parse(countKimCuongTrangChu.text) - kimCuong).ToString();
            countKimCuongShop.text = countKimCuongTrangChu.text;
        }
        unLookPlayers[indexPlayerBuy] = true;
        UpdateIconChoosePlayer();
        listPlayerInShops[indexPlayerBuy].GetComponent<Image>().sprite = newBGShop;
        listPlayerInShops[indexPlayerBuy].transform.GetChild(2).gameObject.SetActive(false);
    }
}
