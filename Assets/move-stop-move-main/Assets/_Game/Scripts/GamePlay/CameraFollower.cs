using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : Singleton<CameraFollower>
{
    public enum State { MainMenu, Gameplay, Shop}

    [SerializeField] Transform tf;

    [Header("Rotation")]
    [SerializeField] Vector3 playerRotate;
    [SerializeField] Vector3 gamePlayRotate;

    [Header("Offset")]
    [SerializeField] Vector3 playerOffset;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 offsetMax;
    [SerializeField] Vector3 offsetMin;

    [SerializeField] Transform[] offsets;

    [SerializeField] float moveSpeed = 5f;
    Transform target;

    private Vector3 targetOffset;
    private Quaternion targetRotate;

    public Camera Camera;

    private void Awake()
    {
        target = FindObjectOfType<Player>().transform;
        Camera = Camera.main;
    }

    private void LateUpdate()
    {
        offset = Vector3.Lerp(offset, targetOffset, Time.deltaTime * moveSpeed);
        tf.rotation = Quaternion.Lerp(tf.rotation, targetRotate, Time.deltaTime * moveSpeed);
        tf.position = Vector3.Lerp(tf.position, target.position + targetOffset, Time.deltaTime * moveSpeed);
    }

    //rate
    public void SetRateOffset(float rate)
    {
        targetOffset = Vector3.Lerp(offsetMin, offsetMax, rate);
    }

    public void ChangeState(State state)
    {
        targetOffset = offsets[(int)state].localPosition;
        targetRotate = offsets[(int)state].localRotation;
        

        switch (state)
        {
            case State.MainMenu:
                targetOffset = playerOffset;
                targetRotate = Quaternion.Euler(playerRotate);
                break;

            case State.Gameplay:
                targetOffset = offsetMin;
                targetRotate = Quaternion.Euler(gamePlayRotate);
                break;

            default:
                break;
        }
    }
}
