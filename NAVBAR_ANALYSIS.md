# Navigation Bar Complete Analysis

## Overview
The navigation bar is a complex, responsive component that combines Bootstrap 5, custom CSS, Tailwind CSS, and custom JavaScript. It features a fixed position, dropdown menus, theme toggle, and responsive mobile/desktop layouts.

---

## 1. HTML STRUCTURE

### Main Container
**Location:** `Pages/Shared/_Layout.cshtml` (lines 1148-1255)

```1148:1149:Pages/Shared/_Layout.cshtml
    <nav class="navbar navbar-expand-lg navbar-light fixed-top" id="navbar" style="background-color: rgba(255, 255, 255, 0.9); backdrop-filter: blur(12px); border-bottom: 1px solid rgba(0,0,0,0.1); padding-top: 1rem; padding-bottom: 1rem; padding-left: 1.5rem; padding-right: 1.5rem; max-width: 100%; width: 100%; overflow-x: hidden !important; overflow-y: visible !important; box-sizing: border-box; top: 0; margin-top: 0; position: fixed; scrollbar-width: none !important; -ms-overflow-style: none !important;">
        <div class="container-fluid" style="max-width: 1400px; margin: 0 auto; width: 100%; overflow-x: hidden !important; overflow-y: visible !important; box-sizing: border-box; padding-left: 1rem; padding-right: 1rem; scrollbar-width: none !important; -ms-overflow-style: none !important;">
```

**Key Classes:**
- `navbar` - Bootstrap base class
- `navbar-expand-lg` - Expands at large breakpoint (992px)
- `navbar-light` - Light theme variant
- `fixed-top` - Fixed positioning at top of viewport
- `id="navbar"` - Unique identifier for JavaScript targeting

**Inline Styles:**
- Background with transparency and backdrop blur
- Overflow controls (hidden on x-axis, visible on y-axis for dropdowns)
- Fixed positioning with top: 0

### Logo Section
**Location:** Lines 1151-1157

```1151:1157:Pages/Shared/_Layout.cshtml
            <a class="navbar-brand d-flex align-items-center gap-3 hover:text-swiss-red transition-colors group cursor-pointer text-decoration-none" asp-page="/Index" style="text-decoration: none; display: flex; align-items: center; gap: 0.75rem; max-width: calc(100% - 150px); overflow: hidden; flex-shrink: 1;">
                <span class="font-grotesk font-bold text-4xl tracking-tighter leading-none text-charcoal dark:text-white group-hover:text-swiss-red transition-colors" style="font-family: 'Space Grotesk', sans-serif; font-weight: 700; font-size: 2.25rem; letter-spacing: -0.02em; line-height: 1; color: #111111; transition: color 0.3s; white-space: nowrap;">LLAMA</span>
                <div class="d-flex flex-column text-[10px] font-mono font-bold leading-tight uppercase tracking-widest border-l-2 border-charcoal dark:border-white pl-2 group-hover:border-swiss-red transition-colors" style="display: flex; flex-direction: column; font-size: 10px; font-family: 'JetBrains Mono', monospace; font-weight: 700; line-height: 1.25; text-transform: uppercase; letter-spacing: 0.1em; border-left: 2px solid #111111; padding-left: 0.5rem; transition: border-color 0.3s; flex-shrink: 0;">
                    <span>coffee</span>
                    <span>& cacao</span>
                </div>
            </a>
```

