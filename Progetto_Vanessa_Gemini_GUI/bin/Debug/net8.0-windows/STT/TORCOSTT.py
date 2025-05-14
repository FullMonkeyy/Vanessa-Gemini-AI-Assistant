import torch
import librosa
from transformers import Wav2Vec2ForCTC, Wav2Vec2Processor

import transformers
import win32file
import warnings
if True:
    transformers.logging.set_verbosity(transformers.logging.ERROR)
    warnings.filterwarnings("ignore", category=FutureWarning, module="huggingface_hub.file_download")
    processor = Wav2Vec2Processor.from_pretrained("jonatasgrosman/wav2vec2-large-xlsr-53-italian", local_files_only=True)
    model = Wav2Vec2ForCTC.from_pretrained("jonatasgrosman/wav2vec2-large-xlsr-53-italian", ignore_mismatched_sizes=True)

    # Specify the device (GPU or CPU)
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    model.to(device)  # Move the model to the specified device
def transcribe_wav_to_text(wav_path):
    # Load the audio file as an array of samples
    speech_array, sampling_rate = librosa.load(wav_path, sr=16_000)

    # Preprocess the audio file
    inputs = processor(speech_array, sampling_rate=16_000, return_tensors="pt", padding=True)
    inputs = inputs.to(device)  # Move the inputs to the specified device

    # Inference
    with torch.no_grad():
        logits = model(inputs.input_values, attention_mask=inputs.attention_mask).logits

    # Decode token IDs to text
    predicted_ids = torch.argmax(logits, dim=-1)
    predicted_text = processor.batch_decode(predicted_ids)[0]

    return predicted_text
def write_to_pipe(pipe, text):
    """Write text to pipe"""
    try:
        win32file.WriteFile(pipe, text.encode("utf-8") + b"\n")
    except Exception as e:
        print(f"Error writing to pipe: {e}")

def TelegramVoiceRecognizer():

    voicepath="TELEGRAMVOICES/VoiceDIDavide.wav"

    pipe = win32file.CreateFile(
        r'\\.\pipe\TELEGRAMVOICERECOGNITION',
        win32file.GENERIC_READ | win32file.GENERIC_WRITE,
        0,
        None,
        win32file.OPEN_EXISTING,
        0,
        None
    )

    print("Voice recognition per telegram online")
    while True:
        try:
            response = win32file.ReadFile(pipe, 1024)
        except Exception as e:
            print(f"Error reading from pipe: {e}")
            break

        text = "-9898989898989998989"
        while text == "-9898989898989998989":
            print("Vanessa sta processando il voice message di telegram")
            text = transcribe_wav_to_text(voicepath)
            if not "-9898989898989998989" in text:
                print(text)

        write_to_pipe(pipe, text)


#TelegramVoiceRecognizer()
