using UnityEngine;
using System.Collections.Generic;

public class PoseDetector : MonoBehaviour
{
    public enum HandPose
    {
        None,
        OpenHand,      // 1. Mano abierta
        Fist,          // 2. Puño
        Pointing,      // 3. Señalando (solo índice)
        Peace,         // 4. Paz/Victoria (índice + medio)
        ThumbsUp,      // 5. Pulgar arriba
        Pinch,         // 6. Pellizco
        HornSign       // 7. Cuernos (índice + meñique)
    }

    public HandPose currentPose = HandPose.None;
    public HandReceiver handReceiver;

    // Umbral para detectar dedos doblados/extendidos
    public float extensionThreshold = 0.02f;

    void Update()
    {
        if (handReceiver == null || !handReceiver.handDetected || handReceiver.landmarks.Count < 21)
        {
            currentPose = HandPose.None;
            return;
        }

        currentPose = DetectPose(handReceiver.landmarks);

        // Debug cada 60 frames para ver qué pasa
        if (Time.frameCount % 60 == 0 && currentPose != HandPose.None)
        {
            Debug.Log($"Pose detectada: {currentPose}");
        }
    }

    HandPose DetectPose(List<Vector3> landmarks)
    {
        // Obtener estado de cada dedo (extendido o no)
        bool thumbExtended = IsThumbExtended(landmarks);
        bool indexExtended = IsFingerExtended(landmarks, 8, 6, 5);
        bool middleExtended = IsFingerExtended(landmarks, 12, 10, 9);
        bool ringExtended = IsFingerExtended(landmarks, 16, 14, 13);
        bool pinkyExtended = IsFingerExtended(landmarks, 20, 18, 17);

        // Contar dedos extendidos (excluyendo pulgar)
        int extendedCount = (indexExtended ? 1 : 0) + (middleExtended ? 1 : 0) +
                            (ringExtended ? 1 : 0) + (pinkyExtended ? 1 : 0);

        // DEBUG: Ver estado de dedos
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Dedos - I:{indexExtended} M:{middleExtended} R:{ringExtended} P:{pinkyExtended} T:{thumbExtended}");
        }

        // 1. DETECTAR PINCH (pellizco) - PRIORIDAD ALTA
        float thumbIndexDistance = Vector3.Distance(landmarks[4], landmarks[8]);
        if (thumbIndexDistance < 0.05f)
        {
            return HandPose.Pinch;
        }

        // 2. DETECTAR HORN SIGN (cuernos) - índice + meñique extendidos
        if (indexExtended && pinkyExtended && !middleExtended && !ringExtended)
        {
            return HandPose.HornSign;
        }

        // 3. DETECTAR PEACE (paz) - índice + medio extendidos
        if (indexExtended && middleExtended && !ringExtended && !pinkyExtended && !thumbExtended)
        {
            return HandPose.Peace;
        }

        // 4. DETECTAR POINTING (señalando) - solo índice extendido
        if (indexExtended && !middleExtended && !ringExtended && !pinkyExtended && !thumbExtended)
        {
            return HandPose.Pointing;
        }

        // 5. DETECTAR THUMBS UP - solo pulgar extendido
        if (thumbExtended && !indexExtended && !middleExtended && !ringExtended && !pinkyExtended)
        {
            return HandPose.ThumbsUp;
        }

        // 6. DETECTAR OPEN HAND - todos los dedos extendidos
        if (extendedCount == 4 && thumbExtended)
        {
            return HandPose.OpenHand;
        }

        // 7. DETECTAR FIST - ningún dedo extendido
        if (extendedCount == 0 && !thumbExtended)
        {
            return HandPose.Fist;
        }

        return HandPose.None;
    }

    // Detectar si un dedo está extendido (para índice, medio, anular, meñique)
    bool IsFingerExtended(List<Vector3> landmarks, int tipId, int pipId, int mcpId)
    {
        // La punta debe estar más arriba (Y menor) que la articulación PIP
        // Y la articulación PIP debe estar más arriba que MCP
        return landmarks[tipId].y < landmarks[pipId].y &&
               landmarks[pipId].y < landmarks[mcpId].y;
    }

    // Detectar si el pulgar está extendido (funciona diferente)
    bool IsThumbExtended(List<Vector3> landmarks)
    {
        // Para pulgar: comparar X (izquierda/derecha) porque se mueve horizontalmente
        // En mano derecha: punta (4) debe estar más a la derecha que la base (2)
        // En mano izquierda: sería al revés, pero asumimos mano derecha
        return landmarks[4].x > landmarks[2].x;
    }

    // Método público para obtener pose actual
    public HandPose GetCurrentPose()
    {
        return currentPose;
    }
}