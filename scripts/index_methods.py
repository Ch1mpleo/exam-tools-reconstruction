from pathlib import Path
import json, re, html, hashlib
from review_annotations import enrich
ROOT=Path(__file__).resolve().parents[1]
manifest=json.loads((ROOT/'exam-input-manifest.json').read_text(encoding='utf-8-sig'))
aliases=json.loads((ROOT/'evidence/EOS-normalized/semantic-aliases.json').read_text())
summaries={
'PEALogin.AuthenticateForm.btnLogin_Click':'Validates four textboxes, calls ConductTest through a TCP .NET Remoting proxy, handles registration statuses, then decompresses/loads the returned GUI and supplies its session and authentication form. Only NEW/REASSIGN enter that success path.',
'PEALogin.AuthenticateForm.AuthenticateForm_Load':'Checks administrator membership, decrypts/deserializes PEA_Server_Info.dat, checks the client version identifier and displays the configured domain. Errors request application exit. File existence uses StartupPath; the decrypt call receives a relative filename.',
'PEALogin.AuthenticateForm.IsAdministrator':'Checks the current WindowsPrincipal for WindowsBuiltInRole.Administrator. This is an operating-system role check, separate from the remote exam-account login.',
'PEALogin.AuthenticateForm.btnCancel_Click':'Requests WinForms application exit. There is no remote cancellation or submission call.',
'PEALogin.Program.Main':'STA entry point: enable visual styles, select compatible text rendering and start the message loop with AuthenticateForm.',
'GradeLib.GZipHelper.Compress':'Writes all input through a GZipStream, closes it and returns compressed memory. Any exception returns null.',
'GradeLib.GZipHelper.DeCompress':'Allocates originSize bytes, performs one decompression read and returns the allocation. The implementation does not loop or validate the returned byte count.',
'GradeLib.SolutionZip.CreateZip':'Checks the solution directory, sums all recursive file sizes and rejects zero bytes or more than 10 MiB. Saves the directory as a ZIP and returns the ZIP file bytes.',
'GradeLib.GivenExtract.Extract':'Reads a ZIP and extracts entries to the selected directory with overwrite enabled. Returns true on completion; exceptions set the ref log message and return false.',
'GradeLib.TestCase.Validate':'Returns Input.Count > 0 && Mark > 0. This validates a test-case record, not a student program.',
'GradeLib.TestCase.File2TestCase':'Parses text section markers into a TestCase. Records input/output, whitespace and case flags, and mark; parsing stops after the mark. PRN additionally resolves data/output file references. See this exact build-specific body.',
'GradeLib.TestCase.LoadTestCase':'Build-specific test-case discovery. CSD parses the directory file list. PRN filters text files and can traverse matching tc* directories. Calls File2TestCase for selected files.',
'GradeLib.TestCase.File2TestCaseCSD':'CSD-specific directory parser. Recognizes data.txt and the numbered reference-output file, then parses metadata sections from other files. Preserves the original filename and stream-handling quirks.',
'GradeLib.TestCase.DetectTestCasesContainTCFolder':'Returns true when the directory exists, contains tc* subdirectories, and has no top-level tc*.txt files. Chooses the alternate PRN discovery layout.',
'GradeLib.TestCase.ConvertZipToByteArray':'Throws FileNotFoundException for a missing path; otherwise returns File.ReadAllBytes. It does not parse ZIP contents.',
'QuestionLib.QuestionHelper.SaveSubmitPaper':'Validates the model; locks by normalized login; serializes to .tmp; moves or atomically replaces the main .dat and retains .bak; removes a leftover temporary file. Exceptions inside the write block are swallowed.',
'QuestionLib.QuestionHelper.LoadSubmitPaper':'Rejects a missing/blank primary path. Deserializes and validates the primary model; only a thrown exception leads to a .bak attempt. Returns null if recovery fails.',
'QuestionLib.QuestionHelper.IsValidSubmitPaper':'Requires a nonnull SubmitPaper, a nonempty LoginId and a nonnull SPaper.',
'QuestionLib.QuestionHelper.GetLock':'Under a shared dictionary lock, creates or returns the per-login lock object used to serialize saves.',
'QuestionLib.QuestionHelper.GetPassage':'Searches the original Paper reading passages by PID; returns the matching passage or null.',
'QuestionLib.QuestionHelper.Re_ConstructPaper':'Walks the submitted paper structure and restores presentation/model data from the original paper, matching identifiers through specialized helpers; also restores image/essay/audio data.',
'QuestionLib.QuestionHelper.ReConstructQuestion':'Requires matching QID; copies original metadata, question text/image and mark. Matches answers by QAID and handles fill-blank normalized text separately from other answer types.',
'QuestionLib.QuestionHelper.ReConstructEssay':'For matching essay IDs, restores original question-bank/course identifiers and question text.',
'QuestionLib.QuestionHelper.ReConstructImagePaper':'Assigns the original image data to the submitted image-paper model.',
'QuestionLib.ExamAppLock.Start':'Starts a periodic application sweep if no timer exists. Stores the current process ID as an exclusion. Overload without an interval uses 3000 ms. Existence does not prove activation by this shell.',
'QuestionLib.ExamAppLock.Stop':'Under the shared lock, disposes the sweep timer and clears the reference.',
'QuestionLib.ExamAppLock.SweepSafe':'Uses an Interlocked guard to prevent concurrent sweeps, suppresses sweep exceptions, and resets the guard in finally.',
'QuestionLib.ExamAppLock.Sweep':'Enumerates processes. Under the conditions shown, kills another process with a main window while excluding this process and explorer.exe. Disposes each Process object. This method was not executed during analysis.',
'QuestionLib.ExamNetworkLock.Start':'Records the current process and optional exam-server IPv4 exclusion; creates a periodic sweep timer only if absent. The default interval is 3000 ms. Activation by a runtime GUI remains unresolved.',
'QuestionLib.ExamNetworkLock.Stop':'Disposes and clears the network sweep timer under its synchronization lock.',
'QuestionLib.ExamNetworkLock.SweepSafe':'Uses an Interlocked concurrency guard, separately attempts IPv4 and IPv6 sweeps and suppresses their exceptions, then resets the guard.',
'QuestionLib.ExamNetworkLock.SweepIpv4':'Reads owner-PID IPv4 TCP rows, applies state/process/address exclusions and may ask SetTcpEntry to delete a connection. Allocates/frees each native row buffer. This method was not executed.',
'QuestionLib.ExamNetworkLock.SweepIpv6':'Examines IPv6 rows and global-address/process conditions; may kill an owning process outside its exclusions. This method was not executed.',
'QuestionLib.ExamNetworkLock.GetIpv4Rows':'Queries GetExtendedTcpTable for required IPv4 buffer length, allocates unmanaged memory, marshals owner-PID rows and frees the buffer.',
'QuestionLib.ExamNetworkLock.GetIpv6Rows':'Queries GetExtendedTcpTable for required IPv6 buffer length, allocates unmanaged memory, marshals owner-PID rows and frees the buffer.',
'QuestionLib.ExamNetworkLock.IsPrivateOrLocalIpv4':'Classifies selected local/private IPv4 ranges from the integer representation; its exact boundary cases are shown below.',
'QuestionLib.ExamNetworkLock.IsGlobalIpv6':'Excludes unspecified/loopback, link-local, unique-local and multicast address patterns through byte tests; returns true for the remaining patterns.',
'QuestionLib.ExamNetworkLock.IpStringToUInt32':'Splits a dotted IPv4 string, parses four components, casts them to bytes and converts them using BitConverter. Returns zero on the explicit parsing failures.',
'QuestionLib.ExamNetworkLock.BuildCriticalProcessPaths':'Builds an exclusion set from named system processes under Windows system directories and the Windows Explorer path.',
'EncryptData.EncryptSupport.ObjectToByteArray':'Returns null for a null object; otherwise BinaryFormatter-serializes it into a MemoryStream and returns the bytes.',
'EncryptData.EncryptSupport.ByteArrayToObject':'Writes bytes to a MemoryStream, seeks to its start, and BinaryFormatter-deserializes an object. Input validation and exception behavior are exactly those in the source.',
'EncryptData.EncryptSupport.GetMD5':'Computes an MD5 digest of the message encoding used in this build and formats the digest as a string. Read the exact body for encoding and hexadecimal casing.'
}
eos_details={
'003d':'Creates the login UI, styles it and wires events; no remote calls occur here.',
'0046':'EOS login orchestration: validate inputs, choose endpoint/public address, connect and Ping, compare required version, ConductExam, handle status, load GUI bytes, optionally run camera check-in, instantiate exam form and attach cleanup. See README.md for the ordered branch explanation.',
'0047':'Builds tcp://IP:port/Server, obtains an IRemoteServer proxy and calls Ping. Returns the proxy only on success; exceptions return null.',
'0048':'Returns true only for a nonblank server-required version that differs from the local string under OrdinalIgnoreCase. Empty/null versions and exceptions return false. This is equality, not numeric ordering.',
'004a':'Decrypts/deserializes the local EOS server file, configures the domain and validates/adds each endpoint. Missing/invalid configuration requests exit.',
'004b':'Adds a valid address/positive-port endpoint, choosing a label and preserving optional Public_IP.',
'004c':'Checks the current Windows principal for Administrator membership.',
'004d':'Shows an existing undisposed font-check form, creating it when necessary.',
'004e':'Checks ghosts.mp3 exists and plays it; otherwise shows an audio-file error.',
'004f':'Reads the MP3 into memory, constructs an NAudio reader and WaveOut, initializes playback, sets volume to 1 and calls Play. Playback disposal is not visible here.',
'0050':'Requests application exit and terminates the process with Environment.Exit(0).',
'0049':'Requests application exit and terminates the process with Environment.Exit(0).',
'00e7':'FormClosed callback: calls ExamNetworkLock.Stop and ExamAppLock.Stop. It does not start either lock.',
'00f3':'Uppercases the string and inserts one space between successive characters.',
'00f4':'Draws the brand mark using Bezier paths, a pen and a brush, disposing drawing objects.',
'00f5':'Builds a bordered input container with a centered child and focus-color callbacks.',
'00f6':'Builds nested padded panels and docks the supplied child inside them.',
'00f7':'Draws a rectangle at the panel border using a disposable pen.',
'011f':'Shows a new login form, then hides the rules form.',
'0120':'Enables Next exactly when consent is checked, and changes button colors.',
'0124':'Checks current Windows Administrator-role membership.',
'0125':'Checks path indicators for running from a temporary/archive location; displays an error and requests exit when unsuitable. Also checks administrator membership.',
'0126':'Empty handler: no instructions with application effects.',
'0128':'Constructs the rules UI and loads the Vietnamese rules text from resources; connects consent/Next/load handlers.',
'016c':'Creates a font by name, compares its resolved name with the requested name, and disposes it.',
'016d':'Enumerates installed font families, checks named Chinese/Japanese fonts and renders a diagnostic report.',
'016e':'Closes the font-check form.',
'0170':'Constructs the font-check utility controls and event connections.',
'019b':'STA entry point: installs UI/AppDomain exception handling, enables styles and runs RulesForm.',
'019c':'Logs a WinForms thread exception and displays a recovery message.',
'019d':'Logs the AppDomain exception object when it is an Exception.',
'019e':'Appends timestamp and exception text to eos_unhandled_errors.log; failures while logging are swallowed.',
'0001':'Original Confuser module initializer: initializes packed resources and the constant pool and starts anti-debug behavior. Retained for analysis; excluded from the readable compatibility build.',
'0002':'Injected anti-debug worker. Includes debugger/profiler checks, thread monitoring and fail-fast behavior. Retained source only; it was not executed.',
'0003':'Decodes the packed LZMA-style byte stream using the injected range-decoder types.',
'0009':'Decodes a packed resource assembly, loads it and installs an AssemblyResolve handler in the protected original. The analysis script recovered its resource bytes offline instead.',
'000a':'AssemblyResolve callback: returns the stored packed assembly when the requested assembly identity matches; otherwise returns null.'
}
catalog=[];assembly_summary=[]
for path in sorted((ROOT/'evidence').glob('*.members.json')):
    identity=path.name.removesuffix('.members.json');build,assembly=identity.split('.',1)
    evidence=json.loads(path.read_text(encoding='utf-8-sig')); n=evidence['normalized_result']
    export=ROOT/'evidence/method-exports'/identity
    if identity=='EOS.EOSClient':export=ROOT/'evidence/EOS-normalized'
    index=json.loads((export/'method-index.json').read_text(encoding='utf-8-sig'))
    index={x['token']:x for x in index}
    assembly_summary.append({'build':build,'assembly':assembly,'methods':len(n['methods']),'sha256':n['artifact']['sha256'],'mvid':n['module']['mvid']})
    for m in n['methods']:
        token=m['token']; row=index[token];readable=row['name'];type_name=row['type']
        if identity=='EOS.EOSClient':
            for old,new in aliases['types'].items():type_name=type_name.replace(old,new)
            readable=aliases['methods'].get(token,readable)
        source_file=export/'methods'/(token+'.cs')
        assert source_file.exists(),source_file
        source=source_file.read_text(encoding='utf-8-sig')
        description=summaries.get(type_name+'.'+row['name'])
        if identity=='EOS.EOSClient':description=eos_details.get(token[-4:],description)
        if not description:
            if not row['has_body']:description='Declaration without a managed method body. For interfaces, the implementation belongs to an implementing server/client, not to this contract DLL. P/Invoke declarations identify an external native boundary.'
            elif row['name'].startswith('get_'):description='Property getter: returns the stored or computed property value shown in the recovered body.'
            elif row['name'].startswith('set_'):description='Property setter: writes the supplied value to the corresponding field shown in the recovered body.'
            elif row['name']=='.ctor':description='Instance constructor: initializes the base class and the fields/collections shown below. An empty constructor adds no business operation.'
            elif row['name']=='.cctor':description='Static constructor: initializes this type before its first applicable use. Review its assignments and calls below.'
            elif row['instructions']<=12 and len(row['calls'] or [])==1:description='Short forwarding method. Its direct target and arguments are shown below; this is plumbing around that operation.'
            elif row['name']=='Dispose':description='Disposes owned components/resources when requested and delegates lifetime cleanup as shown below.'
            elif row['name']=='InitializeComponent':description='Constructs WinForms controls, assigns layout/style properties and wires event callbacks. Runtime business logic resides in those handlers.'
            elif identity=='EOS.EOSClient' and token[-4:] in {'0004','0005','0006','0007','0008'}:description='Injected generic constant decoder: transforms an integer identifier into a tagged offset in the protected constant pool and returns a string, scalar or array. Application string callsites were decoded offline in the readable project.'
            else:
                ops=[]
                targets=' '.join(x['target'] for x in row['calls'] or [])
                for marker,label in [('System.IO.','file/stream I/O'),('System.Threading.','threading/synchronization'),('System.Windows.Forms.','WinForms UI'),('System.Security.Cryptography.','cryptographic operations'),('System.Reflection.','reflection'),('System.Text.RegularExpressions.','regular expressions')]:
                    if marker in targets:ops.append(label)
                description='Recovered implementation'+(' involving '+', '.join(ops) if ops else '')+'. The full ordered body below is the implementation evidence; the call list records static references, not an observed runtime trace.'
        description,review=enrich(identity,row,description)
        if identity=='EOS.HashCodeProvider' and token=='0x06000001':
            source_file=ROOT/'src/EOS/HashCodeProvider/HashCodeProvider/IdentityHashCodeProvider.cs'
            source=source_file.read_text(encoding='utf-8-sig')
        catalog.append({'id':identity+':'+token,'build':build,'assembly':assembly,'token':token,'review_status':review,
          'type':type_name,'name':readable,'original_name':m['name'],'signature':row['signature'],
          'summary':description,'source':source,'source_path':'../'+source_file.relative_to(ROOT).as_posix(),
          'instructions_original':m['body'].get('instruction_count',0) if m.get('body') else 0,
          'has_body':row['has_body'],'calls':row['calls'] or [],'fields':row['fields'] or [],
          'sha256':n['artifact']['sha256'],'mvid':n['module']['mvid'],
          'evidence_path':'../'+path.relative_to(ROOT).as_posix(),
          'provenance':'Recovered and normalized C#' if identity=='EOS.EOSClient' else 'Decompiler-produced C#'})
assert len(catalog)==1426,len(catalog)
(ROOT/'evidence/method-catalog.json').write_text(json.dumps(catalog,ensure_ascii=True),encoding='utf-8')
print(json.dumps({'methods':len(catalog),'assemblies':len(assembly_summary)}))
