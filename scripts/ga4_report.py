#!/usr/bin/env python3
"""Pull Purple Bean GA4 traffic as JSON for agents or a weekly read.

Setup (one time):
  1. Google Cloud Console → create/select a project
  2. Enable “Google Analytics Data API”
  3. IAM → Service Accounts → create purplebean-ga4-reader → JSON key
     Save the key as secrets/ga4-service-account.json (gitignored)
  4. analytics.google.com → Admin → Property access management
     Add the service account email as Viewer
  5. Admin → Property settings → copy the numeric Property ID
     Put it in scripts/ga4.env (see ga4.env.example)

  pip install -r scripts/requirements-ga4.txt
  python scripts/ga4_report.py --days 7
"""

from __future__ import annotations

import argparse
import json
import os
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def load_env_file(path: Path) -> None:
    if not path.is_file():
        return
    for raw in path.read_text(encoding="utf-8").splitlines():
        line = raw.strip()
        if not line or line.startswith("#") or "=" not in line:
            continue
        key, value = line.split("=", 1)
        key, value = key.strip(), value.strip().strip('"').strip("'")
        os.environ.setdefault(key, value)


def resolve_credentials() -> Path | None:
    raw = os.environ.get("GOOGLE_APPLICATION_CREDENTIALS", "").strip()
    candidates = []
    if raw:
        candidates.append(Path(raw))
        if not Path(raw).is_absolute():
            candidates.append(ROOT / raw)
    candidates.append(ROOT / "secrets" / "ga4-service-account.json")
    for path in candidates:
        if path.is_file():
            os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = str(path)
            return path
    return None


def row_dict(row, dimension_names: list[str], metric_names: list[str]) -> dict:
    out: dict = {}
    for i, name in enumerate(dimension_names):
        out[name] = row.dimension_values[i].value
    for i, name in enumerate(metric_names):
        value = row.metric_values[i].value
        out[name] = int(value) if value.isdigit() else float(value) if value else 0
    return out


def run_table(
    client,
    property_id: str,
    start: str,
    end: str,
    dimensions: list[str],
    metrics: list[str],
    limit: int = 15,
    event_name: str | None = None,
):
    from google.analytics.data_v1beta.types import (
        DateRange,
        Dimension,
        Filter,
        FilterExpression,
        Metric,
        RunReportRequest,
    )

    request = RunReportRequest(
        property=f"properties/{property_id}",
        dimensions=[Dimension(name=name) for name in dimensions],
        metrics=[Metric(name=name) for name in metrics],
        date_ranges=[DateRange(start_date=start, end_date=end)],
        limit=limit,
    )
    if event_name:
        request.dimension_filter = FilterExpression(
            filter=Filter(
                field_name="eventName",
                string_filter=Filter.StringFilter(
                    value=event_name,
                    match_type=Filter.StringFilter.MatchType.EXACT,
                ),
            )
        )
    response = client.run_report(request)
    rows = [row_dict(row, dimensions, metrics) for row in response.rows]
    totals = {}
    if response.totals:
        for i, name in enumerate(metrics):
            value = response.totals[0].metric_values[i].value
            totals[name] = int(value) if value.isdigit() else float(value) if value else 0
    return {"rows": rows, "totals": totals}


def run_realtime(client, property_id: str, dimensions: list[str], metrics: list[str], limit: int = 25, event_name: str | None = None):
    from google.analytics.data_v1beta.types import (
        Dimension,
        Filter,
        FilterExpression,
        Metric,
        RunRealtimeReportRequest,
    )

    request = RunRealtimeReportRequest(
        property=f"properties/{property_id}",
        dimensions=[Dimension(name=name) for name in dimensions],
        metrics=[Metric(name=name) for name in metrics],
        limit=limit,
    )
    if event_name:
        request.dimension_filter = FilterExpression(
            filter=Filter(
                field_name="eventName",
                string_filter=Filter.StringFilter(
                    value=event_name,
                    match_type=Filter.StringFilter.MatchType.EXACT,
                ),
            )
        )
    response = client.run_realtime_report(request)
    return [row_dict(row, dimensions, metrics) for row in response.rows]


