"""Summarize bounded validation; fail on missing observations or regressions."""
from pathlib import Path
from collections import Counter
import hashlib,json
ROOT=Path(__file__).resolve().parents[1]
read=lambda name:json.loads((ROOT/name).read_text(encoding='utf-8-sig'))
static=read('evidence/reconstruction-static-audit.json')
assert all(x['matches_manifest'] for x in static['inputs'])
assert len(static['inputs'])==42 and len(static['assemblies'])==13
assert all(x['ordered_call_tokens_match'] and x['exception_clause_count_match'] for x in static['eos_normalization'])
contracts=[t for a in static['assemblies'] for t in a['contracts']]
assert all(t['fields_match'] and t['method_signatures_match'] and t['serializable_match'] for t in contracts)
assert not any(x['matches'] for x in static['runtime_type_search'])
counts={};diffs=[]
for pack,expected in [('EOS',45),('PEA-CSD',29),('PEA-PRN',31)]:
    old=read(f'evidence/probe-{pack}-original.json');new=read(f'evidence/probe-{pack}-rebuilt.json')
    assert len(old)==expected and set(old)==set(new)
    diffs.extend({'pack':pack,'case':k} for k in old if old[k]!=new[k])
    counts[pack]=len(old)
    assert old['gzip.roundtrip.131072']['equal'] is True
    if pack!='EOS':
        assert old['practical.validate']==[False,False,True]
        assert old['zip.valid-and-extract']['extracted'] is True
    else:
        assert old['identity.null']==0 and old['identity.virtual-dispatch']['override_calls']==0
        assert old['question.local-cache']['recovered_login']=='Student'
assert not diffs,diffs
trace=read('evidence/login-trace-audit.json');assert len(trace['cases'])==54 and trace['failures']==0
assert all(x['pass'] for x in trace['cases'])
catalog=read('evidence/method-catalog.json');coverage=Counter(m['review_status'] for m in catalog)
assert len(catalog)==1426 and coverage['needs-review']==0
methods=[m for a in static['assemblies'] for m in a['methods']]
hashes={}
for p in sorted((ROOT/'src').rglob('*')):
    if p.is_file() and p.suffix in {'.cs','.csproj','.resx','.xml','.config'} and not {'bin','obj','EOS.Analysis'}.intersection(p.parts):hashes[p.relative_to(ROOT).as_posix()]=hashlib.sha256(p.read_bytes()).hexdigest()
hashes['Directory.Build.props']=hashlib.sha256((ROOT/'Directory.Build.props').read_bytes()).hexdigest()
report={'status':'bounded-local-validation-passed','scope':'Available EOS, PEA CSD and PEA PRN artifacts and selected bundled first-party libraries only',
 'additional_inputs_expected':False,'external_implementations':'Documented analysis boundaries, not required deliverables',
 'inputs_verified':42,'assemblies':13,'methods_indexed':1426,
 'canonical_il_matches':sum(x['exact_canonical_il'] for x in methods),
 'contract_types_matching':len(contracts),'eos_normalized_methods_call_and_exception_counts_preserved':444,
 'helper_cases_per_pack':counts,'helper_cases_total':sum(counts.values()),'helper_differences':diffs,
 'mocked_login_cases':len(trace['cases']),'mocked_login_failures':0,'description_coverage':dict(coverage),
 'source_sha256':hashes,'missing_runtime_types':['ExamClient.frmEnglishExamClient','ExamClient.PreExamEntry','PEAClient.FrmPEAClient'],
 'limitations':['The 54 login cases interpret CIL against mocks; they do not run a real server or GUI.',
 'The 105 helper comparisons load only selected DLL helpers into separate processes with synthetic data.',
 'Database methods, lock timers, process control, monitoring and camera implementations were not executed.',
 'No full equivalence proof; default compatibility target is Framework 4.8; injected EOS protector startup is excluded.',
 'Unseen downloaded GUI/server implementation is outside the available evidence; no behavior is invented for it.']}
(ROOT/'evidence/reconstruction-audit-summary.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
(ROOT/'analysis').mkdir(exist_ok=True)
(ROOT/'analysis/validation.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
identities=[{k:a[k] for k in ('build','name','original_sha256','original_mvid','original_runtime','original_methods')} for a in static['assemblies']]
(ROOT/'analysis/assemblies.json').write_text(json.dumps(identities,indent=2)+'\n',encoding='utf-8',newline='\n')
(ROOT/'analysis/eos-aliases.json').write_text(json.dumps(read('evidence/EOS-normalized/semantic-aliases.json'),indent=2)+'\n',encoding='utf-8',newline='\n')
print(json.dumps({k:v for k,v in report.items() if k not in {'source_sha256','limitations'}}))
