using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using System.IO;

public class InputFileBtn : MonoBehaviour
{

	void Start ()
	{

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
			FocusStacking fs = new FocusStacking(filePaths);
			fs.StartFocusStacking(filePaths);
    }

  }
}
