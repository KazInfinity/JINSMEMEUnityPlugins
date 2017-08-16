#import <UIKit/UIKit.h>
#import <MEMELib/MEMELib.h>

@interface MEMEProxy : NSObject<MEMELibDelegate>

@property (strong, nonatomic) MEMERealTimeData *currentRealTimeData;
@property (strong, nonatomic) NSMutableDictionary *cachePheral;

// セッション開始&初期化
void MEMEStartSession(char *appClientId, char *appClientSecret);
// セッション終了&後始末
void MEMEEndSession();

char* MEMEGetSensorValues(char* peripheralUUIDStr);


#pragma mark MEMELib C++ Delegates
typedef void (*MemeAppAuthorizedDelegate)(int result_code);
typedef void (*MemePeripheralFoundDelegate)(char* deviceUUID);
typedef void (*MemePeripheralConnectedDelegate)();
typedef void (*MemePeripheralDisconnetedDelegate)();
typedef void (*MemeRealTimeModeDataReceivedDelegate)();
typedef void (*MemeCommandResponseDelegate)();
typedef void (*MemeFirmwareAuthorizedDelegate)();

@property (nonatomic) MemeAppAuthorizedDelegate AppAuthorizedDelegate;
@property (nonatomic) MemePeripheralFoundDelegate PeripheralFoundDelegate;
@property (nonatomic) MemePeripheralConnectedDelegate PeripheralConnectedDelegate;
@property (nonatomic) MemePeripheralDisconnetedDelegate PeripheralDisconnetedDelegate;
@property (nonatomic) MemeRealTimeModeDataReceivedDelegate RealTimeModeDataReceivedDelegate;
@property (nonatomic) MemeCommandResponseDelegate CommandResponseDelegate;
@property (nonatomic) MemeFirmwareAuthorizedDelegate FirmwareAuthorizedDelegate;

@end

//@property UInt8 fitError; // 0: 正常 1: 装着エラー
//@property BOOL isWalking;
//
//@property BOOL noiseStatus;
//
//@property UInt8 powerLeft; // 5: フル充電 0: 空
//
//@property UInt8 eyeMoveUp;
//@property UInt8 eyeMoveDown;
//@property UInt8 eyeMoveLeft;
//@property UInt8 eyeMoveRight;
//
//@property UInt8 blinkSpeed;    // in ms
//@property UInt16 blinkStrength;
//
//@property float roll;
//@property float pitch;
//@property float yaw;
//
//@property float accX;
//@property float accY;
//@property float accZ;
