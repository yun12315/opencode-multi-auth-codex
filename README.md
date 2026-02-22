# opencode-multi-auth-codex

Multi-account OAuth rotation plugin for OpenCode with a local dashboard, force mode, weighted settings, limits probing, and reliability hardening.

## Documentation map

- `README.md` -> primary operator and developer documentation for current behavior.
- `docs/ADMIN_MERGE_BRIEF.md` -> concise upstream/admin review summary.
- `docs/PHASE_H_VALIDATION.md` -> final validation report (current readiness reference).
- `codextesting.md` -> live/manual testing runbook.
- `docs/README.md` -> full docs index with authoritative vs historical references.

## What this project does

- Rotates requests across multiple ChatGPT/Codex OAuth accounts.
- Keeps a local account store with migration, validation, and atomic writes.
- Provides a localhost dashboard to manage accounts and limits.
- Supports force mode (pin one alias), account enable/disable, and re-auth.
- Supports settings-driven rotation strategy (`round-robin`, `least-used`, `random`, `weighted-round-robin`).
- Probes limits safely and keeps authoritative data quality rules.
- Gates non-core Antigravity features behind a feature flag.

## Current implementation status

- Core phases A-G are implemented in this workspace.
- Validation scripts are available for: unit, integration, web-headless, failure, stress, sandbox, soak.
- Web hardening fixes are in place:
  - localhost-only bind enforcement
  - malformed JSON returns deterministic `400` without process crash
  - dashboard client script parse issue fixed

## Behavior guarantees (latest)

- Rate-limit handling sleeps an alias until reset when reset timing is known (`Retry-After`, rate-limit window reset, or parsed provider reset text), instead of retrying that alias immediately.
- Force mode is strict: when enabled, requests stay pinned to the forced alias and do not silently fall back to other aliases.
- Rotation strategy control is shown next to Force Mode in the dashboard.
- Strategy changes from dashboard settings are applied to runtime selection logic (not just persisted state/UI display).
- Force Mode and strategy interaction is explicit:
  - while Force Mode is ON, strategy changes are saved
  - saved strategy becomes active when Force Mode is turned OFF
- Dashboard controls include mouseover help text for Force Mode and rotation strategy definitions.
- Account enable/disable toggle is authoritative for eligibility in rotation.

## Rotation strategy reference

- `round-robin` -> cycle through healthy enabled accounts in order.
- `least-used` -> prefer the healthy enabled account with the lowest usage count.
- `random` -> pick randomly from healthy enabled accounts.
- `weighted-round-robin` -> split traffic by configured account weights (example: `0.70/0.20/0.10` ≈ `70%/20%/10%`).
- Force Mode precedence -> when Force Mode is ON, strategy is paused; strategy changes are saved and become active when Force Mode is OFF.

## Repository structure

- `src/` -> TypeScript source
- `dist/` -> compiled output (`tsc` generated)
- `tests/unit/` -> unit tests
- `tests/integration/` -> integration tests
- `tests/web-headless/` -> headless UI smoke tests
- `tests/failure/` -> failure-injection tests
- `tests/stress/` -> stress/concurrency tests
- `tests/sandbox/` -> sandbox isolation tests
- `tests/soak/` -> soak scaffolding
- `docs/` -> QA and phase documentation (see `docs/README.md` for canonical/historical split)
- `IMPLEMENTATION_PLAN.md` -> full plan and contracts
- `TEST_EXECUTION_PLAN.md` -> required test order and gates
- `codextesting.md` -> live testing TODO for Codex CLI sessions

## Requirements

- Node.js 20+
- npm
- OpenCode CLI
- ChatGPT/Codex OAuth accounts

## Install and use

### Plugin install (recommended)

In your OpenCode config (`~/.config/opencode/opencode.json`):

```json
{
  "plugin": ["github:guard22/opencode-multi-auth-codex"]
}
```

### From source

```bash
git clone https://github.com/guard22/opencode-multi-auth-codex.git
cd opencode-multi-auth-codex
npm ci
npm run build
```

### Quick start

```bash
# Add accounts
opencode-multi-auth add personal
opencode-multi-auth add work

# Check status
opencode-multi-auth status

# Start dashboard
opencode-multi-auth web --host 127.0.0.1 --port 3434
```

Open `http://127.0.0.1:3434`.

## CLI commands

- `opencode-multi-auth add <alias>` -> add account via OAuth
- `opencode-multi-auth remove <alias>` -> remove account
- `opencode-multi-auth list` -> list configured accounts
- `opencode-multi-auth status` -> full status
- `opencode-multi-auth path` -> print store path
- `opencode-multi-auth web --host 127.0.0.1 --port 3434` -> run dashboard
- `opencode-multi-auth service install|disable|status` -> systemd user service helpers

