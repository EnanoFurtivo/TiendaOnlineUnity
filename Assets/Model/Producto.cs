using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model
{
    [Serializable]
    public class Producto
    {
        public int id { get; set; }
        public string sku { get; set; }
        public string titulo { get; set; }
        public double precio { get; set; }
        public int stock { get; set; }
        public string preview_path { get; set; }
        public float scale { get; set; }
        public string obj_path { get; set; }
        public string mtl_path { get; set; }
        public Sprite preview { get; set; }
        public Stream obj { get { return ApiController.FetchStream(this.obj_path); } } 
        public Stream mtl { get { return ApiController.FetchStream(this.mtl_path); } }

    }
}
