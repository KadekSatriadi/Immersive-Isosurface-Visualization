using UnityEngine;
using System.IO;

public class SvrFileInputReaderManager : MonoBehaviour {
    public string filepath;
    public SvrReader VTRReader;
    public SvrReader VTKReader;
    public SvrReader VTIReader;
    public SvrReader DICOMReader;
    
    public SvrReader GetReader()
    {
        if (!File.Exists(filepath) && !Directory.Exists(filepath)) throw new UnityException("filepath does not exists. (" + filepath + ")");

        if(Directory.Exists(filepath))
        {
            return VTRReader;
        }
        else
        {
            FileInfo info = new FileInfo(filepath);
            string type = info.Extension;
            switch (type)
            {
                case ".vtr":
                    return VTRReader;
                    break;
                case ".vti":
                    return  VTIReader;
                    break;
                case ".vtk":
                    return VTKReader;
                    break;
                case ".dcm":
                    return DICOMReader;
                    break;
            }
        }

        throw new UnityException("Unkown data input was specified");
    }
}
