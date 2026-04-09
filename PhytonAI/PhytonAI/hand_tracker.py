import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import socket
import json
import os  # <-- añadido

# Configuración de MediaPipe Hand Landmarker
model_path = "hand_landmarker.task"
if not os.path.exists(model_path):
    print("Descargando modelo...")
    import urllib.request
    url = "https://storage.googleapis.com/mediapipe-models/hand_landmarker/hand_landmarker/float16/1/hand_landmarker.task"
    urllib.request.urlretrieve(url, model_path)

base_options = python.BaseOptions(model_asset_path=model_path)
options = vision.HandLandmarkerOptions(base_options=base_options, num_hands=1)
detector = vision.HandLandmarker.create_from_options(options)

# UDP config
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
UNITY_IP = "127.0.0.1"
UNITY_PORT = 5052

cap = cv2.VideoCapture(0)

while cap.isOpened():
    success, frame = cap.read()
    if not success:
        continue

    frame = cv2.flip(frame, 1)
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb)
    result = detector.detect(mp_image)

    hand_data = {"detected": False, "landmarks": []}

    if result.hand_landmarks:
        hand_data["detected"] = True
        for hand in result.hand_landmarks:
            for i, lm in enumerate(hand):
                hand_data["landmarks"].append({
                    "id": i,
                    "x": lm.x,
                    "y": lm.y,
                    "z": lm.z
                })
                # dibujar puntos
                h, w, _ = frame.shape
                cx, cy = int(lm.x * w), int(lm.y * h)
                cv2.circle(frame, (cx, cy), 5, (0, 255, 0), -1)

    sock.sendto(json.dumps(hand_data).encode(), (UNITY_IP, UNITY_PORT))
    cv2.imshow('Hand Tracking', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()