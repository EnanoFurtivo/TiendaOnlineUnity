using Assets.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using UnityEngine;

public static class ApiController
{
    public static string ServerAdress       = "http://localhost/TiendaOnline";
    public static string TokenEndpoint      = "/api/usuarios/generar_token";
    public static string VendedoresEndpoint = "/api/vendedores/list";
    public static string ProductosEndpoint  = "/api/productos/list";
    public static string OrdenEndpoint      = "/api/ordenes/add";
    public static Credentials Credentials   = new Credentials();

    public static List<Vendedor> FetchVendedores()
    {
        string url = ServerAdress + VendedoresEndpoint;

        string response = RequestJson(url);
        Debug.Log(response);
        if (response == null)
            return null;

        List<Vendedor> vendedores = JsonConvert.DeserializeObject<List<Vendedor>>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        if (vendedores == null)
            return null;

        foreach (Vendedor v in vendedores)
            v.preview = RequestSprite(v.preview_path, 32, 32);

        return vendedores;
    }
    public static List<Producto> FetchProductos(Vendedor vendedor)
    {
        string url = ServerAdress + ProductosEndpoint + "?id_vendedor=" + vendedor.id;

        string response = RequestJson(url);
        Debug.Log(response);
        if (response == null)
            return null;

        List<Producto> productos = JsonConvert.DeserializeObject<List<Producto>>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        if (productos == null)
            return null;

        foreach (Producto p in productos)
            p.preview = RequestSprite(p.preview_path, 32, 32);

        return productos;
    }

    private static string GetToken()
    {
        if (Credentials.token == "")
            RequestToken();
        return Credentials.token;
    }
    private static void RequestToken()
    {
        string url = ServerAdress + TokenEndpoint;
        var parameters = new NameValueCollection(){
            { "username", Credentials.username },
            { "password", Credentials.password }
        };

        try
        {
            using (WebClient wc = new WebClient())
            {
                var response = wc.UploadValues(url, parameters);
                var responseString = Encoding.Default.GetString(response);

                Debug.Log("resppobnse: " + responseString);

                Credentials cd = JsonConvert.DeserializeObject<Credentials>(responseString);

                Debug.Log("token: " + cd.token);

                if (cd.token == null)
                    throw new Exception();

                Credentials.token = cd.token;
                Credentials.user_id = cd.user_id;

                Debug.Log("Token: " + Credentials.token);
                Debug.Log("User id: " + Credentials.user_id);
            }
        }
        catch
        {
            string parametersStr = "[";
            foreach (string key in parameters.AllKeys)
                parametersStr += key + "=" + parameters[key] + ",";
            parametersStr += "]";

            Debug.LogError("No se pudo realizar la peticion a " + url + "... con parametros: " + parametersStr);
        }
    }

    public static bool SendOrden(List<Producto> orden, Vendedor vendedor)
    {
        string productos = "{";
        bool first = true;

        List<Producto> filter = new List<Producto>();
        foreach (Producto p in orden)
        {
            if (!filter.Exists(elem => elem.id == p.id))
            {
                if (!first) productos += ",";
                productos += "\"" + p.id + "\":\"" + orden.FindAll(elem => elem.id == p.id).Count + "\"";
                filter.Add(p);
            }
            first = false;
        }
        productos += "}";
        NameValueCollection parameters = new NameValueCollection();
        parameters["vendedor_id"] = vendedor.id.ToString();
        parameters["comprador_id"] = Credentials.user_id.ToString();
        parameters["items"] = productos;

        Debug.Log(parameters["vendedor_id"]);
        Debug.Log(parameters["comprador_id"]);
        Debug.Log(parameters["items"]);

        string response = RequestJson(ServerAdress + OrdenEndpoint, parameters);
        Debug.Log(response);

        return true;
    }

    private static string RequestBlob(string url)
    {
        return null;
    }
    private static Sprite RequestSprite(string url, int with, int height)
    {
        try
        {
            byte[] data;
            using (WebClient wc = new WebClient())
            {
                data = wc.DownloadData(new System.Uri(ServerAdress + url));
                Texture2D texture = new Texture2D(with, height);
                ImageConversion.LoadImage(texture, data);
                Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                return sprite;
            }
        }
        catch
        {
            return null;
        }
    }
    public static Stream FetchStream(string url)
    {
        try
        {
            byte[] data;
            using (WebClient wc = new WebClient())
            {
                data = wc.DownloadData(new System.Uri(ServerAdress + url));
                return new MemoryStream(data);
            }
        }
        catch
        {
            return null;
        }
    }
    private static string RequestJson(string url, NameValueCollection parameters = null)
    {
        if(parameters == null)
            parameters = new NameValueCollection();

        try
        {
            using (WebClient wc = new WebClient())
            {
                parameters["auth_token"] = GetToken();
                var response = wc.UploadValues(url, parameters);
                string resultString = Encoding.Default.GetString(response);
                return resultString;
            }
        }
        catch
        {
            string parametersStr = "[";
            foreach (string key in parameters.AllKeys)
                parametersStr += key + "=" + parameters[key] + ",";
            parametersStr += "]";
            
            Debug.LogError("No se pudo realizar la peticion a " + url + "... con parametros: "+ parametersStr);
            return null; 
        }
    }
}