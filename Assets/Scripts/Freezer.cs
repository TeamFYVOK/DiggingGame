using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace eXTRIVAL
{

// オブジェクト
public class Freezer : MonoBehaviour
{
    float freezeTime_;
    float freezeDelayTime_;
    bool isDoFreeze_ = false;

	// 停止中か?
    public bool isFreezing { get; private set; } 

    void FixedUpdate ()
    {
        if (isDoFreeze_ == true) {
            isDoFreeze_ = false;
            _BeginFreeze ();
        }
        // ウェイト処理
        if (freezeDelayTime_ > 0) {
            freezeDelayTime_ = Mathf.Max (freezeDelayTime_ - Time.deltaTime, 0);
            if (freezeDelayTime_ == 0) {
                isDoFreeze_ = true;
            }
        }
        else {
            if (freezeTime_ > 0) {
                freezeTime_ = Mathf.Max (freezeTime_ - Time.deltaTime, 0);
                if (freezeTime_ == 0) {
                    _EndFreeze ();
                }
            }
        }
            
	}

        
    /**
     * フリーズウェイトの開始
     *
     * @param time 停止フレーム
     * @param delay 停止開始するまでのフレーム
     */
    public void Freeze (float t, float delay = 0)
    {
        freezeTime_ = t > freezeTime_ ? t : freezeTime_;
        freezeDelayTime_ = delay > freezeDelayTime_ ? delay : freezeDelayTime_;
        if (freezeDelayTime_ == 0) {
            isDoFreeze_ = true;
        }
    }
        
    /**
     * フリーズ時間の再設定
     */
    public void ResetFreeze (float t, float delay)
    {
        freezeTime_ = t;
        freezeDelayTime_ = delay;
        if (freezeDelayTime_ == 0)
            isDoFreeze_ = true;
    }
        
    /// freeze
    Dictionary<Animation,float> animationPauseList_ = new Dictionary<Animation,float> ();
    Vector3 freezeVelocity_;
    RigidbodyConstraints freezeConstraints_;
        
    // フリーズ開始
    private void _BeginFreeze ()
    {
        if (isFreezing) {
            _EndFreeze ();
        }
            
        animationPauseList_.Clear ();
        Animation[] animations = GetComponentsInChildren<Animation> ();
        foreach (Animation animation in animations) {
            if (animation.clip == null)
                continue;
            animationPauseList_.Add (animation, animation [animation.clip.name].speed);
            animation [animation.clip.name].speed = 0;
        }
            
        ParticleEmitter[] emitters = GetComponentsInChildren<ParticleEmitter> ();
        foreach (ParticleEmitter emmitter in emitters) {
            emmitter.enabled = false;
        }
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem> ();
        foreach (ParticleSystem particle in particles) {
            particle.Pause ();
        }
            
        if (GetComponent<Rigidbody>() != null) {
            freezeVelocity_ = GetComponent<Rigidbody>().velocity;
            freezeConstraints_ = GetComponent<Rigidbody>().constraints;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    
        isFreezing = true;
        
        gameObject.BroadcastMessage ("OnFreeze", SendMessageOptions.DontRequireReceiver);
    }
        
    // フリーズ終了
    private void _EndFreeze ()
    {
        foreach (Animation animation in animationPauseList_.Keys) {
            if (animation == null)
                continue;
            animation [animation.clip.name].speed = animationPauseList_ [animation];
        }
        animationPauseList_.Clear ();
            
        ParticleEmitter[] emitters = GetComponentsInChildren<ParticleEmitter> ();
        foreach (ParticleEmitter emmitter in emitters) {
            emmitter.enabled = true;
        }
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem> ();
        foreach (ParticleSystem particle in particles) {
            particle.Play ();
        }

        if (GetComponent<Rigidbody>() != null) {
            GetComponent<Rigidbody>().constraints = freezeConstraints_;
            if (freezeVelocity_ != Vector3.zero ) {
                GetComponent<Rigidbody>().velocity = freezeVelocity_;           
            }
        }
		gameObject.BroadcastMessage ("OnUnfreeze", SendMessageOptions.DontRequireReceiver);
        isFreezing = false;        
    }
}

}
