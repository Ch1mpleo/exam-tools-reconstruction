// Capture the HTML's deterministic export view; encode variable-duration GIFs.
// npm ci, then npm run docs:gifs. CHROME_PATH may select a local browser.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {spawnSync} = require('node:child_process');
const {pathToFileURL} = require('node:url');
const {chromium} = require('playwright-core');
const ffmpeg = require('ffmpeg-static');
const root = path.resolve(__dirname, '..');
const html = path.join(root, 'docs/flow.html');
const work = path.join(root, 'evidence/readme-animation');
const output = path.join(root, 'docs/media');
const only = process.argv.find(arg => arg.startsWith('--only='))?.slice(7);
const preview = process.argv.includes('--preview');
const digest = file => crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
// 25 fps while moving, followed by a short hold: 1.6 seconds per step.
const motionFrames = 16, frameSeconds = .04, holdSeconds = .96;

// Verify the encoded animation, rather than assuming the requested delays/loop.
function gifInfo(file) {
  const data=fs.readFileSync(file);
  if (!['GIF89a','GIF87a'].includes(data.toString('ascii',0,6))) throw Error('Not a GIF: '+file);
  const info={width:data.readUInt16LE(6),height:data.readUInt16LE(8),frames:0,duration_seconds:0,loop_count:null};
  let cursor=13+((data[10]&128)?3*(1<<((data[10]&7)+1)):0), delay=0;
  function blocks() {
    const parts=[];
    while (data[cursor]) {const size=data[cursor++];parts.push(data.subarray(cursor,cursor+size));cursor+=size;}
    cursor++;return parts;
  }
  while (cursor<data.length) {
    const tag=data[cursor++];
    if (tag===0x3b) break;
    if (tag===0x21) {
      const label=data[cursor++], parts=blocks();
      if (label===0xf9) delay=parts[0].readUInt16LE(1);
      if (label===0xff && parts[0].toString('ascii')==='NETSCAPE2.0') info.loop_count=parts[1].readUInt16LE(1);
    } else if (tag===0x2c) {
      const packed=data[cursor+8];cursor+=9;
      if (packed&128) cursor+=3*(1<<((packed&7)+1));
      cursor++;blocks();info.frames++;info.duration_seconds+=delay/100;delay=0;
    } else throw Error(`Unexpected GIF block ${tag} at ${cursor-1}`);
  }
  info.duration_seconds=Number(info.duration_seconds.toFixed(2));
  return info;
}

function encode(args) {
  const result = spawnSync(ffmpeg, ['-hide_banner','-loglevel','error','-y',...args], {encoding:'utf8'});
  if (result.error || result.status !== 0) throw Error(result.error?.message || result.stderr);
}
function browserPath() {
  const candidates = [process.env.CHROME_PATH,chromium.executablePath()];
  if (process.platform === 'win32') {
    for (const prefix of [process.env.LOCALAPPDATA,process.env.PROGRAMFILES,process.env['PROGRAMFILES(X86)']].filter(Boolean)) {
      candidates.push(path.join(prefix,'Google/Chrome/Application/chrome.exe'));
      candidates.push(path.join(prefix,'Microsoft/Edge/Application/msedge.exe'));
    }
  } else candidates.push('/usr/bin/google-chrome','/usr/bin/chromium','/usr/bin/chromium-browser','/Applications/Google Chrome.app/Contents/MacOS/Google Chrome');
  const found = candidates.filter(Boolean).find(file => fs.existsSync(file));
  if (!found) throw Error('Install Chromium with npx playwright-core install chromium, or set CHROME_PATH.');
  return found;
}

