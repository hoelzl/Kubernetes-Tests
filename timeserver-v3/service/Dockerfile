FROM python:3.12
ENV PYTHONUNBUFFERED 1

COPY ./app /app

WORKDIR /app
RUN pip install --no-cache-dir -r requirements.txt

CMD ["uvicorn", "timeserver:app", "--host", "0.0.0.0", "--port", "80"]
