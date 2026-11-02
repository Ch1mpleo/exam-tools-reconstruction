using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using QuestionLib.Entity;

namespace QuestionLib;

public class QuestionHelper
{
	public static char[] lo_deli = new char[1] { ';' };

	private static readonly Dictionary<string, object> _locks = new Dictionary<string, object>();

	public static string[] MultipleChoiceQuestionType = new string[2] { "Grammar", "Indicate Mistake" };

	public static string fillBlank_Pattern = "\\([0-9a-zA-Z;:=?<>/`,'’ .+_~!@#$%^&*\\r\\n-]+\\)";

	private static char[] splitChars = new char[3] { ' ', '-', '\t' };

	public static int LineWidth = 100;

	// [behavior 0x060000a2] Validates the model; locks by normalized login; serializes to .tmp; moves or atomically replaces
	// [behavior 0x060000a2] the main .dat and retains .bak; removes a leftover temporary file. Exceptions inside the write
	// [behavior 0x060000a2] block are swallowed.
	public static void SaveSubmitPaper(string folder, SubmitPaper submitPaper)
	{
		if (!IsValidSubmitPaper(submitPaper))
		{
			return;
		}
		string text = Path.Combine(folder, submitPaper.LoginId.Trim().ToLower() + ".dat");
		string text2 = text + ".tmp";
		string destinationBackupFileName = text + ".bak";
		lock (GetLock(submitPaper.LoginId.Trim().ToLower()))
		{
			try
			{
				using (FileStream serializationStream = new FileStream(text2, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(serializationStream, submitPaper);
				}
				if (File.Exists(text))
				{
					File.Replace(text2, text, destinationBackupFileName);
				}
				else
				{
					File.Move(text2, text);
				}
			}
			catch
			{
			}
			finally
			{
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
			}
		}
	}

	// [behavior 0x060000a3] Under a shared dictionary lock, creates or returns the per-login lock object used to serialize
	// [behavior 0x060000a3] saves.
	private static object GetLock(string key)
	{
		lock (_locks)
		{
			if (!_locks.ContainsKey(key))
			{
				_locks[key] = new object();
			}
			return _locks[key];
		}
	}

	// [behavior 0x060000a4] Rejects a missing/blank primary path. Deserializes and validates the primary model; only a
	// [behavior 0x060000a4] thrown exception leads to a .bak attempt. Returns null if recovery fails.
	public static SubmitPaper LoadSubmitPaper(string savedFile)
	{
		if (string.IsNullOrEmpty(savedFile) || !File.Exists(savedFile))
		{
			return null;
		}
		try
		{
			using FileStream serializationStream = new FileStream(savedFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			SubmitPaper submitPaper = (SubmitPaper)binaryFormatter.Deserialize(serializationStream);
			if (!IsValidSubmitPaper(submitPaper))
			{
				return null;
			}
			return submitPaper;
		}
		catch (Exception)
		{
			string path = savedFile + ".bak";
			if (File.Exists(path))
			{
				try
				{
					using FileStream serializationStream2 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
					BinaryFormatter binaryFormatter2 = new BinaryFormatter();
					SubmitPaper submitPaper2 = (SubmitPaper)binaryFormatter2.Deserialize(serializationStream2);
					if (IsValidSubmitPaper(submitPaper2))
					{
						return submitPaper2;
					}
				}
				catch
				{
				}
			}
			return null;
		}
	}

	// [behavior 0x060000a5] Requires a nonnull SubmitPaper, a nonempty LoginId and a nonnull SPaper.
	private static bool IsValidSubmitPaper(SubmitPaper sp)
	{
		if (sp == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(sp.LoginId))
		{
			return false;
		}
		if (sp.SPaper == null)
		{
			return false;
		}
		return true;
	}

	// [behavior 0x060000a6] Searches the original Paper reading passages by PID; returns the matching passage or null.
	private static Passage GetPassage(Paper oPaper, int pid)
	{
		foreach (Passage readingQuestion in oPaper.ReadingQuestions)
		{
			if (readingQuestion.PID == pid)
			{
				return readingQuestion;
			}
		}
		return null;
	}

	// [behavior 0x060000a7] Requires matching QID; copies original metadata, question text/image and mark. Matches answers
	// [behavior 0x060000a7] by QAID and handles fill-blank normalized text separately from other answer types.
	private static bool ReConstructQuestion(Question sq, Question oq)
	{
		if (sq.QID == oq.QID)
		{
			sq.QBID = oq.QBID;
			sq.CourseId = oq.CourseId;
			sq.Text = oq.Text;
			sq.Mark = oq.Mark;
			sq.ImageData = oq.ImageData;
			sq.ImageSize = oq.ImageSize;
			bool flag = false;
			if (sq.QType == QuestionType.FILL_BLANK_ALL)
			{
				flag = true;
			}
			if (sq.QType == QuestionType.FILL_BLANK_GROUP)
			{
				flag = true;
			}
			if (sq.QType == QuestionType.FILL_BLANK_EMPTY)
			{
				flag = true;
			}
			foreach (QuestionAnswer questionAnswer3 in sq.QuestionAnswers)
			{
				foreach (QuestionAnswer questionAnswer4 in oq.QuestionAnswers)
				{
					if (questionAnswer3.QAID != questionAnswer4.QAID)
					{
						continue;
					}
					if (flag)
					{
						string text = RemoveSpaces(questionAnswer3.Text).Trim().ToLower();
						string value = RemoveSpaces(questionAnswer4.Text).Trim().ToLower();
						if (text.Equals(value))
						{
							questionAnswer3.Chosen = true;
							questionAnswer3.Selected = true;
						}
					}
					else
					{
						questionAnswer3.Text = questionAnswer4.Text;
						questionAnswer3.Chosen = questionAnswer4.Chosen;
					}
					break;
				}
			}
			return true;
		}
		return false;
	}

	// [behavior 0x060000a8] For matching essay IDs, restores original question-bank/course identifiers and question text.
	private static void ReConstructEssay(EssayQuestion sEssay, EssayQuestion oEssay)
	{
		if (sEssay.EQID == oEssay.EQID)
		{
			sEssay.QBID = oEssay.QBID;
			sEssay.CourseId = oEssay.CourseId;
			sEssay.Question = oEssay.Question;
		}
	}

	// [behavior 0x060000a9] Assigns the original image data to the submitted image-paper model.
	private static void ReConstructImagePaper(ImagePaper sIP, ImagePaper oIP)
	{
		sIP.Image = oIP.Image;
	}

	// [behavior 0x060000aa] Walks the submitted paper structure and restores presentation/model data from the original
	// [behavior 0x060000aa] paper, matching identifiers through specialized helpers; also restores image/essay/audio data.
	public static Paper Re_ConstructPaper(Paper oPaper, SubmitPaper submitPaper)
	{
		Paper sPaper = submitPaper.SPaper;
		foreach (Passage readingQuestion in sPaper.ReadingQuestions)
		{
			Passage passage2 = GetPassage(oPaper, readingQuestion.PID);
			readingQuestion.QBID = passage2.QBID;
			readingQuestion.Text = passage2.Text;
			readingQuestion.CourseId = passage2.CourseId;
			foreach (Question passageQuestion in readingQuestion.PassageQuestions)
			{
				foreach (Question passageQuestion2 in passage2.PassageQuestions)
				{
					if (ReConstructQuestion(passageQuestion, passageQuestion2))
					{
						break;
					}
				}
			}
		}
		foreach (MatchQuestion matchQuestion3 in sPaper.MatchQuestions)
		{
			foreach (MatchQuestion matchQuestion4 in oPaper.MatchQuestions)
			{
				if (matchQuestion3.MID == matchQuestion4.MID)
				{
					matchQuestion3.QBID = matchQuestion4.QBID;
					matchQuestion3.CourseId = matchQuestion4.CourseId;
					matchQuestion3.ColumnA = matchQuestion4.ColumnA;
					matchQuestion3.ColumnB = matchQuestion4.ColumnB;
					matchQuestion3.Solution = matchQuestion4.Solution;
					matchQuestion3.Mark = matchQuestion4.Mark;
					break;
				}
			}
		}
		foreach (Question grammarQuestion in sPaper.GrammarQuestions)
		{
			foreach (Question grammarQuestion2 in oPaper.GrammarQuestions)
			{
				if (ReConstructQuestion(grammarQuestion, grammarQuestion2))
				{
					break;
				}
			}
		}
		foreach (Question indicateMQuestion in sPaper.IndicateMQuestions)
		{
			foreach (Question indicateMQuestion2 in oPaper.IndicateMQuestions)
			{
				if (ReConstructQuestion(indicateMQuestion, indicateMQuestion2))
				{
					break;
				}
			}
		}
		foreach (Question fillBlankQuestion in sPaper.FillBlankQuestions)
		{
			foreach (Question fillBlankQuestion2 in oPaper.FillBlankQuestions)
			{
				if (ReConstructQuestion(fillBlankQuestion, fillBlankQuestion2))
				{
					break;
				}
			}
		}
		if (oPaper.EssayQuestion != null)
		{
			ReConstructEssay(sPaper.EssayQuestion, oPaper.EssayQuestion);
		}
		if (oPaper.ImgPaper != null)
		{
			ReConstructImagePaper(sPaper.ImgPaper, oPaper.ImgPaper);
		}
		sPaper.OneSecSilence = oPaper.OneSecSilence;
		sPaper.ListAudio = oPaper.ListAudio;
		return sPaper;
	}

	// [behavior 0x060000ab] Trims the string, repeatedly replaces two ASCII spaces with one until length stops changing, and
	// [behavior 0x060000ab] returns it. Internal tabs/newlines are not removed.
	public static string RemoveSpaces(string s)
	{
		s = s.Trim();
		string text;
		do
		{
			text = s;
			s = s.Replace("  ", " ");
		}
		while (s.Length != text.Length);
		return s;
	}

	// [behavior 0x060000ac] Trims the string and removes ASCII spaces, repeating until length stabilizes. Internal
	// [behavior 0x060000ac] tabs/newlines remain.
	public static string RemoveAllSpaces(string s)
	{
		s = s.Trim();
		string text;
		do
		{
			text = s;
			s = s.Replace(" ", "");
		}
		while (s.Length != text.Length);
		return s;
	}

	// [behavior 0x060000ad] Returns true exactly for FILL_BLANK_ALL, FILL_BLANK_GROUP and FILL_BLANK_EMPTY; false for other
	// [behavior 0x060000ad] enum values.
	public static bool IsFillBlank(QuestionType qt)
	{
		return qt switch
		{
			QuestionType.FILL_BLANK_ALL => true, 
			QuestionType.FILL_BLANK_GROUP => true, 
			QuestionType.FILL_BLANK_EMPTY => true, 
			_ => false, 
		};
	}

	// [behavior 0x060000ae] Removes occurrences of Environment.NewLine, then applies RemoveSpaces. It does not independently
	// [behavior 0x060000ae] recognize every possible newline convention.
	private static string RemoveNewLine(string s)
	{
		s = s.Replace(Environment.NewLine, "");
		s = RemoveSpaces(s);
		return s;
	}

	// [behavior 0x060000af] Protects regex-matched parenthesized blanks with (###), splits platform newlines, tokenizes
	// [behavior 0x060000af] whitespace/hyphens, wraps tokens to width, splits long words with hyphens, then restores
	// [behavior 0x060000af] normalized blank text. The long-token loop can emit double newlines. Width <= 1 is not safely
	// [behavior 0x060000af] validated.
	public static string WordWrap(string str, int width)
	{
		string pattern = fillBlank_Pattern;
		Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		MatchCollection matchCollection = regex.Matches(str);
		str = regex.Replace(str, "(###)");
		string[] array = SplitLines(str);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			string str2 = array[i];
			if (i < array.Length - 1)
			{
				str2 = array[i] + Environment.NewLine;
			}
			ArrayList arrayList = Explode(str2, splitChars);
			int num = 0;
			for (int j = 0; j < arrayList.Count; j++)
			{
				string text = (string)arrayList[j];
				if (num + text.Length > width)
				{
					if (num > 0)
					{
						if (!stringBuilder.ToString().EndsWith(Environment.NewLine))
						{
							stringBuilder.Append(Environment.NewLine);
						}
						num = 0;
					}
					while (text.Length > width)
					{
						stringBuilder.Append(text.Substring(0, width - 1) + "-");
						text = text.Substring(width - 1);
						if (!stringBuilder.ToString().EndsWith(Environment.NewLine))
						{
							stringBuilder.Append(Environment.NewLine);
						}
						stringBuilder.Append(Environment.NewLine);
					}
					text = text.TrimStart();
				}
				stringBuilder.Append(text);
				num += text.Length;
			}
		}
		str = stringBuilder.ToString();
		pattern = "\\(###\\)";
		regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		for (int k = 0; k < matchCollection.Count; k++)
		{
			string replacement = RemoveNewLine(matchCollection[k].Value);
			str = regex.Replace(str, replacement, 1);
		}
		return str;
	}

	// [behavior 0x060000b0] Tokenizes around supplied separator characters. Whitespace becomes a separate token; non-
	// [behavior 0x060000b0] whitespace separators stay attached to the preceding text. Appends the remaining suffix,
	// [behavior 0x060000b0] including an empty suffix.
	private static ArrayList Explode(string str, char[] splitChars)
	{
		ArrayList arrayList = new ArrayList();
		int num = 0;
		while (true)
		{
			int num2 = str.IndexOfAny(splitChars, num);
			if (num2 == -1)
			{
				break;
			}
			string text = str.Substring(num, num2 - num);
			char c = str.Substring(num2, 1)[0];
			if (char.IsWhiteSpace(c))
			{
				arrayList.Add(text);
				arrayList.Add(c.ToString());
			}
			else
			{
				arrayList.Add(text + c);
			}
			num = num2 + 1;
		}
		arrayList.Add(str.Substring(num));
		return arrayList;
	}

	// [behavior 0x060000b1] Constructs a compiled regex from Environment.NewLine and splits the input on that platform
	// [behavior 0x060000b1] newline sequence.
	private static string[] SplitLines(string str)
	{
		string newLine = Environment.NewLine;
		Regex regex = new Regex(newLine, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		return regex.Split(str);
	}

	// [behavior 0x060000b2] Finds matches of the configured parenthesized-blank regex, removes each outer parenthesis pair
	// [behavior 0x060000b2] and returns the inner strings in match order. It does not split semicolon alternatives.
	public static string[] GetFillBlankWord(string text)
	{
		string pattern = fillBlank_Pattern;
		Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		MatchCollection matchCollection = regex.Matches(text);
		string[] array = new string[matchCollection.Count];
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string text2 = matchCollection[i].Value.Remove(0, 1);
			text2 = text2.Remove(text2.Length - 1, 1);
			array[i] = text2;
		}
		return array;
	}

	// [behavior 0x060000b3] Computes hours, minutes and seconds using integer division/remainder, prefixes zero and keeps
	// [behavior 0x060000b3] the last two characters of each component. Hours above 99 truncate; negative seconds are not
	// [behavior 0x060000b3] rejected.
	public static string Sec2TimeString(int sec)
	{
		int num = sec / 3600;
		int num2 = sec % 3600 / 60;
		int num3 = sec % 60;
		string text = "0" + num;
		text = text.Substring(text.Length - 2, 2);
		string text2 = "0" + num2;
		text2 = text2.Substring(text2.Length - 2, 2);
		string text3 = "0" + num3;
		text3 = text3.Substring(text3.Length - 2, 2);
		return text + ":" + text2 + ":" + text3;
	}
}
