using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    public List<float> targetWeights = new() { 0.0f, 0.0f, 0.0f };
    public List<float> struggleWeights = new() { 0.0f, 0.0f, 0.0f };

    public List<float> bellied = new() { 100f };
    public float eatTime;
    public float eating = 100f;
    public float last_belly_size = 0;

    public GameObject scavengerPrefab;
    public GameObject mealHead;

    // Start is called before the first frame update
    void Start()
    {
        eatTime = Time.time;
        var scavenger = Instantiate(scavengerPrefab);
        scavenger.transform.position = mealHead.transform.parent.position;
        scavenger.transform.rotation = gameObject.transform.rotation;
        GetComponentInChildren<Animator>().SetBool("Eat", true);
        scavenger.transform.Find("Scavenger").GetComponent<Animator>().SetBool("Eat", true);
    }

    // Update is called once per frame
    void Update()
    {
        //var offset = Time.time - start;
        //GetComponentInChildren<Animator>().SetLayerWeight(2, Fix((-Mathf.Pow(offset/3f-2.5f, 2)+1)));
        var creatureAnimator = GetComponentInChildren<Animator>();
        float bellySize = 0;

        if (bellied.Any())
        {
            bellySize = 0.25f + bellied.Sum(food => food / 300f);
        }

        if (eating != null)
        {
            var value = -Mathf.Pow((Time.time - eatTime) / 4f - 1.9f, 2) + 1;

            Debug.Log(Mathf.Min(0, value) * (eating / 300f) + " = " + Mathf.Min(0, value) + " * " + (eating / 300f));
            bellySize += Mathf.Min(0, value) * eating / 300f;
        }

        last_belly_size += (Mathf.Max(0, Mathf.Min(1, bellySize)) - last_belly_size) * Time.deltaTime;
        
        creatureAnimator.SetLayerWeight(1, last_belly_size);
        creatureAnimator.SetLayerWeight(2, Mathf.Sin(Time.time) * 0.5f * last_belly_size);
        creatureAnimator.SetLayerWeight(3, Mathf.Sin(Time.time / 2) * 0.5f * last_belly_size);
        creatureAnimator.SetLayerWeight(4, Mathf.Sin(Time.time / 3) * 0.5f * last_belly_size);
        creatureAnimator.SetLayerWeight(5, Mathf.Sin(Time.time / 5) * 0.5f * last_belly_size);
        for (int i = 0; i < 3; i++)
        {
            targetWeights[i] = Mathf.Min(1, Mathf.Max(0, targetWeights[i] - Time.deltaTime));
            struggleWeights[i] += (targetWeights[i] - struggleWeights[i]) * Time.deltaTime;
            for (int j = 6 + i * 4; j < 8 + i * 4; j++)
            {
                creatureAnimator.SetLayerWeight(j, Mathf.Min(1, Mathf.Max(0,
                    (struggleWeights[i] + Mathf.Sin(Time.time / j) * 0.1f) * (bellySize * 2))));
            }
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            targetWeights[0] = Mathf.Min(targetWeights[0] + 0.25f, 1f);
        }
        else if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
        {
            targetWeights[1] = Mathf.Min(targetWeights[1] + 0.25f, 1f);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            targetWeights[2] = Mathf.Min(targetWeights[2] + 0.25f, 1f);
        }
    }
}