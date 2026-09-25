(function () {
    const stage = document.getElementById('talk-stage');
    if (!stage) return;

    const slides = Array.from(document.querySelectorAll('.talk-slide'));
    const numEl = document.querySelector('[data-talk-num]');
    const maps = new Map();
    let index = 0;
    let geo = { world: null, departments: null, provinces: null, districts: null };

    const ink = '#271825';
    const paper = '#F3EEE6';
    const stone = '#E4DCD0';
    const purple = '#722EA8';
    const plum = '#3B1464';
    const south = '#8E56B8';
    const soft = '#CDB6E0';

    const coffeeFill = {
        piura: '#E6B325',
        cajamarca: '#E07A3D',
        amazonas: '#2F6B4F',
        'san martin': '#7CB342',
        huanuco: '#1AA6A6',
        pasco: '#3D8BFF',
        junin: '#5B4BDB',
        ayacucho: '#C45C26',
        cusco: '#722EA8',
        puno: '#C23B6E'
    };

    const labelSide = {
        piura: 'left',
        cajamarca: 'left',
        'san martin': 'left',
        pasco: 'left',
        ayacucho: 'left',
        amazonas: 'right',
        huanuco: 'right',
        junin: 'right',
        cusco: 'right',
        puno: 'right'
    };
    const labelLat = {
        piura: -4.5,
        cajamarca: -6.15,
        'san martin': -7.75,
        pasco: -10.55,
        ayacucho: -14.25,
        amazonas: -4.35,
        huanuco: -9.15,
        junin: -11.4,
        cusco: -13.3,
        puno: -15.6
    };
    const labelName = {
        piura: 'Piura',
        cajamarca: 'Cajamarca',
        'san martin': 'San Martín',
        pasco: 'Pasco',
        ayacucho: 'Ayacucho',
        amazonas: 'Amazonas',
        huanuco: 'Huánuco',
        junin: 'Junín',
        cusco: 'Cusco',
        puno: 'Puno'
    };

    function fold(s) {
        return (s || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().trim();
    }

    function regionColor(name) {
        const n = fold(name);
        for (const key of Object.keys(coffeeFill)) {
            if (n === key || n.replace('í', 'i') === key) return coffeeFill[key];
        }
        return stone;
    }

    function isCoffeeRegion(name) {
        return regionColor(name) !== stone;
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
        if (geo.departments) return geo;
        const [world, departments, provinces, districts] = await Promise.all([
            fetch('/data/talk/world-belt.geojson').then(function (r) { return r.json(); }),
            fetch('/data/talk/peru-departments.geojson').then(function (r) { return r.json(); }),
            fetch('/data/talk/peru-provinces.geojson').then(function (r) { return r.json(); }),
            fetch('/data/talk/peru-districts.geojson').then(function (r) { return r.json(); })
        ]);
        geo = { world: world, departments: departments, provinces: provinces, districts: districts };
        return geo;
    }

    function baseMap(el) {
        const map = L.map(el, {
            zoomControl: false,
            attributionControl: false,
            scrollWheelZoom: false,
            dragging: true,
            zoomSnap: 0,
            zoomDelta: 0.5,
            renderer: L.svg()
        });
        map.setView([0, 0], 2, { animate: false });
        return map;
    }

    function featureName(feature) {
        const p = feature.properties || {};
        return p.name || p.shapeName || p.NAME_1 || p.NAME_2 || '';
    }

    function armFit(map, el, bounds, padding) {
        let tries = 0;
        el._talkFit = function () {
            map.invalidateSize({ animate: false, pan: false });
            const size = map.getSize();
            if ((!size || size.x < 80 || size.y < 80) && tries < 15) {
                tries += 1;
                requestAnimationFrame(el._talkFit);
                return;
            }
            map.fitBounds(bounds, { animate: false, padding: padding || [20, 20] });
        };
        el._talkFit();
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
        map.invalidateSize({ animate: false, pan: false });
        if (typeof el._talkFit === 'function') el._talkFit();
    }

    const beltColor = {
        MEX: '#E24B4B', GTM: '#F08A3C', SLV: '#E6B325', HND: '#7CB342',
        NIC: '#2E9B6A', CRI: '#1AA6A6', PAN: '#3D8BFF', COL: '#5B4BDB',
        VEN: '#8E56B8', ECU: '#C45C26', PER: '#722EA8', BRA: '#D4A017',
        BOL: '#C23B6E', ETH: '#2F6B4F', KEN: '#E07A3D', RWA: '#3E7CB1',
        UGA: '#C9A227', TZA: '#6B4C9A', YEM: '#B85C38', IND: '#E25B5B',
        VNM: '#3AA76D', IDN: '#2A6FDB', PNG: '#7A4E2D'
    };
    const zoomColor = {
        central: '#5B4BDB',
        huanuco: '#1AA6A6',
        north: '#E07A3D',
        convencion: '#722EA8',
        sandia: '#C23B6E'
    };
    const zoomPale = {
        central: '#DDD8F6',
        huanuco: '#D2EFEF',
        north: '#F8DCCB',
        convencion: '#E7D6F4',
        sandia: '#F6D5E2'
    };

    const provinceSets = {
        central: ['chanchamayo', 'satipo', 'oxapampa'],
        huanuco: ['leoncio prado'],
        north: ['jaen', 'san ignacio', 'rodriguez de mendoza', 'chachapoyas', 'utcubamba'],
        convencion: ['la convencion'],
        sandia: ['sandia']
    };

    const parentDepartments = {
        central: ['junin', 'pasco'],
        huanuco: ['huanuco'],
        north: ['cajamarca', 'amazonas'],
        convencion: ['cusco'],
        sandia: ['puno']
    };

    function paint(map, mode, el) {
        if (mode === 'belt') {
            const tropic = 23.436;
            L.geoJSON(geo.world, {
                smoothFactor: 0.35,
                style: function (f) {
                    const iso = (f.properties && (f.properties['ISO3166-1-Alpha-3'] || f.properties.ISO_A3)) || '';
                    const fill = beltColor[iso];
                    if (fill) {
                        return { color: '#ffffff', weight: 0.4, fillColor: fill, fillOpacity: 1 };
                    }
                    return { color: '#e6e6e6', weight: 0.35, fillColor: '#ffffff', fillOpacity: 1 };
                }
            }).addTo(map);
            const tropicLine = { color: ink, weight: 0.6, opacity: 0.55, interactive: false };
            L.polyline([[tropic, -175], [tropic, 175]], tropicLine).addTo(map);
            L.polyline([[-tropic, -175], [-tropic, 175]], tropicLine).addTo(map);
            const beltTag = function (latlng, text) {
                L.marker(latlng, {
                    interactive: false,
                    keyboard: false,
                    icon: L.divIcon({
                        className: 'talk-belt-tag',
                        html: text,
                        iconSize: [240, 14],
                        iconAnchor: [0, 7]
                    })
                }).addTo(map);
            };
            beltTag([tropic, -120], 'Tropic of Cancer · 23.5°N');
            beltTag([-tropic, -120], 'Tropic of Capricorn · 23.5°S');
            L.marker([-19.6, -75.2], {
                interactive: false,
                keyboard: false,
                icon: L.divIcon({
                    className: 'talk-belt-tag talk-belt-tag--peru',
                    html: 'Peru',
                    iconSize: [72, 16],
                    iconAnchor: [36, 0]
                })
            }).addTo(map);
            armFit(map, el, [[-38, -130], [38, 160]], [28, 8]);
            return;
        }

        if (mode === 'country' || mode === 'regions') {
            L.geoJSON(geo.departments, {
                smoothFactor: 0,
                style: function (f) {
                    const name = featureName(f);
                    const active = isCoffeeRegion(name);
                    return {
                        color: '#271825',
                        weight: 0.4,
                        opacity: 0.45,
                        fillColor: active ? regionColor(name) : '#ffffff',
                        fillOpacity: 1
                    };
                },
                onEachFeature: function (f, layer) {
                    const key = fold(featureName(f));
                    if (!labelSide[key]) return;
                    const side = labelSide[key];
                    const bounds = layer.getBounds();
                    const mid = bounds.getCenter();
                    const label = L.latLng(labelLat[key] || mid.lat, side === 'left' ? -82.55 : -67.15);
                    const edge = L.latLng(mid.lat, side === 'left' ? bounds.getWest() : bounds.getEast());
                    L.polyline([label, edge], {
                        color: ink,
                        weight: 0.6,
                        opacity: 0.55,
                        interactive: false
                    }).addTo(map);
                    L.marker(label, {
                        interactive: false,
                        keyboard: false,
                        icon: L.divIcon({
                            className: 'talk-callout talk-callout--' + side,
                            html: labelName[key] || featureName(f),
                            iconSize: [120, 16],
                            iconAnchor: side === 'left' ? [124, 8] : [-8, 8]
                        })
                    }).addTo(map);
                }
            }).addTo(map);
            armFit(map, el, [[-18.36, -81.33], [-0.02, -68.65]], [12, 88]);
            return;
        }

        const names = provinceSets[mode] || [];
        const parents = parentDepartments[mode] || [];
        const country = L.geoJSON(geo.departments, {
            smoothFactor: 0,
            interactive: false,
            style: function (f) {
                const here = parents.indexOf(fold(featureName(f))) !== -1;
                return {
                    color: here ? '#271825' : '#d0d0d0',
                    weight: here ? 1.15 : 0.45,
                    fillColor: here ? '#f7f7f7' : '#ffffff',
                    fillOpacity: 1
                };
            }
        }).addTo(map);
        const provinces = L.geoJSON(geo.provinces, {
            smoothFactor: 0,
            interactive: false,
            filter: function (f) {
                return names.indexOf(fold(featureName(f))) !== -1;
            },
            style: function () {
                return {
                    color: '#271825',
                    weight: 1.35,
                    fillColor: zoomPale[mode] || '#f3f3f3',
                    fillOpacity: 1
                };
            }
        }).addTo(map);
        L.geoJSON(geo.districts, {
            smoothFactor: 0,
            filter: function (f) {
                if (names.indexOf(fold(f.properties && f.properties.province)) === -1) return false;
                const role = (f.properties && f.properties.role) || 'district';
                return role === 'gem' || role === 'hub';
            },
            style: function () {
                return {
                    color: '#ffffff',
                    weight: 0.7,
                    fillColor: zoomColor[mode] || purple,
                    fillOpacity: 1
                };
            },
            interactive: false
        }).addTo(map);

        const home = country.getLayers().filter(function (layer) {
            return parents.indexOf(fold(featureName(layer.feature))) !== -1;
        });
        const frame = home.length ? L.featureGroup(home).getBounds() : (provinces.getLayers().length ? provinces.getBounds() : null);
        if (frame) {
            armFit(map, el, frame.pad(0.18), [32, 32]);
        }
        addLocator(el, mode);
    }

    function addLocator(el, mode) {
        let box = el.querySelector('.talk-locator');
        if (!box) {
            box = document.createElement('div');
            box.className = 'talk-locator';
            box.setAttribute('aria-hidden', 'true');
            el.appendChild(box);
        }
        if (box._mini) {
            box._mini.invalidateSize();
            return;
        }
        const mini = L.map(box, {
            zoomControl: false,
            attributionControl: false,
            dragging: false,
            scrollWheelZoom: false,
            doubleClickZoom: false,
            boxZoom: false,
            keyboard: false,
            zoomSnap: 0,
            renderer: L.svg()
        });
        mini.setView([-9.2, -75], 4, { animate: false });
        box._mini = mini;
        const parents = parentDepartments[mode] || [];
        const names = provinceSets[mode] || [];
        L.geoJSON(geo.departments, {
            smoothFactor: 0.8,
            interactive: false,
            style: function (f) {
                const here = parents.indexOf(fold(featureName(f))) !== -1;
                return {
                    color: here ? '#271825' : '#bdbdbd',
                    weight: here ? 1.15 : 0.4,
                    fillColor: '#f3f3f3',
                    fillOpacity: 1
                };
            }
        }).addTo(mini);
        L.geoJSON(geo.provinces, {
            smoothFactor: 0.6,
            interactive: false,
            filter: function (f) {
                return names.indexOf(fold(featureName(f))) !== -1;
            },
            style: function () {
                return {
                    color: '#271825',
                    weight: 0.6,
                    fillColor: zoomColor[mode] || purple,
                    fillOpacity: 1
                };
            }
        }).addTo(mini);
        if (!box.querySelector('.talk-locator__cap')) {
            const cap = document.createElement('div');
            cap.className = 'talk-locator__cap';
            cap.textContent = 'Peru';
            box.appendChild(cap);
        }
        let miniTries = 0;
        const fitMini = function () {
            mini.invalidateSize({ animate: false, pan: false });
            const size = mini.getSize();
            if ((!size || size.x < 40 || size.y < 40) && miniTries < 20) {
                miniTries += 1;
                requestAnimationFrame(fitMini);
                return;
            }
            mini.fitBounds([[-18.36, -81.33], [-0.02, -68.65]], { animate: false, padding: [6, 6] });
        };
        fitMini();
        setTimeout(fitMini, 250);
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

    loadGeo();
    go(0);
    stage.focus();
})();
