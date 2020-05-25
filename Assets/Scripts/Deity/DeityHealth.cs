using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeityHealth : MonoBehaviour {
    DeitySound _Sounds;
    DeityAnimations _Animations;
    Deity deity;
    MeshRenderer mRender;
    FollowPilot follower;

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

    private void Awake()
    {
        _Sounds = GetComponent<DeitySound>();
        _Animations = GetComponentInParent<DeityAnimations>();
        deity = GetComponentInParent<Deity>();
        mRender = GetComponent<MeshRenderer>();
        follower = GetComponentInParent<FollowPilot>();
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
        bullet.ResetBullet();
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
        //disable follower
        follower.enabled = false;
        //fall anim
        _Animations.Animator.SetTrigger("fall");
        //find spot on ground in front of me to move towards at fall speed
        crashPoint = RaycastToGround();
        //set falling 
        deity.mover.MoveTo(crashPoint, fallSpeed);
        //change health state
        healthState = HealthStates.FALLING;
    }

    //when i hit the ground and explode
    void Crash()
    {
        mRender.material = deathMat;
        exploded.Play();
        healthState = HealthStates.CRASHED;
        //alien sound 
        _Sounds.PlayRandomSoundRandomPitch(_Sounds.deathSounds, _Sounds.myAudioSource.volume);
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
