using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kitware.VTK;

public class SvrVTIReader : SvrReader
{
    public vtkImageData volume;


    public override void Read()
    {
        vtkXMLImageDataReader reader = vtkXMLImageDataReader.New();
        reader.SetFileName(filepath);
        reader.Update();
        volume = reader.GetOutput();
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
