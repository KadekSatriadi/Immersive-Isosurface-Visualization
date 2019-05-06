using Kitware.VTK;
using System.IO;

public class SvrDICOMSeriesReader : SvrReader {
    public vtkImageData volume;
    
    public override void Read()
    {
        if (File.Exists(filepath))
        {
            filepath = new FileInfo(filepath).DirectoryName;
        }
        vtkDICOMImageReader reader = vtkDICOMImageReader.New();
        reader.SetDirectoryName(filepath);
        reader.Update();
        volume = reader.GetOutput();
        print(volume.ToString());
        range[0] = volume.GetPointData().GetArray(0).GetRange()[0];
        range[1] = volume.GetPointData().GetArray(0).GetRange()[1];
        for (int i = 0; i < volume.GetPointData().GetNumberOfArrays(); i++)
        {
            scalarNames.Add(volume.GetPointData().GetArray(i).GetName());
        }
    }

    public override vtkDataObject GetOutput()
    {
        return volume;
    }

}