**Structure:**
- Uses Tailwind classes (`group`, `hover:text-swiss-red`, `dark:text-white`)
- Two-part logo: "LLAMA" (large) + "coffee & cacao" (small, vertical)
- Hover effects change color to Swiss red (#FF3B30)
- Responsive max-width calculation

### OS Link (Special Button)
**Location:** Line 1160

```1160:1160:Pages/Shared/_Layout.cshtml
            <a class="nav-link os-nav-link @(Context.Request.Path.StartsWithSegments("/OS") ? "active fw-bold" : "")" asp-page="/OS" style="background-color: #0059ff !important; color: white !important; padding: 0.5rem 1rem !important; border-radius: 4px; font-weight: bold; margin-right: 10px; order: -1; flex-shrink: 0; white-space: nowrap;">OS</a>
```

**Features:**
- Blue background (#0059ff)
- `order: -1` places it before logo in flex layout
- Active state detection via Razor syntax

### Mobile Toggle Button
**Location:** Lines 1162-1164

```1162:1164:Pages/Shared/_Layout.cshtml
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
```

**Bootstrap Integration:**
- `data-bs-toggle="collapse"` - Bootstrap collapse trigger
- `data-bs-target="#navbarNav"` - Targets the collapsible content
- Hamburger icon rendered by Bootstrap

### Navigation Collapse Container
**Location:** Line 1166

```1166:1166:Pages/Shared/_Layout.cshtml
            <div class="collapse navbar-collapse" id="navbarNav">
```

**Responsive Behavior:**
- `collapse` - Hidden by default on mobile
- `navbar-collapse` - Bootstrap class for navbar-specific collapse
- Shows/hides based on Bootstrap breakpoints and toggle state

### Desktop Navigation Links
**Location:** Lines 1168-1198

```1168:1198:Pages/Shared/_Layout.cshtml
                <ul class="navbar-nav mx-auto mb-2 mb-lg-0 d-none d-xl-flex gap-2" style="margin-left: auto; margin-right: auto; gap: 2rem; overflow-x: hidden !important; overflow-y: visible !important; scrollbar-width: none !important; -ms-overflow-style: none !important;">
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/Shop") ? "active fw-bold" : "")" asp-page="/Shop" style="font-size: 0.875rem; font-weight: 500; letter-spacing: 0.05em; opacity: 0.6; transition: opacity 0.3s;">SHOP</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/GreenBeans") ? "active fw-bold" : "")" asp-page="/GreenBeans" style="font-size: 0.875rem; font-weight: 500; letter-spacing: 0.05em; opacity: 0.6; transition: opacity 0.3s;">GREEN BEANS</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/Tours") ? "active fw-bold" : "")" asp-page="/Tours" style="font-size: 0.875rem; font-weight: 500; letter-spacing: 0.05em; opacity: 0.6; transition: opacity 0.3s;">TOUR</a>
                    </li>
                    <li class="nav-item dropdown">
                        <button class="nav-link dropdown-toggle @(Context.Request.Path.StartsWithSegments("/Learn") || Context.Request.Path.StartsWithSegments("/Newbie") || Context.Request.Path.StartsWithSegments("/BrewMethods") || Context.Request.Path.StartsWithSegments("/BeanAtlas") || Context.Request.Path.StartsWithSegments("/RoastScience") || Context.Request.Path.StartsWithSegments("/CoffeeAlchemy") ? "active fw-bold" : "")" id="guideDropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false" aria-haspopup="true" style="font-size: 0.875rem; font-weight: 500; letter-spacing: 0.05em; opacity: 0.6; transition: opacity 0.3s; cursor: pointer; background: none; border: none; color: inherit; padding: 0.5rem 1rem; display: flex; align-items: center; gap: 0.25rem;">
                            GUIDE
                            <svg class="dropdown-toggle-icon" width="10" height="6" viewBox="0 0 10 6" fill="none" xmlns="http://www.w3.org/2000/svg" style="transition: transform 0.3s;">
                                <path d="M1 1L5 5L9 1" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                            </svg>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="guideDropdown" style="overflow-x: hidden !important; overflow-y: visible !important; scrollbar-width: none !important; -ms-overflow-style: none !important; margin-top: 0.5rem;">
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/Learn") ? "active" : "")" asp-page="/Learn">Flavor Spectrum</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/BrewMethods") ? "active" : "")" asp-page="/BrewMethods">Brew Methods 101</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/BeanAtlas") ? "active" : "")" asp-page="/BeanAtlas">The Bean Atlas</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/RoastScience") ? "active" : "")" asp-page="/RoastScience">Roast Science</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/CoffeeAlchemy") ? "active" : "")" asp-page="/CoffeeAlchemy">Coffee Alchemy</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/Newbie") ? "active" : "")" asp-page="/Newbie">Newbie Guide</a></li>
                        </ul>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/SocialCoffee") ? "active fw-bold" : "")" asp-page="/SocialCoffee" style="font-size: 0.875rem; font-weight: 500; letter-spacing: 0.05em; opacity: 0.6; transition: opacity 0.3s;">SOCIAL</a>
                    </li>
                </ul>
```

**Key Features:**
- `d-none d-xl-flex` - Hidden on mobile, visible on XL screens (1200px+)
- `mx-auto` - Centers the navigation
- Active state detection via Razor `@()` syntax
- Custom SVG chevron icon (not Bootstrap's default)
- Dropdown uses `aria-labelledby` for accessibility

### Mobile Navigation Links
**Location:** Lines 1201-1234

```1201:1234:Pages/Shared/_Layout.cshtml
                <ul class="navbar-nav d-lg-none mb-2 mb-lg-0">
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/Shop") ? "active fw-bold" : "")" asp-page="/Shop">Shop</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/GreenBeans") ? "active fw-bold" : "")" asp-page="/GreenBeans">Green Beans</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/Tours") ? "active fw-bold" : "")" asp-page="/Tours">Tour</a>
                    </li>
                    <li class="nav-item dropdown">
                        <button class="nav-link dropdown-toggle @(Context.Request.Path.StartsWithSegments("/Learn") || Context.Request.Path.StartsWithSegments("/Newbie") || Context.Request.Path.StartsWithSegments("/BrewMethods") || Context.Request.Path.StartsWithSegments("/BeanAtlas") || Context.Request.Path.StartsWithSegments("/RoastScience") || Context.Request.Path.StartsWithSegments("/CoffeeAlchemy") ? "active fw-bold" : "")" id="guideDropdownMobile" type="button" data-bs-toggle="dropdown" aria-expanded="false" aria-haspopup="true" style="cursor: pointer; background: none; border: none; color: inherit; padding: 0.5rem 1rem; display: flex; align-items: center; gap: 0.25rem; width: 100%; text-align: left;">
                            Guide
                            <svg class="dropdown-toggle-icon" width="10" height="6" viewBox="0 0 10 6" fill="none" xmlns="http://www.w3.org/2000/svg" style="transition: transform 0.3s; margin-left: auto;">
                                <path d="M1 1L5 5L9 1" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                            </svg>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="guideDropdownMobile" style="overflow-x: hidden !important; overflow-y: visible !important; scrollbar-width: none !important; -ms-overflow-style: none !important;">
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/Learn") ? "active" : "")" asp-page="/Learn">Flavor Spectrum</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/BrewMethods") ? "active" : "")" asp-page="/BrewMethods">Brew Methods 101</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/BeanAtlas") ? "active" : "")" asp-page="/BeanAtlas">The Bean Atlas</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/RoastScience") ? "active" : "")" asp-page="/RoastScience">Roast Science</a></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/CoffeeAlchemy") ? "active" : "")" asp-page="/CoffeeAlchemy">Coffee Alchemy</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item @(Context.Request.Path.StartsWithSegments("/Newbie") ? "active" : "")" asp-page="/Newbie">Newbie Guide</a></li>
                        </ul>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/SocialCoffee") ? "active fw-bold" : "")" asp-page="/SocialCoffee">Social</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link @(Context.Request.Path.StartsWithSegments("/Cacao") ? "active fw-bold" : "")" asp-page="/Cacao">Cacao</a>
                    </li>
                </ul>
```

**Key Features:**
- `d-lg-none` - Only visible below 992px (mobile/tablet)
- Separate dropdown with `id="guideDropdownMobile"`
- Full-width button (`width: 100%`)
- Chevron icon positioned with `margin-left: auto`

### Right Controls Section
**Location:** Lines 1237-1252

```1237:1252:Pages/Shared/_Layout.cshtml
                <div class="d-flex align-items-center gap-3 ms-auto" style="flex-shrink: 0 !important; min-width: fit-content;">
                    <!-- CONDOR TAB (Right Side) -->
                    <a class="nav-link cacao-nav-link d-none d-md-block @(Context.Request.Path.StartsWithSegments("/Condor") ? "active fw-bold" : "")" asp-page="/Condor" style="font-size: 0.875rem; font-weight: 700; letter-spacing: 0.05em; transition: all 0.3s; border-right: 1px solid rgba(0,0,0,0.2); padding-right: 1.5rem; margin-right: 0.5rem; background-color: #ffd700 !important; color: #000 !important; padding: 0.5rem 1rem !important; border-radius: 4px;">CONDOR</a>
                    <!-- CACAO TAB (Right Side) -->
                    <a class="nav-link cacao-nav-link d-none d-md-block @(Context.Request.Path.StartsWithSegments("/Cacao") ? "active fw-bold" : "")" asp-page="/Cacao" style="font-size: 0.875rem; font-weight: 700; letter-spacing: 0.05em; transition: all 0.3s; border-right: 1px solid rgba(0,0,0,0.2); padding-right: 1.5rem; margin-right: 0.5rem; background-color: #0059ff !important; color: white !important; padding: 0.5rem 1rem !important; border-radius: 4px;">CACAO</a>
                    
                    <!-- THEME TOGGLE SWITCH -->
                    <button id="theme-toggle" type="button" class="theme-toggle-btn" aria-label="Toggle Dark Mode" style="flex-shrink: 0; width: 3rem; height: 1.5rem; border-radius: 9999px; background-color: #e5e7eb; position: relative; border: none; cursor: pointer; transition: background-color 0.3s;">
                        <div id="toggle-circle" class="toggle-circle" style="width: 1rem; height: 1rem; border-radius: 9999px; background-color: white; position: absolute; left: 0.25rem; top: 50%; transform: translateY(-50%); transition: all 0.3s; display: flex; align-items: center; justify-content: center; font-size: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.1);">
                            <i class="bi bi-sun-fill theme-icon-light" style="color: black;"></i>
                            <i class="bi bi-moon-fill theme-icon-dark" style="display: none; color: white;"></i>
                        </div>
                    </button>
                    
                    @await Component.InvokeAsync("CartIcon")
                </div>
```

**Components:**
1. **CONDOR Link** - Gold background (#ffd700), hidden on mobile (`d-none d-md-block`)
2. **CACAO Link** - Blue background (#0059ff), hidden on mobile
3. **Theme Toggle** - Custom switch button with animated circle
4. **CartIcon** - ViewComponent that renders shopping cart icon

---

## 2. CSS STYLING

### A. Embedded Styles in `<head>` Section
**Location:** Lines 25-1143 in `_Layout.cshtml`

#### Bootstrap Dropdown Overrides (Lines 25-108)
```25:108:Pages/Shared/_Layout.cshtml
    <style>
        /* HIDE Bootstrap's default dropdown chevron - we use custom SVG */
        .dropdown-toggle::after {
            display: none !important;
            content: none !important;
        }
        
        /* CRITICAL: Force hide ALL dropdown menus by default - highest specificity */
        .dropdown-menu,
        ul.dropdown-menu,
        .navbar-nav .dropdown-menu,
        .dropdown .dropdown-menu {
            display: none !important;
            visibility: hidden !important;
            opacity: 0 !important;
            pointer-events: none !important;
            position: absolute !important;
        }
        
        /* Show dropdown when Bootstrap adds .show class */
        .dropdown-menu.show,
        ul.dropdown-menu.show,
        .navbar-nav .dropdown-menu.show,
        .dropdown .dropdown-menu.show {
            display: block !important;
            visibility: visible !important;
            opacity: 1 !important;
            pointer-events: auto !important;
        }
        
        /* ALSO show when button has aria-expanded="true" (backup trigger) */
        .dropdown-toggle[aria-expanded="true"] ~ .dropdown-menu,
        .dropdown-toggle[aria-expanded="true"] + .dropdown-menu,
        #guideDropdown[aria-expanded="true"] ~ ul[aria-labelledby="guideDropdown"],
        #guideDropdownMobile[aria-expanded="true"] ~ ul[aria-labelledby="guideDropdownMobile"],
        .dropdown:has(.dropdown-toggle[aria-expanded="true"]) .dropdown-menu {
            display: block !important;
            visibility: visible !important;
            opacity: 1 !important;
            pointer-events: auto !important;
        }
        
        /* Ensure dropdown menus can escape navbar overflow on desktop */
        @@media (min-width: 1200px) {
            .navbar-nav.d-xl-flex .dropdown-menu {
                position: absolute !important;
                top: 100% !important;
                left: auto !important;
                right: 0 !important;
                z-index: 1055 !important;
            }
            
            /* CRITICAL: Override all styles when dropdown is shown on desktop */
            .navbar-nav.d-xl-flex .dropdown-menu.show,
            .navbar-nav.d-xl-flex .dropdown.show .dropdown-menu {
                display: block !important;
                visibility: visible !important;
                opacity: 1 !important;
                pointer-events: auto !important;
            }
            
            /* Ensure dropdown menu is hidden by default on desktop */
            .navbar-nav.d-xl-flex .dropdown-menu:not(.show) {
                display: none !important;
                visibility: hidden !important;
                opacity: 0 !important;
                pointer-events: none !important;
            }
            
            /* Allow dropdown to overflow navbar container */
            #navbarNav {
                overflow: visible !important;
            }
            
            .navbar-nav.d-xl-flex {
                overflow: visible !important;
            }
        }
        
        /* Ensure the nav item doesn't hide the menu */
        .nav-item.dropdown {
            overflow: visible !important;
        }
    </style>
```

**Purpose:**
- Hides Bootstrap's default dropdown chevron
- Forces dropdowns hidden by default (multiple selectors for specificity)
- Shows dropdowns when `.show` class is added OR `aria-expanded="true"`
- Desktop-specific positioning (right-aligned, z-index 1055)
- Ensures overflow visibility for dropdowns

#### Global Styles (Lines 139-1143)
**Key Sections:**

1. **Scrollbar Hiding** (Lines 152-336)
   - Removes all scrollbars from navbar and related elements
   - Uses `scrollbar-width: none` (Firefox) and `::-webkit-scrollbar` (Chrome/Safari)

2. **Navbar Base Styles** (Lines 480-621)
```480:621:Pages/Shared/_Layout.cshtml
        /* Clean, minimalist navigation */
        .navbar {
            background: var(--white) !important;
            border-bottom: 1px solid #E0E0E0;
            box-shadow: 0 2px 4px rgba(0,0,0,0.08);
            transition: all 0.3s ease;
            z-index: 1050 !important;
        }
        
        .dark .navbar {
            background: var(--dark-bg) !important;
            border-bottom: 1px solid var(--dark-border);
            box-shadow: 0 2px 4px rgba(0,0,0,0.3);
        }
        
        .navbar.scrolled {
            background: rgba(255, 255, 255, 0.9) !important;
            backdrop-filter: blur(10px);
        }
        
        .dark .navbar.scrolled {
            background: rgba(10, 10, 10, 0.9) !important;
        }
        
        .navbar-collapse {
            visibility: visible !important;
        }
        
        @@media (min-width: 992px) {
            .navbar-collapse {
                display: flex !important;
            }
        }
        
        @@media (max-width: 991px) {
            .navbar-collapse {
                display: none !important;
            }
            .navbar-collapse.show {
                display: block !important;
            }
        }
        
        .navbar-brand {
            color: #000000 !important;
            font-weight: 700 !important;
            font-size: 1.5rem;
            text-decoration: none;
            font-family: 'Space Grotesk', 'JetBrains Mono', 'SUSE Mono', 'Courier New', monospace;
            display: flex;
            align-items: center;
            transition: color 0.3s ease;
            visibility: visible !important;
            max-width: 100%;
            overflow: hidden;
            flex-shrink: 1;
        }
        
        @@media (max-width: 576px) {
            .navbar-brand {
                font-size: 1rem;
                max-width: calc(100% - 100px);
                overflow: hidden;
                gap: 0.5rem !important;
            }
            .navbar-brand > span {
                font-size: 1.5rem !important;
            }
            .navbar-brand > div {
                font-size: 8px !important;
                padding-left: 0.25rem !important;
            }
            #navbar {
                padding: 0.75rem 0.5rem !important;
            }
            .navbar .container-fluid {
                padding-left: 0.5rem !important;
                padding-right: 0.5rem !important;
            }
        }
        
        .dark .navbar-brand {
            color: white !important;
        }
        
        .navbar-brand:hover {
            color: #000000 !important;
        }
        
        .dark .navbar-brand:hover {
            color: white !important;
        }
        
        /* New Logo Structure Styling */
        .navbar-brand .group:hover span {
            color: #FF3B30 !important;
        }
        
        .dark .navbar-brand .group:hover span {
            color: #FF3B30 !important;
        }
        
        .navbar-brand .group:hover .border-l-2 {
            border-color: #FF3B30 !important;
        }
        
        /* Desktop Links (xl breakpoint) */
        @@media (min-width: 1200px) {
            .navbar-nav.d-xl-flex {
                display: flex !important;
            }
            .navbar-nav.d-lg-none {
                display: none !important;
            }
        }
        
        @@media (max-width: 1199px) {
            .navbar-nav.d-xl-flex {
                display: none !important;
            }
        }
        
        /* Ensure mobile links are hidden on desktop when navbar is expanded */
        @@media (min-width: 992px) {
            .navbar-collapse:not(.show) .navbar-nav.d-lg-none {
                display: none !important;
            }
        }
        
        /* Navbar backdrop blur for new structure */
        #navbar {
            backdrop-filter: blur(12px) !important;
            -webkit-backdrop-filter: blur(12px) !important;
            top: 0 !important;
            margin-top: 0 !important;
            padding-top: 1rem !important;
            padding-bottom: 1rem !important;
        }
        
        .dark #navbar {
            background-color: rgba(10, 10, 10, 0.9) !important;
        }
```

3. **Nav Link Styles** (Lines 642-677)
```642:677:Pages/Shared/_Layout.cshtml
        .nav-link {
            color: var(--charcoal) !important;
            font-weight: 500;
            padding: 0.5rem 1rem !important;
            transition: all 0.3s ease;
            border-radius: 4px;
            font-family: 'Inter', 'JetBrains Mono', 'SUSE Mono', 'Courier New', monospace;
            opacity: 0.8 !important;
            display: block !important;
            visibility: visible !important;
        }
        
        button.nav-link {
            width: 100%;
            text-align: left;
        }
        
        .dark .nav-link {
            color: rgba(255, 255, 255, 0.8) !important;
        }
        
        button.nav-link {
            color: inherit !important;
        }
        
        .nav-link:hover {
            color: var(--charcoal) !important;
            opacity: 1 !important;
            background-color: transparent !important;
        }
        
        .dark .nav-link:hover {
            color: white !important;
            opacity: 1 !important;
        }
```

4. **Dropdown Styles** (Lines 688-752)
```688:752:Pages/Shared/_Layout.cshtml
        /* Dropdown styles */
        .nav-item.dropdown {
            position: relative;
        }
        
        .dropdown-toggle-icon {
            transition: transform 0.3s ease;
            margin-left: 0.25rem;
        }
        
        .dropdown-toggle[aria-expanded="true"] .dropdown-toggle-icon {
            transform: rotate(180deg);
        }
        
        /* Dropdown menu styling - display is handled by rules in head */
        .dropdown-menu {
            border: 1px solid #E0E0E0;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            border-radius: 4px;
            padding: 0.5rem 0;
            min-width: 200px;
            z-index: 1051 !important;
        }
        
        .dark .dropdown-menu {
            background-color: var(--dark-bg) !important;
            border-color: var(--dark-border);
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
        }
        
        .dropdown-item {
            padding: 0.5rem 1rem;
            font-size: 0.875rem;
            color: var(--charcoal);
            transition: all 0.2s ease;
        }
        
        .dark .dropdown-item {
            color: rgba(255, 255, 255, 0.8);
        }
        
        .dropdown-item:hover {
            background-color: #f5f5f5;
            color: var(--charcoal);
        }
        
        .dark .dropdown-item:hover {
            background-color: rgba(255, 255, 255, 0.1);
            color: white;
        }
        
        .dropdown-item.active {
            background-color: #ff6b35;
            color: white;
        }
        
        .dropdown-divider {
            margin: 0.5rem 0;
            border-color: #E0E0E0;
        }
        
        .dark .dropdown-divider {
            border-color: var(--dark-border);
        }
```

5. **Theme Toggle Styles** (Lines 365-427)
```365:427:Pages/Shared/_Layout.cshtml
        /* Theme Toggle Button */
        .theme-toggle-btn {
            width: 48px !important;
            height: 24px !important;
            background: #e5e7eb !important;
            border-radius: 9999px !important;
            position: relative !important;
            display: flex !important;
            align-items: center !important;
            cursor: pointer !important;
            border: none !important;
            transition: background-color 0.3s ease !important;
            padding: 0 !important;
            flex-shrink: 0 !important;
            visibility: visible !important;
            opacity: 1 !important;
        }
        
        .dark .theme-toggle-btn {
            background: rgba(255, 255, 255, 0.1) !important;
        }
        
        .toggle-circle {
            width: 16px !important;
            height: 16px !important;
            border-radius: 9999px !important;
            background: white !important;
            position: absolute !important;
            left: 4px !important;
            transition: all 0.3s ease !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            font-size: 10px !important;
            box-shadow: 0 1px 3px rgba(0,0,0,0.2) !important;
            visibility: visible !important;
        }
        
        .dark .toggle-circle {
            left: 28px !important;
            background: #FF3B30 !important;
        }
        
        .theme-icon-light,
        .theme-icon-dark {
            font-size: 10px !important;
            color: #000 !important;
            display: block !important;
            visibility: visible !important;
        }
        
        .dark .theme-icon-light {
            display: none !important;
        }
        
        .dark .theme-icon-dark {
            display: block !important;
            color: white !important;
        }
        
        .theme-icon-dark {
            display: none !important;
        }
```

### B. External CSS Files

#### 1. Bootstrap CSS (CDN)
**Location:** Line 20
```20:20:Pages/Shared/_Layout.cshtml
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
```

#### 2. Bootstrap Icons (CDN)
**Location:** Line 22
```22:22:Pages/Shared/_Layout.cshtml
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
```

#### 3. Tailwind CSS (CDN)
**Location:** Line 110
```110:110:Pages/Shared/_Layout.cshtml
    <script src="https://cdn.tailwindcss.com"></script>
```

#### 4. Custom CSS File
**Location:** Line 112
```112:112:Pages/Shared/_Layout.cshtml
    <link rel="stylesheet" href="~/css/coffee-app.css">
```

**Note:** `coffee-app.css` primarily styles cards and other page elements, NOT the navbar.

#### 5. CartIcon Component Styles
**Location:** `Pages/Shared/Components/CartIcon/Default.cshtml` (lines 3-45)

```3:45:Pages/Shared/Components/CartIcon/Default.cshtml
<style>
    .cart-icon-container {
        position: relative;
        display: inline-block;
        margin-left: 15px;
    }

    .cart-icon-link {
        color: var(--charcoal);
        text-decoration: none;
        font-size: 1.5rem;
        position: relative;
        transition: color 0.2s;
    }

    .cart-icon-link:hover {
        color: #0059ff;
    }

    .cart-count-badge {
        position: absolute;
        top: -8px;
        right: -10px;
        background: #ff6b35;
        color: white;
        border-radius: 50%;
        min-width: 20px;
        height: 20px;
        padding: 0 4px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.7rem;
        font-weight: 700;
        border: 2px solid white;
        z-index: 1000;
        box-shadow: 0 2px 4px rgba(0,0,0,0.2);
    }
    
    .dark .cart-count-badge {
        border: 2px solid #0a0a0a;
    }
</style>
```

---

## 3. JAVASCRIPT FUNCTIONALITY

### A. Bootstrap JavaScript
**Location:** Line 1309
```1309:1309:Pages/Shared/_Layout.cshtml
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
```

**Provides:**
- Dropdown functionality
- Collapse functionality (mobile menu toggle)
- Event system (`show.bs.dropdown`, `hide.bs.dropdown`, etc.)

### B. Theme Toggle JavaScript
**Location:** Lines 1312-1346

```1312:1346:Pages/Shared/_Layout.cshtml
    <script>
        const themeToggleBtn = document.getElementById('theme-toggle');
        const toggleCircle = document.getElementById('toggle-circle');
        const htmlElement = document.documentElement;
        const navbar = document.getElementById('navbar');

        // Check local storage or system preference
        if (localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
            htmlElement.classList.add('dark');
        } else {
            htmlElement.classList.remove('dark');
        }

        themeToggleBtn.addEventListener('click', () => {
            // Animate Icon
            toggleCircle.classList.add('rotate-icon');
            setTimeout(() => toggleCircle.classList.remove('rotate-icon'), 500);

            if (htmlElement.classList.contains('dark')) {
                htmlElement.classList.remove('dark');
                localStorage.theme = 'light';
            } else {
                htmlElement.classList.add('dark');
                localStorage.theme = 'dark';
            }
        });

        // Add background to nav on scroll
        window.addEventListener('scroll', () => {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    </script>
```

**Functionality:**
1. **Theme Detection:** Checks `localStorage.theme` or system preference
2. **Theme Toggle:** Adds/removes `dark` class on `<html>` element
3. **Icon Animation:** Rotates toggle circle on click
4. **Scroll Effect:** Adds `scrolled` class when page scrolls > 50px

### C. Dropdown Initialization JavaScript
**Location:** Lines 1348-1524

**Key Functions:**

1. **Page Load Initialization** (Lines 1352-1359)
```1352:1359:Pages/Shared/_Layout.cshtml
            document.querySelectorAll('.dropdown-menu').forEach(function(menu) {
                menu.classList.remove('show');
                // Remove any inline styles that might interfere
                menu.style.removeProperty('display');
                menu.style.removeProperty('visibility');
                menu.style.removeProperty('opacity');
                menu.style.removeProperty('pointer-events');
            });
```
- Removes `.show` class from all dropdowns on page load
- Clears inline styles to let CSS handle visibility

2. **Bootstrap Dropdown Initialization** (Lines 1370-1407)
```1370:1407:Pages/Shared/_Layout.cshtml
            // Initialize all Bootstrap dropdowns
            console.log('[DROPDOWN] Starting dropdown initialization...');
            document.querySelectorAll('.dropdown-toggle').forEach(function(toggle) {
                console.log('[DROPDOWN] Found dropdown toggle:', toggle.id || toggle.className);
                
                // Get the associated dropdown menu
                const dropdownId = toggle.getAttribute('id');
                const dropdownMenu = dropdownId ? document.querySelector(`[aria-labelledby="${dropdownId}"]`) : toggle.nextElementSibling;
                
                if (!dropdownMenu || !dropdownMenu.classList.contains('dropdown-menu')) {
                    console.warn('[DROPDOWN] No dropdown menu found for toggle:', toggle.id || toggle.className);
                    return; // Skip if no menu found
                }
                
                console.log('[DROPDOWN] Found dropdown menu for', dropdownId || 'toggle', '- hiding initially');
                
                // Force hide this dropdown menu initially (but don't use inline styles with !important)
                dropdownMenu.classList.remove('show');
                // Let CSS handle the hiding, don't set inline styles that conflict
                
                // Try Bootstrap initialization first
                let bootstrapInitialized = false;
                if (typeof bootstrap !== 'undefined') {
                    try {
                        if (!bootstrap.Dropdown.getInstance(toggle)) {
                            new bootstrap.Dropdown(toggle);
                            bootstrapInitialized = true;
                            console.log('[DROPDOWN] Bootstrap dropdown initialized for:', dropdownId || 'toggle');
                        } else {
                            bootstrapInitialized = true;
                            console.log('[DROPDOWN] Bootstrap dropdown already initialized for:', dropdownId || 'toggle');
                        }
                    } catch (e) {
                        console.error('[DROPDOWN] Bootstrap dropdown initialization failed:', e);
                    }
                } else {
                    console.warn('[DROPDOWN] Bootstrap is not available!');
                }
```

**Process:**
- Finds all `.dropdown-toggle` buttons
- Locates associated menu via `aria-labelledby` or `nextElementSibling`
- Initializes Bootstrap Dropdown instance
- Falls back to manual toggle if Bootstrap unavailable

3. **Bootstrap Event Handlers** (Lines 1461-1503)
```1461:1503:Pages/Shared/_Layout.cshtml
                // Rotate chevron icon when dropdown opens/closes (Bootstrap events)
                toggle.addEventListener('show.bs.dropdown', function() {
                    console.log('[DROPDOWN] Bootstrap show.bs.dropdown event fired for:', dropdownId || 'toggle');
                    const icon = this.querySelector('.dropdown-toggle-icon');
                    if (icon) {
                        icon.style.transform = 'rotate(180deg)';
                    }
                    // Remove any conflicting inline styles BEFORE Bootstrap adds .show
                    dropdownMenu.style.removeProperty('display');
                    dropdownMenu.style.removeProperty('visibility');
                    dropdownMenu.style.removeProperty('opacity');
                    dropdownMenu.style.removeProperty('pointer-events');
                    console.log('[DROPDOWN] Removed inline styles, Bootstrap will add .show class');
                });
                
                toggle.addEventListener('shown.bs.dropdown', function() {
                    console.log('[DROPDOWN] Bootstrap shown.bs.dropdown event fired - dropdown is now visible');
                    console.log('[DROPDOWN] Dropdown menu classes:', dropdownMenu.className);
                    console.log('[DROPDOWN] Dropdown menu inline styles:', {
                        display: dropdownMenu.style.display,
                        visibility: dropdownMenu.style.visibility,
                        opacity: dropdownMenu.style.opacity
                    });
                    console.log('[DROPDOWN] Dropdown menu computed styles:', {
                        display: window.getComputedStyle(dropdownMenu).display,
                        visibility: window.getComputedStyle(dropdownMenu).visibility,
                        opacity: window.getComputedStyle(dropdownMenu).opacity
                    });
                });
                
                toggle.addEventListener('hide.bs.dropdown', function() {
                    console.log('[DROPDOWN] Bootstrap hide.bs.dropdown event fired for:', dropdownId || 'toggle');
                    const icon = this.querySelector('.dropdown-toggle-icon');
                    if (icon) {
                        icon.style.transform = 'rotate(0deg)';
                    }
                    // Remove any conflicting inline styles
                    dropdownMenu.style.removeProperty('display');
                    dropdownMenu.style.removeProperty('visibility');
                    dropdownMenu.style.removeProperty('opacity');
                    dropdownMenu.style.removeProperty('pointer-events');
                    console.log('[DROPDOWN] Removed inline styles, Bootstrap will remove .show class');
                });
```

**Events:**
- `show.bs.dropdown` - Before dropdown opens (rotates icon, clears inline styles)
- `shown.bs.dropdown` - After dropdown opens (logging for debugging)
- `hide.bs.dropdown` - Before dropdown closes (resets icon, clears inline styles)

4. **Click Outside Handler** (Lines 1506-1523)
```1506:1523:Pages/Shared/_Layout.cshtml
            // Close dropdowns when clicking outside
            document.addEventListener('click', function(e) {
                if (!e.target.closest('.dropdown')) {
                    document.querySelectorAll('.dropdown-menu.show').forEach(function(menu) {
                        menu.classList.remove('show');
                        // Remove any conflicting inline styles - let CSS handle it
                        menu.style.removeProperty('display');
                        menu.style.removeProperty('visibility');
                        menu.style.removeProperty('opacity');
                        menu.style.removeProperty('pointer-events');
                    });
                    document.querySelectorAll('.dropdown-toggle[aria-expanded="true"]').forEach(function(btn) {
                        btn.setAttribute('aria-expanded', 'false');
                        const icon = btn.querySelector('.dropdown-toggle-icon');
                        if (icon) icon.style.transform = 'rotate(0deg)';
                    });
                }
            });
```

**Functionality:**
- Closes all open dropdowns when clicking outside
- Resets `aria-expanded` attributes
- Resets icon rotation

---

## 4. RESPONSIVE BREAKPOINTS

### Bootstrap Breakpoints Used:
- **xs:** < 576px (mobile)
- **sm:** ≥ 576px
- **md:** ≥ 768px
- **lg:** ≥ 992px (tablet/desktop)
- **xl:** ≥ 1200px (large desktop)

### Navbar Behavior by Breakpoint:

#### Mobile (< 992px)
- Hamburger menu visible
- Desktop nav links hidden (`d-none d-xl-flex`)
- Mobile nav links visible (`d-lg-none`)
- CONDOR/CACAO links hidden (`d-none d-md-block`)
- Navbar collapses by default

#### Tablet (992px - 1199px)
- Hamburger menu hidden
- Desktop nav links still hidden (need XL breakpoint)
- Mobile nav links visible in expanded navbar
- CONDOR/CACAO links visible (≥ 768px)

#### Desktop (≥ 1200px)
- Hamburger menu hidden
- Desktop nav links visible (`d-xl-flex`)
- Mobile nav links hidden (`d-lg-none`)
- All controls visible

---

## 5. DROPDOWN MECHANICS

### How Dropdowns Work:

1. **HTML Structure:**
   - Button with `data-bs-toggle="dropdown"` and `id="guideDropdown"`
   - Menu with `aria-labelledby="guideDropdown"` and class `dropdown-menu`

2. **Bootstrap Initialization:**
   - JavaScript creates `bootstrap.Dropdown` instance
   - Bootstrap manages `.show` class addition/removal
   - Bootstrap handles `aria-expanded` attribute

3. **CSS Visibility Control:**
   - Default: `display: none !important` (multiple selectors)
   - When `.show` added: `display: block !important`
   - Backup: `aria-expanded="true"` also triggers visibility

4. **Icon Animation:**
   - JavaScript listens to Bootstrap events
   - Rotates SVG icon 180deg when opening
   - Resets to 0deg when closing

5. **Positioning:**
   - Desktop: `dropdown-menu-end` aligns to right
   - Mobile: Full width in collapsed menu
   - Z-index: 1055 (above navbar's 1050)

---

## 6. COMPONENT INTEGRATION

### CartIcon ViewComponent
**Location:** `ViewComponents/CartIconViewComponent.cs`

**Process:**
1. Server-side: `CartIconViewComponent` gets cart count from `CartService`
2. Renders: `Pages/Shared/Components/CartIcon/Default.cshtml`
3. Output: Shopping cart icon with badge showing item count
4. Styling: Component-specific styles in its own view file

**Integration Point:**
```1251:1251:Pages/Shared/_Layout.cshtml
                    @await Component.InvokeAsync("CartIcon")
```

---

## 7. KEY DESIGN DECISIONS

### Why Multiple CSS Layers?
1. **Bootstrap CSS** - Base framework styles
2. **Embedded `<style>`** - Override Bootstrap, handle dropdown visibility
3. **Inline Styles** - Fine-tune positioning, overflow, specific elements
4. **Tailwind Classes** - Utility classes for logo hover effects
5. **Component Styles** - Isolated styles for CartIcon

### Why Custom Dropdown Logic?
- Bootstrap's default behavior conflicts with strict hiding requirements
- Need to ensure dropdowns are hidden by default (multiple CSS rules)
- Custom SVG chevron instead of Bootstrap's `::after` pseudo-element
- Complex overflow handling for navbar container

### Why Separate Desktop/Mobile Navs?
- Different styling (uppercase vs. title case)
- Different layouts (centered vs. stacked)
- Different dropdown IDs for proper Bootstrap initialization
- Better control over responsive behavior

---

## 8. POTENTIAL ISSUES & COMPLEXITY

### CSS Specificity Wars
- Multiple `!important` declarations
- Overlapping selectors for same elements
- Inline styles competing with CSS classes

### JavaScript Complexity
- Bootstrap initialization + fallback manual toggle
- Event handlers clearing inline styles
- Multiple event listeners on same elements

### Responsive Complexity
- Two separate navigation structures
- Different breakpoints for different elements
- Conditional visibility based on multiple factors

### Dropdown Visibility Logic
- CSS rules with `!important` forcing hidden state
- Bootstrap adding `.show` class
- JavaScript clearing inline styles
- Multiple backup triggers (`aria-expanded`, `.show` class)

---

## SUMMARY

The navigation bar is a **sophisticated, multi-layered component** that combines:

1. **HTML:** Bootstrap structure with custom elements and Razor syntax
2. **CSS:** Bootstrap base + embedded overrides + inline styles + Tailwind utilities
3. **JavaScript:** Bootstrap initialization + custom event handlers + theme toggle + scroll effects
4. **Components:** Server-side ViewComponent for cart icon
5. **Responsive:** Separate mobile/desktop structures with complex breakpoint logic

The dropdown functionality is particularly complex due to the need to override Bootstrap's default behavior while maintaining accessibility and responsive design.

