(function () {
    const stage = document.getElementById('talk-stage');
    if (!stage) return;

    const slides = Array.from(document.querySelectorAll('.talk-slide'));
    const numEl = document.querySelector('[data-talk-num]');
    const maps = new Map();
    let index = 0;
    let geo = { world: null, adm1: null, adm2: null };

    const coffeeFill = {
        cusco: '#722EA8',
        cajamarca: '#16A34A',
        junin: '#F05A28',
        'san martin': '#00B4D8',
        amazonas: '#82C341',
        pasco: '#E76F51',
        puno: '#4EA8DE',
        huanuco: '#C2F970',
        piura: '#8B8490',
        ayacucho: '#B5E48C'
    };

    function fold(s) {
        return (s || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().trim();
    }

    function regionColor(name) {
        const n = fold(name);
        for (const key of Object.keys(coffeeFill)) {
            if (n === key || n.replace('í', 'i') === key) return coffeeFill[key];
        }
        return '#e8e2d8';
    }

    function isCoffeeRegion(name) {
        return regionColor(name) !== '#e8e2d8';
    }

    function go(n) {
        index = Math.max(0, Math.min(slides.length - 1, n));
        slides.forEach(function (slide, i) {
            if (i === index) slide.setAttribute('data-on', '');
            else slide.removeAttribute('data-on');
        });
        if (numEl) numEl.textContent = (index + 1) + ' / ' + slides.length;
        const mapEl = slides[index].querySelector('[data-talk-map]');
        if (mapEl) ensureMap(mapEl);
    }

    async function loadGeo() {
        if (geo.adm1) return geo;
        const [world, adm1, adm2] = await Promise.all([
            fetch('/data/world_countries.geojson').then(function (r) { return r.json(); }),
            fetch('/data/peru_adm1.geojson').then(function (r) { return r.json(); }),
            fetch('/data/peru_adm2.geojson').then(function (r) { return r.json(); })
        ]);
        geo = { world: world, adm1: adm1, adm2: adm2 };
        return geo;
    }

    function baseMap(el) {
        const map = L.map(el, {
            zoomControl: false,
            attributionControl: false,
            scrollWheelZoom: false,
            dragging: true
        });
        return map;
    }

    function featureName(feature) {
        const p = feature.properties || {};
        return p.shapeName || p.NAME_1 || p.NAME_2 || p.name || '';
    }

    async function ensureMap(el) {
        const mode = el.getAttribute('data-talk-map');
        await loadGeo();
        let map = maps.get(el);
        if (!map) {
            map = baseMap(el);
            maps.set(el, map);
            paint(map, mode, el);
        }
        setTimeout(function () {
            map.invalidateSize();
            if (typeof el._talkFit === 'function') el._talkFit();
        }, 120);
    }

    function paint(map, mode, el) {
        if (mode === 'belt') {
            const layer = L.geoJSON(geo.world, {
                style: function (f) {
                    const iso = (f.properties && f.properties['ISO3166-1-Alpha-3']) || '';
                    const peru = iso === 'PER';
                    return {
                        color: peru ? '#722EA8' : '#c4bbb0',
                        weight: peru ? 2 : 0.5,
                        fillColor: peru ? '#722EA8' : '#ddd4c8',
                        fillOpacity: 1
                    };
                }
            }).addTo(map);
            const peru = layer.getLayers().find(function (l) {
                return (l.feature.properties['ISO3166-1-Alpha-3'] === 'PER');
            });
            if (peru) {
                el._talkFit = function () { map.fitBounds(peru.getBounds().pad(1.35), { maxZoom: 5 }); };
            } else {
                el._talkFit = function () { map.setView([-10, -75], 4); };
            }
            el._talkFit();
            return;
        }

        const adm1 = L.geoJSON(geo.adm1, {
            style: function (f) {
                const name = featureName(f);
                const active = isCoffeeRegion(name);
                const color = regionColor(name);
                return {
                    color: '#271825',
                    weight: active ? 1.1 : 0.4,
                    fillColor: color,
                    fillOpacity: active ? 0.78 : 0.12
                };
            },
            onEachFeature: function (f, layer) {
                layer.bindTooltip(featureName(f), { sticky: true, opacity: 0.95 });
            }
        }).addTo(map);

        if (mode === 'regions') {
            el._talkFit = function () { map.fitBounds(adm1.getBounds().pad(0.08), { maxZoom: 6 }); };
            el._talkFit();
            return;
        }

        const focus = {
            north: ['cajamarca'],
            center: ['junin'],
            south: ['cusco']
        }[mode];

        if (focus) {
            const hit = adm1.getLayers().find(function (l) {
                return focus.indexOf(fold(featureName(l.feature))) !== -1;
            });
            if (hit) {
                el._talkFit = function () { map.fitBounds(hit.getBounds().pad(0.25), { maxZoom: 8 }); };
                el._talkFit();
            }
        }

        const towns = {
            north: ['jaen', 'san ignacio'],
            center: ['chanchamayo', 'satipo'],
            south: ['la convencion']
        }[mode] || [];

        if (towns.length) {
            L.geoJSON(geo.adm2, {
                filter: function (f) {
                    return towns.indexOf(fold(featureName(f))) !== -1;
                },
                style: function () {
                    return { color: '#111', weight: 1.6, fillColor: '#F5C518', fillOpacity: 0.55 };
                },
                onEachFeature: function (f, layer) {
                    layer.bindTooltip(featureName(f), { sticky: true });
                }
            }).addTo(map);
        }
    }

    document.querySelector('[data-talk-prev]')?.addEventListener('click', function () { go(index - 1); });
    document.querySelector('[data-talk-next]')?.addEventListener('click', function () { go(index + 1); });
    document.querySelector('[data-talk-fs]')?.addEventListener('click', function () {
        if (!document.fullscreenElement) document.documentElement.requestFullscreen();
        else document.exitFullscreen();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowRight' || e.key === ' ' || e.key === 'PageDown') {
            e.preventDefault();
            go(index + 1);
        } else if (e.key === 'ArrowLeft' || e.key === 'PageUp') {
            e.preventDefault();
            go(index - 1);
        } else if (e.key === 'Home') {
            go(0);
        } else if (e.key === 'End') {
            go(slides.length - 1);
        } else if (e.key === 'f' || e.key === 'F') {
            if (!document.fullscreenElement) document.documentElement.requestFullscreen();
            else document.exitFullscreen();
        }
    });

    let touchX = 0;
    stage.addEventListener('touchstart', function (e) {
        touchX = e.changedTouches[0].screenX;
    }, { passive: true });
    stage.addEventListener('touchend', function (e) {
        const dx = e.changedTouches[0].screenX - touchX;
        if (dx < -50) go(index + 1);
        if (dx > 50) go(index - 1);
    }, { passive: true });

    go(0);
    stage.focus();
})();
