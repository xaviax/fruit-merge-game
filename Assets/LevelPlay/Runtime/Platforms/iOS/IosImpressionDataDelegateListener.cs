#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.LevelPlay
{
    // Implemented by iOS ad classes (banner / interstitial / rewarded) that want to receive
    // per-instance impression data callbacks via the shared LPMImpressionDataDelegateWrapper.
    internal interface IIosImpressionDataReceiver
    {
        void InvokeImpressionDataReadyEvent(string impressionDataJson);
    }

    sealed class IosImpressionDataDelegateListener : IosNativeObject
    {
        readonly DidReceiveImpressionData _kImpressionCallback = OnImpression;

        internal IosImpressionDataDelegateListener(IosNativeObject ad) : base(false)
        {
            NativePtr = LPMImpressionDataDelegateCreate(ad.NativePtr, _kImpressionCallback);
        }

        public override void Dispose()
        {
            if (NativePtr != IntPtr.Zero)
            {
                LPMImpressionDataDelegateDestroy(NativePtr);
                NativePtr = IntPtr.Zero;
            }
            base.Dispose();
        }

        [DllImport("__Internal", EntryPoint = "LPMImpressionDataDelegateCreate")]
        private static extern IntPtr LPMImpressionDataDelegateCreate(IntPtr adNativePtr, DidReceiveImpressionData callback);

        [DllImport("__Internal", EntryPoint = "LPMImpressionDataDelegateDestroy")]
        private static extern void LPMImpressionDataDelegateDestroy(IntPtr delegatePtr);

        delegate void DidReceiveImpressionData(IntPtr adPtr, string impressionDataJson);

        [MonoPInvokeCallback(typeof(DidReceiveImpressionData))]
        static void OnImpression(IntPtr adPtr, string impressionDataJson)
        {
            var ad = Get<IosNativeObject>(adPtr);
            (ad as IIosImpressionDataReceiver)?.InvokeImpressionDataReadyEvent(impressionDataJson);
        }
    }
}
#endif
