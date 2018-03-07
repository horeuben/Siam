﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    //Movement
    public float moveSpeed;
    public float drag;
    public float terminalRotationSpeed;
    public VirtualJoystick joystick;
    private float stunDuration;
    //public GameObject camera;

    //Health system
    private int maxHealth = 7;
    private int startHealth = 5;
    public int currHealth;

    public Image[] healthImages;

    //Scoring
    public int score;

    //Game objects
    public GameObject arrowSpawnPoint;
    public float waitTime;
    public GameObject arrow;
    public GameObject taichiObj;
  
    private Rigidbody controller;
    private GameObject taichiShield;

    private void Start()
    {
        controller = GetComponent<Rigidbody>();
        controller.maxAngularVelocity = terminalRotationSpeed;
        controller.drag = drag;
        taichiShield = taichiObj;
        currHealth = startHealth;
        UpdateUIHealth();
        stunDuration = 0;
    }

    // Update is called once per frame
   private void Update() {
       
        Vector3 dir = Vector3.zero;
        // for keyboard movement
        /*    dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");
            dir *= moveSpeed * Time.deltaTime;
            if (dir.magnitude > 1)
                dir.Normalize();*/
        if (currHealth <= 0)
            Destroy(this.gameObject);
        else
        {
            if (stunDuration <= 0)
            { 
                stunDuration = 0;
                if (joystick.inputVector != Vector3.zero)
                {
                    dir = joystick.inputVector;
                    controller.MovePosition(transform.position + dir);
                    transform.forward = dir;
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                    taichiShield.transform.position = transform.position;
                }

                //TODO: detect projectiles using triggers in order to know what to do
                //boss projectile: - hp, player projectile, stun? pickups: do smth


                //shoot testing, can be removed later
                if (Input.GetKeyDown(KeyCode.Space))//Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            }
            else
            {
                stunDuration -= 1 * Time.deltaTime;
            }
        }
    }

    //Instantiate an arrow object to shoot at current direction character is facing
    public void Shoot()
    {
        Instantiate(arrow.transform, arrowSpawnPoint.transform.position,arrowSpawnPoint.transform.rotation);
    }

    //Instantiate a taichi shield around character to reflect all incoming projectiles for x seconds
    public void Taichi()
    {
        //spawn taichi shield for x seconds
        taichiShield = Instantiate(taichiObj,transform.position,transform.rotation);
        //uses animation created by cyrus.
        //makes character immune for x seconds (disable rigidbody collider maybe)
        //makes projectiles that collides with character to be sent to direction player is facing.
    }

    //Update UI to display current hp user has
    void UpdateUIHealth()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (currHealth <= i)
            {
                healthImages[i].enabled = false;
            }
            else
            {
                healthImages[i].enabled = true;
            }
        }
    }

    //Add or deal damage to user, negative means damage
    public void AddHp(int amount)
    {
        currHealth += amount;
        currHealth = Mathf.Clamp(currHealth, 0, maxHealth);
        UpdateUIHealth();
    }

    //Add skill drop from map to player
    public void AddSkill()
    {

    }

    //Use added skill of player
    public void UseAddedSkill()
    {

    }

    // if touch object that triggers effect
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkillDrop"))
        {
            // get a random skill
            AddHp(1);
            Destroy(other.gameObject);
        }
    }
    // if collide onto other objects
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BossProjectile"))
        {
            AddHp(-1);
            stunDuration += 1f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Arrow"))
        {
            //set stun
            stunDuration += 1f;
            Destroy(other.gameObject);
        }

    }
}
