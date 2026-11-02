// Compact, deterministic views of the same graph used by the interactive page.
// No separate architecture or inferred runtime implementation is introduced.
(() => {
  const overview = [
    {title:'Read the local configuration',caption:'EncryptData decrypts the local .dat file. ServerInfo supplies the domain and server endpoint before registration.',active:['config','shell'],state:{config:'Encrypted .dat',shell:'Endpoint configured'},message:{edge:'config-shell',label:'ServerInfo'}},
    {title:'Use the supplied helper libraries',caption:'EncryptData handles serialization and crypto. EOS uses QuestionLib and NAudio; PEA uses GradeLib and Ionic.Zip. These are local helpers, not the remote exam engine.',active:['libs','shell'],state:{libs:'Local helpers',shell:'Supplied EXE'},message:{edge:'libs-shell',label:'Local helpers'}},
    {title:'Register through the remote contract',caption:'The login shell calls ConductExam for EOS or ConductTest for PEA through a TCP .NET Remoting proxy. IRemote.dll declares the interface; its server implementation is outside the supplied files.',active:['shell','server'],state:{shell:'Registration sent',server:'IRemoteServer'},message:{edge:'shell-server',label:'ConductExam / Test'}},
    {title:'Receive a session and GUI bytes',caption:'A successful registration returns EOSData or PEAData. The GUI field contains a compressed executable .NET assembly, not just an exam document.',active:['server','shell'],state:{server:'Status + payload',shell:'Session received'},message:{edge:'server-shell',label:'GUI bytes'}},
    {title:'Load the downloaded exam form',caption:'The shell GZip-decompresses GUI, calls Assembly.Load and passes the session through SetExamData. The actual exam-form implementation was not included in the supplied packs.',active:['shell','runtime'],state:{shell:'Session handed over',runtime:'Code from server'},message:{edge:'shell-runtime',label:'Assembly.Load'}},
    {title:'Stop at the submission boundary',caption:'The contracts declare SubmitPaper for EOS and SubmitSolution for PEA, returning SubmitStatus. The downloaded submit caller and the server acceptance logic are outside the supplied code.',active:['runtime','server'],state:{runtime:'Caller not supplied',server:'Submit contract only'},message:{edge:'runtime-server',label:'Contract only'}}
  ];
  const labels = {
    overview:['Configuration','Local helpers','Register','Receive GUI','Load form','Submit contract'],
    eos:['Start','Configure','Connect','Register','Receive','Load GUI','Check-in','Handoff'],
    'pea-csd':['Start','Configure','Validate','Register','Receive','Load GUI','Attach data','Handoff'],
    'pea-prn':['Start','Configure','Validate','Register','Receive','Load GUI','Attach data','Handoff']
  };
  const baseState = {config:'Encrypted .dat',libs:'Supplied locally',shell:'Supplied EXE',server:'Code not supplied',runtime:'Code not supplied'};
  const cases = [{id:'overview',name:'Shared architecture',steps:overview}, ...D.cases];
  const style = document.createElement('style');
  style.textContent = `
    #readme-card{display:none}
    body.readme-export{width:1000px;height:740px;margin:0;overflow:hidden;background:var(--bg)}
    .readme-export>.wrap{display:none}
    .readme-export #readme-card{display:grid;width:1000px;height:740px;padding:24px 28px;grid-template-rows:64px 1fr 36px;gap:18px}
    .export-heading{display:flex;justify-content:space-between;align-items:center;border-bottom:1px solid var(--line);padding-bottom:14px}
    .export-eyebrow{font-size:11px;letter-spacing:1.8px;text-transform:uppercase;color:var(--muted);margin:0 0 4px}
    .export-heading h1{font-size:27px;letter-spacing:-.6px;line-height:1.15;margin:0}
    .export-badge{font-size:12px;color:var(--accent);border:1px solid var(--line);border-radius:5px;padding:7px 11px}
    .export-body{display:grid;grid-template-columns:344px minmax(0,1fr);gap:32px;align-items:center;min-height:0}
    #export-diagram .diagram{padding:0;border:0;background:transparent}
    #export-diagram .legend{display:none}
    #export-diagram .node text{font-size:16px}
    #export-diagram .node .state{font-size:13px}
    #export-diagram .node.active rect{fill:color-mix(in srgb,var(--accent) 8%,var(--surface))}
    #export-diagram path{stroke:var(--muted);stroke-width:1.3;opacity:.38}
    #export-diagram path.active{stroke:var(--accent);stroke-width:2.3;opacity:1}
    #export-diagram .node path{opacity:.5}
    #export-diagram .node rect{stroke:var(--muted);stroke-width:1;fill:var(--surface)}
    #export-diagram .node.active rect{stroke:var(--accent);stroke-width:2}
    #export-diagram .flow-marker text{display:none}
    .export-copy{align-self:stretch;display:flex;flex-direction:column;justify-content:center;min-width:0}
    .export-count{display:flex;align-items:center;justify-content:space-between;gap:8px;font-size:12px;color:var(--accent);letter-spacing:1.1px;font-variant-numeric:tabular-nums;margin:0 0 13px}
    #export-action{border:1px solid var(--line);border-radius:4px;padding:4px 8px;font-size:11px;letter-spacing:0;white-space:nowrap}
    #export-title{font-size:27px;line-height:1.2;letter-spacing:-.5px;margin:0 0 13px;max-width:29ch}
    #export-caption{font-size:17px;line-height:1.6;margin:0;color:var(--muted);min-height:138px}
    .export-rail{display:grid;grid-template-columns:repeat(4,minmax(0,1fr));gap:13px 9px;margin:22px 0 20px;padding:0;list-style:none}
    .export-rail li{font-size:11px;color:var(--muted);line-height:1.35}
    .export-rail span{display:block;height:3px;background:var(--line);margin-bottom:7px}
    .export-rail .done span,.export-rail .current span{background:var(--accent)}
    .export-rail .current{font-weight:600;color:var(--accent)}
    .export-context{border-left:2px solid var(--accent);padding-left:13px}
    .export-context strong{font-size:12px;font-weight:600}
    #export-context{font-size:12px;line-height:1.6;margin:5px 0 0;color:var(--muted)}
    .export-footer{display:flex;justify-content:space-between;align-items:center;border-top:1px solid var(--line);font-size:11px;color:var(--muted);gap:18px}
    .export-key{display:inline-flex;align-items:center;gap:7px}
    .export-key i{display:inline-block;width:18px;border-top:2px solid var(--accent)}
    .export-key i.dashed{border-top-style:dashed}
  `;
  document.head.append(style);
  const card = document.createElement('main');
  card.id = 'readme-card';
  card.innerHTML = `<header class="export-heading"><div><p class="export-eyebrow">Architecture / recovered binaries</p><h1 id="export-name"></h1></div><span class="export-badge">EOS + PEA</span></header>
    <section class="export-body"><div id="export-diagram"></div><div class="export-copy"><p class="export-count"><span id="export-count"></span><span id="export-action"></span></p><h2 id="export-title"></h2><p id="export-caption"></p><ol class="export-rail" id="export-rail"></ol><div class="export-context"><strong>Local package / runtime boundary</strong><p id="export-context"></p></div></div></section>
    <div class="export-footer"><span class="export-key"><i></i>Supplied callsite / local operation</span><span class="export-key"><i class="dashed"></i>Implementation outside supplied files</span><span>Animated guide · not a runtime recording</span></div>`;
  document.body.append(card);

  function show(id, stepIndex=0, progress=1) {
    const item = cases.find(c => c.id === id);
    if (!item || !Number.isInteger(stepIndex) || stepIndex < 0 || stepIndex >= item.steps.length) throw Error('Invalid export step');
    stop();
    document.body.classList.add('readme-export');
    if (!$('export-diagram').firstChild) $('export-diagram').append(document.querySelector('.diagram'));
    const s = item.steps[stepIndex], state = {...baseState};
    for (let i=0;i<=stepIndex;i++) Object.assign(state,item.steps[i].state);
    nodeState(s.active,state);
    animateMessage(s.message,false);
    if (s.message) {
      const p=$('edge-'+s.message.edge), t=Math.max(0,Math.min(1,progress)), eased=t*t*(3-2*t);
      const pt=p.getPointAtLength(p.getTotalLength()*(s.message.edge==='server-shell'?.65:.7)*eased);
      $('message').setAttribute('transform',`translate(${pt.x} ${pt.y})`);
      const rect=$('message').querySelector('rect');
      for (const [name,value] of Object.entries({x:-6,y:-6,width:12,height:12,rx:6})) rect.setAttribute(name,value);
    }
    $('export-name').textContent=id==='overview'?'EOS & PEA: shared architecture':`${item.name}: session walkthrough`;
    $('export-count').textContent=`${String(stepIndex+1).padStart(2,'0')} / ${String(item.steps.length).padStart(2,'0')}  ·  ${id==='overview'?'ARCHITECTURE':'SESSION FLOW'}`;
    $('export-action').textContent=s.message?.label || 'Local step';
    $('export-title').textContent=s.title;
    $('export-caption').textContent=s.caption;
    $('export-rail').innerHTML=labels[id].map((label,i)=>`<li class="${i<stepIndex?'done':i===stepIndex?'current':''}"><span></span>${String(i+1).padStart(2,'0')} ${esc(label)}</li>`).join('');
    $('export-context').textContent=id==='overview'?'IRemote declares the messages. Server decisions and downloaded form bodies are outside the recovered source.':item.payload;
    return {id,step:stepIndex,title:s.title,edge:s.message?.edge??null};
  }
  window.flowExport = {
    describe:()=>cases.map(c=>({id:c.id,steps:c.steps.map(s=>({title:s.title,caption:s.caption,moving:!!s.message}))})),
    show
  };
  const query=new URLSearchParams(location.search);
  if (query.has('export')) {
    document.documentElement.dataset.theme='light';
    show(cases.some(c=>c.id===location.hash.slice(1))?location.hash.slice(1):'overview');
  }
})();
