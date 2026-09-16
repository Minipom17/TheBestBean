---
name: ga4-traffic
description: Pull Purple Bean Google Analytics 4 traffic via scripts/ga4_report.py and summarize it. Use when the user asks about site traffic, GA4, cities, channels, page views, WhatsApp leads, or weekly engagement.
---

# Purple Bean GA4 traffic

Tracking is already on the live site (`G-TR8742RMGE` in `appsettings.json`). Reading it uses the Data API, not the web tag.

## Fetch

```bash
python scripts/ga4_report.py --days 7 --pretty
```

Pass `--days 28` for a month. Pipe JSON into the reply; do not dump raw tables unless asked.

If the script exits 2, the service account is not set up yet. Tell the user to:

1. Enable **Google Analytics Data API** in Google Cloud
2. Create service account `purplebean-ga4-reader`, download JSON → `secrets/ga4-service-account.json`
3. GA4 Admin → Property access → add that email as **Viewer**
4. Copy `scripts/ga4.env.example` → `scripts/ga4.env` and set the numeric **Property ID** (not `G-TR8742RMGE`)

Never print or commit the JSON key or `scripts/ga4.env`.

## Summarize

From the JSON, report:

- Users, sessions, page views vs the window
- Top cities (Cusco / Lima / abroad)
- Channels (organic, direct, social, paid)
- Top pages (`/Experiences`, `/Tour`, `/Coffee`, `/`)
- Lead events: `whatsapp_click`, `generate_lead`, `purchase`

Call out spikes and likely causes (workshop pages, Instagram, WhatsApp). Do not invent numbers if the script failed.