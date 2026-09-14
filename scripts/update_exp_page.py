import re

file_path = r'c:\Users\alext\source\repos\TheBestBean\TheBestBean\Pages\Experiences.cshtml'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# 1. Add functions block
functions_block = """@Html.AntiForgeryToken()

@functions {
    public static string GetTimeCategory(string duration) {
        if (string.IsNullOrEmpty(duration)) return "all";
        var d = duration.ToLower();
        if (d.Contains("1 hour")) return "1-hour";
        if (d.Contains("2.5") || d.Contains("2") || d.Contains("3") || d.Contains("4")) return "2-5-hours";
        if (d.Contains("day") && !d.Contains("days") && !d.Contains("multi")) return "day";
        if (d.Contains("days") || d.Contains("multi")) return "multi-day";
        return "all";
    }
}"""
content = content.replace('@Html.AntiForgeryToken()', functions_block)

# 2. Add Time filter (Urban Labs)
urban_time_filter = """                    <div class="exp-filter-row mt-4">
                        <span class="exp-filter-label">Time</span>
                        <div class="exp-when" id="urban-time-filters" role="group" aria-label="Filter by time">
                            <button type="button" class="exp-when-btn filter-btn active" data-filter="all">All</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="1-hour">1 hours</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="2-5-hours">2.5 hours</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="day">day</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="multi-day">multi - day</button>
                        </div>
                    </div>"""
content = re.sub(
    r'(<div class="exp-when" id="urban-frequency-filters" role="group" aria-label="Filter by schedule">[\s\S]*?</div>\s*</div>)',
    r'\1\n' + urban_time_filter,
    content
)

# 3. Add Time filter (Expeditions)
exp_time_filter = """                    <div class="exp-filter-row mt-4">
                        <span class="exp-filter-label">Time</span>
                        <div class="exp-when" id="expedition-time-filters" role="group" aria-label="Filter by time">
                            <button type="button" class="exp-when-btn filter-btn active" data-filter="all">All</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="1-hour">1 hours</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="2-5-hours">2.5 hours</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="day">day</button>
                            <button type="button" class="exp-when-btn filter-btn" data-filter="multi-day">multi - day</button>
                        </div>
                    </div>"""
content = re.sub(
    r'(<div class="exp-when" id="expedition-frequency-filters" role="group" aria-label="Filter by schedule">[\s\S]*?</div>\s*</div>)',
    r'\1\n' + exp_time_filter,
    content
)

# 4. Add data-time to lab-item and expedition-item
content = re.sub(
    r'(<article class="lab-item flex flex-col group w-full" data-region="@featuredGeo.Region" data-place="@featuredGeo.PlaceKey" data-frequency="@ExperienceGeo.WhenKey\(featuredLab.Month\)")',
    r'\1 data-time="@GetTimeCategory(featuredLab.Duration)"',
    content
)
content = re.sub(
    r'(<article class="lab-item flex flex-col group" data-region="@labGeo.Region" data-place="@labGeo.PlaceKey" data-frequency="@ExperienceGeo.WhenKey\(exp.Month\)")',
    r'\1 data-time="@GetTimeCategory(exp.Duration)"',
    content
)
content = re.sub(
    r'(<article class="expedition-item flex flex-col group" data-region="@expeditionGeo.Region" data-place="@expeditionGeo.PlaceKey" data-frequency="@ExperienceGeo.WhenKey\(exp.Month\)")',
    r'\1 data-time="@GetTimeCategory(exp.Duration)"',
    content
)

# 5. Update JS filter logic
js_replacement = """            const labItems = document.querySelectorAll('.lab-item');
            let currentUrbanLocation = 'all';
            let currentUrbanFrequency = 'all';
            let currentUrbanTime = 'all';

            function filterUrbanLabs() {
                labItems.forEach(item => {
                    const itemFrequency = item.getAttribute('data-frequency') || 'all';
                    const itemTime = item.getAttribute('data-time') || 'all';
                    const matchesLocation = matchesGeo(item, currentUrbanLocation);
                    const matchesFrequency = currentUrbanFrequency === 'all' || itemFrequency === currentUrbanFrequency;
                    const matchesTime = currentUrbanTime === 'all' || itemTime === currentUrbanTime;
                    item.style.display = (matchesLocation && matchesFrequency && matchesTime) ? 'flex' : 'none';
                });
            }

            bindFilterGroup('#urban-location-filters .filter-btn', value => {
                currentUrbanLocation = value;
                filterUrbanLabs();
            });
            bindFilterGroup('#urban-frequency-filters .filter-btn', value => {
                currentUrbanFrequency = value;
                filterUrbanLabs();
            });
            bindFilterGroup('#urban-time-filters .filter-btn', value => {
                currentUrbanTime = value;
                filterUrbanLabs();
            });

            const expeditionItems = document.querySelectorAll('.expedition-item');
            let currentExpeditionLocation = 'all';
            let currentExpeditionFrequency = 'all';
            let currentExpeditionTime = 'all';

            function filterExpeditions() {
                expeditionItems.forEach(item => {
                    const itemFrequency = item.getAttribute('data-frequency') || 'all';
                    const itemTime = item.getAttribute('data-time') || 'all';
                    const matchesLocation = matchesGeo(item, currentExpeditionLocation);
                    const matchesFrequency = currentExpeditionFrequency === 'all' || itemFrequency === currentExpeditionFrequency;
                    const matchesTime = currentExpeditionTime === 'all' || itemTime === currentExpeditionTime;
                    item.style.display = (matchesLocation && matchesFrequency && matchesTime) ? 'flex' : 'none';
                });
            }

            bindFilterGroup('#expedition-location-filters .filter-btn', value => {
                currentExpeditionLocation = value;
                filterExpeditions();
            });
            bindFilterGroup('#expedition-frequency-filters .filter-btn', value => {
                currentExpeditionFrequency = value;
                filterExpeditions();
            });
            bindFilterGroup('#expedition-time-filters .filter-btn', value => {
                currentExpeditionTime = value;
                filterExpeditions();
            });"""
