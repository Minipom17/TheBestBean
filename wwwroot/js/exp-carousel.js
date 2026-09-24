(function () {
    function moveCarousel(id, direction, evt) {
        if (evt) {
            evt.preventDefault();
            evt.stopPropagation();
        }

        const container = document.getElementById('carousel-' + id);
        if (!container) return;

        const slides = container.querySelectorAll('.exp-carousel__slide');
        const images = slides.length ? slides : container.querySelectorAll('img');
        const total = images.length;
        if (total <= 1) return;

        let current = parseInt(container.getAttribute('data-current') || '0', 10);
        current = (current + direction + total) % total;
        container.setAttribute('data-current', current.toString());
        container.classList.remove('is-dragging');
        container.style.transform = `translateX(-${current * 100}%)`;
    }

    function bindCarouselSwipe(frame) {
        const id = frame.getAttribute('data-carousel-id');
        const container = document.getElementById('carousel-' + id);
        if (!container) return;

        const slides = container.querySelectorAll('.exp-carousel__slide');
        const images = slides.length ? slides : container.querySelectorAll('img');
        if (images.length <= 1) return;

        let startX = 0;
        let startY = 0;
        let deltaX = 0;
        let tracking = false;
        let horizontal = null;
        let pointerId = null;

        const width = () => frame.getBoundingClientRect().width || 1;

        const onPointerDown = (e) => {
            if (e.pointerType === 'mouse' && e.button !== 0) return;
            if (e.target.closest && e.target.closest('.exp-carousel__btn')) return;
            tracking = true;
            horizontal = null;
            pointerId = e.pointerId;
            startX = e.clientX;
            startY = e.clientY;
            deltaX = 0;
            container.classList.add('is-dragging');
            try { frame.setPointerCapture(e.pointerId); } catch (_) {}
        };

        const onPointerMove = (e) => {
            if (!tracking || e.pointerId !== pointerId) return;
            deltaX = e.clientX - startX;
            const deltaY = e.clientY - startY;

            if (horizontal === null) {
                if (Math.abs(deltaX) < 8 && Math.abs(deltaY) < 8) return;
                horizontal = Math.abs(deltaX) > Math.abs(deltaY);
                if (!horizontal) {
                    tracking = false;
                    container.classList.remove('is-dragging');
                    container.style.transform = `translateX(-${parseInt(container.getAttribute('data-current') || '0', 10) * 100}%)`;
                    try { frame.releasePointerCapture(e.pointerId); } catch (_) {}
                    return;
                }
            }

            if (!horizontal) return;
            e.preventDefault();
            const current = parseInt(container.getAttribute('data-current') || '0', 10);
            const pct = (deltaX / width()) * 100;
            container.style.transform = `translateX(calc(-${current * 100}% + ${pct}%))`;
        };

        const finish = (e) => {
            if (!tracking || (e && e.pointerId !== pointerId)) return;
            tracking = false;
            container.classList.remove('is-dragging');
            try { frame.releasePointerCapture(pointerId); } catch (_) {}

            const threshold = Math.min(56, width() * 0.18);
            if (horizontal && Math.abs(deltaX) >= threshold) {
                moveCarousel(id, deltaX < 0 ? 1 : -1);
            } else {
                const current = parseInt(container.getAttribute('data-current') || '0', 10);
                container.style.transform = `translateX(-${current * 100}%)`;
            }
            horizontal = null;
            deltaX = 0;
            pointerId = null;
        };

        frame.addEventListener('pointerdown', onPointerDown);
        frame.addEventListener('pointermove', onPointerMove, { passive: false });
        frame.addEventListener('pointerup', finish);
        frame.addEventListener('pointercancel', finish);
        frame.addEventListener('lostpointercapture', finish);
    }

    function initCarousels() {
        document.querySelectorAll('.exp-carousel[data-carousel-id]').forEach(bindCarouselSwipe);
    }

    window.moveCarousel = moveCarousel;
    window.bindCarouselSwipe = bindCarouselSwipe;
    window.initExpCarousels = initCarousels;

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initCarousels);
    } else {
        initCarousels();
    }
})();
