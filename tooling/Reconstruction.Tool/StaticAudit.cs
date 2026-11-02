using System.Security.Cryptography;
using System.Text.Json;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

internal static class StaticAudit {
 public static void Run(string root) {
  root=Path.GetFullPath(root);var options=new JsonSerializerOptions{WriteIndented=true};
  using var manifest=JsonDocument.Parse(File.ReadAllText(Path.Combine(root,"exam-input-manifest.json")));
  var selected=new HashSet<string>{"EOSClient.exe","PEALogin.exe","IRemote.dll","QuestionLib.dll","GradeLib.dll","EncryptData.dll","HashCodeProvider.dll"};
  var inputs=new List<object>();var assemblies=new List<object>();var expected=new[]{"ExamClient.frmEnglishExamClient","ExamClient.PreExamEntry","PEAClient.FrmPEAClient"};
  var runtimeSearch=new List<object>();
  foreach(var e in manifest.RootElement.EnumerateArray()) {
   string path=Path.Combine(root,e.GetProperty("Path").GetString()!),build=e.GetProperty("Build").GetString()!,name=Path.GetFileName(path);
   string hash=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
   inputs.Add(new{build,path,sha256=hash,matches_manifest=hash.Equals(e.GetProperty("SHA256").GetString(),StringComparison.OrdinalIgnoreCase)});
   if(Path.GetExtension(path) is not (".exe" or ".dll"))continue;
   using var original=ModuleDefMD.Load(path);
   runtimeSearch.Add(new{build,file=name,types=original.GetTypes().Count(),matches=original.GetTypes().Where(t=>expected.Contains(t.FullName)).Select(t=>t.FullName).ToArray()});
   if(!selected.Contains(name))continue;
   string stem=Path.GetFileNameWithoutExtension(path),builtName=name=="EOSClient.exe"?"EOSClient.Reconstructed.exe":name;
   string builtPath=Path.Combine(root,"src",build,stem,"bin","Debug","net48",builtName);
   using var rebuilt=ModuleDefMD.Load(builtPath);
   var rebuiltMethods=rebuilt.GetTypes().SelectMany(t=>t.Methods).GroupBy(m=>m.FullName).ToDictionary(g=>g.Key,g=>g.First());
   var methodRows=new List<object>();
   foreach(var m in original.GetTypes().SelectMany(t=>t.Methods)) {
    rebuiltMethods.TryGetValue(m.FullName,out var other);
    if(name=="EOSClient.exe")other=rebuilt.GetTypes().SelectMany(t=>t.Methods).FirstOrDefault(x=>x.Name.String.EndsWith($"_M{m.MDToken.Rid:x4}"));
    var first=Canonical(m);var second=other==null?null:Canonical(other);
    methodRows.Add(new{token=$"0x{m.MDToken.Raw:x8}",name=m.FullName,rebuilt_name=other?.FullName,
     original_instructions=m.Body?.Instructions.Count??0,exact_canonical_il=second!=null&&first.SequenceEqual(second),
     ordered_calls_match=other!=null&&Calls(m).SequenceEqual(Calls(other)),has_body=m.HasBody});
   }
   var contracts=new List<object>();
   if(stem=="IRemote")foreach(var t in original.GetTypes().Where(t=>!t.IsGlobalModuleType)) {
    var b=rebuilt.Find(t.FullName,false);
    string[] Fields(TypeDef x)=>x.Fields.Select(f=>$"{f.Name}:{f.FieldSig}:{(f.HasConstant?f.Constant.Value:"")}").Order().ToArray();
    string[] Methods(TypeDef x)=>x.Methods.Where(m=>!m.IsConstructor).Select(m=>m.Name+":"+m.MethodSig).Order().ToArray();
    contracts.Add(new{type=t.FullName,fields_match=b!=null&&Fields(t).SequenceEqual(Fields(b)),method_signatures_match=b!=null&&Methods(t).SequenceEqual(Methods(b)),serializable_match=b!=null&&t.IsSerializable==b.IsSerializable});
   }
   assemblies.Add(new{build,name,original_sha256=hash,rebuilt_sha256=Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(builtPath))),
    original_mvid=original.Mvid,original_runtime=original.RuntimeVersion,rebuilt_runtime=rebuilt.RuntimeVersion,
    original_methods=original.GetTypes().Sum(t=>t.Methods.Count),rebuilt_methods=rebuilt.GetTypes().Sum(t=>t.Methods.Count),methods=methodRows,contracts});
  }
  string normalizedPath=Path.Combine(root,"evidence","EOS-normalized","EOSClient.analysis.exe");
  var eosPath=Path.Combine(root,manifest.RootElement.EnumerateArray().First(e=>e.GetProperty("Path").GetString()!.EndsWith("EOSClient.exe")).GetProperty("Path").GetString()!);
  using var eos=ModuleDefMD.Load(eosPath);using var normalized=ModuleDefMD.Load(normalizedPath);
  var normalizedRows=eos.GetTypes().SelectMany(t=>t.Methods).Select(m=>{
   var n=(MethodDef)normalized.ResolveToken(m.MDToken.Raw);return new{token=$"0x{m.MDToken.Raw:x8}",
    ordered_call_tokens_match=CallTokens(m).SequenceEqual(CallTokens(n)),
    exception_clause_count_match=(m.Body?.ExceptionHandlers.Count??0)==(n.Body?.ExceptionHandlers.Count??0),
    original_instructions=m.Body?.Instructions.Count??0,normalized_instructions=n.Body?.Instructions.Count??0};}).ToArray();
  var report=new{scope="Supplied manifest only; no server contacted, no application launched",inputs,assemblies,runtime_type_search=runtimeSearch,
   eos_normalization=normalizedRows,limitations=new[]{"Canonical IL equality is a bounded static check; mismatches require interpretation and are not automatically bugs.","Ordered call preservation does not prove control-flow equivalence.","Framework 4.8 and excluded Confuser startup are explicit build adaptations.","No claim of runtime equivalence is made by this static audit."}};
  File.WriteAllText(Path.Combine(root,"evidence","reconstruction-static-audit.json"),JsonSerializer.Serialize(report,options));
  Console.WriteLine($"Audited {inputs.Count} inputs, {assemblies.Count} selected assemblies, {normalizedRows.Length} EOS method tokens.");
 }
 static string[] Calls(MethodDef m)=>m.Body?.Instructions.Where(i=>i.Operand is IMethod && (i.OpCode.FlowControl==FlowControl.Call||i.OpCode.Code is Code.Ldftn or Code.Ldvirtftn)).Select(i=>i.OpCode.Name+":"+((IMethod)i.Operand).FullName).ToArray()??[];
 static string[] CallTokens(MethodDef m)=>m.Body?.Instructions.Where(i=>i.Operand is IMethod && (i.OpCode.FlowControl==FlowControl.Call||i.OpCode.Code is Code.Ldftn or Code.Ldvirtftn)).Select(i=>i.OpCode.Name+":"+((IMethod)i.Operand).MDToken.Raw).ToArray()??[];
 static string[] Canonical(MethodDef m) {
  if(!m.HasBody)return [];
  m.Body.SimplifyMacros(m.Parameters);var code=m.Body.Instructions.Where(i=>i.OpCode.Code!=Code.Nop).ToArray();
  var all=m.Body.Instructions;int Position(Instruction i){int n=all.IndexOf(i);while(n<all.Count&&all[n].OpCode.Code==Code.Nop)n++;return n==all.Count?code.Length:Array.IndexOf(code,all[n]);}
  return code.Select(i=>i.OpCode.Name+":"+(i.Operand switch {
   Instruction x=>Position(x).ToString(),IList<Instruction> xs=>string.Join(',',xs.Select(Position)),
   Local l=>"local:"+l.Index,Parameter p=>"arg:"+p.Index,IMethod mm=>mm.FullName,IField f=>f.FullName,ITypeDefOrRef t=>t.FullName,
   null=>"",_=>i.Operand.ToString()})).ToArray();
 }
}
