"""Prepare isolated .NET Framework 4.8 compatibility projects from recovered C#."""
from pathlib import Path
import json, re, shutil, xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[1]
manifest=json.loads((ROOT/'exam-input-manifest.json').read_text(encoding='utf-8-sig'))
names={'EOSClient.exe','PEALogin.exe','IRemote.dll','EncryptData.dll','QuestionLib.dll','GradeLib.dll','HashCodeProvider.dll'}
projects={}
for entry in manifest:
    name=Path(entry['Path']).name
    if name not in names:continue
    stem=Path(name).stem;build=entry['Build'];dest=ROOT/'src'/build/stem
    origin=ROOT/'src/EOS.Analysis' if name=='EOSClient.exe' else ROOT/'decompiled'/build/stem
    shutil.copytree(origin,dest,dirs_exist_ok=True)
    projects[(build,stem)]=(dest,entry)

type_map={'Type_000d':'AuthenticateForm','Type_000e':'ExamCloseHandlers','Type_0010':'ClientTheme',
          'Type_0011':'FieldFocusHandlers','Type_0012':'RulesForm','Type_0013':'FontCheckForm',
          'Type_0014':'Program','Type_0015':'Resources'}
method_map={0x3c:'Dispose',0x3d:'InitializeComponent',0x3e:'PaintBrandGlyph',0x3f:'PaintPanelBorder',
0x40:'FieldEnter',0x41:'FieldLeave',0x42:'LoginMouseEnter',0x43:'LoginMouseLeave',
0x44:'ExitMouseEnter',0x45:'ExitMouseLeave',0x46:'LoginClick',0x47:'ConnectAndPing',
0x48:'HasVersionMismatch',0x49:'ExitClick',0x4a:'LoadServerConfiguration',0x4b:'AddEndpoint',
0x4c:'IsAdministrator',0x4d:'ShowFontCheck',0x4e:'CheckSound',0x4f:'PlayMp3',0x50:'Closing',
0xe7:'StopSessionLocks',0xf3:'LetterSpaceUppercase',0xf4:'DrawBrandGlyph',0xf5:'WrapInputField',
0xf6:'WrapContent',0xf7:'DrawPanelBorder',0x11f:'NextClick',0x120:'RulesConsentChanged',
0x121:'PaintBrandGlyph',0x122:'NextMouseEnter',0x123:'NextMouseLeave',0x124:'IsAdministrator',
0x125:'ValidateStartupEnvironment',0x126:'EmptyHandler',0x127:'Dispose',0x128:'InitializeComponent',
0x16c:'FontResolvesExactly',0x16d:'CheckInstalledFonts',0x16e:'CloseClick',0x16f:'Dispose',
0x170:'InitializeComponent',0x19b:'Main',0x19c:'HandleUiException',0x19d:'HandleUnhandledException',0x19e:'AppendErrorLog'}
field_map={'String_F002e':'ClientVersion','String_F0032':'domain','ServerInfo_F004d':'serverInfo',
           'ZEJ_RC_Q_c___Y___T__Qpw8_F004e':'fontCheckForm','Container_F0038':'components',
           'IContainer_F0061':'components','IContainer_F006a':'components'}
shell=ROOT/'src/EOS/EOSClient'
for file in sorted((ROOT/'src/EOS.Analysis').glob('*.cs')):
    if file.name=='-Module-.cs':continue
    source=file.read_text(encoding='utf-8-sig')
    for old,new in re.findall(r'Control_set_Name_M[0-9a-f]+\(\(Control\)(\w+), "(\w+)"\)',source):
        if old!='this':field_map.setdefault(old,new)
aliases=[]
def rewrite(source):
    def rename(m):
        token=int(m[1],16)
        return method_map.get(token,m[0].split('_M')[0])+f'_M{token:04x}' if token in method_map else m[0]
    source=re.sub(r'\b[A-Za-z_]\w*_M([0-9a-f]{4})\b',rename,source)
    for old,new in {**type_map,**field_map}.items():source=re.sub(r'\b'+re.escape(old)+r'\b',new,source)
    for token in (0x3c,0x127,0x16f):
        source=source.replace(f'protected virtual void Dispose_M{token:04x}', 'protected override void Dispose')
    source=source.replace('Main_M019b','Main')
    source=source.replace('CultureInfo_F006f = cultureInfo_F006f;', 'CultureInfo_F006f = value;')
    for method in ('ExamNetworkLock_Stop_M00e8','ExamAppLock_Stop_M00e9','Control_set_BackColor_M011d'):
        source=re.sub(r'(?m)^(\s*)static (void '+method+r'\()',r'\1internal static \2',source)
    return source
for file in sorted((ROOT/'src/EOS.Analysis').glob('*.cs')):
    if file.name=='-Module-.cs':continue
    source=rewrite(file.read_text(encoding='utf-8-sig'))
    source='// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.\n'+source
    dest=shell/(type_map.get(file.stem,file.stem)+'.cs')
    dest.write_text(source,encoding='utf-8',newline='\n')
    copied_raw=shell/file.name
    if dest!=copied_raw:copied_raw.unlink(missing_ok=True)
