using Assets.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneController : MonoBehaviour
{
    public GameObject Producto;
    public TMP_Dropdown Dropdown;
    public CanvasRenderer ListProductos;
    public CanvasRenderer ListOrden;
    //public XRInteractionManager InteractionManager;

    public Button ButtonAgregar;
    public Button ButtonEliminar;
    public Button ButtonEnviar;

    public TMP_Text Descripcion;
    public TMP_Text Stock;
    public TMP_Text Titulo;

    private ProductoController ProductoController;
    private DropdownController DropdownController;
    private ListController ProductosController;
    private ListController OrdenController;

    private List<Vendedor> Vendedores;
    private List<Producto> Orden = new List<Producto>();
    private Vendedor VendedorActual;

    void Start()
    {
        //Get Components//
        ProductoController  = Producto.GetComponent<ProductoController>();
        DropdownController  = Dropdown.GetComponent<DropdownController>();
        ProductosController = ListProductos.GetComponent<ListController>();
        OrdenController     = ListOrden.GetComponent<ListController>();

        //Subscribe to events//
        DropdownController.VendedorChanged.AddListener(VendedorChanged);
        ProductosController.ProductoClicked.AddListener(ProductoClicked);
        OrdenController.ProductoClicked.AddListener(ProductoClicked);
        ButtonAgregar.onClick.AddListener(Agregar);
        ButtonEliminar.onClick.AddListener(Eliminar);
        ButtonEnviar.onClick.AddListener(Enviar);

        //Load dropdown//
        Vendedores = ApiController.FetchVendedores();
        DropdownController.InitializeDropdown(Vendedores);
        ProductosController.PopulateList(Vendedores[0].GetProductos());
        VendedorActual = Vendedores[0];
        OrdenController.ClearList();
    }
    private void Agregar()
    {
        Debug.Log("Agregar producto clicked");
        if(ProductoController.Producto != null && ProductoController.Producto.stock > 0)
        {
            Debug.Log("Agregar producto " + ProductoController.Producto.titulo + " ok");
            Orden.Add(ProductoController.Producto);
            ProductoController.Producto.stock--;
            OrdenController.PopulateList(Orden);
            UpdateDescripcion(ProductoController.Producto);
        }
    }
    private void Eliminar()
    {
        Debug.Log("Eliminar producto clicked");
        if (ProductoController.Producto != null)
        {
            if (Orden.Remove(ProductoController.Producto))
            {
                Debug.Log("Eliminar producto " + ProductoController.Producto.titulo + " ok");
                OrdenController.PopulateList(Orden);
                ProductoController.Producto.stock++;
                ProductoController.ClearProducto();
                UpdateDescripcion(null);
            }
        }
    }
    private void Enviar()
    {
        Debug.Log("Enviar orden clicked");
        if (Orden.Count > 0)
        {
            if (ApiController.SendOrden(Orden, VendedorActual))
            {
                Orden = new List<Producto>();
                OrdenController.ClearList();
                ProductoController.ClearProducto();
                UpdateDescripcion(null);
            }
            else
                Debug.LogError("Error al enviar la orden...");
        }
        else
            Debug.Log("Orden vacia");
    }

    private void UpdateDescripcion(Producto producto)
    {
        if (producto == null)
            Descripcion.text = "";
        else
            Descripcion.text = producto.titulo + "\n" + "Stock disponible: " + producto.stock + "\n" + "Precio: " + producto.precio + "$\n";
    }

    private void ProductoClicked(Producto producto)
    {
        Debug.Log("Producto " + producto.titulo + " clicked");
        ProductoController.SetProducto(/*InteractionManager, */producto);
        UpdateDescripcion(producto);
    }
    private void VendedorChanged(Vendedor vendedor)
    {
        Debug.Log("Vendedor " + vendedor.username + " clicked");
        ProductosController.PopulateList(vendedor.GetProductos());
        VendedorActual = vendedor;
    }
}
