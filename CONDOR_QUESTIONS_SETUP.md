# Condor Questions Setup Guide

## Overview
This guide shows you how to manage questions in Google Sheets and have them automatically appear on the Condor survey page.

## Step 1: Create Questions Sheet in Google Sheets

1. Open your Google Sheet: https://docs.google.com/spreadsheets/d/1GM_MMyk633WtSGnr-OaXNpVexczQ45bPx_Lx33uEEqk/edit
2. Create a new sheet/tab called **"Questions"**
3. Set up the following structure:

### Questions Sheet Structure

| Page | Question Number | Question Text | Field ID | Option 1 | Option 2 | Option 3 | Option 4 | Option 5 | Multi-Select | Grid Count |
|------|----------------|---------------|----------|---------|---------|-----------|-----------|-----------|--------------|------------|
| 1 | 1 | Flavor Strength? | q1 | Too Weak | A little Weak | Just Right | A little Strong | Too Strong | FALSE | 5 |
| 1 | 2 | Sweetness? | q2 | Not Sweet Enough | A little Savory | Just Right | A little Too Sweet | Far Too Sweet | FALSE | 5 |
| 2 | 3 | Nicotine Kick? | q3 | Way Weaker | A little Weaker | Perfect | A little Stronger | Way Stronger | FALSE | 5 |
| 2 | 4 | Mouth Feel? | q4 | Irritating / Burn | Slight Burn | Pleasant Tingle | Smooth | No Feeling | FALSE | 5 |
| 3 | 5 | What do you prefer? (Pick up to 2) | q5 | More Sweet 🍭 | Less Sweet 🧂 | Minty / Ice ❄️ | Fruity 🍓 | | TRUE | 4 |
| 4 | Vote | IS THIS FLAVOR A WINNER? | vote | YES! 😍 | MAYBE 🤔 | NO 😢 | | | FALSE | 3 |

### Column Descriptions:
- **Page**: Which page the question appears on (1-4)
- **Question Number**: Order within the page
- **Question Text**: The label text shown to users
- **Field ID**: The form field name (q1, q2, q3, q4, q5, vote)
- **Option 1-5**: The button text for each option (leave empty if not needed)
- **Multi-Select**: TRUE if users can select multiple options, FALSE for single select
- **Grid Count**: Number of options (2, 3, 4, or 5)

## Step 2: Update Google Apps Script

1. Go to **Extensions → Apps Script** in your Google Sheet
2. Add a new function to fetch questions:

```javascript
function doGet(e) {
  // Fetch questions from the "Questions" sheet
  const ss = SpreadsheetApp.getActiveSpreadsheet();
  const questionsSheet = ss.getSheetByName('Questions');
  
  if (!questionsSheet) {
    return ContentService.createTextOutput(JSON.stringify({error: 'Questions sheet not found'}))
      .setMimeType(ContentService.MimeType.JSON);
  }
  
  const data = questionsSheet.getDataRange().getValues();
  const headers = data[0];
  
  // Skip header row
  const questions = [];
  for (let i = 1; i < data.length; i++) {
    const row = data[i];
    if (!row[0]) break; // Stop at first empty row
    
    const question = {
      page: parseInt(row[0]) || 1,
      questionNumber: row[1] || '',
      questionText: row[2] || '',
      fieldId: row[3] || '',
      options: [],
      multiSelect: row[9] === 'TRUE' || row[9] === true,
      gridCount: parseInt(row[10]) || 2
    };
    
    // Add options (columns 4-8)
    for (let j = 4; j <= 8; j++) {
      if (row[j] && row[j].toString().trim()) {
        question.options.push(row[j].toString().trim());
      }
    }
    
    questions.push(question);
  }
  
  // Group by page
  const pages = {};
  questions.forEach(q => {
    if (!pages[q.page]) pages[q.page] = [];
    pages[q.page].push(q);
  });
  
  return ContentService.createTextOutput(JSON.stringify({pages: pages}))
    .setMimeType(ContentService.MimeType.JSON);
}
```

3. **Deploy as Web App:**
   - Click **Deploy → New deployment**
   - Choose type: **Web app**
   - Execute as: **Me**
   - Who has access: **Anyone**
   - Click **Deploy**
   - Copy the **Web App URL** (you'll need this)

## Step 3: Update the HTML Page

The HTML page will be updated to:
1. Fetch questions from Google Sheets on page load
2. Dynamically render questions based on the data
3. Maintain all existing functionality

## Step 4: How to Update Questions

Simply edit the "Questions" sheet in Google Sheets:
- Change question text
- Add/remove options
- Change page numbers
- Modify multi-select settings

The changes will appear on the website after a page refresh (no code changes needed).

## Notes:
- The "vote" question (page 4) has special styling - it will always appear on the final page
- Page 4 also has hardcoded input fields (name, email, venue, notes) that won't change
- Make sure Field IDs match the hidden input names in the form (q1, q2, q3, q4, q5, vote)


