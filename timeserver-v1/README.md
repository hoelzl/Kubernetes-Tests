# Building and Deploying the Time Server V1

## Building the Docker image for the initial version

Ensure that the line `Version = "0.0.1"` is uncommented in `timeserver.py` and
that the line `# Version = "0.0.2"` is commented out.

Then build the Docker image for the time server:

```bash
docker build . --tag mhoelzl/timeserver:0.0.1
```

Push the Docker image to Docker Hub:

```bash
docker push mhoelzl/timeserver:0.0.1
```

Deploy the time server to Kubernetes:

```bash
kubectl create -f deploy.yaml --save-config
```

Check the status of the deployment:

```bash
kubectl get deploy
```

Check the status of the pods:

```bash
kubectl get pods
```

If you have other pods running and want to focus on the time server pods:

```bash
kubectl get pods --selector=pod=timeserver-pod
```

## Temporary Access to the Time Server

Forward a port from our local machine to the container:

```bash
kubectl port-forward deploy/timeserver 8080:80
```

In another terminal, you can now access the time server.

```bash
curl localhost:8080
```

## Logging

To log access to the time server run the following command:

```bash
kubectl logs -f deploy/timeserver
```

This will continuously stream the logs to the terminal. Press `Ctrl-C` to stop.

To get the logs for a specific pod, run the following command:

```bash
kubectl logs <pod-name>
```

You can get the pod name by running `kubectl get pods`.

## Exposing a Service and Load Balancing

Stop the port forwarding by pressing `Ctrl-C` in the terminal in which you
started the port forwarding. You can check, that the port forwarding is stopped
by running `curl localhost:8080` again. It should fail.

To expose the time server as a service, run the following command:

```bash
kubectl create -f service.yaml --save-config
```

To check the status of the service, run:

```bash
kubectl get service
```

Now you can access the time server through the service. Depending on the external
IP of the service shown in the above output, you can access the time server with
the following command:

```bash
curl localhost:8080
```

or

```bash
curl <external-ip>:8080
```

To make the service accessible from outside the cluster, you can use kubectl to
establish a port forwarding for the service:

```bash
kubectl port-forward service/timeserver 8888:8080 --address 0.0.0.0
```

You can also port-forward to the deployment directly:

```bash
kubectl port-forward deploy/timeserver 8888:80 --address 0.0.0.0
```

In either way you should be able to access the services with either the
`http://localhost:8888` or `http://<hostname>:8888` URLs.

## Deploying Changes

First, uncomment the line `# Version = "0.0.2"` in `timeserver.py` and rebuild
the Docker image with a new version number.

```bash
docker build . --tag mhoelzl/timeserver:0.0.2
```

Push the new Docker image to Docker Hub:

```bash
docker push mhoelzl/timeserver:0.0.2
```

Run the following command to update the time server deployment:

```bash
kubectl apply -f deploy-v2.yaml
```

## Interacting with the Time Server Pods

To get a shell in a pod, run the following command:

```bash
kubectl exec -it deploy/timeserver -- /bin/bash
```

To copy files to or from a pod, run the following command:

```bash
kubectl cp $POD_NAME:/path/to/file /path/on/local/machine
kubectl cp /path/on/local/machine $POD_NAME:/path/to/file
```

You can get the pod name by running `kubectl get pods` or `kubectl get pods -o
name`.

## Cleaning Up

To delete the time server deployment and service, run the following commands:

```bash
kubectl delete deploy timeserver
kubectl delete service timeserver
```