def main() -> int:
    parser = argparse.ArgumentParser(description="Fetch Purple Bean GA4 traffic as JSON.")
    parser.add_argument("--days", type=int, default=7, help="Lookback window ending yesterday (or today).")
    parser.add_argument(
        "--include-today",
        action="store_true",
        help="End the window at today so same-day WhatsApp clicks are included.",
    )
    parser.add_argument("--pretty", action="store_true", help="Indent JSON for reading.")
    parser.add_argument(
        "--realtime",
        action="store_true",
        help="Last ~30 minutes only (device fingerprint for a live click test).",
    )
    args = parser.parse_args()

    load_env_file(ROOT / "scripts" / "ga4.env")
    creds = resolve_credentials()
    property_id = os.environ.get("GA4_PROPERTY_ID", "").strip()

    if not creds or not property_id or property_id == "000000000":
        sys.stderr.write(
            "GA4 is not wired yet.\n"
            "  1. Enable Google Analytics Data API in Google Cloud\n"
            "  2. Create a service account JSON key -> secrets/ga4-service-account.json\n"
            "  3. Add that email as Viewer on the GA4 property\n"
            "  4. Copy scripts/ga4.env.example to scripts/ga4.env and set GA4_PROPERTY_ID\n"
            "Measurement ID G-TR8742RMGE is the website tag, not the Property ID.\n"
        )
        return 2

    try:
        from google.analytics.data_v1beta import BetaAnalyticsDataClient
    except ImportError:
        sys.stderr.write("Install the client: pip install -r scripts/requirements-ga4.txt\n")
        return 2

    start = f"{args.days}daysAgo"
    end = "today" if args.include_today else "yesterday"
    client = BetaAnalyticsDataClient()

    if args.realtime:
        report = {
            "site": "https://purplebean.coffee",
            "window": "last_30_minutes",
            "active": run_realtime(
                client,
                property_id,
                ["minutesAgo", "deviceCategory", "platform", "city"],
                ["activeUsers"],
                40,
            ),
            "pages": run_realtime(
                client,
                property_id,
                ["unifiedScreenName", "deviceCategory", "city"],
                ["eventCount"],
                20,
            ),
            "events": run_realtime(client, property_id, ["eventName"], ["eventCount"], 25),
            "whatsapp": run_realtime(
                client,
                property_id,
                ["deviceCategory", "platform", "city"],
                ["eventCount"],
                20,
                event_name="whatsapp_click",
            ),
        }
        json.dump(report, sys.stdout, indent=2 if args.pretty else None)
        sys.stdout.write("\n")
        return 0

    totals = run_table(
        client,
        property_id,
        start,
        end,
        dimensions=[],
        metrics=["activeUsers", "sessions", "screenPageViews", "engagementRate", "ecommercePurchases"],
        limit=1,
    )["totals"]
    events = run_table(client, property_id, start, end, ["eventName"], ["eventCount", "activeUsers"], 40)
    report = {
        "site": "https://purplebean.coffee",
        "measurementId": "G-TR8742RMGE",
        "propertyId": property_id,
        "range": {"start": start, "end": end, "days": args.days},
        "totals": totals,
        "cities": run_table(client, property_id, start, end, ["city", "country"], ["activeUsers", "sessions"], 15)["rows"],
        "channels": run_table(client, property_id, start, end, ["sessionDefaultChannelGroup"], ["sessions", "activeUsers"], 12)["rows"],
        "pages": run_table(client, property_id, start, end, ["pagePath"], ["screenPageViews", "activeUsers"], 20)["rows"],
        "events": events["rows"],
        "leadEvents": [
            row for row in events["rows"] if row.get("eventName") in {"whatsapp_click", "generate_lead", "purchase"}
        ],
        "whatsappByDate": run_table(
            client, property_id, start, end, ["date"], ["eventCount", "activeUsers"], 40, event_name="whatsapp_click"
        )["rows"],
        "whatsappByPage": run_table(
            client, property_id, start, end, ["pagePath"], ["eventCount", "activeUsers"], 15, event_name="whatsapp_click"
        )["rows"],
        "whatsappByCity": run_table(
            client,
            property_id,
            start,
            end,
            ["city", "country"],
            ["eventCount", "activeUsers"],
            15,
            event_name="whatsapp_click",
        )["rows"],
        "whatsappByChannel": run_table(
            client,
            property_id,
            start,
            end,
            ["sessionDefaultChannelGroup"],
            ["eventCount", "activeUsers"],
            12,
            event_name="whatsapp_click",
        )["rows"],
        "whatsappByDevice": run_table(
            client,
            property_id,
            start,
            end,
            ["deviceCategory"],
            ["eventCount", "activeUsers"],
            8,
            event_name="whatsapp_click",
        )["rows"],
        "whatsappClicks": run_table(
            client,
            property_id,
            start,
            end,
            ["date", "deviceCategory", "operatingSystem", "browser", "city", "pagePath"],
            ["eventCount", "activeUsers"],
            40,
            event_name="whatsapp_click",
        )["rows"],
    }

    json.dump(report, sys.stdout, indent=2 if args.pretty else None)
    sys.stdout.write("\n")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
