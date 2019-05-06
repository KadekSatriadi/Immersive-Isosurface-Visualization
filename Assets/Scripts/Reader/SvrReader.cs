using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kitware.VTK;

public class SvrReader : MonoBehaviour {
    public string filepath;
    public double[] range;
    public List<string> scalarNames;

    void Start()
    {
        range = new double[2];
    }

    public virtual void Read()
    {
    }

    public virtual vtkDataObject GetOutput()
    {
        return vtkDataObject.New();
    }

    public virtual void Reset() { }

}
