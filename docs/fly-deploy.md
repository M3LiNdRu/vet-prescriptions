# Deploying to Fly.io — Beginner Guide

This guide walks you through deploying VetPrescription to Fly.io for the first time.

---

## What is Fly.io?

Fly.io runs Docker containers on servers close to your users. Instead of managing a server yourself, you give Fly a Docker image and it handles the rest: running it, restarting it if it crashes, and exposing it over HTTPS.

### How Fly concepts map to this project

| Docker Compose | Fly.io equivalent |
|---|---|
| `docker-compose.yml` service | **App** — each service becomes its own Fly app |
| Named volume (`mongo-data`) | **Volume** — persistent disk attached to a machine |
| `environment:` variables | **Secrets** (sensitive) or `[env]` in `fly.toml` (non-sensitive) |
| Container-to-container DNS (`mongo:27017`) | **Internal DNS** (`vet-prescription-mongo.internal:27017`) — only reachable between your own apps |
| `ports:` mapping | **Services** — Fly handles TLS termination automatically |

This project deploys **three apps**:
1. `vet-prescription-mongo` — MongoDB database
2. `vet-prescription-api` — .NET backend API
3. `vet-prescription-frontend` — React frontend served by nginx

---

## Step 1 — Install flyctl

`flyctl` is the Fly.io CLI. Run this in your terminal:

```bash
curl -L https://fly.io/install.sh | sh
```

Then add it to your PATH (the installer will tell you the exact line). Verify it works:

```bash
fly version
```

---

## Step 2 — Create a Fly account and log in

```bash
fly auth signup   # creates a new account (opens browser)
# or
fly auth login    # if you already have an account
```

---

## Step 3 — Get your API token

You'll need this to let GitHub Actions deploy on your behalf.

```bash
fly tokens create deploy -x 999999h -n "github-actions"
```

Copy the token — you'll add it to GitHub in Step 7.

---

## Step 4 — Create the three Fly apps

Run these commands once to register the app names with Fly:

```bash
fly apps create vet-prescription-mongo
fly apps create vet-prescription-api
fly apps create vet-prescription-frontend
```

---

## Step 5 — Create the MongoDB volume

A Fly **Volume** is a persistent disk. Without it, MongoDB data would be lost every time the machine restarts.

```bash
fly volume create mongo_data \
  --app vet-prescription-mongo \
  --region mad \
  --size 1
```

- `--region mad` = Madrid (closest to Catalonia)
- `--size 1` = 1 GB (enough for development)

---

## Step 6 — Set secrets

Secrets are environment variables that are encrypted at rest and never visible in logs or config files.

### MongoDB connection string for the API

```bash
fly secrets set \
  "ConnectionStrings__MongoDB=mongodb://vet-prescription-mongo.internal:27017" \
  --app vet-prescription-api
```

The hostname `vet-prescription-mongo.internal` is Fly's private DNS — it only works between your own apps on the same Fly network, which is exactly what we want.

---

## Step 7 — Add FLY_API_TOKEN to GitHub

1. Go to your GitHub repository → **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**
3. Name: `FLY_API_TOKEN`
4. Value: paste the token from Step 3
5. Click **Add secret**

From now on, every push to `master` that passes CI will automatically deploy to Fly.io.

---

## Step 8 — First manual deploy

The GitHub Action will handle future deploys, but the first time you need to deploy manually to initialise everything.

Deploy in this order — **mongo first**, because the API needs it running:

```bash
# 1. Deploy MongoDB
fly deploy --config fly/mongo/fly.toml --app vet-prescription-mongo --remote-only

# 2. Deploy the API
fly deploy --config fly/api/fly.toml --app vet-prescription-api --remote-only

# 3. Deploy the frontend
fly deploy --config fly/frontend/fly.toml --app vet-prescription-frontend --remote-only
```

`--remote-only` tells Fly to build the Docker image on their servers instead of your machine (faster, no local Docker required).

---

## Checking logs

If something goes wrong, check the logs:

```bash
fly logs --app vet-prescription-api
fly logs --app vet-prescription-mongo
fly logs --app vet-prescription-frontend
```

To stream live logs:

```bash
fly logs --app vet-prescription-api --tail
```

---

## Checking app status

```bash
fly status --app vet-prescription-api
fly machines list --app vet-prescription-api
```

---

## Your app URLs

Once deployed, your apps are live at:

- Frontend: `https://vet-prescription-frontend.fly.dev`
- API: `https://vet-prescription-api.fly.dev` (also used by the frontend internally)

---

## Auto-sleep

The `fly.toml` configs use `auto_stop_machines = true` with `min_machines_running = 0`. This means machines **sleep when idle** and wake up on the first request (a few seconds delay). This keeps costs at zero on the free tier. If you want the app always-on, set `min_machines_running = 1`.
