from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler
from datetime import datetime, timedelta
from random import random
import json

VERSION = "0.0.4"

last_ready_time = datetime.now()


class RequestHandler(BaseHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

    def do_GET(self):
        global last_ready_time
        global service_ok
        match self.path:
            case "/" | "/time":
                now = datetime.now()
                response_json = {
                    "time": now.strftime("%H:%M:%S"),
                    "timezone": "UTC",
                    "version": VERSION,
                }
                self.send_reply(response_json)
            case "/admin/alive":
                if datetime.now() > last_ready_time + timedelta(seconds=30):
                    self.send_reply({"status": "Not Alive"}, status=503)
                else:
                    self.send_reply({"status": "Alive"})
            case "/admin/ready":
                service_ok = True  # This would be a check of the service's health
                if service_ok:
                    last_ready_time = datetime.now()
                    self.send_reply({"status": "Ready"})
                else:
                    self.send_reply({"status": "Not Ready"}, status=503)

    def send_reply(self, data, status=200):
        self.send_response(status)
        self.send_header("Content-type", "application/json")
        self.end_headers()
        response_str = json.dumps(data) + "\n"
        self.wfile.write(response_str.encode("utf-8"))


def run(server_class=ThreadingHTTPServer, handler_class=RequestHandler):
    try:
        server_address = ("", 80)
        httpd = server_class(server_address, handler_class)
        print(f"Listening on {httpd.server_address[0]}:{httpd.server_address[1]}...")
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("\nShutting down the server...")
        httpd.shutdown()


if __name__ == "__main__":
    run()
