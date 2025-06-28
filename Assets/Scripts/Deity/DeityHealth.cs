using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Handles the health and behavior status of Deities. 
/// </summary>
public class DeityHealth : MonoBehaviour 
{
    Deity deity;
    DeityManager deityMan;
    DeitySound _Sounds;
    DeityAnimations _Animations;

    public Deity MyDeity => deity;
  
    MeshRenderer mRender; 
    MeshRenderer[] mRenderers;

    [Tooltip("Check if this is Deity VII")]
    public bool destroyerOfWorlds;

    private int totalHP;
    public int healthPoints = 133;
    [SerializeField] private Image healthbar;
    [SerializeField] private CanvasGroup healthGroup;
    public ObjectPooler splosionPooler;
    [SerializeField] public Vector3 splosionOffset = new Vector3(0, 0, -3);
    
    [SerializeField]
    private CloudGenerator deityCloudGen;

    public HealthStates healthState;
    public enum HealthStates
    {
        ALIVE, FALLING, CRASHED,
    }

    public LayerMask groundedMask;
    Vector3 crashPoint;
    public Material lifeMat, deathMat;
    public float fallSpeed;
    private float zSpeed = 100f;

    public ParticleSystem exploded;
    public Hallucination deathHallucination;
    
    private void Awake()
    {
        totalHP = healthPoints;
        deityMan = FindObjectOfType<DeityManager>();
        _Sounds = GetComponent<DeitySound>();
        if(_Sounds == null)
        {
            _Sounds = GetComponentInParent<DeitySound>();
        }
        _Animations = GetComponentInParent<DeityAnimations>();
        deity = GetComponentInParent<Deity>();
        mRender = GetComponent<MeshRenderer>();

        if (mRender == null)
        {
            mRenderers = GetComponentsInChildren<MeshRenderer>();
        }
    }

    private void Start()
    {
        healthState = HealthStates.ALIVE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            //take damage
            TakeDamage(other.gameObject, 1);
        }

        if(other.CompareTag("Ground") && healthState == HealthStates.FALLING)
        {
            //crash
            Crash();
        }
    }

    public void TakeDamage(GameObject bull, int dmgAmt)
    {
        //get bullet
        Bullet bullet = bull.GetComponent<Bullet>();
        //spawn splosion
        GameObject splosion = splosionPooler.GrabObject();
        splosion.transform.position = bull.transform.position + splosionOffset;
        splosion.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        //particle system
        ParticleSystem sParticles = splosion.GetComponent<ParticleSystem>();
        sParticles.Play();
        //reset bullet
        bullet.ResetBullet(transform);
        //sub health
        healthPoints-= dmgAmt;
        //explosion sound 
        int voiceToCheck = _Sounds.CountUpArray(_Sounds.voiceCounter, _Sounds.voices.Length - 1);
        if(_Sounds.voices[voiceToCheck].isPlaying == false)
            _Sounds.PlaySoundMultipleAudioSources(_Sounds.explosionSounds);
        
        if(healthbar)
            healthbar.fillAmount = (float) healthPoints / totalHP;
        if (healthPoints <= 0 && healthState == HealthStates.ALIVE)
        {
            //Fall
            Fall();
        }
    }

    void Fall()
    {
        //disable healthbar 
        if (healthGroup)
        {
            LeanTween.alphaCanvas(healthGroup, 0f, 3f);
        }
            
        //fall anim
        _Animations.Animator.SetTrigger("fall");
        //find spot on ground in front of me to move towards at fall speed
        crashPoint = RaycastToGround();
        //set falling 
        deity.mover.MoveTo(crashPoint, fallSpeed);
        //change health state
        healthState = HealthStates.FALLING;
        //invoke deity died event. 
        deityMan.OnDeityDied();
        //stop strafing
        deity.SetFall();
        //alien sound 
        _Sounds.PlayRandomSoundRandomPitch(_Sounds.deathSounds, _Sounds.myAudioSource.volume);
        //play halluc
        if(deathHallucination)
            deathHallucination.PlayHallucination();
    }

    //when i hit the ground and explode
    void Crash()
    {
        //set mats 
        if(mRender)
            mRender.material = deathMat;
        else
        {
            for (int i = 0; i < mRenderers.Length; i++)
            {
                mRenderers[i].material = deathMat;
            }
        }
        exploded.Play();
        healthState = HealthStates.CRASHED;
        deity.SetCrash();

        //get final rest pos 
        Vector3 finalRestPos = new Vector3(transform.position.x, transform.position.y, -500f); //TODO this didnt work for the envy squid. Is it crashing properly?
        //set deity to move with terrain 
        deity.mover.MoveTo(finalRestPos, zSpeed); 
        //disable deity cloud gen
        deityCloudGen.gameObject.SetActive(false);
    }

    //finds point below deity to move to 
    Vector3 RaycastToGround()
    {
        RaycastHit hit;
        Vector3 down = transform.TransformDirection(Vector3.down);
        //check for point on ground 
        if (Physics.Raycast(transform.position, down, out hit, Mathf.Infinity, groundedMask))
        {
            return hit.point;
        }
        //return point that's 1000 below me
        else
        {
            return new Vector3(transform.position.x, transform.position.y - 1050, transform.position.z);
        }
    }
}
