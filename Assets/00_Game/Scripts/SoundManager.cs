using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource audioSourceFX;
    public AudioSource audioSourceBG;
    public AudioSource audioRun;
    public AudioClip clipClick, clipAnVP, clipAnCauHoi, clipTraLoiDung, clipTraLoiSai;
    public AudioClip clipBeAttackBoss, clipBeAttackPlayer, clipAttackSung, clipAttackKiem, clipWin, clipLose, clipBossAttack;

    public void PlayClick() {
            audioSourceFX.PlayOneShot(clipClick);
    }
    public void PlayAnVP() {
            audioSourceFX.PlayOneShot(clipAnVP);
    }
    public void PlayAnCauHoi() {
            audioSourceFX.PlayOneShot(clipAnCauHoi);
    }
    public void PlayTraLoiDung() {
            audioSourceFX.PlayOneShot(clipTraLoiDung);
    }
    public void PlayTraLoiSai() {
            audioSourceFX.PlayOneShot(clipTraLoiSai);
    }
    public void PlayBeAttackBoss() {
            audioSourceFX.PlayOneShot(clipBeAttackBoss);
    }
    public void PlayBeAttackPlayer() {
            audioSourceFX.PlayOneShot(clipBeAttackPlayer);
    }
    public void PlayAttackSung() {
            audioSourceFX.PlayOneShot(clipAttackSung);
    }
    public void PlayAttackKiem() {
            audioSourceFX.PlayOneShot(clipAttackKiem);
    }
    public void PlayWin() {
            audioSourceFX.PlayOneShot(clipWin);
    }
    public void PlayBossAttack() {
        audioSourceFX.PlayOneShot(clipBossAttack);
    }
    public void PlayLose() {
            audioSourceFX.PlayOneShot(clipLose);
    }
    public void PlayBG() {
        audioSourceBG.Play();
    }
    public void StopBG() {
        audioSourceBG.Stop();
    }
    public void PlayRun() {
        if(GlobalInstance.instance.gameManagerInstance.isAttackSound)
            audioRun.Play();
    }
    public void StopRun() {
        if(GlobalInstance.instance.gameManagerInstance.isAttackSound)
            audioRun.Stop();
    }
}

