# Fix Gemini Quota Issue - Quick Guide

## 🚨 Problem
Gemini API free tier quota is exhausted (0 requests remaining). LinkerAI is falling back to basic deterministic matching.

## ✅ Solution: Use Groq API (Recommended)

Groq is **FREE**, **FAST**, and has **generous quotas**. It's already integrated in the code!

### Step 1: Get Groq API Key
1. Go to: https://console.groq.com/
2. Sign up (free, no credit card needed)
3. Create an API key
4. Copy the key

### Step 2: Configure the Key

**Option A: Environment Variable (Recommended for Production)**
```bash
export Groq__ApiKey="your-groq-api-key-here"
```

**Option B: appsettings.json (For Local Testing)**
Edit `IsLink.API/appsettings.json`:
```json
{
  "Groq": {
    "ApiKey": "ygsk_ZUqeRiutzhc9zJ4YGT9DWGdyb3FYVoPkatBhkSNYS6H7bctVaMK8"
  }
}
```

### Step 3: Restart Server
The server will automatically use Groq as the primary provider, with Gemini as fallback.

### Step 4: Verify
```bash
curl http://localhost:5001/api/linkerai/status
```

You should see:
```json
{
  "success": true,
  "providerMode": "auto",
  "geminiConfigured": true,
  "groqConfigured": true,
  ...
}
```

## Alternative Solutions

### Option 2: Get New Gemini API Key
1. Create new Google Cloud project
2. Enable Gemini API
3. Get new API key
4. Free tier resets daily

### Option 3: Wait for Gemini Quota Reset
- Free tier quota resets daily
- Check: https://ai.dev/usage?tab=rate-limit

## Current Provider Order
The code automatically tries providers in this order:
1. **Groq** (if configured) ← **Use this!**
2. **Gemini** (if configured)
3. **Fallback** (deterministic matching)

## Quick Test After Setup
```bash
# Start server with Groq
export Groq__ApiKey="your-key"
export AI_PROVIDER="auto"  # or "groq" to force Groq only
cd IsLink.API
dotnet run
```

Then test at: http://localhost:5173/linkerai

