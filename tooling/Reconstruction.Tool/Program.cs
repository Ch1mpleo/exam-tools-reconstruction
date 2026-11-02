if(args.Length>0 && args[0]=="audit") StaticAudit.Run(args[1]);
else if(args.Length>0 && args[0]=="login-audit") LoginTraceAudit.Run(args[1]);
else Analyzer.Run(args);
