const fs = require('fs');

const path = 'c:/Users/alext/source/repos/TheBestBean/TheBestBean/wwwroot/presentations/coffee-presentation.html';
let html = fs.readFileSync(path, 'utf8');

// Find all <h1> tags that look like slide titles.
// They typically have class="...text-sca-navy..." or similar.
const h1Regex = /<h1[^>]*>/g;

let match;
let count = 0;
let newHtml = html;

// We want to alternate: Black, Orange, Black, Orange.
// Or whatever index. Let's make index 0 Black, index 1 Orange, etc.
// But some slides might have text-white (like Aroma ID). Let's skip text-white ones.

const replacements = [];

while ((match = h1Regex.exec(html)) !== null) {
    const h1Tag = match[0];
    if (h1Tag.includes('text-white')) {
        continue;
    }
    
    // Check if it's an orange one or black one based on count
    const isOrange = count % 2 === 1; // 0=Black, 1=Orange, 2=Black, 3=Orange
    
    let newH1Tag = h1Tag;
    
    if (isOrange) {
        // Remove text-sca-navy
        newH1Tag = newH1Tag.replace(/\s*text-sca-navy\s*/, ' ');
        // Ensure no duplicate style
        newH1Tag = newH1Tag.replace(/\s*style="[^"]*"/, '');
        // Add style
        newH1Tag = newH1Tag.replace('class="', 'class="').replace('">', '" style="color: rgb(255 114 45 / 80%);">');
    } else {
        // Make it Black (text-sca-navy)
        // Ensure text-sca-navy is present if not
        if (!newH1Tag.includes('text-sca-navy')) {
             newH1Tag = newH1Tag.replace('class="', 'class="text-sca-navy ');
        }
        // Remove style
        newH1Tag = newH1Tag.replace(/\s*style="[^"]*"/, '');
    }
    
    // clean up spaces
    newH1Tag = newH1Tag.replace(/\s+class="/, ' class="').replace(/\s+style="/, ' style="');
    
    replacements.push({
        old: h1Tag,
        new: newH1Tag,
        index: match.index
    });
    
    count++;
}

// apply replacements from end to start
for (let i = replacements.length - 1; i >= 0; i--) {
    const rep = replacements[i];
    newHtml = newHtml.substring(0, rep.index) + rep.new + newHtml.substring(rep.index + rep.old.length);
}

fs.writeFileSync(path, newHtml);
console.log(`Replaced ${replacements.length} h1 tags.`);
