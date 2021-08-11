using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class UISoundSystem : MonoBehaviour
    {
        public AudioClip pointerPress = null;

        public AudioClip grabbableGrabBegin = null;
        public AudioClip grabbableGrabEnd = null;

        public AudioClip actionMenuActivate = null;
        public AudioClip actionMenuDeactivate = null;
        public AudioClip actionMenuComplete = null;
        public AudioClip actionMenuBrowse = null;
        public AudioClip actionMenuReturn = null;

        public AudioClip elevatorUp = null;
        public AudioClip elevatorDown = null;
        public AudioClip elevatorComplete = null;
        public AudioClip elevatorBegin = null;

        private static UISoundSystem instance = null;
        private AudioSource source = null;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            source = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip)
        {
            if (clip != null)
            {
                source.PlayOneShot(clip);
            }
        }

        public void Play(AudioClip clip, float volume)
        {
            if(clip != null)
            {
                source.PlayOneShot(clip, volume);
            }
        }

        public static UISoundSystem Get() 
        {
            return instance;
        }

        public static void PlayS(AudioClip clip)
        {
            Get().Play(clip);
        }

        public static void PlayS(AudioClip clip, float volume)
        {
            Get().Play(clip, volume);
        }
    }
}

