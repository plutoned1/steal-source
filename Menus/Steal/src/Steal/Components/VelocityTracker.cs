using System;
using UnityEngine;

namespace Steal.Components
{
	// Token: 0x0200002D RID: 45
	public class VelocityTracker : MonoBehaviour
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00009A50 File Offset: 0x00007C50
		private void Start()
		{
			this.rotationLast = base.transform.rotation.eulerAngles;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00009A78 File Offset: 0x00007C78
		private void LateUpdate()
		{
			this.rotationDelta = base.transform.rotation.eulerAngles - this.rotationLast;
			this.rotationLast = base.transform.rotation.eulerAngles;
			this._velocity = (base.transform.position - this._previousvelocity) / Time.deltaTime;
			this._previousvelocity = base.transform.position;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00009AF9 File Offset: 0x00007CF9
		public Vector3 angularVelocity
		{
			get
			{
				return this.rotationDelta;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00009B01 File Offset: 0x00007D01
		public Vector3 velocity
		{
			get
			{
				return this._velocity;
			}
		}

		// Token: 0x0400006B RID: 107
		private Vector3 rotationLast;

		// Token: 0x0400006C RID: 108
		private Vector3 rotationDelta;

		// Token: 0x0400006D RID: 109
		private Vector3 _previousvelocity;

		// Token: 0x0400006E RID: 110
		private Vector3 _velocity;
	}
}
