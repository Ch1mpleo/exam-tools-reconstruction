using dnlib.DotNet;
using dnlib.DotNet.Emit;

// Narrow partial evaluator: specialize only the exact seven-instruction Confuser
// xor/dup/stloc/rem.un/switch dispatcher. No calls or application operations run.
internal static class FlowCleaner {
record Dispatch(Instruction Start, Local Local, int Xor, Instruction[] Targets);
record State(int Index, int?[] Locals, List<int?> Stack);
public static int Clean(MethodDef method) {
 if (!method.HasBody) return 0;
 var body=method.Body; body.SimplifyMacros(method.Parameters);
 var code=body.Instructions.ToArray();
 var indices=code.Select((i,n)=>(i,n)).ToDictionary(x=>x.i,x=>x.n);
 var dispatches=new Dictionary<Instruction,Dispatch>();
 for(int i=0;i+6<code.Length;i++) {
  if(code[i].IsLdcI4() && code[i+1].OpCode.Code==Code.Xor && code[i+2].OpCode.Code==Code.Dup &&
     code[i+3].OpCode.Code==Code.Stloc && code[i+4].IsLdcI4() && code[i+5].OpCode.Code==Code.Rem_Un &&
     code[i+6].OpCode.Code==Code.Switch && code[i+6].Operand is IList<Instruction> targets && code[i+4].GetLdcI4Value()==targets.Count)
   dispatches[code[i]]=new(code[i],(Local)code[i+3].Operand,code[i].GetLdcI4Value(),targets.ToArray());
 }
 if(dispatches.Count==0) return 0;
 var tracked=dispatches.Values.Select(x=>x.Local.Index).ToHashSet();
 var queue=new Queue<State>(); var seen=new HashSet<string>();
 queue.Enqueue(new(0,new int?[body.Variables.Count],new()));
 foreach(var eh in body.ExceptionHandlers) {
  if(eh.HandlerStart!=null) queue.Enqueue(new(indices[eh.HandlerStart],new int?[body.Variables.Count],eh.HandlerType==ExceptionHandlerType.Catch?new(){null}:new()));
  if(eh.FilterStart!=null) queue.Enqueue(new(indices[eh.FilterStart],new int?[body.Variables.Count],new(){null}));
 }
 var branchSeeds=new Dictionary<Instruction,HashSet<int?>>();
 var branchStates=new Dictionary<Instruction,List<State>>();
 var steps=0;
 while(queue.TryDequeue(out var state)) {
  if(++steps>300000) return 0; // Fail closed: no partial rewrite on state explosion.
  if(state.Index<0 || state.Index>=code.Length || state.Stack.Count>body.MaxStack+16) continue;
  var key=state.Index+":"+string.Join(',',tracked.Select(i=>state.Locals[i]?.ToString()??"?"))+":"+string.Join(',',state.Stack.Select(x=>x?.ToString()??"?"));
  if(!seen.Add(key)) continue;
  var ins=code[state.Index]; var op=ins.OpCode.Code;
  var stack=new List<int?>(state.Stack); var locals=(int?[])state.Locals.Clone();
  int? Pop() { if(stack.Count==0) return null; var x=stack[^1]; stack.RemoveAt(stack.Count-1); return x; }
  void Go(int index) => queue.Enqueue(new(index,(int?[])locals.Clone(),new(stack)));
  if(ins.IsLdcI4()) stack.Add(ins.GetLdcI4Value());
  else if(op==Code.Ldloc) stack.Add(tracked.Contains(((Local)ins.Operand).Index)?locals[((Local)ins.Operand).Index]:null);
  else if(op==Code.Stloc) { var v=Pop(); if(tracked.Contains(((Local)ins.Operand).Index)) locals[((Local)ins.Operand).Index]=v; }
  else if(op==Code.Dup) stack.Add(stack.Count>0?stack[^1]:null);
  else if(op==Code.Pop) Pop();
  else if(op is Code.Xor or Code.Mul or Code.Add or Code.Sub or Code.And or Code.Or or Code.Rem_Un or Code.Shl or Code.Shr_Un) {
   var b=Pop(); var a=Pop(); int? v=null;
   if(a.HasValue && b.HasValue) unchecked { v=op switch {
    Code.Xor=>a.Value^b.Value, Code.Mul=>a.Value*b.Value, Code.Add=>a.Value+b.Value, Code.Sub=>a.Value-b.Value,
    Code.And=>a.Value&b.Value, Code.Or=>a.Value|b.Value, Code.Rem_Un=>b.Value!=0?(int)((uint)a.Value%(uint)b.Value):null,
    Code.Shl=>a.Value<<(b.Value&31), Code.Shr_Un=>(int)((uint)a.Value>>(b.Value&31)), _=>null}; }
   stack.Add(v);
  }
  else if(op is Code.Br or Code.Br_S) {
   if(!branchStates.TryGetValue(ins,out var snapshots)) branchStates[ins]=snapshots=new();
   snapshots.Add(new(indices[(Instruction)ins.Operand],(int?[])locals.Clone(),new(stack)));
   if(dispatches.ContainsKey((Instruction)ins.Operand)) {
    if(!branchSeeds.TryGetValue(ins,out var seeds)) branchSeeds[ins]=seeds=new();
    seeds.Add(stack.Count>0?stack[^1]:null);
   }
   Go(indices[(Instruction)ins.Operand]); continue;
  }
  else if(op==Code.Switch) {
   var v=Pop(); var targets=(IList<Instruction>)ins.Operand;
   if(v.HasValue) { if((uint)v.Value<targets.Count) Go(indices[targets[v.Value]]); else Go(state.Index+1); }
   else {foreach(var t in targets) Go(indices[t]); Go(state.Index+1);} continue;
  }
  else if(op is Code.Leave or Code.Leave_S) { stack.Clear(); Go(indices[(Instruction)ins.Operand]); continue; }
  else {
   ins.CalculateStackUsage(out var pushes,out var pops);
   if(pops<0) stack.Clear(); else for(int i=0;i<pops;i++) Pop();
   for(int i=0;i<pushes;i++) stack.Add(null);
   if(ins.OpCode.FlowControl==FlowControl.Cond_Branch) { Go(indices[(Instruction)ins.Operand]); Go(state.Index+1); continue; }
   if(ins.OpCode.FlowControl is FlowControl.Return or FlowControl.Throw || op is Code.Endfinally or Code.Endfilter) continue;
  }
  Go(state.Index+1);
 }
 int changed=0;
 // Thread predecessor edges through a pure merge/arithmetic tail. This handles
 // ternary seed choices without confusing the application's unknown condition.
 foreach(var pair in branchStates) {
  var outcomes=new List<(Instruction Target,int Pops,Dictionary<int,int> Writes)>();
  foreach(var snapshot in pair.Value) {
   var stack=new List<int?>(snapshot.Stack); var loc=(int?[])snapshot.Locals.Clone();
   int pc=snapshot.Index; var writes=new Dictionary<int,int>(); bool ok=false;
   int? Take() {if(stack.Count==0)return null;var x=stack[^1];stack.RemoveAt(stack.Count-1);return x;}
   for(int budget=0;budget<50 && pc<code.Length;budget++) {
    var i=code[pc]; var c=i.OpCode.Code;
    if(dispatches.TryGetValue(i,out var d)) {
     var seed=Take(); if(!seed.HasValue)break;
     int value=seed.Value^d.Xor; writes[d.Local.Index]=value;
     int pops=snapshot.Stack.Count-stack.Count;
     // The tail must consume a stack suffix and preserve every lower value.
     if(pops<0 || !stack.SequenceEqual(snapshot.Stack.Take(stack.Count)))break;
     outcomes.Add((d.Targets[(uint)value%(uint)d.Targets.Length],pops,writes));ok=true;break;
    }
    if(i.IsLdcI4()) stack.Add(i.GetLdcI4Value());
    else if(c==Code.Nop) {}
    else if(c==Code.Dup) stack.Add(stack.Count>0?stack[^1]:null);
    else if(c==Code.Pop)Take();
    else if(c==Code.Ldloc && tracked.Contains(((Local)i.Operand).Index))stack.Add(loc[((Local)i.Operand).Index]);
    else if(c==Code.Stloc && tracked.Contains(((Local)i.Operand).Index)) {
     var v=Take();if(!v.HasValue)break;int n=((Local)i.Operand).Index;loc[n]=v;writes[n]=v.Value;
    }
    else if(c is Code.Xor or Code.Mul or Code.Add or Code.Sub) {
     var b=Take();var a=Take();if(!a.HasValue||!b.HasValue)break;
     unchecked {stack.Add(c switch {Code.Xor=>a.Value^b.Value,Code.Mul=>a.Value*b.Value,Code.Add=>a.Value+b.Value,_=>a.Value-b.Value});}
    }
    else if(c is Code.Br or Code.Br_S) {pc=indices[(Instruction)i.Operand];continue;}
    else break;
    pc++;
   }
   if(!ok){outcomes.Clear();break;}
  }
  if(outcomes.Count==0)continue;
  var first=outcomes[0];
  if(outcomes.Any(x=>x.Target!=first.Target||x.Pops!=first.Pops||!x.Writes.OrderBy(y=>y.Key).SequenceEqual(first.Writes.OrderBy(y=>y.Key))))continue;
  int at=body.Instructions.IndexOf(pair.Key);pair.Key.OpCode=OpCodes.Nop;pair.Key.Operand=null;
  for(int n=0;n<first.Pops;n++)body.Instructions.Insert(++at,Instruction.Create(OpCodes.Pop));
  foreach(var w in first.Writes) {body.Instructions.Insert(++at,Instruction.CreateLdcI4(w.Value));body.Instructions.Insert(++at,Instruction.Create(OpCodes.Stloc,body.Variables[w.Key]));}
  body.Instructions.Insert(++at,Instruction.Create(OpCodes.Br,first.Target));changed++;
 }
 foreach(var pair in branchSeeds) {
  if(pair.Key.OpCode.Code==Code.Nop)continue;
  if(pair.Value.Count!=1 || !pair.Value.First().HasValue) continue;
  var d=dispatches[(Instruction)pair.Key.Operand!];
  var value=pair.Value.First()!.Value ^ d.Xor;
  var target=d.Targets[(uint)value%(uint)d.Targets.Length];
  int at=body.Instructions.IndexOf(pair.Key);
  pair.Key.OpCode=OpCodes.Pop; pair.Key.Operand=null;
  body.Instructions.Insert(++at,Instruction.CreateLdcI4(value));
  body.Instructions.Insert(++at,Instruction.Create(OpCodes.Stloc,d.Local));
  body.Instructions.Insert(++at,Instruction.Create(OpCodes.Br,target));
  changed++;
 }
 // Entry fallthrough: constant seed directly before a recognized dispatcher.
 foreach(var d in dispatches.Values) {
  int at=body.Instructions.IndexOf(d.Start);
  if(at==0 || !body.Instructions[at-1].IsLdcI4()) continue;
  var seed=body.Instructions[at-1]; var value=seed.GetLdcI4Value()^d.Xor;
  seed.OpCode=OpCodes.Ldc_I4; seed.Operand=value;
  body.Instructions.Insert(at++,Instruction.Create(OpCodes.Stloc,d.Local));
  body.Instructions.Insert(at,Instruction.Create(OpCodes.Br,d.Targets[(uint)value%(uint)d.Targets.Length]));
  changed++;
 }
 body.KeepOldMaxStack=true;
 return changed;
}
}
