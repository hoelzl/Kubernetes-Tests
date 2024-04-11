# Timeserver V3: Rolling Update

This version of the time server can be used to update version 2 of the time
server with pods that don't fail their health checks.

## Building the Docker Image

Building the Docker image is not necessary, since the deployment uses the image
`mhoelzl/timeserver:0.0.4` from Docker Hub. If you want to build the image
anyway, you can follow these steps. You need to change the `mhoelzl` prefix to
your own Docker Hub username.

Change to the `service` directory and run the following command to build the
Docker image:

```bash
docker build . --tag mhoelzl/timeserver:0.0.4
```

Push the Docker image to Docker Hub:

```bash
docker push mhoelzl/timeserver:0.0.4
```

## Deploying to Kubernetes

To deploy this version of the time server, you can use the following command
from the `kubernetes` directory:

```bash
kubectl apply -f .
```

This will create the deployment and service for the time server. You can check
that the services are failing and being restarted periodically by running:

```bash
kubectl get pods -w
```

You should see that Kubernetes rolls out the new version of the time server
without any downtime. The pods will be replaced one by one, with old pods being
terminated as new pods are launched.

To see that both versions of the time server are running at the same time, you
can run the following command while the services are rolled out. On Linux/WSL:

```bash
while true; do curl -s http://localhost:8080; echo ""; sleep 0.2; done
```

On Windows using PowerShell, you can use:

```powershell
while ($true) { curl.exe -s http://localhost:8080; echo ""; Start-Sleep -Milliseconds 200 }
```

Before the rollout, all messages should have version number `0.0.3`. During the
rollout, you should see some messages with version `0.0.3` interspersed with
messages showing version `0.0.4`. After the rollout is complete, all messages
should be from version `0.0.4`.
