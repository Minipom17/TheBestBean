(function () {
    const root = document.querySelector('.booking-cal');
    if (!root) return;

    const slotIdInput = document.getElementById('slotId');
    const reserveBtn = document.getElementById('reserve-btn');
    const guests = document.getElementById('participants');
    const grid = root.querySelector('[data-cal-grid]');
    const timesEl = root.querySelector('[data-cal-times]');
    const monthEl = root.querySelector('[data-cal-month]');
    const pickedEl = root.querySelector('[data-cal-picked]');
    let slots = [];
    try { slots = JSON.parse(root.getAttribute('data-slots') || '[]'); } catch (e) { slots = []; }

    const byDay = {};
    slots.forEach(function (s) {
        (byDay[s.day] || (byDay[s.day] = [])).push(s);
    });

    const daysWithSlots = Object.keys(byDay).sort();
    let view = daysWithSlots.length ? parseDay(daysWithSlots[0]) : new Date();
    let selectedDay = daysWithSlots[0] || null;

    function parseDay(key) {
        const parts = (key || '').split('-').map(Number);
        return new Date(parts[0], (parts[1] || 1) - 1, parts[2] || 1);
    }

    function ymd(d) {
        return d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0');
    }

    function monthHasSlots(year, month) {
        const prefix = year + '-' + String(month + 1).padStart(2, '0') + '-';
        return daysWithSlots.some(function (day) { return day.indexOf(prefix) === 0; });
    }

    function setSlot(slot) {
        if (!slotIdInput || !reserveBtn) return;
        slotIdInput.value = slot ? slot.id : '';
        reserveBtn.disabled = !slot;
        reserveBtn.textContent = slot ? ('Reserve · ' + slot.time) : 'Choose a date to reserve';
        if (pickedEl) {
            pickedEl.textContent = slot
                ? slot.label + ' · ' + slot.remaining + ' seat' + (slot.remaining === 1 ? '' : 's') + ' left'
                : '';
        }
        if (guests && slot) {
            const cap = Number(guests.getAttribute('data-max')) || 6;
            const max = Math.min(cap, Math.max(1, slot.remaining || 1));
            Array.from(guests.options).forEach(function (opt) {
                opt.disabled = Number(opt.value) > max;
            });
            if (Number(guests.value) > max) guests.value = String(max);
        }
    }

    function renderTimes(day) {
        const list = byDay[day] || [];
        if (!timesEl) return;
        if (!list.length) {
            timesEl.innerHTML = '<p class="text-[10px] text-v-gray m-0">No open times that day — try a purple date.</p>';
            setSlot(null);
            return;
        }
        timesEl.innerHTML = list.map(function (s) {
            return '<button type="button" class="booking-cal__time" data-slot-id="' + s.id + '">' + s.time + '<span>' + s.remaining + ' left</span></button>';
        }).join('');
        timesEl.querySelectorAll('[data-slot-id]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                timesEl.querySelectorAll('.booking-cal__time').forEach(function (b) { b.classList.remove('is-on'); });
                btn.classList.add('is-on');
                const slot = list.find(function (s) { return String(s.id) === btn.getAttribute('data-slot-id'); });
                setSlot(slot);
            });
        });
        timesEl.querySelector('.booking-cal__time')?.click();
    }

    function render() {
        const year = view.getFullYear();
        const month = view.getMonth();
        if (monthEl) {
            monthEl.textContent = view.toLocaleString('en-GB', { month: 'long', year: 'numeric' });
        }
        const first = new Date(year, month, 1);
        const startWeekday = (first.getDay() + 6) % 7;
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const todayKey = daysWithSlots[0] ? daysWithSlots[0] : ymd(new Date());
        const cells = [];
        for (var i = 0; i < startWeekday; i++) cells.push('<span class="booking-cal__day is-empty"></span>');
        for (var d = 1; d <= daysInMonth; d++) {
            const date = new Date(year, month, d);
            const key = ymd(date);
            const open = !!byDay[key];
            const on = key === selectedDay;
            const past = key < todayKey && !open;
            const cls = ['booking-cal__day'];
            if (open) cls.push('has-slot');
            if (on) cls.push('is-on');
            if (past) cls.push('is-past');
            if (!open && !past) cls.push('is-closed');
            cells.push('<button type="button" class="' + cls.join(' ') + '"' + (open ? ' data-day="' + key + '"' : ' disabled') + '>' + d + '</button>');
        }
        if (grid) {
            grid.innerHTML = cells.join('');
            grid.querySelectorAll('[data-day]').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    selectedDay = btn.getAttribute('data-day');
                    render();
                    renderTimes(selectedDay);
                });
            });
        }
        root.querySelectorAll('[data-cal-nav]').forEach(function (btn) {
            const dir = Number(btn.getAttribute('data-cal-nav'));
            const probe = new Date(year, month + dir, 1);
            btn.disabled = !monthHasSlots(probe.getFullYear(), probe.getMonth());
        });
    }

    root.querySelectorAll('[data-cal-nav]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const next = new Date(view.getFullYear(), view.getMonth() + Number(btn.getAttribute('data-cal-nav')), 1);
            if (Number(btn.getAttribute('data-cal-nav')) < 0 && !monthHasSlots(next.getFullYear(), next.getMonth())) return;
            view = next;
            render();
        });
    });

    if (!slots.length && timesEl) {
        timesEl.innerHTML = '<p class="text-sm m-0">No open seats online right now. <a href="https://wa.me/51993779381">WhatsApp us</a> for a date.</p>';
        setSlot(null);
        render();
        return;
    }

    render();
    if (selectedDay) renderTimes(selectedDay);
})();
