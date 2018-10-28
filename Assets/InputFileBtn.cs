using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System.IO;

public class InputFileBtn : MonoBehaviour
{
	FocusStacking m_fs;
	[SerializeField]
	GameObject empty;

	void Start ()
	{
		m_fs = empty.GetComponent<FocusStacking>();
	}

	void Update ()
	{

	}

	public void OnClick()
	{
		var Dlg = new FolderBrowserDialog();

		if (Dlg.ShowDialog() == DialogResult.OK)
		{
      string[] filePaths = Directory.GetFiles(Dlg.SelectedPath);
			m_fs.StartFocusStacking(filePaths);
    }
  }
}
