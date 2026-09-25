# Booking WhatsApp alerts & cutoffs

## WhatsApp ops alerts

Copy `docs/whatsapp.secrets.example.json` to `secrets/whatsapp.json` on the server (or set env vars).

### CallMeBot (fastest self-notify)

1. On the lab phone, open WhatsApp and message **+34 644 66 78 53**: `I allow callmebot to send me messages`
2. Save the API key it replies with into `secrets/whatsapp.json` → `ApiKey`
3. Restart the app

You get a WhatsApp when:

- Someone with an **experience** in the cart lands on checkout (once per browser session)
- They click **Pay** (Yape / Culqi / PayPal start)
- PayPal capture finishes as **Paid**
- Reminder: evening before a booked session (after 17:00 Lima)
- Reminder: morning of (after 06:30 Lima)

Tourist-looking checkouts (PayPal USD/CAD or non-Peru phone) are prefixed with **TOURIST**.

Optional: set `NotifyEmail` for an SMTP fallback (uses existing `secrets/email.json`).

## Booking cutoffs

| Session (Lima) | Online booking closes |
|---|---|
| Morning (start before 12:00) | End of the **previous calendar day** |
| Afternoon / evening | **4 hours** before start |
| Already has guests | Stays open to fill seats (until 30 min before) |
| Admin **Open now** | Bypass cutoff until start |

Admin → **Booking slots** → **Open now** on a closed time (e.g. 9am, open the noon tour).
