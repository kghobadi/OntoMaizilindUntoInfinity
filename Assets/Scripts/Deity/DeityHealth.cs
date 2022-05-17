using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeityHealth : MonoBehaviour {
    DeityManager deityMan;
    DeitySound _Sounds;
    DeityAnimations _Animations;
    [HideInInspector] public Deity deity;
    MeshRenderer mRender; 
    MeshRenderer[] mRenderers;

    [Tooltip("Check if this is Deity VII")]
    public bool destroyerOfWorlds;
    public int healthPoints = 133;
    public ObjectPooler splosionPooler;

    public HealthStates healthState;
    public enum HealthStates
    {
        ALIVE, FALLING, CRASHED,
    }

    public LayerMask groundedMask;
    Vector3 crashPoint;
    public Material lifeMat, deathMat;
    public float fallSpeed;

    public ParticleSystem exploded;
    public Hallucination deathHallucination;
    
    private void Awake()
    {
        deityMan = FindObjectOfType<DeityManager>();
        _Sounds = GetComponent<DeitySound>();
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
        if(other.tag == "Bullet")
        {
            //take damage
            TakeDamage(other.gameObject);
        }

        if(other.tag == "Ground")
        {
            //crash
            Crash();
        }
    }

    void TakeDamage(GameObject bull)
    {
        //get bullet
        Bullet bullet = bull.GetComponent<Bullet>();
        //spawn splosion
        GameObject splosion = splosionPooler.GrabObject();
        splosion.transform.position = bull.transform.position;
        //particle system
        ParticleSystem sParticles = splosion.GetComponent<ParticleSystem>();
        sParticles.Play();
        //reset bullet
        bullet.ResetBullet(transform);
        //sub health
        healthPoints--;
        //explosion sound 
        int voiceToCheck = _Sounds.CountUpArray(_Sounds.voiceCounter, _Sounds.voices.Length - 1);
        if(_Sounds.voices[voiceToCheck].isPlaying == false)
            _Sounds.PlaySoundMultipleAudioSources(_Sounds.explosionSounds);


        if (healthPoints <= 0 && healthState == HealthStates.ALIVE)
        {
            //Fall
            Fall();
        }
    }

    void Fall()
    {
        //fall anim
        _Animations.Animator.SetTrigger("fall");
        //find spot on ground in front of me to move towards at fall speed
        crashPoint = RaycastToGround();
        //set falling 
        deity.mover.MoveTo(crashPoint, fallSpeed);
        //change health state
        healthState = HealthStates.FALLING;
        //invoke deity died event. 
        deityMan.deityDied.Invoke();
        //remove from deity list 
        deityMan.deities.Remove(this);
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
            return new Vector3(transform.position.x, transform.position.y - 1000, transform.position.z);
        }
    }
}
