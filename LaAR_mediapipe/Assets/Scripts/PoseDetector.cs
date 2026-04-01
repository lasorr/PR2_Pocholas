using UnityEngine;
using System.Collections.Generic;

public class PoseDetector : MonoBehaviour
{
    public enum HandPose
    {
        None,
        OpenHand,
        Fist,
        Pointing,
        Peace,
        ThumbsUp,
        Pinch
    }

    public HandPose currentPose = HandPose.None;
    public HandReceiver handReceiver;

   void Update()
{
    if (handReceiver == null || !handReceiver.handDetected)
    {
        currentPose = HandPose.None;
        // Cambia el color de un cubo para indicar que no hay mano
        GetComponent<Renderer>().material.color = Color.gray;
        return;
    }

    currentPose = DetectPose(handReceiver.landmarks);
    
    // Cambia el color según el gesto (ejemplo)
    switch (currentPose)
    {
        case HandPose.OpenHand:
            GetComponent<Renderer>().material.color = Color.green;
            break;
        case HandPose.Fist:
            GetComponent<Renderer>().material.color = Color.red;
            break;
        case HandPose.Pointing:
            GetComponent<Renderer>().material.color = Color.blue;
            break;
        default:
            GetComponent<Renderer>().material.color = Color.white;
            break;
    }
}

    HandPose DetectPose(List<Vector3> landmarks)
    {
        if (landmarks == null || landmarks.Count < 21)
            return HandPose.None;

        // Obtener las puntas de los dedos (IDs conocidos)
        Vector3 thumbTip = landmarks[4];
        Vector3 indexTip = landmarks[8];
        Vector3 middleTip = landmarks[12];
        Vector3 ringTip = landmarks[16];
        Vector3 pinkyTip = landmarks[20];

        bool indexExt = IsTipAboveBase(landmarks, 8, 6);   // índice
        bool middleExt = IsTipAboveBase(landmarks, 12, 10);
        bool ringExt = IsTipAboveBase(landmarks, 16, 14);
        bool pinkyExt = IsTipAboveBase(landmarks, 20, 18);
        bool thumbExt = IsTipAboveBase(landmarks, 4, 2);   // pulgar

        int extendedCount = (indexExt ? 1 : 0) + (middleExt ? 1 : 0) +
                            (ringExt ? 1 : 0) + (pinkyExt ? 1 : 0);

        // Detectar gestos
        if (extendedCount == 5 && thumbExt) return HandPose.OpenHand;
        if (extendedCount == 0) return HandPose.Fist;
        if (indexExt && !middleExt && !ringExt && !pinkyExt) return HandPose.Pointing;
        if (indexExt && middleExt && !ringExt && !pinkyExt) return HandPose.Peace;
        if (thumbExt && !indexExt && !middleExt && !ringExt && !pinkyExt) return HandPose.ThumbsUp;
        
        // Pinza: distancia entre pulgar e índice
        if (Vector3.Distance(thumbTip, indexTip) < 0.05f) return HandPose.Pinch;

        return HandPose.None;
    }

    bool IsTipAboveBase(List<Vector3> landmarks, int tipId, int baseId)
    {
        // Comparación simple en Y (asumiendo cámara vertical)
        return landmarks[tipId].y > landmarks[baseId].y;
    }
}