# PR2_Pocholas
Aquest projecte, és la entrega de la pràctica 2 de l'asignatura de Realitat Virtual i Augmentada. És una aplicació de mobil AR amb el nom de LaAR, que fa servir el tracking de mans de unity en AR Foundation per permetre al usuari tocar diferents sons amb les mans. 

# Hand Tracker - Quick Setup

## 1. Install Python 3.10.11
Download: https://www.python.org/downloads/release/python-31011/

## 2. Check version
py --version

## 3. Install packages
py -m pip install mediapipe opencv-python numpy

## 4. Create hand_tracker.py

## 5. Fix MediaPipe (if needed)
py -m pip install --upgrade pip
py -m pip uninstall mediapipe
py -m pip install mediapipe

## 6. Test MediaPipe
py
>>> import mediapipe as mp
>>> print(dir(mp))
>>> exit()

## 7. Run script
py hand_tracker.py

## 8. Run from Desktop
cd Desktop
py hand_tracker.py

## Troubleshooting
pip install --upgrade opencv-python mediapipe numpy

## Distribució projecte

📁Assets

-> 📁 Audios

-> 📁 Scenes

-> 📁 Scripts : AudioManager, HandReceiver, PoseDetector.

-> 📁 Settings

-> 📁 XR

( +others)

Dintre de la carpeta de projecte també hi ha un script de Phyton el qual farem servir paral·lelament per poder provar els scripts amb l'ordinador.
