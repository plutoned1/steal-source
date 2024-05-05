using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Steal.Background.Security.Auth
{
	// Token: 0x0200003D RID: 61
	public class json_wrapper
	{
		// Token: 0x0600010B RID: 267 RVA: 0x0000D6AB File Offset: 0x0000B8AB
		public static bool is_serializable(Type to_check)
		{
			return to_check.IsSerializable || to_check.IsDefined(typeof(DataContractAttribute), true);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000D6C8 File Offset: 0x0000B8C8
		public json_wrapper(object obj_to_work_with)
		{
			this.current_object = obj_to_work_with;
			Type type = this.current_object.GetType();
			this.serializer = new DataContractJsonSerializer(type);
			if (!json_wrapper.is_serializable(type))
			{
				throw new Exception(string.Format("the object {0} isn't a serializable", this.current_object));
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000D718 File Offset: 0x0000B918
		public object string_to_object(string json)
		{
			object obj;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(json)))
			{
				obj = this.serializer.ReadObject(memoryStream);
			}
			return obj;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000D760 File Offset: 0x0000B960
		public T string_to_generic<T>(string json)
		{
			return (T)((object)this.string_to_object(json));
		}

		// Token: 0x040000BC RID: 188
		private DataContractJsonSerializer serializer;

		// Token: 0x040000BD RID: 189
		private object current_object;
	}
}
