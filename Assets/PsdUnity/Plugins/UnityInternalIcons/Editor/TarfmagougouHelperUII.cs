#define UNITY_5_3_AND_UP

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	#undef UNITY_5_3_AND_UP
#endif

namespace tarfmagougou
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	
	public static class TarfmagougouHelperUII
	{
		#if UNITY_5_3_AND_UP
		static StackTraceLogType _log_type_backup;
		#endif
		public static void DeactivateLogTrace()
		{
			#if UNITY_5_4
			_log_type_backup = Application.GetStackTraceLogType(LogType.Log);
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			#elif UNITY_5_3
			_log_type_backup = Application.stackTraceLogType;
			Application.stackTraceLogType = StackTraceLogType.None;
			#endif
		}
		
		public static void ActivateLogTrace()
		{
			#if UNITY_5_4
			Application.SetStackTraceLogType(LogType.Log, _log_type_backup);
			#elif UNITY_5_3
			Application.stackTraceLogType = _log_type_backup;
			#endif
		}
		
		public static void DisableLogging()
		{
			#if UNITY_5_3_AND_UP
			Debug.unityLogger.logEnabled = false;
			#endif
		}
		
		public static void EnableLogging()
		{
			#if UNITY_5_3_AND_UP
			Debug.unityLogger.logEnabled = true;
			#endif
		}
		
		public static void SetWindowTitle(EditorWindow w, string s)
		{
			#if UNITY_5_0
			w.title = s;
			#else
			w.titleContent = new GUIContent("NoScope Stats");
			#endif
		}
		
		public static GUIStyle GetMiniGreyLabelStyle()
		{
			#if UNITY_5_3_AND_UP
			return EditorStyles.centeredGreyMiniLabel;
			#else
			return EditorStyles.miniLabel;
			#endif
		}
		
		public static Texture2D GetAssociatedAlphaSplitTexture(Sprite s)
		{
			#if UNITY_5_3_AND_UP
			return s.associatedAlphaSplitTexture;
			#else
			return null;
			#endif
		}
	}
}
