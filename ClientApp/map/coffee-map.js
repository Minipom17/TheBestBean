import L from 'leaflet';

const productionByIso3 = {
    PER: 'arabica', COL: 'arabica', BRA: 'both',
    ECU: 'arabica', BOL: 'arabica', VEN: 'arabica',
    CRI: 'arabica', PAN: 'arabica', NIC: 'arabica', HND: 'arabica', SLV: 'arabica', GTM: 'arabica', MEX: 'arabica',
    ETH: 'arabica', KEN: 'arabica', RWA: 'arabica', UGA: 'robusta', TZA: 'arabica', BDI: 'arabica',
    VNM: 'robusta', IDN: 'both', PHL: 'robusta', PNG: 'arabica', IND: 'robusta', LKA: 'robusta',
    YEM: 'arabica', CMR: 'robusta', COD: 'robusta', CIV: 'robusta', NGA: 'robusta',
    LAO: 'arabica', THA: 'arabica', CHN: 'both',
    PRY: 'arabica', ARG: 'none', CHL: 'none', URY: 'none'
};

const productionByName = {
    'Peru': 'arabica', 'Colombia': 'arabica', 'Brazil': 'both', 'Ecuador': 'arabica', 'Bolivia': 'arabica', 'Venezuela': 'arabica',
    'Costa Rica': 'arabica', 'Panama': 'arabica', 'Nicaragua': 'arabica', 'Honduras': 'arabica', 'El Salvador': 'arabica', 'Guatemala': 'arabica', 'Mexico': 'arabica',
    'Ethiopia': 'arabica', 'Kenya': 'arabica', 'Rwanda': 'arabica', 'Uganda': 'robusta', 'Tanzania': 'arabica', 'Burundi': 'arabica',
    'Vietnam': 'robusta', 'Indonesia': 'both', 'Philippines': 'robusta', 'Papua New Guinea': 'arabica', 'India': 'robusta', 'Sri Lanka': 'robusta', 'Yemen': 'arabica',
    'Cameroon': 'robusta', 'Democratic Republic of the Congo': 'robusta', 'Congo (Kinshasa)': 'robusta', "Cote d'Ivore": 'robusta', "Cote d'Ivoire": 'robusta', 'Ivory Coast': 'robusta', 'Nigeria': 'robusta',
    'Laos': 'arabica', 'Lao PDR': 'arabica', 'Thailand': 'arabica', 'China': 'both', 'Paraguay': 'arabica'
};

const peruAdm1TypeByName = {
    'Cusco': 'arabica',
    'Junín': 'arabica', 'Junin': 'arabica',
    'San Martín': 'arabica', 'San Martin': 'arabica',
    'Cajamarca': 'arabica',
    'Pasco': 'arabica',
    'Amazonas': 'arabica',
    'Puno': 'arabica',
    'Ayacucho': 'arabica',
    'Huánuco': 'arabica', 'Huanuco': 'arabica',
    'Piura': 'arabica',
    'Lambayeque': 'arabica',
    'Ucayali': 'arabica',
    'Loreto': 'arabica',
    'Apurímac': 'arabica', 'Apurimac': 'arabica',
    'La Libertad': 'arabica',
    'Huancavelica': 'arabica',
    'Madre de Dios': 'arabica'
};

const laConvencionNames = new Set(['La Convención', 'La Convencion']);

function colorByType(type) {
    if (type === 'arabica') return '#2e7d32';
    if (type === 'robusta') return '#1565c0';
    if (type === 'both') return '#8e24aa';
    return '#e9eef2';
}

function getIso3FromFeature(feature) {
    const props = feature && feature.properties ? feature.properties : {};
    // Try various ISO3 property formats
    const candidates = [
        feature.id,
        // ISO3166-1-Alpha-3 format (common in some GeoJSON datasets)
        props['ISO3166-1-Alpha-3'], props['iso3166-1-alpha-3'],
        // Natural Earth formats
        props.iso_a3, props.ISO_A3,
        props.adm0_a3, props.ADM0_A3,
        // Other common formats
        props.iso3, props.ISO3,
        props.iso_a3_eh, props.ISO_A3_EH,
        props.WB_A3, props.wb_a3
    ];
    for (const candidate of candidates) {
        if (candidate && typeof candidate === 'string' && candidate.length >= 3 && candidate !== '-99') {
            return candidate.toUpperCase();
        }
    }
    return null;
}

