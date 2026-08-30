# Dropdown Root Cause Analysis

## Problem Statement
The "GUIDE" dropdown in the navbar does not open when clicked.

## Current Implementation Analysis

### 1. HTML Structure ✅ CORRECT
```html
<li class="nav-item dropdown">
    <button id="guideDropdown" 
            class="nav-link dropdown-toggle" 
            data-bs-toggle="dropdown"
            aria-expanded="false">
        GUIDE
    </button>
    <ul class="dropdown-menu dropdown-menu-end" 
        aria-labelledby="guideDropdown">
        <!-- menu items -->
    </ul>
</li>
```
**Status:** Structure matches Bootstrap requirements perfectly.

---

### 2. Bootstrap JavaScript Loading ✅ CORRECT
- Bootstrap 5.3.3 bundle loaded from CDN (line 1293)
- Loaded before our custom JavaScript
- Should be available globally as `bootstrap`

---

### 3. CSS Rules - POTENTIAL ISSUES

#### Issue A: Media Query Limitation
```css
@media (min-width: 1200px) {
    .navbar-nav.d-xl-flex .dropdown-menu.show {
        display: block !important;
    }
}
```
**Problem:** This rule ONLY applies on screens ≥1200px. If testing on smaller screen, this won't apply.

**However:** We also have base rules (lines 44-55) that should work on all screen sizes.

#### Issue B: CSS Specificity Conflict
We have multiple rules trying to show the dropdown:
1. `.dropdown-menu.show` (specificity: 0,1,1)
2. `.navbar-nav .dropdown-menu.show` (specificity: 0,2,1)  
3. `.nav-item.dropdown .dropdown-menu.show` (specificity: 0,3,1)
4. `@media (min-width: 1200px) .navbar-nav.d-xl-flex .dropdown-menu.show` (specificity: 0,4,1)

**Potential Issue:** If a more specific rule exists elsewhere that hides it, it could win.

#### Issue C: Duplicate Hiding Rule
Line 236 has another hiding rule:
```css
.dropdown-menu:not(.show) {
    display: none !important;
}
```
This is less specific than our other rules, so should be fine, but it's redundant.

---

### 4. JavaScript Initialization - POTENTIAL ISSUES

#### Issue A: Menu Finding Logic
```javascript
const dropdownMenu = dropdownId ? 
    document.querySelector(`[aria-labelledby="${dropdownId}"]`) : 
    toggle.nextElementSibling;
```
**Analysis:** This should work. The menu has `aria-labelledby="guideDropdown"` and button has `id="guideDropdown"`.

#### Issue B: Bootstrap Initialization
```javascript
new bootstrap.Dropdown(toggle);
```
**Potential Issue:** If Bootstrap is already initialized elsewhere, or if there's an error, this might fail silently.

#### Issue C: Event Handler Interference
```javascript
// Line 1440-1442: Logging handler (shouldn't interfere)
toggle.addEventListener('click', function(e) {
    console.log('[DROPDOWN] Click detected...');
});
```
**Analysis:** This just logs, doesn't prevent default, so shouldn't interfere.

#### Issue D: Document Click Handler - CRITICAL
```javascript
// Lines 1491-1507
document.addEventListener('click', function(e) {
    if (!e.target.closest('.dropdown')) {
        // Closes all dropdowns
        document.querySelectorAll('.dropdown-menu.show').forEach(function(menu) {
            menu.classList.remove('show');
        });
    }
});
```
**POTENTIAL ROOT CAUSE:** 
- This runs on EVERY click
- If the click event bubbles in an unexpected way, this might close the dropdown immediately after Bootstrap opens it
- The timing might be: Bootstrap opens → event bubbles → our handler closes it

**Test:** This handler should NOT fire when clicking the button (since button is inside `.dropdown`), but if there's an event propagation issue, it might.

---

### 5. Bootstrap Event Handlers

```javascript
toggle.addEventListener('show.bs.dropdown', function() {
    // Removes inline styles
    dropdownMenu.style.removeProperty('display');
    // ...
});
```
**Potential Issue:** We're removing inline styles that Bootstrap might be trying to set. But our CSS has `!important`, so this should be fine.

---

## ROOT CAUSE HYPOTHESIS

### Most Likely: Document Click Handler Timing Issue

**Theory:** 
1. User clicks "GUIDE" button
2. Bootstrap's click handler fires, adds `.show` class
3. Click event bubbles to document
4. Our document click handler fires
5. Handler checks `!e.target.closest('.dropdown')`
6. If the check fails (button might not be considered "inside" `.dropdown` due to event target), it closes the dropdown

**Why this could happen:**
- Event target might be the SVG icon inside the button, not the button itself
- `closest('.dropdown')` might not find the parent `<li class="dropdown">` if the event target is deeply nested

### Second Most Likely: CSS Specificity Issue

**Theory:**
- A more specific CSS rule is overriding our `.show` rules
- Or the media query limitation means rules don't apply on the test screen size
- Or CSS rule order is wrong

### Third Most Likely: Bootstrap Not Initializing

**Theory:**
- Bootstrap fails to initialize silently
- Our fallback handler doesn't work correctly
- Or Bootstrap can't find the menu

---

## DIAGNOSTIC PLAN

### Step 1: Check Browser Console
```javascript
// Run in browser console:
console.log('Bootstrap:', typeof bootstrap);
console.log('Dropdown instance:', bootstrap.Dropdown.getInstance(document.getElementById('guideDropdown')));
```

### Step 2: Test Manual Toggle
```javascript
// Run in browser console:
const btn = document.getElementById('guideDropdown');
const menu = document.querySelector('[aria-labelledby="guideDropdown"]');
menu.classList.add('show');
console.log('Menu classes:', menu.className);
console.log('Computed display:', window.getComputedStyle(menu).display);
```

### Step 3: Check Event Flow
- Add breakpoints in browser DevTools
- Click the button
- See which handlers fire and in what order
- Check if document click handler fires

### Step 4: Inspect CSS
- Inspect the dropdown menu element
- Check which CSS rules are applied
- Check computed styles
- See if `.show` class is present
- See if `display: none` is still being applied

---

## PROPOSED FIX STRATEGY

### Option 1: Fix Document Click Handler
Make it more robust to handle nested elements:
```javascript
document.addEventListener('click', function(e) {
    // Check if click is inside ANY dropdown (button, menu, or parent)
    const clickedInsideDropdown = e.target.closest('.dropdown') || 
                                  e.target.closest('.dropdown-toggle') ||
                                  e.target.closest('.dropdown-menu');
    
    if (!clickedInsideDropdown) {
        // Close dropdowns
    }
});
```

### Option 2: Simplify CSS
Remove conflicting rules and use simpler selectors:
- Remove duplicate hiding rules
- Ensure `.show` rules are more specific than hiding rules
- Test on all screen sizes

### Option 3: Let Bootstrap Handle It
Remove our custom JavaScript and let Bootstrap's default behavior work:
- Remove manual initialization
- Remove document click handler (Bootstrap has its own)
- Only keep icon rotation handlers

---

## RECOMMENDATION

**Before making changes, we need to:**
1. See browser console output
2. Inspect the element when clicked
3. Check computed styles
4. Verify Bootstrap is loaded and initialized
5. Test event flow

**Then we can make a targeted fix based on the actual root cause.**

