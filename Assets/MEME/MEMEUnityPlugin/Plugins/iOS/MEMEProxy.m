#import "MEMEProxy.h"

@implementation MEMEProxy

MEMEProxy *instance;

- (id) init:(NSString *)appClientId clientSecret:(NSString*)clientSecret {
    self = [super init];
    // アプリ認証とSDK認証を行います
    [MEMELib setAppClientId:appClientId clientSecret:clientSecret];
    [MEMELib sharedInstance].delegate = self;
    return self;
}

// 他アプリケーションでつなげているJINS MEMEを取得します
- (NSArray*)MEMEgetConnectedByOthers {
    NSArray* array = [[MEMELib sharedInstance] getConnectedByOthers];
    return array;
}

// キャリブレーション状態を取得します
- (MEMECalibStatus)MEMEisCalibrated {
    MEMECalibStatus status = [[MEMELib sharedInstance] isCalibrated];
    return status;
}

// =============================================================================
#pragma mark
#pragma mark MEMELib Delegates

// 認証結果を受け取る通知
- (void) memeAppAuthorized:(MEMEStatus)status
{
    NSString* result = [self checkMEMEStatus:status];
    NSLog( @"APP認証結果");
    NSLog(@"%@", result);
    instance.AppAuthorizedDelegate(status);
}

// スキャン結果受信通知
- (void) memePeripheralFound: (CBPeripheral *) peripheral withDeviceAddress:(NSString *)address
{
    NSString* str = [peripheral.identifier UUIDString];
    NSLog(@"スキャン結果: %@", str);
    instance.PeripheralFoundDelegate(MakeStringCopy([str UTF8String]));
    [instance.cachePheral setObject:peripheral forKey:str];
}

// JINS MEMEへの接続完了通知
- (void) memePeripheralConnected: (CBPeripheral *)peripheral
{
    NSLog(@"JINS MEMEへの接続完了: %@", [peripheral.identifier UUIDString]);
    instance.PeripheralConnectedDelegate();
}

// JINS MEMEとの切断通知
- (void) memePeripheralDisconneted: (CBPeripheral *)peripheral
{
    NSLog(@"MEME Device Disconnected: %@", [peripheral.identifier UUIDString]);
    instance.PeripheralDisconnetedDelegate();
}

// リアルタイムモードのデータを受け取るデリゲート
- (void) memeRealTimeModeDataReceived: (MEMERealTimeData *) data
{
    // 確認用
    // NSLog(@"リアルタイムデータ受信");
    self.currentRealTimeData = data;
    instance.RealTimeModeDataReceivedDelegate();
}

// コマンド実行結果のイベントを受け取ります
- (void) memeCommandResponse: (MEMEResponse) response
{
    NSLog(@"コマンド実行結果のイベントを受け取り:%d/%s", response.eventCode, response.commandResult ? "TRUE" : "FALSE");
    instance.CommandResponseDelegate();
}

// ファームウェアの認証結果を受け取ります
- (void) memeFirmwareAuthorized: (MEMEStatus) status
{
    NSLog(@"ファームウェアの認証結果 : %@", [self checkMEMEStatus:status]);
    instance.FirmwareAuthorizedDelegate();
}

