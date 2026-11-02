using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

// Only a fixed allowlist of pure/data/file helpers is invoked. No client EXE,
// database method, process-control helper, timer, camera or network call runs.
internal static class Program {
 static string root,build,mode,fixture;static Dictionary<string,string> originals;
 static readonly JavaScriptSerializer Json=new JavaScriptSerializer{MaxJsonLength=int.MaxValue};
 static readonly SortedDictionary<string,object> Results=new SortedDictionary<string,object>();
 static readonly Dictionary<string,Assembly> Loaded=new Dictionary<string,Assembly>();
 static string LibraryPath(string name) => mode=="original"?originals[name]:Path.Combine(root,"src",build,Path.GetFileNameWithoutExtension(name),"bin","Debug","net48",name);
 static Assembly Library(string name) {if(!new[]{"EncryptData.dll","GradeLib.dll","QuestionLib.dll","HashCodeProvider.dll","IRemote.dll"}.Contains(name))throw new Exception("Not an allowed helper assembly");if(!Loaded.ContainsKey(name))Loaded[name]=Assembly.LoadFrom(LibraryPath(name));return Loaded[name];}
 static object Invoke(Type type,string name,object self,params object[] args) {
  var methods=type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance).Where(m=>m.Name==name&&m.GetParameters().Length==args.Length).ToArray();
  var match=methods.FirstOrDefault(m=>m.GetParameters().Select((p,i)=>args[i]==null||p.ParameterType.IsInstanceOfType(args[i])).All(x=>x))??methods.Single();
  return match.Invoke(self,args);
 }
 static void Test(string name,Func<object> run) {try{Results[name]=run();}catch(Exception ex){while(ex is TargetInvocationException&&ex.InnerException!=null)ex=ex.InnerException;Results[name]=new{exception=ex.GetType().FullName};}}
 static void Set(object obj,string name,object value) {var t=obj.GetType();var p=t.GetProperty(name);if(p!=null)p.SetValue(obj,value,null);else t.GetField(name).SetValue(obj,value);}
 static object Get(object obj,string name) {var t=obj.GetType();var p=t.GetProperty(name);return p!=null?p.GetValue(obj,null):t.GetField(name).GetValue(obj);}
 static object Snapshot(object obj) {
  if(obj==null)return null;
  if(obj is byte[])return Convert.ToBase64String((byte[])obj);
  if(obj is string)return ((string)obj).Replace(fixture,"<fixture>");
  if(obj.GetType().IsPrimitive||obj is decimal)return obj;
  if(obj is DateTime)return ((DateTime)obj).ToString("O");
  if(obj is IEnumerable)return ((IEnumerable)obj).Cast<object>().Select(Snapshot).ToArray();
  var result=new SortedDictionary<string,object>();
  foreach(var p in obj.GetType().GetProperties().Where(p=>p.CanRead&&p.GetIndexParameters().Length==0))result[p.Name]=Snapshot(p.GetValue(obj,null));
  return result;
 }
 static string F(string name)=>Path.Combine(fixture,name);
 static void Main(string[] args) {
  root=Path.GetFullPath(args[0]);build=args[1];mode=args[2];fixture=Path.Combine(root,"evidence","audit-fixtures",build,mode);Directory.CreateDirectory(fixture);
  var entries=Json.Deserialize<List<Dictionary<string,object>>>(File.ReadAllText(Path.Combine(root,"exam-input-manifest.json")));
  originals=entries.Where(x=>(string)x["Build"]==build).ToDictionary(x=>Path.GetFileName((string)x["Path"]),x=>Path.Combine(root,(string)x["Path"]));
  AppDomain.CurrentDomain.AssemblyResolve+=(s,e)=> {string file=new AssemblyName(e.Name).Name+".dll";return originals.ContainsKey(file)?Assembly.LoadFrom(originals[file]):null;};
  Crypto();Compression();Contracts();
  if(build=="EOS"){QuestionHelpers();IdentityHash();}else PracticalHelpers();
  string output=Path.Combine(root,"evidence",$"probe-{build}-{mode}.json");File.WriteAllText(output,Json.Serialize(Results));
  Console.WriteLine($"{build} / {mode}: {Results.Count} synthetic helper observations.");
 }
 static void Crypto() {
  var t=Library("EncryptData.dll").GetType("EncryptData.EncryptSupport");
  foreach(var message in new[]{"","abc","Việt Nam"}) {var value=message;Test("crypto.md5."+value,()=>((string)Invoke(t,"GetMD5",null,value)).Select(c=>(int)c).ToArray());}
  Test("crypto.object-null",()=>Invoke(t,"ObjectToByteArray",null,new object[]{null}));
  Test("crypto.serialize-roundtrip",()=>{var bytes=(byte[])Invoke(t,"ObjectToByteArray",null,new object[]{new List<string>{"a","β"}});return Snapshot(Invoke(t,"ByteArrayToObject",null,bytes));});
  Test("crypto.deserialize-invalid",()=>Invoke(t,"ByteArrayToObject",null,new byte[]{1,2,3}));
  foreach(var text in new[]{"","Hi","Fixture dữ liệu"}){var value=text;Test("crypto.des."+value,()=>{var encrypted=(string)Invoke(t,"Encryption",null,Encoding.Unicode.GetBytes(value),"p4ssW0rd");var decrypted=(string)Invoke(t,"Decryption",null,Convert.FromBase64String(encrypted),"p4ssW0rd");return new{cipher=encrypted,decoded_units=decrypted.Select(c=>(int)c).ToArray()};});}
  Test("crypto.invalid-key",()=>Invoke(t,"Encryption",null,Encoding.Unicode.GetBytes("fixture"),"short"));
  Test("crypto.file-roundtrip",()=>{string method=build=="EOS"?"EncryptQuestions_SaveToFile":"Encrypt_SaveToFile";string read=build=="EOS"?"DecryptQuestions_FromFile":"Decrypt_FromFile";var bytes=Encoding.Unicode.GetBytes("Fixture");Invoke(t,method,null,F("synthetic-encrypted.bin"),bytes,"p4ssW0rd");return Convert.ToBase64String((byte[])Invoke(t,read,null,F("synthetic-encrypted.bin"),"p4ssW0rd"));});
  if(build=="EOS")Test("crypto.aes-string-roundtrip",()=>{var x=(string)Invoke(t,"Encrypt",null,"Fixture 日本語","0123456789abcdef");return new{cipher=x,text=Invoke(t,"Decrypt",null,x,"0123456789abcdef")};});
 }
 static void Compression() {
  string lib=build=="EOS"?"QuestionLib":"GradeLib";var t=Library(lib+".dll").GetType(lib+".GZipHelper");
  foreach(int size in new[]{0,1,1024,131072}) {int n=size;Test("gzip.roundtrip."+n,()=>{var input=Enumerable.Range(0,n).Select(i=>(byte)(i%251)).ToArray();var zipped=(byte[])Invoke(t,"Compress",null,input);var actual=(byte[])Invoke(t,"DeCompress",null,zipped,n);return new{length=actual.Length,equal=actual.SequenceEqual(input)};});}
  Test("gzip.short-request",()=>{var zipped=(byte[])Invoke(t,"Compress",null,Encoding.ASCII.GetBytes("abcdef"));return Convert.ToBase64String((byte[])Invoke(t,"DeCompress",null,zipped,3));});
  Test("gzip.oversized-request",()=>{var zipped=(byte[])Invoke(t,"Compress",null,Encoding.ASCII.GetBytes("abc"));return Convert.ToBase64String((byte[])Invoke(t,"DeCompress",null,zipped,8));});
  Test("gzip.invalid-input",()=>Invoke(t,"DeCompress",null,new byte[]{1,2,3},10));
 }
 static void Contracts() {
  var a=Library("IRemote.dll");
  foreach(string name in new[]{"RegisterStatus","SubmitStatus"})Test("contract.enum."+name,()=>{var t=a.GetType("IRemote."+name);return Enum.GetNames(t).Select(x=>x+"="+Convert.ToInt32(Enum.Parse(t,x))).ToArray();});
  if(build=="EOS")return;
  var h=a.GetType("IRemote.SubmitHistory");
  Test("history.compare",()=>{var early=Activator.CreateInstance(h);var late=Activator.CreateInstance(h);Set(early,"Time",new DateTime(2020,1,1));Set(late,"Time",new DateTime(2020,1,2));return new[]{Invoke(h,"CompareTo",early,late),Invoke(h,"CompareTo",late,early),Invoke(h,"CompareTo",early,early),Invoke(h,"CompareTo",early,new object[]{null})};});
 }
 static void PracticalHelpers() {
  var a=Library("GradeLib.dll");var tc=a.GetType("GradeLib.TestCase");
  Test("practical.testcase-default",()=>Snapshot(Activator.CreateInstance(tc)));
  Test("practical.validate",()=>{var x=Activator.CreateInstance(tc);var values=new List<bool>{(bool)Invoke(tc,"Validate",x)};((IList)Get(x,"Input")).Add("sample");values.Add((bool)Invoke(tc,"Validate",x));Set(x,"Mark",1.0f);values.Add((bool)Invoke(tc,"Validate",x));return values;});
  string flat=F("testcases");Directory.CreateDirectory(flat);File.WriteAllText(Path.Combine(flat,"tc01.txt"),"INPUT:\nfixture\nOUTPUT:\nexpected\nREMOVE_SPACES:\nYES\nCASE_SENSITIVE:\nNO\nMARK:\n2.5\n");
  Test("practical.parse-flat",()=>Snapshot(Invoke(tc,"LoadTestCase",null,flat)));
  Test("practical.parse-private",()=>Snapshot(Invoke(tc,"File2TestCase",null,Path.Combine(flat,"tc01.txt"))));
  if(build=="PEA-PRN") {
   string nested=F("nested");Directory.CreateDirectory(Path.Combine(nested,"tc1"));File.Copy(Path.Combine(flat,"tc01.txt"),Path.Combine(nested,"tc1","tc01.txt"),true);
   Test("practical.parse-nested",()=>Snapshot(Invoke(tc,"LoadTestCase",null,nested)));
   Test("practical.detect-missing",()=>Invoke(tc,"DetectTestCasesContainTCFolder",null,F("absent")));
   Test("practical.zip-bytes-missing",()=>Invoke(tc,"ConvertZipToByteArray",null,F("absent.zip")));
  } else Test("practical.parse-csd-directory",()=>Snapshot(Invoke(tc,"File2TestCaseCSD",null,flat,1)));
  var zip=a.GetType("GradeLib.SolutionZip");
  Test("zip.missing",()=>Invoke(zip,"CreateZip",null,F("absent"),F("missing.zip")));
  Directory.CreateDirectory(F("empty"));Test("zip.empty",()=>Invoke(zip,"CreateZip",null,F("empty"),F("empty.zip")));
  Directory.CreateDirectory(F("solution"));File.WriteAllText(Path.Combine(F("solution"),"main.txt"),"fixture solution");
  Test("zip.valid-and-extract",()=>{var bytes=(byte[])Invoke(zip,"CreateZip",null,F("solution"),F("solution.zip"));Directory.CreateDirectory(F("extracted"));var extract=a.GetType("GradeLib.GivenExtract");object[] args={F("solution.zip"),F("extracted"),""};bool ok=(bool)Invoke(extract,"Extract",null,args);return new{has_zip=bytes.Length>0,extracted=ok,text=File.ReadAllText(Path.Combine(F("extracted"),"main.txt")),log=args[2]};});
 }
 static void QuestionHelpers() {
  var a=Library("QuestionLib.dll");var q=a.GetType("QuestionLib.QuestionHelper");
  foreach(string s in new[]{"", "  one   two  ", "a\tb\nc", "12345678901234567890"}) {
   string input=s;foreach(string name in new[]{"RemoveSpaces","RemoveAllSpaces"}){string method=name;Test("question."+method+"."+s,()=>Invoke(q,method,null,input));}
   Test("question.wrap."+s,()=>Invoke(q,"WordWrap",null,input,8));
  }
  foreach(int sec in new[]{-1,0,59,60,3599,3600,86399}){int n=sec;Test("question.time."+sec,()=>Invoke(q,"Sec2TimeString",null,n));}
  Test("question.fill-blank",()=>Snapshot(Invoke(q,"GetFillBlankWord",null,"A (one) B (two;three) C")));
  Test("question.prepare-submit",()=>{var t=a.GetType("QuestionLib.Entity.Question");var qa=a.GetType("QuestionLib.Entity.QuestionAnswer");var obj=Activator.CreateInstance(t);Set(obj,"Text","Question fixture");Set(obj,"CourseId","LAB");Set(obj,"ImageData",new byte[]{1,2});Set(obj,"ImageSize",2);var ans=Activator.CreateInstance(qa);Set(ans,"Text","Option fixture");((IList)Get(obj,"QuestionAnswers")).Add(ans);Invoke(t,"Preapare2Submit",obj);return new{text=Get(obj,"Text"),course=Get(obj,"CourseId"),image=Get(obj,"ImageData"),imageSize=Get(obj,"ImageSize"),answer=Get(ans,"Text")};});
  Test("question.local-cache",()=>{var paper=Activator.CreateInstance(a.GetType("QuestionLib.Paper"));Set(paper,"ExamCode","LAB");var t=a.GetType("QuestionLib.SubmitPaper");var submit=Activator.CreateInstance(t);Set(submit,"LoginId","Student");Set(submit,"SPaper",paper);Directory.CreateDirectory(F("cache"));Invoke(q,"SaveSubmitPaper",null,F("cache"),submit);var path=Path.Combine(F("cache"),"student.dat");var first=Invoke(q,"LoadSubmitPaper",null,path);Set(submit,"TimeLeft",42);Invoke(q,"SaveSubmitPaper",null,F("cache"),submit);File.WriteAllText(path,"corrupt fixture");var recovered=Invoke(q,"LoadSubmitPaper",null,path);return new{first_login=Get(first,"LoginId"),has_backup=File.Exists(path+".bak"),recovered_login=Get(recovered,"LoginId"),recovered_time=Get(recovered,"TimeLeft")};});
 }
 sealed class CustomHash {public int Calls;public override int GetHashCode(){Calls++;return 12345;}}
 static void IdentityHash() {
  var t=Library("HashCodeProvider.dll").GetType("HashCodeProvider.IdentityHashCodeProvider");var provider=(IHashCodeProvider)Activator.CreateInstance(t);
  Test("identity.virtual-dispatch",()=>{var obj=new CustomHash();int first=provider.GetHashCode(obj);int second=provider.GetHashCode(obj);return new{override_calls=obj.Calls,stable=first==second,is_custom_hash=first==12345};});
  Test("identity.null",()=>provider.GetHashCode(null));
 }
}
