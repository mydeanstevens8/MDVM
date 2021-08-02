namespace MDVM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MyGrabbable : MonoBehaviour
    {
        public enum GrabLockState
        {
            Free,
            Restricted,
            Locked
        }

        [System.Serializable]
        public struct GrabLockSettings
        {
            [Header("X")]
            [Tooltip("The state of the lock. Free means unlocked, restricted means restricted to these bounds below. Locked means completely locked.")]
            public GrabLockState lockX;
            [Tooltip("The minimum X value in local space.")]
            public float minX;
            [Tooltip("The maximum X value in local space.")]
            public float maxX;


            [Header("Y")]
            [Tooltip("The state of the lock. Free means unlocked, restricted means restricted to these bounds below. Locked means completely locked.")]
            public GrabLockState lockY;
            [Tooltip("The minimum Y value in local space.")]
            public float minY;
            [Tooltip("The maximum Y value in local space.")]
            public float maxY;


            [Header("Z")]
            [Tooltip("The state of the lock. Free means unlocked, restricted means restricted to these bounds below. Locked means completely locked.")]
            public GrabLockState lockZ;
            [Tooltip("The minimum Z value in local space.")]
            public float minZ;
            [Tooltip("The maximum Z value in local space.")]
            public float maxZ;



            public GrabLockSettings(GrabLockState lockX, GrabLockState lockY, GrabLockState lockZ) : this()
            {
                this.lockX = lockX;
                this.lockY = lockY;
                this.lockZ = lockZ;
            }
        }

        public enum ControlMode
        {
            [Tooltip("Move the object and rotate towards the grabber while moving.")]
            MoveAndLookAt,
            [Tooltip("Move the object without rotating.")]
            Move,
            [Tooltip("Rotate the object without moving.")]
            Rotate,
            [Tooltip("Lock the object in place.")]
            Locked
        }

        [Header("Base Config")]
        [Tooltip("The control mode of this grabbable. Controls whether or not the object will be moved, rotated or otherwise by the grabber.")]
        public ControlMode mode = ControlMode.MoveAndLookAt;

        [Tooltip("Sets how fast will the object rotate in Rotate mode.")]
        public float rotationSpeed = 1.0f;

        [Tooltip("The object that will be grabbed by the grabber when interacting with this object. If set to None, defaults to this object itself.")]
        public GameObject objectToApplyGrab = null;

        [Tooltip("Whether or not to lock the object to a specific local position axis while dragging.")]
        public GrabLockSettings lockPosition = new GrabLockSettings();

        [Tooltip("Whether or not to lock the object to a specific local rotation axis while dragging.")]
        public GrabLockSettings lockRotation = new GrabLockSettings();

        protected Vector3 objectToApplyGrabRelativePosition = Vector3.zero;

        // Used for restricting the position of elements.
        protected Vector3 initialPosition = Vector3.zero;

        // Used for restricting the rotation of elements.
        protected Quaternion initialRotation = Quaternion.identity;

        // Used when moving outside the bounds set in restricted mode.
        // Does not apply in locked mode for a particular axis.
        protected bool vRestrictionBlockedX = false;
        protected bool vRestrictionBlockedY = false;
        protected bool vRestrictionBlockedZ = false;

        protected bool isBeingGrabbed = false;

        void Awake()
        {
            if(objectToApplyGrab == null)
            {
                objectToApplyGrab = gameObject;
            }
        }

        private void Start()
        {
            initialPosition = objectToApplyGrab.transform.localPosition;
            initialRotation = objectToApplyGrab.transform.localRotation;

            // Bound on startup
            BoundGrabbedObjectPosition();
        }

        public void DoGrab()
        {
            MyGrabberCollection.Get().AttemptGrab(gameObject);
        }

        public void StopGrab()
        {
            MyGrabberCollection.Get().EndAllGrabs();
        }

        public virtual void GrabbableStart(MyGrabber grabber, Transform grabbingCenter, Vector3 relativeDisplacement, Quaternion relativeRotation)
        {
            isBeingGrabbed = true;

            // Vector from us to the grabbed object. No need to worry about rotation settings.
            objectToApplyGrabRelativePosition = grabbingCenter.transform.InverseTransformVector(objectToApplyGrab.transform.position - transform.position);
        }

        public virtual void GrabbableUpdate(MyGrabber grabber, Transform grabbingCenter, Vector3 relativeDisplacement, Quaternion relativeRotation)
        {
            switch(mode)
            {
                case ControlMode.MoveAndLookAt:
                    DoMove(grabber, grabbingCenter, relativeDisplacement, relativeRotation);
                    DoLookAtRotate(grabber, grabbingCenter, relativeDisplacement, relativeRotation);
                    break;
                case ControlMode.Move:
                    DoMove(grabber, grabbingCenter, relativeDisplacement, relativeRotation);
                    break;
                case ControlMode.Rotate:
                    DoRotate(grabber, grabbingCenter, relativeDisplacement, relativeRotation);
                    break;
                default:
                    break;
            }
        }
        public virtual void GrabbableEnd(MyGrabber grabber, Transform grabbingCenter, Vector3 relativeDisplacement, Quaternion relativeRotation)
        {
            isBeingGrabbed = false;
        }

        protected virtual void DoMove(MyGrabber grabber, Transform grabbingCenter, Vector3 relativeDisplacement, Quaternion relativeRotation)
        {
            Vector3 newPosition = grabbingCenter.transform.TransformPoint(relativeDisplacement) + grabbingCenter.transform.TransformVector(objectToApplyGrabRelativePosition);
            objectToApplyGrab.transform.position = newPosition;

            BoundGrabbedObjectPosition();
        }

        protected virtual void DoLookAtRotate(MyGrabber grabber, Transform grabbingCenter, Vector3 relativeDisplacement, Quaternion relativeRotation)
        {
            Quaternion newRotation = grabbingCenter.transform.rotation * grabber.GrabbedObjectRotation;
            objectToApplyGrab.transform.rotation = newRotation;
        }

        protected virtual void DoRotate(MyGrabber grabber, Transform grabbingCenter, Vector3 relativeDisplacement, Quaternion relativeRotation)
        {
            // Calculate our initial displacement
            Vector3 displacementFromInitial = 
                grabbingCenter.transform.TransformPoint(relativeDisplacement) 
                + grabbingCenter.transform.TransformVector(objectToApplyGrabRelativePosition);

            Vector3 sdfi = rotationSpeed * displacementFromInitial;

            Quaternion asRotation = Quaternion.Euler(-sdfi.y, sdfi.x, sdfi.z);

            objectToApplyGrab.transform.localRotation = asRotation;
        }


        protected virtual void BoundGrabbedObjectPosition()
        {
            // Position bound at the initial starting position.
            Vector3 boundCenter = initialPosition;

            // Set to lock position info when using the restricted state.
            if (lockPosition.lockX == GrabLockState.Restricted) boundCenter.x = (lockPosition.minX + lockPosition.maxX) / 2;
            if (lockPosition.lockY == GrabLockState.Restricted) boundCenter.y = (lockPosition.minY + lockPosition.maxY) / 2;
            if (lockPosition.lockZ == GrabLockState.Restricted) boundCenter.z = (lockPosition.minZ + lockPosition.maxZ) / 2;

            // In the case of restricted axes, these size settings will apply.
            Vector3 boundSize = new Vector3(
                lockPosition.maxX - lockPosition.minX,
                lockPosition.maxY - lockPosition.minY,
                lockPosition.maxZ - lockPosition.minZ
                );

            // Lock each axis to zero size if that axis is locked.
            if (lockPosition.lockX == GrabLockState.Locked) boundSize.x = 0;
            if (lockPosition.lockY == GrabLockState.Locked) boundSize.y = 0;
            if (lockPosition.lockZ == GrabLockState.Locked) boundSize.z = 0;

            // When free, set to infinity.
            if (lockPosition.lockX == GrabLockState.Free) boundSize.x = Mathf.Infinity;
            if (lockPosition.lockY == GrabLockState.Free) boundSize.y = Mathf.Infinity;
            if (lockPosition.lockZ == GrabLockState.Free) boundSize.z = Mathf.Infinity;

            Bounds b = new Bounds(boundCenter, boundSize);

            Vector3 currentLocalPosition = objectToApplyGrab.transform.localPosition;

            // Restrict the object to inside the specified bounds.
            Vector3 correctedLocalPosition = b.ClosestPoint(currentLocalPosition);

            // Check restricted axis locks
            if (lockPosition.lockX == GrabLockState.Restricted)
            {
                vRestrictionBlockedX = (currentLocalPosition.x - correctedLocalPosition.x) != 0;
            }
            if (lockPosition.lockY == GrabLockState.Restricted)
            {
                vRestrictionBlockedY = (currentLocalPosition.y - correctedLocalPosition.y) != 0;
            }
            if (lockPosition.lockZ == GrabLockState.Restricted)
            {
                vRestrictionBlockedZ = (currentLocalPosition.z - correctedLocalPosition.z) != 0;
            }


            objectToApplyGrab.transform.localPosition = correctedLocalPosition;
        }

        public bool RestrictionBlocked 
        { 
            get {
                return vRestrictionBlockedX || vRestrictionBlockedY || vRestrictionBlockedZ;
            } 
        }

        public bool RestrictionBlockedX
        {
            get
            {
                return vRestrictionBlockedX;
            }
        }

        public bool RestrictionBlockedY
        {
            get
            {
                return vRestrictionBlockedY;
            }
        }

        public bool RestrictionBlockedZ
        {
            get
            {
                return vRestrictionBlockedZ;
            }
        }

        public bool BeingGrabbed
        {
            get
            {
                return isBeingGrabbed;
            }
        }
    }

}