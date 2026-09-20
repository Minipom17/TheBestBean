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
    """Prefer GOOGLE_APPLICATION_CREDENTIALS file; else materialize GA4_SERVICE_ACCOUNT_JSON secret."""
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

    # Cursor / cloud secret: full service-account JSON as env string
    json_blob = os.environ.get("GA4_SERVICE_ACCOUNT_JSON", "").strip()
    if json_blob:
        try:
            parsed = json.loads(json_blob)
        except json.JSONDecodeError:
            sys.stderr.write("GA4_SERVICE_ACCOUNT_JSON is set but is not valid JSON.\n")
            return None
        secrets_dir = ROOT / "secrets"
        secrets_dir.mkdir(parents=True, exist_ok=True)
        out = secrets_dir / "ga4-service-account.json"
        out.write_text(json.dumps(parsed), encoding="utf-8")
        out.chmod(0o600)
        os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = str(out)
        return out
    return None


def row_dict(row, dimension_names: list[str], metric_names: list[str]) -> dict:
    out: dict = {}
    for i, name in enumerate(dimension_names):
        out[name] = row.dimension_values[i].value
    for i, name in enumerate(metric_names):
        value = row.metric_values[i].value
        out[name] = int(value) if value.isdigit() else float(value) if value else 0
    return out


def run_table(client, property_id: str, start: str, end: str, dimensions: list[str], metrics: list[str], limit: int = 15):
    from google.analytics.data_v1beta.types import DateRange, Dimension, Metric, RunReportRequest

    request = RunReportRequest(
        property=f"properties/{property_id}",
        dimensions=[Dimension(name=name) for name in dimensions],
        metrics=[Metric(name=name) for name in metrics],
        date_ranges=[DateRange(start_date=start, end_date=end)],
        limit=limit,
    )
    response = client.run_report(request)
    rows = [row_dict(row, dimensions, metrics) for row in response.rows]
    totals = {}
    if response.totals:
        for i, name in enumerate(metrics):
            value = response.totals[0].metric_values[i].value
            totals[name] = int(value) if value.isdigit() else float(value) if value else 0
    return {"rows": rows, "totals": totals}


def main() -> int:
    parser = argparse.ArgumentParser(description="Fetch Purple Bean GA4 traffic as JSON.")
    parser.add_argument("--days", type=int, default=7, help="Lookback window ending yesterday.")
    parser.add_argument("--pretty", action="store_true", help="Indent JSON for reading.")
    args = parser.parse_args()

    load_env_file(ROOT / "scripts" / "ga4.env")
    creds = resolve_credentials()
    property_id = os.environ.get("GA4_PROPERTY_ID", "").strip()

    if not creds or not property_id or property_id == "000000000":
        sys.stderr.write(
            "GA4 is not wired yet.\n"
            "  1. Enable Google Analytics Data API in Google Cloud\n"
            "  2. Create service account purplebean-ga4-reader → JSON key\n"
            "  3. Add that email as Viewer on the GA4 property\n"
            "  4. Set Cursor secrets GA4_PROPERTY_ID + GA4_SERVICE_ACCOUNT_JSON\n"
            "     (or secrets/ga4-service-account.json + scripts/ga4.env)\n"
            "Measurement ID G-TR8742RMGE is the website tag, not the Property ID.\n"
        )
        return 2

    try:
        from google.analytics.data_v1beta import BetaAnalyticsDataClient
    except ImportError:
        sys.stderr.write("Install the client: pip install -r scripts/requirements-ga4.txt\n")
        return 2

    start = f"{args.days}daysAgo"
    end = "yesterday"
    client = BetaAnalyticsDataClient()

    totals = run_table(
        client,
        property_id,
        start,
        end,
        dimensions=[],
        metrics=["activeUsers", "sessions", "screenPageViews", "engagementRate", "ecommercePurchases"],
        limit=1,
    )["totals"]
    events = run_table(client, property_id, start, end, ["eventName"], ["eventCount", "activeUsers"], 25)
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
    }

    json.dump(report, sys.stdout, indent=2 if args.pretty else None)
    sys.stdout.write("\n")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
