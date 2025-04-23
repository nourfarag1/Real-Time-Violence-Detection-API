#!/usr/bin/env python
"""
ingest‑mjpeg – Expose a local webcam (IVCam, USB, etc.) as an MJPEG HTTP stream.

Usage:
    python app.py --index 0 --port 9000 --cam-id 123e4567-e89b-12d3-a456-426614174000

• --index   : integer webcam index (default 0)
• --port    : TCP port to listen on (default 9000)
• --cam-id  : camera GUID used in URL (/stream/<cam-id>.mjpg)
"""

import argparse, cv2, time
from flask import Flask, Response

# --------------------------------------------------------------------------- CLI
parser = argparse.ArgumentParser()
parser.add_argument("--index",   type=int, default=0,   help="camera index")
parser.add_argument("--port",    type=int, default=9000,help="HTTP port")
parser.add_argument("--cam-id",  required=True,         help="camera GUID")
args = parser.parse_args()

CAM_INDEX = args.index
PORT      = args.port
CAM_ID    = args.cam_id

# ---------------------------------------------------------------------- OpenCam
def open_camera():
    """Try DirectShow first (Windows), then fallback to default backend."""
    for backend in (cv2.CAP_DSHOW, cv2.CAP_ANY):
        cap = cv2.VideoCapture(CAM_INDEX, backend)
        ok, _ = cap.read()
        if ok:
            print(f"[INFO] Camera opened at index {CAM_INDEX} (backend {backend})")
            return cap
        cap.release()
    raise RuntimeError(f"Cannot open webcam at index {CAM_INDEX}")

cap = open_camera()

# ---------------------------------------------------------------------- Flask App
app = Flask(__name__)

def mjpeg_stream():
    while True:
        ok, frame = cap.read()
        if not ok:
            time.sleep(0.05)
            continue
        _, jpg = cv2.imencode(".jpg", frame, [int(cv2.IMWRITE_JPEG_QUALITY), 80])
        yield (b"--frame\r\n"
               b"Content-Type: image/jpeg\r\n\r\n" +
               jpg.tobytes() + b"\r\n")
        # 20 fps ⇒ sleep 50 ms
        time.sleep(0.05)

@app.route(f"/stream/{CAM_ID}.mjpg")
def stream():
    return Response(mjpeg_stream(),
                    mimetype="multipart/x-mixed-replace; boundary=frame")

# --------------------------------------------------------------------------- Run
if __name__ == "__main__":
    print(f"[INFO] Starting MJPEG server on 0.0.0.0:{PORT}/stream/{CAM_ID}.mjpg")
    app.run(host="0.0.0.0", port=PORT, threaded=True)
