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
		var Dlg = new OpenFileDialog();

   // Dlg.Filter = "画像ファイル(*.jpeg;*.JPG;*.JPEG;*.png;*.PNG)|*.jpeg;*.JPG;*.JPEG;*.png;*.PNG";
    Dlg.Title = "画像を選択";
    Dlg.Multiselect = true;

    if (Dlg.ShowDialog() == DialogResult.OK)
		{
      string[] filePaths = Dlg.FileNames;
      var SaveFileDlg = new OpenFileDialog();
      SaveFileDlg.FileName = "FocusStackImage.png";
      SaveFileDlg.Filter = "画像ファイル(*.jpeg;*.JPG;*.JPEG;*.png;*.PNG)|*.jpeg;*.JPG;*.JPEG;*.png;*.PNG";
      SaveFileDlg.Title = "保存先を選択して下さい";
      SaveFileDlg.CheckFileExists = false;

			if (SaveFileDlg.ShowDialog() == DialogResult.OK)
				m_fs.StartFocusStacking( filePaths, CreateExtensionJPGorPNG(SaveFileDlg.FileName) );

     }
  }


  private string CreateExtensionJPGorPNG(string filePath)
  {
    string path = new string( filePath.ToCharArray() );

    if ( path.EndsWith(".jpeg") 
      || path.EndsWith(".JPG")  
      || path.EndsWith(".JPEG") 
      || path.EndsWith(".png")
      || path.EndsWith(".PNG")
      ) return path;
    else return path + ".png";
  }

}
