#import "LPMImpressionDataDelegateWrapper.h"

@implementation LPMImpressionDataDelegateWrapper

- (instancetype)initWithAdNativePtr:(void *)adNativePtr
                           callback:(DidReceiveImpressionData)callback {
    self = [super init];
    if (self) {
        self.adNativePtr = adNativePtr;
        self.onImpression = callback;
    }
    return self;
}

#pragma mark - LPMImpressionDataDelegate

- (void)impressionDataDidSucceed:(LPMImpressionData *)impressionData {
    if (!self.onImpression) {
        return;
    }

    NSDictionary *allData = [impressionData allData];
    NSString *jsonString = @"";
    if (allData) {
        NSError *error = nil;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:allData
                                                           options:0
                                                             error:&error];
        if (jsonData) {
            jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] ?: @"";
        } else {
            NSLog(@"LPMImpressionDataDelegateWrapper: failed to serialize impression data: %@", error);
        }
    }

    self.onImpression(self.adNativePtr, [jsonString UTF8String]);
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void *LPMImpressionDataDelegateCreate(void *adNativePtr, DidReceiveImpressionData callback) {
    LPMImpressionDataDelegateWrapper *wrapper =
        [[LPMImpressionDataDelegateWrapper alloc] initWithAdNativePtr:adNativePtr
                                                             callback:callback];
    return (__bridge_retained void *)wrapper;
}

void LPMImpressionDataDelegateDestroy(void *delegateRef) {
    if (delegateRef == NULL) {
        return;
    }
    LPMImpressionDataDelegateWrapper *wrapper =
        (__bridge_transfer LPMImpressionDataDelegateWrapper *)delegateRef;
    wrapper.onImpression = nil;
    wrapper.adNativePtr = NULL;
}

#ifdef __cplusplus
}
#endif
