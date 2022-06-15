using Assets.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public static class ButtonExtension
{
	internal static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
	{
		button.onClick.AddListener(delegate () {
			OnClick(param);
		});
	}
}

public class ListController : MonoBehaviour
{
	public UnityEvent<Producto> ProductoClicked = new UnityEvent<Producto>();
	public Sprite DefaultIcon;

	private GameObject ButtonTemplate;
	private Transform ScrollableList;

	void Start()
	{
		ScrollableList = transform.GetChild(0);
		ButtonTemplate = ScrollableList.GetChild(0).gameObject;
		ButtonTemplate.SetActive(false);
	}

    public void PopulateList(List<Producto> list)
    {
		ClearList();
		GameObject g;
		foreach (Producto p in list)
		{
			g = Instantiate(ButtonTemplate, ScrollableList);
			g.transform.GetChild(0).GetComponent<Image>().sprite = (p.preview == null) ? DefaultIcon : p.preview;
			g.transform.GetChild(1).GetComponent<TMP_Text>().text = p.titulo;
			//g.transform.GetChild(2).GetComponent<TMP_Text>().text = p.titulo;
			g.GetComponent<Button>().AddEventListener(p, ItemClicked);
			g.SetActive(true);
		}
	}
	public void ClearList()
    {
		if (ScrollableList.childCount > 1)
			for (int i = 1; i < ScrollableList.childCount; i++)
				Destroy(ScrollableList.GetChild(i).gameObject);
    }
	private void ItemClicked(Producto p)
	{
		ProductoClicked.Invoke(p);
	}

}