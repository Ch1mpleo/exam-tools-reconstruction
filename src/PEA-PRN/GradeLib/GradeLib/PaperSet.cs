using System;
using System.Collections.Generic;

namespace GradeLib;

[Serializable]
public class PaperSet
{
	public bool LockClient;

	public int TimeAllow = 0;

	public string TestName { get; set; }

	public string Language { get; set; }

	public List<TopicQuestion> listTopicQuestion { get; set; }

	public List<Paper> listPaper { get; set; }

	public List<PaperJPG> listPaperImage { get; set; }

	public PaperSet()
	{
		listTopicQuestion = new List<TopicQuestion>();
		listPaper = new List<Paper>();
		listPaperImage = new List<PaperJPG>();
	}
}
