using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public List<Card> heldCards = new List<Card>();
    public List<Vector3> cardPositions = new List<Vector3>();
    public Transform minPos, maxPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCardPositionInHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetCardPositionInHand()
    {
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;
        if(heldCards.Count > 1) 
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        for(int i = 0; i < heldCards.Count; i++)
        {
            cardPositions.Add(minPos.position + (distanceBetweenPoints * i));

            //heldCards[i].transform.position = cardPositions[i];
            //heldCards[i].transform.rotation = minPos.rotation;

            //This will set where the card should move to
            heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);

            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }     
    }
}
//   //heldCards[i].transform.position = cardPositions[i]; -> lo ha comentado de ehcho al final del vídeo quita la rotation también supongo porque ahora funciona con el lerp para la animacion
// preguntar de cuando abre el paint que significa que  al incio e smas grande la sepración pero luego ams pequeña, no entiendo lerp
//  Quaternion no entiendo anda croe que e sla para la rotaion y el tio en el instpector unity le deja ver los tipos pero  a mi no me deja

// Vídeo Raising cards preguntar como sabe el tamaño del collider tan exacto
//         _handController = FindObjectOfType<HandController>();  MoveToPoint(_handController.cardPositions[handPosition] + new Vector3(0, 1f, .5f), Quaternion.identity); y la declaración  del controlador no la entiendo para que srve esto

