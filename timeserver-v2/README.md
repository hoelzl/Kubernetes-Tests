# Timeserver V2

This version of the time server contains health checks so that Kubernetes can
monitor the status of the application and provide zero-downtime updates.

- The time server now implements health checks for
  - Liveness (`/admin/alive`)
  - Readiness (`/admin/ready`)
- The deployment configuration has been updated to include the health checks
- Each pod has a 10% chance to fail the readiness check
- It will never recover on its own, but needs to be restarted to become active
  again
- The liveness checks are implemented, so that the pod will report no longer
  being alive after 30 seconds of being unresponsive

## Building the Docker Image

Building the Docker image is not necessary, since the deployment uses the image
`mhoelzl/timeserver:0.0.3` from Docker Hub. If you want to build the image
anyway, you can follow these steps. You need to change the `mhoelzl` prefix to
your own Docker Hub username.

Change to the `service` directory and run the following command to build the
Docker image:

```bash
docker build . --tag mhoelzl/timeserver:0.0.3
```

Push the Docker image to Docker Hub:

```bash
docker push mhoelzl/timeserver:0.0.3
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

You should see that from time to time a pod changes its `READY` status from
`1/1` to `0/1` and then its `STATUS` from `Running` to `Terminating`. After a
short while, you should see the pod being restarted and going back to status
`Running`.

Sometimes you will see a pod entering the `CrashLoopBackOff` state. This is
because the pod is failing the readiness check too often.