content = re.sub(
    r'const labItems = document\.querySelectorAll\(\'\.lab-item\'\);[\s\S]*?bindFilterGroup\(\'#expedition-frequency-filters \.filter-btn\', value => \{\s*currentExpeditionFrequency = value;\s*filterExpeditions\(\);\s*\}\);',
    js_replacement,
    content
)

# 6. Update Syllabus for featured lab
old_featured_syllabus = """                                                var isLast = i == featuredLab.Syllabus.Count - 1;
                                                var paddingClasses = isLast ? "pt-4 pb-0" : "py-4 border-b border-v-light-gray";
                                                <div class="flex flex-col gap-1 @paddingClasses">
                                                    <span class="font-mono text-[10px] font-bold text-v-black dark:text-[#D6D0C8] uppercase tracking-widest">PART 0@(i + 1)</span>
                                                    <span class="text-sm @(isLast ? "font-semibold text-v-black dark:text-[#D6D0C8]" : "font-normal text-v-gray")">@Html.Raw(featuredLab.Syllabus[i])</span>
                                                </div>"""
new_featured_syllabus = """                                                var isLast = i == featuredLab.Syllabus.Count - 1;
                                                var paddingClasses = isLast ? "pt-4 pb-0" : "py-4 border-b border-v-light-gray";
                                                var syllabusItem = featuredLab.Syllabus[i];
                                                var detailParts = syllabusItem.Split(new string[] { "||" }, 2, StringSplitOptions.None);
                                                var mainPart = detailParts[0];
                                                var detailBody = detailParts.Length > 1 ? detailParts[1].Trim() : null;
                                                <div class="flex flex-col gap-1 @paddingClasses">
                                                    <span class="font-mono text-[10px] font-bold text-v-black dark:text-[#D6D0C8] uppercase tracking-widest">PART 0@(i + 1)</span>
                                                    <span class="text-sm @(isLast ? "font-semibold text-v-black dark:text-[#D6D0C8]" : "font-normal text-v-gray")">@Html.Raw(mainPart)</span>
                                                    @if (!string.IsNullOrWhiteSpace(detailBody))
                                                    {
                                                        <details class="tour-syllabus-details mt-2">
                                                            <summary class="tour-syllabus-details__summary">
                                                                <span class="text-[10px] font-bold uppercase tracking-widest font-mono-jb text-v-gray">details</span>
                                                                <span class="tour-syllabus-details__chevron" aria-hidden="true"></span>
                                                            </summary>
                                                            <div class="tour-syllabus-details__body text-sm text-v-gray leading-relaxed font-normal pt-2">
                                                                @Html.Raw(detailBody)
                                                            </div>
                                                        </details>
                                                    }
                                                </div>"""
content = content.replace(old_featured_syllabus, new_featured_syllabus)

# 7. Update Syllabus for expeditions
old_exp_syllabus = """                                                var isLast = i == exp.Syllabus.Count - 1;
                                                var paddingClasses = isLast ? "pt-4 pb-0" : "py-4 border-b border-v-light-gray";
                                                
                                                <div class="flex flex-col gap-1 @paddingClasses">
                                                    <span class="font-mono text-[10px] font-bold text-v-black dark:text-[#D6D0C8] uppercase tracking-widest">STEP 0@(i + 1)</span>
                                                    <span class="text-sm @(isLast ? "font-semibold text-v-black dark:text-[#D6D0C8]" : "font-normal text-v-gray")">@Html.Raw(exp.Syllabus[i])</span>
                                                </div>"""
new_exp_syllabus = """                                                var isLast = i == exp.Syllabus.Count - 1;
                                                var paddingClasses = isLast ? "pt-4 pb-0" : "py-4 border-b border-v-light-gray";
                                                var syllabusItem = exp.Syllabus[i];
                                                var detailParts = syllabusItem.Split(new string[] { "||" }, 2, StringSplitOptions.None);
                                                var mainPart = detailParts[0];
                                                var detailBody = detailParts.Length > 1 ? detailParts[1].Trim() : null;
                                                
                                                <div class="flex flex-col gap-1 @paddingClasses">
                                                    <span class="font-mono text-[10px] font-bold text-v-black dark:text-[#D6D0C8] uppercase tracking-widest">STEP 0@(i + 1)</span>
                                                    <span class="text-sm @(isLast ? "font-semibold text-v-black dark:text-[#D6D0C8]" : "font-normal text-v-gray")">@Html.Raw(mainPart)</span>
                                                    @if (!string.IsNullOrWhiteSpace(detailBody))
                                                    {
                                                        <details class="tour-syllabus-details mt-2">
                                                            <summary class="tour-syllabus-details__summary">
                                                                <span class="text-[10px] font-bold uppercase tracking-widest font-mono-jb text-v-gray">details</span>
                                                                <span class="tour-syllabus-details__chevron" aria-hidden="true"></span>
                                                            </summary>
                                                            <div class="tour-syllabus-details__body text-sm text-v-gray leading-relaxed font-normal pt-2">
                                                                @Html.Raw(detailBody)
                                                            </div>
                                                        </details>
                                                    }
                                                </div>"""
content = content.replace(old_exp_syllabus, new_exp_syllabus)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)
print("File updated successfully.")
