using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kitware.VTK;

public class SvrVTKReader : SvrReader
{
    public vtkStructuredPoints volume;


    public override void Read()
    {
        vtkStructuredPointsReader reader = vtkStructuredPointsReader.New();
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