## Dashboard/API endpoints

- `GET /api/state`
- `GET /api/logs`
- `POST /api/sync`
- `POST /api/auth/start`
- `POST /api/switch`
- `POST /api/remove`
- `POST /api/account/meta`
- `POST /api/token/refresh`
- `POST /api/limits/refresh`
- `POST /api/limits/stop`
- `GET /api/accounts`
- `PUT /api/accounts/:alias/enabled`
- `POST /api/accounts/:alias/reauth`
- `GET /api/force`
- `POST /api/force`
- `POST /api/force/clear`
- `GET /api/settings`
- `PUT /api/settings`
- `GET /api/settings/feature-flags`
- `PUT /api/settings/feature-flags`
- `POST /api/settings/reset`
- `POST /api/settings/preset`
- `POST /api/antigravity/refresh` (feature-flag gated)
- `POST /api/antigravity/refresh-all` (feature-flag gated)

## Environment variables

### Storage and auth

- `OPENCODE_MULTI_AUTH_STORE_DIR` -> override store directory
- `OPENCODE_MULTI_AUTH_STORE_FILE` -> override store file path
- `OPENCODE_MULTI_AUTH_CODEX_AUTH_FILE` -> override Codex `auth.json`
- `CODEX_SOFT_STORE_PASSPHRASE` -> encrypt account store at rest
- `CODEX_SOFT_LOG_PATH` -> override dashboard log path

### Rotation and limits

- `OPENCODE_MULTI_AUTH_ROTATION_STRATEGY` (settings source override; runtime rotation follows persisted dashboard settings)
- `OPENCODE_MULTI_AUTH_CRITICAL_THRESHOLD`
- `OPENCODE_MULTI_AUTH_LOW_THRESHOLD`
- `OPENCODE_MULTI_AUTH_TOKEN_FAILURE_COOLDOWN_MS`
- `OPENCODE_MULTI_AUTH_PROBE_EFFORT`
- `OPENCODE_MULTI_AUTH_LIMITS_PROBE_MODELS`

### Model mapping and runtime behavior

- `OPENCODE_MULTI_AUTH_PREFER_CODEX_LATEST`
- `OPENCODE_MULTI_AUTH_CODEX_LATEST_MODEL`
- `OPENCODE_MULTI_AUTH_INJECT_MODELS`
- `OPENCODE_MULTI_AUTH_TRUNCATION`
- `OPENCODE_MULTI_AUTH_DEBUG`

### Feature flags

- `OPENCODE_MULTI_AUTH_ANTIGRAVITY_ENABLED`

### Notifications

- `OPENCODE_MULTI_AUTH_NOTIFY`
- `OPENCODE_MULTI_AUTH_NOTIFY_SOUND`
- `OPENCODE_MULTI_AUTH_NOTIFY_MAC_OPEN`
- `OPENCODE_MULTI_AUTH_NOTIFY_NTFY_URL`
- `OPENCODE_MULTI_AUTH_NOTIFY_NTFY_TOKEN`
- `OPENCODE_MULTI_AUTH_NOTIFY_UI_BASE_URL`

## Security rules

- Dashboard host is loopback-only (`127.0.0.1`, `::1`, `localhost`).
- Non-loopback host bind is rejected.
- Sensitive token patterns are redacted in logs.
- Store file permissions are restricted (`0o600`).
- Antigravity APIs are blocked when feature flag is off.

## Build and test

```bash
npm ci
npm run lint
npm run build
npx tsc --noEmit

npm run test:unit
npm run test:integration
npm run test:web:headless
npm run test:failure
npm run test:stress
npm run test:sandbox
npm run test:soak:48h
```

Current test script surfaces are scaffolded and active. For true long soak, set a long duration and keep the run alive.

## Live validation runbook

Use `codextesting.md` for the Codex CLI live-testing checklist and copy-paste command flow.

## Troubleshooting

- If dashboard start fails with localhost error, check `--host` and use loopback only.
- If a request returns `INVALID_JSON`, verify payload body is valid JSON.
- If an alias action returns `ACCOUNT_NOT_FOUND`, refresh account list first.
- If re-auth is blocked with `ACCOUNT_DISABLED`, enable the account before re-auth.
- If encrypted store appears locked, export `CODEX_SOFT_STORE_PASSPHRASE` before launching.

## Development notes

- Edit `src/*`, never hand-edit `dist/*`.
- Run `npm run build` after source changes.
- Keep manual/live tests sandboxed (temp HOME/store/auth paths).

## License

MIT