(async () => {
  fs.mkdirSync(output,{recursive:true});
  fs.mkdirSync(work,{recursive:true});
  const browser = await chromium.launch({executablePath:browserPath(),headless:true});
  const results = [], errors = [];
  try {
    const page = await browser.newPage({viewport:{width:1000,height:740},deviceScaleFactor:1,colorScheme:'light',reducedMotion:'reduce'});
    page.on('pageerror',e => errors.push(e.message));
    await page.goto(pathToFileURL(html).href+'?export=1');
    await page.waitForFunction(() => !!window.flowExport);
    const cases = await page.evaluate(() => window.flowExport.describe());
    if (only && !cases.some(item => item.id === only)) throw Error('Unknown case: '+only);
    for (const item of cases.filter(item => !only || item.id === only)) {
      const folder = path.join(work,item.id);
      fs.mkdirSync(folder,{recursive:true});
      const frames = [];
      let serial=0;
      for (let step=0;step<item.steps.length;step++) {
        const moving = item.steps[step].moving;
        const poses = moving && !preview ? Array.from({length:motionFrames+1},(_,i)=>i/motionFrames) : [1];
        for (const progress of poses) {
          await page.evaluate(({id,step,progress}) => window.flowExport.show(id,step,progress),{id:item.id,step,progress});
          const layout = await page.evaluate(() => {
            const card=document.getElementById('readme-card'), rect=card.getBoundingClientRect();
            const outside=[...card.querySelectorAll('h1,h2,p,li,.export-footer')].filter(el => {
              const r=el.getBoundingClientRect();return r.left<rect.left || r.right>rect.right || r.top<rect.top || r.bottom>rect.bottom;
            }).map(el => el.id || el.tagName);
            return {width:rect.width,height:rect.height,outside};
          });
          if (layout.width!==1000 || layout.height!==740 || layout.outside.length) throw Error(`Export overflow: ${item.id}/${step}: ${JSON.stringify(layout)}`);
          const file=`frame-${String(serial++).padStart(3,'0')}.png`;
          await page.screenshot({path:path.join(folder,file),animations:'disabled'});
          const duration=progress===1?(moving?holdSeconds:motionFrames*frameSeconds+holdSeconds):frameSeconds;
          frames.push({file,duration,step,progress});
        }
        if (preview) break;
      }
      if (preview) {
        console.log(`${item.id}: ${path.join(folder,frames[0].file)}`);
        continue;
      }
      const concat=path.join(folder,'frames.ffconcat');
      fs.writeFileSync(concat,'ffconcat version 1.0\n'+frames.map(f=>`file '${f.file}'\nduration ${f.duration.toFixed(2)}\n`).join('')+`file '${frames.at(-1).file}'\n`);
      const palette=path.join(folder,'palette.png');
      encode(['-f','concat','-safe','0','-i',concat,'-vf','palettegen=max_colors=128:stats_mode=diff','-frames:v','1','-update','1',palette]);
      const name=item.id==='overview'?'architecture':item.id, destination=path.join(output,name+'.gif');
      encode(['-f','concat','-safe','0','-i',concat,'-i',palette,'-lavfi','[0:v][1:v]paletteuse=dither=bayer:bayer_scale=4:diff_mode=rectangle','-fps_mode','vfr','-loop','0','-final_delay','4','-gifflags','+offsetting+transdiff',destination]);
      const bytes=fs.statSync(destination).size;
      if (bytes>2*1024*1024) throw Error(`GIF exceeds the 2 MiB export budget: ${destination}`);
      const encoded=gifInfo(destination);
      if (encoded.width!==1000 || encoded.height!==740 || encoded.frames<=item.steps.length || encoded.loop_count!==0) throw Error('Invalid encoded animation: '+JSON.stringify(encoded));
      results.push({id:item.id,path:path.relative(root,destination).replaceAll('\\','/'),steps:item.steps.length,poses:frames.length,...encoded,loop:'infinite',bytes,sha256:digest(destination)});
      console.log(`${name}.gif: ${item.steps.length} steps, ${(bytes/1024).toFixed(0)} KiB`);
    }
    if (errors.length) throw Error(errors.join('\n'));
    if (!preview) fs.writeFileSync(path.join(work,'receipt.json'),JSON.stringify({html_sha256:digest(html),results,console_errors:errors},null,2)+'\n');
  } finally {await browser.close();}
})().catch(error => {console.error(error);process.exitCode=1;});
