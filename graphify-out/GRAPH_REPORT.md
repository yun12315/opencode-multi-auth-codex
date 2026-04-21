# Graph Report - G:\workspace\opencode-multi-auth-codex  (2026-04-21)

## Corpus Check
- 41 files · ~59,618 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 320 nodes · 702 edges · 16 communities detected
- Extraction: 84% EXTRACTED · 16% INFERRED · 0% AMBIGUOUS · INFERRED: 110 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]
- [[_COMMUNITY_Community 15|Community 15]]

## God Nodes (most connected - your core abstractions)
1. `loadStore()` - 39 edges
2. `updateAccount()` - 22 edges
3. `syncCodexAuthFile()` - 17 edges
4. `saveStore()` - 16 edges
5. `refreshRateLimitsForAccount()` - 15 edges
6. `login_account()` - 14 edges
7. `probeRateLimitsForAccount()` - 12 edges
8. `getNextAccount()` - 12 edges
9. `syncAuthFromOpenCode()` - 11 edges
10. `main()` - 11 edges

## Surprising Connections (you probably didn't know these)
- `refreshRateLimitsForAccount()` --calls--> `calculateLimitsConfidence()`  [INFERRED]
  G:\workspace\opencode-multi-auth-codex\src\limits-refresh.ts → G:\workspace\opencode-multi-auth-codex\src\types.ts
- `refreshRateLimitsForAccount()` --calls--> `isRateLimitErrorText()`  [INFERRED]
  G:\workspace\opencode-multi-auth-codex\src\limits-refresh.ts → G:\workspace\opencode-multi-auth-codex\src\rate-limits.ts
- `add_account_to_store()` --calls--> `append()`  [INFERRED]
  G:\workspace\opencode-multi-auth-codex\auto-login\auto_login.py → G:\workspace\opencode-multi-auth-codex\src\logger.ts
- `findAccountAliasByToken()` --calls--> `loadStore()`  [INFERRED]
  G:\workspace\opencode-multi-auth-codex\src\auth-sync.ts → G:\workspace\opencode-multi-auth-codex\src\store.ts
- `syncAuthFromOpenCode()` --calls--> `getAccountIdFromClaims()`  [INFERRED]
  G:\workspace\opencode-multi-auth-codex\src\auth-sync.ts → G:\workspace\opencode-multi-auth-codex\src\codex-auth.ts

## Communities

### Community 0 - "Community 0"
Cohesion: 0.09
Nodes (40): ensureValidToken(), activateForce(), checkAndAutoClearForce(), clearForce(), getForceState(), getRemainingForceTimeMs(), isForceActive(), isRotationStrategy() (+32 more)

### Community 1 - "Community 1"
Cohesion: 0.07
Nodes (36): createAuthorizationFlow(), findAvailablePort(), getRedirectUri(), loginAccount(), tryListenOnPort(), getCodexAuthPath(), appendPendingLoginOutput(), consumeProcessLines() (+28 more)

### Community 2 - "Community 2"
Cohesion: 0.11
Nodes (31): add_account_to_store(), build_auth_url(), CallbackServer, cmd_check(), cmd_login(), decode_jwt_payload(), exchange_code_for_tokens(), fetch_userinfo_email() (+23 more)

### Community 3 - "Community 3"
Cohesion: 0.11
Nodes (28): resolveRateLimitedUntil(), ensureWindow(), extractRateLimitUpdate(), getBlockingRateLimitResetAt(), hasMeaningfulRateLimits(), hasMeaningfulRateLimitWindow(), isRateLimitErrorText(), matchWindowKey() (+20 more)

### Community 4 - "Community 4"
Cohesion: 0.13
Nodes (23): refreshToken(), refreshRateLimits(), refreshRateLimitsForAccount(), append(), ensureDir(), logInfo(), logWarn(), sanitize() (+15 more)