function resolveCountryType(feature) {
    const props = feature && feature.properties ? feature.properties : {};
    const iso = getIso3FromFeature(feature);
    // Try various name property formats (lowercase 'name' is common, also try Natural Earth formats)
    const name = props.name || props.NAME || props.NAME_LONG || props.NAME_EN || props.ADMIN || props.NAME_ENGL || '';
    const type = (iso && productionByIso3[iso]) || productionByName[name] || 'none';
    return { iso: iso || 'N/A', name: name || 'Unknown', type };
}

async function loadPeruAdm(level, geoDataUrl) {
    const url = `${geoDataUrl}&level=${level}&country=PER`;
    const response = await fetch(url, { cache: 'reload' });
    if (!response.ok) {
        throw new Error(`Failed to load Peru ${level.toUpperCase()} data`);
    }
    return await response.json();
}

function createLegendControl() {
    return L.Control.extend({
        onAdd: function () {
            const div = L.DomUtil.create('div', 'leaflet-control legend p-2');
            div.style.background = 'rgba(255,255,255,0.9)';
            div.style.border = '1px solid #c7cdd3';
            div.style.borderRadius = '6px';
            div.style.fontSize = '12px';
            div.innerHTML = `
                <div><strong>Legend</strong></div>
                <div><span style="display:inline-block;width:10px;height:10px;background:#2e7d32;border:1px solid #556;margin-right:6px"></span>Arabica</div>
                <div><span style="display:inline-block;width:10px;height:10px;background:#1565c0;border:1px solid #556;margin-right:6px"></span>Robusta</div>
                <div><span style="display:inline-block;width:10px;height:10px;background:#8e24aa;border:1px solid #556;margin-right:6px"></span>Both</div>
                <hr style="margin:6px 0"/>
                <div>ADM1: Regions</div>
                <div>ADM2: Provinces (click region)</div>`;
            return div;
        },
        onRemove: function () { }
    });
}

function createResetControl(map, beltBounds, regionsLayerRef) {
    return L.Control.extend({
        onAdd: function () {
            const btn = L.DomUtil.create('button');
            btn.className = 'btn btn-sm btn-outline-primary';
            btn.innerText = 'Reset View';
            btn.style.background = 'white';
            btn.style.border = '1px solid #c7cdd3';
            btn.style.borderRadius = '6px';
            L.DomEvent.on(btn, 'click', function (e) {
                L.DomEvent.stopPropagation(e);
                if (regionsLayerRef.layer) {
                    map.removeLayer(regionsLayerRef.layer);
                    regionsLayerRef.layer = null;
                }
                map.fitBounds(beltBounds, { padding: [10, 10] });
            });
            return btn;
        },
        onRemove: function () { }
    });
}

