# Pet Clinic - Remote Access With One Stable Link

> How to open your app from phone/laptop using the same URL even on different Wi-Fi networks.

---

## 1) Why your local link changes

Local links like `http://10.224.x.x:5173` are private LAN addresses.
They only work when your phone and PC are on the same network.
When you move to another Wi-Fi, that LAN IP is no longer reachable.

To get one stable link from anywhere, you need a **public endpoint**:
- either a cloud deployment, or
- a secure tunnel from your local PC.

For development, a tunnel is the fastest option.

---

## 2) What was already fixed in this project (and why)

File updated: `velinn-petclinic_js/src/vite.config.ts`

```ts
server: {
  host: '0.0.0.0',
  port: 5173,
  strictPort: true,
  proxy: {
    '/api/v1': {
      target: apiProxyTarget,
      changeOrigin: true
    }
  }
}
```

Why these settings:
- `host: '0.0.0.0'` allows connections from devices outside localhost (same LAN test).
- `port: 5173` makes URL consistent.
- `strictPort: true` prevents automatic fallback to 5174/5175, which breaks saved links.

This solves stable LAN access. For cross-network access, continue below.

---

## 3) Recommended approach: Cloudflare Tunnel (same link everywhere)

### Step 1 - Prepare domain in Cloudflare
1. Create/sign in to Cloudflare account.
2. Add your domain to Cloudflare and complete DNS onboarding.

Why:
A stable public hostname (for example `PetClinic.velinn.com`) must be managed by Cloudflare DNS.

### Step 2 - Install cloudflared on Windows
1. Install `cloudflared` (official Cloudflare package).
2. Verify installation:

```powershell
cloudflared --version
```

Why:
`cloudflared` is the local connector that publishes your local app to a public URL securely.

### Step 3 - Authenticate this machine
Run:

```powershell
cloudflared tunnel login
```

Why:
This authorizes your PC to create and run tunnels under your Cloudflare account.

### Step 4 - Create a named tunnel
Run:

```powershell
cloudflared tunnel create petclinic-dev
```

Why:
A named tunnel gives repeatable setup and stable identity.

### Step 5 - Attach DNS hostname to the tunnel
Run:

```powershell
cloudflared tunnel route dns petclinic-dev PetClinic.velinn.com
```

Why:
This maps your permanent public hostname to the tunnel.

### Step 6 - Create tunnel config
Create file:
`C:\Users\<your-user>\.cloudflared\config.yml`

Use:

```yaml
tunnel: petclinic-dev
credentials-file: C:\Users\<your-user>\.cloudflared\<tunnel-id>.json
ingress:
  - hostname: PetClinic.velinn.com
    service: http://localhost:5173
  - service: http_status:404
```

Why:
This forwards public HTTPS traffic to your local Vite app.

### Step 7 - Run the frontend app
From project frontend folder:

```powershell
npm run dev
```

Why:
Tunnel forwards traffic to your running local app at port 5173.

### Step 8 - Run the tunnel
Run:

```powershell
cloudflared tunnel run petclinic-dev
```

Why:
This starts secure outbound connection to Cloudflare edge and activates your public link.

### Step 9 - Open from any network
Use:

`https://PetClinic.velinn.com`

Why:
This URL is public and independent of your local Wi-Fi IP.

---

## 4) Optional: start tunnel automatically on boot

```powershell
cloudflared service install
```

Why:
Keeps the same link available without manually running tunnel each time.

---

## 5) Troubleshooting checklist

### A) Public URL opens but page is blank or errors
- Confirm `npm run dev` is running.
- Confirm Vite still listens on `5173` with strict port.

### B) Frontend opens but API calls fail
- Ensure backend API is running.
- Verify frontend proxy target in `velinn-petclinic_js/src/vite.config.ts`.

### C) Phone cannot open public URL
- Disable VPN/private relay on phone.
- Test both Wi-Fi and mobile data.
- Confirm tunnel process is running on PC.

### D) Local network URL fails on phone
- Confirm PC and phone are on same Wi-Fi.
- Check Windows Firewall inbound rule for TCP 5173 (administrator may be required).

### E) Browser says "Your connection isn't private"
Most common causes and fixes:

1. Placeholder hostname was used literally.
- `PetClinic.velinn.com` in this guide is the selected hostname.
- If DNS/cert issues appear, use lowercase in DNS records and tests: `petclinic.velinn.com`.

2. DNS record not attached to the tunnel yet.
- Re-run:

```powershell
cloudflared tunnel route dns petclinic-dev PetClinic.velinn.com
```

3. Cloudflare proxy is disabled (gray cloud).
- In Cloudflare DNS, the record for your dev hostname must be **Proxied** (orange cloud).

4. Universal SSL certificate is still provisioning.
- New hostname certificates can take a few minutes to become valid globally.
- Wait 5-15 minutes, then test again.

5. SSL/TLS mode is misconfigured.
- In Cloudflare Dashboard -> SSL/TLS -> Overview, set mode to **Full** (or **Full (strict)** when origin certs are in place).

6. Local browser cache/HSTS is stale.
- Try private/incognito mode first.
- Then test from mobile data to bypass local DNS cache.

Quick verification commands:

```powershell
nslookup PetClinic.velinn.com
cloudflared tunnel list
cloudflared tunnel info petclinic-dev
```

Expected result:
- Hostname resolves publicly.
- Tunnel status is healthy.
- HTTPS opens without certificate warning.

---

## 6) Security note

Tunnel-based access exposes a development app publicly.
Before production use:
- deploy frontend and API to managed hosting,
- enable HTTPS and hardened auth,
- set production CORS/env config,
- avoid exposing local developer machines long-term.

---

## 7) Quick command reference

```powershell
# Frontend
npm run dev

# Tunnel (after one-time setup)
cloudflared tunnel run petclinic-dev

# Verify Vite listening on 5173
netstat -ano | findstr LISTENING | findstr :5173
```

---

Document version: 1.1
Last updated: 2026-03-19