#pragma mark UTILITY
// アプリおよびSDK認証、JINS MEMEとの接続状況を定義します。
// TODO ENUMを文字列にしているだけなので仮実装する
- (NSString *) checkMEMEStatus: (MEMEStatus) status
{
    if (status == MEME_OK)
    {
        // アプリ認証に成功し、JINS MEMEへの接続が確立されている
        return @"MEME_OK";
    }
    else if(status == MEME_ERROR)
    {
        // JINS MEMEへのコマンド発行に失敗した
        return @"MEME_ERROR";
    }
    else if(status == MEME_ERROR_SDK_AUTH)
    {
        // SDK認証に失敗した
        return @"MEME_ERROR_SDK_AUTH";
    }
    else if(status == MEME_ERROR_APP_AUTH)
    {
        // アプリ認証に失敗した
        return @"MEME_ERROR_APP_AUTH";
    }
    else if(status == MEME_ERROR_CONNECTION)
    {
        // JINS MEMEへの接続が確立されていない
        return @"MEME_ERROR_CONNECTION";
    }
    else if(status == MEME_DEVICE_INVALID)
    {
        // デバイス無効
        return @"MEME_DEVICE_INVALID";
    }
    else if(status == MEME_CMD_INVALID)
    {
        // コマンド実行を許容していません
        return @"MEME_CMD_INVALID";
    }
    else if(status == MEME_ERROR_FW_CHECK)
    {
        // MEMEのファームウェアが古い
        return @"MEME_ERROR_FW_CHECK";
    }
    else if(status == MEME_ERROR_BL_OFF)
    {
        // Bluetoothが有効でありません
        return @"MEME_ERROR_BL_OFF";
    }
    else
    {
        return @"その他の信号";
    }
}

// =============================================================================
#pragma mark - UnityInterface - Common

