import base64
import os
import asyncio
from playwright.async_api import async_playwright

async def main():
    # 1. Read logo and base64 encode
    with open(r"C:\Users\alext\Desktop\PBlogos\logo.png", "rb") as f:
        logo_data = base64.b64encode(f.read()).decode("utf-8")
    
    # 2. Update html file
    with open("render_qr.html", "r", encoding="utf-8") as f:
        html = f.read()
    
    html = html.replace("LOGO_BASE64_PLACEHOLDER", f"data:image/png;base64,{logo_data}")
    
    with open("render_qr_ready.html", "w", encoding="utf-8") as f:
        f.write(html)
        
    # 3. Use playwright to render and screenshot
    async with async_playwright() as p:
        browser = await p.chromium.launch()
        page = await browser.new_page(viewport={"width": 1000, "height": 1000})
        
        url = "file:///" + os.path.abspath("render_qr_ready.html").replace("\\", "/")
        await page.goto(url)
        
        # Wait for the JS to finish
        await page.wait_for_function("window.isDone === true")
        
        # Take a screenshot of the canvas element
        element = await page.query_selector("canvas")
        if not element:
            # Fallback if svg was used
            element = await page.query_selector("svg")
            
        artifact_path = r"C:\Users\alext\.gemini\antigravity-ide\brain\e046bdf1-7954-4f16-a92e-b5d259f7b56e\purplebean_FINAL_qr.png"
        await element.screenshot(path=artifact_path)
        
        await browser.close()
        print(f"Saved exact styling QR code to {artifact_path}")

asyncio.run(main())
