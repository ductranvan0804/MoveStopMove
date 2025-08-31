using UnityEngine;

public class ButtonState : MonoBehaviour
{
    public enum State { Buy, Equip, Equipped }

    [SerializeField] GameObject[] buttonState;

    public void SetState(State state)
    {
        for (int i = 0; i < buttonState.Length; i++)
        {
            buttonState[i].SetActive(false);
        }

        // (int)state: �p ki?u enum ? int ?? d�ng l�m ch? s? m?ng
        buttonState[(int)state].SetActive(true);
    }
}
