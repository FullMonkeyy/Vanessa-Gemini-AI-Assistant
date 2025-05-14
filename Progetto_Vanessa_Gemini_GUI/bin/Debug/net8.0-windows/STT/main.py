
import speech_recognition as sr

import win32pipe, win32file, pywintypes
import threading

import TORCOSTT as TOC



recognizer_instance = sr.Recognizer()
recognizer_instance.energy_threshold = 400
ProcessoAttivo = True


def adjust_microphone(source):
    """Adjust microphone for ambient noise"""
    recognizer_instance.adjust_for_ambient_noise(source, duration=5)

def recognize_speech(source):
    """Recognize speech and return text"""
    try:
        audio = recognizer_instance.listen(source, timeout=1, phrase_time_limit=45)
        print("Elabaroazione voce a testo in corso...")
        text = recognizer_instance.recognize_google(audio, language="it-IT")

        text = text.lower()
        print("Elabaroazione voce a testo completata.")
        return text
    except sr.exceptions.WaitTimeoutError:
        print("Listening timed out. Trying again...")

        return "-9898989898989998989"
    except Exception as e:
        print("Silenzio...")
        return "-9898989898989998989"

def write_to_pipe(pipe, text):
    """Write text to pipe"""
    try:
        win32file.WriteFile(pipe, text.encode("utf-8") + b"\n")
    except Exception as e:
        print(f"Error writing to pipe: {e}")

def main():

    TelegramVoiceThread = threading.Thread(target=TOC.TelegramVoiceRecognizer)
    TelegramVoiceThread.start()
    with sr.Microphone() as source:
        adjust_microphone(source)
        pipe = win32file.CreateFile(
            r'\\.\pipe\STT',
            win32file.GENERIC_READ | win32file.GENERIC_WRITE,
            0,
            None,
            win32file.OPEN_EXISTING,
            0,
            None
        )

        print("Voice recognition online")
        while True:
            try:
                response = win32file.ReadFile(pipe, 1024)
            except Exception as e:
                print(f"Error reading from pipe: {e}")
                break

            text = "-9898989898989998989"
            while text == "-9898989898989998989":
                print("Vanessa sta ascoltando adesso")
                text = recognize_speech(source)
                if not "-9898989898989998989" in text:
                    print(text)
                with open('test.txt', 'w') as f:
                    f.write(text)

            write_to_pipe(pipe, text)

if __name__ == "__main__":
    main()

    # audio_data = audio.get_raw_data()

    # audio_data = audio.get_raw_data()

    # Get the sample rate from the audio object
    # #sample_rate = audio.sample_rate

# Create a WAV file
# with wave.open('output.wav', 'wb') as wav_file:
# Set the WAV file parameters
# wav_file.setnchannels(1)  # Mono audio
# wav_file.setsampwidth(2)  # 16-bit audio
# wav_file.setframerate(sample_rate)

# Write the audio data to the WAV file
#  wav_file.writeframes(audio_data)