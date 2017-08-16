using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MEME;

public class TestMEME : MonoBehaviour
{
	// Please set your app's id and secret.
	private const string appClientId = "348852870091694";
	private const string appClientSecret = "t6q6ghah6k8qdelj16ohjq5mctwl0a4s";

	/// <summary>認証初期化ボタン</summary>
	[SerializeField]
	private Button appAuthorizedBtn;

	/// <summary>デバイススキャンボタン</summary>
	[SerializeField]
	private Button startScanningBtn;

	/// <summary>接続ボタン</summary>
	[SerializeField]
	private Button connectBtn;

	/// <summary>データ受信開始ボタン</summary>
	[SerializeField]
	private Button startDataBtn;

	/// <summary>データ受信停止ボタン</summary>
	[SerializeField]
	private Button endDataBtn;
	/// <summary>スキャン停止ボタン</summary>
	[SerializeField]
	private Button stopScanningBtn;

	/// <summary>切断ボタン</summary>
	[SerializeField]
	private Button disConnectBtn;

	[SerializeField]
	private Text consoleText;

	[SerializeField]
	private Text statusText;
	[SerializeField]
	private Button debugStatusBtn;

	[SerializeField]
	private ConnectButtonUI connectUI;

	public void Start()
	{
		appAuthorizedBtn.onClick.AddListener (OnClickAppAuthorizedBtn);
		startScanningBtn.onClick.AddListener (OnClickStartScanningBtn);
		connectBtn.onClick.AddListener (OnClickConnectBtn);
		startDataBtn.onClick.AddListener (OnClickStartDataBtn);
		endDataBtn.onClick.AddListener (OnClickEndDataBtn);
		stopScanningBtn.onClick.AddListener (OnClickStopScanningBtn);
		disConnectBtn.onClick.AddListener (OnClickDisConnectBtn);
		//debugStatusBtn.onClick.AddListener (OnDebug);

	}


	void OnEnable()
	{
	}

	void OnDisable()
	{
	}

	void Update()
	{
		if (isInit == false)
		{
			statusText.text = "";
			return;
		}

		statusText.text = MEMEBehaviour.IsConnected () ? "接続済み" : "未接続";
		if (MEMEBehaviour.IsConnected ())
		{
			statusText.text += string.Format ("\nデータ受信:{0}", MEMEBehaviour.IsDataReceivingPlugin () ? "可能" : "不可能");
			statusText.text += string.Format ("\nSDK Version:{0}", MEMEBehaviour.GetSDKVersionPlugin ());
			statusText.text += string.Format ("\nHW Version:{0}", MEMEBehaviour.GetHWVersion ());
			statusText.text += string.Format ("\nFW Version:{0}", MEMEBehaviour.GetFWVersionPlugin ());

			string deviceName = "";
			int deviceType = MEMEBehaviour.GetConnectedDeviceTypePlugin ();
			if (deviceType == 1)
				deviceName = "ES (ウェリントン)";
			else if (deviceType == 2)
				deviceName = "MT(サングラス)";
			else  if (deviceType == 3)
				deviceName = "SW ES (ウェリントン)";
			statusText.text += string.Format ("\nDevice Type:{0}", deviceName);

			string subDeviceName = "";
			int subDeviceType = MEMEBehaviour.GetConnectedDeviceSubTypePlugin ();
			if (subDeviceType == 1)
				subDeviceName = "シャイニーブラック";
			else if (subDeviceType == 2)
				subDeviceName = "マッドブラック";
			statusText.text += string.Format ("\nDevice Sub Type:{0}", subDeviceName);
		}
	}

	private bool isInit = false;

	/// <summary>
	/// 認証処理実行(Delegate登録も実行)
	/// </summary>
	private void OnClickAppAuthorizedBtn()
	{
		MEMEBehaviour.StartSession(appClientId, appClientSecret, OnMemeAppAuthorized);
		MEMEBehaviour.SetMemeRealTimeModeDataReceivedDelegate (OnMemeRealTimeModeDataReceived);
		MEMEBehaviour.SetMemeCommandResponseDelegate (OnMemeCommandResponse);
		MEMEBehaviour.SetMemeFirmwareAuthorizedDelegate (OnMemeFirmwareAuthorized);
		isInit = true;
	}

	/// <summary>
	/// デバイススキャン実行
	/// </summary>
	private void OnClickStartScanningBtn()
	{
		connectUI.Init ();
		MEMEBehaviour.MEMEstartScanningPeripherals (OnMemePeripheral);
	}

	/// <summary>
	/// デバイス接続実行
	/// </summary>
	private void OnClickConnectBtn()
	{
//		MEMEBehaviour.MEMEconnectPeripheral (cacheDeviceID, OnMemePeripheralConnected);
	}

	/// <summary>
	/// データ受信開始実行
	/// </summary>
	private void OnClickStartDataBtn()
	{
		MEMEBehaviour.MEMEstartDataReport ();
	}

	/// <summary>
	/// データ受信停止実行
	/// </summary>
	private void OnClickEndDataBtn()
	{
		MEMEBehaviour.MEMEstopDataReport ();
	}

	/// <summary>
	/// デバイススキャン停止
	/// </summary>
	private void OnClickStopScanningBtn()
	{
		connectUI.Init ();
		MEMEBehaviour.MEMEstopScanningPeripherals ();
	}

	/// <summary>
	/// デバイス切断実行
	/// </summary>
	private void OnClickDisConnectBtn()
	{
		isInit = false;
		MEMEBehaviour.MEMEdiconnectPeripheral (OnMemePeripheralDisconneted);
		MEMEBehaviour.EndSession();
	}

	private void OnMemeAppAuthorized(string text)
	{
		consoleText.text = "認証完了";
	}

//	private string cacheDeviceID;

	private void OnMemePeripheral(string text)
	{
		connectUI.Add (text, ()=> MEMEBehaviour.MEMEconnectPeripheral (text, OnMemePeripheralConnected));
		consoleText.text = "スキャン完了";
//		cacheDeviceID = text;
	}

	private void OnMemePeripheralConnected(string text)
	{
		consoleText.text = "デバイス接続";
	}

	private void OnMemePeripheralDisconneted(string text)
	{
		consoleText.text = "デバイス切断";
	}

	private void OnMemeRealTimeModeDataReceived(string text)
	{
		//consoleText.text = "リアルタイムデータ受信";
	}

	private void OnMemeCommandResponse(string text)
	{
		consoleText.text = "コマンド実行結果完了";
	}

	private void OnMemeFirmwareAuthorized(string text)
	{
		consoleText.text = "ファームウェアの認証完了";
	}
}