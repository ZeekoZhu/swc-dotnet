// Automatically generated by Interoptopus.

#pragma warning disable 0105
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SwcDotNet;
#pragma warning restore 0105

namespace SwcDotNet
{
    public static partial class SwcWrapInterop
    {
        public const string NativeLib = "swc_dotnet";

        static SwcWrapInterop()
        {
        }


        /// Destroys the given instance.
        ///
        /// # Safety
        ///
        /// The passed parameter MUST have been created with the corresponding init function;
        /// passing any other value results in undefined behavior.
        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "swc_wrap_destroy")]
        public static extern FFIError swc_wrap_destroy(ref IntPtr context);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "swc_wrap_new")]
        public static extern FFIError swc_wrap_new(ref IntPtr context);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "swc_wrap_parse")]
        public static extern IntPtr swc_wrap_parse(IntPtr it, ParseParams options);

        [DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "swc_wrap_lookup_char")]
        public static extern FFILoc swc_wrap_lookup_char(IntPtr context, uint byte_pos);

    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct FFILoc
    {
        /// 1-based line number.
        public ulong line;
        /// 0-based column number.
        public ulong col;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct ParseParams
    {
        public string src;
        public string options;
        public OptionStringRef filename;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct StringRef
    {
        public string value;
    }

    public enum FFIError
    {
        Ok = 0,
        Null = 100,
        Panic = 200,
    }

    ///Option type containing boolean flag and maybe valid data.
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct OptionStringRef
    {
        ///Element that is maybe valid.
        StringRef t;
        ///Byte where `1` means element `t` is valid.
        byte is_some;
    }

    public partial struct OptionStringRef
    {
        public static OptionStringRef FromNullable(StringRef? nullable)
        {
            var result = new OptionStringRef();
            if (nullable.HasValue)
            {
                result.is_some = 1;
                result.t = nullable.Value;
            }

            return result;
        }

        public StringRef? ToNullable()
        {
            return this.is_some == 1 ? this.t : (StringRef?)null;
        }
    }



    public partial class SwcWrap : IDisposable
    {
        private IntPtr _context;

        private SwcWrap() {}

        public static SwcWrap New()
        {
            var self = new SwcWrap();
            var rval = SwcWrapInterop.swc_wrap_new(ref self._context);
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
            return self;
        }

        public void Dispose()
        {
            var rval = SwcWrapInterop.swc_wrap_destroy(ref _context);
            if (rval != FFIError.Ok)
            {
                throw new InteropException<FFIError>(rval);
            }
        }

        public string Parse(ParseParams options)
        {
            var s = SwcWrapInterop.swc_wrap_parse(_context, options);
            return Marshal.PtrToStringAnsi(s);
        }

        public FFILoc LookupChar(uint byte_pos)
        {
            return SwcWrapInterop.swc_wrap_lookup_char(_context, byte_pos);
        }

        public IntPtr Context => _context;
    }



    public class InteropException<T> : Exception
    {
        public T Error { get; private set; }

        public InteropException(T error): base($"Something went wrong: {error}")
        {
            Error = error;
        }
    }

}
