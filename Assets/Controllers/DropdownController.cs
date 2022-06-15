using Assets.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static TMPro.TMP_Dropdown;

public class DropdownController : MonoBehaviour
{
    public UnityEvent<Vendedor> VendedorChanged = new UnityEvent<Vendedor>();
    public Sprite DefaultIcon;

    private TMP_Dropdown Dropdown;
    private Vendedor[] ArrVendedores;

    void Start()
    {
        Dropdown = gameObject.GetComponent<TMP_Dropdown>();
        Dropdown.onValueChanged.AddListener(SelectedIndexChanged);
    }

    public void InitializeDropdown(List<Vendedor> listVendedores)
    {
        ArrVendedores = listVendedores.ToArray();
        List<OptionData> listOptions = new List<OptionData>();

        foreach (Vendedor vendedor in listVendedores)
        {
            Sprite spr = (vendedor.preview == null) ? DefaultIcon : vendedor.preview;
            listOptions.Add(new OptionData(vendedor.username, spr));
        }

        Dropdown.AddOptions(listOptions);
    }
    
    private void SelectedIndexChanged(int value)
    {
        try
        {
            VendedorChanged.Invoke(ArrVendedores[value]);
        }
        catch (Exception) { ; }
    }

}
