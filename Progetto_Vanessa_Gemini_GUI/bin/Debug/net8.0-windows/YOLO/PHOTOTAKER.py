import os
os.add_dll_directory("C:/Program Files/NVIDIA GPU Computing Toolkit/CUDA/v11.8/bin")
import cv2
import dlib

def PersonTOFACEIMAGE(IMAGE,name,i):
    path="FACES/FORTRAINING/"

    # Initialize the Dlib HOG face detector
    detector = dlib.get_frontal_face_detector()



    perc=0
    img=IMAGE
    height, width = img.shape[:2]
    resize_ratio = 0.4
    new_width = int(width * resize_ratio)
    new_height = int(height * resize_ratio)
    img = cv2.resize(img, (new_width, new_height), interpolation=cv2.INTER_AREA)

    # Convert the image to RGB format (Dlib works better with RGB)


    # Detect faces

    detections = detector(img, 1)

    for detection in detections:
        x1, y1, x2, y2 = detection.left(), detection.top(), detection.right(), detection.bottom()
        salvare=img[int(y1-y1*perc*5):int(y2+y2*perc), int(x1-x1*perc):int(x2+x2*perc)]
        cv2.imshow("Il coglio in addestramento",salvare)
        cv2.imwrite(path+f"{name}_{str(i)}.png", salvare)
        cv2.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 2)  # Draw green rectangle
        label = ""
        cv2.putText(img, label, (x1, y1 - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 0, 255), 2)  # White text with black shadow

    cv2.destroyAllWindows()


