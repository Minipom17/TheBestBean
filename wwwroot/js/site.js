(function () {
    function rowHeight(feed) {
        var row = feed.querySelector(".lab-board__row");
        return row ? row.getBoundingClientRect().height : 34.4;
    }

    function syncCoffeeLabFeed() {
        var intro = document.querySelector(".coffee-intro");
        if (!intro) return;
        var details = intro.querySelector(".how-roast__details");
        var copy = intro.querySelector(".how-roast__copy");
        var feed = intro.querySelector(".lab-board__feed");
        var head = intro.querySelector(".lab-board__head");
        if (!feed || !copy) return;

        var six = rowHeight(feed) * 6;
        if (details && details.open) {
            var copyH = copy.getBoundingClientRect().height;
            var headH = head ? head.getBoundingClientRect().height : 0;
            feed.style.maxHeight = Math.max(six, copyH - headH) + "px";
        } else {
            feed.style.maxHeight = six + "px";
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        var details = document.querySelector(".coffee-intro .how-roast__details");
        if (details) {
            details.addEventListener("toggle", syncCoffeeLabFeed);
        }
        window.addEventListener("resize", syncCoffeeLabFeed);
        syncCoffeeLabFeed();
    });
})();

(function () {
    function fmtLotDate(iso) {
        if (!iso) return "—";
        var d = new Date(iso + "T12:00:00");
        if (isNaN(d.getTime())) return "—";
        return d.toLocaleDateString(undefined, { day: "numeric", month: "short", year: "numeric" });
    }

    function fillLotSheet(data, focus) {
        var dialog = document.getElementById("lot-sheet");
        if (!dialog || !data) return;
        dialog.querySelector("[data-lot-title]").textContent = data.name || "Coffee lot";
        var bits = [data.farmer, data.variety, data.process, data.region].filter(Boolean);
        dialog.querySelector("[data-lot-meta]").textContent = bits.join(" · ");
        dialog.querySelector("[data-lot-warehouse]").textContent = data.warehouse || "Cusco";
        var trail = dialog.querySelector("[data-lot-trail]");
        trail.innerHTML =
            "<dt>Harvested</dt><dd>" + fmtLotDate(data.harvested) + "</dd>" +
            "<dt>Fermented</dt><dd>" + fmtLotDate(data.fermented) + "</dd>" +
            "<dt>Dried</dt><dd>" + fmtLotDate(data.dried) + "</dd>" +
            "<dt>Arrived Cusco</dt><dd>" + fmtLotDate(data.arrived) + "</dd>";
        var green = dialog.querySelector("[data-lot-green]");
        var roast = dialog.querySelector("[data-lot-roast]");
        green.classList.toggle("is-focus", focus === "green");
        roast.classList.toggle("is-focus", focus === "roast");
        var list = dialog.querySelector("[data-lot-roasts]");
        var roasts = data.roasts || [];
        if (roasts.length === 0) {
            list.innerHTML = "<p class=\"lot-sheet__empty\">No roast on the books yet — we roast after you order.</p>";
        } else {
            list.innerHTML = roasts.map(function (r) {
                var right = [r.kg, r.profile, r.phase].filter(Boolean).join(" · ");
                return "<div class=\"lot-sheet__roast\"><span>" + fmtLotDate(r.date) + "</span><span>" + right + "</span></div>";
            }).join("");
        }
        if (typeof dialog.showModal === "function") {
            dialog.showModal();
        }
    }

    document.addEventListener("click", function (e) {
        var close = e.target.closest("[data-lot-close]");
        if (close) {
            var dialog = close.closest("dialog");
            if (dialog) dialog.close();
            return;
        }
        var btn = e.target.closest(".lot-sheet-open");
        if (!btn) return;
        e.preventDefault();
        e.stopPropagation();
        var raw = btn.getAttribute("data-lot");
        if (!raw) return;
        try {
            fillLotSheet(JSON.parse(raw), btn.getAttribute("data-lot-focus") || "green");
        } catch (err) {
            console.error("Lot sheet JSON", err);
        }
    });

    document.addEventListener("click", function (e) {
        var dialog = document.getElementById("lot-sheet");
        if (!dialog || !dialog.open) return;
        if (e.target === dialog) dialog.close();
    });
})();
