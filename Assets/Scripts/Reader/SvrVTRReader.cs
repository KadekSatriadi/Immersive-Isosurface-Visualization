using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kitware.VTK;

public class SvrVTRReader : SvrReader {
    public vtkRectilinearGrid volume;

    public override void Read()
    {
        volume = vtkRectilinearGrid.New();
        vtkXMLRectilinearGridReader reader = vtkXMLRectilinearGridReader.New();
        reader.SetFileName(filepath);
        reader.Update();
        volume.DeepCopy(reader.GetOutput());
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