char* MakeStringCopy (const char* string) {
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

// セッション開始&初期化
void MEMEStartSession(char *appClientId, char *appClientSecret)
{
    NSLog(@"StartSession");
    if (instance != NULL)
    {
        return;
    }
    NSString *clientId = [NSString stringWithCString:appClientId encoding:NSUTF8StringEncoding];
    NSString *clientSecret = [NSString stringWithCString:appClientSecret encoding:NSUTF8StringEncoding];
    instance = [[MEMEProxy alloc] init:clientId clientSecret:clientSecret];
    instance.cachePheral = [NSMutableDictionary dictionary];
}

// JINS MEMEのスキャンを開始します。
void _MEMEstartScanningPeripherals()
{
    // Start Scanning
    MEMEStatus status = [[MEMELib sharedInstance] startScanningPeripherals];
    //return [self checkMEMEStatus: status];
}

// JINS MEMEと接続します。
void _MEMEconnectPeripheral(char* devicePheral)
{
    NSString *str = [ [ NSString alloc ] initWithUTF8String:devicePheral];
    CBPeripheral* cache = [instance.cachePheral objectForKey:str];
    if([str isEqualToString:[cache.identifier UUIDString]])
    {
        MEMEStatus status = [[MEMELib sharedInstance] connectPeripheral : cache];
    }
}

// データ受信を開始します
void _MEMEstartDataReport()
{
    MEMEStatus status = [[MEMELib sharedInstance] startDataReport];
}

// データ受信を停止します
void _MEMEstopDataReport()
{
    MEMEStatus status = [[MEMELib sharedInstance] stopDataReport];
}

// JINS MEMEのスキャンを停止します。
void _MEMEstopScanningPeripherals()
{
    MEMEStatus status = [[MEMELib sharedInstance] stopScanningPeripherals];
    //return [self checkMEMEStatus: status];
}

// JINS MEMEから切断します。
void _MEMEdiconnectPeripheral()
{
    MEMEStatus status = [[MEMELib sharedInstance] disconnectPeripheral];
}

// セッション終了&後始末
void MEMEEndSession() {
    [MEMELib sharedInstance].delegate = nil;
    instance = NULL;
}

/*
 * 自動再接続の設定を行います。
 * TRUEかつ、JINS MEMEと接続されていない場合、MemeLibは自動的に直近で接続したJINS MEMEとの接続を試みます。
 */
void _SetAutoConnectPlugin(bool isAutoConnect)
{
    [[MEMELib sharedInstance] setAutoConnect:isAutoConnect];
}

#pragma mark - UnityInterface - Status

// データ取得
char* MEMEGetSensorValues() {
    NSString *value = [NSString stringWithFormat:@"%f,%f,%f,%u,%u,%u,%u,%u,%u,%d,%f,%f,%f,%u,%u",
                       instance.currentRealTimeData.pitch,
                       instance.currentRealTimeData.yaw,
                       instance.currentRealTimeData.roll,
                       instance.currentRealTimeData.eyeMoveUp,
                       instance.currentRealTimeData.eyeMoveDown,
                       instance.currentRealTimeData.eyeMoveLeft,
                       instance.currentRealTimeData.eyeMoveRight,
                       instance.currentRealTimeData.blinkSpeed,
                       instance.currentRealTimeData.blinkStrength,
                       instance.currentRealTimeData.isWalking,
                       instance.currentRealTimeData.accX,
                       instance.currentRealTimeData.accY,
                       instance.currentRealTimeData.accZ,
                       instance.currentRealTimeData.fitError,
                       instance.currentRealTimeData.powerLeft
                       ];
    return MakeStringCopy([value UTF8String]);
}

// JINS MEMEに接続済みかどうかを返します。
bool _IsConnected()
{
    return [[MEMELib sharedInstance] isConnected];
}

// SDKのバージョンを取得します
char* _GetSDKVersionPlugin()
{
    return MakeStringCopy([[[MEMELib sharedInstance] getSDKVersion] UTF8String]);
}

// ファームウェアのバージョンを取得します
char* _GetFWVersionPlugin()
{
    return MakeStringCopy([[[MEMELib sharedInstance] getFWVersion] UTF8String]);
}

// デバイスタイプを取得します
// 1:ES (ウェリントン), 2:MT(サングラス), 6:SW ES (ウェリントン)
int _GetConnectedDeviceTypePlugin()
{
    return [[MEMELib sharedInstance] getConnectedDeviceType];
}

// デバイスタイプを取得します
// 1:シャイニーブラック, 2:マッドブラック
int _GetConnectedDeviceSubTypePlugin()
{
    return [[MEMELib sharedInstance] getConnectedDeviceSubType];
}

// データ受信状況
bool _IsDataReceivingPlugin()
{
    return [[MEMELib sharedInstance] isDataReceiving];
}

// ハードウェアバージョンを取得します
int _GetHWVersion()
{
    return [[MEMELib sharedInstance] getHWVersion];
}

#pragma Delegete
void _SetMemeAppAuthorizedDelegate(MemeAppAuthorizedDelegate callback) {
    NSLog(@"_SetMemeAppAuthorizedDelegate called");
    instance.AppAuthorizedDelegate = callback;
}
void _SetMemePeripheralFoundDelegate(MemePeripheralFoundDelegate callback) {
    NSLog(@"_SetMemePeripheralFoundDelegate called");
    instance.PeripheralFoundDelegate = callback;
}
void _SetMemePeripheralConnectedDelegate(MemePeripheralConnectedDelegate callback) {
    NSLog(@"_SetMemePeripheralConnectedDelegate called");
    instance.PeripheralConnectedDelegate = callback;
}
void _SetMemePeripheralDisconnetedDelegate(MemePeripheralDisconnetedDelegate callback) {
    NSLog(@"_SetMemePeripheralDisconnetedDelegate called");
    instance.PeripheralDisconnetedDelegate = callback;
}
void _SetMemeRealTimeModeDataReceivedDelegate(MemeRealTimeModeDataReceivedDelegate callback) {
    NSLog(@"_SetMemeRealTimeModeDataReceivedDelegate called");
    instance.RealTimeModeDataReceivedDelegate = callback;
}
void _SetMemeCommandResponseDelegate(MemeCommandResponseDelegate callback) {
    NSLog(@"_SetMemeCommandResponseDelegate called");
    instance.CommandResponseDelegate = callback;
}
void _SetMemeFirmwareAuthorizedDelegate(MemeFirmwareAuthorizedDelegate callback) {
    NSLog(@"_SetMemeFirmwareAuthorizedDelegate called");
    instance.FirmwareAuthorizedDelegate = callback;
}
@end
