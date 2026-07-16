#include <jni.h>
#include <string>

extern "C" JNIEXPORT jstring JNICALL Java_com_example_tst2_MainActivity_stringFromJNI(
        JNIEnv* env,
        jobject /* this */) {
    std::string hello = "Hello from C++";
    return env->NewStringUTF(hello.c_str());
}

extern "C" JNIEXPORT const char* JNICALL GetNativeString() {
    static std::string message = "Hello from C++ (Direct Call)";
    return message.c_str();
}

extern "C" JNIEXPORT const char* JNICALL GetNativeString2(const char* strData) {
    static std::string message = "Hello from C++ (Direct Call) = ";
    message += strData;
    return message.c_str();
}

extern "C" JNIEXPORT const char* JNICALL GetNativeString3(const char* strData) {
    std::string message = "Привет из C++ (Direct Call) = ";
    message += strData;

    char* pData = new char[message.size() + 1];
    memset(pData, 0, message.size() + 1);
    memcpy( pData, message.c_str(), message.size());
    return pData;
}

extern "C" JNIEXPORT void JNICALL FreeNativeString3(char* strData) {
    delete[] strData;

}