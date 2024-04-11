from fastapi import FastAPI, status
from fastapi.responses import JSONResponse
from datetime import datetime, timedelta
from pydantic import BaseModel

app = FastAPI()

VERSION = "0.0.4"

last_ready_time = datetime.now()
service_ok = True


class TimeResponse(BaseModel):
    time: str
    timezone: str
    version: str


class StatusResponse(BaseModel):
    status: str


@app.get("/", response_model=TimeResponse)
@app.get("/time", response_model=TimeResponse)
def get_time():
    now = datetime.now()
    return TimeResponse(
        time=now.strftime("%H:%M:%S"),
        timezone="UTC",
        version=VERSION,
    )


@app.get("/admin/alive", response_model=StatusResponse)
def check_alive():
    global last_ready_time
    if datetime.now() > last_ready_time + timedelta(seconds=30):
        return JSONResponse(
            content={"status": "Not Alive"},
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
        )
    return StatusResponse(status="Alive")


@app.get("/admin/ready", response_model=StatusResponse)
def check_ready():
    global last_ready_time
    global service_ok
    if service_ok:
        last_ready_time = datetime.now()
        return StatusResponse(status="Ready")
    return JSONResponse(
        content={"status": "Not Ready"}, status_code=status.HTTP_503_SERVICE_UNAVAILABLE
    )


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=80)
