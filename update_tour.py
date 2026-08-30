import re

with open('Pages/Tour.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

new_content = """@page "/Tour/{id}"
@model TheBestBean.Pages.TourModel
@{
    Layout = "_Layout";
    ViewData["Title"] = $"12° SUR // {(Model.Tour?.Title?.ToUpper() ?? "DOSSIER")}";
}

@if (Model.Tour != null)
{
    <!-- Huge Hero Image (Rounded Corners) -->
    <div class="max-w-[1600px] mx-auto w-full px-4 md:px-12 mt-6">
        <div class="w-full aspect-[21/9] md:aspect-[3/1] bg-gray-100 overflow-hidden rounded-3xl relative shadow-sm">
             <img src="@Model.Tour.ImageUrl" alt="@Model.Tour.Title" class="absolute inset-0 w-full h-full object-cover" />
        </div>
    </div>

    <!-- Main Content -->
    <div class="flex-grow max-w-[1600px] mx-auto w-full px-6 md:px-12 relative pb-32 font-sans text-v-black bg-v-white mt-12 md:mt-24">
        
        <main class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-24 relative">
            
            <!-- Left Grid: Sticky Reservation Card -->
            <div class="lg:col-span-4 flex flex-col relative order-2 lg:order-1">
                <div class="lg:sticky lg:top-32 bg-white rounded-3xl border border-gray-100 p-8 shadow-sm">
                    <h3 class="text-4xl font-bold tracking-tight text-v-black mb-2">$@Model.Tour.Price.ToString("0") <span class="text-xs text-v-gray font-normal">USD / Participant</span></h3>
                    
                    <div class="flex flex-col gap-4 mt-8 pt-8 border-t border-gray-100">
                        <div class="flex justify-between items-center text-sm">
                            <span class="text-v-gray font-normal">Duration</span>
                            <span class="text-v-black font-semibold">@Model.Tour.Duration</span>
                        </div>
                        <div class="flex justify-between items-center text-sm">
                            <span class="text-v-gray font-normal">Location</span>
                            <span class="text-v-black font-semibold">@Model.Tour.Location</span>
                        </div>
                        <div class="flex justify-between items-center text-sm">
                            <span class="text-v-gray font-normal">Level</span>
                            <span class="text-v-black font-semibold">@Model.Tour.Level</span>
                        </div>
                    </div>

                    <form method="post" asp-page-handler="AddToCart" class="mt-8 pt-8 border-t border-gray-100">
                        <input type="hidden" name="tourId" value="@Model.Tour.Id" />
                        <input type="hidden" name="title" value="@Model.Tour.Title" />
                        <input type="hidden" name="price" value="@Model.Tour.Price" />
                        <input type="hidden" name="date" value="Oct 15, 2026 (10:00 AM)" />
                        <input type="hidden" name="participants" value="1" />
                        
                        <button type="submit" class="w-full bg-v-black text-v-white rounded-full py-4 font-bold tracking-tight text-lg hover:bg-v-red transition-colors border-0 cursor-pointer">
                            Reserve Position
                        </button>
                        <p class="text-[10px] text-v-gray text-center mt-4">Requires a 50% deposit to secure equipment.</p>
                    </form>
                </div>
            </div>

            <!-- Right Grid: Details & Syllabus -->
            <div class="lg:col-span-8 flex flex-col gap-16 pt-4 order-1 lg:order-2">
                
                <!-- Title Block -->
                <div>
                    <span class="text-[10px] font-bold uppercase tracking-widest text-v-gray font-mono-jb block mb-4">@Model.Tour.Type &bull; @Model.Tour.Duration</span>
                    <h1 class="text-5xl md:text-7xl font-bold tracking-tighter text-v-black leading-[0.9] m-0">
                        @Model.Tour.Title
                    </h1>
                </div>

                <!-- Overview -->
                <div>
                    <h2 class="text-2xl font-bold tracking-tight text-v-black leading-none mb-6">
                        Overview
                    </h2>
                    <p class="text-v-gray text-lg md:text-xl leading-relaxed font-normal max-w-3xl">
                        @Model.Tour.LongDescription
                    </p>
                </div>

                <!-- Syllabus & Timeline -->
                <div>
                    <h2 class="text-2xl font-bold tracking-tight text-v-black mb-8">Syllabus & Timeline</h2>
                    
                    <div class="flex flex-col relative pl-2">
                        <div class="absolute left-[11px] top-2 bottom-2 w-px bg-gray-200"></div>
                        @for (int i = 0; i < Model.Tour.Syllabus.Count; i++)
                        {
                            var moduleText = Model.Tour.Syllabus[i];
                            var parts = moduleText.Split(new[] { ':' }, 2);
                            var headerPart = parts.Length > 1 ? parts[0].Trim() : $"DAY {(i + 1)}";
                            var bodyPart = parts.Length > 1 ? parts[1].Trim() : moduleText;

                            <div class="relative pl-8 pb-10 last:pb-0">
                                <div class="absolute left-0 top-1.5 w-2.5 h-2.5 bg-v-black rounded-full shadow-[0_0_0_4px_white]"></div>
                                <span class="text-[10px] font-bold uppercase tracking-widest text-v-gray block mb-1">@headerPart</span>
                                <h4 class="text-sm font-bold text-v-black m-0 leading-tight">@bodyPart</h4>
                            </div>
                        }
                    </div>
                </div>

                <!-- Equipment & Gear -->
                @if (Model.Tour.ProvidedEquipment.Any() || Model.Tour.RequiredGear.Any())
                {
                    <div class="grid grid-cols-1 md:grid-cols-2 gap-12 mt-12 pt-12 border-t border-v-black/10">
                        <div>
                            <span class="text-[10px] font-bold uppercase tracking-widest text-v-black font-mono-jb block mb-6">LAB INCLUSIONS</span>
                            <ul class="list-none p-0 m-0 space-y-4 font-mono-jb text-xs text-v-gray">
                                @foreach (var item in Model.Tour.ProvidedEquipment)
                                {
                                    <li class="flex items-start gap-3">
                                        <span class="text-v-black shrink-0">+</span>
                                        <span class="leading-relaxed">@item</span>
                                    </li>
                                }
                            </ul>
                        </div>
                        <div>
                            <span class="text-[10px] font-bold uppercase tracking-widest text-v-black font-mono-jb block mb-6">MANDATORY PROTOCOL</span>
                            <ul class="list-none p-0 m-0 space-y-4 font-mono-jb text-xs text-v-gray">
                                @foreach (var item in Model.Tour.RequiredGear)
                                {
                                    <li class="flex items-start gap-3">
                                        <span class="text-v-black shrink-0">+</span>
                                        <span class="leading-relaxed">@item</span>
                                    </li>
                                }
                            </ul>
                        </div>
                    </div>
                }
            </div>
        </main>
    </div>
}
else
{
    <div class="flex-grow max-w-[1600px] mx-auto w-full px-6 md:px-12 relative pb-32 font-sans text-v-black bg-v-white">
        <div class="text-left py-32 border-b border-v-black/10 font-sans mt-24">
            <h1 class="text-6xl md:text-8xl font-bold mb-8 tracking-tighter uppercase text-v-black">DOSSIER NOT FOUND</h1>
            <p class="text-lg text-v-gray mb-16">The requested protocol does not exist in the current archive.</p>
            <a asp-page="/Experiences" class="group flex items-center gap-4 cursor-pointer border-none bg-transparent p-0 m-0 text-decoration-none">
                <span class="w-12 h-12 bg-v-black rounded-full flex items-center justify-center text-v-white group-hover:bg-v-red transition-colors duration-300">
                    <i class="bi bi-arrow-left text-xl"></i>
                </span>
                <span class="text-xl md:text-2xl font-bold tracking-tight text-v-black group-hover:text-v-red transition-colors duration-300">
                    RETURN TO ARCHIVE
                </span>
            </a>
        </div>
    </div>
}
"""

with open('Pages/Tour.cshtml', 'w', encoding='utf-8') as f:
    f.write(new_content)

print("Tour page updated.")
