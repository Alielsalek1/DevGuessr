const { chromium } = require('playwright');
(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ ignoreHTTPSErrors: true });
  const logs = [];
  const errors = [];
  page.on('console', msg => logs.push(`[${msg.type()}] ${msg.text()}`));
  page.on('pageerror', err => errors.push(err.message));
  await page.goto('https://localhost/', { waitUntil: 'domcontentloaded', timeout: 30000 });
  await page.waitForTimeout(2500);
  const result = await page.evaluate(() => {
    const root = document.querySelector('app-root');
    const shell = document.querySelector('app-shell');
    const header = document.querySelector('header');
    const homeTitle = document.querySelector('h1');
    return {
      title: document.title,
      rootExists: !!root,
      rootChildren: root ? root.children.length : -1,
      shellExists: !!shell,
      headerExists: !!header,
      homeTitle: homeTitle ? homeTitle.textContent?.trim() : null,
      bodyTextLen: (document.body?.innerText || '').trim().length,
      firstBodyText: (document.body?.innerText || '').trim().slice(0, 200)
    };
  });
  console.log('RESULT', JSON.stringify(result));
  console.log('LOG_COUNT', logs.length);
  console.log('ERROR_COUNT', errors.length);
  logs.forEach(l => console.log('LOG', l));
  errors.forEach(e => console.log('ERR', e));
  await browser.close();
})();
