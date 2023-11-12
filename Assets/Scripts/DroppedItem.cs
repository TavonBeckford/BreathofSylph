using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public float[] dropRates;

    void Start()
    {

    }

    public GameObject DropItem()
    {
        float randomValue = Random.value;

        float cumulativeDropRate = 0f;
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            cumulativeDropRate += dropRates[i];

            if (randomValue <= cumulativeDropRate)
            {
                GameObject itemDrop = Instantiate(itemPrefabs[i], transform.position, Quaternion.identity);
                return itemDrop;
            }
        }

        // If the loop completes without returning, return null or handle the case accordingly
        return null;
    }
}

