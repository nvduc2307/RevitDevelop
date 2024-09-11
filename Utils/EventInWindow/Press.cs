using System.Runtime.InteropServices;

namespace Utils.EventInWindow
{
    public class Press
    {
        public enum KEYBOARD_MSG : uint
        {
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101
        }
        [DllImport("USER32.DLL")]
        public static extern bool PostMessage(
            IntPtr hWnd, uint msg, uint wParam, uint lParam);
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(
            uint uCode, uint uMapType);
        public static void OneKey(IntPtr handle, char letter)
        {
            var scanCode = MapVirtualKey(letter,
                (uint)MVK_MAP_TYPE.VKEY_TO_SCANCODE);
            var keyDownCode = (uint)
                              WH_KEYBOARD_LPARAM.KEYDOWN
                              | (scanCode << 16);
            var keyUpCode = (uint)
                            WH_KEYBOARD_LPARAM.KEYUP
                            | (scanCode << 16);
            PostMessage(handle,
                (uint)KEYBOARD_MSG.WM_KEYDOWN,
                letter, keyDownCode);
            PostMessage(handle,
                (uint)KEYBOARD_MSG.WM_KEYUP,
                letter, keyUpCode);
        }
        public static void Keys(
            IntPtr revitHandle,
            string command)
        {
            foreach (var letter in command) OneKey(revitHandle, letter);
        }
        private enum WH_KEYBOARD_LPARAM : uint
        {
            KEYDOWN = 0x00000001,
            KEYUP = 0xC0000001
        }
        private enum MVK_MAP_TYPE : uint
        {
            VKEY_TO_SCANCODE = 0,
            SCANCODE_TO_VKEY = 1,
            VKEY_TO_CHAR = 2,
            SCANCODE_TO_LR_VKEY = 3
        }
    }
}