async function initialiseMap(container) {
    const geoDataUrl = container.dataset.geoDataUrl || '/CoffeeBeans?handler=GeoData';
    const worldCountriesUrl = container.dataset.worldUrl || '/data/world_countries.geojson';
    // Get base path for static assets (handles /coffee subpath deployment)
    const pathBase = window.location.pathname.split('/CoffeeBeans')[0] || '';
    const coffeeBeanIconPath = `${pathBase}/images/coffee-bean-marker.png`;

    const beltBounds = L.latLngBounds(L.latLng(-30, -180), L.latLng(25, 180));

    const map = L.map(container, {
        zoomControl: true,
        dragging: true,
        scrollWheelZoom: true,
        doubleClickZoom: true,
        boxZoom: true,
        keyboard: true,
        tap: true,
        touchZoom: true,
        worldCopyJump: true,
        maxBounds: beltBounds,
        maxBoundsViscosity: 0.8,
        minZoom: 2
    });

    map.fitBounds(beltBounds, { padding: [10, 10] });

    // SOLUTION NOTE: Use ISO3166-1-Alpha-3 and lowercase 'name' properties for country identification
    // This GeoJSON format uses: properties['ISO3166-1-Alpha-3'] for ISO codes, properties.name for country names
    
    // Option 1: CartoDB Positron (lighter labels, more readable)
    const baseLayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
        attribution: '© OpenStreetMap contributors, © CARTO',
        maxZoom: 19,
        subdomains: 'abcd'
    });
    
    // Option 2: OpenStreetMap (original, darker labels)
    // const baseLayer = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    //     attribution: '© OpenStreetMap contributors',
    //     maxZoom: 19
    // });
    
    baseLayer.addTo(map);

    const regionsLayerRef = { layer: null };
    const countryLabelsRef = { group: null }; // Store custom country labels

    const LegendControl = createLegendControl();
    (new LegendControl({ position: 'bottomleft' })).addTo(map);

    const ResetControl = createResetControl(map, beltBounds, regionsLayerRef);
    (new ResetControl({ position: 'topleft' })).addTo(map);

    // Create layer group for custom country labels
    countryLabelsRef.group = L.layerGroup().addTo(map);

    let countryLayer = null;

    try {
        const worldResponse = await fetch(worldCountriesUrl, { cache: 'reload' });
        if (!worldResponse.ok) {
            throw new Error('Failed to load world countries dataset');
        }
        const worldGeoJson = await worldResponse.json();

        // Debug counter for logging first few countries
        let debugCount = 0;

        countryLayer = L.geoJSON(worldGeoJson, {
            style: function (feature) {
                const { type } = resolveCountryType(feature);
                return {
                    color: '#44515c',
                    weight: 1.0,
                    opacity: 1,
                    fill: true,
                    fillColor: colorByType(type),
                    fillOpacity: type !== 'none' ? 0.85 : 0.25
                };
            },
            onEachFeature: function (feature, layer) {
                const info = resolveCountryType(feature);
                
                // Debug first few countries
                if (debugCount < 5) {
                    console.log(`[${debugCount}] Country: "${info.name}" ISO: "${info.iso}" Type: "${info.type}"`, 
                        'Available props:', Object.keys(feature.properties || {}).slice(0, 8));
                    debugCount++;
                }
                
                layer.bindTooltip(`${info.name} [${info.iso}] • ${info.type}`, { sticky: true });
                
                // Force apply style after layer creation
                const { type } = info;
                layer.setStyle({
                    fillColor: colorByType(type),
                    fillOpacity: type !== 'none' ? 0.85 : 0.25,
                    fill: true
                });
                
                // Add custom country label (customizable - modify styles in the div below)
                // These labels are visible over colored areas and fully customizable
                if (layer.getBounds && countryLabelsRef.group) {
                    const bounds = layer.getBounds();
                    const center = bounds.getCenter();
                    // Show labels for coffee-producing countries
                    if (type !== 'none') {
                        const fontSize = '15px'; // CUSTOMIZE: Change font size (e.g., '18px', '20px')
                        const fontWeight = '700'; // CUSTOMIZE: Change weight ('400', '600', '700', 'bold')
                        const textColor = '#1a1a1a'; // CUSTOMIZE: Change color ('#000000', '#ffffff', '#2e7d32', etc.)
                        const shadowColor = 'rgba(255,255,255,0.9)'; // CUSTOMIZE: Text shadow color
                        
                        const labelDiv = L.divIcon({
                            html: `<div style="
                                color: ${textColor}; 
                                font-weight: ${fontWeight}; 
                                font-size: ${fontSize}; 
                                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                text-shadow: 2px 2px 3px ${shadowColor}, -1px -1px 2px ${shadowColor};
                                pointer-events: none;
                                white-space: nowrap;
                                text-align: center;
                            ">${info.name}</div>`,
                            className: 'country-label',
                            iconSize: [150, 25],
                            iconAnchor: [75, 12]
                        });
                        const labelMarker = L.marker(center, { 
                            icon: labelDiv, 
                            interactive: false, 
                            zIndexOffset: -500 
                        });
                        countryLabelsRef.group.addLayer(labelMarker);
                        
                        // Show/hide labels based on zoom level for better readability
                        const updateLabelVisibility = () => {
                            const zoom = map.getZoom();
                            // Hide labels at very low zoom, show at medium+ zoom
                            if (zoom < 3) {
                                labelMarker.setOpacity(0);
                            } else {
                                labelMarker.setOpacity(1);
                            }
                        };
                        map.on('zoomend', updateLabelVisibility);
                        updateLabelVisibility();
                    }
                }

                layer.on('click', async () => {
                    map.fitBounds(layer.getBounds(), { maxZoom: 6, animate: true });

                    if (regionsLayerRef.layer) {
                        map.removeLayer(regionsLayerRef.layer);
                        regionsLayerRef.layer = null;
                    }

                    if (info.iso !== 'PER') {
                        return;
                    }

                    try {
                        const adm1 = await loadPeruAdm('adm1', geoDataUrl);
                        // Create layer group for region labels
                        const regionLabelsGroup = L.layerGroup().addTo(map);
                        
                        regionsLayerRef.layer = L.geoJSON(adm1, {
                            style: function (ff) {
                                // Completely transparent - no background color
                                return { color: 'transparent', weight: 0, fill: false, fillOpacity: 0 };
                            },
                            onEachFeature: function (ff, ll) {
                                const nm = ff.properties.shapeName || ff.properties.NAME_1 || ff.properties.shapeName_1 || 'Region';
                                ll.bindTooltip(nm, { sticky: true });
                                
                                // Minimalist style: coffee bean above region name
                                if (ll.getBounds) {
                                    const bounds = ll.getBounds();
                                    const center = bounds.getCenter();
                                    const leftEdge = bounds.getWest();
                                    
                                    // Position label on left side (over ocean)
                                    const labelPosition = L.latLng(center.lat, leftEdge - 0.3);
                                    
                                    // Zoom-based scaling function
                                    const getBeanSize = (zoom) => {
                                        if (zoom < 4) return 12; // Very small when zoomed out
                                        if (zoom < 5) return 16;
                                        if (zoom < 6) return 20;
                                        return 24; // Full size when zoomed in
                                    };
                                    
                                    const getFontSize = (zoom) => {
                                        if (zoom < 4) return '10px';
                                        if (zoom < 5) return '11px';
                                        if (zoom < 6) return '12px';
                                        return '13px';
                                    };
                                    
                                    const updateMarkerSize = () => {
                                        const currentZoom = map.getZoom();
                                        const beanSize = getBeanSize(currentZoom);
                                        const fontSize = getFontSize(currentZoom);
                                        const spacing = 4;
                                        const totalHeight = beanSize + spacing + 18;
                                        
                                        const combinedLabelDiv = L.divIcon({
                                            html: `<div style="
                                                display: flex;
                                                flex-direction: column;
                                                align-items: center;
                                                background: transparent;
                                                border: none;
                                                padding: 0;
                                                pointer-events: none;
                                                white-space: nowrap;
                                            ">
                                                <img src="${coffeeBeanIconPath}" 
                                                     style="width: ${beanSize}px; height: ${beanSize}px; display: block; margin-bottom: ${spacing}px;" 
                                                     alt="coffee bean" />
                                                <span style="
                                                    color: #000;
                                                    font-weight: 400;
                                                    font-size: ${fontSize};
                                                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                                    text-shadow: none;
                                                    line-height: 1;
                                                ">${nm}</span>
                                            </div>`,
                                            className: 'region-label',
                                            iconSize: [beanSize + 40, totalHeight],
                                            iconAnchor: [(beanSize + 40) / 2, totalHeight]
                                        });
                                        
                                        combinedMarker.setIcon(combinedLabelDiv);
                                    };
                                    
                                    // Create initial marker
                                    const initialZoom = map.getZoom();
                                    const beanSize = getBeanSize(initialZoom);
                                    const fontSize = getFontSize(initialZoom);
                                    const spacing = 4;
                                    const totalHeight = beanSize + spacing + 18;
                                    
                                    const combinedLabelDiv = L.divIcon({
                                        html: `<div style="
                                            display: flex;
                                            flex-direction: column;
                                            align-items: center;
                                            background: transparent;
                                            border: none;
                                            padding: 0;
                                            pointer-events: none;
                                            white-space: nowrap;
                                        ">
                                            <img src="${coffeeBeanIconPath}" 
                                                 style="width: ${beanSize}px; height: ${beanSize}px; display: block; margin-bottom: ${spacing}px;" 
                                                 alt="coffee bean" />
                                            <span style="
                                                color: #000;
                                                font-weight: 400;
                                                font-size: ${fontSize};
                                                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                                text-shadow: none;
                                                line-height: 1;
                                            ">${nm}</span>
                                        </div>`,
                                        className: 'region-label',
                                        iconSize: [beanSize + 40, totalHeight],
                                        iconAnchor: [(beanSize + 40) / 2, totalHeight]
                                    });
                                    
                                    const combinedMarker = L.marker(labelPosition, {
                                        icon: combinedLabelDiv,
                                        interactive: false,
                                        zIndexOffset: 1000
                                    });
                                    regionLabelsGroup.addLayer(combinedMarker);
                                    
                                    // Update size when zoom changes
                                    map.on('zoomend', updateMarkerSize);
                                }
                                if (nm === 'Cusco') {
                                    ll.on('click', async () => {
                                        try {
                                            const adm2 = await loadPeruAdm('adm2', geoDataUrl);
                                            const adm2Layer = L.geoJSON(adm2, {
                                                filter: function (feature) {
                                                    const prov = feature.properties.NAME_1 || feature.properties.shapeName_1 || '';
                                                    return prov === 'Cusco';
                                                },
                                                style: function (feature) {
                                                    const district = feature.properties.NAME_2 || feature.properties.shapeName_2 || '';
                                                    const isLaConvencion = laConvencionNames.has(district);
                                                    return {
                                                        color: isLaConvencion ? '#b71c1c' : '#5c6bc0',
                                                        weight: 1.4,
                                                        fillColor: isLaConvencion ? '#ef5350' : '#c5cae9',
                                                        fillOpacity: isLaConvencion ? 0.6 : 0.35
                                                    };
                                                },
                                                onEachFeature: function (feature, layer) {
                                                    const district = feature.properties.NAME_2 || feature.properties.shapeName_2 || 'Province';
                                                    layer.bindTooltip(district, { sticky: true });
                                                }
                                            }).addTo(map);
                                            map.fitBounds(adm2Layer.getBounds(), { maxZoom: 7, animate: true });
                                        } catch (err) {
                                            console.warn('ADM2 load failed for PER', err);
                                        }
                                    });
                                }
                            }
                        }).addTo(map);
                    } catch (err) {
                        console.error('ADM1 load failed for PER', err);
                    }
                });
            }
        }).addTo(map);

        // Auto focus on Peru once data loads
        const peruLayer = countryLayer.getLayers().find(layer => {
            const iso = getIso3FromFeature(layer.feature);
            return iso === 'PER';
        });

        if (peruLayer) {
            map.fitBounds(peruLayer.getBounds(), { maxZoom: 5, animate: true });
            if (regionsLayerRef.layer) {
                map.removeLayer(regionsLayerRef.layer);
                regionsLayerRef.layer = null;
            }
            try {
                const adm1 = await loadPeruAdm('adm1', geoDataUrl);
                // Create layer group for region labels
                const regionLabelsGroup = L.layerGroup().addTo(map);
                
                regionsLayerRef.layer = L.geoJSON(adm1, {
                    style: function (ff) {
                        // Completely transparent - no background color
                        return { color: 'transparent', weight: 0, fill: false, fillOpacity: 0 };
                    },
                    onEachFeature: function (ff, ll) {
                        const nm = ff.properties.shapeName || ff.properties.NAME_1 || ff.properties.shapeName_1 || 'Region';
                        ll.bindTooltip(nm, { sticky: true });
                        
                        // Minimalist style: coffee bean above region name
                        if (ll.getBounds) {
                            const bounds = ll.getBounds();
                            const center = bounds.getCenter();
                            const leftEdge = bounds.getWest();
                            
                            // Position label on left side (over ocean)
                            const labelPosition = L.latLng(center.lat, leftEdge - 0.3);
                            
                            // Zoom-based scaling function
                            const getBeanSize = (zoom) => {
                                if (zoom < 4) return 12; // Very small when zoomed out
                                if (zoom < 5) return 16;
                                if (zoom < 6) return 20;
                                return 24; // Full size when zoomed in
                            };
                            
                            const getFontSize = (zoom) => {
                                if (zoom < 4) return '10px';
                                if (zoom < 5) return '11px';
                                if (zoom < 6) return '12px';
                                return '13px';
                            };
                            
                            const updateMarkerSize = () => {
                                const currentZoom = map.getZoom();
                                const beanSize = getBeanSize(currentZoom);
                                const fontSize = getFontSize(currentZoom);
                                const spacing = 4;
                                const totalHeight = beanSize + spacing + 18;
                                
                                const combinedLabelDiv = L.divIcon({
                                    html: `<div style="
                                        display: flex;
                                        flex-direction: column;
                                        align-items: center;
                                        background: transparent;
                                        border: none;
                                        padding: 0;
                                        pointer-events: none;
                                        white-space: nowrap;
                                    ">
                                        <img src="${coffeeBeanIconPath}" 
                                             style="width: ${beanSize}px; height: ${beanSize}px; display: block; margin-bottom: ${spacing}px;" 
                                             alt="coffee bean" />
                                        <span style="
                                            color: #000;
                                            font-weight: 400;
                                            font-size: ${fontSize};
                                            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                            text-shadow: none;
                                            line-height: 1;
                                        ">${nm}</span>
                                    </div>`,
                                    className: 'region-label',
                                    iconSize: [beanSize + 40, totalHeight],
                                    iconAnchor: [(beanSize + 40) / 2, totalHeight]
                                });
                                
                                combinedMarker.setIcon(combinedLabelDiv);
                            };
                            
                            // Create initial marker
                            const initialZoom = map.getZoom();
                            const beanSize = getBeanSize(initialZoom);
                            const fontSize = getFontSize(initialZoom);
                            const spacing = 4;
                            const totalHeight = beanSize + spacing + 18;
                            
                            const combinedLabelDiv = L.divIcon({
                                html: `<div style="
                                    display: flex;
                                    flex-direction: column;
                                    align-items: center;
                                    background: transparent;
                                    border: none;
                                    padding: 0;
                                    pointer-events: none;
                                    white-space: nowrap;
                                ">
                                    <img src="${coffeeBeanIconPath}" 
                                         style="width: ${beanSize}px; height: ${beanSize}px; display: block; margin-bottom: ${spacing}px;" 
                                         alt="coffee bean" />
                                    <span style="
                                        color: #000;
                                        font-weight: 400;
                                        font-size: ${fontSize};
                                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                        text-shadow: none;
                                        line-height: 1;
                                    ">${nm}</span>
                                </div>`,
                                className: 'region-label',
                                iconSize: [beanSize + 40, totalHeight],
                                iconAnchor: [(beanSize + 40) / 2, totalHeight]
                            });
                            
                            const combinedMarker = L.marker(labelPosition, {
                                icon: combinedLabelDiv,
                                interactive: false,
                                zIndexOffset: 1000
                            });
                            regionLabelsGroup.addLayer(combinedMarker);
                            
                            // Update size when zoom changes
                            map.on('zoomend', updateMarkerSize);
                        }
                    }
                }).addTo(map);
            } catch (err) {
                console.warn('Auto PER ADM1 failed', err);
            }
        }
    } catch (error) {
        console.error('Map data load failed', error);
        container.innerHTML = `<div style="padding:20px;text-align:center;color:red;">Map failed to load: ${error.message}</div>`;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('coffee-belt-map');
    if (!container) {
        return;
    }
    initialiseMap(container).catch(err => {
        console.error('Map initialization error', err);
        container.innerHTML = `<div style="padding:20px;text-align:center;color:red;">Map failed to load: ${err.message}</div>`;
    });
});
