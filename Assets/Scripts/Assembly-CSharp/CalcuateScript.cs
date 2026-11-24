using UnityEngine;
using TMPro;

public class CalcuateScript : MonoBehaviour
{

    public TMP_Text Answer;

    public TMP_InputField Num1Text;

    public TMP_InputField Num2Text;

    public TMP_Dropdown Symbol;

    public bool DivideByZero;

    private float solution;

    private float num1;

    private float num2;

    private void Start()
    {
    }

    private void Update()
    {
        if (Num1Text.text != string.Empty)
        {
            num1 = float.Parse(Num1Text.text);
        }
        if (Num2Text.text != string.Empty)
        {
            num2 = float.Parse(Num2Text.text);
        }
        if (Symbol.options[Symbol.value].text == "+ Plus")
        {
            solution = num1 + num2;
            DivideByZero = false;
        }
        else if (Symbol.options[Symbol.value].text == "- Minus")
        {
            solution = num1 - num2;
            DivideByZero = false;
        }
        else if (Symbol.options[Symbol.value].text == "x Times")
        {
            solution = num1 * num2;
            DivideByZero = false;
        }
        else if (Symbol.options[Symbol.value].text == "÷ Divided By")
        {
            if (num2 == 0f)
            {
                DivideByZero = true;
            }
            else
            {
                DivideByZero = false;
                solution = num1 / num2;
            }
        }
    }

    public void CalcuateAnswer()
    {
        if (DivideByZero)
        {
            Answer.text = "Cannot Divide by 0";
        }
        else
        {
            Answer.text = solution.ToString();
        }
    }
}
