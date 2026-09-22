(function () {
  function qs(sel, root) {
    return (root || document).querySelector(sel);
  }

  function clear(el) {
    while (el && el.firstChild) el.removeChild(el.firstChild);
  }

  function addFact(dl, label, value) {
    if (!value || !String(value).trim()) return;
    var dt = document.createElement("dt");
    dt.textContent = label;
    var dd = document.createElement("dd");
    dd.textContent = value;
    dl.appendChild(dt);
    dl.appendChild(dd);
  }

  function openModal(kind, btn) {
    var modal = qs("#lab-stock-modal");
    if (!modal) return;

    var title = qs("#lab-stock-modal-title", modal);
    var pill = qs("#lab-stock-modal-pill", modal);
    var lede = qs("#lab-stock-modal-lede", modal);
    var facts = qs("#lab-stock-modal-facts", modal);
    var roasts = qs("#lab-stock-modal-roasts", modal);
    var empty = qs("#lab-stock-modal-empty", modal);

    title.textContent = btn.getAttribute("data-name") || "";
    clear(facts);
    clear(roasts);
    roasts.hidden = true;
    empty.hidden = true;
    lede.textContent = "";

    var kg = btn.getAttribute("data-kg") || "";

    if (kind === "green") {
      pill.textContent = "Green bean";
      pill.className = "lab-board__pill lab-board__pill--green";
      lede.textContent = kg + " green in the lab.";
      addFact(facts, "Producer", btn.getAttribute("data-producer"));
      addFact(facts, "Process", btn.getAttribute("data-process"));
      addFact(facts, "Altitude", btn.getAttribute("data-altitude"));
      addFact(facts, "Cup", btn.getAttribute("data-flavor"));
      if (btn.getAttribute("data-has-roast") !== "1") {
        empty.hidden = false;
        empty.textContent = "No roasted stock on hand — we roast after you order and ship the next day.";
      }
    } else {
      pill.textContent = "Roasted";
      pill.className = "lab-board__pill";
      lede.textContent = kg + " roasted on hand.";

      var raw = btn.getAttribute("data-roasts") || "[]";
      var drops = [];
      try {
        drops = JSON.parse(raw);
      } catch (_) {
        drops = [];
      }

      if (!drops.length) {
        empty.hidden = false;
        empty.textContent = "No roast batches logged yet — roast after you order.";
      } else {
        roasts.hidden = false;
        drops.forEach(function (d) {
          var li = document.createElement("li");
          var strong = document.createElement("strong");
          strong.textContent = d.date || (d.daysAgo + "d ago");
          li.appendChild(strong);

          if (d.grams != null) {
            var g = document.createElement("span");
            g.textContent = Math.round(Number(d.grams)) + " g";
            li.appendChild(g);
          }

          if (d.profile) {
            var p = document.createElement("span");
            p.textContent = d.profile;
            li.appendChild(p);
          }

          if (d.phaseLabel) {
            var phase = document.createElement("span");
            phase.className = "lab-board__phase";
            phase.textContent = d.phaseLabel;
            if (d.phaseHint) phase.title = d.phaseHint;
            li.appendChild(phase);
          }

          roasts.appendChild(li);
        });
      }
    }

    modal.hidden = false;
    modal.setAttribute("aria-hidden", "false");
    document.documentElement.classList.add("lab-stock-modal-open");
    var closeBtn = qs("[data-lab-modal-close]", modal);
    if (closeBtn) closeBtn.focus();
  }

  function closeModal() {
    var modal = qs("#lab-stock-modal");
    if (!modal || modal.hidden) return;
    modal.hidden = true;
    modal.setAttribute("aria-hidden", "true");
    document.documentElement.classList.remove("lab-stock-modal-open");
  }

  document.addEventListener("click", function (e) {
    var btn = e.target.closest("[data-lab-popup]");
    if (btn) {
      e.preventDefault();
      openModal(btn.getAttribute("data-lab-popup"), btn);
      return;
    }
    if (e.target.closest("[data-lab-modal-close]")) {
      closeModal();
    }
  });

  document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") closeModal();
  });
})();
