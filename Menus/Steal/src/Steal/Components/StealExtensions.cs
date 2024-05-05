using System;
using UnityEngine;

namespace Steal.Components
{
	// Token: 0x0200002C RID: 44
	internal static class StealExtensions
	{
		// Token: 0x06000096 RID: 150 RVA: 0x0000992C File Offset: 0x00007B2C
		public static bool TryGetComponentInParent(this Collider component, Type tosearc, out Component go)
		{
			go = null;
			bool flag;
			try
			{
				go = component.GetComponentInParent(tosearc.GetType());
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00009964 File Offset: 0x00007B64
		public static bool TryReplace(this string orgstring, string replaced, string replace, out string result)
		{
			result = orgstring;
			bool flag;
			try
			{
				if (string.IsNullOrEmpty(replaced))
				{
					flag = false;
				}
				else if (string.IsNullOrEmpty(orgstring))
				{
					flag = false;
				}
				else
				{
					result = orgstring.Replace(replaced, replace);
					flag = true;
				}
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000099B0 File Offset: 0x00007BB0
		public static string Remove(this string orgstring, string removed)
		{
			string text;
			try
			{
				string text2;
				if (string.IsNullOrEmpty(removed))
				{
					text = orgstring;
				}
				else if (string.IsNullOrEmpty(orgstring))
				{
					text = orgstring;
				}
				else if (!orgstring.Contains(removed))
				{
					text = orgstring;
				}
				else if (orgstring.TryReplace(removed, "", out text2))
				{
					text = text2;
				}
				else
				{
					text = orgstring;
				}
			}
			catch (Exception)
			{
				text = orgstring;
			}
			return text;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00009A10 File Offset: 0x00007C10
		public static bool AddIfNot(this string orgstring, string add, out string result)
		{
			result = orgstring;
			bool flag;
			try
			{
				if (orgstring.Contains(add))
				{
					flag = false;
				}
				else
				{
					result = add + orgstring;
					flag = true;
				}
			}
			catch (Exception)
			{
				flag = false;
			}
			return flag;
		}
	}
}
