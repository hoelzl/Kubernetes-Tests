from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler
from datetime import datetime
import json

VERSION = "0.0.1"
# VERSION = "0.0.2"

class RequestHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        if VERSION == "0.0.1":
            self.do_GET_v1()
        else:
            self.do_GET_v2()

    def do_GET_v1(self):
        self.send_response(200)
        self.send_header("Content-type", "text/plain")
        self.end_headers()
        now = datetime.now()
        response = now.strftime("The time is: %H:%M:%S, UTC\n")
        self.wfile.write(response.encode("utf-8"))

    def do_GET_v2(self):
        self.send_response(200)
        self.send_header("Content-type", "application/json")
        self.end_headers()
        now = datetime.now()
        response_json = {"time": now.strftime("%H:%M:%S"), "timezone": "UTC"}
        response_str = json.dumps(response_json) + "\n"
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
