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
    
    [audioSession setActive:NO withOptions:AVAudioSessionSetActiveOptionNotifyOthersOnDeactivation error:nil];
    [audioSession setCategory:AVAudioSessionCategoryPlayAndRecord withOptions:AVAudioSessionCategoryOptionMixWithOthers | AVAudioSessionCategoryOptionDefaultToSpeaker error:nil];
    [audioSession setActive:YES withOptions:AVAudioSessionSetActiveOptionNotifyOthersOnDeactivation error:nil];
}

@end
