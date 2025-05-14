import TelegramVisualizer as tv
import os
import FACENCODER
#Write your bin folder if it necessary. This is mine however yours would be easily different
os.add_dll_directory("C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8/bin")
from ultralytics import YOLO
import PHOTOTAKER
import cv2
import threading
import win32pipe, win32file, pywintypes
def empty_directory(directory_path):
    for root, _, files in os.walk(directory_path):
        for file in files:
            # Construct the file path
            file_path = os.path.join(root, file)

            # Check if it's a file (not a directory)
            if os.path.isfile(file_path):
                # Remove the file
                os.remove(file_path)


telegram= threading.Thread(target=tv.TelegramVisualizer, )
telegram.start()

if True:

    acceso=True

    model = YOLO("yolov8s.pt",)

    CAP=cv2.VideoCapture(0)

    lista_oggetti=[]
    lista_oggetti_real=[]
    DetectionREQUEST=False

    pipe = win32file.CreateFile(
            r'\\.\pipe\VISUALIZER',
            win32file.GENERIC_READ | win32file.GENERIC_WRITE,
            0,
            None,
            win32file.OPEN_EXISTING,
            0,
            None
    )

    frame=""
    nameFACE=""
    caricata=""
    perc=0.15
    print("Telecamera online")
    while True:
        caricata = ""
        try:
            response = win32file.ReadFile(pipe, 1024)
            response=str(response[1])
            response=response[2:-5]
            if "Cosa vedi?" in response:
                DetectionREQUEST=True
            else:
                DetectionREQUEST = False
        except Exception as e:
            print(f"Errore durante la lettura dalla pipe: {e}")
            break
        empty_directory("FACES/FORTRAINING")
        print("INIZIO ACQUISIZIONE")
        face_database = FACENCODER.open_face_database()
        for i in range(10):
            # Cattura il frame corrente
            ret, frame = CAP.read()
            #print(frame.shape)

            lista_oggetti_real = lista_oggetti
            lista_oggetti.clear()
            if not ret:
                print("Errore durante la lettura del frame")
                break
            results = model.predict(source=frame, show=False,verbose=False,imgsz=int(1280*0.5))
            tmpstringa=""
            for detection in results:
                for box in detection.boxes:
                    x1, y1, x2, y2 = box.xyxy[0]
                    label = detection.names[int(box.cls[0])]
                    score = box.conf[0]
                    if score>0.75:
                        #print(label)
                        if "person" in label:

                            nameFACE=FACENCODER.recognize(frame[int(y1-y1*perc*5):int(y2+y2*perc), int(x1-x1*perc):int(x2+x2*perc)],face_database)

                            if DetectionREQUEST==False:
                                crop_x1 = min(int(x1), int(x2))  # Get the smaller x-coordinate
                                crop_y1 = min(int(y1), int(y2))  # Get the smaller y-coordinate
                                crop_x2 = max(int(x1), int(x2))  # Get the larger x-coordinate
                                crop_y2 = max(int(y1), int(y2))  # Get the larger y-coordinate

                                # Crop the frame using the window
                                cropped_image = frame[crop_y1:crop_y2, crop_x1:crop_x2]
                                PHOTOTAKER.PersonTOFACEIMAGE(cropped_image, response,i)


                        if DetectionREQUEST:
                            # Disegna il bounding box e l'etichetta sul frame
                            lista_oggetti.append(f"{label}: {score:.2f}% accuratezza")
                            cv2.rectangle(frame, (int(x1), int(y1)), (int(x2), int(y2)), color=(0, 255, 0), thickness=2)

                            if "person" in label:
                                cv2.putText(frame, f"{label} NOME':{nameFACE} ({score:.2f}%)", (int(x1), int(y1) - 5), cv2.FONT_HERSHEY_SIMPLEX, 0.5,
                                        (0, 255, 0), 2)
                                tmpstringa = tmpstringa  + f"{label} NOME:{nameFACE} ({score:.2f}%)"+ ";"
                            else:
                                cv2.putText(frame, f"{label}: {score:.2f}", (int(x1), int(y1) - 5), cv2.FONT_HERSHEY_SIMPLEX, 0.5,
                                                (0, 255, 0), 2)
                                tmpstringa = tmpstringa + f"{label}: {score:.2f}% accuratezza"+ ";"

                cv2.waitKey(20)


                print(f" MOMENTO: {i}")
                caricata= caricata + tmpstringa + f" MOMENTO: {i}\n"

        if not DetectionREQUEST:
            FACENCODER.add_face_encoding('FACES/FORTRAINING')

        try:
            cv2.imwrite("IMMAGINEPRESAVISIONE.png", frame)
            win32file.WriteFile(pipe, tmpstringa.encode("utf-8") + b"\n")
        except Exception as e:
            print(f"Errore durante la scrittura sulla pipe: {e}")




