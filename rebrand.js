const fs = require('fs');
const path = require('path');

const rootDir = 'c:/Users/alext/source/repos/TheBestBean/TheBestBean';

function walkSync(dir, callback) {
    const files = fs.readdirSync(dir);
    files.forEach((file) => {
        var filepath = path.join(dir, file);
        const stats = fs.statSync(filepath);
        if (stats.isDirectory()) {
            if (file !== 'bin' && file !== 'obj' && file !== '.git' && file !== 'wwwroot') {
                walkSync(filepath, callback);
            }
        } else if (stats.isFile()) {
            if (filepath.endsWith('.cshtml') || filepath.endsWith('.cs') || filepath.endsWith('.html')) {
                callback(filepath);
            }
        }
    });
}

// Global Rebrand
walkSync(rootDir, (filepath) => {
    let content = fs.readFileSync(filepath, 'utf8');
    let original = content;

    // Standard Replacements
    content = content.replace(/LLAMA COFFEE/g, '12° SUR');
    content = content.replace(/Llama Coffee/g, '12° Sur');
    content = content.replace(/LLAMA Coffee/g, '12° Sur');
    content = content.replace(/info@llamacoffee\.com/g, 'info@12dsur.com');
    content = content.replace(/LLAMA/g, '12° Sur');

    if (content !== original) {
        fs.writeFileSync(filepath, content, 'utf8');
        console.log('Rebranded:', filepath);
    }
});

// Specifically update the navbar in _Header.cshtml
const headerPath = path.join(rootDir, 'Pages', 'Shared', '_Header.cshtml');
let header = fs.readFileSync(headerPath, 'utf8');
// It currently might look like:
// <span class="font-grotesk tracking-widest leading-none drop-shadow-sm flex flex-col items-center">
//     12° Sur
//     <span class="block text-xs uppercase tracking-[0.3em] font-sans font-medium text-sca-coral mt-1">Coffee</span>
// </span>
// Or it might already be changed by the global replace.
// Let's replace the whole span block.
header = header.replace(/<span class="font-grotesk tracking-widest leading-none drop-shadow-sm flex flex-col items-center">[\s\S]*?<\/span>/, 
    '<span class="font-grotesk tracking-widest leading-none drop-shadow-sm text-2xl font-bold flex items-center gap-2">12° Sur <span class="text-sca-coral text-xl font-light">| COFFEE</span></span>');

// Specifically update the Tours link in _Header.cshtml
header = header.replace(/<a asp-page="\/Tours"/g, '<a asp-page="/Experiences"');
header = header.replace(/>Tours<\/a>/g, '>Experiences</a>');
fs.writeFileSync(headerPath, header, 'utf8');
console.log('Updated _Header.cshtml Navbar & Tours links.');
