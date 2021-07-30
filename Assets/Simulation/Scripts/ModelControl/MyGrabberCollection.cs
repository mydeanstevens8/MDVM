using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDVM
{
    [DisallowMultipleComponent]
    public class MyGrabberCollection : MonoBehaviour
    {
        private MyGrabber leftGrabber = null;
        private MyGrabber rightGrabber = null;

        private static MyGrabberCollection instance = null;
        private void Awake()
        {
            instance = this;
        }

        public void RegisterLeft(MyGrabber grabber)
        {
            leftGrabber = grabber;
        }

        public void RegisterRight(MyGrabber grabber)
        {
            rightGrabber = grabber;
        }

        public bool AttemptLeftGrab(GameObject grabbed)
        {
            if(leftGrabber != null)
            {
                leftGrabber.BeginGrab(grabbed);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AttemptRightGrab(GameObject grabbed)
        {
            if (rightGrabber != null)
            {
                rightGrabber.BeginGrab(grabbed);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AttemptGrab(GameObject grabbed)
        {
            if (UI.SwitchPointerControl.Get().IsLeftPointerActive())
            {
                return AttemptLeftGrab(grabbed);
            }
            else
            {
                return AttemptRightGrab(grabbed);
            }
        }

        public void EndAllGrabs()
        {
            if (leftGrabber != null)
            {
                leftGrabber.EndGrab();
            }
            if (rightGrabber != null)
            {
                rightGrabber.EndGrab();
            }
        }

        public static MyGrabberCollection Get()
        {
            return instance;
        }
    }
}

