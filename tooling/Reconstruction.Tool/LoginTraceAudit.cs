using System.Text.Json;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

// Independent CIL interpreter with mocked external operations. This does NOT load
// a target in the CLR, execute a GUI assembly, or contact a server. Unsupported
// instructions/APIs fail the audit rather than silently succeeding.
internal static class LoginTraceAudit {
 internal sealed class Obj(string type) {public string Type=type;public Dictionary<string,object?> Fields=new();public override string ToString()=>Type;}
 sealed record Address(Func<object?> Get,Action<object?> Set);
 sealed class Fault(string message):Exception(message);
 sealed class Halt:Exception;
 record Scenario(string Name,int Status=0,string Version="2.0.2.6.0.7.2.9",bool Ping=true,bool Skip=false,bool NullPhoto=false,string? Blank=null,bool ProxyFault=false,bool VersionFault=false,bool GuiFault=false,bool PublicIp=false);
 sealed class Machine {
  readonly ModuleDefMD module;readonly Scenario scenario;readonly bool eos;
  readonly Dictionary<string,string> strings;readonly Obj self,server=new("server"),data=new("data"),info=new("info");
  readonly Dictionary<string,object?> statics=new();public List<string> Events=new();public int Steps;
  public Machine(ModuleDefMD module,Scenario scenario,bool eos,Dictionary<string,string> strings){
   this.module=module;this.scenario=scenario;this.eos=eos;this.strings=strings;
   var t=module.GetTypes().First(x=>x.Name=="AuthenticateForm"||x.MDToken.Raw==0x0200000d&&eos);self=new(t.FullName);
   string User(string kind)=>scenario.Blank==kind?"  ":kind switch{"user"=>" Student ","password"=>"pass","test"=>"LAB","domain"=>"FU.EDU.VN","server"=>"LAB server",_=>""};
   foreach(var f in t.Fields){string name=f.Name;uint rid=f.MDToken.Rid;object? v=null;
    bool OriginalField(uint n)=>eos&&t.Name!="AuthenticateForm"&&rid==n;
    string? tag=name switch{"txtUser"=>"user","txtPassword"=>"password","txtTestName" or "txtExamCode"=>"test","txtDomain"=>"domain","cbServer" or "cmbServer" or "cboServer"=>"server","btnLogin"=>"login","btnCancel" or "btnExit"=>"exit",_=>null};
    if(OriginalField(0x30))tag="user";if(OriginalField(0x31))tag="password";if(OriginalField(0x3e))tag="test";if(OriginalField(0x3f))tag="server";if(OriginalField(0x2f))tag="login";if(OriginalField(0x35))tag="exit";
    if(f.FieldSig.Type.FullName.StartsWith("System.Windows.Forms.")){var c=new Obj(tag??name);c.Fields["Text"]=User(tag??"");c.Fields["Enabled"]=true;v=c;if(f.FieldSig.Type.FullName.EndsWith("ComboBox")){c.Type="server";c.Fields["Text"]=User("server");var ep=new Obj("EOSClient.EndpointItem");ep.Fields["IP"]="10.0.0.2";ep.Fields["Port"]=9000;ep.Fields["Public_IP"]=scenario.PublicIp?"203.0.113.10":"";c.Fields["SelectedItem"]=ep;}}
    if(name is "domain" or "String_F0032"||OriginalField(0x32))v=User("domain");
    if(name is "si" or "serverInfo" or "ServerInfo_F004d"||OriginalField(0x4d))v=info;
    self.Fields[name]=v;
   }
   info.Fields["IP"]="10.0.0.2";info.Fields["Port"]=9000;
   data.Fields["Status"]=scenario.Status;data.Fields["GUI"]=new byte[]{1,2,3,4};data.Fields["OriginSize"]=4;data.Fields["SkipCameraCheckIn"]=scenario.Skip;
  }
  public object? Run(MethodDef entry){try{return Execute(entry,[self,new Obj("sender"),new Obj("event")]);}catch(Halt){Events.Add("halt");return null;}}
  public string State(){
   object? View(object? value)=>value switch{Obj o=>o.Fields.OrderBy(x=>x.Key).ToDictionary(x=>x.Key,x=>View(x.Value)),Array a=>"array:"+a.Length,_=>value};
   var ui=self.Fields.Values.OfType<Obj>().Where(o=>o.Type is "login" or "exit" or "user" or "password" or "test" or "server").OrderBy(o=>o.Type).Select(o=>new{control=o.Type,enabled=o.Fields.GetValueOrDefault("Enabled")}).ToArray();
   return JsonSerializer.Serialize(new{session=View(data),server=View(info),ui});
  }
  object? Execute(MethodDef m,object?[] args){
   if(!m.HasBody)throw new NotSupportedException("Body absent: "+m.FullName);
   m.Body.SimplifyMacros(m.Parameters);var code=m.Body.Instructions;var positions=code.Select((x,i)=>(x,i)).ToDictionary(x=>x.x,x=>x.i);
   var locals=m.Body.Variables.Select(l=>l.Type.ElementType is ElementType.I4 or ElementType.U4 or ElementType.Boolean or ElementType.U1? (object?)0:null).ToArray();var stack=new List<object?>();
   object? Pop(){if(stack.Count==0)throw new NotSupportedException("Stack underflow at "+m.FullName);var x=stack[^1];stack.RemoveAt(stack.Count-1);return x;}
   object? Read(object? o)=>o is Address a?a.Get():o;
   for(int pc=0;pc<code.Count;pc++){
    if(++Steps>250000)throw new NotSupportedException("CIL step budget exceeded");var i=code[pc];var op=i.OpCode.Code;
    try{
     if(i.IsLdcI4()){stack.Add(i.GetLdcI4Value());continue;}
     switch(op){
      case Code.Nop:break;case Code.Ldnull:stack.Add(null);break;case Code.Ldstr:stack.Add(i.Operand);break;
      case Code.Ldarg:stack.Add(args[((Parameter)i.Operand).Index]);break;
      case Code.Ldarga:{int n=((Parameter)i.Operand).Index;stack.Add(new Address(()=>args[n],v=>args[n]=v));break;}
      case Code.Starg:args[((Parameter)i.Operand).Index]=Pop();break;
      case Code.Ldloc:stack.Add(locals[((Local)i.Operand).Index]);break;
      case Code.Stloc:locals[((Local)i.Operand).Index]=Pop();break;
      case Code.Ldloca:{int n=((Local)i.Operand).Index;stack.Add(new Address(()=>locals[n],v=>locals[n]=v));break;}
      case Code.Dup:stack.Add(stack[^1]);break;case Code.Pop:Pop();break;
      case Code.Ldc_I8:stack.Add(i.Operand);break;
      case Code.Xor:case Code.And:case Code.Or:case Code.Add:case Code.Sub:case Code.Mul:case Code.Rem_Un:case Code.Rem:case Code.Shr_Un:case Code.Shr:case Code.Shl:{int b=Int(Pop()),a=Int(Pop());unchecked{stack.Add(op switch{Code.Xor=>a^b,Code.And=>a&b,Code.Or=>a|b,Code.Add=>a+b,Code.Sub=>a-b,Code.Mul=>a*b,Code.Rem_Un=>(int)((uint)a%(uint)b),Code.Rem=>a%b,Code.Shr_Un=>(int)((uint)a>>(b&31)),Code.Shr=>a>>(b&31),_=>a<<(b&31)});}break;}
      case Code.Conv_I4:case Code.Conv_U4:stack.Add(Int(Pop()));break;
      case Code.Conv_U1:stack.Add((int)(byte)Int(Pop()));break;
      case Code.Ceq:{var b=Read(Pop());var a=Read(Pop());stack.Add(Equals(a,b)?1:0);break;}
      case Code.Cgt:case Code.Cgt_Un:case Code.Clt:case Code.Clt_Un:{var b=Read(Pop());var a=Read(Pop());bool v;if(op==Code.Cgt_Un&&b==null)v=a!=null;else v=op switch{Code.Cgt=>Int(a)>Int(b),Code.Clt=>Int(a)<Int(b),Code.Cgt_Un=>(uint)Int(a)>(uint)Int(b),_=>(uint)Int(a)<(uint)Int(b)};stack.Add(v?1:0);break;}
      case Code.Br:pc=positions[(Instruction)i.Operand]-1;break;
      case Code.Brtrue:case Code.Brfalse:{bool v=Truth(Pop());if(v==(op==Code.Brtrue))pc=positions[(Instruction)i.Operand]-1;break;}
      case Code.Beq:case Code.Bne_Un:case Code.Bge:case Code.Bge_Un:case Code.Bgt:case Code.Bgt_Un:case Code.Ble:case Code.Ble_Un:case Code.Blt:case Code.Blt_Un:{var b=Read(Pop());var a=Read(Pop());bool branch=op switch{Code.Beq=>Equals(a,b),Code.Bne_Un=>!Equals(a,b),Code.Bge=>Int(a)>=Int(b),Code.Bgt=>Int(a)>Int(b),Code.Ble=>Int(a)<=Int(b),Code.Blt=>Int(a)<Int(b),Code.Bge_Un=>(uint)Int(a)>=(uint)Int(b),Code.Bgt_Un=>(uint)Int(a)>(uint)Int(b),Code.Ble_Un=>(uint)Int(a)<=(uint)Int(b),_=>(uint)Int(a)<(uint)Int(b)};if(branch)pc=positions[(Instruction)i.Operand]-1;break;}
      case Code.Switch:{int v=Int(Pop());var targets=(IList<Instruction>)i.Operand;if((uint)v<targets.Count)pc=positions[targets[v]]-1;break;}
      case Code.Leave:stack.Clear();pc=positions[(Instruction)i.Operand]-1;break;
      case Code.Ldfld:{var f=(IField)i.Operand;var o=(Obj)Read(Pop())!;stack.Add(o.Fields.GetValueOrDefault(f.Name.String));break;}
      case Code.Stfld:{var f=(IField)i.Operand;var value=Pop();var o=(Obj)Read(Pop())!;o.Fields[f.Name.String]=value;break;}
      case Code.Ldsfld:{var f=(IField)i.Operand;if(!statics.TryGetValue(f.FullName,out var value)){value=f.FieldSig.Type.FullName==f.DeclaringType.FullName?new Obj("closure"):null;statics[f.FullName]=value;}stack.Add(value);break;}
      case Code.Stsfld:statics[((IField)i.Operand).FullName]=Pop();break;
      case Code.Ldtoken:stack.Add(((ITypeDefOrRef)i.Operand).FullName);break;
      case Code.Ldftn:case Code.Ldvirtftn:stack.Add(new Obj("callback"));break;
      case Code.Castclass:break;
      case Code.Isinst:{var value=Pop();stack.Add(value);break;}
      case Code.Newarr:{int n=Int(Pop());stack.Add(((ITypeDefOrRef)i.Operand).FullName=="System.Byte"?new byte[n]:new object?[n]);break;}
      case Code.Ldlen:stack.Add(((Array)Read(Pop())!).Length);break;
      case Code.Stelem_Ref:case Code.Stelem_I1:{var value=Pop();int n=Int(Pop());var a=(Array)Read(Pop())!;a.SetValue(op==Code.Stelem_I1?(byte)Int(value):value,n);break;}
      case Code.Ldelem_Ref:{int n=Int(Pop());stack.Add(((Array)Read(Pop())!).GetValue(n));break;}
      case Code.Box:case Code.Unbox_Any:break;
      case Code.Call:case Code.Callvirt:case Code.Newobj:{
       var target=(IMethod)i.Operand;var sig=target.MethodSig;var callArgs=new object?[sig.Params.Count+(sig.HasThis&&op!=Code.Newobj?1:0)];for(int k=callArgs.Length-1;k>=0;k--)callArgs[k]=Read(Pop());
       object? result=Call(target,callArgs,op==Code.Newobj);
       if(op==Code.Newobj||sig.RetType.ElementType!=ElementType.Void)stack.Add(result);break;
      }
      case Code.Ret:return m.MethodSig.RetType.ElementType==ElementType.Void?null:Pop();
      case Code.Throw:throw new Fault(Pop()?.ToString()??"throw");
      default:throw new NotSupportedException($"Unsupported {op} in {m.FullName} at {i.Offset:x4}");
     }
    }catch(Fault fault){
     var handler=m.Body.ExceptionHandlers.Where(h=>h.HandlerType==ExceptionHandlerType.Catch&&positions[h.TryStart]<=pc&&(h.TryEnd==null||pc<positions[h.TryEnd])).OrderByDescending(h=>positions[h.TryStart]).FirstOrDefault();
     if(handler==null)throw;stack.Clear();var exception=new Obj("Exception");exception.Fields["Message"]=fault.Message;stack.Add(exception);pc=positions[handler.HandlerStart]-1;
    }
    catch(Exception ex)when(ex is not Halt){throw new NotSupportedException($"{m.Name} IL_{i.Offset:x4} ({op}): {ex.Message}",ex);}
   }
   throw new NotSupportedException("Method ran past ret: "+m.FullName);
  }
  object? Call(IMethod target,object?[] a,bool create){
   string type=target.DeclaringType.FullName,name=target.Name;
   if(type=="EOSClient.EndpointItem"&&name.StartsWith("get_"))return ((Obj)a[0]!).Fields.GetValueOrDefault(name[4..]);
   if(type=="System.Int32"&&name=="ToString")return Convert.ToString(a[0],System.Globalization.CultureInfo.InvariantCulture);
   var def=target.ResolveMethodDef();
   if(def?.Module==module && !create){
    if(def.DeclaringType.IsGlobalModuleType && def.MDToken.Rid is >=4 and <=8){string key=$"{def.MDToken.Raw:x8}:{Int(a[0])}";return strings.TryGetValue(key,out var text)?text:throw new NotSupportedException("Unmapped constant "+key);}
    return Execute(def,a);
   }
   if(create){if(type=="System.String")throw new NotSupportedException("String constructor");return new Obj(type);}
   if(type=="System.String")return name switch{
    "Trim"=>((string)a[0]!).Trim(),"ToLower"=>((string)a[0]!).ToLowerInvariant(),"ToUpper"=>((string)a[0]!).ToUpperInvariant(),
    "IsNullOrEmpty"=>string.IsNullOrEmpty((string?)a[0])?1:0,"get_Length"=>((string)a[0]!).Length,
    "Equals"=>a.Length==3?string.Equals((string?)a[0],(string?)a[1],(StringComparison)Int(a[2]))?1:0:Equals(a[0],a[1])?1:0,
    "Concat"=>a.Length==1&&a[0] is object?[] many?string.Concat(many):string.Concat(a),_=>throw new NotSupportedException(type+"."+name)};
   if(name=="get_Text" && a[0] is Obj control)return control.Fields.GetValueOrDefault("Text")??"";
   if(name=="set_Enabled"&&a[0] is Obj button){button.Fields["Enabled"]=Truth(a[1]);Events.Add("enabled:"+button.Type+":"+Truth(a[1]));return null;}
   if(name=="get_SelectedItem")return ((Obj)a[0]!).Fields.GetValueOrDefault("SelectedItem");
   if(type is "IRemote.ServerInfo" or "IRemote.RegisterData" ||type=="EOSClient.EndpointItem") {
    var o=(Obj)a[0]!;if(name.StartsWith("get_"))return o.Fields.GetValueOrDefault(name[4..]);if(name.StartsWith("set_")){o.Fields[name[4..]]=a[1];return null;}
   }
   if(type=="IRemote.IRemoteServer"){
    if(name=="Ping"){Events.Add("ping");return scenario.Ping?1:0;}
    if(name=="GetMinRequiredClientVersion"){Events.Add("version");if(scenario.VersionFault)throw new Fault("version fault");return scenario.Version;}
    if(name is "ConductExam" or "ConductTest"){Events.Add(name+":"+JsonSerializer.Serialize(((Obj)a[1]!).Fields.OrderBy(p=>p.Key).ToDictionary()));return data;}
   }
   if(type=="System.Activator"&&name=="GetObject"){Events.Add("proxy:"+a[1]);if(scenario.ProxyFault)throw new Fault("proxy fault");return server;}
   if(type=="System.Environment"&&name=="get_MachineName")return "LABPC";
   if(type=="System.Type"&&name=="GetTypeFromHandle")return a[0];
   if(type=="System.Type"&&name=="get_TypeHandle")return a[0];
   if(type.EndsWith(".GZipHelper")&&name=="DeCompress"){Events.Add("decompress");if(scenario.GuiFault)throw new Fault("GUI decode fault");return new byte[]{4,3,2,1};}
   if(type=="System.Reflection.Assembly"&&name=="Load"){Events.Add("load-assembly");return new Obj("assembly");}
   if(type=="System.Reflection.Assembly"&&name=="GetType")return a[1];
   if(type=="System.Type"&&name=="GetMethod")return (string)a[0]+"::"+(string)a[1]!;
   if(type=="System.Reflection.MethodBase"&&name=="Invoke"){
    string key=(string)a[0]!;Events.Add("reflect:"+key);if(key.EndsWith("::RunCheckIn"))return scenario.NullPhoto?null:new byte[]{7,8};return null;
   }
   if(type=="System.Activator"&&name=="CreateInstance"){Events.Add("create-form:"+a[0]);return new Obj((string)a[0]!);}
   if(type=="IRemote.IExamclient"&&name is "SetExamData" or "SetAuthenForm"){Events.Add(name);return null;}
   if(type=="System.Windows.Forms.MessageBox"&&name=="Show"){Events.Add("message:"+a[0]);return 1;}
   if(name is "Show" or "Hide"&&type.StartsWith("System.Windows.Forms.")){Events.Add(name);return null;}
   if(type=="System.Windows.Forms.Form"&&name=="add_FormClosed"){Events.Add("cleanup-handler");return null;}
   if(type=="System.Windows.Forms.Application"&&name=="Exit"){Events.Add("application-exit");return null;}
   if(type=="System.Environment"&&name=="Exit")throw new Halt();
   if(type=="System.Windows.Forms.Application"&&name=="set_UseWaitCursor"||type=="System.Windows.Forms.Cursor"&&name=="set_Current")return null;
   if(type=="System.Windows.Forms.Cursors"&&name=="get_Default")return new Obj("cursor");
   if(type=="System.Exception"&&name=="get_Message")return ((Obj)a[0]!).Fields["Message"];
   if(type=="System.Object"&&name=="ToString")return a[0] is Obj o&&o.Type=="Exception"?o.Fields["Message"]:a[0]?.ToString();
   throw new NotSupportedException("Unsupported API "+type+"::"+name);
  }
  static int Int(object? o)=>o switch{int n=>n,uint n=>unchecked((int)n),long n=>unchecked((int)n),bool b=>b?1:0,byte b=>b,null=>0,_=>Convert.ToInt32(o)};
  static bool Truth(object? o)=>o!=null&&(o is not int n||n!=0)&&(o is not bool b||b);
 }
 public static void Run(string root){
  root=Path.GetFullPath(root);using var input=JsonDocument.Parse(File.ReadAllText(Path.Combine(root,"exam-input-manifest.json")));
  var constants=JsonSerializer.Deserialize<JsonElement[]>(File.ReadAllText(Path.Combine(root,"evidence","EOS-normalized","decoded-strings.json")))!;
  var strings=constants.ToDictionary(x=>x.GetProperty("decoder_token").GetString()![2..]+":"+x.GetProperty("argument").GetInt32(),x=>x.GetProperty("value").GetString()!);
  var rows=new List<object>();int failures=0;
  foreach(string pack in new[]{"EOS","PEA-CSD","PEA-PRN"}){
   bool eos=pack=="EOS";string name=eos?"EOSClient.exe":"PEALogin.exe";
   string originalPath=Path.Combine(root,input.RootElement.EnumerateArray().First(x=>x.GetProperty("Build").GetString()==pack&&Path.GetFileName(x.GetProperty("Path").GetString())==name).GetProperty("Path").GetString()!);
   var modes=new Dictionary<string,string>{{"original",originalPath},{"rebuilt",Path.Combine(root,"src",pack,Path.GetFileNameWithoutExtension(name),"bin","Debug","net48",eos?"EOSClient.Reconstructed.exe":name)}};
   if(eos)modes.Add("normalized",Path.Combine(root,"evidence","EOS-normalized","EOSClient.analysis.exe"));
   var scenarios=new List<Scenario>();for(int status=0;status<(eos?10:9);status++)scenarios.Add(new("status-"+status,status));
   foreach(string blank in new[]{"user","password","test","domain"}.Concat(eos?new[]{"server"}:[]))scenarios.Add(new("blank-"+blank,Blank:blank));
   scenarios.Add(new("proxy-fault",ProxyFault:true));scenarios.Add(new("GUI-decode-fault",GuiFault:true));
   if(eos)scenarios.AddRange([new("ping-false",Ping:false),new("version-mismatch",Version:"different"),new("version-empty",Version:""),new("version-fault",VersionFault:true),new("public-IP",PublicIp:true),new("camera-skipped",Skip:true),new("camera-null",NullPhoto:true)]);
   foreach(var scenario in scenarios){
    var outcomes=new Dictionary<string,object>();var traces=new Dictionary<string,string[]>();
    foreach(var mode in modes){try{
     using var module=ModuleDefMD.Load(mode.Value);var entry=mode.Key=="rebuilt"?module.GetTypes().SelectMany(t=>t.Methods).First(m=>m.Name==(eos?"LoginClick_M0046":"btnLogin_Click")):(MethodDef)module.ResolveToken(eos?0x06000046u:0x06000004u);
     var machine=new Machine(module,scenario,eos,strings);machine.Run(entry);traces[mode.Key]=machine.Events.Append("FINAL-STATE:"+machine.State()).ToArray();outcomes[mode.Key]=new{events=machine.Events,final_state=machine.State(),steps=machine.Steps};
    }catch(Exception ex){outcomes[mode.Key]=new{error=ex.Message};}}
    bool pass=traces.Count==modes.Count&&traces.Values.All(t=>t.SequenceEqual(traces["original"]));if(!pass)failures++;
    rows.Add(new{pack,scenario=scenario.Name,pass,outcomes});
   }
  }
  File.WriteAllText(Path.Combine(root,"evidence","login-trace-audit.json"),JsonSerializer.Serialize(new{mode="Independent CIL simulation with mocked external calls; no target loaded/executed by CLR",cases=rows,failures},new JsonSerializerOptions{WriteIndented=true}));
  Console.WriteLine($"Login trace audit: {rows.Count} scenarios, {failures} unresolved/failed.");
 }
}
