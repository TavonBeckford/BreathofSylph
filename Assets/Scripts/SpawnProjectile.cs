using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    public GameObject firePoint;
    public List<GameObject> vfx = new List<GameObject>();

    private GameObject effectToSpawn;
    public float timeToFire = 0;

    public bool firePointEquipped = false;

    // Update is called once per frame

    private void Start()
    {
        effectToSpawn = vfx[0];
    }


    void Update()
    {
        
        if (IsFirePointActiveAndEquipped())
        {
            if (Time.time >= timeToFire)
            {
                if (effectToSpawn != null && effectToSpawn.GetComponent<ProjectileMove>() != null)
                {
                    timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().firerate;
                    SpawnVFX();
                }
                else
                {
                    Debug.LogError("ProjectileMove component or firerate is missing on the projectile effect.");
                }
            }
        }
        else
        {
            // Optionally provide feedback or handle the case where the fire point is not active or not equipped
            Debug.Log("Firepoint is not active or not equipped to the player.");
        }
    }

    public bool IsFirePointActiveAndEquipped()
    {
        /*
        // Find the firePoint GameObject with the "FirePoint" tag
        firePoint = GameObject.FindGameObjectWithTag("FirePoint");

        if (firePoint != null)
        {
            GameObject parentOfTheFirePoint = firePoint.transform.parent?.gameObject;

            if (parentOfTheFirePoint != null && parentOfTheFirePoint.activeInHierarchy)
            {
                // Check if the firePoint is equipped to the player using the EquipmentManager
                EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();

                if (equipmentManager != null && equipmentManager.IsFirePointEquipped(parentOfTheFirePoint))
                {
                    Debug.Log("The firepoint is equipped");
                    firePointEquipped = true;
                    return true;
                }
            }
        }

        return false;
        */

        // Find all GameObjects with the "FirePoint" tag
        GameObject[] firePoints = GameObject.FindGameObjectsWithTag("FirePoint");

        foreach (GameObject fp in firePoints)
        {

            GameObject parentOfTheFirePoint = fp.transform.parent?.gameObject;

            if (parentOfTheFirePoint != null && parentOfTheFirePoint.activeInHierarchy)
            {
                // Check if the firePoint is equipped to the player using the EquipmentManager
                EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();

                if (equipmentManager != null && equipmentManager.IsFirePointEquipped(parentOfTheFirePoint))
                {
                    firePoint = fp;
                    firePointEquipped = true;
                    return true;
                }
            }
        }

        // None of the fire points are equipped
        firePointEquipped = false;
        return false;


    }




    private void SpawnVFX()
    {
        GameObject spawnedVFX;

        if (firePoint != null)
        {
            spawnedVFX = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No active firepoint");
        }
    }
}