### Community 5 - "Community 5"
Cohesion: 0.16
Nodes (21): buildAlias(), fetchEmail(), findAccountAliasByEmail(), findAccountAliasByToken(), syncAuthFromOpenCode(), asString(), copyConfigToml(), decodeJwtPayload() (+13 more)

### Community 6 - "Community 6"
Cohesion: 0.13
Nodes (12): logError(), getRefreshQueueState(), waitForQueueToFinish(), applyPreset(), getRuntimeSettings(), getSettings(), getSettingsWithInfo(), isFeatureEnabled() (+4 more)

### Community 7 - "Community 7"
Cohesion: 0.25
Nodes (20): buildAlias(), decodeJwtPayload(), ensureDir(), findMatchingAlias(), fingerprintTokens(), getAccountIdFromClaims(), getAccountUserIdFromClaims(), getCodexAuthStatus() (+12 more)

### Community 8 - "Community 8"
Cohesion: 0.12
Nodes (4): convertSseToJson(), extractPathAndSearch(), parseSseStream(), toCodexBackendUrl()

### Community 9 - "Community 9"
Cohesion: 0.29
Nodes (11): getFlagValue(), main(), listAccounts(), removeAccount(), disableService(), ensureDir(), getServiceDir(), getServiceFilePath() (+3 more)

### Community 10 - "Community 10"
Cohesion: 0.47
Nodes (9): buildFastProviderModel(), buildProviderModel(), fetchAvailableModels(), filterGPT5Models(), generateModelVariants(), getDefaultModels(), getModelLimits(), getModels() (+1 more)

### Community 11 - "Community 11"
Cohesion: 0.67
Nodes (0): 

### Community 12 - "Community 12"
Cohesion: 0.67
Nodes (0): 

### Community 13 - "Community 13"
Cohesion: 0.67
Nodes (0): 

### Community 14 - "Community 14"
Cohesion: 1.0
Nodes (0): 

### Community 15 - "Community 15"
Cohesion: 1.0
Nodes (0): 

## Knowledge Gaps
- **4 isolated node(s):** `Login to Outlook Web and get past all Microsoft interstitials.     Returns the`, `Refresh Outlook inbox and extract the verification code from the latest email.`, `HTTP handler that captures the OAuth callback code into a shared list.`, `Full OAuth login. Strategy:     1. Navigate to OpenAI auth     2. Enter email`
  These have ≤1 connection - possible missing edges or undocumented components.
- **Thin community `Community 14`** (1 nodes): `jest.config.js`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Community 15`** (1 nodes): `localhost.test.ts`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `append()` connect `Community 4` to `Community 2`, `Community 6`?**
  _High betweenness centrality (0.184) - this node is a cross-community bridge._
- **Why does `add_account_to_store()` connect `Community 2` to `Community 4`?**
  _High betweenness centrality (0.152) - this node is a cross-community bridge._
- **Why does `loadStore()` connect `Community 0` to `Community 1`, `Community 4`, `Community 5`, `Community 6`, `Community 7`, `Community 9`?**
  _High betweenness centrality (0.069) - this node is a cross-community bridge._
- **Are the 22 inferred relationships involving `loadStore()` (e.g. with `findAccountAliasByToken()` and `syncAuthFromOpenCode()`) actually correct?**
  _`loadStore()` has 22 INFERRED edges - model-reasoned connections that need verification._
- **Are the 17 inferred relationships involving `updateAccount()` (e.g. with `syncAuthFromOpenCode()` and `refreshToken()`) actually correct?**
  _`updateAccount()` has 17 INFERRED edges - model-reasoned connections that need verification._
- **Are the 4 inferred relationships involving `syncCodexAuthFile()` (e.g. with `loadStore()` and `updateAccount()`) actually correct?**
  _`syncCodexAuthFile()` has 4 INFERRED edges - model-reasoned connections that need verification._
- **Are the 5 inferred relationships involving `saveStore()` (e.g. with `activateForce()` and `clearForce()`) actually correct?**
  _`saveStore()` has 5 INFERRED edges - model-reasoned connections that need verification._