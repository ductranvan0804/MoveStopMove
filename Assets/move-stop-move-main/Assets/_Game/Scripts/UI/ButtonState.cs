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

        // (int)state: Ép ki?u enum ? int ?? dùng làm ch? s? m?ng
        buttonState[(int)state].SetActive(true);
    }
}
