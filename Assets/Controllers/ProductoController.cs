using Assets.Model;
using Dummiesman;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ProductoController : MonoBehaviour
{
    public Producto Producto = null;
    private GameObject ProductoObj;

    /*public void SetMesh(Mesh mesh, Material[] materials)
    {
        transform.GetComponent<MeshFilter>().mesh = mesh;
        if (materials.Length == 1)
            transform.GetComponent<MeshRenderer>().material = materials[0];
        else
            for (int i = 0; i < materials.Length; i++)
                transform.GetComponent<MeshRenderer>().materials[i] = materials[i];
    }*/
    public void ClearProducto()
    {
        try
        {
            this.Producto = null;
            Destroy(ProductoObj);
        }
        catch (Exception) { ; }
    }
    public void SetProducto(Producto producto)
    {
        Producto = producto;

        try
        {
            Destroy(ProductoObj);
        }
        catch (Exception) { ; }

        try
        {
            if (producto.obj != null)
            {
                if (producto.mtl != null)
                    ProductoObj = new OBJLoader().Load(producto.obj, producto.mtl);
                else
                    ProductoObj = new OBJLoader().Load(producto.obj);

                ProductoObj.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
                ProductoObj.transform.localScale = new Vector3(producto.scale, producto.scale, producto.scale);

                var mesh = ProductoObj.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                var collider = ProductoObj.AddComponent<MeshCollider>();
                collider.convex = true;
                collider.sharedMesh = mesh;
                var interactable = ProductoObj.AddComponent<XRGrabInteractable>();

                //interactable.interactionManager = interactionManager;
                //interactable.colliders.Add(collider);
                //interactionManager.RegisterInteractable((IXRInteractable)interactable);
                //boxCollider.size = new Vector3(producto.scale, producto.scale, producto.scale);
            } else
                Debug.LogWarning("Producto "+producto.titulo+" has missing 3D assets");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }
}
