namespace GradeLib;

public class Exam
{
	public string ExamCode { get; set; }

	public string ExamName { get; set; }

	public string Language { get; set; }

	public string Description { get; set; }

	public Exam()
	{
	}

	public Exam(string code, string name, string language, string desc)
	{
		ExamCode = code;
		ExamName = name;
		Language = language;
		Description = desc;
	}
}
