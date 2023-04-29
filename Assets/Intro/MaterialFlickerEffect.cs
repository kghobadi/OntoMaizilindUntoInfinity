using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFlickerEffect : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Renderer rend;
    [Tooltip("Minimum random light intensity")]
    public Color minIntensity;
    [Tooltip("Maximum random light intensity")]
    public Color maxIntensity;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;
    public string Property;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;


    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
    }

    void Update()
    {
        if (rend == null)
            return;

        // pop off an item if too big
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity.a, maxIntensity.a);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;
        float tempA = lastSum / (float)smoothQueue.Count;

        // Calculate new smoothed average
        rend.material.color = new Color (minIntensity.r, minIntensity.g, minIntensity.b, tempA);
    }

}