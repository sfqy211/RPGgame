using System;
using System.Runtime.InteropServices;

public static class InputLanguageManager
{
    // 导入User32.dll的LoadKeyboardLayout函数
    [DllImport("user32.dll")]
    private static extern IntPtr LoadKeyboardLayout(string _keyboardLayoutId, uint _flags);

    // 设置输入语言的常量，'00000409' 是美国英语的语言代码
    private const string EnglishInputLocale = "00000409"; // English (US)

    // 加载并设置英文输入法
    public static void SwitchToEnglishInput()
    {
        LoadKeyboardLayout(EnglishInputLocale, 1);
    }
}