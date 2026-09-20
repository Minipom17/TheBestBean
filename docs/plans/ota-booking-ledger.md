# Plan: OTA booking ledger (minute-level sync)

**Status:** planned — build tomorrow  
**Not building now.** No channel manager (Bókun/Rezdy). No real-time Viator hold API.

## Goal

One capacity source of truth so Viator / GetYourGuide / direct site bookings don’t double-sell seats — updates on the order of **~1 minute**, not checkout-instant.

## Source of truth

Existing `ExperienceSlot`:

- `Capacity`
- `BookedCount`
- `Remaining = Capacity - BookedCount`

Direct site checkout already decrements (or should only sell when `Remaining > 0`).

## Architecture

```
Viator / GYG / email / admin form
        │  (~1 min ingest)
        ▼
  OtaBooking ledger (idempotent by external ref)
        │
        ▼
  ExperienceSlot.BookedCount += guests
        │
        ▼
  purplebean.coffee only offers Remaining > 0
```

Accept rare same-minute race across channels; reconcile by hand or light oversell policy.

## Build tomorrow (ordered)

### 1. Data model

- `OtaBooking` (or `ExternalBooking`):
  - `Source` — `viator` | `gyg` | `direct` | `manual` | …
  - `ExternalRef` — OTA booking code (unique with Source)
  - `ExperienceId`, `ExperienceSlotId` (nullable until matched)
  - `StartAt`, `GuestCount`
  - `Status` — `confirmed` | `cancelled` | `amended`
  - `RawPayload` / note (optional)
  - `CreatedAt`, `UpdatedAt`
- Unique index on `(Source, ExternalRef)` for idempotent ingest.

### 2. Ledger service

- `ApplyBookingAsync` — match slot (experience + time), increment `BookedCount`, store row.
- `CancelBookingAsync` — decrement if previously applied; mark cancelled.
- Never double-apply same `(Source, ExternalRef)`.

### 3. Admin UI (first usable slice)

- “Log OTA booking” form: source, ref, experience, date/time, guests.
- List recent external bookings + link to slot.
- Enough to run the business tomorrow without APIs.

### 4. Site checkout guard

- Confirm cart/checkout refuses slots with `Remaining == 0`.
- Show clear “sold out” on Tour/Experiences calendars.

### 5. Later (not day one)

- Email forward ingest (Viator/GYG confirmation → parse → ledger).
- Poll/export if a supplier export exists.
- Full Supplier API (availability/hold) only if volume justifies certification.

## Explicitly out of scope (for now)

- Paying Bókun/Rezdy channel fees.
- Real-time availability check during OTA checkout funnel.
- Scraping supplier portals.

## Success criteria

- One admin action logs an OTA booking and frees/blocks the matching slot on the site within the next page load.
- Same OTA ref applied twice does not double-count guests.
- Cancel reverses capacity.

## Notes

- OTAs still take ~20–30% commission; this only avoids a second software cut and overbooking.
- Push direct (site + WhatsApp) to shrink OTA share over time.
