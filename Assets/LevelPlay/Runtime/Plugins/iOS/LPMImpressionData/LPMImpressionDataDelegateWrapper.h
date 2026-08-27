#import <Foundation/Foundation.h>
#import <IronSource/LPMImpressionDataDelegate.h>

NS_ASSUME_NONNULL_BEGIN

typedef void (*DidReceiveImpressionData)(void *adNativePtr, const char *impressionDataJson);

@interface LPMImpressionDataDelegateWrapper : NSObject <LPMImpressionDataDelegate>
@property (nonatomic) void *adNativePtr;
@property (assign) DidReceiveImpressionData onImpression;

- (instancetype)initWithAdNativePtr:(void *)adNativePtr
                           callback:(DidReceiveImpressionData)callback;
@end

NS_ASSUME_NONNULL_END

#ifdef __cplusplus
extern "C" {
#endif

void *LPMImpressionDataDelegateCreate(void *adNativePtr, DidReceiveImpressionData callback);
void LPMImpressionDataDelegateDestroy(void *delegateRef);

#ifdef __cplusplus
}
#endif
