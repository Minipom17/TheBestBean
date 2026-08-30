# Dropdown Diagnostic - Root Cause Analysis

## Current HTML Structure

```html
<li class="nav-item dropdown">
    <button class="nav-link dropdown-toggle" 
            id="guideDropdown" 
            type="button" 
            data-bs-toggle="dropdown" 
            aria-expanded="false" 
            aria-haspopup="true">
        GUIDE
        <svg class="dropdown-toggle-icon">...</svg>
    </button>
    <ul class="dropdown-menu dropdown-menu-end" 
        aria-labelledby="guideDropdown">
        <li><a class="dropdown-item">...</a></li>
    </ul>
</li>
```

**Structure Analysis:**
- ✅ Button has `data-bs-toggle="dropdown"` - CORRECT
- ✅ Button has `id="guideDropdown"` - CORRECT
- ✅ Menu has `aria-labelledby="guideDropdown"` - CORRECT
- ✅ Menu is sibling of button - CORRECT
- ✅ Parent has `.dropdown` class - CORRECT

**Bootstrap Requirements: MET**

---

## CSS Rules Analysis

### Rule 1: Hide by Default (Lines 33-41)
```css
.dropdown-menu:not(.show),
ul.dropdown-menu:not(.show),
.navbar-nav .dropdown-menu:not(.show),
.dropdown .dropdown-menu:not(.show) {
    display: none !important;
    visibility: hidden !important;
    opacity: 0 !important;
    pointer-events: none !important;
}
```
**Analysis:** This should work - only hides menus WITHOUT `.show` class.

### Rule 2: Show when .show is present (Lines 44-55)
```css
.dropdown-menu.show,
ul.dropdown-menu.show,
.navbar-nav .dropdown-menu.show,
.nav-item.dropdown .dropdown-menu.show,
.dropdown .dropdown-menu.show,
li.dropdown .dropdown-menu.show {
    display: block !important;
    visibility: visible !important;
    opacity: 1 !important;
    pointer-events: auto !important;
    position: absolute !important;
}
```
**Analysis:** Multiple selectors, should work. BUT - CSS specificity might be an issue.

### Rule 3: Desktop-specific (Lines 72-96)
```css
@media (min-width: 1200px) {
    .navbar-nav.d-xl-flex .dropdown-menu {
        position: absolute !important;
        top: 100% !important;
        left: auto !important;
        right: 0 !important;
        z-index: 1055 !important;
    }
    
    .navbar-nav.d-xl-flex .dropdown-menu.show,
    .navbar-nav.d-xl-flex .dropdown.show .dropdown-menu {
        display: block !important;
        visibility: visible !important;
        opacity: 1 !important;
        pointer-events: auto !important;
    }
    
    .navbar-nav.d-xl-flex .dropdown-menu:not(.show) {
        display: none !important;
        visibility: hidden !important;
        opacity: 0 !important;
        pointer-events: none !important;
    }
}
```
**Analysis:** This is DESKTOP ONLY (≥1200px). If testing on smaller screen, this won't apply!

### Rule 4: Duplicate hiding rule (Line 236)
```css
.dropdown-menu:not(.show) {
    overflow: visible !important;
    max-height: none !important;
    display: none !important;
}
```
**Analysis:** This is a DUPLICATE of Rule 1, but less specific. Should be fine.

---

## JavaScript Analysis

### Initialization Flow:
1. DOMContentLoaded fires
2. Finds all `.dropdown-toggle` buttons
3. Finds menu via `aria-labelledby` or `nextElementSibling`
4. Removes `.show` class initially
5. Tries to initialize Bootstrap Dropdown
6. Adds event listeners for Bootstrap events

### Potential Issues:

**Issue 1: Click Handler Interference (Lines 1491-1507)**
```javascript
document.addEventListener('click', function(e) {
    if (!e.target.closest('.dropdown')) {
        // Closes all dropdowns
    }
});
```
**Problem:** This runs on EVERY click. If Bootstrap's click handler runs first and opens the dropdown, this might immediately close it if the click event bubbles incorrectly.

**Issue 2: Inline Style Removal (Lines 1452-1456)**
```javascript
dropdownMenu.style.removeProperty('display');
dropdownMenu.style.removeProperty('visibility');
// etc.
```
**Problem:** Bootstrap might be trying to set inline styles, and we're removing them. This could cause a race condition.

**Issue 3: Event Listener Order**
- Bootstrap's default click handler
- Our custom click handler (if Bootstrap didn't initialize)
- Our Bootstrap event listeners (show.bs.dropdown, etc.)
- Our document-level click handler

These might be conflicting.

---

## Bootstrap Dropdown Behavior

### How Bootstrap Dropdowns Work:
1. User clicks button with `data-bs-toggle="dropdown"`
2. Bootstrap's data API handler intercepts the click
3. Bootstrap finds the dropdown menu (sibling or via aria-labelledby)
4. Bootstrap adds `.show` class to menu
5. Bootstrap sets `aria-expanded="true"` on button
6. Bootstrap fires `show.bs.dropdown` event
7. Bootstrap fires `shown.bs.dropdown` event

### Special Navbar Behavior:
- Bootstrap detects if dropdown is in `.navbar`
- Disables Popper.js positioning (uses static positioning)
- Relies on CSS for positioning

---

## ROOT CAUSE HYPOTHESIS

### Most Likely Issues (in order of probability):

1. **CSS Specificity Conflict**
   - Multiple rules with `!important`
   - Desktop media query might not match
   - Rule order might be wrong

2. **JavaScript Interference**
   - Document click handler closing dropdown immediately
   - Inline style removal conflicting with Bootstrap
   - Event propagation issues

3. **Bootstrap Not Initializing**
   - Bootstrap JS not loaded
   - Bootstrap already initialized elsewhere
   - Error during initialization

4. **CSS Rule Order**
   - Later rules overriding earlier ones
   - Media queries not matching viewport size

---

## DIAGNOSTIC STEPS NEEDED

1. **Check Browser Console:**
   - Is Bootstrap loaded? (`typeof bootstrap`)
   - Are there JavaScript errors?
   - Do the `[DROPDOWN]` console logs appear?
   - What do they say?

2. **Inspect Element:**
   - Does the menu have `.show` class when clicked?
   - What are the computed styles?
   - Is `display: none` still being applied?

3. **Check CSS Specificity:**
   - Which CSS rule is actually winning?
   - Is the media query matching?
   - Are there conflicting `!important` rules?

4. **Test Bootstrap Directly:**
   - Try initializing dropdown manually in console
   - Check if Bootstrap events fire
   - Verify Bootstrap can find the menu

---

## TESTING CHECKLIST

- [ ] Open browser console (F12)
- [ ] Check if `typeof bootstrap` returns "object"
- [ ] Click GUIDE button
- [ ] Check console for `[DROPDOWN]` logs
- [ ] Inspect the `<ul class="dropdown-menu">` element
- [ ] Check if `.show` class is added
- [ ] Check computed styles (especially `display`)
- [ ] Check if any JavaScript errors appear
- [ ] Test on different screen sizes (desktop vs mobile)
- [ ] Check if Bootstrap events fire (`show.bs.dropdown`)

---

## NEXT STEPS

Before making any changes, we need to:
1. Understand what's happening in the browser
2. Identify which layer is failing (CSS, JS, or Bootstrap)
3. Determine the exact root cause
4. Then fix it with a targeted solution

