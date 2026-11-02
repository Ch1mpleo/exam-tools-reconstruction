using System;
using System.Collections;
using NHibernate;
using QuestionLib.Business;

namespace QuestionLib.Entity;

[Serializable]
public class Passage
{
	private int _pid;

	private string _courseId;

	private int _chapterId;

	private string _text;

	private ArrayList _passageQuestions;

	private int _QBID;

	public int QBID
	{
		get
		{
			return _QBID;
		}
		set
		{
			_QBID = value;
		}
	}

	public int PID
	{
		get
		{
			return _pid;
		}
		set
		{
			_pid = value;
		}
	}

	public string CourseId
	{
		get
		{
			return _courseId;
		}
		set
		{
			_courseId = value;
		}
	}

	public int ChapterId
	{
		get
		{
			return _chapterId;
		}
		set
		{
			_chapterId = value;
		}
	}

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
		}
	}

	public ArrayList PassageQuestions
	{
		get
		{
			return _passageQuestions;
		}
		set
		{
			_passageQuestions = value;
		}
	}

	public Passage()
	{
		_passageQuestions = new ArrayList();
	}

	// [behavior 0x06000133] Formats the numeric passage identifier as a string.
	public override string ToString()
	{
		return _pid.ToString();
	}

	// [behavior 0x06000134] Uses BOQuestion.LoadPassageQuestion for this PID and replaces the PassageQuestions collection
	// [behavior 0x06000134] with the query result.
	public void LoadQuestions(ISessionFactory sessionFactory)
	{
		BOQuestion bOQuestion = new BOQuestion(sessionFactory);
		_passageQuestions = (ArrayList)bOQuestion.LoadPassageQuestion(_pid);
	}

	// [behavior 0x06000135] Clears passage text/course ID, then calls Preapare2Submit on every contained question. It
	// [behavior 0x06000135] mutates the same object graph.
	public void Preapare2Submit()
	{
		Text = null;
		CourseId = null;
		foreach (Question passageQuestion in PassageQuestions)
		{
			passageQuestion.Preapare2Submit();
		}
	}
}
