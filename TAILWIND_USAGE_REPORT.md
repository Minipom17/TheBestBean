# Tailwind CSS Usage Report

## Summary
Tailwind CSS is being used in **3 main areas** of the application, with most usage having inline style fallbacks (suggesting uncertainty about Tailwind working properly).

---

## 1. TAILWIND CDN & CONFIG

### Location: `Pages/Shared/_Layout.cshtml`

**Line 110:** Tailwind CDN loaded
```html
<script src="https://cdn.tailwindcss.com"></script>
```

**Lines 115-137:** Tailwind config defined
```javascript
tailwind.config = {
    darkMode: 'class',
    theme: {
        extend: {
            colors: {
                'swiss-red': '#FF3B30',
                'off-white': '#F9F9F9',
                'charcoal': '#111111',
                'dark-bg': '#0a0a0a',
                'dark-card': '#111111',
                'dark-border': 'rgba(255, 255, 255, 0.1)',
                'cacao-brown': '#5D4037',
            },
            fontFamily: {
                'sans': ['"Inter"', 'sans-serif'],
                'grotesk': ['"Space Grotesk"', 'sans-serif'],
            },
            transitionProperty: {
                'colors': 'background-color, border-color, color, fill, stroke',
            }
        }
    }
}
```

---

## 2. NAVBAR LOGO (Minimal Usage)

### Location: `Pages/Shared/_Layout.cshtml` (Lines 1151-1153)

**Tailwind Classes Used:**
- `d-flex` (Bootstrap, not Tailwind - but mixed)
- `gap-3` (Tailwind)
- `hover:text-swiss-red` (Tailwind custom color)
- `transition-colors` (Tailwind)
- `group` (Tailwind)
- `font-grotesk` (Tailwind custom font)
- `font-bold` (Tailwind)
- `text-4xl` (Tailwind)
- `tracking-tighter` (Tailwind)
- `leading-none` (Tailwind)
- `text-charcoal` (Tailwind custom color)
- `dark:text-white` (Tailwind dark mode)
- `group-hover:text-swiss-red` (Tailwind)
- `flex-column` (Bootstrap)
- `text-[10px]` (Tailwind arbitrary value)
- `font-mono` (Tailwind)
- `leading-tight` (Tailwind)
- `uppercase` (Tailwind)
- `tracking-widest` (Tailwind)
- `border-l-2` (Tailwind)
- `border-charcoal` (Tailwind custom color)
- `dark:border-white` (Tailwind dark mode)
- `group-hover:border-swiss-red` (Tailwind)
- `pl-2` (Tailwind)

**Note:** All of these have inline style fallbacks, suggesting the developer wasn't confident Tailwind would work.

---

## 3. SHOP PAGE (Extensive Usage)

### Location: `Pages/Shop.cshtml`

**Major Tailwind Usage Areas:**

