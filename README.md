# JINSMEME for Unity
JINSMEME用Unityプラグイン
JINSMEMEフレームワークをUnityで利用できるようにしたプラグインです。
※現状iOS版のみ対応しています。（Androidは利用できません。）

正式なシーケンスは確認中
Unity 5.5.0f3 と SDKVer 1.1.3で作成しています。

＜事前作業＞
1."Assets/Builds/Frameworks"内に"MEMELib.framework"を入れてください。

 [JINS MEME Developerサイト]
 
 https://jins-meme.github.io/sdkdoc/

2. 1でアクセスしたJINSサイトにアクセスしてアプリ枠登録をしてください。

 発行されたアプリID/アプリSecretを[TestMEME(.cs).appClientId]と[TestMEME(.cs).appClientSecret]に転記してください。

3.iOSビルど後にXCodeプロジェクトの[Build Phases]->[Copy Files]に以下の設定をしてください。

 Destination : Frameworks

 [+]ボタンクリックで"MEMELib.framework"を追加

これでビルドできると思います。

（一応動くバージョンであるため今後さまざまなところに修正が入る可能性があります。）

＜サンプル動作手順＞

端末のBluetoothをONにする

1.[認証]ボタン→コンソールに"認証完了"と表示されます。

2.[スキャン]ボタン→デバイスのスキャンを開始します。→デバイスが見つかると"接続するデバイス"にボタンが表示されます。

3.JINS MEMEデバイスボタンを長押し→"接続するデバイス"にデバイス表示

4."接続するデバイス"のボタンをクリック→コンソールに"デバイス接続"/デバイス情報が表示されます。

5.[データ受信開始]ボタン→データの受信を開始

6.[データ受信停止]ボタン→データの受信を終了

7.[スキャン停止ボタン]ボタン→デバイスのスキャンを停止

8.[切断]ボタン→終了処理
