import asyncio
from playwright.async_api import async_playwright

async def run():
    async with async_playwright() as p:
        browser = await p.chromium.launch()
        page = await browser.new_page()
        await page.goto("http://localhost:5173")

        # Check title
        title = await page.inner_text("h1")
        print(f"Title found: {title}")

        # Check for upload input
        upload_input = await page.query_selector("input[type='file']")
        if upload_input:
            print("Upload input found.")
        else:
            print("Upload input NOT found.")

        # Capture screenshot
        await page.screenshot(path="screenshot.png")
        print("Screenshot saved to screenshot.png")

        await browser.close()

if __name__ == "__main__":
    asyncio.run(run())