#### A. Header Section (Lines 30-36)
- `py-5` (Tailwind)
- `flex flex-col md:flex-row` (Tailwind responsive)
- `justify-between` (Tailwind)
- `items-end` (Tailwind)
- `mb-12` (Tailwind)
- `border-b border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `pb-8` (Tailwind)
- `inline-block` (Tailwind)
- `px-3 py-1` (Tailwind)
- `bg-black dark:bg-white` (Tailwind dark mode)
- `text-white dark:text-black` (Tailwind dark mode)
- `text-xs` (Tailwind)
- `font-bold font-mono` (Tailwind)
- `mb-6` (Tailwind)
- `font-grotesk` (Tailwind custom)
- `text-5xl md:text-7xl` (Tailwind responsive)
- `tracking-tight` (Tailwind)
- `leading-none` (Tailwind)

#### B. Starter Kit Section (Lines 149-350+)
- `py-3` (Tailwind)
- `border border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `mb-12` (Tailwind)
- `hover:border-gray-300` (Tailwind)
- `dark:hover:border-white/20` (Tailwind dark mode)
- `transition-all duration-300` (Tailwind)
- `group` (Tailwind)
- `bg-gray-50 dark:bg-[#111]` (Tailwind dark mode)
- `p-12` (Tailwind)
- `d-flex flex-column` (Bootstrap mixed with Tailwind)
- `justify-content-center` (Bootstrap)
- `align-items-center` (Bootstrap)
- `border-bottom border-lg-bottom-0` (Bootstrap)
- `border-lg-end` (Bootstrap)
- `border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `position-relative` (Bootstrap)
- `overflow-hidden` (Tailwind)
- `h-48` (Tailwind)
- `mx-auto` (Tailwind)
- `object-contain` (Tailwind)
- `dark:brightness-90` (Tailwind dark mode)
- `font-mono` (Tailwind)
- `text-xs` (Tailwind)
- `uppercase` (Tailwind)
- `text-gray-400` (Tailwind)
- `mt-3` (Tailwind)
- `d-block` (Bootstrap)
- `w-32 h-32` (Tailwind)
- `bg-white dark:bg-black` (Tailwind dark mode)
- `rounded-lg` (Tailwind)
- `shadow-sm` (Tailwind)
- `border border-gray-100` (Tailwind)
- `dark:border-white/10` (Tailwind dark mode)
- `text-4xl` (Tailwind)
- `p-8 md:p-12` (Tailwind responsive)
- `bg-white dark:bg-[#111]` (Tailwind dark mode)
- `bg-yellow-400` (Tailwind)
- `text-black` (Tailwind)
- `text-[10px]` (Tailwind arbitrary)
- `px-2 py-1` (Tailwind)
- `tracking-wider` (Tailwind)
- `text-green-600 dark:text-green-400` (Tailwind dark mode)
- `text-3xl md:text-4xl` (Tailwind responsive)
- `mb-4` (Tailwind)
- `font-sans` (Tailwind)
- `text-gray-600 dark:text-gray-400` (Tailwind dark mode)
- `mb-8` (Tailwind)
- `leading-relaxed` (Tailwind)
- `max-w-lg` (Tailwind)
- `text-sm` (Tailwind)
- `text-gray-700 dark:text-gray-300` (Tailwind dark mode)
- `text-green-500` (Tailwind)

**Note:** This section has **extensive Tailwind usage** with many inline style fallbacks.

---

## 4. INDEX PAGE (Extensive Usage)

### Location: `Pages/Index.cshtml`

**Major Tailwind Usage Areas:**

#### A. Hero Section (Lines 7-19)
- `px-0` (Tailwind)
- `border-b border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `flex flex-col md:flex-row` (Tailwind responsive)
- `justify-between` (Tailwind)
- `items-end` (Tailwind)
- `mb-12` (Tailwind)
- `pb-8` (Tailwind)
- `transition-colors duration-500` (Tailwind)
- `inline-block` (Tailwind)
- `px-3 py-1` (Tailwind)
- `bg-black dark:bg-white` (Tailwind dark mode)
- `text-white dark:text-black` (Tailwind dark mode)
- `text-xs` (Tailwind)
- `font-bold font-mono` (Tailwind)
- `mb-6` (Tailwind)
- `font-grotesk` (Tailwind custom)
- `text-5xl md:text-8xl` (Tailwind responsive)
- `tracking-tight` (Tailwind)
- `leading-none` (Tailwind)
- `mt-8 md:mt-0` (Tailwind responsive)
- `text-sm` (Tailwind)
- `text-gray-500 dark:text-gray-400` (Tailwind dark mode)
- `max-w-xs` (Tailwind)
- `text-right` (Tailwind)

#### B. Hero Grid (Lines 22-62)
- `grid grid-cols-1 lg:grid-cols-12` (Tailwind responsive grid)
- `gap-0` (Tailwind)
- `border-t border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `lg:col-span-8` (Tailwind responsive)
- `border-r border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `border-b dark:border-b-dark-border` (Tailwind dark mode)
- `relative` (Tailwind)
- `group` (Tailwind)
- `overflow-hidden` (Tailwind)
- `cursor-pointer` (Tailwind)
- `aspect-[16/10]` (Tailwind arbitrary)
- `bg-gray-100 dark:bg-[#111]` (Tailwind dark mode)
- `w-full h-full` (Tailwind)
- `object-cover` (Tailwind)
- `transition-transform duration-700` (Tailwind)
- `group-hover:scale-105` (Tailwind)
- `dark:opacity-90` (Tailwind dark mode)
- `dark:group-hover:opacity-100` (Tailwind dark mode)
- `absolute bottom-0 left-0` (Tailwind)
- `p-8` (Tailwind)
- `bg-gradient-to-t from-black/80 to-transparent` (Tailwind gradient)
- `font-mono` (Tailwind)
- `text-white/80` (Tailwind opacity)
- `text-xs` (Tailwind)
- `mb-2` (Tailwind)
- `tracking-wider` (Tailwind)
- `text-white` (Tailwind)
- `text-3xl md:text-5xl` (Tailwind responsive)
- `font-grotesk` (Tailwind custom)
- `font-medium` (Tailwind)
- `lg:col-span-4` (Tailwind responsive)
- `flex flex-col` (Tailwind)
- `flex-1` (Tailwind)
- `p-8` (Tailwind)
- `flex justify-between` (Tailwind)
- `items-start` (Tailwind)
- `text-xl` (Tailwind)
- `group-hover:text-swiss-red` (Tailwind custom)
- `transition-colors` (Tailwind)
- `-rotate-45` (Tailwind)
- `group-hover:rotate-0` (Tailwind)
- `transition-transform duration-300` (Tailwind)
- `self-center` (Tailwind)
- `my-6` (Tailwind)
- `hidden dark:block` (Tailwind dark mode)
- `inset-0` (Tailwind)
- `bg-white/5` (Tailwind opacity)
- `blur-2xl` (Tailwind)
- `rounded-full` (Tailwind)
- `transform scale-75` (Tailwind)
- `group-hover:scale-100` (Tailwind)
- `transition-transform duration-500` (Tailwind)
- `h-40` (Tailwind)
- `mix-blend-multiply` (Tailwind)
- `dark:mix-blend-normal` (Tailwind dark mode)
- `dark:brightness-90` (Tailwind dark mode)
- `dark:contrast-125` (Tailwind dark mode)
- `relative z-10` (Tailwind)
- `group-hover:scale-110` (Tailwind)
- `items-end` (Tailwind)
- `border-t border-gray-200` (Tailwind)
- `dark:border-dark-border` (Tailwind dark mode)
- `pt-4` (Tailwind)
- `hover:bg-off-white` (Tailwind custom)
- `dark:hover:bg-white/5` (Tailwind dark mode)
- `border-r dark:border-r-dark-border` (Tailwind dark mode)
- `lg:border-r-0` (Tailwind responsive)
- `hover:bg-black` (Tailwind)
- `hover:text-white` (Tailwind)
- `dark:hover:bg-white` (Tailwind dark mode)
- `dark:hover:text-black` (Tailwind dark mode)
- `justify-center` (Tailwind)
- `text-decoration-none` (Tailwind)
- `mb-3` (Tailwind)
- `opacity-60` (Tailwind)
- `text-3xl` (Tailwind)
- `font-bold` (Tailwind)
- `leading-tight` (Tailwind)
- `mb-6` (Tailwind)
- `items-center` (Tailwind)
- `justify-between` (Tailwind)
- `mt-auto` (Tailwind)
- `border-t border-gray-300` (Tailwind)
- `dark:border-white/20` (Tailwind dark mode)
- `group-hover:border-white/30` (Tailwind)
- `dark:group-hover:border-black/20` (Tailwind dark mode)
- `gap-2` (Tailwind)

**Note:** This page has **very extensive Tailwind usage** with inline style fallbacks throughout.

---

## 5. STATIC HTML FILES (Not Part of Main App)

### Location: `wwwroot/Media/BEANCARD.HTML` and `wwwroot/Media/index.html`

These appear to be standalone demo/static files and load Tailwind separately. They're not part of the main Razor Pages application.

---

## 6. CUSTOM CSS FALLBACKS

### Location: `Pages/Shop.cshtml` (Lines 616-629)

There are custom CSS rules that duplicate Tailwind classes:
```css
/* Ensure Tailwind classes work with Bootstrap */
.bg-yellow-400 {
    background-color: #facc15 !important;
}

.text-green-600 {
    color: #16a34a !important;
}

.dark .text-green-400 {
    color: #4ade80 !important;
}
```

This suggests the developer was unsure if Tailwind would work and added fallbacks.

---

## SUMMARY

### Tailwind Usage by File:

1. **`_Layout.cshtml`** - Minimal (navbar logo only)
2. **`Shop.cshtml`** - **EXTENSIVE** (equipment section, starter kit)
3. **`Index.cshtml`** - **EXTENSIVE** (hero section, grid layout)
4. **Static HTML files** - Separate (not part of main app)

### Key Observations:

1. **Heavy reliance on inline style fallbacks** - Every Tailwind class has a corresponding inline style, suggesting uncertainty about Tailwind working
2. **Mixed Bootstrap + Tailwind** - Many elements use both frameworks simultaneously
3. **Custom Tailwind config** - Defines custom colors and fonts
4. **Dark mode usage** - Extensive use of `dark:` variants
5. **Responsive utilities** - Heavy use of `md:`, `lg:` breakpoints

### Impact of Removing Tailwind:

- **Navbar logo** - Would need ~20 lines of custom CSS
- **Shop page** - Would need ~200+ lines of custom CSS
- **Index page** - Would need ~300+ lines of custom CSS

**Total estimated replacement:** ~500+ lines of custom CSS to replace Tailwind functionality.

---

## RECOMMENDATION

Given the extensive usage, you have two options:

1. **Remove Tailwind** - Replace with custom CSS (significant refactoring)
2. **Remove Bootstrap** - Go full Tailwind (also significant refactoring)

Since Bootstrap is more deeply integrated (navbar structure, dropdowns, grid system), **removing Tailwind and replacing with custom CSS** is the safer path, but will require substantial work on Shop.cshtml and Index.cshtml.

