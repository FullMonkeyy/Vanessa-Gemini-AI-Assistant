import os
import face_recognition
import cv2
import pickle
import numpy as np

def recognize_faces(directory):
    """
    Recognize faces in a directory and return a dictionary of face encodings.
    """
    faces = {}
    for filename in os.listdir(directory):
        filepath = os.path.join(directory, filename)
        if filename.endswith(('.jpg', '.jpeg', '.png')):
            image = face_recognition.load_image_file(filepath)
            face_encodings = face_recognition.face_encodings(image)
            if face_encodings:
                face_encoding = face_encodings[0]
                name = os.path.splitext(filename)[0]
                faces[name] = face_encoding
    return faces

def open_face_database():
    """
    Open the face database from a pickle file.
    """
    try:
        with open('face_data.pkl', 'rb') as f:
            face_database = pickle.load(f)
        return face_database
    except FileNotFoundError:
        return {}

def add_face_encoding(directory):
    """
    Add new face encodings to the face database.
    """
    face_database = open_face_database()
    new_face_encoding = recognize_faces(directory)
    face_database.update(new_face_encoding)
    with open('face_data.pkl', 'wb') as f:
        pickle.dump(face_database, f)

def recognize(image: np.ndarray, face_database):
    """
    Recognize a face in an image using the face database.
    """
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    boxes = face_recognition.face_locations(rgb, model="hog")
    encodings = face_recognition.face_encodings(rgb, boxes)

    names = []
    for encoding in encodings:
        matches = face_recognition.compare_faces(list(face_database.values()), encoding)
        name = "UNKNOWN"
        if True in matches:
            match_index = matches.index(True)
            name = list(face_database.keys())[match_index]
        names.append(name)

    return names[0] if names else "UNKNOWN"

def initialize():
    """
    Initialize the face database.
    """
    print("Initializing face database...")
    directories = ['FACES/DAVIDE', 'FACES/MOZ', 'FACES/RICCARDO']
    face_dicts = [recognize_faces(directory) for directory in directories]
    face_database = {}
    for face_dict in face_dicts:
        face_database.update(face_dict)
    with open('face_data.pkl', 'wb') as f:
        pickle.dump(face_database, f)
    print("Face database initialized.")

# Initialize the face database
#initialize()

# Open the face database
#face_database = open_face_database()

# Recognize a face
#image = cv2.imread('FACES/RICCARDO/RICCARDO_70.png')
#name = recognize(image, face_database)
#print(f"The face belongs to: {name}")

# Add a new face encoding
#add_face_encoding('FACES/FORTRAINING')