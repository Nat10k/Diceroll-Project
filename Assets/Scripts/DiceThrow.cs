using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceThrow : MonoBehaviour
{
    [SerializeField] Rigidbody dice1, dice2;
    [SerializeField] List<Vector3> faceVectors = new();
    [SerializeField] Text dice1Result, dice2Result;
    Vector3 dice1InitPos, dice2InitPos;
    float throwForce = 10;
    Coroutine diceCheck;

    private void Awake()
    {
        dice1InitPos = dice1.transform.position;
        dice2InitPos = dice2.transform.position;
        dice1.constraints = RigidbodyConstraints.FreezeAll;
        dice2.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void ThrowOneDice()
    {
        dice1.constraints = RigidbodyConstraints.None;
        dice1.AddForceAtPosition(Vector3.forward * throwForce, new Vector3(Random.Range(-2,2), Random.Range(-2, 2), Random.Range(-2, 2)), ForceMode.Impulse);
        if (diceCheck != null)
        {
            StopCoroutine(diceCheck);
            diceCheck = null;
        }
        diceCheck = StartCoroutine(DetectDiceSide(1));
    }

    public void ThrowTwoDice()
    {
        dice1.constraints = RigidbodyConstraints.None;
        dice2.constraints = RigidbodyConstraints.None;
        dice1.AddForceAtPosition(Vector3.forward * throwForce, new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2)), ForceMode.Impulse);
        dice2.AddForceAtPosition(Vector3.forward * throwForce, new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2,2)), ForceMode.Impulse);
        if (diceCheck != null)
        {
            StopCoroutine(diceCheck);
            diceCheck = null;
        }
        diceCheck = StartCoroutine(DetectDiceSide(2));
    }

    public void ResetDice()
    {
        dice1.transform.position = dice1InitPos;
        dice2.transform.position = dice2InitPos;

        dice1.transform.rotation = Quaternion.identity;
        dice2.transform.rotation = Quaternion.identity;

        dice1.constraints = RigidbodyConstraints.FreezeAll;
        dice2.constraints = RigidbodyConstraints.FreezeAll;
    }

    private int GetDiceNumber(Rigidbody dice)
    {
        int topFaceIndex = -1;
        float maxDot = -1;
        for (int i=0; i<faceVectors.Count; i++)
        {
            if (Vector3.Dot(dice.transform.TransformDirection(faceVectors[i]), Vector3.up) > maxDot)
            {
                topFaceIndex = i;
                maxDot = Vector3.Dot(dice.transform.TransformDirection(faceVectors[i]), Vector3.up);
            }
        }
        return topFaceIndex + 1;
    }

    private IEnumerator DetectDiceSide(int throwCount)
    {
        yield return new WaitForFixedUpdate();
        while (dice1.velocity != Vector3.zero || dice2.velocity != Vector3.zero || dice1.angularVelocity != Vector3.zero ||
            dice2.angularVelocity != Vector3.zero)
        {
            yield return null;
        }
        Debug.Log(dice1.transform.up);
        dice1Result.text = "Dice 1 : " + GetDiceNumber(dice1).ToString();
        if (throwCount == 2)
        {
            dice2Result.text = "Dice 2 : " + GetDiceNumber(dice2).ToString();
        } 
    }
}
