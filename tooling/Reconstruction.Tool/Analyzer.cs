using System.Text.Json;
using System.Text.RegularExpressions;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using System.Reflection.Metadata.Ecma335;

internal static class Analyzer {
public static void Run(string[] args) {
var input = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
Directory.CreateDirectory(output);
using var module = ModuleDefMD.Load(input); // File bytes only; does not load target into CLR.
string Token(IMDTokenProvider p) => $"0x{p.MDToken.Raw:x8}";
string Safe(string n) => Regex.Replace(n, "[^a-zA-Z0-9_]", "_");
bool Scrambled(string n) => n.Any(c => c > 127) || !Regex.IsMatch(n, "^[A-Za-z_][A-Za-z0-9_`]*$");
var rows = new List<object>();
foreach (var type in module.GetTypes()) {
    foreach (var method in type.Methods) {
        var body = method.Body;
        var calls = body?.Instructions.Where(i => i.Operand is IMethod && (i.OpCode.FlowControl == FlowControl.Call || i.OpCode.Code is Code.Ldftn or Code.Ldvirtftn))
            .Select(i => new { offset = $"IL_{i.Offset:x4}", opcode = i.OpCode.Name, target = ((IMethod)i.Operand).FullName, token = Token((IMethod)i.Operand) }).ToArray();
        rows.Add(new { token = Token(method), type_token = Token(type), type = type.FullName, name = method.Name.String,
            signature = method.FullName, has_body = body != null, instructions = body?.Instructions.Count ?? 0,
            calls, strings = body?.Instructions.Where(i => i.OpCode.Code == Code.Ldstr).Select(i => i.Operand.ToString()).ToArray(),
            fields = body?.Instructions.Where(i => i.Operand is IField).Select(i => ((IField)i.Operand).FullName).Distinct().ToArray() });
    }
}
var options = new JsonSerializerOptions { WriteIndented = true };
File.WriteAllText(Path.Combine(output, "method-index.json"), JsonSerializer.Serialize(rows, options));
File.WriteAllText(Path.Combine(output, "types.json"), JsonSerializer.Serialize(module.GetTypes().Select(t => new {
    token = Token(t), name = t.FullName, base_type = t.BaseType?.FullName,
    fields = t.Fields.Select(f => new { token = Token(f), name = f.Name.String, type = f.FieldSig?.Type.FullName })
}), options));
if (args.Length >= 3 && args[2] == "export") {
    var resolver = new UniversalAssemblyResolver(input,false,".NETFramework,Version=v3.5");
    if(args.Length>3)resolver.AddSearchDirectory(args[3]);
    var decompiler = new CSharpDecompiler(input,resolver,new DecompilerSettings { RemoveDeadStores = true });
    var methodDirectory=Path.Combine(output,"methods");Directory.CreateDirectory(methodDirectory);
    int failures=0;
    foreach(var method in module.GetTypes().SelectMany(t=>t.Methods)) {
        try {File.WriteAllText(Path.Combine(methodDirectory,Token(method)+".cs"),decompiler.DecompileAsString(MetadataTokens.EntityHandle((int)method.MDToken.Raw)));}
        catch(Exception ex){failures++;File.WriteAllText(Path.Combine(methodDirectory,Token(method)+".error.txt"),ex.ToString());}
    }
    Console.WriteLine($"Exported {rows.Count} method records, {failures} decompiler failures.");
    return;
}
if (args.Length < 3 || args[2] != "normalize") return;
var renames = new List<object>();
var originals = module.GetTypes().SelectMany(t => new IMemberDef[] {t}.Concat(t.Methods).Concat(t.Fields).Concat(t.Properties).Concat(t.Events))
    .ToDictionary(m => m, m => m.FullName);
foreach (var type in module.GetTypes()) {
    if (!type.IsGlobalModuleType && Scrambled(type.Name)) type.Name = $"Type_{type.MDToken.Rid:x4}";
    foreach (var field in type.Fields)
        if (Scrambled(field.Name)) field.Name = $"{Safe(field.FieldSig.Type.TypeName)}_F{field.MDToken.Rid:x4}";
    foreach (var method in type.Methods) {
        if (method.IsConstructor || !Scrambled(method.Name)) continue;
        string prefix = "Method";
        var instructions = method.Body?.Instructions;
        var targets = instructions?.Where(i => i.OpCode.Code is Code.Call or Code.Callvirt or Code.Newobj).Select(i => i.Operand as IMethod).ToArray();
        if (instructions != null && instructions.Count <= 12 && targets?.Length == 1)
            prefix = Safe(targets[0]!.DeclaringType.TypeName + "_" + targets[0]!.Name.String.Replace(".ctor", "Create"));
        if (method == module.EntryPoint) prefix = "Main";
        method.Name = $"{prefix}_M{method.MDToken.Rid:x4}";
    }
    foreach (var p in type.Properties) if (Scrambled(p.Name)) p.Name = $"Property_{p.MDToken.Rid:x4}";
    foreach (var e in type.Events) if (Scrambled(e.Name)) e.Name = $"Event_{e.MDToken.Rid:x4}";
}
foreach (var pair in originals) if (pair.Key.FullName != pair.Value)
    renames.Add(new { token = Token(pair.Key), original = pair.Value, readable = pair.Key.FullName });
var cleanRows = module.GetTypes().SelectMany(t=>t.Methods).Select(m=> {int count=0; for(int round=0;round<5;round++){int n=FlowCleaner.Clean(m);count+=n;if(n==0)break;}return new {token=Token(m), specialized_edges=count};}).ToArray();
File.WriteAllText(Path.Combine(output,"flow-cleaning.json"),JsonSerializer.Serialize(cleanRows,options));
var writerOptions = new ModuleWriterOptions(module);
writerOptions.MetadataOptions.Flags |= MetadataFlags.PreserveAll;
writerOptions.Logger = DummyLogger.NoThrowInstance;
module.Write(Path.Combine(output, "EOSClient.analysis.exe"), writerOptions);
File.WriteAllText(Path.Combine(output, "renames.json"), JsonSerializer.Serialize(renames, options));
Console.WriteLine($"Indexed {rows.Count} methods; normalized {renames.Count} names in analysis copy.");
}
}
