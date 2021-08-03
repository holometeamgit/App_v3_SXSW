#import <objc/runtime.h>
#import "AudioSessionCategoryAppController.h"

@implementation UnityAppController (AudioSessionCategoryAppController)

+ (void)load {
    Method original;
    Method swizzled;
    
    original = class_getInstanceMethod(
        self, @selector(startUnity:));
    swizzled = class_getInstanceMethod(
        self,
        @selector(startUnityAudioSession:));
    method_exchangeImplementations(original, swizzled);
}

- (void)startUnityAudioSession:(UIApplication *)application {
    [self startUnityAudioSession:application];

    AVAudioSession* audioSession = [AVAudioSession sharedInstance];
    [audioSession setCategory:AVAudioSessionCategoryPlayback error:nil];
    [audioSession setActive:YES error:nil];
}

@end
