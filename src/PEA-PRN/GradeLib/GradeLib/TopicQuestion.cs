using System;
using System.Collections.Generic;

namespace GradeLib;

[Serializable]
public class TopicQuestion
{
	public int TopicCode { get; set; }

	public List<Question> listQuestion { get; set; }

	public TopicQuestion()
	{
		listQuestion = new List<Question>();
	}
}
