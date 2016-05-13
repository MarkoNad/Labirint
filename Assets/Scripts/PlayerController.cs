using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class PlayerController : MonoBehaviour {
    public float walkSpeed;
    public GameObject bodyManager;

    private const float ROTATION_SMOOTHING_FACTOR = 0.01f;
    private float DEFAULT_WALK_SPEED = 1.5f;
    private float MINIMAL_WALKING_SPEED = 1.5f;

    private Animator animator;
    private float currentSpeed;
    private bool walkingDisabled;

    private BodySourceManager source;
    private Body[] data = null;
    private Body trackedBody = null;



    void Start() {
        if (bodyManager == null) {
            print("body source manager missing");
            return;
        }

        source = bodyManager.GetComponent<BodySourceManager>();

        if (source == null) {
            print("invalid body manager");
            return;
        }

        animator = GetComponent<Animator>();
        if (walkSpeed <= 0) walkSpeed = DEFAULT_WALK_SPEED;
        currentSpeed = 0;
        walkingDisabled = true;
    }



    void Update() {
        manageKinectData();
        walk();
        rotate();
    }



    private void manageKinectData() {
        if (data == null) {
            data = source.GetData();
        }

        int idx = -1;
        for (int i = 0; i < source.getBodyCount(); i++) {
            if (data == null) return;
            if (data[i].IsTracked) {
                idx = i;
            }
        }

        if (idx > -1) {
            trackedBody = data[idx];
        }
    }



    private void walk() {
        if (trackedBody == null) return;

        bool wallInFront = transform.Find("colliderDetectors").Find("wallDetector").GetComponent<WallDetector>().getWallInFront();

        if(wallInFront || walkingDisabled) {
            currentSpeed = 0;
        }

        if (trackedBody.HandRightState != HandState.Closed && !wallInFront && !walkingDisabled) {
            float speedMultiplier = (trackedBody.Joints[JointType.HandRight].Position.Y + 0.75f) * 4;
            currentSpeed = walkSpeed * speedMultiplier < MINIMAL_WALKING_SPEED ? 0 : walkSpeed * speedMultiplier;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
        animator.SetFloat("Speed", currentSpeed);
    }



    public void setWalkingDisabled(bool disabled) {
        walkingDisabled = disabled;
    }



    private void rotate() {
        if (trackedBody == null) return;

        if (trackedBody.HandLeftState != HandState.Closed && !walkingDisabled) {
            float angleY = (float)(trackedBody.Joints[JointType.HandLeft].Position.X);    
            Quaternion target = Quaternion.Euler(0, gameObject.transform.rotation.y + angleY * 2000, 0);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, target, Time.time * ROTATION_SMOOTHING_FACTOR);
        }
    }


}
