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
      var FileDlg = new OpenFileDialog();
      FileDlg.FileName = "FocusStackImage.JPG";
      FileDlg.InitialDirectory = Dlg.SelectedPath;
      FileDlg.Filter = "JPEGファイル(*.jpeg;*.JPG;*.JPEG)|*.jpeg;*.JPG;*.JPEG|pngファイル(*.png)|*.png";
      FileDlg.Title = "保存先を選択して下さい";
      FileDlg.CheckFileExists = false;

      if (FileDlg.ShowDialog() == DialogResult.OK)
        m_fs.StartFocusStacking( filePaths, CreateExtensionJPGorPNG(FileDlg.FileName) );

     }
  }


  private string CreateExtensionJPGorPNG(string filePath)
  {
    string path = new string( filePath.ToCharArray() );

    if ( path.EndsWith(".jpeg") 
      || path.EndsWith(".JPG")  
      || path.EndsWith(".JPEG") 
      || path.EndsWith(".png")
      )  return path;
    else return path + ".png";
  }

}
