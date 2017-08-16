using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MEME
{
	public class MEMEBehaviour
	{
		public class MEMERealtimeData
		{
			public uint fitError;
			public uint isWalking;

			public uint powerLeft;

			public uint eyeMoveUp;
			public uint eyeMoveDown;
			public uint eyeMoveLeft;
			public uint eyeMoveRight;

			public uint blinkSpeed;
			public uint blinkStrength;

			public float roll;
			public float pitch;
			public float yaw;

			public float accX;
			public float accY;
			public float accZ;

			public bool isValid;
			public string raw;
		}

		public enum MEMERealTimeDataIndex
		{
			Pitch,
			Yaw,
			Roll,
			EyeMoveUp,
			EyeMoveDown,
			EyeMoveLeft,
			EyeMoveRight,
			BlinkSpeed,
			BlinkStrength,
			IsWalking,
			AccX,
			AccY,
			AccZ,
			FitError,
			PowerLeft
		}

		// 認証結果を受取通知,Setter
		private static UnityAction<string> _MemeAppAuthorizedAction;
		private delegate void MemeAppAuthorizedDelegate(int result_code);
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemeAppAuthorizedDelegate))]
		static void MemeAppAuthorizedCallback(int result_code) {
			Debug.Log("MemeAppAuthorizedCallback called:" + result_code);
			if (_MemeAppAuthorizedAction != null)
				_MemeAppAuthorizedAction ("終了");
		}
		[DllImport("__Internal")]
		private static extern void _SetMemeAppAuthorizedDelegate(MemeAppAuthorizedDelegate callback);

		// 認証結果を受け取る通知
		private static UnityAction<string> _MemePeripheralFoundAction;
		private delegate void MemePeripheralFoundDelegate(string deviceUUID);
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemePeripheralFoundDelegate))]
		static void MemePeripheralFoundCallback(string deviceUUID) {
			Debug.LogError("MemePeripheralFoundCallback called:" + deviceUUID);
			if (_MemePeripheralFoundAction != null)
				_MemePeripheralFoundAction (deviceUUID);
		}
		[DllImport("__Internal")]
		private static extern void _SetMemePeripheralFoundDelegate(MemePeripheralFoundDelegate callback);

		// JINS MEMEへの接続完了通知
		private static UnityAction<string> _MemePeripheralConnectedAction;
		private delegate void MemePeripheralConnectedDelegate();
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemePeripheralConnectedDelegate))]
		static void MemePeripheralConnectedCallback() {
			Debug.Log("MemePeripheralConnectedCallback called");
			if (_MemePeripheralConnectedAction != null)
				_MemePeripheralConnectedAction ("終了");
		}
		[DllImport("__Internal")]
		private static extern void _SetMemePeripheralConnectedDelegate(MemePeripheralConnectedDelegate callback);

		// JINS MEMEとの切断通知
		private static UnityAction<string> _MemePeripheralDisconnetedAction;
		private delegate void MemePeripheralDisconnetedDelegate();
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemePeripheralDisconnetedDelegate))]
		static void MemePeripheralDisconnetedCallback() {
			Debug.Log("MemePeripheralDisconnetedCallback called");
			if (_MemePeripheralDisconnetedAction != null)
				_MemePeripheralDisconnetedAction ("終了");
		}
		[DllImport("__Internal")]
		private static extern void _SetMemePeripheralDisconnetedDelegate(MemePeripheralDisconnetedDelegate callback);

		// リアルタイムモードのデータを受け取るデリゲート
		private static UnityAction<string> _MemeRealTimeModeDataReceivedAction;
		private delegate void MemeRealTimeModeDataReceivedDelegate();
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemeRealTimeModeDataReceivedDelegate))]
		static void MemeRealTimeModeDataReceivedCallback() {
			Debug.Log("MemeRealTimeModeDataReceivedCallback called");
			if (_MemeRealTimeModeDataReceivedAction != null)
				_MemeRealTimeModeDataReceivedAction ("終了");
		}
		[DllImport("__Internal")]
		private static extern void _SetMemeRealTimeModeDataReceivedDelegate(MemeRealTimeModeDataReceivedDelegate callback);
		public static void SetMemeRealTimeModeDataReceivedDelegate(UnityAction<string> onFinished = null)
		{
			if (onFinished != null)
				_MemeRealTimeModeDataReceivedAction = onFinished;
			_SetMemeRealTimeModeDataReceivedDelegate (MemeRealTimeModeDataReceivedCallback);
		}

		// コマンド実行結果のイベントを受け取ります
		private static UnityAction<string> _MemeCommandResponseAction;
		private delegate void MemeCommandResponseDelegate();
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemeCommandResponseDelegate))]
		static void MemeCommandResponseCallback() {
			Debug.Log("MemeCommandResponseCallback called");
			if (_MemeCommandResponseAction != null)
				_MemeCommandResponseAction ("終了");
		}
		[DllImport("__Internal")]
		private static extern void _SetMemeCommandResponseDelegate(MemeCommandResponseDelegate callback);
		public static void SetMemeCommandResponseDelegate(UnityAction<string> onFinished = null)
		{
			if (onFinished != null)
				_MemeCommandResponseAction = onFinished;
			_SetMemeCommandResponseDelegate (MemeCommandResponseCallback);
		}

		// ファームウェアの認証結果を受け取ります
		private static UnityAction<string> _MemeFirmwareAuthorizedAction;
		private delegate void MemeFirmwareAuthorizedDelegate();
		[AOT.MonoPInvokeCallbackAttribute(typeof(MemeFirmwareAuthorizedDelegate))]
		static void MemeFirmwareAuthorizedCallback() {
			Debug.Log("MemeFirmwareAuthorizedCallback called");
			if (_MemeFirmwareAuthorizedAction != null)
				_MemeFirmwareAuthorizedAction ("終了");
		}
		[DllImport("__Internal")]
		private static extern void _SetMemeFirmwareAuthorizedDelegate(MemeFirmwareAuthorizedDelegate callback);
		public static void SetMemeFirmwareAuthorizedDelegate(UnityAction<string> onFinished = null)
		{
			if (onFinished != null)
				_MemeFirmwareAuthorizedAction = onFinished;
			_SetMemeFirmwareAuthorizedDelegate (MemeFirmwareAuthorizedCallback);
		}


		// セッション開始&初期化
		[DllImport("__Internal")]
		private static extern void MEMEStartSession(string appClientId, string appClientSecret);
		public static void StartSession(string appClientId, string appClientSecret, UnityAction<string> onFinished = null) 
	    {
			if (Application.platform == RuntimePlatform.IPhonePlayer) 
	        {
				MEMEStartSession(appClientId, appClientSecret);
				_MemeAppAuthorizedAction = onFinished;
				_SetMemeAppAuthorizedDelegate (MemeAppAuthorizedCallback);
			}
		}

		[DllImport("__Internal")]
		private static extern int _MEMEstartScanningPeripherals();
		/// <summary>
		/// JINS MEMEのスキャンを開始します
		/// </summary>
		public static int MEMEstartScanningPeripherals(UnityAction<string> onFinished = null)
		{
			_MemePeripheralFoundAction = onFinished;
			_SetMemePeripheralFoundDelegate (MemePeripheralFoundCallback);
			return _MEMEstartScanningPeripherals ();
		}

		[DllImport("__Internal")]
		private static extern void _MEMEconnectPeripheral(string deviceID);
		/// <summary>
		/// JINS MEMEと接続します
		/// </summary>
		public static void MEMEconnectPeripheral(string deviceID, UnityAction<string> onFinished = null)
		{
			_MemePeripheralConnectedAction = onFinished;
			_SetMemePeripheralConnectedDelegate (MemePeripheralConnectedCallback);
			_MEMEconnectPeripheral (deviceID);
		}

		// データ受信開始
		[DllImport("__Internal")]
		private static extern void _MEMEstartDataReport();
		/// <summary>
		/// JINS MEMEとデータ受信開始
		/// </summary>
		public static void MEMEstartDataReport()
		{
			_MEMEstartDataReport ();
		}
		// データ受信停止
		[DllImport("__Internal")]
		private static extern void _MEMEstopDataReport();
		/// <summary>
		/// JINS MEMEとデータ受信停止
		/// </summary>
		public static void MEMEstopDataReport()
		{
			_MEMEstopDataReport ();
		}

		[DllImport("__Internal")]
		private static extern int _MEMEstopScanningPeripherals();
		/// <summary>
		/// JINS MEMEのスキャンを停止します
		/// </summary>
		public static int MEMEstopScanningPeripherals()
		{
			return _MEMEstopScanningPeripherals ();
		}

		[DllImport("__Internal")]
		private static extern int _MEMEdiconnectPeripheral();
		/// <summary>
		/// JINS MEMEから切断します
		/// </summary>
		public static int MEMEdiconnectPeripheral(UnityAction<string> onFinished = null)
		{
			_MemePeripheralDisconnetedAction = onFinished;
			_SetMemePeripheralDisconnetedDelegate (MemePeripheralDisconnetedCallback);
			return _MEMEdiconnectPeripheral ();
		}

		// セッション終了&後始末
		[DllImport("__Internal")]
		private static extern void MEMEEndSession();
		public static void EndSession() 
	    {
			MEMEEndSession();
		}

		// 自動再接続の設定を行います
		[DllImport("__Internal")]
		private static extern void _SetAutoConnectPlugin(bool isAutoConnect);
		public static void SetAutoConnectPlugin(bool isAutoConnect) 
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				_SetAutoConnectPlugin(isAutoConnect);
			}
		}

		// データ取得
		[DllImport("__Internal")]
		private static extern string MEMEGetSensorValues();
		public static MEMERealtimeData GetSensorValues() 
	    {
			MEMERealtimeData result = new MEMERealtimeData ();
			if (Application.platform == RuntimePlatform.IPhonePlayer) 
	        {
				var values = MEMEGetSensorValues();
				if (string.IsNullOrEmpty(values))
				{
					return result;
				}

				var parts = values.Split(',');
				if (values.Length > 0) {
					result.fitError = uint.Parse (parts [(uint)MEMERealTimeDataIndex.FitError]);
					result.isWalking = uint.Parse (parts [(uint)MEMERealTimeDataIndex.IsWalking]);
					result.powerLeft = uint.Parse (parts [(uint)MEMERealTimeDataIndex.PowerLeft]);
					result.eyeMoveUp = uint.Parse(parts[(uint)MEMERealTimeDataIndex.EyeMoveUp]);
					result.eyeMoveDown = uint.Parse(parts[(uint)MEMERealTimeDataIndex.EyeMoveDown]);
					result.eyeMoveLeft = uint.Parse(parts[(uint)MEMERealTimeDataIndex.EyeMoveLeft]);
					result.eyeMoveRight = uint.Parse(parts[(uint)MEMERealTimeDataIndex.EyeMoveRight]);
					result.blinkStrength = uint.Parse(parts[(uint)MEMERealTimeDataIndex.BlinkSpeed]);
					result.blinkStrength = uint.Parse(parts[(uint)MEMERealTimeDataIndex.BlinkStrength]);
					result.pitch = float.Parse (parts [(uint)MEMERealTimeDataIndex.Pitch]); 
					result.yaw = float.Parse (parts [(uint)MEMERealTimeDataIndex.Yaw]); 
					result.roll = float.Parse(parts[(uint)MEMERealTimeDataIndex.Roll]);
					result.accX = float.Parse(parts[(uint)MEMERealTimeDataIndex.AccX]);
					result.accY = float.Parse(parts[(uint)MEMERealTimeDataIndex.AccY]);
					result.accZ = float.Parse(parts[(uint)MEMERealTimeDataIndex.AccZ]);

					result.raw = values;
					result.isValid = true;
				}
			}
			return result;
		}

		[DllImport("__Internal")]
		private static extern bool _IsConnected();
		/// <summary>
		/// JINS MEMEに接続済みかどうかを返します。
		/// </summary>
		public static bool IsConnected()
		{
			return _IsConnected();
		}

		[DllImport("__Internal")]
		private static extern string _GetSDKVersionPlugin();
		/// <summary>
		/// SDKのバージョンを取得します
		/// </summary>
		public static string GetSDKVersionPlugin()
		{
			return _GetSDKVersionPlugin ();
		}

		[DllImport("__Internal")]
		private static extern string _GetFWVersionPlugin();
		/// <summary>
		/// ファームウェアのバージョンを取得します
		/// </summary>
		public static string GetFWVersionPlugin()
		{
			return _GetFWVersionPlugin ();
		}

		[DllImport("__Internal")]
		private static extern int _GetConnectedDeviceTypePlugin();
		/// <summary>
		/// デバイスタイプを取得します
		/// 1:ES (ウェリントン), 2:MT(サングラス), 6:SW ES (ウェリントン)
		/// </summary>
		public static int GetConnectedDeviceTypePlugin()
		{
			return _GetConnectedDeviceTypePlugin ();
		}

		[DllImport("__Internal")]
		private static extern int _GetConnectedDeviceSubTypePlugin();
		/// <summary>
		/// デバイスタイプを取得します
		/// 1:ES (ウェリントン), 2:MT(サングラス), 6:SW ES (ウェリントン)
		/// </summary>
		public static int GetConnectedDeviceSubTypePlugin()
		{
			return _GetConnectedDeviceSubTypePlugin ();
		}

		[DllImport("__Internal")]
		private static extern bool _IsDataReceivingPlugin();
		/// <summary>
		/// データ受信状況
		/// </summary>
		public static bool IsDataReceivingPlugin()
		{
			return _IsDataReceivingPlugin ();
		}

		[DllImport("__Internal")]
		private static extern int _GetHWVersion();
		/// <summary>
		/// ハードウェアバージョンを取得します
		/// </summary>
		public static int GetHWVersion()
		{
			return _GetHWVersion ();
		}
	}
}