using System;
using System.Collections.Generic;

namespace GradeLib;

[Serializable]
public class Paper
{
	public string PaperCode { get; set; }

	public List<int> listQuestionCode { get; set; }

	public List<CandidateItem> listCandidate { get; set; }

	public Paper()
	{
		listQuestionCode = new List<int>();
		listCandidate = new List<CandidateItem>();
	}
}
