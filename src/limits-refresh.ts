import {
  getBlockingRateLimitResetAt,
  isRateLimitErrorText,
  mergeRateLimits,
  parseRateLimitResetFromError
} from './rate-limits.js'
import { loadStore, updateAccount } from './store.js'
import { probeRateLimitsForAccount } from './probe-limits.js'
import { logError, logInfo } from './logger.js'
import { calculateLimitsConfidence } from './types.js'
import type { AccountCredentials } from './types.js'

export interface LimitRefreshResult {
  alias: string
  updated: boolean
  error?: string
}

export async function refreshRateLimitsForAccount(account: AccountCredentials): Promise<LimitRefreshResult> {
  updateAccount(account.alias, { limitStatus: 'running', limitError: undefined })
  logInfo(`Refreshing limits for ${account.alias}`)
  const probe = await probeRateLimitsForAccount(account)
  
  // Phase C: Only accept authoritative limits from successful completed sessions
  if (!probe.isAuthoritative || !probe.rateLimits) {
    logError(`Limit probe failed for ${account.alias}: ${probe.error || 'Probe failed'}`)
    const now = Date.now()
    const errorText = probe.error || 'Probe failed'
    const likelyRateLimit = isRateLimitErrorText(errorText)
    const parsedResetAt = parseRateLimitResetFromError(errorText, now)
    const fallbackResetAt = likelyRateLimit
      ? getBlockingRateLimitResetAt(account.rateLimits, now)
      : undefined
    const rateLimitedUntil = parsedResetAt ?? fallbackResetAt
    
    // Phase C: Update only error metadata, preserve prior limits
    const updates: Partial<AccountCredentials> = {
      limitStatus: 'error',
      limitError: errorText,
      lastLimitErrorAt: now,
      limitsConfidence: calculateLimitsConfidence(
        account.lastLimitProbeAt,
        now,
        'error'
      )
    }
    if (typeof rateLimitedUntil === 'number' && rateLimitedUntil > now) {
      updates.rateLimitedUntil = rateLimitedUntil
    }
    updateAccount(account.alias, updates)
    return {
      alias: account.alias,
      updated: false,
      error: errorText
    }
  }

  // Phase C: Only merge authoritative limits from successful probe
  const now = Date.now()
  updateAccount(account.alias, {
    rateLimits: mergeRateLimits(account.rateLimits, probe.rateLimits),
    limitStatus: 'success',
    limitError: undefined,
    lastLimitProbeAt: now,
    limitsConfidence: calculateLimitsConfidence(now, account.lastLimitErrorAt, 'success')
  })
  
  logInfo(`Limits refreshed for ${account.alias} using model ${probe.probeModel || 'unknown'}, effort ${probe.probeEffort || 'default'}`)
  return { alias: account.alias, updated: true }
}

export async function refreshRateLimits(
  accounts: AccountCredentials[],
  alias?: string
): Promise<LimitRefreshResult[]> {
  if (alias) {
    const account = accounts.find((acc) => acc.alias === alias)
    if (!account) {
      return [{ alias, updated: false, error: 'Unknown alias' }]
    }
    return [await refreshRateLimitsForAccount(account)]
  }

  const store = loadStore()
  const results: LimitRefreshResult[] = []
  for (const account of accounts) {
    results.push(await refreshRateLimitsForAccount(account))
  }
  if (results.length === 0 && !store.activeAlias) {
    return [{ alias: 'active', updated: false, error: 'No accounts configured' }]
  }
  return results
}
