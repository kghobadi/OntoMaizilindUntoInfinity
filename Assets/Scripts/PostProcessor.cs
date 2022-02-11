using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessor : MonoBehaviour
{
    //spectrum ref
    AudioSpectrum spectrum;

    //post processing profiler references
    [Header("Post Processing")]
    public PostProcessingProfile myPost;
    public ColorGradingModel.Settings colorGrader;

    //for color shifts
    public bool positiveHueShift, positiveTempShift, positiveTintShift;

    //for min level color shifts
    public float minHueShift, minTempShift, minTintShift;

    //for random timers color shifts
    public float hueTimer, tempTimer, tintTimer, shiftTimerTotal = 5;
    public float tempShifter, tintShifter, hueShifter, satShifter, contrastShifter;
    public bool hueCanShift= true, tempCanShift=true, tintCanShift=true;

    public int globalLevelRef, hueLevelRef, tintLevelRef, tempLevelRef;

    public bool colorShiftingOn;

    //nuclear bomb stuff 
    [Header("Nuclear Bombs")]
    public Transform explosionParent;
    public GameObject nuclearBomb;
    public int bombLevelRef;
    public float bombMin;
    public int bombCounter = 0;
    public bool canSpawnBombs = true;
    public float bombTimer, bombTimerTotal = 0.1f;
    public Camera player;
    public AudioSource music;

    //calibrate all the post processing values at start because these change outside playmode
    void Start()
    {
        //reset timers
        hueTimer = shiftTimerTotal;
        tempTimer = shiftTimerTotal;
        tintTimer = shiftTimerTotal;    

        spectrum = GetComponent<AudioSpectrum>();

        colorGrader = myPost.colorGrading.settings;

        //reset starting values of everything to 'normal'
        colorGrader.basic.temperature = 0;
        colorGrader.basic.tint = 0;
        colorGrader.basic.hueShift = 0;
        colorGrader.basic.saturation = 1;
        colorGrader.basic.contrast = 1;

        myPost.colorGrading.settings = colorGrader;

        //start with positive color values
        positiveHueShift = true;
        positiveTempShift = true;
        positiveTintShift = true;

        player = Camera.main;
    }

    void Update()
    {
        if (colorShiftingOn)
        {
            ColorMultipliers();
        }
        
        myPost.colorGrading.settings = colorGrader;
        
        //check if music was loud enough to spawn bomb 
        //Debug.Log("nuclear level = " + (spectrum.MeanLevels[bombLevelRef] * 100));
        if(spectrum.MeanLevels[bombLevelRef] * 100 > bombMin && canSpawnBombs)
        {
            SpawnBomb();
        }

        if (!canSpawnBombs)
        {
            bombTimer -= Time.deltaTime;

            if(bombTimer < 0)
            {
                canSpawnBombs = true;
            }
        }

        if (colorShiftingOn)
        {
            MinLevelColorShifts();
            CanShiftTimers();
            //SwitchLevelRefs();
        }
    }

    void SpawnBomb()
    {
        Vector2 xz = Random.insideUnitCircle * 2500;
        Vector3 spawnPos = new Vector3(xz.x, 0, xz.y);
        GameObject nuke = Instantiate(nuclearBomb, spawnPos, Quaternion.identity, explosionParent);
        bombCounter++;
        canSpawnBombs = false;
        bombTimer = bombTimerTotal;
        player.transform.LookAt(nuke.transform.position + new Vector3(0,50f,0));
        Debug.Log("spawned abomb");
    }

    //tie parts of colorGrader to audioSpectrum values
    void ColorMultipliers() {
        //tint shift
        if (positiveTintShift)
        {
            colorGrader.basic.tint = spectrum.MeanLevels[globalLevelRef] * tintShifter;
        }
        else
        {
            colorGrader.basic.hueShift = spectrum.MeanLevels[globalLevelRef] * -hueShifter;
        }

        //hue shift
        if (positiveHueShift)
        {
            colorGrader.basic.hueShift = spectrum.MeanLevels[globalLevelRef] * hueShifter;

        }
        else
        {
            colorGrader.basic.tint = spectrum.MeanLevels[globalLevelRef] * -tintShifter;
        }

        //temp shift
        if (positiveTempShift)
        {
            colorGrader.basic.temperature = spectrum.MeanLevels[globalLevelRef] * tempShifter;
        }
        else
        {
            colorGrader.basic.temperature = spectrum.MeanLevels[globalLevelRef] * -tempShifter;
        }

        //if (spectrum.MeanLevels[levelRef] * satShifter > 1)
        //{
        //    colorGrader.basic.saturation = spectrum.MeanLevels[levelRef] * satShifter;
        //    colorGrader.basic.contrast = spectrum.MeanLevels[levelRef] * contrastShifter;
        //}
    }

    public void DisableExplosions()
    {
        explosionParent.gameObject.SetActive(false);
        canSpawnBombs = false;
        bombTimer = 100f;
    }

    //set level reference from audio spectrum using number keys
    void SwitchLevelRefs()
    {
        //for checking best levels per song
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            globalLevelRef = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            globalLevelRef = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            globalLevelRef = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            globalLevelRef = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            globalLevelRef = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            globalLevelRef = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            globalLevelRef = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            globalLevelRef = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            globalLevelRef = 9;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            globalLevelRef = 0;
        }
    }

    //shift from positive to 
    void MinLevelColorShifts()
    {
        //hue
        if (hueCanShift)
        {
            if (spectrum.MeanLevels[globalLevelRef] * 100 > minHueShift)
            {
                positiveHueShift = !positiveHueShift;
                hueCanShift = false;
            }
        }

        //tint
        if (tintCanShift)
        {
            if (spectrum.MeanLevels[globalLevelRef] * 100 > minTintShift)
            {
                positiveTintShift = !positiveTintShift;
                tintCanShift = false;
            }
        }

        //temp
        if (tempCanShift)
        {
            if (spectrum.MeanLevels[globalLevelRef] * 100 > minTempShift)
            {
                positiveTempShift = !positiveTempShift;
                tempCanShift = false;
            }
        }
       
    }

    //How can i reactively set the ShiftTimerTotal based on a songs tempo?
    //and what about the level mins?? we need to somehow analyze the song before or while it plays using the spectrum

    //these are timers reset color shift 
    void CanShiftTimers()
    {
        if (!hueCanShift)
        {
            hueTimer -= Time.deltaTime;
            if (hueTimer < 0)
            {
                hueCanShift = true;
                hueTimer = shiftTimerTotal ;
            }
        }

        if (!tempCanShift)
        {
            tempTimer -= Time.deltaTime;
            if (tempTimer < 0)
            {
                tempCanShift = true;
                tempTimer = shiftTimerTotal -2;
            }
        }

        if (!tintCanShift)
        {
            tintTimer -= Time.deltaTime;
            if (tintTimer < 0)
            {
                tintCanShift = true;
                tintTimer = shiftTimerTotal + 2;
            }
        }
        

    }


}