resx=ROOT/'decompiled/EOS/EmbeddedDependency/--Zp.resx'
shutil.copy2(resx,shell/'RulesForm.resx')
identity_file=ROOT/'src/EOS/HashCodeProvider/HashCodeProvider/IdentityHashCodeProvider.cs'
identity_source=identity_file.read_text(encoding='utf-8-sig')
identity_source=identity_source.replace('using System.Collections;', 'using System.Collections;\nusing System.Runtime.CompilerServices;')
identity_source=identity_source.replace('return obj.GetHashCode();', '// Original IL calls Object.GetHashCode nonvirtually.\n\t\treturn RuntimeHelpers.GetHashCode(obj);')
identity_file.write_text(identity_source,encoding='utf-8',newline='\n')
for (build,stem),(dest,entry) in projects.items():
    original_dir=ROOT/'src/EOS.Analysis' if stem=='EOSClient' else ROOT/'decompiled'/build/stem
    oldproj=next(original_dir.glob('*.csproj'))
    old=ET.parse(oldproj).getroot()
    references=['<Reference Include="System.Windows.Forms" />','<Reference Include="System.Drawing" />','<Reference Include="System.Data" />','<Reference Include="System.Configuration" />']
    for ref in old.findall('.//Reference'):
        name=ref.get('Include');simple=name.split(',')[0]
        if simple.startswith('System.') or simple=='System':continue
        if (build,simple) in projects:
            references.append(f'<ProjectReference Include="../{simple}/{simple}.csproj" />')
        elif simple=='oEkaOfreSFdoigvXuhZRhePtFbDIA':continue
        else:
            path=f'$(ExamInputsRoot)/{build}/{simple}.dll'
            references.append(f'<Reference Include="{simple}"><HintPath>{path}</HintPath></Reference>')
    resources=[]
    if stem=='EOSClient':
        resources=['<Compile Remove="-Module-.cs" />','<EmbeddedResource Update="RulesForm.resx"><LogicalName>RulesForm.resources</LogicalName></EmbeddedResource>']
    else:
        for resource in old.findall('.//EmbeddedResource'):
            resources.append(ET.tostring(resource,encoding='unicode').strip())
    assembly='EOSClient.Reconstructed' if stem=='EOSClient' else stem
    properties=f'<TargetFramework>net48</TargetFramework><LangVersion>latest</LangVersion><GenerateAssemblyInfo>false</GenerateAssemblyInfo><AllowUnsafeBlocks>true</AllowUnsafeBlocks><CheckForOverflowUnderflow>false</CheckForOverflowUnderflow><AssemblyName>{assembly}</AssemblyName><RootNamespace></RootNamespace>'
    if stem in ('EOSClient','PEALogin'):properties+='<OutputType>WinExe</OutputType><UseWindowsForms>true</UseWindowsForms>'
    if stem=='EOSClient':properties+='<StartupObject>Program</StartupObject>'
    csproj=f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>{properties}</PropertyGroup><ItemGroup><PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies.net48" Version="1.0.3" PrivateAssets="all" />'+''.join(references+resources)+'</ItemGroup></Project>'
    target=dest/(stem+'.csproj')
    project_xml=ET.fromstring(csproj)
    ET.indent(project_xml,space='  ')
    target.write_text(ET.tostring(project_xml,encoding='unicode')+'\n',encoding='utf-8',newline='\n')
    for extra in dest.glob('*.csproj'):
        if extra!=target:extra.unlink()
    aliases.append({'build':build,'assembly':stem,'project':str(target.relative_to(ROOT)),'target_framework':'net48','original_sha256':entry['SHA256']})
(ROOT/'evidence/source-projects.json').write_text(json.dumps(aliases,indent=2),encoding='utf-8',newline='\n')
(ROOT/'evidence/EOS-normalized/semantic-aliases.json').write_text(json.dumps({'types':type_map,'methods':{f'0x0600{k:04x}':v for k,v in method_map.items()},'fields':field_map},indent=2),encoding='utf-8',newline='\n')
# Apply constants and aliases to the method-by-method EOS reading copies too.
exec_namespace={'__file__':str(ROOT/'scripts/decode_eos_constants.py')}
exec((ROOT/'scripts/decode_eos_constants.py').read_text(),exec_namespace)
for file in (ROOT/'evidence/EOS-normalized/methods').glob('*.cs'):
    source=exec_namespace['pattern'].sub(exec_namespace['decode'],file.read_text(encoding='utf-8-sig'))
    file.write_text(rewrite(source),encoding='utf-8',newline='\n')
(ROOT/'evidence/EOS-normalized/decoded-strings.json').write_text(json.dumps(list(exec_namespace['records'].values()),indent=2),encoding='utf-8',newline='\n')
# Keep provenance digests stable across Git checkouts and Windows generation.
for folder,entry in projects.values():
    for file in folder.rglob('*'):
        if file.suffix in {'.cs','.csproj','.resx','.xml','.config'} and not {'bin','obj'}.intersection(file.parts):
            file.write_text(file.read_text(encoding='utf-8-sig'),encoding='utf-8',newline='\n')
print(f'Prepared {len(projects)} compatibility projects.')
