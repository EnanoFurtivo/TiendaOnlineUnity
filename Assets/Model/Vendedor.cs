using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model
{
    [Serializable]
    public class Vendedor
    {
        public int id { get; set; }
        public string username { get; set; }
        public string mail { get; set; }
        public string telefono { get; set; }
        public string preview_path { get; set; }
        public Sprite preview { get; set; }

        private List<Producto> productos;

        public static List<Vendedor> GetAllVendedores()
        {
            return ApiController.FetchVendedores();
        }

        public void SetProductos(List<Producto> productos)
        {
            this.productos = productos;
        }
        public List<Producto> GetProductos()
        {
            if (productos == null)
                this.productos = ApiController.FetchProductos(this);
            
            if (productos == null)
                this.productos = new List<Producto>();

            return productos;
        }
        public Producto GetProducto(int id)
        {
            return GetProductos().Find(p => p.id == id);
        }
    }
}
