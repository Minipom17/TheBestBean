const fs = require('fs');
const path = require('path');

const filePath = path.join(__dirname, 'wwwroot', 'presentations', 'coffee-presentation.html');
let html = fs.readFileSync(filePath, 'utf8');

let count = 1;
// Replace <span>0XX</span>
html = html.replace(/<span[^>]*>\s*0\d\d\s*<\/span>/g, (match) => {
    const numStr = String(count).padStart(3, '0');
    count++;
    return match.replace(/0\d\d/, numStr);
});

// Update total pages
html = html.replace(/<span id="total-pages">\d+<\/span>/, `<span id="total-pages">${count - 1}</span>`);

fs.writeFileSync(filePath, html, 'utf8');
console.log(`Updated ${count - 1} page numbers.`);
