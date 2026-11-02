"""Add behavior notes to substantive recovered methods; code remains unchanged."""
from pathlib import Path
import json,re,textwrap
ROOT=Path(__file__).resolve().parents[1]
catalog=json.loads((ROOT/'evidence/method-catalog.json').read_text(encoding='utf-8'))
projects=json.loads((ROOT/'evidence/source-projects.json').read_text(encoding='utf-8'))
declaration=re.compile(r'^\s*(?:public|private|protected|internal)\s+(?:static\s+|virtual\s+|override\s+|unsafe\s+)*(?:[\w<>\[\],.?]+\s+)+(?P<name>[\w.]+)\s*\(')
count=0
for project in projects:
    folder=(ROOT/project['project']).parent
    rows=[m for m in catalog if m['build']==project['build'] and m['assembly']==project['assembly']]
    for path in folder.rglob('*.cs'):
        if {'bin','obj'}.intersection(path.parts) or path.name=='-Module-.cs':continue
        candidates=[m for m in rows if m['type'].split('/')[-1].split('.')[-1]==path.stem]
        if not candidates:continue
        lines=path.read_text(encoding='utf-8-sig').splitlines();out=[]
        for line in lines:
            if line.lstrip().startswith('// [behavior '):continue
            match=declaration.match(line)
            if match:
                name=match['name'];m=next((m for m in candidates if name==m['name'] or name.endswith('_M'+m['token'][-4:])),None)
                if m and not m['original_name'].startswith(('get_','set_')) and not m['original_name'].startswith('.'):
                    note=m['summary']
                    if not note.startswith(('Recovered implementation','Small model operation','Short forwarding method')):
                        indent=line[:len(line)-len(line.lstrip())]
                        for wrapped in textwrap.wrap(note,width=96):out.append(indent+'// [behavior '+m['token']+'] '+wrapped)
                        count+=1
            out.append(line)
        path.write_text('\n'.join(out)+'\n',encoding='utf-8',newline='\n')
print(f'Added behavior notes to {count} method declarations.')
