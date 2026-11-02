using System.Windows.Forms;

namespace IRemote;

public interface IExamclient
{
	void SetExamData(PEAData peaData);

	void SetAuthenForm(Form f);
}
