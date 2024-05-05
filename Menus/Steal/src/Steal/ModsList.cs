using System;
using System.Collections.Generic;
using System.Linq;
using Steal;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class ModsList : MonoBehaviour
{
	// Token: 0x06000014 RID: 20 RVA: 0x00002967 File Offset: 0x00000B67
	public void OnDisable()
	{
		ModsList.displayedText = " ";
		ModsList.displayedText = "";
		ModsList.displayedText2 = "";
		this.guiStyle = null;
		this.guiStyle2 = null;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002998 File Offset: 0x00000B98
	private void OnEnable()
	{
		this.guiStyle = new GUIStyle();
		this.guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		this.guiStyle.normal.textColor = MenuPatch.GetTheme(UI.Theme)[2];
		this.guiStyle.fontSize = 20;
		this.guiStyle.wordWrap = true;
		this.guiStyle2 = new GUIStyle();
		this.guiStyle2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		this.guiStyle2.normal.textColor = MenuPatch.GetTheme(UI.Theme)[2];
		this.guiStyle2.fontSize = 20;
		this.guiStyle2.wordWrap = true;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002A58 File Offset: 0x00000C58
	private void Start()
	{
		this.guiStyle = new GUIStyle();
		this.guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		this.guiStyle.normal.textColor = MenuPatch.GetTheme(UI.Theme)[2];
		this.guiStyle.fontSize = 20;
		this.guiStyle.wordWrap = true;
		this.guiStyle2 = new GUIStyle();
		this.guiStyle2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		this.guiStyle2.normal.textColor = MenuPatch.GetTheme(UI.Theme)[2];
		this.guiStyle2.fontSize = 20;
		this.guiStyle2.wordWrap = true;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002B18 File Offset: 0x00000D18
	private void OnGUI()
	{
		this.guiStyle.normal.textColor = MenuPatch.GetTheme(UI.Theme)[2];
		float num = (float)Screen.width;
		float num2 = 10f;
		float num3 = 5f;
		float num4 = 10f;
		float num5 = 5f;
		float num6 = 10f;
		Color color = GUI.color;
		foreach (string text in ModsList.modsEnabled.OrderByDescending((string m) => m.Length).ToList<string>())
		{
			GUIContent guicontent = new GUIContent(text);
			Vector2 vector = this.guiStyle.CalcSize(guicontent);
			float num7 = num - vector.x - num2 - num4;
			GUI.color = new Color(0f, 0f, 0f, 0.5f);
			GUI.Box(new Rect(num7 - num3, num6 - num3 / 2f, vector.x + 2f * num3, vector.y + num3), GUIContent.none);
			GUI.color = color;
			GUI.Label(new Rect(num7, num6, vector.x, vector.y), text, this.guiStyle);
			GUI.backgroundColor = MenuPatch.GetTheme(UI.Theme)[0];
			GUI.Box(new Rect(num - num2, num6, num4, vector.y), GUIContent.none);
			num6 += vector.y + num5;
		}
		GUI.color = color;
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002CCC File Offset: 0x00000ECC
	private void Update()
	{
		ModsList.deltaTime += (Time.unscaledDeltaTime - ModsList.deltaTime) * 0.1f;
		ModsList.displayedText2 = "steal.lol - FPS:" + Mathf.RoundToInt(1f / ModsList.deltaTime).ToString();
		this.timer += Time.deltaTime;
		if (this.timer > 0.2f)
		{
			this.timer = 0f;
			this.highlightedIndex = (this.highlightedIndex + 1) % ModsList.displayedText2.Length;
		}
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002D60 File Offset: 0x00000F60
	public static void RefreshText()
	{
		List<string> list = new List<string>();
		foreach (MenuPatch.Button button in MenuPatch.buttons)
		{
			if (button.Enabled)
			{
				list.Add(button.buttonText);
			}
		}
		ModsList.modsEnabled = list;
		IOrderedEnumerable<string> orderedEnumerable = ModsList.modsEnabled.OrderByDescending((string s) => s.Length);
		ModsList.displayedText = string.Join("\n", orderedEnumerable);
	}

	// Token: 0x04000015 RID: 21
	public static List<string> modsEnabled = new List<string>();

	// Token: 0x04000016 RID: 22
	private static string displayedText = string.Empty;

	// Token: 0x04000017 RID: 23
	private GUIStyle guiStyle;

	// Token: 0x04000018 RID: 24
	private static string displayedText2 = "steal.lol";

	// Token: 0x04000019 RID: 25
	private GUIStyle guiStyle2;

	// Token: 0x0400001A RID: 26
	private int highlightedIndex;

	// Token: 0x0400001B RID: 27
	private float timer;

	// Token: 0x0400001C RID: 28
	private const float changeInterval = 0.2f;

	// Token: 0x0400001D RID: 29
	public static float deltaTime = 0f;
}
