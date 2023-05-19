using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float limitX = 6;
    private float posX = 0;
    void Start()
    {
        posX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x > 0) {
            transform.position += new Vector3(0.2f, 0, 0);
            if(transform.position.x > posX + limitX)
                Destroy(gameObject);
        } else {
            transform.position += new Vector3(-0.2f, 0, 0);
            if(transform.position.x < posX - limitX)
                Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("boss")) {
            this.PostEvent(EventID.attackBoss, obj.gameObject.GetComponent<BossScript>().idBoss);
            Destroy(gameObject);
        }
    }
}
